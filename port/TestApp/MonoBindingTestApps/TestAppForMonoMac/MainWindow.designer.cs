// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace TestAppForMonoMac
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		MonoMac.AppKit.NSScrollView scrollvwList { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tblvwGlyphList { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSlider sldrFontSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField labelFontSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imgvwSmallGlyph { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imgvwLargeGlyph { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField labelStatus { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (scrollvwList != null) {
				scrollvwList.Dispose ();
				scrollvwList = null;
			}

			if (tblvwGlyphList != null) {
				tblvwGlyphList.Dispose ();
				tblvwGlyphList = null;
			}

			if (sldrFontSize != null) {
				sldrFontSize.Dispose ();
				sldrFontSize = null;
			}

			if (labelFontSize != null) {
				labelFontSize.Dispose ();
				labelFontSize = null;
			}

			if (imgvwSmallGlyph != null) {
				imgvwSmallGlyph.Dispose ();
				imgvwSmallGlyph = null;
			}

			if (imgvwLargeGlyph != null) {
				imgvwLargeGlyph.Dispose ();
				imgvwLargeGlyph = null;
			}

			if (labelStatus != null) {
				labelStatus.Dispose ();
				labelStatus = null;
			}
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
