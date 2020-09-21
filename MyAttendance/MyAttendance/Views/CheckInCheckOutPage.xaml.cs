
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyAttendance.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckInCheckOutPage : ContentPage
    {
        public CheckInCheckOutPage()
        {
            InitializeComponent();
            this.Title = "CheckIn - CheckOut";
        }
    }
}