#pragma once
#include <openvr_driver.h>
#include <string>
#include <vector>

#include "PSMoveClient_CAPI.h"
#include "server_driver.h"
#include "watchdog.h"

/*
Platform specific definitions
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
		IServerTrackedDeviceProvider implementation as per:
		https://github.com/ValveSoftware/openvr/wiki/IServerTrackedDeviceProvider_Overview 
	*/
	static CServerDriver_PSMoveService g_ServerTrackedDeviceProvider;
	
	/*  
		IClientTrackedDeviceProvider implementation as per: 
		https://github.com/ValveSoftware/openvr/wiki/IClientTrackedDeviceProvider_Overview 
	*/
	static CWatchdogDriver_PSMoveService g_WatchdogDriverPSMoveService;

	/* 
		Factory driver function as per :
		https://github.com/ValveSoftware/openvr/wiki/Driver-Factory-Function
	*/
	HMD_DLL_EXPORT void* HmdDriverFactory(const char* pInterfaceName, int* pReturnCode);
}