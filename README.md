# NOTICE OF ARCHIVAL
This project is no longer in development and is now archived. Anyone that wants to continue work on this project is welcome in their own fork, but repo is no longer accepting pull requests, considering any issues, anwswering questions, or offering support. WYSIWYG.

# PSMoveSteamVRBridge [![Build status](https://ci.appveyor.com/api/projects/status/epo1qleh474o539v?svg=true)](https://ci.appveyor.com/project/HipsterSloth/psmovesteamvrbridge)  [![Documentation](https://img.shields.io/badge/code-documented-brightgreen.svg)](https://superevensteven.github.io/PSMoveSteamVRBridge/annotated.html)
PSMoveSteamVRBridge is a client for [PSMoveService](https://github.com/cboulay/PSMoveService) that takes the pose and button data of PSMove/DualShock4/PSNavi controller and forwards it into SteamVR. The [FAQ](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki/Frequently-Asked-Questions) is a good starting point for any specific questions you may have about the project. 

**NOTE** This is alpha software. If you are downloading this project to play games on SteamVR please be aware that this tool may not work for the game you want to play so buyer beware.

# Prebuilt Releases
You can download prebuilt releases (Windows only at the moment) from the [Releases](https://github.com/HipsterSloth/PSMoveSteamVRBridge/releases) page. Then follow the initial setup instructions found in the [wiki](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki#initial-setup-video). 

# Building from source
If you want to make modifications to the service or want to debug it, you can build the project from source by following the  [Building-from-source](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki/Building-from-source) instructions. Currently Win10 is the only supported build platform with OS X and Linux support hopefully coming in the near future.

# Documentation
* General setup guides, troubleshooting and design docs can be found on the [wiki](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki)
* Documentation for the code is hosted on [codedocs](https://codedocs.xyz/HipsterSloth/PSMoveSteamVRBridge/) (In Progress)

### Installing the Driver in SteamVR
#### Automated Installer
In the installer folder of this project is an XML definition file for the BitRock installer project. This is a cross-platform installer that offer free licenses for opensource projects. This project's installer is built using that.

[![alt text][bitrock-logo]][bitrock-url]

# Attribution and Thanks
Special thanks to the following people who helped make this project possible:
* Thomas Perl and his [psmoveapi](https://github.com/thp/psmoveapi) project which laid the groundwork for this project.
* Alexander Nitsch and his work on [psmove-pair-win](https://github.com/nitsch/psmove-pair-win) that enabled psmove controller pairing over Bluetooth on Windows. Alex also did nearly all of the investigation into the PSMove communication protocol.
* Eugene Zatepyakin and his [PS3EYEDriver](https://github.com/inspirit/PS3EYEDriver) project which enabled access to the PS3Eye camera.
* Ritesh Oedayrajsingh Varma and his work on [PS3EYEDriver](https://github.com/rovarma/PS3EYEDriver) enabling improved camera polling performance (consistent 60fps)
* Frédéric Lopez and his work on [PS-Move-Setup](https://github.com/Fredz66/PS-Move-Setup) that enabled co registration of  and HMD with the PSMove.
* Greg New - Improvements to the SteamVR plugin and config tool
* YossiMH - Improvements to touch pad mappings and help with the HMD/Controller alignment tool
* William (zelmon64) - Many improvements to config tool UX, beta testing, and troubleshooting hero
* Antonio Jose Ramos Marquez - Work on PS4EyeDriver and PSX hardware reverse engineering
* Stephen O'Hair -  Huge refactor of the SteamVR plugin to help get it working with the new SteamVR input system

[bitrock-logo]:https://github.com/alatnet/OpenPSVR/blob/master/installer/installer-logo.png "BitRock Installer"
[bitrock-url]:http://bitrock.com
