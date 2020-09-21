using System.ComponentModel;
using Xamarin.Forms;
using MyAttendance.ViewModels;

namespace MyAttendance.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}