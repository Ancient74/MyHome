#pragma once
#include <memory>
#include "WinApiMessageHandler.h"

enum class ShutdownMode {
	NormalShutdown, ForceShutdown, NormalRestart, ForceRestart
};

class DesktopManagerImpl
{
public: 
	DesktopManagerImpl();
	void Shutdown(ShutdownMode shutdownMode) const;
	void OpenInBrowser(const std::wstring& link) const;
};

