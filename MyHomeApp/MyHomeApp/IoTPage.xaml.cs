using MyHomeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IoTPage : ContentPage, IErrorHandling, IoTPageViewModel.IIoTPageNavigator, IoTPageViewModel.IUserPrompt
    {
        public IoTPage()
        {
            InitializeComponent();
        }

        public void ShowErrorMessage(string message)
        {
            this.DisplaySnackBarAsync(message, "", null, TimeSpan.FromSeconds(3));
        }

        public void NavigateToAddIotPage(IoTDeviceModel model = null)
        {
            var addIotPopup = new AddIotDevicePopup();
            var addIoTPopupViewModel = new AddIoTPopupViewModel(addIotPopup, model);
            addIotPopup.BindingContext = addIoTPopupViewModel;
            var viewModel = (IoTPageViewModel)BindingContext;
            addIoTPopupViewModel.UpdatedIoTDevice += (device) => {
                viewModel.DeviceUpdated(device);
            };
            Navigation.ShowPopup(addIotPopup);
        }

        public async Task<bool> AskUser(string question)
        {
            return await DisplayAlert("Question", question, "Yes", "No", FlowDirection.LeftToRight);
        }

        public Task NavigateToIotDevicePage(IoTDeviceModel model)
        {
            var devicePage = new IoTDevicePage();
            var ioTDeviceViewModel = new IoTDeviceViewModel(model, devicePage);
            devicePage.BindingContext = ioTDeviceViewModel;
            return Navigation.PushAsync(devicePage);
        }
    }
}
