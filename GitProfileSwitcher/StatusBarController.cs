using System;
using System.Linq;
using Foundation;
using AppKit;
using GitProfileSwitcher.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitProfileSwitcher
{
    public class StatusBarController : NSObject
    {
        private readonly NSStatusItem _statusItem;
        private readonly NSMenu _profilesMenu = new NSMenu();
        private readonly NSMenuItem _launch;
        private readonly NSMenuItem _useGravatar;
        private bool _isLoginItem = false;

        public Configuration Configuration { get; private set; }
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
            _useGravatar = new NSMenuItem("Use Gravatar", HandleUseGravatarClicked);
            NSMenuItem about = new NSMenuItem("About", HandleAboutClicked);
            NSMenuItem quit = new NSMenuItem("Quit", "q", HandleQuitButtonClicked);

            GetGitProfiles();

            _profilesMenu.AddItem(new NSMenuItem("Loading Profiles..."));
            _profilesMenu.AddItem(NSMenuItem.SeparatorItem);
            _profilesMenu.AddItem(_launch);
            _profilesMenu.AddItem(about);
            _profilesMenu.AddItem(NSMenuItem.SeparatorItem);
            _profilesMenu.AddItem(quit);

            _statusItem.Menu = _profilesMenu;
        }

        private void HandleAddProfileClicked(object sender, EventArgs e)
        {
            // TODO: Add a Profile modal dialog
        }

        private void HandleUseGravatarClicked(object sender, EventArgs e)
        {
            Configuration.UseGravatar = !Configuration.UseGravatar;
            Configuration.Save().GetAwaiter().OnCompleted(() => {
                _useGravatar.State = Configuration.UseGravatar
                    ? NSCellStateValue.On
                    : NSCellStateValue.Off;
            });
        }

        private void HandleLaunchAtLoginClicked(object sender, EventArgs e)
        {
            // Check if the app is setup as a dameon or whatever
            // TODO: Use launchctl to keep the app launched as a dameon (https://stackoverflow.com/a/40952619/1363247)

            _isLoginItem = !_isLoginItem;
            _launch.State = _isLoginItem ? NSCellStateValue.On : NSCellStateValue.Off;
        }

        private void HandleQuitButtonClicked(object sender, EventArgs e)
        {
            AppShouldTerminate = true;
            NSApplication.SharedApplication.Terminate(sender as NSObject);
		}

        private void HandleAboutClicked(object sender, EventArgs e)
        {
            AboutWindow.ShowWindow(sender as NSObject);
        }

        private void GetGitProfiles()
        {
            var loadConfigTask = Configuration.Load();
            loadConfigTask.GetAwaiter().OnCompleted(() => {
                if (loadConfigTask.IsCompletedSuccessfully)
                {
                    Configuration = loadConfigTask.Result;
                    PopulateGitProfiles();
                }
                else if (loadConfigTask.IsFaulted)
                {
                    // TODO: Log this? Let the user know to submit feedback?
                    //var alert = NSAlert.WithMessage("Failed to load Git profiles.", "Retry", "Okay", "Quit", null);
                    // TODO: Show the alert?
                }
            });
        }

        private void PopulateGitProfiles()
        {
            // Remove Loading... menu item
            _profilesMenu.RemoveItemAt(0);

            _profilesMenu.InsertItem(new NSMenuItem("Add a Profile...", HandleAddProfileClicked), 0);

            if (Configuration.Profiles.Count == 0)
            {
                return;
            }

            (Configuration.Profiles as IEnumerable<Profile>).Reverse().ToList().ForEach(profile => {
                _profilesMenu.InsertItem(new NSMenuItem(profile.Name), 0);
            });
        }
    }
}
