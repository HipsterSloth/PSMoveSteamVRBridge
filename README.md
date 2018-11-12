# PSMoveSteamVRBridge [![Build status](https://ci.appveyor.com/api/projects/status/epo1qleh474o539v?svg=true)](https://ci.appveyor.com/project/HipsterSloth/psmovesteamvrbridge)  [![Documentation](https://img.shields.io/badge/code-documented-brightgreen.svg)](https://superevensteven.github.io/PSMoveSteamVRBridge/annotated.html)
PSMoveSteamVRBridge is a client for [PSMoveService](https://github.com/cboulay/PSMoveService) that takes the pose and button data of PSMove/DualShock4/PSNavi controller and forwards it into SteamVR. The [FAQ](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki/Frequently-Asked-Questions) is a good starting point for any specific questions you may have about the project. 

**NOTE** This is alpha software still heavily in development. If you are downloading this project to play games on SteamVR please be aware that this tool may not work for the game you want to play so buyer beware. That said, if you are feeling brave and want to test this we appreciate the feedback about what works and what doesn't.

# Prebuilt Releases
You can download prebuilt releases (Windows only at the moment) from the [Releases](https://github.com/HipsterSloth/PSMoveSteamVRBridge/releases) page. Then follow the initial setup instructions found in the [wiki](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki#initial-setup-video). 

# Building from source
If you want to make modifications to the service or want to debug it, you can build the project from source by following the  [Building-from-source](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki/Building-from-source) instructions. Currently Win10 is the only supported build platform with OS X and Linux support hopefully coming in the near future.

# Documentation
* General setup guides, troubleshooting and design docs can be found on the [wiki](https://github.com/HipsterSloth/PSMoveSteamVRBridge/wiki)
* Documentation for the code is hosted on [codedocs](https://codedocs.xyz/HipsterSloth/PSMoveSteamVRBridge/) (In Progress)

# Getting Help
Please start with the wiki. If you can't find help with your problem then please search through the issues (especially the closed ones) to see if your problem has been addressed already. If you still find no previous mention of your problem then you have one of two options:

A) Join us in the the [PSMoveService Google Group](https://groups.google.com/forum/#!forum/psmoveservice) to ask your question. There are several people there who have experience debugging problems with the PSMoveService and PSMoveSteamVRBridge.

B) If your problem actually is a new bug and you have files to attach (logs, pictures, etc) then go ahead and create a new issue. That said, it's probably best to start with the Google Group first anyway since we can help add context before posting an issue.

# Near Term Goals
 * Ongoing Stabilization of plugin
 
# Long Term Goals
 * In VR configuration of settings

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
