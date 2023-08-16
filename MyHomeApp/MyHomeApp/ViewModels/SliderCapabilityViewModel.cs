using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyHomeApp.ViewModels
{
    internal class SliderCapabilityViewModel : CapabilityViewModelBase
    {
        private IIoTHomeApi ioTHomeApi;
        private IErrorHandling errorHandling;
        private IoTDeviceSliderCapability model;
        private bool isEnabled = true;
        private Task<IoTDeviceSliderCapability> ongoingUpdate;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public SliderCapabilityViewModel(IIoTHomeApi ioTHomeApi, IErrorHandling errorHandling, IoTDeviceSliderCapability model) : base(model)
        {
            this.ioTHomeApi = ioTHomeApi;
            this.errorHandling = errorHandling;
            this.model = model;
            ongoingUpdate = Task.FromResult(model);

            DragCompletedCommand = new RelayCommand(DragCompleted, (obj) => IsEnabled);
        }

        public RelayCommand DragCompletedCommand { get; set; }

        public float Current { get => Map(model.Current, model.Min, model.Max, 0, 1); set 
            {
                if (Math.Abs(Current - value) < 0.0001)
                    return;

               
                model.Current = Map(value, 0, 1, model.Min, model.Max);
                OnPropertyChanged();
                ValueChanged();
            }
        }

        public override bool IsEnabled { get => isEnabled; set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                DragCompletedCommand.RefreshCanExecute();
                OnPropertyChanged();
            } 
        }

        private async void DragCompleted(object obj)
        {
            model = await ValueChanged();
            OnPropertyChanged(nameof(Current));
        }

        private Task<IoTDeviceSliderCapability> ValueChanged()
        {
            try
            {
                tokenSource.Cancel();
                tokenSource = new CancellationTokenSource();

                ongoingUpdate = ongoingUpdate.ContinueWith(
                    async t => await ioTHomeApi.PostModel(model), tokenSource.Token, TaskContinuationOptions.LazyCancellation, TaskScheduler.Current).Unwrap();

                return ongoingUpdate;
            }
            catch (Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
            }
            return Task.FromResult(model);
        }

        private float Map(float value, float from, float to, float newFrom, float newTo)
        {
            return (value - from) / (to - from) * (newTo - newFrom) + newFrom;
        }
    }
}
