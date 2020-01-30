// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace GitProfileSwitcher
{
	[Register ("AboutViewController")]
	partial class AboutViewController
	{
		[Outlet]
		GitProfileSwitcher.Controls.HyperlinkTextField GitHubLink { get; set; }

		[Outlet]
		GitProfileSwitcher.Controls.HyperlinkTextField TwitterLink { get; set; }

		[Outlet]
		AppKit.NSTextField VersionLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (GitHubLink != null) {
				GitHubLink.Dispose ();
				GitHubLink = null;
			}

			if (TwitterLink != null) {
				TwitterLink.Dispose ();
				TwitterLink = null;
			}

			if (VersionLabel != null) {
				VersionLabel.Dispose ();
				VersionLabel = null;
			}
		}
	}
}
