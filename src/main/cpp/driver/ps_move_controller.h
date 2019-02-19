#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>
#include <chrono>

namespace steamvrbridge {

	class PSMoveControllerConfig : public ControllerConfig
	{
	public:
		PSMoveControllerConfig(class PSMoveController *ownerController, const std::string &fnamebase = "PSMoveControllerConfig");

        Config *Clone() override { return new PSMoveControllerConfig(*this); }
        void OnConfigChanged(Config *newConfig) override;
		bool ReadFromJSON(const configuru::Config &pt) override;

		// Rumble state
		bool rumble_suppressed;

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

		// Settings values. Used to adjust throwing power using linear velocity and acceleration.
		float linear_velocity_multiplier;
		float linear_velocity_exponent;
	};

	/* A trackable PS Move controller. The controller class bridges the PSMoveService controller to
	OpenVR's tracked device.*/
	class PSMoveController : public Controller {

	public:

		// Constructor/Destructor
		PSMoveController(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const char *psmSerialNo);
		virtual ~PSMoveController();

		// Overridden Implementation of vr::ITrackedDeviceServerDriver
		vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
		void Deactivate() override;

		// TrackableDevice interface implementation
		vr::ETrackedDeviceClass GetTrackedDeviceClass() const override { return vr::TrackedDeviceClass_Controller; }
		void Update() override;
		void RefreshWorldFromDriverPose() override;

		// IController interface implementation
        void OnControllerModelChanged() override;
		const char *GetControllerSettingsPrefix() const override { return "playstation_move"; }
		bool HasPSMControllerId(int ControllerID) const override { return ControllerID == m_nPSMControllerId; }
		const PSMController * GetPSMControllerView() const override { return m_PSMServiceController; }
		std::string GetPSMControllerSerialNo() const override { return m_strPSMControllerSerialNo; }
		PSMControllerType GetPSMControllerType() const override { return PSMController_Move; }

	protected:
		const PSMoveControllerConfig *getConfig() const { return static_cast<const PSMoveControllerConfig *>(m_config); }
		ControllerConfig *AllocateControllerConfig() override { 
			std::string fnamebase= std::string("psmove_") + m_strPSMControllerSerialNo;
			return new PSMoveControllerConfig(this, fnamebase); 
		}

	private:
		void UpdateEmulatedTrackpad();
		void UpdateBatteryChargeState(PSMBatteryState newBatteryEnum);
		void UpdateControllerState();
		void UpdateTrackingState();
		void UpdateRumbleState();

		// Controller State
		int m_nPSMControllerId;
		PSMController *m_PSMServiceController;
		std::string m_strPSMControllerSerialNo;

		// Used to report the controllers calibration status
		vr::ETrackingResult m_trackingStatus;

		// Used to ignore old state from PSM Service
		int m_nPoseSequenceNumber;

		// Cached for answering version queries from vrserver
		bool m_bIsBatteryCharging;
		float m_fBatteryChargeFraction;

		// True while the touchpad is considered active (touched or pressed) 
		// after the initial touchpad delay, if any.
		bool m_bTouchpadWasActive;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTouchpadPressTime;

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

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}