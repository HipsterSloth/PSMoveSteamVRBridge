#ifndef _OPENVR_CONTROLLER_COMPONENT_H
#define _OPENVR_CONTROLLER_COMPONENT_H

namespace vr
{
	// ----------------------------------------------------------------------------------------------
	// Purpose: Controller access on a single tracked device.
	// ----------------------------------------------------------------------------------------------
	class IVRControllerComponent
	{
	public:
		// ------------------------------------
		// Controller Methods
		// ------------------------------------

		/** Gets the current state of a controller. */
		virtual VRControllerState_t GetControllerState() = 0;

		/** Returns a uint64 property. If the property is not available this function will return 0. */
		virtual bool TriggerHapticPulse(uint32_t unAxisId, uint16_t usPulseDurationMicroseconds) = 0;
	};

	static const char *IVRControllerComponent_Version = "IVRControllerComponent_001";

	/** This interface is provided by vrserver to allow the driver to notify
	* the system when something changes about a device. These changes must
	* not change the serial number or class of the device because those values
	* are permanently associated with the device's index. */
	class IVRServerDriverHost
	{
	public:
		/** Notifies the server that a tracked device has been added. If this function returns true
		* the server will call Activate on the device. If it returns false some kind of error
		* has occurred and the device will not be activated. */
		virtual bool TrackedDeviceAdded(const char *pchDeviceSerialNumber, ETrackedDeviceClass eDeviceClass, ITrackedDeviceServerDriver *pDriver) = 0;

		/** Notifies the server that a tracked device's pose has been updated */
		virtual void TrackedDevicePoseUpdated(uint32_t unWhichDevice, const DriverPose_t & newPose, uint32_t unPoseStructSize) = 0;

		/** Notifies the server that vsync has occurred on the the display attached to the device. This is
		* only permitted on devices of the HMD class. */
		virtual void VsyncEvent(double vsyncTimeOffsetSeconds) = 0;

		/** notifies the server that the button was pressed */
		virtual void TrackedDeviceButtonPressed(uint32_t unWhichDevice, EVRButtonId eButtonId, double eventTimeOffset) = 0;

		/** notifies the server that the button was unpressed */
		virtual void TrackedDeviceButtonUnpressed(uint32_t unWhichDevice, EVRButtonId eButtonId, double eventTimeOffset) = 0;

		/** notifies the server that the button was pressed */
		virtual void TrackedDeviceButtonTouched(uint32_t unWhichDevice, EVRButtonId eButtonId, double eventTimeOffset) = 0;

		/** notifies the server that the button was unpressed */
		virtual void TrackedDeviceButtonUntouched(uint32_t unWhichDevice, EVRButtonId eButtonId, double eventTimeOffset) = 0;

		/** notifies the server than a controller axis changed */
		virtual void TrackedDeviceAxisUpdated(uint32_t unWhichDevice, uint32_t unWhichAxis, const VRControllerAxis_t & axisState) = 0;

		/** Notifies the server that the proximity sensor on the specified device  */
		virtual void ProximitySensorState(uint32_t unWhichDevice, bool bProximitySensorTriggered) = 0;

		/** Sends a vendor specific event (VREvent_VendorSpecific_Reserved_Start..VREvent_VendorSpecific_Reserved_End */
		virtual void VendorSpecificEvent(uint32_t unWhichDevice, vr::EVREventType eventType, const VREvent_Data_t & eventData, double eventTimeOffset) = 0;

		/** Returns true if SteamVR is exiting */
		virtual bool IsExiting() = 0;

		/** Returns true and fills the event with the next event on the queue if there is one. If there are no events
		* this method returns false. uncbVREvent should be the size in bytes of the VREvent_t struct */
		virtual bool PollNextEvent(VREvent_t *pEvent, uint32_t uncbVREvent) = 0;

		/** Provides access to device poses for drivers.  Poses are in their "raw" tracking space which is uniquely
		* defined by each driver providing poses for its devices.  It is up to clients of this function to correlate
		* poses across different drivers.  Poses are indexed by their device id, and their associated driver and
		* other properties can be looked up via IVRProperties. */
		virtual void GetRawTrackedDevicePoses(float fPredictedSecondsFromNow, TrackedDevicePose_t *pTrackedDevicePoseArray, uint32_t unTrackedDevicePoseArrayCount) = 0;

		/** Notifies the server that a tracked device's display component transforms have been updated. */
		virtual void TrackedDeviceDisplayTransformUpdated(uint32_t unWhichDevice, HmdMatrix34_t eyeToHeadLeft, HmdMatrix34_t eyeToHeadRight) = 0;
	};

	static const char *IVRServerDriverHost_Version = "IVRServerDriverHost_004";

	inline IVRServerDriverHost *VR_CALLTYPE VRServerDriverHost_004() { return (IVRServerDriverHost*)OpenVRInternal_ModuleServerDriverContext().VRServerDriverHost(); }
}

#endif // _OPENVR_CONTROLLER_COMPONENT_H