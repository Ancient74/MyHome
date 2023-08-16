using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyHomeApp.ViewModels
{
    internal class ActiveAudioDeviceSettingsViewModel : ViewModelBase
    {
        private string name;
        private string id;
        private bool isMuted;
        private float volumeLevel;

        private bool isEnabled = true;

        private IObserver<AudioDeviceModelType> modelObserver;
        private AudioDeviceType audioDeviceType;
        private IGoToSelectDevicePage goToSelectDevicePage;
        private IReturnToMainPageNavigator returnToMainPageNavigator;

        public struct AudioDeviceModelType
        {
            public AudioDeviceModelType(AudioDeviceType audioDeviceType, AudioDeviceModel model)
            {
                AudioDeviceType = audioDeviceType;
                Model = model;
            }
            public AudioDeviceType AudioDeviceType { get; }
            public AudioDeviceModel Model { get; }
        }


        public interface IGoToSelectDevicePage
        {
            Task GoToSelectDevicePage(AudioDeviceType type, IReturnToMainPageNavigator returnToMainPageNavigator);
        }

        public ActiveAudioDeviceSettingsViewModel(AudioDeviceType audioDeviceType, IObserver<AudioDeviceModelType> modelObserver, IGoToSelectDevicePage goToSelectDevicePage, IReturnToMainPageNavigator returnToMainPageNavigator)
        {
            this.modelObserver = modelObserver;
            this.audioDeviceType = audioDeviceType;
            this.goToSelectDevicePage = goToSelectDevicePage;
            this.returnToMainPageNavigator = returnToMainPageNavigator;
            UpdateVolumeLevelCommand = new RelayCommand(UpdateVolumeLevel, CanExecute);
            ToggleMuteCommand = new RelayCommand(ToggleMute, CanExecute);
            ChangeDeviceCommand = new RelayCommand(ChangeDevice, CanExecute);
        }

        private bool CanExecute(object param) => IsEnabled;

        public void UpdateModel(AudioDeviceModel model)
        {
            Name = model.Name;
            id = model.Id;
            VolumeLevel = model.VolumeLevel * 100.0f;
            IsMuted = model.IsMuted;
        }

        private async void ChangeDevice(object param)
        {
            await goToSelectDevicePage.GoToSelectDevicePage(audioDeviceType, returnToMainPageNavigator);
        }

        public RelayCommand UpdateVolumeLevelCommand { get; }
        public RelayCommand ToggleMuteCommand { get; }
        public RelayCommand ChangeDeviceCommand { get; }

        public string Name { get => name; set
            {
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged();
            }
        }

        public bool IsMuted
        {
            get => isMuted; set
            {
                if (isMuted == value)
                    return;
                isMuted = value;
                OnPropertyChanged();
            }
        }

        public float VolumeLevel
        {
            get => volumeLevel; set
            {
                volumeLevel = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled { get => isEnabled; set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;

                UpdateVolumeLevelCommand.RefreshCanExecute();
                ToggleMuteCommand.RefreshCanExecute();
                ChangeDeviceCommand.RefreshCanExecute();
            }
        }

        private void UpdateVolumeLevel(object param)
        {
            modelObserver.OnNext(new AudioDeviceModelType(audioDeviceType, GetModel()));
        }

        private void ToggleMute(object param)
        {
            modelObserver.OnNext(new AudioDeviceModelType(audioDeviceType, GetModel(true)));
        }

        private AudioDeviceModel GetModel(bool toggleMute = false)
        {
            return new AudioDeviceModel(Name, id, VolumeLevel / 100.0f, toggleMute ? !IsMuted : IsMuted);
        }
    }

}
