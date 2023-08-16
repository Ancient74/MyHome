using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MyHomeApp.ViewModels;

namespace MyHomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioConfigPage : ContentPage, IErrorHandling, ActiveAudioDeviceSettingsViewModel.IGoToSelectDevicePage, ChangeAudioDevicePageViewModel.IGoBackToActiveAudioDevicePage
    {
        private IMyHomeApi myHomeApi;
        public AudioConfigPage(IMyHomeApi myHomeApi)
        {
            this.myHomeApi = myHomeApi;
            InitializeComponent();
        }

        public void ShowErrorMessage(string message)
        {
            this.DisplaySnackBarAsync(message, "", null, TimeSpan.FromSeconds(3));
        }

        public async Task GoToSelectDevicePage(AudioDeviceType type, IReturnToMainPageNavigator returnToMainPageNavigator)
        {
            var changeDevicePage = new ChangeDevicePage();
            var changeDevicePageViewModel = new ChangeAudioDevicePageViewModel(myHomeApi, type, this, this, returnToMainPageNavigator);
            changeDevicePage.BindingContext = changeDevicePageViewModel;
            await Navigation.PushAsync(changeDevicePage, true);
        }

        public async Task GoBackToActiveAudioDevicePage()
        {
            await Navigation.PopAsync();
            var viewModel = BindingContext as AudioConfigPageViewModel;
            if (viewModel != null)
                viewModel.RefreshCommand.Execute(null);
        }
    }
}
