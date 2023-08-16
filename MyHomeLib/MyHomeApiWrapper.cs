using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyHomeLib
{
    public class MyHomeApiWrapper
    {
        private MyHomeApiLib.MyHomeController controller;

        public MyHomeApiWrapper()
        {
            controller = new MyHomeApiLib.MyHomeController();
        }

        public IMonitorManager GetMonitorManager()
        {
            return new MonitorManagerWrapper(controller);
        }

        public IDesktopManager GetDesktopManager()
        {
            return new DesktopManagerWrapper((MyHomeApiLib.IDesktopManager)controller);
        }
        
        public IClientFilter GetClientFilter()
        {
            return new ClientFilterWrapper((MyHomeApiLib.IClientFilter)controller);
        }

        public IAudioManager GetAudioManager()
        {
            return new AudioManagerWrapper((MyHomeApiLib.IAudioManager)controller);
        }
    }
}
