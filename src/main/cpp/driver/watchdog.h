#pragma once
#include <chrono>
#include <atomic>
#include <thread>
#include <mutex>

#include <openvr_driver.h>
#include "settings_util.h"
#include "PSMoveClient_CAPI.h"

namespace steamvrbridge {

	/*  
		IClientTrackedDeviceProvider implementation as per: 
		https://github.com/ValveSoftware/openvr/wiki/IClientTrackedDeviceProvider_Overview 
	*/
	class CWatchdogDriver_PSMoveService : public vr::IVRWatchdogProvider
	{
	public:
		CWatchdogDriver_PSMoveService();
		virtual ~CWatchdogDriver_PSMoveService();

		static CWatchdogDriver_PSMoveService * getInstance() {
			if (m_instance == nullptr)
				m_instance = new CWatchdogDriver_PSMoveService();

			return m_instance;
		}

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

		ServerDriverConfig m_config;

		PSMControllerList controllerList;

		static CWatchdogDriver_PSMoveService * m_instance;
	};
}