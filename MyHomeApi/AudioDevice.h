#pragma once
#include <winrt/base.h>
#include "MyHomeApi_i.h"
#include <mmdeviceapi.h>
#include <Endpointvolume.h>
#include "StringUtils.h"
#pragma comment(lib, "windowsapp")

class AudioDeviceImpl;

class AudioDevice : public winrt::implements<AudioDevice, IAudioDevice>
{
public:
    AudioDevice(const ATL::CComPtr<IMMDevice>& device);

    HRESULT STDMETHODCALLTYPE GetName(BSTR* name) override;
    HRESULT STDMETHODCALLTYPE GetId(BSTR* deviceId) override;

    HRESULT STDMETHODCALLTYPE ToggleMute() override;
    HRESULT STDMETHODCALLTYPE IsMuted(BOOL* isMuted) override;

    HRESULT STDMETHODCALLTYPE GetVolumeLevel(float* volumeLevel) override;
    HRESULT STDMETHODCALLTYPE SetVolumeLevel(float volumeLevel) override;

private:
    std::unique_ptr<AudioDeviceImpl> m_impl;
};

class AudioDeviceImpl
{
public:
    AudioDeviceImpl(const ATL::CComPtr<IMMDevice>& device);

    const std::wstring& GetName() const;
    const std::wstring& GetId() const;

    void ToggleMute();
    bool IsMuted();

    float GetVolumeLevel() const;
    void SetVolumeLevel(float volumeLevel);

private:
    ATL::CComPtr<IMMDevice> m_device;
    ATL::CComPtr<IAudioEndpointVolume> m_volumeEndpoint;

    std::wstring m_name;
    std::wstring m_id;
    float m_mindB;
    float m_maxdB;
    float m_incrementdB;
};
