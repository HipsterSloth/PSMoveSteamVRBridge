#pragma once
#include "PSMoveClient_CAPI.h"
#include "trackabledevice.h"

namespace steamvrbridge {

	// Represents a PSMoveService Tracker such as a PSEye camera. Implements an OpenVR TrackedDevice
	class PSMServiceTracker : public TrackableDevice
	{
	public:
		PSMServiceTracker(const PSMClientTrackerInfo *trackerInfo);
		virtual ~PSMServiceTracker();

		// Overridden Implementation of vr::ITrackedDeviceServerDriver
		virtual vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
		virtual void Deactivate() override;

		// Overridden Implementation of CPSMoveTrackedDeviceLatest
		virtual vr::ETrackedDeviceClass GetTrackedDeviceClass() const override { return vr::TrackedDeviceClass_TrackingReference; }
		virtual void Update() override;

		bool HasTrackerId(int ControllerID);
		void SetClientTrackerInfo(const PSMClientTrackerInfo *trackerInfo);

	private:
		// Which tracker
		int m_nTrackerId;

		// The static information about this tracker
		PSMClientTrackerInfo m_tracker_info;
	};
}