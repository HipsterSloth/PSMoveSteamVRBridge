#pragma once
#include "PSMoveClient_CAPI.h"
#include "constants.h"

namespace steamvrbridge {

	/*
		Interface that defines what a controller is and can do in the context of this driver. (Self-contained header)
	*/
	class IController {

		/*
			The following methods must be overridden in order for an implementing object to be considered
			a Controller object. They also offer guidelines as to how to they should generally be
			implemented while still	respecting each controllers implementation may be different.

			With regards to function naming conventions used, functions prefixed with 'Update' suggest
			both object state change and OpenVR/PSMS notification whereas functions prefixed with 'Set/Get'
			suggest object state change only.
		*/

	public:
		// Empty desctructor since this is an interface, does not need to be implemented.
		virtual ~IController() {};

		// Returns true if the controller has a PSM assigned the given ControllerID.
		virtual bool HasPSMControllerId(int ControllerID) const = 0;

		// Returns the PSM controller view.
		virtual const PSMController * GetPSMControllerView() const = 0;

		// Returns the PSM controller serial number.
		virtual std::string GetPSMControllerSerialNo() const = 0;

		// Returns the PSM controller type.
		virtual PSMControllerType GetPSMControllerType() const = 0;

		// Sets the controller's pending haptic duration, amplitude, frequency given OpenVR shaptic vibration event data.
		virtual void SetPendingHapticVibration(vr::VREvent_HapticVibration_t hapticData) = 0;

		// Updates the rumble state of the controller.
		virtual void UpdateRumbleState() = 0;

	private:
		// Updates the controllers state for the given button.
		virtual void UpdateButtonState(ePSButtonID button, bool buttonState) = 0;

		// Updates the controller state of the touchpad direction.
		virtual void UpdateTouchPadDirection() = 0;

		// Updates the controllers state. This includes buttons, trackpads and haptics.
		virtual void UpdateControllerState() = 0;

		// Updates the tracking state of the controller and notifies OpenVR.
		virtual void UpdateTrackingState() = 0;

		// Updates the controller battery state given the new state and notifies OpenVR.
		virtual void UpdateBatteryChargeState(PSMBatteryState newBatteryEnum) = 0;

		// Sets the trigger value.
		virtual void SetTriggerValue(float latestTriggerValue) = 0;
	};
}