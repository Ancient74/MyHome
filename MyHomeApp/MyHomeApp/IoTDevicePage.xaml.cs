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
    public partial class IoTDevicePage : ContentPage, IErrorHandling
    {
        public IoTDevicePage()
        {
            InitializeComponent();
        }

        public void ShowErrorMessage(string message)
        {
            this.DisplaySnackBarAsync(message, "", null, TimeSpan.FromSeconds(3));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            (BindingContext as IDisposable)?.Dispose();
        }
    }
}