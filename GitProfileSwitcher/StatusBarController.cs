using Foundation;
using AppKit;

namespace GitProfileSwitcher
{
    public class StatusBarController : NSObject
    {
        private readonly NSStatusItem _statusItem;
        private readonly NSMenu _profilesMenu = new NSMenu();
        private readonly NSMenuItem _launch;
        private bool _isLoginItem = false;

        public NSWindowController AboutWindow { get; private set; }
        public bool AppShouldTerminate { get; private set; } = false;

        public StatusBarController()
        {
            _statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Variable);
            _statusItem.HighlightMode = true;

			var storyboard = NSStoryboard.FromName("Main", null);
			AboutWindow = storyboard.InstantiateControllerWithIdentifier("AboutWindow") as NSWindowController;

            // Init the profiles menu

            var button = _statusItem.Button;
            button.Image = new NSImage("StatusBarIcon.png") {
                Template = true
            };
            // TODO: Add an alternate image asset
            button.AlternateImage = new NSImage("StatusBarIcon.png") {
                Template = true
            };
            button.Target = this;

            _launch = new NSMenuItem("Launch at Login", HandleLaunchAtLoginClicked);
            NSMenuItem about = new NSMenuItem("About", HandleAboutMenuItemClicked);
            NSMenuItem quit = new NSMenuItem("Quit", "q", HandleQuitButtonClicked);

            _profilesMenu.AddItem(_launch);
            _profilesMenu.AddItem(about);
            _profilesMenu.AddItem(NSMenuItem.SeparatorItem);
            _profilesMenu.AddItem(quit);

            _statusItem.Menu = _profilesMenu;
        }

        void HandleLaunchAtLoginClicked(object sender, System.EventArgs e)
        {
            // Check if the app is setup as a dameon or whatever
            // TODO: Use launchctl to keep the app launched as a dameon (https://stackoverflow.com/a/40952619/1363247)

            _isLoginItem = !_isLoginItem;
            _launch.State = _isLoginItem ? NSCellStateValue.On : NSCellStateValue.Off;
        }

        void HandleQuitButtonClicked(object sender, System.EventArgs e)
        {
            AppShouldTerminate = true;
            NSApplication.SharedApplication.Terminate(sender as NSObject);
		}

        void HandleAboutMenuItemClicked(object sender, System.EventArgs e)
        {
            AboutWindow.ShowWindow(sender as NSObject);
        }
    }
}
