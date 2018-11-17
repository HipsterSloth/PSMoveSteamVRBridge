#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {

	/* An un-tracked PSNavi controller.
	   The controller class bridges the PSMoveService controller to OpenVR's tracked device.*/
	class PSNaviController : public Controller {

	public:
		// Constructor/Destructor
		PSNaviController(PSMControllerID psmControllerID, vr::ETrackedControllerRole trackedControllerRole, const char *psmSerialNo);
		virtual ~PSNaviController();

		// Sends button data to a parent controller
		// instead of acting as an independant device
		void AttachToController(Controller *parent_controller);

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

	private:
		void UpdateThumbstick();
		void UpdateEmulatedTrackpad();
		void UpdateControllerState();
		void UpdateTrackingState();

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

		// Button Remapping
		eEmulatedTrackpadAction psButtonIDToEmulatedTouchpadAction[k_PSMButtonID_Count];
		void LoadEmulatedTouchpadActions(
			vr::IVRSettings *pSettings,
			const ePSMButtonID psButtonID,
			int controllerId = -1);

		// The last normalized thumbstick values (post dead zone application);
		float m_lastSanitizedThumbstick_X;
		float m_lastSanitizedThumbstick_Y;

		// The size of the deadzone for the controller's thumbstick
		float m_thumbstickDeadzone;

		// Override model to use for the controller.
		std::string m_overrideModel;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}