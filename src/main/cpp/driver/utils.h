#pragma once
#include "openvr_driver.h"
#include "PSMoveClient_CAPI.h"
#include <string>
#include <vector>

// Platform specific includes
#if defined( _WIN32 )
#include <windows.h>
#include <direct.h>
#define getcwd _getcwd // suppress "deprecation" warning
#else
#include <unistd.h>
#endif

#if _MSC_VER
#define strcasecmp(a, b) stricmp(a,b)
#pragma warning (disable: 4996) // 'This function or variable may be unsafe': snprintf
#define snprintf _snprintf
#endif

namespace steamvrbridge {

    #ifdef WIN32
    class Win32Utils {
	public:       
	    static std::string GetLastErrorAsString();
    };
    #endif // WIN32

    class StringUtils {
	public:       
        static bool ConvertWideToMultiByte(const wchar_t *wc_string, char *out_mb_serial, const size_t mb_buffer_size);
        static int FindIndexInTable(const char **string_table, const int string_table_count, const char *string);
    };

	class Utils {
	public:       
		static std::string Path_StripFilename(const std::string & sPath, char slash);
		static std::string Path_GetThisModulePath();
		static std::string Path_GetHomeDirectory();
		static std::string Path_GetPSMoveSteamVRBridgeInstallPath(const class ServerDriverConfig *config);
		static std::string Path_GetPSMoveSteamVRBridgeDriverRootPath(const class ServerDriverConfig *config);
		static std::string Path_GetPSMoveSteamVRBridgeDriverBinPath(const class ServerDriverConfig *config);
		static std::string Path_GetPSMoveSteamVRBridgeDriverResourcesPath(const class ServerDriverConfig *config);
		static std::string Path_GetPSMoveServiceInstallPath(const class ServerDriverConfig *config);
		static bool Path_CreateDirectory(const std::string &path);
		static bool Path_FileExists(const std::string& filename);

		static bool IsProcessRunning(const std::string &processName);
		static bool LaunchProcess(const std::string &processPath, const std::string &processName, const std::vector<std::string> &args);
		static bool GetHMDDeviceIndex(vr::TrackedDeviceIndex_t *out_hmd_device_index);
		static bool GetTrackedDevicePose(const vr::TrackedDeviceIndex_t device_index, PSMPosef *out_device_pose);
		static PSMQuatf ExtractHMDYawQuaternion(const PSMQuatf & q);
		static PSMQuatf ExtractPSMoveYawQuaternion(const PSMQuatf & q);
		// Takes a given controller position vector, applies a given quaternion rotation and sets the result to a given output position vector.
		static void GetMetersPosInRotSpace(const PSMQuatf * rotation, PSMVector3f * out_position, const PSMPSMove & view);

		static PSMQuatf openvrMatrixExtractPSMQuatf(const vr::HmdMatrix34_t &openVRTransform);
		static PSMQuatf psmMatrix3fToPSMQuatf(const PSMMatrix3f &psmMat);
		static float psmVector3fDistance(const PSMVector3f &a, const PSMVector3f &b);
		static PSMVector3f psmVector3fLerp(const PSMVector3f &a, const PSMVector3f &b, float u);
		static PSMVector3f openvrMatrixExtractPSMVector3f(const vr::HmdMatrix34_t &openVRTransform);
		static PSMPosef openvrMatrixExtractPSMPosef(const vr::HmdMatrix34_t &openVRTransform);
		static std::string PSMVector3fToString(const PSMVector3f& position);
		static std::string PSMQuatfToString(const PSMQuatf& rotation);
		static std::string PSMPosefToString(const PSMPosef& pose);
		static void GenerateTrackerSerialNumber(char *p, int psize, int tracker);
		static void GenerateControllerSteamVRIdentifier(char *p, int psize, int controller);
	};
}