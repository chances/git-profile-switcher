using System;
using AppKit;
using Foundation;

namespace GitProfileSwitcher
{
    public partial class ViewController : NSViewController
    {
        #region Data Members
        public static event EventHandler QuitButtonClicked;
        public static event EventHandler AboutMenuItemClicked;
        NSTrackingArea hoverarea;
        NSCursor cursor;
        NSMenu settingsMenu;
        NSMenuItem launch;
        bool isLoginItem;

        //This is just to adjust the character spacing of Title Text and is not necessary at all
        NSAttributedString titleString = new NSAttributedString("Make\nEpic\nThings",
                                                               new NSStringAttributes() {
                                                                   ParagraphStyle = new NSMutableParagraphStyle() {
                                                                       LineHeightMultiple = 0.75f
                                                                   }
                                                               });
        #endregion

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
            // Check if the app is in the login items or not
            var script = "tell application \"System Events\"\n get the name of every login item\n if login item \"GitProfileSwitcher\" exists then\n return true\n else\n return false\n end if\n end tell";
            NSAppleScript appleScript = new NSAppleScript(script);
            var errors = new NSDictionary();
            NSAppleEventDescriptor result = appleScript.ExecuteAndReturnError(out errors);
            isLoginItem = result.BooleanValue;

            if (!isLoginItem)
            {
                launch.State = NSCellStateValue.Off;
            }
            else if (isLoginItem)
            {
                launch.State = NSCellStateValue.On;
            }

            NSMenu.PopUpContextMenu(settingsMenu, current, sender as NSView);
        }

        [Export("launch:")]
        void Launch(NSObject sender)
        {
            // Use AppleScript to add this app to Login item list of macOS.
            // The app must be in the Applications Folder
            string script;
            NSAppleScript login;
            NSDictionary errors = new NSDictionary();
            if (!isLoginItem)
            {
                // AppleScript to add app to login items
                script = "tell application \"System Events\"\n make new login item at end of login items with properties {name: \"GitProfileSwitcher\", path:\"/Applications/GitProfileSwitcher.app\", hidden:false}\n end tell";
                login = new NSAppleScript(script);
                login.ExecuteAndReturnError(out errors);
            }
            else
            {
                // AppleScript to delete app from login items
                script = "tell application \"System Events\"\n delete login item \"GitProfileSwitcher\"\n end tell";
                login = new NSAppleScript(script);
                login.ExecuteAndReturnError(out errors);
            }
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
