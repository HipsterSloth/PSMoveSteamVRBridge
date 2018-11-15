#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {

	/* A trackable Virtual controller (tracking bulb + game pad).
	   The controller class bridges the PSMoveService controller to OpenVR's tracked device.*/
	class VirtualController : public Controller {

	public:
		// Constructor/Destructor
		VirtualController(PSMControllerID psmControllerID, vr::ETrackedControllerRole trackedControllerRole, const char *psmSerialNo);
		virtual ~VirtualController();

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
		PSMControllerType GetPSMControllerType() const override { return PSMController_Virtual; }
		void SetPendingHapticVibration(const vr::VREvent_HapticVibration_t &hapticData) override;

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
		bool m_touchpadDirectionsUsed;

		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetPoseButtonPressTime;
		bool m_bResetPoseRequestSent;
		std::chrono::time_point<std::chrono::high_resolution_clock> m_resetAlignButtonPressTime;
		bool m_bResetAlignRequestSent;

		// Button Remapping
		eEmulatedTrackpadAction psButtonIDToEmulatedTouchpadAction[k_PSMButtonID_Count];
		void LoadEmulatedTouchpadActions(
			vr::IVRSettings *pSettings,
			const ePSMButtonID psButtonID,
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

		// The axis to use for trigger input
		int m_steamVRTriggerAxisIndex;

		// The axes to use for touchpad input (virtual controller only)
		int m_virtualTouchpadXAxisIndex;
		int m_virtualTouchpadYAxisIndex;

		// The size of the deadzone for the controller's thumbstick
		float m_thumbstickDeadzone;

		// Treat a thumbstick touch also as a press
		bool m_bThumbstickTouchAsPress;

		// Settings values. Used to adjust throwing power using linear velocity and acceleration.
		float m_fLinearVelocityMultiplier;
		float m_fLinearVelocityExponent;

		// The button to use for controller hmd alignment
		ePSMButtonID m_hmdAlignPSButtonID;

		// Override model to use for the controller.
		std::string m_overrideModel;

		// Optional solver used to determine hand orientation.
		class IHandOrientationSolver *m_orientationSolver;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}