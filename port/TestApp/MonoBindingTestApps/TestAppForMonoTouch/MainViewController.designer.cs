// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace TestAppForMonoTouch
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblvwGlyphList { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView actvw { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblvwGlyphList != null) {
				tblvwGlyphList.Dispose ();
				tblvwGlyphList = null;
			}

			if (actvw != null) {
				actvw.Dispose ();
				actvw = null;
			}
		}
	}
}
