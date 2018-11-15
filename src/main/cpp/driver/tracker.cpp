#include "tracker.h"
#include "trackable_device.h"
#include "utils.h"
#include "constants.h"

namespace steamvrbridge {

	PSMServiceTracker::PSMServiceTracker(const PSMClientTrackerInfo *trackerInfo)
		: TrackableDevice()
		, m_nTrackerId(trackerInfo->tracker_id)
	{
		char buf[256];
		Utils::GenerateTrackerSerialNumber(buf, sizeof(buf), trackerInfo->tracker_id);
		m_strSteamVRSerialNo = buf;

		SetClientTrackerInfo(trackerInfo);
	}

	PSMServiceTracker::~PSMServiceTracker()
	{
	}

	vr::EVRInitError PSMServiceTracker::Activate(vr::TrackedDeviceIndex_t unObjectId)
	{
		vr::EVRInitError result = TrackableDevice::Activate(unObjectId);

		if (result == vr::VRInitError_None)
		{
			vr::CVRPropertyHelpers *properties = vr::VRProperties();

			properties->SetFloatProperty(m_ulPropertyContainer, vr::Prop_FieldOfViewLeftDegrees_Float, m_tracker_info.tracker_hfov / 2.f);
			properties->SetFloatProperty(m_ulPropertyContainer, vr::Prop_FieldOfViewRightDegrees_Float, m_tracker_info.tracker_hfov / 2.f);
			properties->SetFloatProperty(m_ulPropertyContainer, vr::Prop_FieldOfViewTopDegrees_Float, m_tracker_info.tracker_vfov / 2.f);
			properties->SetFloatProperty(m_ulPropertyContainer, vr::Prop_FieldOfViewBottomDegrees_Float, m_tracker_info.tracker_vfov / 2.f);
			properties->SetFloatProperty(m_ulPropertyContainer, vr::Prop_TrackingRangeMinimumMeters_Float, m_tracker_info.tracker_znear * k_fScalePSMoveAPIToMeters);
			properties->SetFloatProperty(m_ulPropertyContainer, vr::Prop_TrackingRangeMaximumMeters_Float, m_tracker_info.tracker_zfar * k_fScalePSMoveAPIToMeters);

			properties->SetInt32Property(m_ulPropertyContainer, vr::Prop_DeviceClass_Int32, vr::TrackedDeviceClass_TrackingReference);

			// The {psmove} syntax lets us refer to rendermodels that are installed
			// in the driver's own resources/rendermodels directory.  The driver can
			// still refer to SteamVR models like "generic_hmd".
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "{psmove}ps3eye_tracker");

			char model_label[16] = "\0";
			snprintf(model_label, sizeof(model_label), "ps3eye_%d", m_tracker_info.tracker_id);
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_ModeLabel_String, model_label);

			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceOff_String, "{psmove}base_status_off.png");
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceSearching_String, "{psmove}base_status_ready.png");
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceSearchingAlert_String, "{psmove}base_status_ready_alert.png");
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceReady_String, "{psmove}base_status_ready.png");
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceReadyAlert_String, "{psmove}base_status_ready_alert.png");
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceNotReady_String, "{psmove}base_status_error.png");
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceStandby_String, "{psmove}base_status_standby.png");
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceAlertLow_String, "{psmove}base_status_ready_low.png");

			// Poll the latest WorldFromDriverPose transform we got from the service
			// Transform used to convert from PSMove Tracking space to OpenVR Tracking Space
			RefreshWorldFromDriverPose();
		}

		return result;
	}

	void PSMServiceTracker::Deactivate()
	{
	}

	void PSMServiceTracker::SetClientTrackerInfo(
		const PSMClientTrackerInfo *trackerInfo)
	{
		m_tracker_info = *trackerInfo;

		m_Pose.result = vr::TrackingResult_Running_OK;

		m_Pose.deviceIsConnected = true;

		// Yaw can't drift because the tracker never moves (hopefully)
		m_Pose.willDriftInYaw = false;
		m_Pose.shouldApplyHeadModel = false;

		// No prediction since that's already handled in the psmove service
		m_Pose.poseTimeOffset = 0.f;

		// Poll the latest WorldFromDriverPose transform we got from the service
		// Transform used to convert from PSMove Tracking space to OpenVR Tracking Space
		RefreshWorldFromDriverPose();

		// No transform due to the current HMD orientation
		m_Pose.qDriverFromHeadRotation.w = 1.f;
		m_Pose.qDriverFromHeadRotation.x = 0.0f;
		m_Pose.qDriverFromHeadRotation.y = 0.0f;
		m_Pose.qDriverFromHeadRotation.z = 0.0f;
		m_Pose.vecDriverFromHeadTranslation[0] = 0.f;
		m_Pose.vecDriverFromHeadTranslation[1] = 0.f;
		m_Pose.vecDriverFromHeadTranslation[2] = 0.f;

		// Set position
		{
			const PSMVector3f &position = m_tracker_info.tracker_pose.Position;

			m_Pose.vecPosition[0] = position.x * k_fScalePSMoveAPIToMeters;
			m_Pose.vecPosition[1] = position.y * k_fScalePSMoveAPIToMeters;
			m_Pose.vecPosition[2] = position.z * k_fScalePSMoveAPIToMeters;
		}

		// Set rotational coordinates
		{
			const PSMQuatf &orientation = m_tracker_info.tracker_pose.Orientation;

			m_Pose.qRotation.w = orientation.w;
			m_Pose.qRotation.x = orientation.x;
			m_Pose.qRotation.y = orientation.y;
			m_Pose.qRotation.z = orientation.z;
		}

		m_Pose.poseIsValid = true;
	}

	void PSMServiceTracker::Update()
	{
		TrackableDevice::Update();

		// This call posts this pose to shared memory, where all clients will have access to it the next
		// moment they want to predict a pose.
		vr::VRServerDriverHost()->TrackedDevicePoseUpdated(m_unSteamVRTrackedDeviceId, m_Pose, sizeof(vr::DriverPose_t));
	}

	bool PSMServiceTracker::HasTrackerId(int TrackerID)
	{
		return TrackerID == m_nTrackerId;
	}
}