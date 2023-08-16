#include "pch.h"
#include <vector>
#include "MonitorManagerImpl.h"
#include <string>
#include <stdexcept>
#include "UserDefinedMessages.h"

namespace
{
    DISPLAYCONFIG_TOPOLOGY_ID GetCurrentTopology()
    {
        UINT32 numPathArrayElements;
        UINT32 numModeInfoArrayElements;

        LONG result = GetDisplayConfigBufferSizes(QDC_DATABASE_CURRENT, &numPathArrayElements, &numModeInfoArrayElements);
        if (result != ERROR_SUCCESS)
            throw std::runtime_error("GetDisplayConfigBufferSizes failed: " + std::to_string(result));

        std::vector<DISPLAYCONFIG_PATH_INFO> pathArray;
        std::vector<DISPLAYCONFIG_MODE_INFO> modeInfoArray;

        pathArray.resize(numPathArrayElements);
        modeInfoArray.resize(numModeInfoArrayElements);

        DISPLAYCONFIG_TOPOLOGY_ID currentTopologyId = DISPLAYCONFIG_TOPOLOGY_FORCE_UINT32;

        result = QueryDisplayConfig(QDC_DATABASE_CURRENT,
            &numPathArrayElements, pathArray.data(),
            &numModeInfoArrayElements, modeInfoArray.data(),
            &currentTopologyId);

        if (result != ERROR_SUCCESS)
            throw std::runtime_error("QueryDisplayConfig failed: " + std::to_string(result));

        return currentTopologyId;
    }
}

MonitorManagerImpl::MonitorManagerImpl(HWND hwnd) : m_hwnd(hwnd)
{
}

MonitorManagerImpl::MonitorMode MonitorManagerImpl::GetMonitorMode() const
{
    switch (GetCurrentTopology())
    {
    case DISPLAYCONFIG_TOPOLOGY_INTERNAL:
        return MonitorMode::PCScreenOnly;
    case DISPLAYCONFIG_TOPOLOGY_CLONE:
        return MonitorMode::Clone;
    case DISPLAYCONFIG_TOPOLOGY_EXTEND:
        return MonitorMode::Extend;
    case DISPLAYCONFIG_TOPOLOGY_EXTERNAL:
        return MonitorMode::SecondScreenOnly;
    default:
        throw std::exception();
    }
}

void MonitorManagerImpl::SetMonitorMode(MonitorMode mode)
{
    DISPLAYCONFIG_TOPOLOGY_ID current = GetCurrentTopology();
    DISPLAYCONFIG_TOPOLOGY_ID topology = current;
    switch (mode)
    {
    case MonitorManagerImpl::MonitorMode::SecondScreenOnly:
        topology = DISPLAYCONFIG_TOPOLOGY_EXTERNAL;
        break;
    case MonitorManagerImpl::MonitorMode::PCScreenOnly:
        topology = DISPLAYCONFIG_TOPOLOGY_INTERNAL;
        break;
    case MonitorManagerImpl::MonitorMode::Clone:
        topology = DISPLAYCONFIG_TOPOLOGY_CLONE;
        break;
    case MonitorManagerImpl::MonitorMode::Extend:
        topology = DISPLAYCONFIG_TOPOLOGY_EXTEND; 
        break;
    }
    if (topology == current)
        return;

    LONG result = SetDisplayConfig(0, NULL, 0, NULL, topology | SDC_APPLY);
    if (result != ERROR_SUCCESS)
        throw std::runtime_error("SetDisplayConfig failed: " + std::to_string(result));

    PostMessage(m_hwnd, WM_MONITORMODECHANGED, 0, static_cast<LPARAM>(mode));
}
