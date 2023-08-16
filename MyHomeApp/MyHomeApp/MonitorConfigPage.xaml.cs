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
    public partial class MonitorConfigPage : ContentPage, IErrorHandling
    {
        public MonitorConfigPage()
        {
            InitializeComponent();
        }

        public void ShowErrorMessage(string message)
        {
            this.DisplaySnackBarAsync(message, "", null, TimeSpan.FromSeconds(3));
        }
    }
}