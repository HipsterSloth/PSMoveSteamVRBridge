#include "utils.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>
#include <math.h>
#include <string>
#include <sstream>

namespace steamvrbridge {

	//==================================================================================================
	// String Helpers
	//==================================================================================================
	int Utils::find_index_of_string_in_table(const char **string_table, const int string_table_count, const char *string)
	{
		int result_index = -1;

		for (int entry_index = 0; entry_index < string_table_count; ++entry_index)
		{
			const char *string_entry = string_table[entry_index];

			if (stricmp(string_entry, string) == 0)
			{
				result_index = entry_index;
				break;
			}
		}

		return result_index;
	}

	//==================================================================================================
	// Path Helpers
	//==================================================================================================
#ifndef MAX_UNICODE_PATH
#define MAX_UNICODE_PATH 32767
#endif

#ifndef MAX_UNICODE_PATH_IN_UTF8
#define MAX_UNICODE_PATH_IN_UTF8 (MAX_UNICODE_PATH * 4)
#endif

	char Path_GetSlash()
	{
#if defined(_WIN32)
		return '\\';
#else
		return '/';
#endif
	}

	/** Returns the specified path without its filename */
	std::string Utils::Path_StripFilename(const std::string & sPath, char slash)
	{
		if (slash == 0)
			slash = Path_GetSlash();

		std::string::size_type n = sPath.find_last_of(slash);
		if (n == std::string::npos)
			return sPath;
		else
			return std::string(sPath.begin(), sPath.begin() + n);
	}

	std::string Utils::Path_GetThisModulePath()
	{
		// gets the path of vrclient.dll itself
#ifdef WIN32
		HMODULE hmodule = NULL;

		::GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, reinterpret_cast<LPCTSTR>(Path_GetThisModulePath), &hmodule);

		wchar_t *pwchPath = new wchar_t[MAX_UNICODE_PATH];
		char *pchPath = new char[MAX_UNICODE_PATH_IN_UTF8];
		::GetModuleFileNameW(hmodule, pwchPath, MAX_UNICODE_PATH);
		WideCharToMultiByte(CP_UTF8, 0, pwchPath, -1, pchPath, MAX_UNICODE_PATH_IN_UTF8, NULL, NULL);
		delete[] pwchPath;

		std::string sPath = pchPath;
		delete[] pchPath;
		return sPath;

#elif defined( OSX ) || defined( LINUX )
	// get the addr of a function in vrclient.so and then ask the dlopen system about it
		Dl_info info;
		dladdr((void *)Path_GetThisModulePath, &info);
		return info.dli_fname;
#endif

	}

	//==================================================================================================
	// HMD Helpers
	//==================================================================================================
	bool Utils::GetHMDDeviceIndex(vr::TrackedDeviceIndex_t *out_hmd_device_index)
	{
		bool bSuccess = false;
		vr::CVRPropertyHelpers *properties_interface = vr::VRProperties();

		if (properties_interface != nullptr)
		{
			for (vr::TrackedDeviceIndex_t nDeviceIndex = 0; nDeviceIndex < vr::k_unMaxTrackedDeviceCount; ++nDeviceIndex)
			{
				vr::PropertyContainerHandle_t container_handle = properties_interface->TrackedDeviceToPropertyContainer(nDeviceIndex);

				if (container_handle != vr::k_ulInvalidPropertyContainer)
				{
					vr::ETrackedPropertyError error_code;
					bool bHasDisplayComponent = properties_interface->GetBoolProperty(container_handle, vr::Prop_HasDisplayComponent_Bool, &error_code);

					if (error_code == vr::TrackedProp_Success && bHasDisplayComponent)
					{
						*out_hmd_device_index = nDeviceIndex;
						bSuccess = true;
						break;
					}
				}
			}
		}

		return bSuccess;
	}

	bool Utils::GetTrackedDevicePose(const vr::TrackedDeviceIndex_t device_index, PSMPosef *out_device_pose)
	{
		bool bSuccess = false;
		vr::IVRServerDriverHost *driver_host_interface = vr::VRServerDriverHost();

		if (driver_host_interface != nullptr && device_index != vr::k_unTrackedDeviceIndexInvalid)
		{
			vr::TrackedDevicePose_t trackedDevicePoses[vr::k_unMaxTrackedDeviceCount];
			vr::VRServerDriverHost()->GetRawTrackedDevicePoses(0.f, trackedDevicePoses, vr::k_unMaxTrackedDeviceCount);

			const vr::TrackedDevicePose_t &device_pose = trackedDevicePoses[device_index];
			if (device_pose.bDeviceIsConnected && device_pose.bPoseIsValid)
			{
				*out_device_pose = openvrMatrixExtractPSMPosef(device_pose.mDeviceToAbsoluteTracking);
				bSuccess = true;
			}
		}

		return bSuccess;
	}

	//==================================================================================================
	// Math Helpers
	//==================================================================================================
	// From: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
	PSMQuatf Utils::openvrMatrixExtractPSMQuatf(const vr::HmdMatrix34_t &openVRTransform)
	{
		PSMQuatf q;

		const float(&a)[3][4] = openVRTransform.m;
		const float trace = a[0][0] + a[1][1] + a[2][2];

		if (trace > 0)
		{
			const float s = 0.5f / sqrtf(trace + 1.0f);

			q.w = 0.25f / s;
			q.x = (a[2][1] - a[1][2]) * s;
			q.y = (a[0][2] - a[2][0]) * s;
			q.z = (a[1][0] - a[0][1]) * s;
		}
		else
		{
			if (a[0][0] > a[1][1] && a[0][0] > a[2][2])
			{
				const float s = 2.0f * sqrtf(1.0f + a[0][0] - a[1][1] - a[2][2]);

				q.w = (a[2][1] - a[1][2]) / s;
				q.x = 0.25f * s;
				q.y = (a[0][1] + a[1][0]) / s;
				q.z = (a[0][2] + a[2][0]) / s;
			}
			else if (a[1][1] > a[2][2])
			{
				const float s = 2.0f * sqrtf(1.0f + a[1][1] - a[0][0] - a[2][2]);

				q.w = (a[0][2] - a[2][0]) / s;
				q.x = (a[0][1] + a[1][0]) / s;
				q.y = 0.25f * s;
				q.z = (a[1][2] + a[2][1]) / s;
			}
			else
			{
				const float s = 2.0f * sqrtf(1.0f + a[2][2] - a[0][0] - a[1][1]);

				q.w = (a[1][0] - a[0][1]) / s;
				q.x = (a[0][2] + a[2][0]) / s;
				q.y = (a[1][2] + a[2][1]) / s;
				q.z = 0.25f * s;
			}
		}

		q = PSM_QuatfNormalizeWithDefault(&q, k_psm_quaternion_identity);

		return q;
	}

	PSMQuatf Utils::psmMatrix3fToPSMQuatf(const PSMMatrix3f &psmMat)
	{
		PSMQuatf q;

		const float(&a)[3][3] = psmMat.m;
		const float trace = a[0][0] + a[1][1] + a[2][2];

		if (trace > 0)
		{
			const float s = 0.5f / sqrtf(trace + 1.0f);

			q.w = 0.25f / s;
			q.x = (a[2][1] - a[1][2]) * s;
			q.y = (a[0][2] - a[2][0]) * s;
			q.z = (a[1][0] - a[0][1]) * s;
		}
		else
		{
			if (a[0][0] > a[1][1] && a[0][0] > a[2][2])
			{
				const float s = 2.0f * sqrtf(1.0f + a[0][0] - a[1][1] - a[2][2]);

				q.w = (a[2][1] - a[1][2]) / s;
				q.x = 0.25f * s;
				q.y = (a[0][1] + a[1][0]) / s;
				q.z = (a[0][2] + a[2][0]) / s;
			}
			else if (a[1][1] > a[2][2])
			{
				const float s = 2.0f * sqrtf(1.0f + a[1][1] - a[0][0] - a[2][2]);

				q.w = (a[0][2] - a[2][0]) / s;
				q.x = (a[0][1] + a[1][0]) / s;
				q.y = 0.25f * s;
				q.z = (a[1][2] + a[2][1]) / s;
			}
			else
			{
				const float s = 2.0f * sqrtf(1.0f + a[2][2] - a[0][0] - a[1][1]);

				q.w = (a[1][0] - a[0][1]) / s;
				q.x = (a[0][2] + a[2][0]) / s;
				q.y = (a[1][2] + a[2][1]) / s;
				q.z = 0.25f * s;
			}
		}

		q = PSM_QuatfNormalizeWithDefault(&q, k_psm_quaternion_identity);

		return q;
	}

	float Utils::psmVector3fDistance(const PSMVector3f &a, const PSMVector3f &b)
	{
		const PSMVector3f diff = PSM_Vector3fSubtract(&a, &b);

		return PSM_Vector3fLength(&diff);
	}

	PSMVector3f Utils::psmVector3fLerp(const PSMVector3f &a, const PSMVector3f &b, float u)
	{
		const PSMVector3f scaled_a = PSM_Vector3fScale(&a, 1.f - u);
		const PSMVector3f scaled_b = PSM_Vector3fScale(&b, u);
		const PSMVector3f result = PSM_Vector3fAdd(&scaled_a, &scaled_b);

		return result;
	}

	PSMVector3f Utils::openvrMatrixExtractPSMVector3f(const vr::HmdMatrix34_t &openVRTransform)
	{
		const float(&a)[3][4] = openVRTransform.m;
		PSMVector3f pos = { a[0][3], a[1][3], a[2][3] };

		return pos;
	}

	PSMPosef Utils::openvrMatrixExtractPSMPosef(const vr::HmdMatrix34_t &openVRTransform)
	{
		PSMPosef pose;
		pose.Orientation = openvrMatrixExtractPSMQuatf(openVRTransform);
		pose.Position = openvrMatrixExtractPSMVector3f(openVRTransform);

		return pose;
	}

	//==================================================================================================
	// Conversion Helpers
	//==================================================================================================
	std::string Utils::PSMVector3fToString(const PSMVector3f& position)
	{
		std::ostringstream stringBuilder;
		stringBuilder << "(" << position.x << ", " << position.y << ", " << position.z << ")";
		return stringBuilder.str();
	}


	std::string Utils::PSMQuatfToString(const PSMQuatf& rotation)
	{
		std::ostringstream stringBuilder;
		stringBuilder << "(" << rotation.w << ", " << rotation.x << ", " << rotation.y << ", " << rotation.z << ")";
		return stringBuilder.str();
	}


	std::string Utils::PSMPosefToString(const PSMPosef& pose)
	{
		std::ostringstream stringBuilder;
		stringBuilder << "[Pos: " << PSMVector3fToString(pose.Position) << ", Rot:" << PSMQuatfToString(pose.Orientation) << "]";
		return stringBuilder.str();
	}

	//==================================================================================================
	// Generate Helpers
	//==================================================================================================
	void Utils::GenerateTrackerSerialNumber(char *p, int psize, int tracker)
	{
		snprintf(p, psize, "psmove_tracker%d", tracker);
	}

	void Utils::GenerateControllerSteamVRIdentifier(char *p, int psize, int controller)
	{
		snprintf(p, psize, "psmove_controller%d", controller);
	}
}