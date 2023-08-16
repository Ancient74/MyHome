#pragma once
#include <winrt/base.h>
#include "MyHomeApi_i.h"
#include <mmdeviceapi.h>
#include <functional>
#include "PolicyConfig.h"
#pragma comment(lib, "windowsapp")

class AudioControllerImpl;

enum class AudioDeviceType
{
    Input, Output
};
class AudioController : public winrt::implements<AudioController, IAudioController>
{
public:
    AudioController(const ATL::CComPtr<IMMDeviceEnumerator>& deviceEnumerator, const ATL::CComPtr<IPolicyConfig>& policyConfig, AudioDeviceType deviceType);
    HRESULT STDMETHODCALLTYPE EnumerateAudioDevices(SAFEARRAY** audioDevice) override;
    HRESULT STDMETHODCALLTYPE GetAudioDevice(BSTR deviceId, IAudioDevice** audioDevice) override;
    HRESULT STDMETHODCALLTYPE GetActiveAudioDevice(IAudioDevice** audioDevice) override;
    HRESULT STDMETHODCALLTYPE ActivateDevice(BSTR deviceId, IAudioDevice** audioDevice) override;

private:
    std::unique_ptr<AudioControllerImpl> m_impl;
};

class AudioControllerImpl
{
public:
    AudioControllerImpl(AudioDeviceType deviceType, const ATL::CComPtr<IMMDeviceEnumerator>& deviceEnumerator, const ATL::CComPtr<IPolicyConfig>& policyConfig);

    std::vector<winrt::com_ptr<IAudioDevice>> EnumerateAudioDevices();
    winrt::com_ptr<IAudioDevice> GetActiveAudioDevice() const;
    winrt::com_ptr<IAudioDevice> GetAudioDevice(const std::wstring& deviceId);
    winrt::com_ptr<IAudioDevice> ActivateDevice(const std::wstring& deviceId);

private:
    void EnumerateAudioDevice(std::function<void(ATL::CComPtr<IMMDevice>)> callback);
    AudioDeviceType m_deviceType;
    ATL::CComPtr<IMMDeviceEnumerator> m_deviceEnumerator;
    ATL::CComPtr<IPolicyConfig> m_policyConfig;
};
