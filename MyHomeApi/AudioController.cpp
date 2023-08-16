#include "pch.h"
#include "AudioController.h"
#include <comdef.h>
#include "AudioDevice.h"
#include "ErrorHandling.h"
#include <atlsafe.h>
#include <stdexcept>

namespace
{
	EDataFlow DeviceTypeToDataFlow(AudioDeviceType type)
	{
		switch (type)
		{
		case AudioDeviceType::Input:
			return EDataFlow::eCapture;
		case AudioDeviceType::Output:
			return EDataFlow::eRender;
		}
		return EDataFlow::EDataFlow_enum_count;
	}
}

AudioController::AudioController(const ATL::CComPtr<IMMDeviceEnumerator>& deviceEnumerator, const ATL::CComPtr<IPolicyConfig>& policyConfig, AudioDeviceType deviceType)
{
	m_impl = std::make_unique<AudioControllerImpl>(deviceType, deviceEnumerator, policyConfig);
}

HRESULT __stdcall AudioController::EnumerateAudioDevices(SAFEARRAY** audioDevice)
{
	if (!audioDevice)
		return E_POINTER;

	auto devices = m_impl->EnumerateAudioDevices();
	ATL::CComSafeArray<VARIANT> result;
	result.Create(static_cast<ULONG>(devices.size()));
	for (int i = 0; i < devices.size(); i++)
	{
		result[i] = winrt::detach_abi(devices[i]);
	}
	*audioDevice = result.Detach();

	return S_OK;
}

HRESULT __stdcall AudioController::GetAudioDevice(BSTR deviceId, IAudioDevice** audioDevice)
{
	if (!audioDevice)
		return E_POINTER;

	auto device = m_impl->GetAudioDevice(deviceId);
	*audioDevice = winrt::detach_abi(device);

	return S_OK;
}

HRESULT __stdcall AudioController::GetActiveAudioDevice(IAudioDevice** audioDevice)
{
	if (!audioDevice)
		return E_POINTER;

	auto device = m_impl->GetActiveAudioDevice();
	*audioDevice = winrt::detach_abi(device);

	return S_OK;
}

HRESULT __stdcall AudioController::ActivateDevice(BSTR deviceId, IAudioDevice** audioDevice)
{
	if (!deviceId)
		return E_INVALIDARG;
	if (!audioDevice)
		return E_POINTER;

	auto device = m_impl->ActivateDevice(StringUtils::ToWstring(deviceId));
	*audioDevice = winrt::detach_abi(device);

	return S_OK;
}

AudioControllerImpl::AudioControllerImpl(AudioDeviceType deviceType, const ATL::CComPtr<IMMDeviceEnumerator>& deviceEnumerator, const ATL::CComPtr<IPolicyConfig>& policyConfig)
	: m_deviceType(deviceType)
	, m_deviceEnumerator(deviceEnumerator)
	, m_policyConfig(policyConfig)
{
}

std::vector<winrt::com_ptr<IAudioDevice>> AudioControllerImpl::EnumerateAudioDevices()
{
	std::vector<winrt::com_ptr<IAudioDevice>>result;

	EnumerateAudioDevice([&result](auto device)
		{
			result.push_back(winrt::make_self<AudioDevice>(device));
		});
	return result;
}

winrt::com_ptr<IAudioDevice> AudioControllerImpl::GetActiveAudioDevice() const
{
	ATL::CComPtr<IMMDevice> defaultDevice;
	CHECK(m_deviceEnumerator->GetDefaultAudioEndpoint(DeviceTypeToDataFlow(m_deviceType), ERole::eConsole, &defaultDevice));
	return winrt::make_self<AudioDevice>(defaultDevice);
}

winrt::com_ptr<IAudioDevice> AudioControllerImpl::ActivateDevice(const std::wstring& deviceId)
{
	CHECK(m_policyConfig->SetDefaultEndpoint(deviceId.c_str(), ERole::eConsole));
	return GetAudioDevice(deviceId);
}

winrt::com_ptr<IAudioDevice> AudioControllerImpl::GetAudioDevice(const std::wstring& deviceId)
{
	winrt::com_ptr<IAudioDevice> result;
	EnumerateAudioDevice([&deviceId, &result](ATL::CComPtr<IMMDevice> device)
		{
			StringUtils::StringTaskMemWrapper idWrapper;
			device->GetId(&idWrapper);
			if (idWrapper == deviceId)
				result = winrt::make_self<AudioDevice>(device);

		});
	return result;
}

void AudioControllerImpl::EnumerateAudioDevice(std::function<void(ATL::CComPtr<IMMDevice>)> callback)
{
	ATL::CComPtr<IMMDeviceCollection> deviceCollection;
	CHECK(m_deviceEnumerator->EnumAudioEndpoints(DeviceTypeToDataFlow(m_deviceType), DEVICE_STATE_ACTIVE, &deviceCollection));

	UINT count{};
	CHECK(deviceCollection->GetCount(&count));

	for (UINT i = 0; i < count; i++)
	{
		ATL::CComPtr<IMMDevice> device;
		CHECK(deviceCollection->Item(i, &device));

		callback(device);
	}
}
