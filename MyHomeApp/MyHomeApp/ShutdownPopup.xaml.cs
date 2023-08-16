using MyHomeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShutdownPopup : Popup<ShutdownPopupResult>, IDismissable<ShutdownPopupResult>
    {
        public ShutdownPopup()
        {
            InitializeComponent();
        }
    }
}
