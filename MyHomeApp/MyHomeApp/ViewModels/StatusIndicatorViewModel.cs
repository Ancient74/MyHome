using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHomeApp.ViewModels
{
    internal class StatusIndicatorViewModel : ViewModelBase
    {
        private IMyHomeApi myHomeApi;
        private Timer timer;
        private Color indicator;
        private ApplicationStatus applicationStatus;
        private IErrorHandling errorHandling;
        private Dictionary<ApplicationStatus, Color> indicators;

        public StatusIndicatorViewModel(IMyHomeApi myHomeApi, IErrorHandling errorHandling)
        {
            this.errorHandling = errorHandling;
            indicators = new Dictionary<ApplicationStatus, Color>();
            indicators.Add(ApplicationStatus.Unreachable, ColorConverters.FromHex("#EA2027"));
            indicators.Add(ApplicationStatus.Normal, ColorConverters.FromHex("#2ecc71"));
            indicators.Add(ApplicationStatus.IpIsNotSet, ColorConverters.FromHex("#bdc3c7"));
            indicators.Add(ApplicationStatus.Initializing, Color.Transparent);

            ApplicationStatus = ApplicationStatus.Initializing;
            Indicator = indicators[ApplicationStatus];

            this.myHomeApi = myHomeApi;
            timer = new Timer();
            timer.Interval = 10000;
            timer.Elapsed += CheckStatus;
            timer.Start();
            CheckStatus(null, null);
        }

        public Color Indicator { get => indicator; set
            {
                if (indicator == value) return;
                indicator = value;
                OnPropertyChanged();
            }
        }

        public ApplicationStatus ApplicationStatus
        {
            get => applicationStatus; set
            {
                applicationStatus = value;
                OnPropertyChanged();
            }
        }

        public async Task Refresh(bool manualRefresh = true)
        {
            timer.Stop();

            try
            {
                ApplicationStatus = await myHomeApi.Ping();
            } catch (OperationCanceledException)
            {
                if (manualRefresh)
                    errorHandling.ShowErrorMessage("Request timeout");
                ApplicationStatus = ApplicationStatus.Unreachable;
            }
            catch (Exception e)
            {
                if (manualRefresh)
                    errorHandling.ShowErrorMessage(e.Message);
                ApplicationStatus = ApplicationStatus.Unreachable;
            }
            Indicator = indicators[ApplicationStatus];

            timer.Start();
        }

        private async void CheckStatus(object sender, EventArgs e)
        {
            await Refresh(false);
        }
    }
}
