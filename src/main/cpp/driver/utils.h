#pragma once
#include "openvr_driver.h"
#include "PSMoveClient_CAPI.h"
#include <string>
// Platform specific includes
#if defined( _WIN32 )
#include <windows.h>
#include <direct.h>
#define getcwd _getcwd // suppress "deprecation" warning
#else
#include <unistd.h>
#endif

namespace steamvrbridge {

	class Utils {
	public:
		static int find_index_of_string_in_table(const char **string_table, const int string_table_count, const char *string);
		static std::string Path_StripFilename(const std::string & sPath, char slash);
		static std::string Path_GetThisModulePath();
		static bool GetHMDDeviceIndex(vr::TrackedDeviceIndex_t *out_hmd_device_index);
		static bool GetTrackedDevicePose(const vr::TrackedDeviceIndex_t device_index, PSMPosef *out_device_pose);
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