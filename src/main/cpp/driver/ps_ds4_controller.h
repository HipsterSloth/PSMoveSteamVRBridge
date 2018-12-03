#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {

	class PSDualshock4ControllerConfig : public ControllerConfig
	{
	public:
		static const int CONFIG_VERSION;

		PSDualshock4ControllerConfig(const std::string &fnamebase = "PSDualshock4ControllerConfig")
			: ControllerConfig(fnamebase)
			, rumble_suppressed(false)
			, extend_Y_meters(0.f)
			, extend_Z_meters(0.f)
			, z_rotate_90_degrees(false)
			, calibration_offset_meters(0.f)
			, disable_alignment_gesture(false)
			, use_orientation_in_hmd_alignment(true)
			, thumbstick_deadzone(k_defaultThumbstickDeadZoneRadius)
			, linear_velocity_multiplier(1.f)
			, linear_velocity_exponent(0.f)
		{
		};

		configuru::Config WriteToJSON() override;
		bool ReadFromJSON(const configuru::Config &pt) override;

		// Rumble state
		bool rumble_suppressed;

		// Virtual extend controller in meters.
		float extend_Y_meters;
		float extend_Z_meters;

		// Rotate controllers orientation 90 degrees about the z-axis (for gun style games).
		bool z_rotate_90_degrees;

		// Settings value: used to determine how many meters in front of the HMD the controller
		// is held when it's being calibrated.
		float calibration_offset_meters;

		// Flag used to completely disable the alignment gesture.
		bool disable_alignment_gesture;

		// Flag to tell if we should use the controller orientation as part of the controller alignment.
		bool use_orientation_in_hmd_alignment;

		// The inner deadzone of the thumbsticks
		float thumbstick_deadzone;

		// Settings values. Used to adjust throwing power using linear velocity and acceleration.
		float linear_velocity_multiplier;
		float linear_velocity_exponent;
	};

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
		vr::ETrackedDeviceClass GetTrackedDeviceClass() const override { return vr::TrackedDeviceClass_Controller; }
		void Update() override;
		void RefreshWorldFromDriverPose() override;

		// IController interface implementation
		const char *GetControllerSettingsPrefix() const override { return "playstation_dualshock4"; }
		bool HasPSMControllerId(int ControllerID) const override { return ControllerID == m_nPSMControllerId; }
		const PSMController * GetPSMControllerView() const override { return m_PSMServiceController; }
		std::string GetPSMControllerSerialNo() const override { return m_strPSMControllerSerialNo; }
		PSMControllerType GetPSMControllerType() const override { return PSMController_Virtual; }

	protected:
		const PSDualshock4ControllerConfig *getConfig() const { return static_cast<const PSDualshock4ControllerConfig *>(m_config); }
		ControllerConfig *AllocateControllerConfig() override { 
			std::string fnamebase= std::string("ds4_") + m_strPSMControllerSerialNo;
			return new PSDualshock4ControllerConfig(fnamebase); 
		}

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

		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTouchpadPressTime;
		bool m_touchpadDirectionsUsed;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetPoseButtonPressTime;
		bool m_bResetPoseRequestSent;
		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetAlignButtonPressTime;
		bool m_bResetAlignRequestSent;

		// Flag to tell if we should use the controller orientation as part of the controller alignment
		bool m_bUseControllerOrientationInHMDAlignment;

		// The last normalized thumbstick values (post dead zone application);
		float m_lastSanitizedLeftThumbstick_X;
		float m_lastSanitizedLeftThumbstick_Y;
		float m_lastSanitizedRightThumbstick_X;
		float m_lastSanitizedRightThumbstick_Y;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}