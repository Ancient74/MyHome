using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace MyHomeApp.ViewModels
{
    internal class MonitorConfigButtonViewModel : ViewModelBase
    {
        private MonitorMode monitorMode;
        private IMonitorModeSelector monitorModeSelector;
        private const string TOGGLE_BIG_PICTURE_TEMPLATE = "BIG_PICTURE_";
        private bool isEnabled = true;

        public interface IMonitorModeSelector
        {
            Task SelectMonitorMode(MonitorMode monitorMode, bool openBigPicture);
        }

        public MonitorConfigButtonViewModel(MonitorMode monitorMode, IMonitorModeSelector monitorModeSelector)
        {
            this.monitorMode = monitorMode;
            this.monitorModeSelector = monitorModeSelector;

            SelectMonitorModeCommand = new RelayCommand(SelectMonitorMode, CanExecute);
            ToggleOpenBigPictureCommand = new RelayCommand(ToggleOpenBigPicture, CanExecute);
        }

        public bool IsEnabled { get => isEnabled; set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                SelectMonitorModeCommand.RefreshCanExecute();
                ToggleOpenBigPictureCommand.RefreshCanExecute();
            }
        }

        private bool CanExecute(object param) => IsEnabled;

        public RelayCommand SelectMonitorModeCommand { get; }
        public RelayCommand ToggleOpenBigPictureCommand { get; }

        public bool OpenBigPicture { get
            {
                var key = TOGGLE_BIG_PICTURE_TEMPLATE + monitorMode.ToString();
                return Preferences.Get(key, false);
            }
            set 
            { 
                var key = TOGGLE_BIG_PICTURE_TEMPLATE + monitorMode.ToString();
                Preferences.Set(key, value);
                OnPropertyChanged();
            }
        }

        private async void SelectMonitorMode(object obj)
        {
            await monitorModeSelector.SelectMonitorMode(monitorMode, OpenBigPicture);
        }

        private void ToggleOpenBigPicture(object obj)
        {
            OpenBigPicture = !OpenBigPicture;
        }
    }
}
