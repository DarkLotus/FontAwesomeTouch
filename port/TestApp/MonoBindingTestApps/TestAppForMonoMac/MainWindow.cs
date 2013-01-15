
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;

using FontAwesome;

namespace TestAppForMonoMac
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}


		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			imgvwSmallGlyph.ImageFrameStyle = NSImageFrameStyle.None;
			imgvwLargeGlyph.ImageFrameStyle = NSImageFrameStyle.None;
			imgvwSmallGlyph.Image = new NSImage (
				FontAwesomeUtil.GetImage (GlyphNames.Font, 20, 20, 12, new CGColor (0f, 1f), new CGColor (0f, 0f))
				, new SizeF (20f, 20f));
			imgvwLargeGlyph.Image = new NSImage (
				FontAwesomeUtil.GetImage (GlyphNames.Font, 20, 20, 18, new CGColor (0f, 1f), new CGColor (0f, 0f))
			    , new SizeF (20f, 20f));
		}

		public void UpdateFontSizeLabel ()
		{
			labelFontSize.IntValue = sldrFontSize.IntValue;
		}

		public NSTableView TableView {
			get { return tblvwGlyphList; }
		}

		public NSSlider SliderFontSize {
			get { return sldrFontSize; }
		}

		public NSTextField LabelStatus {
			get { return labelStatus; }
		}


	}
}

