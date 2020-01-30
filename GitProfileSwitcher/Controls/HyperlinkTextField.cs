using System;
using System.ComponentModel;
using AppKit;
using Foundation;

namespace GitProfileSwitcher.Controls
{
    [Register("HyperlinkTextField"), DesignTimeVisible(true)]
    public class HyperlinkTextField : NSTextField
    {
        private NSTrackingArea _hoverArea;
        private NSCursor _cursor;

        [Export("Href"), Browsable(true)]
        public string Href { get; set; }

        #region Constructors
        public HyperlinkTextField(IntPtr p) : base(p)
        {
        }

        public HyperlinkTextField()
        {
        }
        #endregion

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            AttributedStringValue = new NSAttributedString(StringValue, new NSStringAttributes() {
                UnderlineStyle = NSUnderlineStyle.Single.GetHashCode()
            });

            _hoverArea = new NSTrackingArea(Bounds, NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveAlways, this, null);
            AddTrackingArea(_hoverArea);
            _cursor = NSCursor.CurrentSystemCursor;
        }

        public override void MouseDown(NSEvent e)
        {
            NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl(Href));
        }

        public override void MouseEntered(NSEvent e)
        {
            base.MouseEntered(e);

            _cursor = NSCursor.PointingHandCursor;
            _cursor.Push();
        }

        public override void MouseExited(NSEvent e)
        {
            base.MouseEntered(e);

            _cursor.Pop();
        }

    }
}
