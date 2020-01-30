using AppKit;
using Foundation;

namespace GitProfileSwitcher
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        private readonly StatusBarController _statusBar = new StatusBarController();
        private readonly NSPopover _popOver = new NSPopover();

        public AppDelegate()
        {
        }

        public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            if (_statusBar.AppShouldTerminate)
            {
                return NSApplicationTerminateReply.Now;
            }

            _statusBar.AboutWindow?.Close();
            NSApplication.SharedApplication.KeyWindow?.Close();
            NSApplication.SharedApplication.MainWindow?.Close();
            return NSApplicationTerminateReply.Cancel;
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return false;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Initialize menu bar popup
            var storyboard = NSStoryboard.FromName("Main", null);
            var controller = storyboard.InstantiateControllerWithIdentifier("PopupController") as ViewController;

            _popOver.ContentViewController = controller;
            _statusBar.InitStatusBarItem("StatusBarIcon.png", _popOver);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

    }
}
