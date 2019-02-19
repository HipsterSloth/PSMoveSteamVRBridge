#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {
	class VirtualControllerConfig : public ControllerConfig
	{
	public:
		VirtualControllerConfig(class VirtualController *ownerController, const std::string &fnamebase = "VirtualControllerConfig");

        Config *Clone() override { return new VirtualControllerConfig(*this); }
        void OnConfigChanged(Config *newConfig) override;
		bool ReadFromJSON(const configuru::Config &pt) override;

		// Virtual extend controller in meters.
		float extend_Y_meters;
		float extend_Z_meters;

		// Rotate controllers orientation 90 degrees about the z-axis (for gun style games).
		bool z_rotate_90_degrees;

		// Delay in resetting touchpad position after touchpad press.
		bool delay_after_touchpad_press;

		// Settings values. Used to determine whether we'll map controller movement after touchpad
		// presses to touchpad axis values.
		float meters_per_touchpad_axis_units;

		// The axis to use for trigger input
		int steamvr_trigger_axis_index;

		// The axes to use for touchpad input (virtual controller only)
		int virtual_touchpad_XAxis_index;
		int virtual_touchpad_YAxis_index;

		// The size of the deadzone for the controller's thumbstick
		float thumbstick_deadzone;

		// Treat a thumbstick touch also as a press
		bool thumbstick_touch_as_press;

		// Settings values. Used to adjust throwing power using linear velocity and acceleration.
		float linear_velocity_multiplier;
		float linear_velocity_exponent;

		// The button to use as the system button
		ePSMButtonID system_button_id;
	};

	/* A trackable Virtual controller (tracking bulb + game pad).
	   The controller class bridges the PSMoveService controller to OpenVR's tracked device.*/
	class VirtualController : public Controller {

	public:
		// Constructor/Destructor
		VirtualController(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const char *psmSerialNo);
		virtual ~VirtualController();

		// Overridden Implementation of vr::ITrackedDeviceServerDriver
		vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
		void Deactivate() override;

		// TrackableDevice interface implementation
		vr::ETrackedDeviceClass GetTrackedDeviceClass() const override { return vr::TrackedDeviceClass_Controller; }
		void Update() override;
		void RefreshWorldFromDriverPose() override;

		// IController interface implementation
        void OnControllerModelChanged() override;
		const char *GetControllerSettingsPrefix() const override { return "virtual_controller"; }
		bool HasPSMControllerId(int ControllerID) const override { return ControllerID == m_nPSMControllerId; }
		const PSMController * GetPSMControllerView() const override { return m_PSMServiceController; }
		std::string GetPSMControllerSerialNo() const override { return m_strPSMControllerSerialNo; }
		PSMControllerType GetPSMControllerType() const override { return PSMController_Virtual; }

	protected:
		const VirtualControllerConfig *getConfig() const { return static_cast<const VirtualControllerConfig *>(m_config); }
		ControllerConfig *AllocateControllerConfig() override { 
			std::string fnamebase= std::string("virtual_controller_") + m_strPSMControllerSerialNo;
			return new VirtualControllerConfig(this, fnamebase); 
		}

	private:
		void UpdateEmulatedTrackpad();
		void UpdateControllerState();
		void UpdateTrackingState();

		// Controller State
		int m_nPSMControllerId;
		PSMController *m_PSMServiceController;
		std::string m_strPSMControllerSerialNo;

		// Used to report the controllers calibration status
		vr::ETrackingResult m_trackingStatus;

		// Used to ignore old state from PSM Service
		int m_nPoseSequenceNumber;

		// True while the touchpad is considered active (touched or pressed) 
		// after the initial touchpad delay, if any.
		bool m_bTouchpadWasActive;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTouchpadPressTime;
		bool m_touchpadDirectionsUsed;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetPoseButtonPressTime;
		bool m_bResetPoseRequestSent;
		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetAlignButtonPressTime;
		bool m_bResetAlignRequestSent;

		// The position of the controller in meters in driver space relative to its own rotation
		// at the time when the touchpad was most recently pressed (after being up).
		PSMVector3f m_posMetersAtTouchpadPressTime;

		// The orientation of the controller in driver space at the time when
		// the touchpad was most recently pressed (after being up).
		PSMQuatf m_driverSpaceRotationAtTouchpadPressTime;

		// Optional solver used to determine hand orientation.
		class IHandOrientationSolver *m_orientationSolver;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}