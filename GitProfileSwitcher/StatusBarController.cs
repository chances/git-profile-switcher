﻿using Foundation;
using AppKit;

namespace GitProfileSwitcher
{
    public class StatusBarController : NSObject
    {
        readonly NSStatusBar statusBar = NSStatusBar.SystemStatusBar;
        readonly NSStatusItem statusItem;
        NSStatusBarButton button;
        NSPopover popOver;
        EventMonitor eventMonitor;

        public NSWindowController AboutWindow { get; private set; }
        public bool AppShouldTerminate { get; private set; } = false;

        public StatusBarController()
        {
            statusItem = statusBar.CreateStatusItem(NSStatusItemLength.Variable);
            popOver = new NSPopover();
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
			button = statusItem.Button;
			button.Image = new NSImage(image) {
                Template = true
            };
            // TODO: Add an alternate image asset
            button.AlternateImage = new NSImage(image) {
                Template = true
            };
            button.Action = new ObjCRuntime.Selector("toggle:");
			button.Target = this;

            this.popOver = popOver;

			eventMonitor = new EventMonitor((NSEventMask.LeftMouseDown | NSEventMask.RightMouseDown), MouseEventHandler);
			eventMonitor.Start();
		}

		[Export("toggle:")]
		void Toggle(NSObject sender)
		{
            if (popOver.Shown)
                Close(sender);
            else Show(sender);
		}

        /// <summary>
        /// Shows the popover
        /// </summary>
        /// <param name="sender">Sender.</param>
		public void Show(NSObject sender)
		{
		    button = statusItem.Button;
		    popOver.Show(button.Bounds, button, NSRectEdge.MaxYEdge);
		    eventMonitor.Start();
		}

        /// <summary>
        /// Hides the popover
        /// </summary>
        /// <param name="sender">Sender.</param>
		public void Close(NSObject sender)
		{
		    popOver.PerformClose(sender);
		    eventMonitor.Stop();
		}

		void MouseEventHandler(NSEvent _event)
		{
		    if (popOver.Shown)
		        Close(_event);
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
