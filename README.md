# FP2Lib

A library intended to simplify most common parts of modding **Freedom Planet 2**, as well as provide extra functionality used in [Freedom Manager](https://github.com/Kuborros/FreedomManager).


### Current Functionality:
* Provides *Save file redirection* features used by the mod manager.
* Provides framework to easily instance new NPC.
* Provides framework to easily add new Vinyls and Badges.
* Provides framework to add new playable characters into the game.
* Provides framework to instance NPC from json and AssetBundle pair.
* Provides patches for issues caused by workings of mods and game.
* Provides game build information.

You can read documentation of specific parts on the [Wiki](https://github.com/Kuborros/FP2Lib/wiki)

### Usage:

This library is auto-installed by the mod manager, and exposes no player-visible components in-game.
If you wish to use it _without_ the mod manager, you can install it from the Releases tab like any other mod.
It's main functionality is handled internally, depending on other installed mods which depend on it.
Some of the functions can be managed by the config file. Example config file contains following options which can be edited by hand:
```ini
[Debug]

## Engages cowabunga mode. No sanity checks will be run, will attempt to hook in on any FP2 version.
## Yes, this includes 2015 Sample Versions. Your mileage might vary and bug reports with this mode on will *not* be accepted!
## Some mods might read this value to skip their own checks. For example, a character mod might elect to not check if the version of the game is compatible when this value is set.
# Setting type: Boolean
# Default value: false
Cowabunga = false

[Language Settings]

## Force specific language on launch. Leave empty for default behaviour. 
## This feature is currently inactive, due to apparent cancellation of FP2's language update.
# Setting type: String
# Default value: 
Force Language = 
```
And following ones which should _not_ be touched manually, as they are managed by FP2 Mod Manager. 
Editing _any_ of them at runtime can have **disasterous** results, and are therefore marked as Advanced Settings using Bepinex's config flags.
```ini
[Save Redirection]

## Enable save file redirection.
# Setting type: Boolean
# Default value: false
Enabled = true

## Makes JSON files more human-readable.
# Setting type: Boolean
# Default value: false
Fancy Json = true

## Select save redirection profile.
# Setting type: Int32
# Default value: 1
# Acceptable value range: From 0 to 9
Profile = 0
```

### Compilation:

Clone the repository and load it in Visual Studio. 
The basic required dependencies should be automatically loaded from nuGet, you will however need to provide an Assembly Reference to your copy of Freedom Planet 2's Assembly-Csharp.dll.
After that is added, the project should build without a hitch. 

Packaging release .zip follows the same steps as a normal mod, with exception of the path and modified modinfo.json (here named fp2lib.json).
