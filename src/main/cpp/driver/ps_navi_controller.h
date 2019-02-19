#pragma once
#include "trackable_device.h"
#include "constants.h"
#include "controller.h"
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>

namespace steamvrbridge {

	class PSNaviControllerConfig : public ControllerConfig
	{
	public:
		PSNaviControllerConfig(class PSNaviController *ownerController, const std::string &fnamebase = "NaviControllerConfig");

        Config *Clone() override { return new PSNaviControllerConfig(*this); }
        void OnConfigChanged(Config *newConfig) override;
		bool ReadFromJSON(const configuru::Config &pt) override;

		// The inner deadzone of the thumbsticks
		float thumbstick_deadzone;
	};

	/* An un-tracked PSNavi controller.
	   The controller class bridges the PSMoveService controller to OpenVR's tracked device.*/
	class PSNaviController : public Controller {

	public:
		// Constructor/Destructor
		PSNaviController(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const char *psmSerialNo);
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
        void OnControllerModelChanged() override;
		const char *GetControllerSettingsPrefix() const override { return "playstation_navi"; }
		bool HasPSMControllerId(int ControllerID) const override { return ControllerID == m_nPSMControllerId; }
		const PSMController * GetPSMControllerView() const override { return m_PSMServiceController; }
		std::string GetPSMControllerSerialNo() const override { return m_strPSMControllerSerialNo; }
		PSMControllerType GetPSMControllerType() const override { return PSMController_Virtual; }

	protected:
		const PSNaviControllerConfig *getConfig() const { return static_cast<const PSNaviControllerConfig *>(m_config); }
		ControllerConfig *AllocateControllerConfig() override { 
			std::string fnamebase= std::string("psnavi_") + m_strPSMControllerSerialNo;
			return new PSNaviControllerConfig(this, fnamebase); 
		}

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

		// The last normalized thumbstick values (post dead zone application);
		float m_lastSanitizedThumbstick_X;
		float m_lastSanitizedThumbstick_Y;

		// Callbacks
		static void start_controller_response_callback(const PSMResponseMessage *response, void *userdata);
	};
}