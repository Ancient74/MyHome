#pragma once
#include <winerror.h>
#include <comdef.h>

inline void CHECK(HRESULT hr)
{
	if (SUCCEEDED(hr))
		return;

	_com_error ce(hr);
	throw ce;
}
