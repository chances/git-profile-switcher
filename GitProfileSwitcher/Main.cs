using AppKit;
#if !DEBUG
using Sentry;
#endif

namespace GitProfileSwitcher
{
    public static class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            using var sentry = SentrySdk.Init(
                "https://8f74badcbd494ce6bfe9895143dfb444@sentry.io/2134818");
#endif
            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy =
                NSApplicationActivationPolicy.Accessory;
            NSApplication.Main(args);
        }
    }
}
