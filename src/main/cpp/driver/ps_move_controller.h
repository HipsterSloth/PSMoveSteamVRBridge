#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {

	/* A trackable PS Move controller. The controller class bridges the PSMoveService controller to
	OpenVR's tracked device.*/
	class PSMoveController : public ITrackableDevice, public IController {

	public:

		// Constructor/Destructor
		PSMoveController(PSMControllerID psmControllerID, PSMControllerType psmControllerType, vr::ETrackedControllerRole trackedControllerRole, const char *psmSerialNo);
		virtual ~PSMoveController();

		// Overridden Implementation of vr::ITrackedDeviceServerDriver
		vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
		void Deactivate() override;

		// TrackableDevice interface implementation
		vr::ETrackedDeviceClass GetTrackedDeviceClass() const override { return vr::TrackedDeviceClass_Controller; }
		void Update() override;
		void RefreshWorldFromDriverPose() override;

		// IController interface implementation
		bool HasPSMControllerId(int ControllerID) const override { return ControllerID == m_nPSMControllerId; }
		const PSMController * GetPSMControllerView() const override { return m_PSMServiceController; }
		std::string GetPSMControllerSerialNo() const override { return m_strPSMControllerSerialNo; }
		PSMControllerType GetPSMControllerType() const override { return m_PSMControllerType; }
		void SetPendingHapticVibration(vr::VREvent_HapticVibration_t hapticData) override;
		void UpdateRumbleState() override;

	private:
		// IController interface implementation
		void UpdateButtonState(ePSButtonID button, bool buttonState) override;
		void UpdateTouchPadDirection() override;
		void UpdateControllerState() override;
		void UpdateBatteryChargeState(PSMBatteryState newBatteryEnum) override;
		void SetTriggerValue(float latestTriggerValue) override;
		void UpdateTrackingState() override;

		// Controller State
		int m_nPSMControllerId;
		PSMControllerType m_PSMControllerType;
		PSMController *m_PSMServiceController;
		std::string m_strPSMControllerSerialNo;

		// Used to report the controllers calibration status
		vr::ETrackingResult m_trackingStatus;

		// Used to ignore old state from PSM Service
		int m_nPoseSequenceNumber;

		// Cached for answering version queries from vrserver
		bool m_bIsBatteryCharging;
		float m_fBatteryChargeFraction;

		// Rumble state
		bool m_bRumbleSuppressed;
		float m_pendingHapticDurationSecs;
		float m_pendingHapticAmplitude;
		float m_pendingHapticFrequency;
		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTimeRumbleSent;
		bool m_lastTimeRumbleSentValid;

		// Virtual extend controller in meters.
		float m_fVirtuallExtendControllersYMeters;
		float m_fVirtuallExtendControllersZMeters;

		// Virtually rotate controller.
		bool m_fVirtuallyRotateController;

		// Delay in resetting touchpad position after touchpad press.
		bool m_bDelayAfterTouchpadPress;

		// True while the touchpad is considered active (touched or pressed) 
		// after the initial touchpad delay, if any.
		bool m_bTouchpadWasActive;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTouchpadPressTime;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetPoseButtonPressTime;
		bool m_bResetPoseRequestSent;
		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetAlignButtonPressTime;
		bool m_bResetAlignRequestSent;

		// Button Remapping
		vr::EVRButtonId psButtonIDToVRButtonID[k_EPSButtonID_Count];
		eVRTouchpadDirection psButtonIDToVrTouchpadDirection[k_EPSButtonID_Count];
		void LoadButtonMapping(
			vr::IVRSettings *pSettings,
			const ePSButtonID psButtonID,
			const vr::EVRButtonId defaultVRButtonID,
			const eVRTouchpadDirection defaultTouchpadDirection,
			int controllerId = -1);

		// Settings values. Used to determine whether we'll map controller movement after touchpad
		// presses to touchpad axis values.
		bool m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis;
		float m_fMetersPerTouchpadAxisUnits;

		// Settings value: used to determine how many meters in front of the HMD the controller
		// is held when it's being calibrated.
		float m_fControllerMetersInFrontOfHmdAtCalibration;

		// The position of the controller in meters in driver space relative to its own rotation
		// at the time when the touchpad was most recently pressed (after being up).
		PSMVector3f m_posMetersAtTouchpadPressTime;

		// The orientation of the controller in driver space at the time when
		// the touchpad was most recently pressed (after being up).
		PSMQuatf m_driverSpaceRotationAtTouchpadPressTime;

		// Flag used to completely disable the alignment gesture.
		bool m_bDisableHMDAlignmentGesture;

		// Flag to tell if we should use the controller orientation as part of the controller alignment.
		bool m_bUseControllerOrientationInHMDAlignment;

		// The axis to use for trigger input
		int m_steamVRTriggerAxisIndex;

		// The axes to use for touchpad input (virtual controller only)
		int m_virtualTriggerAxisIndex;
		int m_virtualTouchpadXAxisIndex;
		int m_virtualTouchpadYAxisIndex;

		// Settings values. Used to adjust throwing power using linear velocity and acceleration.
		float m_fLinearVelocityMultiplier;
		float m_fLinearVelocityExponent;

		// The button to use for controller hmd alignment.
		ePSButtonID m_hmdAlignPSButtonID;

		// Override model to use for the controller.
		std::string m_overrideModel;

		// Optional solver used to determine hand orientation.
		class IHandOrientationSolver *m_orientationSolver;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}