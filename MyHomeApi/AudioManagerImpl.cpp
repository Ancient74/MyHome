#include "pch.h"
#include "AudioManagerImpl.h"
#include "AudioController.h"
#include "ErrorHandling.h"

namespace
{
	ATL::CComPtr<IPolicyConfig> CreatePolicyConfig()
	{
		ATL::CComPtr<IPolicyConfig> policyConfig;

		HRESULT hr = CoCreateInstance(__uuidof(CPolicyConfigClient), NULL, CLSCTX_ALL, __uuidof(IPolicyConfig), (LPVOID*)&policyConfig);
		if (!policyConfig) {
			hr = CoCreateInstance(__uuidof(CPolicyConfigClient), NULL, CLSCTX_ALL, __uuidof(IPolicyConfig10), (LPVOID*)&policyConfig);
		}
		if (!policyConfig) {
			hr = CoCreateInstance(__uuidof(CPolicyConfigClient), NULL, CLSCTX_ALL, __uuidof(IPolicyConfig7), (LPVOID*)&policyConfig);
		}
		if (!policyConfig) {
			hr = CoCreateInstance(__uuidof(CPolicyConfigClient), NULL, CLSCTX_ALL, __uuidof(IPolicyConfigVista), (LPVOID*)&policyConfig);
		}
		if (!policyConfig) {
			hr = CoCreateInstance(__uuidof(CPolicyConfigClient), NULL, CLSCTX_ALL, __uuidof(IPolicyConfig10_1), (LPVOID*)&policyConfig);
		}
		if (!policyConfig)
			throw std::exception("PolicyConfig interface has not been created");

		return policyConfig;
	}
}

AudioManagerImpl::AudioManagerImpl() 
{
	CHECK(m_deviceEnumerator.CoCreateInstance(__uuidof(MMDeviceEnumerator), nullptr, CLSCTX_ALL));
	m_policyConfig = CreatePolicyConfig();
	m_outputAudioController = winrt::make_self<AudioController>(m_deviceEnumerator, m_policyConfig, AudioDeviceType::Output);
	m_inputAudioController = winrt::make_self<AudioController>(m_deviceEnumerator, m_policyConfig, AudioDeviceType::Input);
}

winrt::com_ptr<IAudioController> AudioManagerImpl::GetAudioInputController() const
{
	return m_inputAudioController;
}

winrt::com_ptr<IAudioController> AudioManagerImpl::GetAudioOutputController() const
{
	return m_outputAudioController;
}
