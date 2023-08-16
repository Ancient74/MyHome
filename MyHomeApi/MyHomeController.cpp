#include "pch.h"
#include "MyHomeController.h"
#include <string>
#include "UserDefinedMessages.h"
#include "StringUtils.h"

namespace {
	class OpenProcessWrapper
	{
	public:
		OpenProcessWrapper(const std::wstring& name, const std::wstring& cmd)
		{
			m_startup.cb = sizeof(m_startup);
			if (CreateProcess(nullptr, const_cast<LPWSTR>((name + L" " + cmd).c_str()), nullptr, nullptr, FALSE, NORMAL_PRIORITY_CLASS, nullptr, nullptr, &m_startup, &m_info) == 0)
			{
				DWORD error = GetLastError();
				LPSTR messageBuffer = nullptr;
				FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
					NULL, error, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), reinterpret_cast<LPSTR>(&messageBuffer), 0u, NULL);
				std::string message = messageBuffer;
				LocalFree(messageBuffer);
				throw std::runtime_error("CreateProcess failed: " + message);
			}
		}

		void WaitUntilFinished()
		{
			WaitForSingleObject(m_info.hProcess, INFINITE);
		}

		~OpenProcessWrapper()
		{
			CloseHandle(m_info.hProcess);
			CloseHandle(m_info.hThread);
		}

	private:
		STARTUPINFO m_startup{};
		PROCESS_INFORMATION m_info{};
	};
}

MyHomeController::MyHomeController()
	: m_messageHandler(std::make_shared<WinApiMessageHandler>())
	, m_monitorManager(std::make_unique<MonitorManagerImpl>(m_messageHandler->GetHWND()))
	, m_desktopManager(std::make_unique<DesktopManagerImpl>())
	, m_clientFileterImpl(std::make_unique<ClientFilterImpl>())
	, m_audioManagerImpl(std::make_unique<AudioManagerImpl>())
{
}

HRESULT __stdcall MyHomeController::SetMonitorMode(MyMonitorMode monitorMode)
{
	MonitorManagerImpl::MonitorMode mode;
	switch (monitorMode)
	{
	case MonitorMode_PCScreenOnly:
		mode = MonitorManagerImpl::MonitorMode::PCScreenOnly;
		break;
	case MonitorMode_SecondScreenOnly:
		mode = MonitorManagerImpl::MonitorMode::SecondScreenOnly;
		break;
	case MonitorMode_Extend:
		mode = MonitorManagerImpl::MonitorMode::Extend;
		break;
	case MonitorMode_Duplicate:
		mode = MonitorManagerImpl::MonitorMode::Clone;
		break;
	default:
		return E_INVALIDARG;
	}
	m_monitorManager->SetMonitorMode(mode);
	return S_OK;
}

HRESULT __stdcall MyHomeController::OpenBigPicture()
{
	OpenProcessWrapper process(L"steam.exe", L"-start steam://open/bigpicture");
	process.WaitUntilFinished();
	return S_OK;
}

HRESULT __stdcall MyHomeController::Shutdown(MyShutdownMode shutdownMode)
{
	ShutdownMode mode;
	switch (shutdownMode)
	{
	case MyShutdownMode::ShutdownMode_NormalShutdown:
		mode = ShutdownMode::NormalShutdown;
		break;
	case MyShutdownMode::ShutdownMode_ForceShutdown:
		mode = ShutdownMode::ForceShutdown;
		break;
	case MyShutdownMode::ShutdownMode_NormalRestart:
		mode = ShutdownMode::NormalRestart;
		break;
	case MyShutdownMode::ShutdownMode_ForceRestart:
		mode = ShutdownMode::ForceRestart;
		break;
	default:
		return E_INVALIDARG;
	}
	m_desktopManager->Shutdown(mode);
	return S_OK;
}

HRESULT __stdcall MyHomeController::OpenInBrowser(BSTR link)
{
	if (!link)
		return E_POINTER;

	m_desktopManager->OpenInBrowser(StringUtils::ToWstring(link));
	return S_OK;
}

HRESULT __stdcall MyHomeController::IsClientAllowed(BSTR ipAddress, BOOL* isAllowed)
{
	if (!isAllowed)
		return E_POINTER;
	if (!ipAddress)
		return E_INVALIDARG;

	*isAllowed = m_clientFileterImpl->IsClientAllowed(StringUtils::ToWstring(ipAddress));

	return S_OK;
}

HRESULT __stdcall MyHomeController::GetAudioInputController(IAudioController** audioInputController)
{
	if (!audioInputController)
		return E_POINTER;

	auto audioInputControllerPtr = m_audioManagerImpl->GetAudioInputController();
	*audioInputController = winrt::detach_abi(audioInputControllerPtr);

	return S_OK;
}

HRESULT __stdcall MyHomeController::GetAudioOutputController(IAudioController** audioOutputController)
{
	if (!audioOutputController)
		return E_POINTER;
	
	auto audioOutputControllerPtr = m_audioManagerImpl->GetAudioOutputController();
	*audioOutputController = winrt::detach_abi(audioOutputControllerPtr);

	return S_OK;
}
