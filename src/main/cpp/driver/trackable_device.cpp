#include "trackable_device.h"
#include "utils.h"
#include "logger.h"
#include "driver.h"

namespace steamvrbridge {
	ITrackableDevice::ITrackableDevice()
		: m_ulPropertyContainer(vr::k_ulInvalidPropertyContainer)
		, m_unSteamVRTrackedDeviceId(vr::k_unTrackedDeviceIndexInvalid) {
		memset(&m_Pose, 0, sizeof(m_Pose));
		m_Pose.result = vr::TrackingResult_Uninitialized;

		// By default, assume that the tracked devices are in the tracking space as OpenVR
		m_Pose.qWorldFromDriverRotation.w = 1.f;
		m_Pose.qWorldFromDriverRotation.x = 0.f;
		m_Pose.qWorldFromDriverRotation.y = 0.f;
		m_Pose.qWorldFromDriverRotation.z = 0.f;
		m_Pose.vecWorldFromDriverTranslation[0] = 0.f;
		m_Pose.vecWorldFromDriverTranslation[1] = 0.f;
		m_Pose.vecWorldFromDriverTranslation[2] = 0.f;

		m_firmware_revision = 0x0001;
		m_hardware_revision = 0x0001;
	}

	ITrackableDevice::~ITrackableDevice() {}

	// Shared Implementation of vr::ITrackedDeviceServerDriver
	vr::EVRInitError ITrackableDevice::Activate(vr::TrackedDeviceIndex_t unObjectId) {
		vr::CVRPropertyHelpers *properties = vr::VRProperties();

		steamvrbridge::Logger::Info("CPSMoveTrackedDeviceLatest::Activate: %s is object id %d\n", GetSteamVRIdentifier(), unObjectId);
		m_ulPropertyContainer = properties->TrackedDeviceToPropertyContainer(unObjectId);
		m_unSteamVRTrackedDeviceId = unObjectId;

		properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_Firmware_UpdateAvailable_Bool, false);
		properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_Firmware_ManualUpdate_Bool, false);
		properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_ContainsProximitySensor_Bool, false);
		properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_HasCamera_Bool, false);
		properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_Firmware_ForceUpdateRequired_Bool, false);
		properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceCanPowerOff_Bool, false);
		//properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_HardwareRevision_Uint64, m_hardware_revision);
		//properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_FirmwareVersion_Uint64, m_firmware_revision);

		// Configure JSON controller configuration input profile
		vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_InputProfilePath_String, "{psmove}/input/controller_profile.json");

		/* Buttons */
		{
			// System button component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_System, &m_hButtons[k_eButton_System]);

			// Grip button component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_Grip, &m_hButtons[k_eButton_Grip]);

			// Back button component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_Back, &m_hButtons[k_eButton_Back]);

			// Application Menu button component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_Application, &m_hButtons[k_eButton_Application]);
		}

		/* Trigger */
		{
			// Trigger button component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_Trigger, &m_hButtons[k_eButton_Trigger]);

			// Trigger touch component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_Trigger_Touch, &m_hButtons[k_eTouch_Trigger]);

			// Trigger value component
			vr::VRDriverInput()->CreateScalarComponent(m_ulPropertyContainer, k_pch_Trackpad_Value, &m_hAxes[k_eValue_Trigger],
													   vr::VRScalarType_Absolute, vr::VRScalarUnits_NormalizedOneSided);
		}

		/* Trackpad */
		{
			// Create Trackpad Click button component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_Trackpad, &m_hButtons[k_eButton_Trackpad]);

			// Create Trackpad Click button component
			vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, k_pch_Trackpad_Touch, &m_hButtons[k_eTouch_Trackpad]);

			// Create Trackpad Axis X button component
			vr::VRDriverInput()->CreateScalarComponent(m_ulPropertyContainer, k_pch_Trackpad_X, &m_hAxes[k_eTrackpad_X],
													   vr::VRScalarType_Absolute, vr::VRScalarUnits_NormalizedTwoSided);

			// Create Trackpad Axis Y button component
			vr::VRDriverInput()->CreateScalarComponent(m_ulPropertyContainer, k_pch_Trackpad_Y, &m_hAxes[k_eTrackpad_Y],
													   vr::VRScalarType_Absolute, vr::VRScalarUnits_NormalizedTwoSided);
		}

		/* Create Haptic feedback component */
		vr::VRDriverInput()->CreateHapticComponent(m_ulPropertyContainer, k_pch_Haptic, &m_ulHapticComponent);

		return vr::VRInitError_None;
	}

	void ITrackableDevice::Deactivate() {
		steamvrbridge::Logger::Info("CPSMoveTrackedDeviceLatest::Deactivate: %s was object id %d\n", GetSteamVRIdentifier(), m_unSteamVRTrackedDeviceId);
		m_unSteamVRTrackedDeviceId = vr::k_unTrackedDeviceIndexInvalid;
	}

	void ITrackableDevice::EnterStandby() {
		//###HipsterSloth $TODO - No good way to do this at the moment
	}

	void *ITrackableDevice::GetComponent(const char *pchComponentNameAndVersion) {
		return NULL;
	}

	void ITrackableDevice::DebugRequest(const char * pchRequest, char * pchResponseBuffer, uint32_t unResponseBufferSize) {

	}

	vr::DriverPose_t ITrackableDevice::GetPose() {
		// This is only called at startup to synchronize with the driver.
		// Future updates are driven by our thread calling TrackedDevicePoseUpdated()
		return m_Pose;
	}

	// TrackedDevice Interface
	vr::ETrackedDeviceClass ITrackableDevice::GetTrackedDeviceClass() const {
		// TODO implement this properly
		return vr::TrackedDeviceClass_Invalid;
	}

	// Returns the tracked device's role. e.g. TrackedControllerRole_LeftHand
	vr::ETrackedControllerRole ITrackableDevice::GetTrackedDeviceRole() const {
		return m_TrackedControllerRole;
	}

	// Will return true based on whether a TrackedDeviceIndex was assigned during Activate()
	bool ITrackableDevice::IsActivated() const {
		return m_unSteamVRTrackedDeviceId != vr::k_unTrackedDeviceIndexInvalid;
	}

	// Updates the tracked device through OpenVR's IVRDriverInput from its current state.
	void ITrackableDevice::Update() {
		// Touchpad
		if (m_hAxes[k_eTrackpad_X]) vr::VRDriverInput()->UpdateScalarComponent(m_hAxes[k_eTrackpad_X], state.trackpad.axis.x, 0);
		if (m_hAxes[k_eTrackpad_Y]) vr::VRDriverInput()->UpdateScalarComponent(m_hAxes[k_eTrackpad_Y], state.trackpad.axis.y, 0);
		if (m_hButtons[k_eButton_Trackpad]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eButton_Trackpad], state.trackpad.isTouched, 0);
		if (m_hButtons[k_eTouch_Trackpad]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eTouch_Trackpad], (fabs(state.trackpad.axis.x) >= 0.1f || fabs(state.trackpad.axis.y) >= 0.1f), 0);

		// Trigger
		if (m_hAxes[k_eValue_Trigger]) vr::VRDriverInput()->UpdateScalarComponent(m_hAxes[k_eValue_Trigger], state.trigger.value, 0);
		if (m_hButtons[k_eTouch_Trigger]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eTouch_Trigger], fabs(state.trigger.value) >= 0.1f, 0);
		if (m_hButtons[k_eButton_Trigger]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eButton_Trigger], state.trigger.isPressed, 0);

		// Buttons
		if (m_hButtons[k_eButton_Grip]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eButton_Grip], state.grip.isPressed, 0);
		if (m_hButtons[k_eButton_Application]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eButton_Application], state.application.isPressed, 0);
		if (m_hButtons[k_eButton_System]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eButton_System], state.system.isPressed, 0);
		if (m_hButtons[k_eButton_Back]) vr::VRDriverInput()->UpdateBooleanComponent(m_hButtons[k_eButton_Back], state.system.isPressed, 0);
	}

	void ITrackableDevice::RefreshWorldFromDriverPose() {
		steamvrbridge::Logger::Info("Begin CServerDriver_PSMoveService::RefreshWorldFromDriverPose() for device %s\n", GetSteamVRIdentifier());

		const PSMPosef worldFromDriverPose = steamvrbridge::g_ServerTrackedDeviceProvider.GetWorldFromDriverPose();

		steamvrbridge::Logger::Info("worldFromDriverPose: %s \n", steamvrbridge::Utils::PSMPosefToString(worldFromDriverPose).c_str());

		// Transform used to convert from PSMove Tracking space to OpenVR Tracking Space
		m_Pose.qWorldFromDriverRotation.w = worldFromDriverPose.Orientation.w;
		m_Pose.qWorldFromDriverRotation.x = worldFromDriverPose.Orientation.x;
		m_Pose.qWorldFromDriverRotation.y = worldFromDriverPose.Orientation.y;
		m_Pose.qWorldFromDriverRotation.z = worldFromDriverPose.Orientation.z;
		m_Pose.vecWorldFromDriverTranslation[0] = worldFromDriverPose.Position.x;
		m_Pose.vecWorldFromDriverTranslation[1] = worldFromDriverPose.Position.y;
		m_Pose.vecWorldFromDriverTranslation[2] = worldFromDriverPose.Position.z;
	}

	PSMPosef ITrackableDevice::GetWorldFromDriverPose() {
		PSMVector3f psmToOpenVRTranslation = {
			(float)m_Pose.vecWorldFromDriverTranslation[0],
			(float)m_Pose.vecWorldFromDriverTranslation[1],
			(float)m_Pose.vecWorldFromDriverTranslation[2] };
		PSMQuatf psmToOpenVRRotation = PSM_QuatfCreate(
			(float)m_Pose.qWorldFromDriverRotation.w,
			(float)m_Pose.qWorldFromDriverRotation.x,
			(float)m_Pose.qWorldFromDriverRotation.y,
			(float)m_Pose.qWorldFromDriverRotation.x);
		PSMPosef psmToOpenVRPose = PSM_PosefCreate(&psmToOpenVRTranslation, &psmToOpenVRRotation);

		return psmToOpenVRPose;
	}

	const char *ITrackableDevice::GetSteamVRIdentifier() const {
		return m_strSteamVRSerialNo.c_str();
	}

	const vr::TrackedDeviceIndex_t ITrackableDevice::getTrackedDeviceIndex() {
		return m_unSteamVRTrackedDeviceId;
	}
}