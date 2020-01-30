using AppKit;

namespace GitProfileSwitcher
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy =
                NSApplicationActivationPolicy.Accessory;
            NSApplication.Main(args);
        }
    }
}
