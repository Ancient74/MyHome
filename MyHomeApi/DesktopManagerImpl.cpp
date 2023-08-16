#include "pch.h"
#include "DesktopManagerImpl.h"
#include <stdexcept>
#include <atlcom.h>
#include <Shldisp.h>
#include "ErrorHandling.h"
#include <iostream>

namespace
{
    class HandleWrapper {
    public:
        HandleWrapper(HANDLE handle) : m_handle(handle)
        {
        }

        HandleWrapper() = default;

        HANDLE Get() const { return m_handle; }

        PHANDLE operator &() { return &m_handle; }

        bool operator!() { return !m_handle; }

        operator HANDLE() { return m_handle; }

        ~HandleWrapper()
        { 
            if (m_handle)
                CloseHandle(m_handle);
        }

    private:
        HANDLE m_handle;
    };
}

DesktopManagerImpl::DesktopManagerImpl()
{
}

void DesktopManagerImpl::Shutdown(ShutdownMode shutdownMode) const
{
    HandleWrapper token;
    TOKEN_PRIVILEGES privileges;

    if (!OpenProcessToken(GetCurrentProcess(),
        TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &token))
        throw std::runtime_error("OpenProcessToken failed: " + GetLastError());

    LookupPrivilegeValue(NULL, SE_SHUTDOWN_NAME, &privileges.Privileges[0].Luid);

    privileges.PrivilegeCount = 1;  // one privilege to set    
    privileges.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

    AdjustTokenPrivileges(token, FALSE, &privileges, 0, nullptr, 0);

    bool restart = shutdownMode == ShutdownMode::NormalRestart || shutdownMode == ShutdownMode::ForceRestart;
    bool force = shutdownMode == ShutdownMode::ForceShutdown || shutdownMode == ShutdownMode::ForceRestart;

    const DWORD timeoutSec = 10;

    UINT flags = 0;
    flags |= restart ? EWX_REBOOT : EWX_POWEROFF;
    flags |= force ? EWX_FORCE : 0;
    flags |= shutdownMode == ShutdownMode::NormalRestart ? EWX_RESTARTAPPS : 0;

    if (GetLastError() != ERROR_SUCCESS)
        throw std::runtime_error("AdjustTokenPrivileges failed: " + GetLastError());

    if (ExitWindowsEx(flags, SHTDN_REASON_MAJOR_OPERATINGSYSTEM |
        SHTDN_REASON_MINOR_UPGRADE |
        SHTDN_REASON_FLAG_PLANNED) == 0)
        throw std::runtime_error("ExitWindowsEx failed: " + GetLastError());
}

void DesktopManagerImpl::OpenInBrowser(const std::wstring& link) const
{
    ShellExecute(NULL, NULL, link.c_str(), NULL, NULL, SW_SHOWNORMAL);
}
