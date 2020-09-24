using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyAttendance.ViewModels;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyAttendance.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            checkConnection();
            this.BindingContext = new LoginViewModel();
        }

        private bool checkConnection()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                layNoConnection.IsVisible = false;
                //StopElapsedTimeUpdate();
            }
            else
            {
                // your logic...  
                layNoConnection.IsVisible = true;

                if (!isTimeUpdateStarted)
                    StartElapsedTimeUpdate();

            }

            return false;
        }

        private CancellationTokenSource cts;
        private bool isTimeUpdateStarted = false;
        public void StartElapsedTimeUpdate()
        {
            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();
            var ignore = TimeUpdaterAsync(cts.Token);
        }

        public void StopElapsedTimeUpdate()
        {
            if (cts != null) cts.Cancel();
            cts = null;
        }

        public async Task TimeUpdaterAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    isTimeUpdateStarted = true;
                    checkConnection();
                    await Task.Delay(500, ct);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}