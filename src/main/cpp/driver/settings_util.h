#pragma once
#include "openvr_driver.h"

namespace steamvrbridge {

	class SettingsUtil {
	public:
		static bool LoadBool(vr::IVRSettings *pSettings, const char *pchSection, const char *pchSettingsKey, const bool bDefaultValue);
		static int LoadInt(vr::IVRSettings *pSettings, const char *pchSection, const char *pchSettingsKey, const int iDefaultValue);
		static float LoadFloat(vr::IVRSettings *pSettings, const char *pchSection, const char *pchSettingsKey, const float fDefaultValue);
	};
}