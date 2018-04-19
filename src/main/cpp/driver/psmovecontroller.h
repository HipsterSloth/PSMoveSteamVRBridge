#pragma once
#include "trackabledevice.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {

	/* Trackable PS Move controller. The controller calss bridges the PSMoveService controller to
	OpenVR's tracked device*/
	class PSMoveController : public TrackableDevice {

		/* PSMoveService button IDs*/
		enum ePSButtonID
		{
			k_EPSButtonID_0,
			k_EPSButtonID_1,
			k_EPSButtonID_2,
			k_EPSButtonID_3,
			k_EPSButtonID_4,
			k_EPSButtonID_5,
			k_EPSButtonID_6,
			k_EPSButtonID_7,
			k_EPSButtonID_8,
			k_EPSButtonID_9,
			k_EPSButtonID_10,
			k_EPSButtonID_11,
			k_EPSButtonID_12,
			k_EPSButtonID_13,
			k_EPSButtonID_14,
			k_EPSButtonID_15,
			k_EPSButtonID_16,
			k_EPSButtonID_17,
			k_EPSButtonID_18,
			k_EPSButtonID_19,
			k_EPSButtonID_20,
			k_EPSButtonID_21,
			k_EPSButtonID_22,
			k_EPSButtonID_23,
			k_EPSButtonID_24,
			k_EPSButtonID_25,
			k_EPSButtonID_26,
			k_EPSButtonID_27,
			k_EPSButtonID_28,
			k_EPSButtonID_29,
			k_EPSButtonID_30,
			k_EPSButtonID_31,

			k_EPSButtonID_Count,

			k_EPSButtonID_PS = k_EPSButtonID_0,
			k_EPSButtonID_Left = k_EPSButtonID_1,
			k_EPSButtonID_Up = k_EPSButtonID_2,
			k_EPSButtonID_Right = k_EPSButtonID_3,
			k_EPSButtonID_Down = k_EPSButtonID_4,
			k_EPSButtonID_Move = k_EPSButtonID_5,
			k_EPSButtonID_Trackpad = k_EPSButtonID_6,
			k_EPSButtonID_Trigger = k_EPSButtonID_7,
			k_EPSButtonID_Triangle = k_EPSButtonID_8,
			k_EPSButtonID_Square = k_EPSButtonID_9,
			k_EPSButtonID_Circle = k_EPSButtonID_10,
			k_EPSButtonID_Cross = k_EPSButtonID_11,
			k_EPSButtonID_Select = k_EPSButtonID_12,
			k_EPSButtonID_Share = k_EPSButtonID_13,
			k_EPSButtonID_Start = k_EPSButtonID_14,
			k_EPSButtonID_Options = k_EPSButtonID_15,
			k_EPSButtonID_L1 = k_EPSButtonID_16,
			k_EPSButtonID_L2 = k_EPSButtonID_17,
			k_EPSButtonID_L3 = k_EPSButtonID_18,
			k_EPSButtonID_R1 = k_EPSButtonID_19,
			k_EPSButtonID_R2 = k_EPSButtonID_20,
			k_EPSButtonID_R3 = k_EPSButtonID_21,
		};

	public:
		// Constructor/Destructor
		PSMoveController(PSMControllerID psmControllerID, PSMControllerType psmControllerType, vr::ETrackedControllerRole trackedControllerRole, const char *psmSerialNo);
		virtual ~PSMoveController();

		// Overridden Implementation of vr::ITrackedDeviceServerDriver
		virtual vr::EVRInitError Activate(vr::TrackedDeviceIndex_t unObjectId) override;
		virtual void Deactivate() override;

		// Overridden Implementation of TrackedDevice
		virtual vr::ETrackedDeviceClass GetTrackedDeviceClass() const override { return vr::TrackedDeviceClass_Controller; }
		virtual void Update() override;
		virtual void RefreshWorldFromDriverPose() override;

		// PSMoveController Interface (accessor methods?)
		inline bool HasPSMControllerId(int ControllerID) const { return ControllerID == m_nPSMControllerId; }
		inline const PSMController * getPSMControllerView() const { return m_PSMServiceController; }
		inline std::string getPSMControllerSerialNo() const { return m_strPSMControllerSerialNo; }
		inline PSMControllerType getPSMControllerType() const { return m_PSMControllerType; }
		void UpdateRumbleState(float durationSecs);

	private:

		void PSMoveController::UpdateButtonState(ePSButtonID button, bool buttonState);
		void RealignHMDTrackingSpace();
		void HandleTouchPadDirection();
		void UpdateControllerState();
		void UpdateControllerStateFromPsMoveButtonState(ePSButtonID buttonId, PSMButtonState buttonState);
		void HandleTrigger(float latestTriggerValue);
		void UpdateTrackingState();
		
		void UpdateBatteryChargeState(PSMBatteryState newBatteryEnum);

		// Controller State
		int m_nPSMControllerId;
		PSMControllerType m_PSMControllerType;
		PSMController *m_PSMServiceController;
		std::string m_strPSMControllerSerialNo;

		// Used to report the controllers calibration status
		vr::ETrackingResult m_trackingStatus;

		// Used to ignore old state from PSM Service
		int m_nPoseSequenceNumber;

		// To main structures for passing state to vrserver
		//vr::VRControllerState_t m_ControllerState;

		// Cached for answering version queries from vrserver
		bool m_bIsBatteryCharging;
		float m_fBatteryChargeFraction;

		// Rumble state
		bool m_bRumbleSuppressed;
		uint16_t m_pendingHapticPulseDuration;
		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTimeRumbleSent;
		bool m_lastTimeRumbleSentValid;

		//virtual extend controller in meters
		float m_fVirtuallExtendControllersYMeters;
		float m_fVirtuallExtendControllersZMeters;

		// virtually rotate controller
		bool m_fVirtuallyRotateController;

		// delay in resetting touchpad position after touchpad press
		bool m_bDelayAfterTouchpadPress;

		// true while the touchpad is considered active (touched or pressed) 
		// after the initial touchpad delay, if any
		bool m_bTouchpadWasActive;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_lastTouchpadPressTime;
		bool m_touchpadDirectionsUsed;

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

		// Flag used to completely disable the alignment gesture
		bool m_bDisableHMDAlignmentGesture;

		// Flag to tell if we should use the controller orientation as part of the controller alignment
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

		// The button to use for controller hmd alignment
		ePSButtonID m_hmdAlignPSButtonID;

		// Override model to use for the controller
		std::string m_overrideModel;

		// Optional solver used to determine hand orientation
		class IHandOrientationSolver *m_orientationSolver;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}