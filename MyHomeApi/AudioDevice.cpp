#include "pch.h"
#include "AudioDevice.h"
#include "ErrorHandling.h"
#include <Functiondiscoverykeys_devpkey.h>
#include <propvarutil.h>
#include <stdexcept>
#pragma comment(lib, "Propsys.lib")

AudioDeviceImpl::AudioDeviceImpl(const ATL::CComPtr<IMMDevice>& device)
	: m_device(device)
{
	CHECK(device->Activate(__uuidof(IAudioEndpointVolume), CLSCTX_INPROC_SERVER, nullptr, reinterpret_cast<void**>(&m_volumeEndpoint)));
	m_volumeEndpoint->GetVolumeRange(&m_mindB, &m_maxdB, &m_incrementdB);
	
	ATL::CComPtr<IPropertyStore> store;
	CHECK(m_device->OpenPropertyStore(STGM_READ, &store));

	PROPVARIANT variant;
	store->GetValue(PKEY_Device_FriendlyName, &variant);

	STRRET value;
	CHECK(PropVariantToStrRet(variant, &value));

	m_name = value.pOleStr;
	StringUtils::StringTaskMemWrapper idWrapper;
	device->GetId(&idWrapper);
	m_id = idWrapper;
}

const std::wstring& AudioDeviceImpl::GetName() const
{
	return m_name;
}

const std::wstring& AudioDeviceImpl::GetId() const
{
	return m_id;
}

void AudioDeviceImpl::ToggleMute()
{
	bool isMuted = IsMuted();
	m_volumeEndpoint->SetMute(isMuted == FALSE, nullptr);
}

bool AudioDeviceImpl::IsMuted()
{
	BOOL isMuted;
	CHECK(m_volumeEndpoint->GetMute(&isMuted));

	return isMuted == TRUE;
}

float AudioDeviceImpl::GetVolumeLevel() const
{
	float level;
	CHECK(m_volumeEndpoint->GetMasterVolumeLevelScalar(&level));
	return level;
}

void AudioDeviceImpl::SetVolumeLevel(float volumeLevel)
{
	if (!(volumeLevel >= 0 && volumeLevel <= 1.0f))
		throw std::invalid_argument("Volume is out of range [0, 1]: " + std::to_string(volumeLevel));

	CHECK(m_volumeEndpoint->SetMasterVolumeLevelScalar(volumeLevel, nullptr));
}



////////////////////////////////////////////////////////////////////////////////////



AudioDevice::AudioDevice(const ATL::CComPtr<IMMDevice>& device) : m_impl(std::make_unique<AudioDeviceImpl>(device))
{
}


HRESULT __stdcall AudioDevice::GetName(BSTR* name)
{
	if (!name)
		return E_POINTER;

	*name = StringUtils::ToBSTR(m_impl->GetName());

	return S_OK;
}

HRESULT __stdcall AudioDevice::GetId(BSTR* deviceId)
{
	if (!deviceId)
		return E_POINTER;

	*deviceId = StringUtils::ToBSTR(m_impl->GetId());

	return S_OK;
}


HRESULT __stdcall AudioDevice::SetVolumeLevel(float volumeLevel)
{
	try {
		m_impl->SetVolumeLevel(volumeLevel);
	}
	catch (const std::invalid_argument&)
	{
		return E_INVALIDARG;
	}
	return S_OK;
}

HRESULT __stdcall AudioDevice::GetVolumeLevel(float* volumeLevel)
{
	if (!volumeLevel)
		return E_POINTER;

	*volumeLevel = m_impl->GetVolumeLevel();
	return S_OK;
}

HRESULT __stdcall AudioDevice::ToggleMute()
{
	m_impl->ToggleMute();
	return S_OK;
}

HRESULT __stdcall AudioDevice::IsMuted(BOOL* isMuted)
{
	if (!isMuted)
		return E_POINTER;

	*isMuted = m_impl->IsMuted();
	return S_OK;
}
