using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyHomeApp.ViewModels
{

    public interface IReturnToMainPageNavigator
    {
        Task BackToMainPage(string errorMessage = "");
    }

    internal class MainPageViewModel : ViewModelBase
    {
        public interface IMainPageNavigator
        {
            Task NavigateToMonitorConfigPage();
            Task NavigateToAudioConfigPage();
            void NavigateToConfigPage();
            Task NavigateToIoTPage();
        }

        private IMainPageNavigator mainPageNavigator;
        private IErrorHandling errorHandling;
        private IShutdownPopupNavigator shutdownPopupNavigator;
        private bool isRefreshing;
        private IMyHomeApi myHomeApi;
        private StatusIndicatorViewModel statusIndicatorViewModel;

        public MainPageViewModel(IMyHomeApi myHomeApi, IMainPageNavigator mainPageNavigator, IErrorHandling errorHandling, IShutdownPopupNavigator shutdownPopupNavigator)
        {
            this.myHomeApi = myHomeApi;
            this.mainPageNavigator = mainPageNavigator;
            this.errorHandling = errorHandling;
            this.shutdownPopupNavigator = shutdownPopupNavigator;
            statusIndicatorViewModel = new StatusIndicatorViewModel(myHomeApi, errorHandling);
            GoToMonitorConfigPageCommand = new RelayCommand(GoToMonitorConfigPage, CanExecute);
            GoToAudioConfigPageCommand = new RelayCommand(GoToAudioConfigPage, CanExecute);
            GoToConfigPageCommand = new RelayCommand(GoToConfigPage, CanExecute);
            GoToIoTPageCommand = new RelayCommand(GoToIoTPage, CanExecute);
            PCShutdownCommand = new RelayCommand(PCShutdown, CanExecute);
            WakeOnLANCommand = new RelayCommand(WakeOnLAN, CanExecute);
            RefreshCommand = new RelayCommand(Refresh);
        }

        public RelayCommand GoToMonitorConfigPageCommand { get; }
        public RelayCommand GoToAudioConfigPageCommand { get; }
        public RelayCommand GoToConfigPageCommand { get; }
        public RelayCommand GoToIoTPageCommand { get; }
        public RelayCommand PCShutdownCommand { get; }
        public ICommand RefreshCommand { get; }
        public RelayCommand WakeOnLANCommand { get; }

        public bool IsRefreshing { get => isRefreshing; set
            {
                if (isRefreshing == value) return;
                isRefreshing = value;

                foreach (var item in new[] { GoToMonitorConfigPageCommand, GoToAudioConfigPageCommand, GoToConfigPageCommand, PCShutdownCommand, GoToIoTPageCommand, WakeOnLANCommand })
                {
                    item.RefreshCanExecute();
                }

                OnPropertyChanged();
            } }

        public StatusIndicatorViewModel StatusIndicatorViewModel { get => statusIndicatorViewModel; }

        private bool CanExecute(object param) => !IsRefreshing;

        private async void GoToMonitorConfigPage(object param)
        {
            await mainPageNavigator.NavigateToMonitorConfigPage();
        }

        private async void GoToAudioConfigPage(object param)
        {
            await mainPageNavigator.NavigateToAudioConfigPage();
        }

        private void GoToConfigPage(object param)
        {
            mainPageNavigator.NavigateToConfigPage();
        }

        private void GoToIoTPage(object param)
        {
            mainPageNavigator.NavigateToIoTPage();
        }

        private async void WakeOnLAN(object obj)
        {
            try
            {
                await myHomeApi.WakeOnLan();
            }
            catch (Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
            }
        }

        private async void PCShutdown(object param)
        {
            var result = await shutdownPopupNavigator.OpenShutdownPopup();
            try
            {
                switch (result)
                {
                    case ShutdownPopupResult.Cancel:
                        return;
                    case ShutdownPopupResult.NormalShutdown:
                        await myHomeApi.PCShutdown(ShutdownMode.NormalShutdown);
                        break;
                    case ShutdownPopupResult.ForceShutdown:
                        await myHomeApi.PCShutdown(ShutdownMode.ForceShutdown);
                        break;
                    case ShutdownPopupResult.NormalRestart:
                        await myHomeApi.PCShutdown(ShutdownMode.NormalRestart);
                        break;
                    case ShutdownPopupResult.ForceRestart:
                        await myHomeApi.PCShutdown(ShutdownMode.ForceRestart);
                        break;
                }
            } catch (Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
            }
            
        }

        private async void Refresh(object param)
        {
            IsRefreshing = true;
            await statusIndicatorViewModel.Refresh();
            IsRefreshing = false;
        }
    }
}
