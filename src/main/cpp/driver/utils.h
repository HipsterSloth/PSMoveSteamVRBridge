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

#if _MSC_VER
#define strcasecmp(a, b) stricmp(a,b)
#pragma warning (disable: 4996) // 'This function or variable may be unsafe': snprintf
#define snprintf _snprintf
#endif

namespace steamvrbridge {

	class Utils {
	public:
		static int find_index_of_string_in_table(const char **string_table, const int string_table_count, const char *string);
		static std::string Path_StripFilename(const std::string & sPath, char slash);
		static std::string Path_GetThisModulePath();
		static bool GetHMDDeviceIndex(vr::TrackedDeviceIndex_t *out_hmd_device_index);
		static bool GetTrackedDevicePose(const vr::TrackedDeviceIndex_t device_index, PSMPosef *out_device_pose);
		static PSMQuatf ExtractHMDYawQuaternion(const PSMQuatf & q);
		static PSMQuatf ExtractPSMoveYawQuaternion(const PSMQuatf & q);
		// Takes a given controller position vector, applies a given quartenian rotation and sets the result to a given output position vector.
		static void GetMetersPosInRotSpace(const PSMQuatf * rotation, PSMVector3f * out_position, const PSMPSMove & view);

		// Returns the HMD pose in meters.
		// Throws a std::exception when HMD index or HMD pose can't be obtained.
		static PSMPosef Utils::GetHMDPoseInMeters();

		// Returns a PSM pose of the controller aligned to the HMD tracking space. Returns NULL pointer when
		// HMD index or pose can't be obtained.
		static PSMPosef Utils::RealignHMDTrackingSpace(PSMQuatf controllerOrientationInHmdSpaceQuat,
													   PSMVector3f controllerLocalOffsetFromHmdPosition,
													   PSMControllerID controllerId,
													   PSMPosef hmd_pose_meters,
													   bool useControllerOrientation);
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