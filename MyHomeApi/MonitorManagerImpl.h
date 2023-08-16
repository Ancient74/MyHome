#pragma once
class MonitorManagerImpl
{
public:
	MonitorManagerImpl(HWND hwnd);
	enum MonitorMode
	{
		SecondScreenOnly = 1,
		PCScreenOnly,
		Clone,
		Extend
	};

	MonitorMode GetMonitorMode() const;
	void SetMonitorMode(MonitorMode mode);

private:
	HWND m_hwnd;
};
