//========= Copyright Valve Corporation ============//
#include "logger.h"

#include <stdio.h>
#include <stdarg.h>

namespace steamvrbridge{

	static vr::IVRDriverLog * s_pLogFile = NULL;

#if !defined( WIN32)
#define vsnprintf_s vsnprintf
#endif

	bool Logger::InitDriverLog(vr::IVRDriverLog *pDriverLog)
	{
		if (s_pLogFile)
			return false;
		s_pLogFile = pDriverLog;
		return s_pLogFile != NULL;
	}

	void Logger::CleanupDriverLog()
	{
		s_pLogFile = NULL;
	}

	void Logger::DriverLogVarArgs(const char *pMsgFormat, va_list args)
	{
		char buf[1024];
#if defined( WIN32 )
		vsprintf_s(buf, pMsgFormat, args);
#else
		vsnprintf(buf, sizeof(buf), pMsgFormat, args);
#endif

		if (s_pLogFile)
			s_pLogFile->Log(buf);
	}

	/** Provides printf-style debug logging via the vr::IVRDriverLog interface provided by SteamVR
	* during initialization.  Client logging ends up in vrclient_appname.txt and server logging
	* ends up in vrserver.txt.
	*/
	void Logger::Info(const char *pMsgFormat, ...)
	{
		va_list args;
		va_start(args, pMsgFormat);

		Logger::DriverLogVarArgs(pMsgFormat, args);

		va_end(args);
	}

	void Logger::Debug(const char *pMsgFormat, ...)
	{

#ifdef _DEBUG
		va_list args;
		va_start(args, pMsgFormat);

		Logger::DriverLogVarArgs(pMsgFormat, args);

		va_end(args);
#endif
	}
}