# Git Profile Switcher
A macOS Menu Bar app to quickly switch between multiple Git profiles.

## Development
1. Visual Studio for Mac
2. Xcode

## Steps
Here is how you can do it - 
1. In Xcode, delete Window Controller Scene from `Main.storyboard` and provide Storyboard ID to View Controller.
2. Add new entry in info.plist - `Application is agent (UIElement)` with its value set to `Yes` to make the app behave as a ghost.
3. Create a Menu Bar status item using `CreateStatusItem()` method of `NSStatusBar`.
4. Handle status bar icon for dark and light theme using `Template` property of `NSImage`.
5. Handle event handling of status item using `Selector` class of `ObjCRuntime`.
6. Show a popover using `NSPopover` and its `Show()` method.
7. Make popover show on demand and hide when user moves on using `NSEvent` and a custom class.
8. In Xcode, add a button to View Controller and create an action `QuitApplication` by `control + drag`. In VS, call `Terminate` method of `NSApplication`.

Unless stated otherwise, everything is done in Visual Studio for Mac.

## Attribution
Based on [Ambar-Xamarin](https://github.com/AnaghSharma/Ambar-Xamarin).

## License
[MIT License](http://opensource.org/licenses/MIT)

Copyright &copy; 2017-2018 Chance Snow. All rights reserved.
