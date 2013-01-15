// 
// MainWindowController.cs
//  
// Author:
//       Junichi OKADOME (tome@tomesoft.net)
// 
// Copyright 2013 tomesoft.net
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
using MonoMac.ObjCRuntime;
using MonoMac.ImageIO;

using FontAwesome;

namespace TestAppForMonoMac
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		int unitFontSize = 20;
		const int minFontSize = 10;
		const int maxFontSize = 100;

		float CalculateRowHeightFromUnitFontSize (int unitSize)
		{
			return (float)(unitSize + 2f);
		}
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion

		XTableViewSource _tableViewSource;

		internal static readonly NSString _idImageColumn = new NSString ("imageColumn");
		internal static readonly NSString _idNameColumn = new NSString ("nameColumn");
		internal static readonly NSString _idDetailColumn = new NSString ("detailColumn");

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			var columns = Window.TableView.TableColumns ();
			foreach (var col in columns) {
				Window.TableView.RemoveColumn (col);
			}

			var columnForGlyph = new NSTableColumn () {
				Editable = false
				, MinWidth = 20f
				, MaxWidth = 20f
				, Identifier = _idImageColumn
				, ResizingMask = NSTableColumnResizing.None
			};
			var columnForName = new NSTableColumn () {
				Editable = false
				, Width = 150f
				, Identifier = _idNameColumn
				, ResizingMask = NSTableColumnResizing.UserResizingMask
			};
			var columnForDetail = new NSTableColumn () {
				Editable = false
				, Width = 200f
				, Identifier = _idDetailColumn
				, ResizingMask = NSTableColumnResizing.UserResizingMask
			};
			columnForGlyph.HeaderCell.Title = "Glyph";
			columnForName.HeaderCell.Title = "Name";
			columnForDetail.HeaderCell.Title = "";

			Window.TableView.AddColumn (columnForGlyph);
			Window.TableView.AddColumn (columnForName);
			Window.TableView.AddColumn (columnForDetail);


			Window.SliderFontSize.MinValue = (double)minFontSize;
			Window.SliderFontSize.MaxValue = (double)maxFontSize;
			Window.SliderFontSize.TickMarksCount = (maxFontSize - minFontSize) / 10 + 1;
			Window.SliderFontSize.IntValue = unitFontSize;
			Window.SliderFontSize.Activated += (object sender, EventArgs e) => {
//				Console.WriteLine ("activated {0}", Window.SliderFontSize.IntValue);
				this.unitFontSize = Window.SliderFontSize.IntValue;
				UpdateGlyphsWithCurrentFontSize ();
				Window.UpdateFontSizeLabel ();
			};


			UpdateGlyphsWithCurrentFontSize ();
			Window.UpdateFontSizeLabel ();
		}



		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		private void UpdateGlyphsWithCurrentFontSize ()
		{
			//actvw.StartAnimating ();
			LoadFontAndGatherGlyphInfos (this.unitFontSize, (glyphInfos) => {
				InvokeOnMainThread (() => {
					//actvw.StopAnimating ();
					if (glyphInfos != null)
					{
						int nValid = 0;
						int nNG = 0;
						glyphInfos.ForEach ((gi) => {
							if (gi.GlyphId != 0)
								++ nValid;
							else
								++ nNG;
						});
						
						Window.LabelStatus.StringValue = String.Format ("{0} glyphs, {1} valid, {2} NG", glyphInfos.Count, nValid, nNG);

						_tableViewSource =  new XTableViewSource (glyphInfos);
						_tableViewSource.SetRowHeight (CalculateRowHeightFromUnitFontSize (this.unitFontSize));
						NSTableColumn[] columns = Window.TableView.TableColumns ();
						var imgcol = columns.SingleOrDefault ((col) => NSObject.Equals (col.Identifier, _idImageColumn));
						imgcol.MinWidth = imgcol.MaxWidth = (float)this.unitFontSize;

						Window.TableView.Source = _tableViewSource;
						Window.TableView.ReloadData ();
					}
				});
			});
		}
		
		void LoadFontAndGatherGlyphInfos (int unitSize, Action<List<GlyphInfo>> endHandler)
		{
			float scale = Window.BackingScaleFactor;
			Task.Factory.StartNew (() => {
				try {
					Type typGlyphNames = typeof (FontAwesome.GlyphNames);
					var memberinfos = typGlyphNames.GetMembers (BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
					
					var ctFont = new CTFont (FontAwesomeUtil.Font, 20f, CGAffineTransform.MakeIdentity ());
					
					var list = memberinfos.Select ((m) => {
						var fieldInfo = typGlyphNames.GetField (m.Name);
						var rawName = (string)fieldInfo.GetValue (typGlyphNames);
						var glyphval = ctFont.GetGlyphWithName (rawName);
						return new GlyphInfo {
							GlyphName = m.Name
								, RawName = rawName
									, GlyphImage = FontAwesomeUtil.GetImageForBarItem (rawName, unitSize, scale)
									, GlyphId = glyphval
						};
					}).ToList ();
					
					if (endHandler != null)
						endHandler (list);
				}
				catch (Exception ex) {
					Console.WriteLine ("Exception: " + ex);
				}
			});
		}

		public void ExportEveryGlyphsAsPNGTo (string path)
		{
			float scale = Window.BackingScaleFactor;
			var glyphInfos = _tableViewSource.GlyphInfos;
			//actvw.StartAnimating ();
			Task.Factory.StartNew (() => {
				Func<string, string> makeName = (rawName) => {
					return String.Format ("{0}_{1}{2}.png", rawName, (int)this.unitFontSize
					                      , ((scale > 2f) ? "@4x" : (scale > 1f) ? "@2x" : ""));
				};
				//string documentDirPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
				string documentDirPath = path;
				
				NSDictionary prop = new NSDictionary (); //
				int nSucceeded = 0;
				int nFailed = 0;
				glyphInfos.ForEach ((gi) => {
					var url = new NSUrl (System.IO.Path.Combine (documentDirPath, makeName (gi.RawName)), false);
					try {
						CGImageDestination dest = CGImageDestination.FromUrl (url, "public.png", 1);
						dest.AddImage (gi.GlyphImage, prop);
						dest.Close ();
					}
					catch (Exception ex) {
						Console.WriteLine (ex.ToString ());
					}
					if (System.IO.File.Exists (url.Path)) {
						//Console.WriteLine ("Succeeded to write {0}", url);
						++ nSucceeded;
					}
					else {
						Console.WriteLine ("Failed to write {0}", url);
						++ nFailed;
					}
					BeginInvokeOnMainThread (() => {
						//ShowProgressionLabel (String.Format ("{0} of {1} processed", (nSucceeded + nFailed), glyphInfos.Count));
					});
				});
				
				InvokeOnMainThread (() => {
					//actvw.StopAnimating ();
					//DismissProgressionLabel ();
					var alert = new NSAlert ();
					alert.MessageText = String.Format ("{0} succeeded, {1} failed", nSucceeded, nFailed);
					alert.RunModal ();
				});
			});

		}

		public void DoExportGlyphsAsPNG ()
		{
			var panel = NSOpenPanel.OpenPanel;
			panel.CanChooseDirectories = true;
			panel.CanChooseFiles = false;
			panel.CanCreateDirectories = true;
			panel.AllowsMultipleSelection = false;
			panel.Title = "Export Glyphs as PNGs";
			panel.Begin ((result) => {
				//Console.WriteLine ("result = {0} DirectoryUrl = {1}", result, panel.DirectoryUrl);
				if (result == 1) {
					ExportEveryGlyphsAsPNGTo (panel.DirectoryUrl.Path);
				}
			});
		}

		public class GlyphInfo
		{
			public string GlyphName { get; set; }
			public string RawName { get; set; }
			public int GlyphId { get; set; }
			public CGImage GlyphImage { get; set; }
			
			public GlyphInfo () {}
		}

		internal class XTableViewSource : NSTableViewSource
		{
			List<GlyphInfo> glyphInfos;

			public List<GlyphInfo> GlyphInfos { get { return glyphInfos; } }

			internal XTableViewSource (List<GlyphInfo> glyphInfos) : base ()
			{
				this.glyphInfos = glyphInfos;
			}
			
			public override int GetRowCount (NSTableView tableView)
			{
				return glyphInfos.Count;
			}

			float rowHeight = 22f;
			public void SetRowHeight (float height)
			{
				rowHeight = height;
			}
			
			public override float GetRowHeight (NSTableView tableView, int row)
			{
				return rowHeight;
			}

			public override NSTableRowView GetRowView (NSTableView tableView, int row)
			{
				var rowView = new XTableRowView ();
				rowView.CustomBackgroundColor = (row % 2 == 0) ? NSColor.FromDeviceWhite (0.8f, 1f) : NSColor.LightGray;
				return rowView;
			}

			public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				if (NSObject.Equals (tableColumn.Identifier, MainWindowController._idImageColumn))
				{
					NSImageView imageView = tableView.MakeView ("GlyphView", this) as NSImageView;
					if (imageView == null) {
						imageView = new XImageView ();
						SetIdentifierToView (imageView, "GlyphView");
					}
					var img = new NSImage (glyphInfos[row].GlyphImage, new SizeF (rowHeight-2f, rowHeight-2f));
					imageView.Image = img;
					return imageView;
				}
				else if (NSObject.Equals (tableColumn.Identifier, MainWindowController._idNameColumn))
				{
					NSTextField textfield = tableView.MakeView ("NameView", this) as NSTextField;
					if (textfield == null) {
						textfield = new NSTextField ();
						//textfield.BackgroundColor = NSColor.DarkGray;
						textfield.BackgroundColor = NSColor.FromDeviceWhite (0f, 0.5f);
						textfield.Bordered = false;
						SetIdentifierToView (textfield, "NameView");
						//textfield.Alignment = NSTextAlignment.Center;
						textfield.Cell = new XTextFieldCell (textfield);
					}

					textfield.StringValue = glyphInfos[row].GlyphName;
					
					return textfield;
				}
				else if (NSObject.Equals (tableColumn.Identifier, MainWindowController._idDetailColumn))
				{
					
					NSTextField textfield = tableView.MakeView ("DetailView", this) as NSTextField;
					if (textfield == null) {
						textfield = new NSTextField ();
						//textfield.BackgroundColor = NSColor.DarkGray;
						textfield.BackgroundColor = NSColor.FromDeviceWhite (0f, 0.5f);
						textfield.Bordered = false;
						SetIdentifierToView (textfield, "DetailView");

						textfield.Cell = new XTextFieldCell (textfield);
					}

					textfield.StringValue = String.Format ("\"{0}\" 0x{1:x}", glyphInfos[row].RawName, glyphInfos[row].GlyphId);
					
					return textfield;
				}
				return null;
			}
		}

		public class XImageView : NSImageView
		{
			public XImageView () : base () {}
			public XImageView (RectangleF frame) : base (frame) {}
			public XImageView (IntPtr handle) : base (handle) {}
			public XImageView (NSCoder coder) : base (coder) {}
			public XImageView (NSObjectFlag t) : base (t) {}

			public override void DrawRect (RectangleF dirtyRect)
			{
				NSGraphicsContext nsgctxt = NSGraphicsContext.CurrentContext;
				nsgctxt.SaveGraphicsState ();
				try {
					var cg = nsgctxt.GraphicsPort;
					cg.SetFillColor (new CGColor (0.25f, 1f));
					cg.FillRect (dirtyRect);
				}
				finally {
					nsgctxt.RestoreGraphicsState ();
				}
				base.DrawRect (dirtyRect);
			}
		}

		public class XTableRowView : NSTableRowView
		{
			public XTableRowView () : base () {}
			public XTableRowView (IntPtr handle) : base (handle) {}
			public XTableRowView (NSCoder coder) : base (coder) {}
			public XTableRowView (NSObjectFlag t) : base (t) {}

			public NSColor CustomBackgroundColor { get; set; }

			public override void DrawBackgrounn (RectangleF dirtyRect)
			{
				base.DrawBackgrounn (dirtyRect);
				NSGraphicsContext nsgctxt = NSGraphicsContext.CurrentContext;
				nsgctxt.SaveGraphicsState ();
				try {
					var cg = nsgctxt.GraphicsPort;
					float[] comps;
					this.CustomBackgroundColor.GetComponents (out comps);
					cg.SetFillColor (comps);
					cg.FillRect (dirtyRect);
				}
				finally {
					nsgctxt.RestoreGraphicsState ();
				}
				//base.DrawRect (dirtyRect);
			}
		}

		public class XTextFieldCell : NSTextFieldCell
		{
			public XTextFieldCell () : base () {}
			public XTextFieldCell (string aString) : base (aString) {}
			public XTextFieldCell (NSImage image) : base (image) {}
			public XTextFieldCell (NSCoder coder) : base (coder) {}
			public XTextFieldCell (IntPtr handle) : base (handle) {}
			public XTextFieldCell (NSObjectFlag t) : base (t) {}
			public XTextFieldCell (NSTextField owner) : base () { this.Owner = owner; }

			public NSTextField Owner { get; set; }

			public override RectangleF DrawingRectForBounds (RectangleF theRect)
			{
				var rect = base.DrawingRectForBounds (theRect);
				rect.Height = 40f;
				var size = this.CellSizeForBounds (rect);
				rect.Y = Owner.Bounds.GetMidY () - (size.Height / 2f);
				return rect;

			}
		}




		#region MonoMac treating

		static IntPtr selSetIdentifier_ = Selector.GetHandle ("setIdentifier:");

		[Export ("setIdentifier:")]
		public static void SetIdentifierToView (NSView view, string identifier)
		{
			IntPtr value = (identifier != null) ? ((NSString)identifier).Handle : IntPtr.Zero;
//			if (this.IsDirectBinding)
//			{
				Messaging.void_objc_msgSend_IntPtr (view.Handle, selSetIdentifier_, value);
//			}
//			else
//			{
//				Messaging.void_objc_msgSendSuper_IntPtr (view.SuperHandle, NSView.selSetFrame_, value);
//			}
		}
#endregion

	}
}

