# Another Scene Helper (Ash)
A plugin for Play Home for coordinating visibility of clothing and accessory items, and other miscellaneous functionality.

## Features
### Visibility Rules
This feature allows to create rules for clothing and accessories to automatically change the visibility of the dependent item, when the parent item has changed its state. The feature works for clothing and accessories in any combination and supports extended slots from MoreAccessories plugin.

### Visibility Controls
The plugin has a separate tab that provides visibility controls for all the items both in the editor and in h-scene.

### Immersive UI
This is a beta feature that allows to hide some of the less used buttons and UI elements, and scales down the others to provide more immersive h-scene experience.

### Scene Controls
Just a few QoL things to control during H-Scene.
- Toggles for bodily fluids
- Toggle to automatically mute inactive girl

## Installation
Download the latest release, unzip the archive, drag and drop the contents into the game directory.

## How to Use
The main plugin window is bound by default to a backquote `` ` `` key. The hotkey can be changed in the BepInEx plugin/mod settings menu.

### Adding a new rule
 1. Go to the `Items Visibility Coordinator` tab.
 1. Select a girl (the plugin displays only the items currently worn by the girl)
 1. From the list Clothing and Accessories select a `Master Item` - this will be the parent item that will control the behavior of all the dependent items bound to it.
 1. Select the `State of Master Item` - when Master Item will be in that state, all the items dependent on it will chance according to their rules.
 1. Select the `Slave Item` - this is the dependent item.
 1. Select the `State of Slave Item` - this is how the visibility of the dependent item will change, when the Master Item will assume the state you selected for it in (4).
 1. Optionally you can choose to create the rules for every *other* state of the `Master Item`. For example, if in (4) you selected `Dressed` for your master item, it will create rules for `Half` and `Hidden` states.
 1. When everything is selected, and in (7) you selected `Yes` you will be able to choose the state of the `Slave Item` in *other* master states. This allows to quickly create cyclic rules. For example - *hide* the dependent item, when the master is visible, and *show* it in all the other states.   

### Persistence
- The rules are stored in `PlayHome/BepInEx/config/Ash.ItemRules.json`[^1].
- Settings are stored in `PlayHome/BepInEx/config/Ash.Settings.json`.

## Dependencies
The plugin relies on two external libraries - `Newtonsoft.Json` for JSON serialization and `OneOf` for type unions. Both libraries are packaged with the plugin.

## Bugs and Issues
If you found a bug, file a bug report on GitHub with description, logs (`PlayHome/output_log.txt`) and reproduction steps if possible.

## License

Copyright 2025-2026 Ivan Alantiev

Licensed under the GNU GPLv3: https://www.gnu.org/licenses/gpl-3.0.html

[^1]: Note that if you want to edit the file manually, all of the type and assembly names under `$type` property *must* be preserved. Otherwise the file will fail to deserialize.
