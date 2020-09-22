
using MyAttendance.Firebase;
using MyAttendance.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyAttendance.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckInCheckOutPage : ContentPage
    {
        private FirebaseAttendanceRepository attendanceRepository;

        public AttendanceModel CurrenctCheckIn = null;
        public bool hasTodaysCheckIn = false;
        public bool hasCurrentCheckIn = false;
        public bool IsUpdatingIn = false;
        public CheckInCheckOutPage()
        {
            attendanceRepository = new FirebaseAttendanceRepository();
            InitializeComponent();
            this.Title = "CheckIn - CheckOut";

            GetTodaysCheckIn();
        }

        public async void GetAllAttendance()
        {
            var allAttendance = await attendanceRepository.GetAllAttendance();
        }

        public async void GetTodaysCheckIn()
        {
            var todaysAttendance = await attendanceRepository.GetAttendance(DateTime.Now.Date);

            if (todaysAttendance.Count() > 0)
            {
                hasTodaysCheckIn = true;

                CurrenctCheckIn = todaysAttendance.Where(x => x.CheckoutTime == null).LastOrDefault();

                if (CurrenctCheckIn != null) hasCurrentCheckIn = true;

                UpdateDesign();
            }

            UpdateGrid(todaysAttendance);
        }

        public async void CheckIn()
        {
            AttendanceModel attendance = new AttendanceModel()
            {
                Id = Guid.NewGuid(),
                CheckInTime = DateTime.Now,
                CheckoutTime = null,
                Date = DateTime.Now.Date,
                TotalHours = 0
            };

            await attendanceRepository.AddAttendance(attendance);


            CurrenctCheckIn = attendance;
            hasCurrentCheckIn = true;

            StartElapsedTimeUpdate();
            CurrenctCheckInTime.Text = "Checked In at " + CurrenctCheckIn.CheckInTime.Value.ToString("hh:mm tt");

            GetTodaysCheckIn();
            await DisplayAlert("Success", "Checked In successfully.", "OK");
        }

        public async void CheckOut()
        {
            string comment = "";

            DateTime checkedOutTime = DateTime.Now;

            TimeSpan workedHoursTimeSpan = checkedOutTime - CurrenctCheckIn.CheckInTime.Value;
            int hours = workedHoursTimeSpan.Hours;
            int minutes = workedHoursTimeSpan.Minutes;

            decimal workedHours = Convert.ToDecimal(hours.ToString() + "." + minutes.ToString());

            CurrenctCheckIn.CheckoutTime = checkedOutTime;
            CurrenctCheckIn.TotalHours = workedHours;
            CurrenctCheckIn.Comment = comment;

            await attendanceRepository.UpdateAttendance(CurrenctCheckIn);

            StopElapsedTimeUpdate();

            CurrenctCheckIn = null;
            hasTodaysCheckIn = true;
            hasCurrentCheckIn = false;

            CurrenctCheckInTime.Text = "Last checked Out at " + checkedOutTime.ToString("hh:mm tt");
            lblElapsedTime.Text = "00:00";

            GetTodaysCheckIn();
            await DisplayAlert("Success", "Checked Out successfully.", "OK");
        }

        public async void Delete()
        {
            var attendance = (await attendanceRepository.GetAllAttendance()).LastOrDefault();

            //attendance.Comment = "Checked Out";

            await attendanceRepository.DeleteAttendance(attendance.Id);

            await DisplayAlert("Success", "CheckIn details removed successfully.", "OK");
        }

        private void btnCheckIn_Clicked(object sender, EventArgs e)
        {
            CheckIn();
        }

        private void btnCheckOut_Clicked(object sender, EventArgs e)
        {
            CheckOut();
        }

        private void UpdateDesign()
        {
            if (hasTodaysCheckIn)
            {
                TodayCheckedInView.IsVisible = true;
            }

            btnCheckOut.IsVisible = hasCurrentCheckIn;
            btnCheckIn.IsVisible = !hasCurrentCheckIn;

            if (hasCurrentCheckIn)
            {
                StartElapsedTimeUpdate();
                CurrenctCheckInTime.Text = "Checked In at " + CurrenctCheckIn.CheckInTime.Value.ToString("hh:mm tt");
            }

            //GetTodaysCheckIn();
        }

        private void UpdateGrid(List<AttendanceModel> todaysAttendance)
        {
            var totalGridRows = grdTodayCheckIn.RowDefinitions.Count();

            if (totalGridRows > 0)
            {
                for (int i = totalGridRows - 1; i >= 0; i--)
                {
                    grdTodayCheckIn.RowDefinitions.RemoveAt(i);
                }
            }


            var lblHeadCheckedIn = new Label
            {
                Text = "Checked In",
                FontSize = 15,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            var lblHeadCheckedOut = new Label
            {
                Text = "Checked Out",
                FontSize = 15,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            var lblHeadHours = new Label
            {
                Text = "Worked Hours",
                FontSize = 15,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            grdTodayCheckIn.Children.Add(lblHeadCheckedIn, 0, 0);
            grdTodayCheckIn.Children.Add(lblHeadCheckedOut, 1, 0);
            grdTodayCheckIn.Children.Add(lblHeadHours, 2, 0);

            for (int i = 0; i < todaysAttendance.Count; i++)
            {
                grdTodayCheckIn.RowDefinitions.Add(new RowDefinition());

                var lblCheckedIn = new Label
                {
                    Text = todaysAttendance[i].CheckInTime.Value.ToString("hh:mm tt"),
                    FontSize = 15,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                var lblCheckedOut = new Label
                {
                    Text = todaysAttendance[i].CheckoutTime != null ? todaysAttendance[i].CheckoutTime.Value.ToString("hh:mm tt") : "",
                    FontSize = 15,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                var lblHours = new Label
                {
                    Text = todaysAttendance[i].TotalHours.ToString(),
                    FontSize = 15,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };


                grdTodayCheckIn.Children.Add(lblCheckedIn, 0, i + 1);
                grdTodayCheckIn.Children.Add(lblCheckedOut, 1, i + 1);
                grdTodayCheckIn.Children.Add(lblHours, 2, i + 1);
            }

            UpdateDesign();
        }

        private CancellationTokenSource cts;
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
                    TimeSpan workedHoursTimeSpan = DateTime.Now - CurrenctCheckIn.CheckInTime.Value;

                    lblElapsedTime.Text = workedHoursTimeSpan.ToString(@"hh\:mm\:ss");
                    await Task.Delay(100, ct);
                }catch(Exception ex)
                {

                }
            }
        }
    }
}