#pragma once
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>
#include <map>

//-- pre-declarations -----
//class CPSMoveTrackedDeviceLatest;

class CPSMoveTrackedDeviceLatest : public vr::ITrackedDeviceServerDriver
{
public:
	CPSMoveTrackedDeviceLatest();
	virtual ~CPSMoveTrackedDeviceLatest();

	// Shared Implementation of vr::ITrackedDeviceServerDriver
	virtual vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
	virtual void Deactivate() override;
	virtual void EnterStandby() override;
	virtual void *GetComponent(const char *pchComponentNameAndVersion) override;
	virtual void DebugRequest(const char * pchRequest, char * pchResponseBuffer, uint32_t unResponseBufferSize) override;
	virtual vr::DriverPose_t GetPose() override;

	// CPSMoveTrackedDeviceLatest Interface
	virtual vr::ETrackedDeviceClass GetTrackedDeviceClass() const;
	virtual bool IsActivated() const;
	virtual void Update();
	virtual void RefreshWorldFromDriverPose();
	PSMPosef GetWorldFromDriverPose();
	virtual const char *GetSteamVRIdentifier() const;

protected:
	vr::PropertyContainerHandle_t m_ulPropertyContainer;

	// Tracked device identification
	std::string m_strSteamVRSerialNo;

	// Assigned by vrserver upon Activate().  The same ID visible to clients
	vr::TrackedDeviceIndex_t m_unSteamVRTrackedDeviceId;

	// Cached for answering version queries from vrserver
	vr::DriverPose_t m_Pose;
	unsigned short m_firmware_revision;
	unsigned short m_hardware_revision;

	// Component handle registered upon Activate() and called to update button/trigger/axis events
	std::map<int, vr::VRInputComponentHandle_t> m_ulBoolComponentsMap;
	std::map<int, vr::VRInputComponentHandle_t> m_ulScalarComponentsMap;
	vr::VRInputComponentHandle_t m_ulHapticComponent;

};