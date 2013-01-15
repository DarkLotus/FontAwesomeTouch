using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
using MonoTouch.ImageIO;


using FontAwesome;

namespace TestAppForMonoTouch
{
	public partial class MainViewController : UIViewController
	{
		public MainViewController () : base ("MainViewController", null)
		{
			// Custom initialization
		}

		XTableViewSource _tableViewSource;
		int unitFontSize = 20;
		const int minFontSize = 10;
		const int maxFontSize = 100;

		float CalculateRowHeightFromUnitFontSize (int unitSize)
		{
			return (float)(unitSize + 2);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			actvw.HidesWhenStopped = true;

			// Pinch gesture to scale font
			var pinchGestureRecognizer = new UIPinchGestureRecognizer ((recog) => {
				//Console.WriteLine ("recog.State={0}, scale={1}", recog.State, recog.Scale);
				if (recog.State == UIGestureRecognizerState.Began)
				{
				}
				else if (recog.State == UIGestureRecognizerState.Changed)
				{
					int unitSize = (int)Math.Min (maxFontSize, Math.Max (minFontSize, this.unitFontSize * recog.Scale));
					ShowProgressionLabel (String.Format ("Size = {0}", unitSize));
					_tableViewSource.SetHeightForRow (CalculateRowHeightFromUnitFontSize (unitSize));
					tblvwGlyphList.ReloadData ();
				}
				else if (recog.State == UIGestureRecognizerState.Recognized)
				{
					int unitSize = (int)Math.Min (maxFontSize, Math.Max (minFontSize, this.unitFontSize * recog.Scale));
					if (unitSize != unitFontSize) {
						this.unitFontSize = unitSize;
						UpdateGlyphsWithCurrentFontSize ();
					}
					DismissProgressionLabel ();
				}
			});
			tblvwGlyphList.AddGestureRecognizer (pinchGestureRecognizer);

			#region Toolbar button to save every image to PNG

			var saveButton = new UIBarButtonItem (FontAwesomeUtil.GetUIImageForBarItem (GlyphNames.Save), UIBarButtonItemStyle.Plain
			                     , (s, e) => {

				var alert = new UIAlertView ();
				alert.Title = String.Format ("{0}x{0} size of images are going to be generated.", this.unitFontSize);
				alert.Message = "Clear any image in document folder before newly generate?";
				int nYesButton = alert.AddButton ("Clear");
				int nNoButton = alert.AddButton ("No");
				int nCancelButton = alert.AddButton ("Cancel");
				alert.CancelButtonIndex = nCancelButton;
				alert.Clicked += (s2, e2) => {
					if (nYesButton == e2.ButtonIndex) {
						DeleteAllPNGsInDocumentFolder ();
						SaveEveryImageToPNG ();
					}
					else if (nNoButton == e2.ButtonIndex) {
						SaveEveryImageToPNG ();
					}
				};
				alert.Show ();
			});
			SetToolbarItems (new UIBarButtonItem[] { saveButton }, false);
			#endregion

			UpdateGlyphsWithCurrentFontSize ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.NavigationController.Toolbar.BarStyle = UIBarStyle.Black;
			this.NavigationController.SetNavigationBarHidden (true, animated);
			this.NavigationController.SetToolbarHidden (false, animated);
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
			GC.Collect ();
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		UILabel labelProgression;
		private void ShowProgressionLabel (string msg)
		{
			if (labelProgression == null)
			{
				RectangleF rect = tblvwGlyphList.Frame;
				rect.Y = rect.GetMaxY () - 20f;
				rect.Height = 20f;
				labelProgression = new UILabel (rect) {
					AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin
					, TextAlignment = UITextAlignment.Center
					, TextColor = UIColor.White
					, BackgroundColor = UIColor.FromWhiteAlpha (0f, 0.75f)
				};
			}
			
			if (labelProgression.Superview == null)
			{
				this.View.AddSubview (labelProgression);
				this.View.BringSubviewToFront (labelProgression);
			}
			labelProgression.Text = msg;
		}
		
		private void DismissProgressionLabel ()
		{
			if (labelProgression != null && labelProgression.Superview != null)
			{
				labelProgression.RemoveFromSuperview ();
			}
		}
		
		private void DeleteAllPNGsInDocumentFolder ()
		{
			string documentDirPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			foreach (var file in Directory.EnumerateFiles (documentDirPath, "*.png"))
			{
				File.Delete (file);
			}
		}

		private void SaveEveryImageToPNG ()
		{
			float scale = UIScreen.MainScreen.Scale;
			var glyphInfos = _tableViewSource.GlyphInfos;
			actvw.StartAnimating ();
			Task.Factory.StartNew (() => {
				Func<string, string> makeName = (rawName) => {
					return String.Format ("{0}_{1}{2}.png", rawName, (int)this.unitFontSize
					                      , ((scale > 2f) ? "@4x" : (scale > 1f) ? "@2x" : ""));
				};
				string documentDirPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
				
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
					if (File.Exists (url.Path)) {
						//Console.WriteLine ("Succeeded to write {0}", url);
						++ nSucceeded;
					}
					else {
						Console.WriteLine ("Failed to write {0}", url);
						++ nFailed;
					}
					BeginInvokeOnMainThread (() => {
						ShowProgressionLabel (String.Format ("{0} of {1} processed", (nSucceeded + nFailed), glyphInfos.Count));
					});
				});
				
				InvokeOnMainThread (() => {
					actvw.StopAnimating ();
					DismissProgressionLabel ();
					var alert = new UIAlertView ("Result of save"
					                             , String.Format ("{0} succeeded, {1} failed", nSucceeded, nFailed)
					                             , null, "Close", null);
					alert.Show ();
				});
			});
		}

		private void UpdateGlyphsWithCurrentFontSize ()
		{
			actvw.StartAnimating ();
			LoadFontAndGatherGlyphInfos (this.unitFontSize, (glyphInfos) => {
				InvokeOnMainThread (() => {
					actvw.StopAnimating ();
					if (glyphInfos != null)
					{
						_tableViewSource =  new XTableViewSource (glyphInfos);
						_tableViewSource.SetHeightForRow (CalculateRowHeightFromUnitFontSize (this.unitFontSize));
						tblvwGlyphList.Source = _tableViewSource;
						tblvwGlyphList.ReloadData ();
					}
				});
			});
		}
		
		void LoadFontAndGatherGlyphInfos (int unitSize, Action<List<GlyphInfo>> endHandler)
		{
			float scale = UIScreen.MainScreen.Scale;
			Task.Factory.StartNew (() => {
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


		internal class XTableViewSource : UITableViewSource
		{
			List<GlyphInfo> glyphInfos;

			public List<GlyphInfo> GlyphInfos { get { return glyphInfos; } }

			internal XTableViewSource (List<GlyphInfo> glyphInfos) : base ()
			{
				this.glyphInfos = glyphInfos;
			}

			float heightForRow = 22f;
			public void SetHeightForRow (float height)
			{
				heightForRow = height;
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return heightForRow;
			}

			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return glyphInfos.Count;
			}

			public override string TitleForHeader (UITableView tableView, int section)
			{
				int nValid = 0;
				int nNG = 0;
				glyphInfos.ForEach ((gi) => {
					if (gi.GlyphId != 0)
						++ nValid;
					else
						++ nNG;
				});

				return string.Format ("{0} glyphs ({1} valid, {2} NG)", glyphInfos.Count, nValid, nNG);
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				int index = indexPath.Row;
				string identifier = String.Format ("cell_{0}", index);

				UITableViewCell cell = tableView.DequeueReusableCell ((NSString)identifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Value1, identifier);
				}
				var gi = glyphInfos[index];
				float scale = UIScreen.MainScreen.Scale;
				cell.ImageView.BackgroundColor = UIColor.DarkGray;
				cell.ImageView.Image = new UIImage (gi.GlyphImage, scale, UIImageOrientation.Up);
				cell.TextLabel.Text = gi.GlyphName;
				if (gi.GlyphId == 0) // to make invalid glyph red
					cell.ContentView.BackgroundColor = UIColor.Red;
				cell.DetailTextLabel.AdjustsFontSizeToFitWidth = true;
				cell.DetailTextLabel.Text = String.Format ("\"{0}\" 0x{1:x}", gi.RawName, gi.GlyphId);

				return cell;
			}
		}
	}
}

