﻿using AppKit;
using Foundation;

namespace GitProfileSwitcher
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        NSPopover popOver = new NSPopover();

        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Initialize menu bar popup
            var storyboard = NSStoryboard.FromName("Main", null);
            var controller = storyboard.InstantiateControllerWithIdentifier("PopupController") as ViewController;

            popOver.ContentViewController = controller;

            StatusBarController statusBar = new StatusBarController();
            statusBar.InitStatusBarItem("StatusBarIcon.png", popOver);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
