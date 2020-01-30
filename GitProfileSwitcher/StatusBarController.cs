using Foundation;
using AppKit;

namespace GitProfileSwitcher
{
    public class StatusBarController : NSObject
    {
        private readonly NSStatusItem _statusItem;
        private readonly NSPopover _popOver = new NSPopover();
        private readonly EventMonitor _eventMonitor;

        public NSWindowController AboutWindow { get; private set; }
        public bool AppShouldTerminate { get; private set; } = false;

        public StatusBarController()
        {
            _statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Variable);
            ViewController.QuitButtonClicked += HandleQuitButtonClicked;
            ViewController.AboutMenuItemClicked += HandleAboutMenuItemClicked;
			var storyboard = NSStoryboard.FromName("Main", null);
			AboutWindow = storyboard.InstantiateControllerWithIdentifier("AboutWindow") as NSWindowController;
		}

        ~StatusBarController()
        {
            ViewController.QuitButtonClicked -= HandleQuitButtonClicked;
            ViewController.AboutMenuItemClicked -= HandleAboutMenuItemClicked;
        }

        /// <summary>
        /// Initialise a NSStatusItem instance with an image, popover and event handling.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="popOver">Pop over.</param>
        public void InitStatusBarItem(string image, NSPopover popOver)
        {
			var button = _statusItem.Button;
			button.Image = new NSImage(image) {
                Template = true
            };
            // TODO: Add an alternate image asset
            button.AlternateImage = new NSImage(image) {
                Template = true
            };
            button.Action = new ObjCRuntime.Selector("toggle:");
			button.Target = this;

            _popOver = popOver;

			eventMonitor = new EventMonitor((NSEventMask.LeftMouseDown | NSEventMask.RightMouseDown), MouseEventHandler);
			_eventMonitor.Start();
		}

		[Export("toggle:")]
		void Toggle(NSObject sender)
		{
            if (_popOver.Shown)
                Close(sender);
            else Show(sender);
		}

        /// <summary>
        /// Shows the popover
        /// </summary>
        /// <param name="sender">Sender.</param>
		public void Show(NSObject sender)
		{
		    var button = _statusItem.Button;
		    _popOver.Show(button.Bounds, button, NSRectEdge.MaxYEdge);
		    _eventMonitor.Start();
		}

        /// <summary>
        /// Hides the popover
        /// </summary>
        /// <param name="sender">Sender.</param>
		public void Close(NSObject sender)
		{
		    _popOver.PerformClose(sender);
		    _eventMonitor.Stop();
		}

		void MouseEventHandler(NSEvent e)
		{
            if (_popOver.Shown)
		        Close(e);
		}

        void HandleQuitButtonClicked(object sender, System.EventArgs e)
        {
            Close(sender as NSObject);
            AppShouldTerminate = true;
            NSApplication.SharedApplication.Terminate(sender as NSObject);
		}

        void HandleAboutMenuItemClicked(object sender, System.EventArgs e)
        {
            Close(sender as NSObject);
            AboutWindow.ShowWindow(sender as NSObject);
        }
    }
}
