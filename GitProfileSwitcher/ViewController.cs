using System;
using AppKit;
using Foundation;

namespace GitProfileSwitcher
{
    public partial class ViewController : NSViewController
    {
        public static event EventHandler QuitButtonClicked;
        public static event EventHandler AboutMenuItemClicked;
        NSTrackingArea hoverarea;
        NSCursor cursor;
        NSMenu settingsMenu;
        NSMenuItem launch;
        bool isLoginItem = false;

        // Adjust the character spacing of Title Text
        readonly NSAttributedString titleString = new NSAttributedString("Git Profile",
            new NSStringAttributes() {
                ParagraphStyle = new NSMutableParagraphStyle() {
                    LineHeightMultiple = 0.75f
                }
            });

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view
            titleText.AttributedStringValue = titleString;
            hoverarea = new NSTrackingArea(SettingsButton.Bounds, NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveAlways, this, null);
            SettingsButton.AddTrackingArea(hoverarea);

            // Contex Menu for settings
            settingsMenu = new NSMenu();

            launch = new NSMenuItem("Launch at Login", new ObjCRuntime.Selector("launch:"), "");
            NSMenuItem about = new NSMenuItem("About", new ObjCRuntime.Selector("about:"), "");
            NSMenuItem quit = new NSMenuItem("Quit", new ObjCRuntime.Selector("quit:"), "q");

            settingsMenu.AddItem(launch);
            settingsMenu.AddItem(about);
            settingsMenu.AddItem(NSMenuItem.SeparatorItem);
            settingsMenu.AddItem(quit);

            cursor = NSCursor.CurrentSystemCursor;
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        partial void SettingsButtonClick(NSObject sender)
        {
            var current = NSApplication.SharedApplication.CurrentEvent;

            // Check if the app is setup as a dameon or whatever
            // TODO: Use launchctl to keep the app launched as a dameon (https://stackoverflow.com/a/40952619/1363247)

            //if (!isLoginItem)
            //{
            //    launch.State = NSCellStateValue.Off;
            //}
            //else if (isLoginItem)
            //{
            //    launch.State = NSCellStateValue.On;
            //}

            NSMenu.PopUpContextMenu(settingsMenu, current, sender as NSView);
        }

        [Export("launch:")]
        void Launch(NSObject sender)
        {
            isLoginItem = !isLoginItem;
            launch.State = isLoginItem ? NSCellStateValue.On : NSCellStateValue.Off;
        }

        // Delegate About Menu to StatusBarController.cs
        [Export("about:")]
        void About(NSObject sender) => AboutMenuItemClicked?.Invoke(this, null);

        // Delegate Quit Menu to StatusBarController.cs
        [Export("quit:")]
        void Quit(NSObject sender) => QuitButtonClicked?.Invoke(this, null);

        public override void MouseEntered(NSEvent e)
        {
            base.MouseEntered(e);

            cursor = NSCursor.PointingHandCursor;
            cursor.Push();
        }

        public override void MouseExited(NSEvent e)
        {
            base.MouseEntered(e);
            cursor.Pop();
        }
    }
}
