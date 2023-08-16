using MyHomeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveAudioDeviceSettingsView : ContentView
    {
        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource MuteIcon
        {
            get { return (ImageSource)GetValue(MuteIconProperty); }
            set { SetValue(MuteIconProperty, value); }
        }

        public static readonly BindableProperty MuteIconProperty =
            BindableProperty.Create("MuteIcon", typeof(ImageSource), typeof(ActiveAudioDeviceSettingsView), null);

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource UnMuteIcon
        {
            get { return (ImageSource)GetValue(UnMuteIconProperty); }
            set { SetValue(UnMuteIconProperty, value); }
        }

        public static readonly BindableProperty UnMuteIconProperty =
            BindableProperty.Create("UnMuteIcon", typeof(ImageSource), typeof(ActiveAudioDeviceSettingsView), null);

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelText.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create("LabelText", typeof(string), typeof(ActiveAudioDeviceSettingsView), null);



        public ActiveAudioDeviceSettingsView()
        {
            InitializeComponent();
        }
    }
}