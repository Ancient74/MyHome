#include "pch.h"
#include "ClientFilterImpl.h"
#include "MACAddressParser.h"
#include <Iphlpapi.h>
#include <Ws2tcpip.h>
#include <locale>
#include <codecvt>
#include <vector>
#include <algorithm>
#include <fstream>
#include <filesystem>
#pragma comment (lib, "Iphlpapi.lib")
#pragma comment(lib, "Ws2_32.lib")

namespace
{
	std::string ToAscii(const std::wstring& unicodeString)
	{
		std::string str(unicodeString.length(), 0);
		std::transform(unicodeString.begin(), unicodeString.end(), str.begin(), [](wchar_t c) {
			return static_cast<char>(c);
			});
		return str;
	}

	std::string GetEnvVariable(const std::string& key)
	{
		char* buffer;
		size_t size;
		if (_dupenv_s(&buffer, &size, key.c_str()) == 0)
		{
			std::unique_ptr<char, decltype(&free)> release(buffer, &free);
			return buffer ? std::string(buffer) : "";
		}
		return "";
	}
}

ClientFilterImpl::ClientFilterImpl()
{
	std::string pathString = GetEnvVariable("MyHomePath");
	std::filesystem::path path(pathString);
	std::filesystem::path fileName("MACAddressAllowList.txt");
	std::wifstream file(path / fileName, std::ios_base::in | std::ios_base::app);
	if (file.is_open()) {
		std::wstring line;
		while (std::getline(file, line)) {
			std::array<byte, 6> macAddress;
			if (MACAddressParser::Parse(line, macAddress))
				m_allowedMACAddresses.push_back(macAddress);
		}
		file.close();
	}
}

bool ClientFilterImpl::IsClientAllowed(const std::wstring& ipAddress)
{
	if (std::find(m_bypassMACCheck.begin(), m_bypassMACCheck.end(), ipAddress) != m_bypassMACCheck.end())
		return true;

	IN_ADDR address{};
	auto result = InetPton(AF_INET, ipAddress.c_str(), &address);
	if (result == 0)
		throw InvalidIpException(ToAscii(ipAddress));
	else if (result == -1)
		throw InvalidIpException(ToAscii(ipAddress), "Error code: " + WSAGetLastError());

	ULONG bytesLen = 6;
	std::array<byte, 6> macBuffer;

	result = SendARP(address.S_un.S_addr, 0, macBuffer.data(), &bytesLen);
	if (result != NO_ERROR)
		return false;

	bool isAllowed = std::find_if(m_allowedMACAddresses.begin(), m_allowedMACAddresses.end(), [&macBuffer](const auto& allowedMAC) {
		return allowedMAC == macBuffer;
		}) != m_allowedMACAddresses.end();

	if (isAllowed)
		m_bypassMACCheck.push_back(ipAddress);

	return isAllowed;
}

ClientFilterImpl::InvalidIpException::InvalidIpException(const std::string& ipAddress, const std::string& additionalMessage) : m_ipAddress(ipAddress), m_additionalMessage(additionalMessage)
{
}

const char* ClientFilterImpl::InvalidIpException::what() const
{
	m_buffer = "Invalid IP address: " + m_ipAddress + "\n";
	if (!m_additionalMessage.empty())
		m_buffer += "Error message: " + m_additionalMessage;
	return m_buffer.c_str();
}
