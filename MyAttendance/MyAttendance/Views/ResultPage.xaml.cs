using MyAttendance.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyAttendance.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage : ContentPage
    {
        public ItemsViewModel _viewModel;

        public ResultPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ItemsViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}