#include "settingsutil.h"

namespace steamvrbridge {

	bool SettingsUtil::LoadBool(vr::IVRSettings *pSettings, const char *pchSection, const char *pchSettingsKey, const bool bDefaultValue)
	{
		vr::EVRSettingsError eError;
		bool bResult = pSettings->GetBool(pchSection, pchSettingsKey, &eError);

		if (eError != vr::VRSettingsError_None)
		{
			bResult = bDefaultValue;
		}

		return bResult;
	}

	int SettingsUtil::LoadInt(vr::IVRSettings *pSettings, const char *pchSection, const char *pchSettingsKey, const int iDefaultValue)
	{
		vr::EVRSettingsError eError;
		int iResult = pSettings->GetInt32(pchSection, pchSettingsKey, &eError);

		if (eError != vr::VRSettingsError_None)
		{
			iResult = iDefaultValue;
		}

		return iResult;
	}

	float SettingsUtil::LoadFloat(vr::IVRSettings *pSettings, const char *pchSection, const char *pchSettingsKey, const float fDefaultValue)
	{
		vr::EVRSettingsError eError;
		float fResult = pSettings->GetFloat(pchSection, pchSettingsKey, &eError);

		if (eError != vr::VRSettingsError_None)
		{
			fResult = fDefaultValue;
		}

		return fResult;
	}
}