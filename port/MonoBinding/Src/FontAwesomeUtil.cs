#define USE_WEAKREFERENCE
using System;
using System.IO;
using System.Linq;
//using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
//using System.Threading.Tasks;
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
			float scale = scaleOrNull ?? ScaleForDevice;
			float fontSize = fontSizeUnit * scale;
			var ctfont = new CTFont (FontAwesomeUtil.Font, fontSize, CGAffineTransform.MakeIdentity ());
			ushort[] glyphs =new ushort[] { ctfont.GetGlyphWithName (glyphName) };
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
				var y = rect.GetMidY () - bounds.Height / 2f;
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

		static float? _scaleForDevice;
		static float ScaleForDevice {
			get {
				if (_scaleForDevice == null)
				{
					NSThread.MainThread.InvokeOnMainThread (() => {
#if MONOTOUCH
						_scaleForDevice = UIScreen.MainScreen.Scale;
#endif
					});
				}
				return _scaleForDevice ?? 1f;
			}
		}

		// Get square sized image
		// using CoreText
		public static CGImage GetImageForBarItem (string glyphName, int sizeUnit=20
		                                          , float? scaleOrNull = null
		                                          , PointF? offsetOrNull = null)
		{
			float scale = scaleOrNull ?? ScaleForDevice;
			float fontSize = sizeUnit * scale;
			var ctfont = new CTFont (FontAwesomeUtil.Font, fontSize, CGAffineTransform.MakeIdentity ());
			ushort[] glyphs =new ushort[] { ctfont.GetGlyphWithName (glyphName) };
			int size = (int)(sizeUnit * scale);
			PointF offset = offsetOrNull ?? new PointF (0f, 0f);
			offset.X *= scale;
			offset.Y *= scale;

			CGImage img = SetUpBitmapContextForBarItemAndDraw (size, size, (cg, rect) => {

				var bounds = ctfont.GetBoundingRects (CTFontOrientation.Default, glyphs);
				var x = rect.GetMidX () - bounds.Width / 2f;
				var y = rect.GetMidY () - bounds.Height / 2f;
				x += offset.X;
				y += offset.Y;

				// draw bounds
//				cg.SetStrokeColor (1f, 1f);
//				bounds.X = x;
//				bounds.Y = y;
//				cg.StrokeRect (bounds);

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
			float scale = scaleOrNull ?? ScaleForDevice;
			
			CGImage cgImg = GetImage (glyphName, width, height, fontSize, textColor.CGColor, backColor.CGColor, scale, offsetOrNull);
			return new UIImage (cgImg, scale, UIImageOrientation.Up);
		}

		public static UIImage GetUIImageForBarItem (string glyphName, int sizeUnit=20
		                                            , float? scaleOrNull = null
		                                            , PointF? offsetOrNull = null)
		{
			float scale = scaleOrNull ?? ScaleForDevice;
			
			CGImage cgImg = GetImageForBarItem (glyphName, sizeUnit, scale, offsetOrNull);
			return new UIImage (cgImg, scale, UIImageOrientation.Up);
		}
#endif
		



	}
}

