#pragma once
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>
#include <map>

namespace steamvrbridge {

	/* A device tracked by OpenVR through its ITrackedDeviceServerDriver interface definition. It implements
	the required features of an ITrackedDeviceServerDriver and also defines a standard interface for how this
	class should be used.*/
	class TrackableDevice : public vr::ITrackedDeviceServerDriver {
	public:

		enum {
			k_eButton_Trackpad,
			k_eButton_Trigger,
			k_eButton_Grip,
			k_eButton_Application,
			k_eButton_System,
			k_eButton_Guide,
			k_eButton_Back,
			k_eValue_Trigger,
			k_eTrackpad_X,
			k_eTrackpad_Y,
			k_eTouch_Trackpad,
			k_eTouch_Trigger
		} DeviceInput;

		enum eVRTouchpadDirection {
			k_EVRTouchpadDirection_None,

			k_EVRTouchpadDirection_Left,
			k_EVRTouchpadDirection_Up,
			k_EVRTouchpadDirection_Right,
			k_EVRTouchpadDirection_Down,

			k_EVRTouchpadDirection_UpLeft,
			k_EVRTouchpadDirection_UpRight,
			k_EVRTouchpadDirection_DownLeft,
			k_EVRTouchpadDirection_DownRight,

			k_EVRTouchpadDirection_Count
		};

		/* Definition of a Tracked Device's State */
		struct Axis {
			float x = 0.0f;
			float y = 0.0f;
		};

		struct Button {
			//Button(bool pressedState) { isPressed = pressedState; }
			bool isPressed = false;
		};

		struct Trigger {
			// trigger state
			bool isPressed = false;
			bool isTouched = false;
			float value = 0.0f;
		};

		struct TrackPad {
			// trackpad state
			bool isPressed = false;
			bool isTouched = false;
			Axis axis;
		};

		struct State {
			Button grip;
			Button application;
			Button system;
			Button guide;
			Button back;
			Trigger trigger;
			TrackPad trackpad;
		};

		/* The following represent the input paths that physical hardware
		can be mapped to in the OpenVR IVRDriverInput interface */
		const char *k_pch_Trackpad = "/input/trackpad/click";
		const char *k_pch_Trackpad_Touch = "/input/trackpad/touch";
		const char *k_pch_Trackpad_X = "/input/trackpad/x";
		const char *k_pch_Trackpad_Y = "/input/trackpad/y";
		const char *k_pch_Trackpad_Value = "/input/trigger/value";
		const char *k_pch_Haptic = "/output/haptic";
		const char *k_pch_Trigger = "/input/trigger/click";
		const char *k_pch_Trigger_Touch = "/input/trigger/touch";
		const char *k_pch_Application = "/input/application_menu/click";
		const char *k_pch_Back = "/input/back/click";
		const char *k_pch_Guide = "/input/guide/click";
		const char *k_pch_Grip = "/input/grip/click";
		const char *k_pch_System = "/input/system/click";

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

		// State of all buttons/touches/triggers
		State state;

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

		// Component handle registered upon Activate() and called to update button/touch/axis/haptic events
		std::map<int, vr::VRInputComponentHandle_t> m_hButtons;
		std::map<int, vr::VRInputComponentHandle_t> m_hAxes;
		vr::VRInputComponentHandle_t m_ulHapticComponent;
	};
}