//========= Copyright Valve Corporation ============//
#pragma once

#include <string>
#include <openvr_driver.h>

//use a namesless namespace when sharing extern member functions

namespace steamvrbridge
{
	class Logger {
	public:
		static bool InitDriverLog(vr::IVRDriverLog *pDriverLog);
		static void CleanupDriverLog();
		static void DriverLogVarArgs(const char *pMsgFormat, va_list args);
		static void Info(const char *pchFormat, ...);
		static void Debug(const char *pchFormat, ...);
	};
}