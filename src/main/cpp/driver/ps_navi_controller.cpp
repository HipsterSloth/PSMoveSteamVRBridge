// TODO Implementation of a PS navi controller

// TODO - Implement this so direction button presses equate to simluated touchpad directions

//void PSMoveController::UpdateTouchpadState(
//	ePSButtonID buttonId,
//	PSMButtonState buttonState) {

//	/* Now check if the button pressed was a directional touchpad button. If it is was then fake a change
//	in the touchpad axis. i.e. pretend a finger touched the touchpad in the given direction */
//	if (buttonState == PSMButtonState_DOWN || buttonState == PSMButtonState_PRESSED) {
//		if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Left) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.x = -1.0f;
//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Right) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.x = 1.0f;
//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Up) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.y = -1.0f;
//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Down) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.y = 1.0f;
//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_UpLeft) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.x = -0.707f;
//			state.trackpad.axis.y = 0.707f;
//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_UpRight) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.x = 0.707f;
//			state.trackpad.axis.y = 0.707f;
//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_DownLeft) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.x = -0.707f;
//			state.trackpad.axis.y = -0.707f;
//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_DownRight) {
//			m_touchpadDirectionsUsed = true;
//			state.trackpad.axis.x = 0.707f;
//			state.trackpad.axis.y = -0.707f;
//		}
//	}

//	if (buttonState == PSMButtonState_DOWN || buttonState == PSMButtonState_PRESSED) {
//		m_touchpadDirectionsUsed = true;
//		state.trackpad.axis.x = 0.f;
//		state.trackpad.axis.y = 0.f;
//	}
//}