#include "controller.h"
#include "utils.h"
#include "logger.h"
#include "driver.h"

#include <assert.h>

namespace steamvrbridge {

	//-- ControllerConfig -----
	ControllerConfig::ControllerConfig(Controller *ownerController, const std::string &fnamebase)
		: Config(fnamebase)
        , m_ownerController(ownerController)
		, controller_disabled(false)
		, override_model("")
    {
		// Initially no button maps to any enumulated touchpad action
		memset(ps_button_id_to_emulated_touchpad_action, k_EmulatedTrackpadAction_None, k_PSMButtonID_Count * sizeof(vr::EVRButtonId));
	};

	bool ControllerConfig::ReadFromJSON(const configuru::Config &pt) {
        controller_disabled= pt.get_or<bool>("controller_disabled", false);
		override_model= pt.get_or<std::string>("override_model", "");
		return true;
	}

    void ControllerConfig::OnConfigChanged(Config *newConfig)
    {
        ControllerConfig *newControllerConfig= static_cast<ControllerConfig *>(newConfig);

        // These values can just be copied
		memcpy(this->ps_button_id_to_emulated_touchpad_action, newControllerConfig->ps_button_id_to_emulated_touchpad_action, sizeof(ps_button_id_to_emulated_touchpad_action));

        // override model change 
        if (this->override_model != newControllerConfig->override_model)
        {
            this->override_model = newControllerConfig->override_model;
            m_ownerController->OnControllerModelChanged();
        }
        
        // disable state change
        // NOTE: Ideally we could notify the SteamVR if the controller became disabled,
        // however the SteamVR Driver API will only allow you to add new devices, not remove them.
        this->controller_disabled= newControllerConfig->controller_disabled;

        Config::OnConfigChanged(newConfig); 
    }

	void ControllerConfig::ReadEmulatedTouchpadAction(
		const configuru::Config &pt,
		const ePSMButtonID psButtonID) {

		assert(psButtonID >= 0 && psButtonID < k_PSMButtonID_Count);
		eEmulatedTrackpadAction vrTouchpadDirection = k_EmulatedTrackpadAction_None;

		if (pt.has_key("trackpad_mappings")) {
			const configuru::Config &trackpad_pt= pt["trackpad_mappings"];
			const char *szPSButtonName = k_PSMButtonNames[psButtonID];

			if (trackpad_pt.has_key(szPSButtonName)) {
				const std::string &remapButtonToTouchpadDirectionString= trackpad_pt[szPSButtonName].as_string();

				for (int vr_touchpad_direction_index = 0; vr_touchpad_direction_index < k_max_vr_touchpad_actions; ++vr_touchpad_direction_index) {
					if (strcasecmp(remapButtonToTouchpadDirectionString.c_str(), k_VRTouchpadActionNames[vr_touchpad_direction_index]) == 0) {
						vrTouchpadDirection = static_cast<eEmulatedTrackpadAction>(vr_touchpad_direction_index);
						break;
					}
				}
			}
		}

		// Load the mapping
		ps_button_id_to_emulated_touchpad_action[psButtonID] = vrTouchpadDirection;
	}

	//-- Controller -----
	Controller::Controller(PSMControllerHand desiredControllerHand) 
	: TrackableDevice()
	, m_config(nullptr) 
    , m_desiredControllerHand(desiredControllerHand)
    , m_trackedControllerRole(vr::ETrackedControllerRole::TrackedControllerRole_Invalid)
    {
	}

	Controller::~Controller() {
        DisposeConfig();
	}

    void Controller::InitConfig()
    {
		// Create the specific configuration class instance
        // This call has to happen outside of the constructor
        // because virtual functions aren't safe to call in a constructor
		m_config= this->AllocateControllerConfig();

        // Register with ConfigManager for config changes
        m_config->init(); 

        // Create and load the controller config
		m_config->load();
    }

    void Controller::DisposeConfig()
    {
        if (m_config != nullptr) {
			delete m_config;
            m_config= nullptr;
		}
    }

	// Shared Implementation of vr::ITrackedDeviceServerDriver
	vr::EVRInitError Controller::Activate(vr::TrackedDeviceIndex_t unObjectId) {

		vr::EVRInitError result_code= TrackableDevice::Activate(unObjectId);

		if (result_code == vr::EVRInitError::VRInitError_None) {
			vr::CVRPropertyHelpers *properties = vr::VRProperties();
		
			// Configure JSON controller configuration input profile
			char szProfilePath[128];
			snprintf(szProfilePath, sizeof(szProfilePath), "{psmove}/input/%s_profile.json", GetControllerSettingsPrefix());
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_InputProfilePath_String, szProfilePath);

            // Attempt to reserve a controller role based on the desired hand
            m_trackedControllerRole= CServerDriver_PSMoveService::getInstance()->AllocateControllerRole(m_desiredControllerHand);
            if (m_trackedControllerRole != vr::TrackedControllerRole_Invalid)
            {
                properties->SetInt32Property(m_ulPropertyContainer, vr::Prop_ControllerRoleHint_Int32, m_trackedControllerRole);
            }
		}

		return result_code;
	}

	void Controller::Deactivate() {
        // Relinquish our claim to this controller role
        m_trackedControllerRole= vr::TrackedControllerRole_Invalid;
	}

    // Returns the tracked device's role. e.g. TrackedControllerRole_LeftHand
	vr::ETrackedControllerRole Controller::GetTrackedDeviceRole() const {
		return m_trackedControllerRole;
	}

    bool Controller::GetIsControllerDisabled() const
    {
        return m_config->controller_disabled;
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
					m_ulPropertyContainer, k_PSMHapticPaths[haptic_id], &hapticComponentHandle);

			if (result_code == vr::EVRInputError::VRInputError_None)
			{
				HapticState hapticState;
				hapticState.hapticComponentHandle= hapticComponentHandle;
				hapticState.pendingHapticDurationSecs= DEFAULT_HAPTIC_DURATION;
				hapticState.pendingHapticAmplitude= DEFAULT_HAPTIC_AMPLITUDE;
				hapticState.pendingHapticFrequency= DEFAULT_HAPTIC_FREQUENCY;
				hapticState.lastTimeRumbleSent= std::chrono::time_point<std::chrono::high_resolution_clock>();
				hapticState.lastTimeRumbleSentValid= false;

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

	void Controller::UpdateHaptics(const vr::VREvent_HapticVibration_t &hapticData) {
		for (auto &it : m_hapticStates)
		{
			HapticState &state= it.second;

			if (state.hapticComponentHandle == hapticData.componentHandle)
			{
				state.pendingHapticDurationSecs = hapticData.fDurationSeconds;
				state.pendingHapticAmplitude = hapticData.fAmplitude;
				state.pendingHapticFrequency = hapticData.fFrequency;

				break;
			}
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

	bool Controller::HasHapticState(ePSMHapicID haptic_id) const
	{
		return m_hapticStates.count(haptic_id) != 0;
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

	Controller::HapticState * Controller::GetHapticState(ePSMHapicID haptic_id)
	{
		if (m_hapticStates.count(haptic_id) != 0) {
			return &m_hapticStates.at(haptic_id);
		}

		return nullptr;
	}
}