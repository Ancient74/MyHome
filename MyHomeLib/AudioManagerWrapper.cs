namespace MyHomeLib
{
    public interface IAudioDevice
    {
        string GetName();
        string GetId();
        void ToggleMute();
        bool IsMuted();
        float VolumeLevel { get; set; }
    }

    public interface IAudioController
    {
        IEnumerable<IAudioDevice> EnumerateAudioDevices();
        IAudioDevice GetActiveAudioDevice();
        IAudioDevice GetAudioDevice(string deviceId);
        IAudioDevice ActivateDevice(string deviceId);
    }

    public interface IAudioManager
    {
        IAudioController GetAudioInputController();
        IAudioController GetAudioOutputController();
    }

    public class AudioDeviceWrapper : IAudioDevice
    {
        private MyHomeApiLib.IAudioDevice device;

        public AudioDeviceWrapper(MyHomeApiLib.IAudioDevice device)
        {
            this.device = device;
        }
        public string GetId()
        {
            return device.GetId();
        }

        public string GetName()
        {
            return device.GetName();
        }

        public float VolumeLevel { get => device.GetVolumeLevel(); set => device.SetVolumeLevel(value); }

        public bool IsMuted()
        {
            return Convert.ToBoolean(device.IsMuted());
        }

        public void ToggleMute()
        {
            device.ToggleMute();
        }
    }

    public class AudioControllerWrapper : IAudioController
    {
        private MyHomeApiLib.IAudioController audioController;

        public AudioControllerWrapper(MyHomeApiLib.IAudioController audioController)
        {
            this.audioController = audioController;
        }

        public IAudioDevice ActivateDevice(string deviceId)
        {
            return new AudioDeviceWrapper(audioController.ActivateDevice(deviceId));
        }

        public IAudioDevice GetAudioDevice(string deviceId)
        {
            return new AudioDeviceWrapper(audioController.GetAudioDevice(deviceId));
        }

        public IEnumerable<IAudioDevice> EnumerateAudioDevices()
        {
            var arr = audioController.EnumerateAudioDevices();
            for (int i = 0; i < arr.Length; i++)
            {
                object? value = arr.GetValue(i);
                if (value != null)
                {
                    yield return new AudioDeviceWrapper((MyHomeApiLib.IAudioDevice)value);
                }
            }
        }

        public IAudioDevice GetActiveAudioDevice()
        {
            return new AudioDeviceWrapper(audioController.GetActiveAudioDevice());
        }
    }

    public class AudioManagerWrapper : IAudioManager
    {
        private MyHomeApiLib.IAudioManager audioManager;

        public AudioManagerWrapper(MyHomeApiLib.IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        public IAudioController GetAudioInputController()
        {
            return new AudioControllerWrapper(audioManager.GetAudioInputController());
        }

        public IAudioController GetAudioOutputController()
        {
            return new AudioControllerWrapper(audioManager.GetAudioOutputController());
        }
    }
}
