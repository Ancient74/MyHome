#pragma once
#include <thread>
#include <functional>
#include <map>
#include <vector>
#include <mutex>
#include <future>

class WinApiMessageHandler
{
public:
	using HandlerT = void(WPARAM, LPARAM);

	WinApiMessageHandler();
	void Subscribe(UINT message, const std::function<HandlerT>& handler);

	HWND GetHWND() const;

private:
	void PumpMessage(std::stop_token token, std::promise<void>& hwndPromise);

	std::jthread m_messageThread;
	std::mutex m_messagesMutex;
	std::map<UINT, std::vector<std::function<HandlerT>>> m_messages{};
	HWND m_hwnd;
};

