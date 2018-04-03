#pragma once
#include <chrono>
#include <atomic>
#include <thread>
#include <mutex>

#include <openvr_driver.h>
#include "PSMoveClient_CAPI.h"

namespace steamvrbridge {

	//-- definitions -----
	class CWatchdogDriver_PSMoveService : public vr::IVRWatchdogProvider
	{
	public:
		CWatchdogDriver_PSMoveService();

		// Inherited via IClientTrackedDeviceProvider
		virtual vr::EVRInitError Init(vr::IVRDriverContext *pDriverContext);
		virtual void Cleanup() override;

	protected:
		void WorkerThreadFunction();
		void WatchdogLogVarArgs(const char *pMsgFormat, va_list args);
		void WatchdogLog(const char *pMsgFormat, ...);

	private:
		class vr::IVRDriverLog * m_pLogger;
		std::mutex m_loggerMutex;

		bool m_bWasConnected;
		std::atomic_bool m_bExitSignaled;
		std::thread *m_pWatchdogThread;

		std::string m_strPSMoveServiceAddress;
		std::string m_strServerPort;

		PSMControllerList controllerList;
	};
}