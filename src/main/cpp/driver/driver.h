#pragma once
#include <openvr_driver.h>
#include <string>
#include <vector>

#include "PSMoveClient_CAPI.h"
#include "serverdriver.h"
#include "watchdog.h"

namespace steamvrbridge {
	/*
	Here we create the two mandatory implementations of the following interfaces required
	by OpenVR:

	-  IClientTrackedDeviceProvider https://github.com/ValveSoftware/openvr/wiki/IClientTrackedDeviceProvider_Overview
	-  IServerTrackedDeviceProvider https://github.com/ValveSoftware/openvr/wiki/IServerTrackedDeviceProvider_Overview

	When the Steam runtime calls this driver we return our implementations of each.

	*/
	static CServerDriver_PSMoveService g_ServerTrackedDeviceProvider;
	static CWatchdogDriver_PSMoveService g_WatchdogDriverPSMoveService;
}


