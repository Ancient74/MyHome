#pragma once
#include <array>
#include <string>

class MACAddressParser
{
public:
	static bool Parse(const std::wstring& macAddress, std::array<byte, 6>& out);
};

