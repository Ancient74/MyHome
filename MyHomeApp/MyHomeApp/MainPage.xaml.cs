using MyHomeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace MyHomeApp
{
    public enum ShutdownPopupResult { Cancel, NormalShutdown, ForceShutdown, NormalRestart, ForceRestart }

    public interface IShutdownPopupNavigator
    {
        Task<ShutdownPopupResult> OpenShutdownPopup();
    }

    public partial class MainPage : ContentPage, MainPageViewModel.IMainPageNavigator, IErrorHandling, IShutdownPopupNavigator, IReturnToMainPageNavigator
    {
        private IIpAddressService ipAddressService;
        private IMyHomeApi myHomeApi;

        private MainPageViewModel mainPageViewModel;

        public MainPage(string url = "")
        {
            InitializeComponent();
            ipAddressService = new IpAddressService();
            myHomeApi = new MyHomeApi(ipAddressService);
            if (!String.IsNullOrEmpty(url))
            {
                if (ipAddressService.IpAddress != null)
                {
                    myHomeApi.OpenInBrowser(url);
                    return;
                }
            }
            mainPageViewModel = new MainPageViewModel(myHomeApi, this, this, this);
            BindingContext = mainPageViewModel;
        }

        public async Task BackToMainPage(string errorMessage = "")
        {
            await Navigation.PopToRootAsync();
            if (!string.IsNullOrEmpty(errorMessage))
                ShowErrorMessage(errorMessage);
            mainPageViewModel.RefreshCommand.Execute(null);
        }

        public Task NavigateToAudioConfigPage()
        {
            var audioConfigPage = new AudioConfigPage(myHomeApi);
            audioConfigPage.BindingContext = new AudioConfigPageViewModel(myHomeApi, audioConfigPage, audioConfigPage, this);
            return Navigation.PushAsync(audioConfigPage, true);
        }

        public void NavigateToConfigPage()
        {
            var configPopup = new ConfigPopup();
            var configPageViewModel = new ConfigPopupViewModel(ipAddressService, configPopup);
            configPageViewModel.Confirmed += () => mainPageViewModel.RefreshCommand.Execute(null);
            configPopup.BindingContext = configPageViewModel;
            Navigation.ShowPopup(configPopup);
        }

        public Task NavigateToMonitorConfigPage()
        {
            var monitorConfigPage = new MonitorConfigPage();
            monitorConfigPage.BindingContext = new MonitorConfigPageViewModel(myHomeApi, monitorConfigPage, this);
            return Navigation.PushAsync(monitorConfigPage, true);
        }

        public Task NavigateToIoTPage()
        {
            var ioTPage = new IoTPage();
            ioTPage.BindingContext = new IoTPageViewModel(myHomeApi, ioTPage, ioTPage, this, ioTPage);
            return Navigation.PushAsync(ioTPage, true);
        }

        public async Task<ShutdownPopupResult> OpenShutdownPopup()
        {
            var shutdownPopup = new ShutdownPopup();
            shutdownPopup.BindingContext = new ShutdownPopupViewModel(shutdownPopup);
            return await Navigation.ShowPopupAsync(shutdownPopup);
        }

        public void ShowErrorMessage(string message)
        {
            this.DisplaySnackBarAsync(message, "", null, TimeSpan.FromSeconds(3));
        }
    }
}
