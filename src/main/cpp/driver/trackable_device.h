#pragma once
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>
#include <configuru.hpp>

namespace steamvrbridge {

	/* A device tracked by OpenVR through its ITrackedDeviceServerDriver interface definition. It implements
	the required features of an ITrackedDeviceServerDriver and also defines a standard interface for how this
	class should be used.*/
	class TrackableDevice : public vr::ITrackedDeviceServerDriver {
	public:

		TrackableDevice();
		virtual ~TrackableDevice();

		// Shared Implementation of vr::ITrackedDeviceServerDriver
		virtual vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
		virtual void Deactivate() override;
		virtual void EnterStandby() override;
		virtual void *GetComponent(const char *pchComponentNameAndVersion) override;
		virtual void DebugRequest(const char * pchRequest, char * pchResponseBuffer, uint32_t unResponseBufferSize) override;
		virtual vr::DriverPose_t GetPose() override;

		// TrackedDevice Interface
		virtual vr::ETrackedDeviceClass GetTrackedDeviceClass() const;
		virtual vr::ETrackedControllerRole GetTrackedDeviceRole() const;
		virtual bool IsActivated() const;
		virtual void Update();
		virtual void RefreshWorldFromDriverPose();
		PSMPosef GetWorldFromDriverPose();
		virtual const char *GetSteamVRIdentifier() const;
		virtual const vr::TrackedDeviceIndex_t getTrackedDeviceIndex();
		inline vr::PropertyContainerHandle_t getPropertyContainerHandle() const { return m_ulPropertyContainer; }

	protected:
		// OpenVR Properties
		vr::PropertyContainerHandle_t m_ulPropertyContainer;

		// Tracked device identification
		std::string m_strSteamVRSerialNo;

		// Tracked device role
		vr::ETrackedControllerRole m_TrackedControllerRole;

		// Assigned by vrserver upon Activate().  The same ID visible to clients
		vr::TrackedDeviceIndex_t m_unSteamVRTrackedDeviceId;

		// Cached for answering version queries from vrserver
		vr::DriverPose_t m_Pose;
		unsigned short m_firmware_revision;
		unsigned short m_hardware_revision;
	};
}