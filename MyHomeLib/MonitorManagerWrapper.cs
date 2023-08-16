namespace MyHomeLib
{
    public enum MonitorMode { Undefined, PCScreenOnly, SecondScreenOnly, Extend, Duplicate }

    public interface IMonitorManager
    {
        void SetMonitorMode(MonitorMode monitorMode);
        void OpenBigPicture();
    }

    public class MonitorManagerWrapper : IMonitorManager
    {
        private MyHomeApiLib.IMonitorManager monitorManager;
        public MonitorManagerWrapper(MyHomeApiLib.IMonitorManager monitorManager)
        {
            this.monitorManager = monitorManager;
        }

        public void SetMonitorMode(MonitorMode monitorMode)
        {
            MyHomeApiLib.MyMonitorMode mode;
            switch (monitorMode)
            {
                case MonitorMode.PCScreenOnly:
                    mode = MyHomeApiLib.MyMonitorMode.MonitorMode_PCScreenOnly;
                    break;
                case MonitorMode.SecondScreenOnly:
                    mode = MyHomeApiLib.MyMonitorMode.MonitorMode_SecondScreenOnly;
                    break;
                case MonitorMode.Extend:
                    mode = MyHomeApiLib.MyMonitorMode.MonitorMode_Extend;
                    break;
                case MonitorMode.Duplicate:
                    mode = MyHomeApiLib.MyMonitorMode.MonitorMode_Duplicate;
                    break;
                default:
                    throw new ArgumentException("Invalid argument: " + monitorMode.ToString());
            }
            monitorManager.SetMonitorMode(mode);
        }

        public void OpenBigPicture()
        {
            monitorManager.OpenBigPicture();
        }
    }
}
