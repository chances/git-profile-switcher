using AppKit;
using Foundation;

namespace GitProfileSwitcher
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        private readonly StatusBarController _statusBar = new StatusBarController();

        public AppDelegate()
        {
        }

        public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            if (_statusBar.AppShouldTerminate)
            {
                _statusBar.Configuration.Save().GetAwaiter().OnCompleted(() => {
                    NSApplication.SharedApplication.ReplyToApplicationShouldTerminate(true);
                });
                return NSApplicationTerminateReply.Later;
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
            // Insert code here to setup your application
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

    }
}
