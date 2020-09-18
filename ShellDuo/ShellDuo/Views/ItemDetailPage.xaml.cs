using System.ComponentModel;
using Xamarin.Forms;
using ShellDuo.ViewModels;
using Xamarin.Forms.DualScreen;

namespace ShellDuo.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ItemsViewModel();
            _viewModel.Navigation = Navigation;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing(isDetail: true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.OnDisappearing();
        }
    }
}