using AppKit;
using Foundation;

namespace GitProfileSwitcher
{
    public class EventMonitor
    {
        #region Private Members

        NSObject monitor;
        private readonly NSEventMask _mask;
        private readonly GlobalEventHandler _handler;

        #endregion

        #region Constructors

        public EventMonitor()
        {
        }

        public EventMonitor(NSEventMask mask, GlobalEventHandler handler)
        {
            _mask = mask;
            _handler = handler;
        }

        #endregion

        ~EventMonitor()
        {
            Stop();
        }

        /// <summary>
        /// Start monitoring events of a given mask.
        /// </summary>
		public void Start()
        {
            monitor = NSEvent.AddGlobalMonitorForEventsMatchingMask(_mask, _handler) as NSObject;
        }

        /// <summary>
        /// Stop monitoring event and release the resources.
        /// </summary>
		public void Stop()
        {
            if (monitor != null)
            {
                NSEvent.RemoveMonitor(monitor);
                monitor = null;
            }
        }
    }
}
