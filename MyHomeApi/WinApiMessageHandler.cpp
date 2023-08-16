#include "pch.h"
#include "WinApiMessageHandler.h"

WinApiMessageHandler::WinApiMessageHandler()
{
	std::promise<void> hwndPromise;
	auto hwndFuture = hwndPromise.get_future();
	m_messageThread = std::jthread(std::bind(&WinApiMessageHandler::PumpMessage, this, std::placeholders::_1, std::move(hwndPromise)));
	hwndFuture.wait();
}

void WinApiMessageHandler::Subscribe(UINT message, const std::function<HandlerT>& handler)
{
	std::unique_lock<std::mutex> lock(m_messagesMutex);
	m_messages[message].push_back(handler);
}

HWND WinApiMessageHandler::GetHWND() const
{
	return m_hwnd;
}

void WinApiMessageHandler::PumpMessage(std::stop_token token, std::promise<void>& hwndPromise)
{
	WNDCLASS wndClass{};
	wndClass.lpfnWndProc = DefWindowProc;
	wndClass.lpszClassName = L"MyHomeApi";
	wndClass.hInstance = GetModuleHandle(nullptr);
	RegisterClass(&wndClass);

	m_hwnd = CreateWindow(wndClass.lpszClassName, wndClass.lpszClassName, WS_OVERLAPPED, 0, 0, 0, 0, nullptr, nullptr, wndClass.hInstance, nullptr);
	hwndPromise.set_value();

	MSG message{};
	while (GetMessage(&message, nullptr, 0, 0) > 0)
	{
		if (token.stop_requested())
			return;
		TranslateMessage(&message);
		DispatchMessage(&message);
		std::vector<std::function<HandlerT>> handlers;
		{
			std::unique_lock<std::mutex> lock(m_messagesMutex);
			if (decltype(m_messages)::iterator it = m_messages.find(message.message); it != m_messages.end())
			{
				handlers = it->second;
			}
		}
		for (auto& handler : handlers)
			handler(message.wParam, message.lParam);
	}
}
