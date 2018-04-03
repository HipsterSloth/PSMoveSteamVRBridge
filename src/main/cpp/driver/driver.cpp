#include "driver.h"
#include <openvr_driver.h>
#include <string>
#include <vector>
#include "PSMoveClient_CAPI.h"
#include "serverdriver.h"
#include "watchdog.h"

/*
Platform specific macro definitions
*/
#if defined(_WIN32) // WINDOWS
#define HMD_DLL_EXPORT extern "C" __declspec( dllexport )
#define HMD_DLL_IMPORT extern "C" __declspec( dllimport )
#elif defined(GNUC) || defined(COMPILER_GCC) || defined(__GNUC__) // LINUX/DARWIN
#define HMD_DLL_EXPORT extern "C" __attribute__((visibility("default")))
#define HMD_DLL_IMPORT extern "C" 
#else
#error "Unsupported Platform."
#endif

#define LOG_TOUCHPAD_EMULATION 0

namespace steamvrbridge {

	/*
		Driver Factory Fuction https://github.com/ValveSoftware/openvr/wiki/Driver-Factory-Function
		Returns our implementation IClientTrackedDeviceProvider & IServerTrackedDeviceProvider
	*/
	HMD_DLL_EXPORT
		void *HmdDriverFactory(const char *pInterfaceName, int *pReturnCode)
	{
		if (0 == strcmp(vr::IServerTrackedDeviceProvider_Version, pInterfaceName))
		{
			return &g_ServerTrackedDeviceProvider;
		}
		if (0 == strcmp(vr::IVRWatchdogProvider_Version, pInterfaceName))
		{
			return &g_WatchdogDriverPSMoveService;
		}

		if (pReturnCode)
			*pReturnCode = vr::VRInitError_Init_InterfaceNotFound;

		return NULL;
	}
}