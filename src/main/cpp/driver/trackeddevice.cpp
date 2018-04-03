#include "trackeddevice.h"
#include "utils.h"
#include "logger.h"
#include "driver.h"


CPSMoveTrackedDeviceLatest::CPSMoveTrackedDeviceLatest()
	: m_ulPropertyContainer(vr::k_ulInvalidPropertyContainer)
	, m_unSteamVRTrackedDeviceId(vr::k_unTrackedDeviceIndexInvalid)
{
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

CPSMoveTrackedDeviceLatest::~CPSMoveTrackedDeviceLatest()
{
}

// Shared Implementation of vr::ITrackedDeviceServerDriver
vr::EVRInitError CPSMoveTrackedDeviceLatest::Activate(vr::TrackedDeviceIndex_t unObjectId)
{
	vr::CVRPropertyHelpers *properties = vr::VRProperties();

	steamvrbridge::Logger::DriverLog("CPSMoveTrackedDeviceLatest::Activate: %s is object id %d\n", GetSteamVRIdentifier(), unObjectId);
	m_ulPropertyContainer = properties->TrackedDeviceToPropertyContainer(unObjectId);
	m_unSteamVRTrackedDeviceId = unObjectId;

	properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_Firmware_UpdateAvailable_Bool, false);
	properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_Firmware_ManualUpdate_Bool, false);
	properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_ContainsProximitySensor_Bool, false);
	properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_HasCamera_Bool, false);
	properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_Firmware_ForceUpdateRequired_Bool, false);
	properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceCanPowerOff_Bool, false);
	properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_HardwareRevision_Uint64, m_hardware_revision);
	properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_FirmwareVersion_Uint64, m_firmware_revision);

	vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_InputProfilePath_String, "some_deliberately_incorrect_path.json");

	// Register all available hardware components of the PSMove controller, i.e. physical buttons
	vr::VRInputComponentHandle_t m_ulComponent;

	// System button component
	vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, "/input/system/click", &m_ulComponent);
	m_ulBoolComponentsMap[vr::EVRButtonId::k_EButton_System] = m_ulComponent;

	// Grip button component
	vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, "/input/grip/click", &m_ulComponent);
	m_ulBoolComponentsMap[vr::EVRButtonId::k_EButton_Grip] = m_ulComponent;

	// Guide button component
	//vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, "/input/guide/click", &m_ulComponent);
	//m_ulComponentsMap[vr::EVRButtonId::k_EButton_] = m_ulComponent;

	// Back button component
	vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, "/input/back/click", &m_ulComponent);
	m_ulBoolComponentsMap[vr::EVRButtonId::k_EButton_Dashboard_Back] = m_ulComponent;

	// Trigger button component
	vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, "/input/trigger/click", &m_ulComponent);
	m_ulBoolComponentsMap[vr::EVRButtonId::k_EButton_SteamVR_Trigger] = m_ulComponent;

	// Trigger value component
	vr::VRDriverInput()->CreateScalarComponent(m_ulPropertyContainer, "/input/trigger/value", &m_ulComponent, vr::VRScalarType_Absolute, vr::VRScalarUnits_NormalizedOneSided);
	m_ulScalarComponentsMap[vr::EVRButtonId::k_EButton_SteamVR_Trigger] = m_ulComponent;

	// Application Menu button component
	vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, "/input/application_menu/click", &m_ulComponent);
	m_ulBoolComponentsMap[vr::EVRButtonId::k_EButton_ApplicationMenu] = m_ulComponent;

	// Create Trackpad Click button component
	vr::VRDriverInput()->CreateBooleanComponent(m_ulPropertyContainer, "/input/trackpad/click", &m_ulComponent);
	m_ulBoolComponentsMap[vr::EVRButtonId::k_EButton_SteamVR_Touchpad] = m_ulComponent;

	// Create Trackpad Axis X button component
	vr::VRDriverInput()->CreateScalarComponent(m_ulPropertyContainer, "/input/trackpad/x", &m_ulComponent, vr::VRScalarType_Absolute, vr::VRScalarUnits_NormalizedTwoSided);
	m_ulScalarComponentsMap[vr::EVRButtonId::k_EButton_Axis2] = m_ulComponent;

	// Createt Trackpad Axis Y button component
	vr::VRDriverInput()->CreateScalarComponent(m_ulPropertyContainer, "/input/trackpad/y", &m_ulComponent, vr::VRScalarType_Absolute, vr::VRScalarUnits_NormalizedTwoSided);
	m_ulScalarComponentsMap[vr::EVRButtonId::k_EButton_Axis3] = m_ulComponent;

	// Create Haptic feedback component
	// Unfortunately we don't have access to the next polled event coming from SteamVR from here,
	// it does appear to be checked in monitor_psmoveservice.cpp on a background thread though but
	// this doesn't allow a simple surgical fix. Will need to restructure the lifecycle of this driver...
	vr::VRDriverInput()->CreateHapticComponent(m_ulPropertyContainer, "/output/haptic", &m_ulHapticComponent);

	return vr::VRInitError_None;
}

void CPSMoveTrackedDeviceLatest::Deactivate()
{
	steamvrbridge::Logger::DriverLog("CPSMoveTrackedDeviceLatest::Deactivate: %s was object id %d\n", GetSteamVRIdentifier(), m_unSteamVRTrackedDeviceId);
	m_unSteamVRTrackedDeviceId = vr::k_unTrackedDeviceIndexInvalid;
}

void CPSMoveTrackedDeviceLatest::EnterStandby()
{
	//###HipsterSloth $TODO - No good way to do this at the moment
}

void *CPSMoveTrackedDeviceLatest::GetComponent(const char *pchComponentNameAndVersion)
{
	return NULL;
}

void CPSMoveTrackedDeviceLatest::DebugRequest(const char * pchRequest, char * pchResponseBuffer, uint32_t unResponseBufferSize)
{

}

vr::DriverPose_t CPSMoveTrackedDeviceLatest::GetPose()
{
	// This is only called at startup to synchronize with the driver.
	// Future updates are driven by our thread calling TrackedDevicePoseUpdated()
	return m_Pose;
}

// CPSMoveTrackedDeviceLatest Interface
vr::ETrackedDeviceClass CPSMoveTrackedDeviceLatest::GetTrackedDeviceClass() const
{
	return vr::TrackedDeviceClass_Invalid;
}

bool CPSMoveTrackedDeviceLatest::IsActivated() const
{
	return m_unSteamVRTrackedDeviceId != vr::k_unTrackedDeviceIndexInvalid;
}

void CPSMoveTrackedDeviceLatest::Update()
{
}

void CPSMoveTrackedDeviceLatest::RefreshWorldFromDriverPose()
{
	steamvrbridge::Logger::DriverLog("Begin CServerDriver_PSMoveService::RefreshWorldFromDriverPose() for device %s\n", GetSteamVRIdentifier());

	const PSMPosef worldFromDriverPose = steamvrbridge::g_ServerTrackedDeviceProvider.GetWorldFromDriverPose();

	steamvrbridge::Logger::DriverLog("worldFromDriverPose: %s \n", steamvrbridge::Utils::PSMPosefToString(worldFromDriverPose).c_str());

	// Transform used to convert from PSMove Tracking space to OpenVR Tracking Space
	m_Pose.qWorldFromDriverRotation.w = worldFromDriverPose.Orientation.w;
	m_Pose.qWorldFromDriverRotation.x = worldFromDriverPose.Orientation.x;
	m_Pose.qWorldFromDriverRotation.y = worldFromDriverPose.Orientation.y;
	m_Pose.qWorldFromDriverRotation.z = worldFromDriverPose.Orientation.z;
	m_Pose.vecWorldFromDriverTranslation[0] = worldFromDriverPose.Position.x;
	m_Pose.vecWorldFromDriverTranslation[1] = worldFromDriverPose.Position.y;
	m_Pose.vecWorldFromDriverTranslation[2] = worldFromDriverPose.Position.z;
}

PSMPosef CPSMoveTrackedDeviceLatest::GetWorldFromDriverPose()
{
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

const char *CPSMoveTrackedDeviceLatest::GetSteamVRIdentifier() const
{
	return m_strSteamVRSerialNo.c_str();
}