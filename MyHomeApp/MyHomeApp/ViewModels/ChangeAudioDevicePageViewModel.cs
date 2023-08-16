using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHomeApp.ViewModels
{
    internal class ChangeAudioDevicePageViewModel : ViewModelBase
    {
        private IMyHomeApi myHomeApi;
        private AudioDeviceType type;
        private List<AudioDeviceSelectView> devices;
        private IGoBackToActiveAudioDevicePage goBackToActiveDevicePage;
        private IReturnToMainPageNavigator returnToMainPageNavigator;
        private IErrorHandling errorHandling;
        private bool isRefreshing;

        public interface IGoBackToActiveAudioDevicePage
        {
            Task GoBackToActiveAudioDevicePage();
        }

        public ChangeAudioDevicePageViewModel(IMyHomeApi myHomeApi, AudioDeviceType type, IGoBackToActiveAudioDevicePage goBackToActiveDevicePage, IErrorHandling errorHandling, IReturnToMainPageNavigator returnToMainPageNavigator)
        {
            this.myHomeApi = myHomeApi;
            this.type = type;
            this.goBackToActiveDevicePage = goBackToActiveDevicePage;
            this.returnToMainPageNavigator = returnToMainPageNavigator;
            this.errorHandling = errorHandling;

            RefreshCommand = new RelayCommand(Refresh);
            SelectCommand = new RelayCommand(Select, x => Devices != null);
            ChangeDeviceCommand = new RelayCommand(ChangeDevice, CanChangeDevice);

            Refresh(null);
        }

        private async void ChangeDevice(object obj)
        {
            try
            {
                var view = GetSelectedView();
                if (view == null)
                    return;
                await myHomeApi.ActivateAudioDevice(type, view.Id);
                await goBackToActiveDevicePage.GoBackToActiveAudioDevicePage();
            } catch (OperationCanceledException)
            {
                await returnToMainPageNavigator.BackToMainPage("Request timeout");
            } catch(Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
            }
        }

        public ICommand RefreshCommand { get; }
        public RelayCommand SelectCommand { get; }
        public RelayCommand ChangeDeviceCommand { get; }


        public string LabelText { get => type.ToString() + " Device"; }

        public bool IsRefreshing { get => isRefreshing; set {
                if (isRefreshing == value)
                    return;
                isRefreshing = value;
                OnPropertyChanged();
            } }

        public List<AudioDeviceSelectView> Devices { get => devices; set {
                if (devices == value)
                    return;
                devices = value;
                SelectCommand.RefreshCanExecute();
                OnPropertyChanged();
            } }

        private void Select(object selectViewObj)
        {
            var args = selectViewObj as ItemTappedEventArgs;
            var selectView = args.Item as AudioDeviceSelectView;
            foreach (var item in Devices)
            {
                if (item.SelectState == SelectState.Active)
                    continue;
                if (item.SelectState == SelectState.Selected && item != selectView)
                    item.SelectState = SelectState.Normal;
                else if (item == selectView)
                    item.SelectState = SelectState.Selected;
            }
            ChangeDeviceCommand.RefreshCanExecute();
        }

        private bool CanChangeDevice(object param)
        {
            return GetSelectedView() != null;
        }

        private AudioDeviceSelectView GetSelectedView()
        {
            return Devices?.FirstOrDefault(x => x.SelectState == SelectState.Selected);
        }

        private async void Refresh(object param)
        {
            IsRefreshing = true;
            var activeDevice = await myHomeApi.GetActiveAudioDevice(type);
            IEnumerable<AudioDeviceModel> allDevices = (await myHomeApi.GetAllAudioDevices(type)).OrderBy(x => x.Name);
            var devices = new List<AudioDeviceSelectView>();
            var selected = Devices?.FirstOrDefault(x => x.SelectState == SelectState.Selected);
            foreach (var item in allDevices)
            {
                bool isActive = activeDevice.Id == item.Id;
                SelectState selectState = isActive ? SelectState.Active : SelectState.Normal;
                if (selectState == SelectState.Normal && selected != null && selected.Id == item.Id)
                    selectState = SelectState.Selected;
                devices.Add(new AudioDeviceSelectView(item.Name, item.Id, selectState));
            }
            Devices = devices;
            IsRefreshing = false;
        }
    }
}
