TAC Sticky Controls
================

Another mod by Thunder Aerospace Corporation, Sticky Controls makes attitude control inputs persist over time rather than being instantaneous.

For use with the Kerbal Space Program, http://kerbalspaceprogram.com/

Licensed under the Apache License Version 2.0.  See the LICENSE.txt and NOTICE.txt files.

Source code can be found at https://github.com/taraniselsu/TacStickyControls

### Flavor text
Someone -- _Bill?_ -- got a little too excited during his last flight and spilled his drink on the control panel. Now the control stick won't return to neutral when released! The weird thing is, some pilots are saying planes are now _easier_ to fly.


### Features
**NOT ALL FEATURES ARE IMPLEMENTED YET.**
- Each attitude control input (the WASD keys) moves the controls by a small amount and the controls do not return to neutral when released. Works kind of like trim, but more adjustable and easier to work with. May be familiar to players of the Microsoft Flight Simulator series.
- Fully adjustable: easy to change the amount the controls move per key press.
- Immediately center the controls by pressing Z.
- Toggle on/off whenever desired by pressing Alt+Z.
- Displays a small window showing the current control positions, along with the configurable settings.
- **(Not implemented yet.)** _The window can be shown or hidden by clicking the button, which can be displayed on its own, in Blizzy's toolbar, or in the stock toolbar._
- Configurable settings:
  - Speed: how fast the controls move while pressing the key
  - Step: the minimum amount the controls are moved per key press. It also snaps the controls to multiples of this value, so larger values cause it to change in large discreet steps.
  - PrecisionControlsModifier: scales both Speed and Step by this amount when "precision controls" is turned on (the Caps Lock key in KSP's default key bindings).
  - ZeroControlsKey: the key bound to zeroing the controls. The default is 'z'. Note that toggling on/off is always Alt plus this key **(will need to change Alt to support Mac/Linux)**


### Notice
Includes the [KSP Add-on Version Checker's MiniAVC](http://forum.kerbalspaceprogram.com/threads/79745). It does a GET request to http://ksp-avc.cybutek.net/ to find the latest release number. It is **opt-in** and **no information is sent to the server**. I recommend downloading the [full KSP-AVC Plugin](http://forum.kerbalspaceprogram.com/threads/79745) to get the most out of it. Also encourage other mod creators to support it.


### Installation instructions
- Unzip to the KSP directory.

The following files should be in these locations:
{KSP}/GameData/TacStickyControls/TacStickyControls.dll

The following file will be created after launching your first spacecraft:
{KSP}/GameData/TacStickyControls/PluginData/TacStickyControls/StickyControls.cfg



Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
is purely coincidental.
