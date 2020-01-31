using System;
using System.Linq;
using Foundation;
using AppKit;
using GitProfileSwitcher.Models;
using System.Collections.Generic;
using System.Text;

namespace GitProfileSwitcher
{
    public class StatusBarController : NSObject
    {
        private const int MaxSetProfileRetries = 3;
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
            button.Image = NSImage.ImageNamed("StatusBarIcon");
            button.AlternateImage = NSImage.ImageNamed("StatusBarIconAlternate");
            button.Target = this;

            _launch = new NSMenuItem("Launch at Login", HandleLaunchAtLoginClicked);
            _useGravatar = new NSMenuItem("Use Gravatar", HandleUseGravatarClicked);
            NSMenuItem about = new NSMenuItem("About", HandleAboutClicked);
            NSMenuItem quit = new NSMenuItem("Quit Git Profile Switcher", "q", HandleQuitButtonClicked);

            _profilesMenu.AddItem(new NSMenuItem("Loading Profiles..."));
            _profilesMenu.AddItem(NSMenuItem.SeparatorItem);
            _profilesMenu.AddItem(new NSMenuItem("Add a Profile...", HandleAddProfileClicked));
            _profilesMenu.AddItem(new NSMenuItem("Edit Profiles...", ",", HandleEditProfileClicked));
            _profilesMenu.AddItem(NSMenuItem.SeparatorItem);
            _profilesMenu.AddItem(_launch);
            _profilesMenu.AddItem(_useGravatar);
            _profilesMenu.AddItem(about);
            _profilesMenu.AddItem(NSMenuItem.SeparatorItem);
            _profilesMenu.AddItem(quit);

            _statusItem.Menu = _profilesMenu;

            GetGitProfiles();
        }

        private int _setProfileRetryCount = 0;
        private void HandleProfileClicked(object sender, EventArgs e)
        {
            if (sender is NSMenuItem menuItem)
            {
                int currentProfileIndex = (int) menuItem.Tag;
                var profile = Configuration.Profiles.ElementAtOrDefault(currentProfileIndex);
                if (profile != null)
                {
                    for (int i = 0; i < Configuration.Profiles.Count; i++)
                    {
                        _profilesMenu.ItemAt(i).State = NSCellStateValue.Off;
                    }

                    var setGitConfigTask = profile.SetGlobally();
                    setGitConfigTask.GetAwaiter().OnCompleted(() => {
                        var succeeded = setGitConfigTask.IsCompletedSuccessfully &&
                            setGitConfigTask.Result;
                        if (!succeeded)
                        {
                            var alert = new NSAlert()
                            {
                                MessageText = "Failed to set Git user configuration.",
                                AlertStyle = NSAlertStyle.Critical
                            };
                            alert.Window.Title = "Git Profile Switcher";
                            bool userMayRetry = _setProfileRetryCount < MaxSetProfileRetries;
                            alert.AddButton(userMayRetry ? "Retry" : "Report Issue");
                            alert.AddButton("Ignore");

                            // TODO: Add an accessory view for log details/submit feedback when !userMayRetry

                            var response = alert.RunModal();
                            if (userMayRetry && response == (int) NSAlertButtonReturn.First)
                            {
                                // Retry up to MaxSetProfileRetries times
                                HandleProfileClicked(sender, e);
                            }
                            else if (response == (int) NSAlertButtonReturn.First)
                            {
                                // TODO: Report an Issue
                            }
                        }
                        else if (succeeded)
                        {
                            Configuration.CurrentProfileIndex = currentProfileIndex;
                            menuItem.State = NSCellStateValue.On;
                            _setProfileRetryCount = 0;
                        }
                    });
                }
            }
        }

        private void HandleAddProfileClicked(object sender, EventArgs e)
        {
            // TODO: Add a Profile modal dialog
        }

        private void HandleEditProfileClicked(object sender, EventArgs e)
        {
            // TODO: Open a Profiles editor
        }

        private void HandleUseGravatarClicked(object sender, EventArgs e)
        {
            Configuration.UseGravatar = !Configuration.UseGravatar;
            Configuration.Save().GetAwaiter().OnCompleted(() => {
                _useGravatar.State = Configuration.UseGravatar
                    ? NSCellStateValue.On
                    : NSCellStateValue.Off;

                for (int i = 0; i < Configuration.Profiles.Count; i++)
                {
                    _profilesMenu.ItemAt(i).Image = GetGravatar(Configuration.Profiles[i].Email);
                }
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
                    SyncConfiguration();
                }
                else if (loadConfigTask.IsFaulted)
                {
                    Configuration = new Configuration();

                    _profilesMenu.RemoveItemAt(0);
                    _profilesMenu.InsertItem(new NSMenuItem("Failed to load Git profiles"), 0);
                    // TODO: Log this?
                }
            });
        }

        private void SyncConfiguration()
        {
            PopulateGitProfiles();

            _useGravatar.State = Configuration.UseGravatar
                ? NSCellStateValue.On
                : NSCellStateValue.Off;
        }

        private void PopulateGitProfiles()
        {
            // Remove Loading/Error menu item
            _profilesMenu.RemoveItemAt(0);

            if (Configuration.Profiles.Count == 0)
            {
                return;
            }

            (Configuration.Profiles as IEnumerable<Profile>).Reverse().ToList().ForEach(profile => {
                var profileIndex = Configuration.Profiles.IndexOf(profile);
                var label = profile.Alias != null
                    ? $"{profile.Alias} ({profile.Email})"
                    : $"{profile.Email}";
                _profilesMenu.InsertItem(new NSMenuItem(label, HandleProfileClicked) {
                    Tag = profileIndex,
                    State = Configuration.CurrentProfileIndex == profileIndex
                        ? NSCellStateValue.On
                        : NSCellStateValue.Off,
                    Image = GetGravatar(profile.Email)
                }, 0);
            });
        }

        private NSImage GetGravatar(string email)
        {
            if (!Configuration.UseGravatar)
            {
                return null;
            }

            var emailHash = CreateMD5(email).ToLowerInvariant();
            string urlString = $"https://gravatar.com/avatar/{emailHash}?d=identicon&s=32";
            return new NSImage(new NSUrl(urlString));
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
