using MyHomeLib;

namespace MyHomeWebApi
{
    public interface IMyHomeApiService
    {
        IMonitorManager GetMonitorManager();
        IDesktopManager GetDesktopManager();
        IClientFilter GetClientFilter();
        IAudioManager GetAudioManager();
    }

    public class MyHomeApiService : IMyHomeApiService
    {
        private MyHomeApiWrapper wrapper;

        public MyHomeApiService()
        {
            wrapper = new MyHomeApiWrapper();
        }

        public IMonitorManager GetMonitorManager()
        {
            return wrapper.GetMonitorManager();
        }

        public IDesktopManager GetDesktopManager()
        {
            return wrapper.GetDesktopManager();
        }

        public IClientFilter GetClientFilter()
        {
            return wrapper.GetClientFilter();
        }

        public IAudioManager GetAudioManager()
        {
            return wrapper.GetAudioManager();
        }
    }
}
