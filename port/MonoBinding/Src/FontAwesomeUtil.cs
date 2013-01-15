// 
// FontAwesomeUtil.cs
//  
// Author:
//       Junichi OKADOME (tome@tomesoft.net)
// 
// Copyright 2012-2013 tomesoft.net
//	
//	Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//		
//		http://www.apache.org/licenses/LICENSE-2.0
//		
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#define USE_WEAKREFERENCE
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

#if MONOTOUCH
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
#elif MONOMAC
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
using MonoMac.AppKit;
#endif


namespace FontAwesome
{
#if USE_WEAKREFERENCE
	public class WeakHandle<TObj>
		//where TObj : class
	{
		private TObj _strongReference;
		public System.WeakReference Reference { get; protected set; }
		
		public bool IsAlive { get { return this.Reference.IsAlive; } }
		
		public static implicit operator TObj (WeakHandle<TObj> wkh)
		{
			return (TObj)wkh.Reference.Target;
		}
		
		public TObj Target { get { return (TObj)Reference.Target; } }
		
		public bool HasStrongReference
		{
			get { return typeof (TObj).IsValueType ||
				_strongReference != null; }
		}
		
		public void MakeStrong ()
		{
			if (IsAlive)
				_strongReference = Target;
		}
		
		public void MakeWeak ()
		{
			_strongReference = default (TObj);
		}
		
		public WeakHandle (TObj obj) : this (obj, false)
		{
		}

		public WeakHandle (TObj obj, bool hasStrongInitially)
		{
			Reference = new WeakReference (obj);
			if (hasStrongInitially)
				MakeStrong ();
		}
	}
#endif

	public static class FontAwesomeUtil
	{
		const string TtfFileName = "fontawesome-webfont.ttf";
#if USE_WEAKREFERENCE
		static WeakHandle<CGFont> cachedFont;

		public static void MakeStrongCache ()
		{
			if (cachedFont != null || cachedFont.IsAlive)
				cachedFont.MakeStrong ();
		}
		public static void MakeWeakCache ()
		{
			if (cachedFont != null || cachedFont.IsAlive)
				cachedFont.MakeWeak ();
		}
#else
		internal static CGFont cachedFont;
#endif

		public static string PathToLoadFont { get; set; }

		static bool IsValidObject (object obj)
		{
			var nsobj = obj as NSObject;
			if (nsobj != null)
				return (nsobj.Handle != IntPtr.Zero);
			return (obj != null);
		}

		public static CGFont Font
		{
			get
			{
#if USE_WEAKREFERENCE
				if (cachedFont == null || !cachedFont.IsAlive || !IsValidObject (cachedFont.Target))
					LoadFont ();
				return (cachedFont != null) ? cachedFont.Target : null;
#else
				if (cachedFont == null || !IsValidObject (cachedFont))
					LoadFont ();
				return cachedFont;
#endif
			}
		}

		public static void ClearCache ()
		{
#if USE_WEAKREFERENCE
			if (cachedFont != null && cachedFont.IsAlive && IsValidObject (cachedFont.Target))
			{
				cachedFont.Target.Dispose ();
				cachedFont = null;
			}
#else
			if (cachedFont != null && cachedFont.Handle != IntPtr.Zero)
			{
				cachedFont.Dispose ();
				cachedFont = null;
			}
#endif
		}

		public static CGImage GetImage (string glyphName, int widthUnit, int heightUnit
		                                , float fontSizeUnit
		                                , CGColor textColor
		                                , CGColor backColor
		                                , float? scaleOrNull = null
		                                , PointF? offsetOrNull = null)
		{
			float scale = scaleOrNull ?? ScaleOfMainScreen;
			float fontSize = fontSizeUnit * scale;
			var ctfont = new CTFont (FontAwesomeUtil.Font, fontSize, CGAffineTransform.MakeIdentity ());
			float fontMetric = ctfont.AscentMetric + ctfont.DescentMetric;
			ushort[] glyphs = new ushort[] { ctfont.GetGlyphWithName (glyphName) };
			int width = (int)(widthUnit * scale);
			int height = (int)(heightUnit * scale);

			PointF offset = offsetOrNull ?? new PointF (0f, 0f);
			offset.X *= scale;
			offset.Y *= scale;

			CGImage cgImg = SetUpBitmapContextAndDraw (width, height, (cg, rect) => {

				if (backColor.Alpha > 0f)
				{
					cg.SetFillColor (backColor);
					cg.FillRect (cg.GetClipBoundingBox ());
				}

				var bounds = ctfont.GetBoundingRects (CTFontOrientation.Default, glyphs);
				var x = rect.GetMidX () - bounds.Width / 2f;
				x -= bounds.GetMinX ();
				var y = rect.GetMidY () - fontMetric / 2f;
				y += -ctfont.UnderlinePosition + ctfont.UnderlineThickness;

				x += offset.X;
				y += offset.Y;
				
				// draw bounds
//				cg.SetStrokeColor (1f, 1f);
//				bounds.X = x;
//				bounds.Y = y;
//				cg.StrokeRect (bounds);
				
				cg.SetFillColor (textColor);
				cg.SetTextDrawingMode (CGTextDrawingMode.Fill);
				ctfont.DrawGlyphs (cg, glyphs, new PointF[] { new PointF (x, y) }); 
			});

			return cgImg;
		}

		static CGImage SetUpBitmapContextAndDraw (int width, int height, Action<CGContext, RectangleF> drawer)
		{
			byte[] bytes = new byte[width*height*4];
			using (var cg = new CGBitmapContext (bytes, width, height, 8, width*4
			                                          , CGColorSpace.CreateDeviceRGB ()
			                                          , CGImageAlphaInfo.PremultipliedFirst))
			{
				cg.SetAllowsFontSmoothing (true);
				cg.SetAllowsAntialiasing (true);
				cg.SetAllowsFontSubpixelQuantization (true);
				cg.SetAllowsSubpixelPositioning (true);

				drawer (cg, cg.GetClipBoundingBox ());

				return cg.ToImage ();
			}
		}

		static float? _scaleOfMainScreen;
		static float ScaleOfMainScreen {
			get {
				if (_scaleOfMainScreen == null)
				{
					NSThread.MainThread.InvokeOnMainThread (() => {
#if MONOTOUCH
						_scaleOfMainScreen = UIScreen.MainScreen.Scale;
#elif MONOMAC
						_scaleOfMainScreen = NSScreen.MainScreen.BackingScaleFactor;
#endif
					});
				}
				return _scaleOfMainScreen ?? 1f;
			}
		}

		// Get square sized image
		// using CoreText
		public static CGImage GetImageForBarItem (string glyphName, int sizeUnit=20
		                                          , float? scaleOrNull = null
		                                          , PointF? offsetOrNull = null)
		{
			float scale = scaleOrNull ?? ScaleOfMainScreen;
			float fontSize = sizeUnit * scale * 0.9f; // 0.9f is a magic value for adjust
			var ctfont = new CTFont (FontAwesomeUtil.Font, fontSize, CGAffineTransform.MakeIdentity ());
			float fontMetric = ctfont.AscentMetric + ctfont.DescentMetric;
			ushort[] glyphs = new ushort[] { ctfont.GetGlyphWithName (glyphName) };
			int size = (int)(sizeUnit * scale);
			PointF offset = offsetOrNull ?? new PointF (0f, 0f);
			offset.X *= scale;
			offset.Y *= scale;

			CGImage img = SetUpBitmapContextForBarItemAndDraw (size, size, (cg, rect) => {

				var bounds = ctfont.GetBoundingRects (CTFontOrientation.Default, glyphs);
				var x = rect.GetMidX () - bounds.Width / 2f;
				x -= bounds.GetMinX ();
				var y = rect.GetMidY () - fontMetric / 2f;
				y += -ctfont.UnderlinePosition + ctfont.UnderlineThickness;

				x += offset.X;
				y += offset.Y;

				#region for_debug
//				Console.Write ("Glyph geom {0} : {1}", glyphName, bounds);
//				if (rect.Width < bounds.Width || rect.Height < bounds.Height) {
//					Console.Write (" Glyph geom larger than bounds");
//				}
//				Console.WriteLine ();
//
//				// draw underline
//				cg.SaveState ();
//				cg.SetStrokeColor (1f, 0.4f);
//				cg.SetLineWidth (ctfont.UnderlineThickness);
//				cg.BeginPath ();
//				cg.MoveTo (rect.GetMinX (), rect.GetMidY ()+ctfont.UnderlinePosition);
//				cg.AddLineToPoint (rect.GetMaxX (), rect.GetMidY ()+ctfont.UnderlinePosition);
//				cg.StrokePath ();
//
//				cg.SetLineWidth (1f);
//				cg.SetStrokeColor (1f, 0.5f);
//				cg.BeginPath ();
//				cg.StrokeLineSegments (new PointF[] {
//					new PointF (rect.GetMinX (), rect.GetMidY () - (fontMetric / 2f))
//					, new PointF (rect.GetMaxX (), rect.GetMidY () - (fontMetric / 2f))});
//				cg.StrokeLineSegments (new PointF[] {
//					new PointF (rect.GetMinX (), rect.GetMidY () + (fontMetric / 2f))
//					, new PointF (rect.GetMaxX (), rect.GetMidY () + (fontMetric / 2f))});
//
//				cg.RestoreState ();
//				// draw diagonal lines
//				cg.SetStrokeColor (1f, 0.5f);
//				cg.BeginPath ();
//				cg.MoveTo (rect.GetMinX (), rect.GetMinY ());
//				cg.AddLineToPoint (rect.GetMaxX (), rect.GetMaxY ());
//				cg.StrokePath ();
//				cg.BeginPath ();
//				cg.MoveTo (rect.GetMinX (), rect.GetMaxY ());
//				cg.AddLineToPoint (rect.GetMaxX (), rect.GetMinY ());
//				cg.StrokePath ();
//				// draw bounds
//				cg.StrokeRect (new RectangleF (x + bounds.X, y + bounds.Y, bounds.Width, bounds.Height));

				#endregion

				cg.SetFillColor (1f, 1f);
				cg.SetTextDrawingMode (CGTextDrawingMode.Fill);
				ctfont.DrawGlyphs (cg, glyphs, new PointF[] { new PointF (x, y) }); 
			});

			return img;
		}

		static CGImage SetUpBitmapContextForBarItemAndDraw (int width, int height, Action<CGContext, RectangleF> drawer)
		{
			byte[] bytes = new byte[width * height * 4];
			using (var cg = new CGBitmapContext (bytes, width, height, 8, width*4
			                                          , CGColorSpace.CreateDeviceRGB ()
			                                          , CGImageAlphaInfo.PremultipliedFirst))
			{
				cg.SetAllowsFontSmoothing (true);
				cg.SetAllowsAntialiasing (true);
				cg.SetAllowsFontSubpixelQuantization (true);
				cg.SetAllowsSubpixelPositioning (true);

				drawer (cg, cg.GetClipBoundingBox ());
				
				return cg.ToImage ();
			}
		}

		static bool isLoading;
		static bool isLoaded;
		static void LoadFont ()
		{
			if (isLoading)
				return;
			isLoaded = false;
			isLoading = true;
			try
			{
				string path = PathToLoadFont;
				if (String.IsNullOrEmpty (path))
				{
					path = FindFontFilePath (NSBundle.MainBundle.BundlePath);
					if (String.IsNullOrEmpty (path))
						return;
				}

				cachedFont = null;
				LoadFontFrom (path);
				if (cachedFont != null)
				{
					isLoaded = true;
				}
			}
			finally
			{
				isLoading = false;
			}
		}

		static void LoadFontFrom (string path)
		{
			try
			{
				CGDataProvider provider = new CGDataProvider (path);
				CGFont font = CGFont.CreateFromProvider (provider);

				if (font.NumberOfGlyphs > 0)
				{
#if USE_WEAKREFERENCE
					cachedFont = new WeakHandle<CGFont> (font);
#else
					cachedFont = font;
#endif
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine ("Failed to load font : " + ex);
				throw ex;
			}
		}

		static string FindFontFilePath (string path)
		{
			return FindFilePathRecursively (path, (filePath) => {
				return StringComparer.InvariantCultureIgnoreCase.Compare (
					Path.GetFileName (filePath), TtfFileName) == 0;
			});
		}

		static string FindFilePathRecursively (string path, Predicate<string> pred)
		{
			string foundPath = (from filePath in Directory.EnumerateFiles (path)
								where pred (filePath)
			               		select filePath)
							.FirstOrDefault ();
			if (foundPath != null)
				return Path.Combine (path, foundPath);

			foreach (var dir in Directory.EnumerateDirectories (path))
			{
				string found = FindFilePathRecursively (Path.Combine (path, dir), pred);
				if (found != null)
					return found;
			}

			return null;
		}


#if MONOTOUCH
		public static UIImage GetUIImage (string glyphName, int width, int height
		                                  , float fontSize
		                                  , UIColor textColor
		                                  , UIColor backColor
		                                  , float? scaleOrNull = null
		                                  , PointF? offsetOrNull = null)
		{
			float scale = scaleOrNull ?? ScaleOfMainScreen;
			
			CGImage cgImg = GetImage (glyphName, width, height, fontSize, textColor.CGColor, backColor.CGColor, scale, offsetOrNull);
			return new UIImage (cgImg, scale, UIImageOrientation.Up);
		}

		public static UIImage GetUIImageForBarItem (string glyphName, int sizeUnit=20
		                                            , float? scaleOrNull = null
		                                            , PointF? offsetOrNull = null)
		{
			float scale = scaleOrNull ?? ScaleOfMainScreen;
			
			CGImage cgImg = GetImageForBarItem (glyphName, sizeUnit, scale, offsetOrNull);
			return new UIImage (cgImg, scale, UIImageOrientation.Up);
		}
#endif

#if MONOMAC
//		public static NSImage GetNSImage (string glyphName, int width, int height
//		                                  , float fontSize
//		                                  , NSColor textColor
//		                                  , NSColor backColor
//		                                  , float? scaleOrNull = null
//		                                  , PointF? offsetOrNull = null)
//		{
//			float scale = scaleOrNull ?? ScaleOfMainScreen;
//
//			CGImage cgImg = GetImage (glyphName, width, height, fontSize, textColor.CGColor, backColor.CGColor, scale, offsetOrNull);
//			return new NSImage (cgImg, new SizeF (width, height));
//		}
#endif



	}
}

