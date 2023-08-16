using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyHomeApp.ViewModels
{
    internal class AudioConfigPageViewModel : ViewModelBase, IObserver<ActiveAudioDeviceSettingsViewModel.AudioDeviceModelType>
    {
        private IMyHomeApi myHomeApi;
        private IErrorHandling errorHandling;
        private bool isRefreshing;
        private Dictionary<AudioDeviceType, ActiveAudioDeviceSettingsViewModel> audioDeviceViewModels;
        private IReturnToMainPageNavigator returnToMainPageNavigator;

        public AudioConfigPageViewModel(IMyHomeApi myHomeApi, IErrorHandling errorHandling, ActiveAudioDeviceSettingsViewModel.IGoToSelectDevicePage goToSelectDevicePage, IReturnToMainPageNavigator returnToMainPageNavigator)
        {
            this.myHomeApi = myHomeApi;
            this.errorHandling = errorHandling;
            this.returnToMainPageNavigator = returnToMainPageNavigator;
            audioDeviceViewModels = new Dictionary<AudioDeviceType, ActiveAudioDeviceSettingsViewModel>();
            foreach(var type in new[] { AudioDeviceType.Input, AudioDeviceType.Output })
            {
                audioDeviceViewModels[type] = new ActiveAudioDeviceSettingsViewModel(type, this, goToSelectDevicePage, returnToMainPageNavigator);
            }
            RefreshCommand = new RelayCommand(Refresh);
            Refresh(null);
        }
        public ICommand RefreshCommand { get; }


        public bool IsRefreshing
        {
            get => isRefreshing; set
            {
                if (isRefreshing == value) return;
                isRefreshing = value;
                foreach (var item in audioDeviceViewModels)
                {
                    item.Value.IsEnabled = !value;
                }
                OnPropertyChanged();
            }
        }

        public ActiveAudioDeviceSettingsViewModel ActiveOutputDevice { 
            get => audioDeviceViewModels[AudioDeviceType.Output];
            set
            {
                if (ActiveOutputDevice == value)
                    return;
                ActiveOutputDevice = value;
                OnPropertyChanged();
            } }

        public ActiveAudioDeviceSettingsViewModel ActiveInputDevice
        {
            get => audioDeviceViewModels[AudioDeviceType.Input];
            set
            {
                if (ActiveInputDevice == value)
                    return;
                ActiveInputDevice = value;
                OnPropertyChanged();
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(ActiveAudioDeviceSettingsViewModel.AudioDeviceModelType model)
        {
            try
            {
                await myHomeApi.UpdateDevice(model.AudioDeviceType, model.Model);
            }
            catch (OperationCanceledException)
            {
                await returnToMainPageNavigator.BackToMainPage("Request timeout");
                return;
            }
            catch (Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
            }
            Refresh(null);
        }

        private async void Refresh(object param)
        {
            IsRefreshing = true;
            try
            {
                ActiveOutputDevice.UpdateModel(await myHomeApi.GetActiveAudioDevice(AudioDeviceType.Output));
                ActiveInputDevice.UpdateModel(await myHomeApi.GetActiveAudioDevice(AudioDeviceType.Input));
            }
            catch (OperationCanceledException)
            {
                await returnToMainPageNavigator.BackToMainPage("Request timeout");
                return;
            }
            catch (Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
            }
            IsRefreshing = false;
        }
    }
}
