#include "driver.h"

namespace steamvrbridge {

	/*
		The implementation of our driver factory fuction.
		Returns our implementation of:
			- IClientTrackedDeviceProvider 
			- IServerTrackedDeviceProvider
	*/
	HMD_DLL_EXPORT
		void *HmdDriverFactory(const char *pInterfaceName, int *pReturnCode){

		// When the steamvr runtime requests the server tracked device provider implementation, 
		// return our server tracked device provider.
		if (0 == strcmp(vr::IServerTrackedDeviceProvider_Version, pInterfaceName)) {
			return &g_ServerTrackedDeviceProvider;
		}
		
		// When the steamvr runtime requests the watchdog provider implementation, 
		// return our watchdog provider.
		if (0 == strcmp(vr::IVRWatchdogProvider_Version, pInterfaceName)) {
			return &g_WatchdogDriverPSMoveService;
		}

		// If we get to this point it means we had neither implementations and set the returncode
		// to error.
		if (pReturnCode) {
			*pReturnCode = vr::VRInitError_Init_InterfaceNotFound;
		}

		return NULL;
	}
}