using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MyHomeApp.ViewModels
{
    internal class ShutdownPopupViewModel : ViewModelBase
    {
        private IDismissable<ShutdownPopupResult> dismissable;
        private bool force;

        public ShutdownPopupViewModel(IDismissable<ShutdownPopupResult> dismissable)
        {
            this.dismissable = dismissable;
            ShutdownCommand = new RelayCommand(Shutdown);
            RestartCommand = new RelayCommand(Restart);
            CancelCommand = new RelayCommand(Cancel);
            ToggleForceCommand = new RelayCommand(ToggleForce);
        }

        public bool Force { get => force; set {
                if (force == value) return;
                force = value;
                OnPropertyChanged();
            } }

        public ICommand ShutdownCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ToggleForceCommand { get; }

        public void Shutdown(object obj)
        {
            dismissable.Dismiss(Force ? ShutdownPopupResult.ForceShutdown : ShutdownPopupResult.NormalShutdown);
        }

        public void Restart(object obj)
        {
            dismissable.Dismiss(Force ? ShutdownPopupResult.ForceRestart : ShutdownPopupResult.NormalRestart);
        }
        public void Cancel(object obj)
        {
            dismissable.Dismiss(ShutdownPopupResult.Cancel);
        }

        public void ToggleForce(object obj)
        {
            Force = !Force;
        }
    }
}
