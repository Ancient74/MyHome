#pragma once
#include <vector>
#include <string>
#include <sstream>

namespace StringUtils
{
	inline std::vector<std::wstring> Split(const std::wstring& str, wchar_t delimeter)
	{
        std::vector<std::wstring> result;
        std::wstringstream ss(str);
        std::wstring item;

        while (std::getline(ss, item, delimeter)) {
            result.push_back(item);
        }

        return result;
	}

	inline std::wstring ToWstring(const BSTR& bstr)
	{
		if (!bstr)
			return L"";
		return std::wstring(bstr, SysStringLen(bstr));
	}

	inline BSTR ToBSTR(const std::wstring& str)
	{
		return SysAllocStringLen(str.data(), static_cast<UINT>(str.size()));
	}

	class StringTaskMemWrapper
	{
	public:
		wchar_t** operator&() { return &m_buffer; }

		~StringTaskMemWrapper()
		{
			CoTaskMemFree(m_buffer);
		}

		bool operator==(const StringTaskMemWrapper& other) const
		{
			return wcscmp(m_buffer, other.m_buffer) == 0;
		}

		bool operator==(const std::wstring& other) const
		{
			return other == m_buffer;
		}

		operator std::wstring() const
		{
			return m_buffer;
		}

	private:
		wchar_t* m_buffer;
	};
}
