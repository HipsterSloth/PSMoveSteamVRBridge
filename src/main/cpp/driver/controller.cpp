#include "controller.h"
#include "utils.h"
#include "logger.h"
#include "driver.h"

namespace steamvrbridge {

	Controller::Controller() 
	: TrackableDevice() {

	}

	Controller::~Controller()
	{

	}

	// Shared Implementation of vr::ITrackedDeviceServerDriver
	vr::EVRInitError Controller::Activate(vr::TrackedDeviceIndex_t unObjectId) {

		vr::EVRInitError result_code= TrackableDevice::Activate(unObjectId);

		if (result_code != vr::EVRInitError::VRInitError_None) {
			vr::CVRPropertyHelpers *properties = vr::VRProperties();
		
			// Configure JSON controller configuration input profile
			vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_InputProfilePath_String, "{psmove}/input/controller_profile.json");
		}

		return result_code;
	}

	bool Controller::CreateButtonComponent(ePSMButtonID button_id)
	{
		if (m_buttonStates.count(button_id) == 0) {
			vr::EVRInputError result_code= vr::EVRInputError::VRInputError_InvalidParam;
			vr::VRInputComponentHandle_t buttonComponentHandle;

			if (button_id < k_PSMButtonID_Count) {
				result_code=
					vr::VRDriverInput()->CreateBooleanComponent(
						m_ulPropertyContainer, k_PSMButtonPaths[button_id], &buttonComponentHandle);

			}

			if (result_code == vr::EVRInputError::VRInputError_None)
			{
				ButtonState buttonState;
				buttonState.buttonComponentHandle= buttonComponentHandle;
				buttonState.lastButtonState= PSMButtonState_UP;

				m_buttonStates[button_id]= buttonState;
			}

			return result_code == vr::EVRInputError::VRInputError_None;
		}

		return false;
	}

	bool Controller::CreateAxisComponent(ePSMAxisID axis_id) {
		if (m_axisStates.count(axis_id) == 0) {
			vr::EVRInputError result_code= vr::EVRInputError::VRInputError_InvalidParam;
			vr::VRInputComponentHandle_t axisComponentHandle;

			if (axis_id < k_PSMButtonID_Count) {
				result_code=
					vr::VRDriverInput()->CreateScalarComponent(
						m_ulPropertyContainer, k_PSMAxisPaths[axis_id], &axisComponentHandle,
						vr::VRScalarType_Absolute, 
						k_PSMAxisTwoSided[axis_id] ? vr::VRScalarUnits_NormalizedTwoSided : vr::VRScalarUnits_NormalizedOneSided);
			}

			if (result_code == vr::EVRInputError::VRInputError_None)
			{
				AxisState axisState;
				axisState.axisComponentHandle= axisComponentHandle;
				axisState.lastAxisState= 0.f;

				m_axisStates[axis_id]= axisState;
			}

			return result_code == vr::EVRInputError::VRInputError_None;
		}

		return false;
	}

	bool Controller::CreateHapticComponent(ePSMHapicID haptic_id)
	{
		if (m_hapticStates.count(haptic_id) == 0) {
			vr::VRInputComponentHandle_t hapticComponentHandle;
			vr::EVRInputError result_code=
				vr::VRDriverInput()->CreateHapticComponent(
					m_ulPropertyContainer, k_PSMButtonPaths[haptic_id], &hapticComponentHandle);

			if (result_code == vr::EVRInputError::VRInputError_None)
			{
				HapticState hapticState;
				hapticState.hapticComponentHandle= hapticComponentHandle;

				m_hapticStates[haptic_id]= hapticState;
			}

			return result_code == vr::EVRInputError::VRInputError_None;
		}

		return false;
	}

	void Controller::UpdateButton(ePSMButtonID button_id, PSMButtonState button_state, double time_offset)
	{
		if (m_buttonStates.count(button_id) != 0) {
			const bool is_pressed= (button_state == PSMButtonState_PRESSED) || (button_state == PSMButtonState_DOWN);

			m_buttonStates[button_id].lastButtonState= button_state;
			vr::VRDriverInput()->UpdateBooleanComponent(
				m_buttonStates[button_id].buttonComponentHandle, is_pressed, time_offset);
		}
	}

	void Controller::UpdateAxis(ePSMAxisID axis_id, float axis_value, double time_offset)
	{
		if (m_axisStates.count(axis_id) != 0) {
			m_axisStates[axis_id].lastAxisState= axis_value;
			vr::VRDriverInput()->UpdateScalarComponent(
				m_axisStates[axis_id].axisComponentHandle, axis_value, time_offset);
		}
	}

	bool Controller::HasButton(ePSMButtonID button_id) const
	{
		return m_buttonStates.count(button_id) != 0;
	}

	bool Controller::HasAxis(ePSMAxisID axis_id) const
	{
		return m_axisStates.count(axis_id) != 0;
	}

	bool Controller::GetButtonState(ePSMButtonID button_id, PSMButtonState &out_button_state) const
	{
		if (m_buttonStates.count(button_id) != 0) {
			out_button_state= m_buttonStates.at(button_id).lastButtonState;
			return true;
		}

		out_button_state= PSMButtonState_UP;
		return false;
	}

	bool Controller::GetAxisState(ePSMAxisID axis_id, float &out_axis_value) const
	{
		if (m_axisStates.count(axis_id) != 0) {
			out_axis_value= m_axisStates.at(axis_id).lastAxisState;
			return true;
		}

		out_axis_value= 0.f;
		return false;
	}

	bool Controller::IsHapticIDForHapticData(ePSMHapicID haptic_id, const vr::VREvent_HapticVibration_t &hapticData) const
	{
		if (m_hapticStates.count(haptic_id))
		{
			return m_hapticStates.at(haptic_id).hapticComponentHandle == hapticData.componentHandle;
		}

		return false;
	}
}