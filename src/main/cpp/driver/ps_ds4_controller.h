#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {

	/* An un-tracked PSNavi controller.
	   The controller class bridges the PSMoveService controller to OpenVR's tracked device.*/
	class PSDualshock4Controller : public Controller {

	public:
		// Constructor/Destructor
		PSDualshock4Controller(PSMControllerID psmControllerID, vr::ETrackedControllerRole trackedControllerRole, const char *psmSerialNo);
		virtual ~PSDualshock4Controller();

		// Overridden Implementation of vr::ITrackedDeviceServerDriver
		vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
		void Deactivate() override;

		// TrackableDevice interface implementation
		void LoadSettings(vr::IVRSettings *pSettings) override;
		vr::ETrackedDeviceClass GetTrackedDeviceClass() const override { return vr::TrackedDeviceClass_Controller; }
		void Update() override;
		void RefreshWorldFromDriverPose() override;

		// IController interface implementation
		const char *GetControllerSettingsPrefix() const override { return "ds4"; }
		bool HasPSMControllerId(int ControllerID) const override { return ControllerID == m_nPSMControllerId; }
		const PSMController * GetPSMControllerView() const override { return m_PSMServiceController; }
		std::string GetPSMControllerSerialNo() const override { return m_strPSMControllerSerialNo; }
		PSMControllerType GetPSMControllerType() const override { return PSMController_Virtual; }

	private:
		void RemapThumbstick(
			const float thumb_stick_x, const float thumb_stick_y,
			float &out_sanitized_x, float &out_sanitized_y);
		void UpdateThumbsticks();
		void UpdateEmulatedTrackpad();
		void UpdateControllerState();
		void UpdateTrackingState();
		void UpdateRumbleState(PSMControllerRumbleChannel channel);

		// Parent controller to send button events to
		Controller *m_parentController;

		// Controller State
		int m_nPSMControllerId;
		PSMController *m_PSMServiceController;
		std::string m_strPSMControllerSerialNo;

		// Used to report the controllers calibration status
		vr::ETrackingResult m_trackingStatus;

		// Used to ignore old state from PSM Service
		int m_nPoseSequenceNumber;

		// Rumble state
		bool m_bRumbleSuppressed;

		//virtual extend controller in meters
		float m_fVirtuallExtendControllersYMeters;
		float m_fVirtuallExtendControllersZMeters;

		// virtually rotate controller
		bool m_fVirtuallyRotateController;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTouchpadPressTime;
		bool m_touchpadDirectionsUsed;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetPoseButtonPressTime;
		bool m_bResetPoseRequestSent;
		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetAlignButtonPressTime;
		bool m_bResetAlignRequestSent;

		// Settings value: used to determine how many meters in front of the HMD the controller
		// is held when it's being calibrated.
		float m_fControllerMetersInFrontOfHmdAtCalibration;

		// Flag used to completely disable the alignment gesture
		bool m_bDisableHMDAlignmentGesture;

		// Flag to tell if we should use the controller orientation as part of the controller alignment
		bool m_bUseControllerOrientationInHMDAlignment;

		// The last normalized thumbstick values (post dead zone application);
		float m_lastSanitizedLeftThumbstick_X;
		float m_lastSanitizedLeftThumbstick_Y;
		float m_lastSanitizedRightThumbstick_X;
		float m_lastSanitizedRightThumbstick_Y;

		// The size of the deadzone for the controller's thumbstick
		float m_thumbstickDeadzone;

		// Settings values. Used to adjust throwing power using linear velocity and acceleration.
		float m_fLinearVelocityMultiplier;
		float m_fLinearVelocityExponent;

		// The button to use for controller hmd alignment
		ePSMButtonID m_hmdAlignPSButtonID;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}