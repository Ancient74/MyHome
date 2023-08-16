#pragma once
#include "MyHomeApi_i.h"
#include <atlcom.h>
#include <memory>
#include "MonitorManagerImpl.h"
#include "WinApiMessageHandler.h"
#include "DesktopManagerImpl.h"
#include "ClientFilterImpl.h"
#include "AudioManagerImpl.h"

class ATL_NO_VTABLE MyHomeController :
	public ATL::CComObjectRootEx<ATL::CComSingleThreadModel>,
	public ATL::CComCoClass<MyHomeController, &CLSID_MyHomeController>,
	public IMonitorManager,
	public IDesktopManager,
	public IClientFilter,
	public IAudioManager
{
public:
	DECLARE_REGISTRY_RESOURCEID(IDR_MYHOMEAPI)
	
	BEGIN_COM_MAP(MyHomeController)
		COM_INTERFACE_ENTRY(IMonitorManager)
		COM_INTERFACE_ENTRY(IDesktopManager)
		COM_INTERFACE_ENTRY(IClientFilter)
		COM_INTERFACE_ENTRY(IAudioManager)
	END_COM_MAP()

	MyHomeController();
	
	// IMonitorManager
	HRESULT STDMETHODCALLTYPE SetMonitorMode(MyMonitorMode monitorMode) override;
	HRESULT STDMETHODCALLTYPE OpenBigPicture() override;

	// IDesktopManager
	HRESULT STDMETHODCALLTYPE Shutdown(MyShutdownMode shutdownMode) override;
	HRESULT STDMETHODCALLTYPE OpenInBrowser(BSTR link) override;

	// IClientFilter
	HRESULT STDMETHODCALLTYPE IsClientAllowed(BSTR ipAddress, BOOL* isAllowed) override;

	//IAudioManager
	HRESULT STDMETHODCALLTYPE GetAudioInputController(IAudioController** audioInputController) override;
	HRESULT STDMETHODCALLTYPE GetAudioOutputController(IAudioController** audioOutputController) override;

private:
	std::shared_ptr<WinApiMessageHandler> m_messageHandler;
	std::unique_ptr<MonitorManagerImpl> m_monitorManager;
	std::unique_ptr<DesktopManagerImpl> m_desktopManager;
	std::unique_ptr<ClientFilterImpl> m_clientFileterImpl;
	std::unique_ptr<AudioManagerImpl> m_audioManagerImpl;
};

OBJECT_ENTRY_AUTO(__uuidof(MyHomeController), MyHomeController);
