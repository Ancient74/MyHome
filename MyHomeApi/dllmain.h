// dllmain.h: объявление класса модуля.

class CMyHomeApiModule : public ATL::CAtlDllModuleT< CMyHomeApiModule >
{
public :
	DECLARE_LIBID(LIBID_MyHomeApiLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_MYHOMEAPI, "{3380f557-7865-455b-ada6-030481883722}")
};

extern class CMyHomeApiModule _AtlModule;
