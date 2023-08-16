namespace MyHomeLib
{
    public enum ShutdownMode { Undefined, NormalShutdown, ForceShutdown, NormalRestart, ForceRestart }

    public interface IDesktopManager
    {
        void Shutdown(ShutdownMode shutdownMode);
        void OpenInBrowser(string url);
    }
    public class DesktopManagerWrapper : IDesktopManager
    {
        private MyHomeApiLib.IDesktopManager desktopManager;
        public DesktopManagerWrapper(MyHomeApiLib.IDesktopManager desktopManager)
        {
            this.desktopManager = desktopManager;
        }

        public void OpenInBrowser(string url)
        {
            desktopManager.OpenInBrowser(url);
        }

        public void Shutdown(ShutdownMode shutdownMode)
        {
            MyHomeApiLib.MyShutdownMode mode;
            switch (shutdownMode)
            {
                case ShutdownMode.NormalShutdown:
                    mode = MyHomeApiLib.MyShutdownMode.ShutdownMode_NormalShutdown;
                    break;
                case ShutdownMode.ForceShutdown:
                    mode = MyHomeApiLib.MyShutdownMode.ShutdownMode_ForceShutdown;
                    break;
                case ShutdownMode.NormalRestart:
                    mode = MyHomeApiLib.MyShutdownMode.ShutdownMode_NormalRestart;
                    break;
                case ShutdownMode.ForceRestart:
                    mode = MyHomeApiLib.MyShutdownMode.ShutdownMode_ForceRestart;
                    break;
                default:
                    throw new ArgumentException("Invalid argument: " + shutdownMode.ToString());
            }
            desktopManager.Shutdown(mode);
        }
    }
}
