using System;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHomeApp
{
    public partial class App : Application
    {
        public App(string url = "")
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage(url));
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
