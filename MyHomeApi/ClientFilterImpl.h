#pragma once
#include <string>
#include <array>
#include <vector>
#include <map>

class ClientFilterImpl
{
public:

	ClientFilterImpl();

	class InvalidIpException : public std::exception
	{
	public:
		InvalidIpException(const std::string& ipAddress, const std::string& additionalMessage = "");
		const char* what() const override;
		
	private:
		std::string m_ipAddress;
		std::string m_additionalMessage;
		mutable std::string m_buffer;
	};

	bool IsClientAllowed(const std::wstring& ipAddress);

private:
	std::vector<std::array<byte, 6>> m_allowedMACAddresses;
	std::vector<std::wstring> m_bypassMACCheck;
};
