#pragma once
#include <winrt/base.h>
#include "MyHomeApi_i.h"
#include <mmdeviceapi.h>
#include "PolicyConfig.h"
#pragma comment(lib, "windowsapp")

class AudioManagerImpl
{
public:
	AudioManagerImpl();

	winrt::com_ptr<IAudioController> GetAudioInputController() const;
	winrt::com_ptr<IAudioController> GetAudioOutputController() const;

private:
	winrt::com_ptr<IAudioController> m_inputAudioController;
	winrt::com_ptr<IAudioController> m_outputAudioController;
	ATL::CComPtr<IMMDeviceEnumerator> m_deviceEnumerator;
	ATL::CComPtr<IPolicyConfig> m_policyConfig;
};
