using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyHomeApp.ViewModels
{
    internal class MonitorConfigPageViewModel : ViewModelBase, MonitorConfigButtonViewModel.IMonitorModeSelector
    {
        private IMyHomeApi myHomeApi;
        private IErrorHandling errorHandling;
        private bool isLoading;
        private Dictionary<MonitorMode, MonitorConfigButtonViewModel> monitorButtons;
        private IReturnToMainPageNavigator returnToMainPageNavigator;

        public MonitorConfigPageViewModel(IMyHomeApi myHomeApi, IErrorHandling errorHandling, IReturnToMainPageNavigator returnToMainPageNavigator)
        {
            this.myHomeApi = myHomeApi;
            this.errorHandling = errorHandling;
            this.returnToMainPageNavigator = returnToMainPageNavigator;
            monitorButtons = new Dictionary<MonitorMode, MonitorConfigButtonViewModel>();
            foreach (MonitorMode mode in Enum.GetValues(typeof(MonitorMode)))
            {
                monitorButtons.Add(mode, new MonitorConfigButtonViewModel(mode, this));
            }
        }

        public bool IsLoading
        {
            get => isLoading; private set
            {
                if (isLoading == value) return;
                isLoading = value;
                foreach (var button in monitorButtons)
                {
                    button.Value.IsEnabled = !value;
                }
                OnPropertyChanged();
            }
        }

        public MonitorConfigButtonViewModel PCScreenOnlyViewModel { get => monitorButtons[MonitorMode.PCScreenOnly]; }
        public MonitorConfigButtonViewModel SecondScreenOnlyViewModel { get => monitorButtons[MonitorMode.SecondScreenOnly]; }
        public MonitorConfigButtonViewModel ExtendViewModel { get => monitorButtons[MonitorMode.Extend]; }
        public MonitorConfigButtonViewModel DuplicateViewModel { get => monitorButtons[MonitorMode.Duplicate]; }

        public async Task SelectMonitorMode(MonitorMode monitorMode, bool openBigPicture)
        {
            IsLoading = true;
            try
            {
                await myHomeApi.SetMonitorMode(monitorMode);
                if (openBigPicture)
                    await myHomeApi.OpenBigPicture();
            }
            catch (OperationCanceledException)
            {
                await returnToMainPageNavigator.BackToMainPage("Request timeout");
                return;
            }
            catch (Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
                IsLoading = false;
                return;
            }
            IsLoading = false;
            await returnToMainPageNavigator.BackToMainPage();
        }
    }
}
