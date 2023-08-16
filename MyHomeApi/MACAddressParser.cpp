#include "pch.h"
#include "MACAddressParser.h"
#include "StringUtils.h"

bool MACAddressParser::Parse(const std::wstring& macAddress, std::array<byte, 6>& out)
{
    const size_t macAddressSize = 17;
    if (macAddress.size() != macAddressSize)
        return false;

    auto split = StringUtils::Split(macAddress, ':');
    if (split.size() != 6)
        return false;

    for (size_t i = 0; i < 6; i++)
    {
        unsigned int b;
        std::wstringstream ss;
        ss << std::hex << split[i];
        ss >> b;
        out[i] = static_cast<byte>(b);
    }

    return true;
}
