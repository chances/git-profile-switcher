using System;
using Foundation;
using AppKit;

namespace GitProfileSwitcher
{
    public partial class AboutViewController : NSViewController
    {

        #region Constructors

        // Called when created from unmanaged code
        public AboutViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public AboutViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public AboutViewController() : base("AboutView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            GitHubLink.Href = "https://github.com/chances/git-profile-switcher";
            TwitterLink.Href = "https://twitter.com/ChancesOfSnow";

            var bundleVersion = NSBundle.MainBundle
                .InfoDictionary["CFBundleShortVersionString"].ToString();
            VersionLabel.StringValue = VersionLabel.StringValue.Replace("{version}", bundleVersion);
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.ActivationPolicy =
                NSApplicationActivationPolicy.Regular;
        }

        public override void ViewWillDisappear()
        {
            base.ViewWillDisappear();

            NSApplication.SharedApplication.ActivationPolicy =
                NSApplicationActivationPolicy.Accessory;
        }
    }
}
