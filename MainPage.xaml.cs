using MauiLanguages.Helpers;
using MauiLanguages.ViewModels;
using System.Globalization;

namespace MauiLanguages
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel vm;

        public MainPage()
        {
            InitializeComponent();

            vm = new MainPageViewModel();
            BindingContext = vm;
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            vm.FilterItems(e.NewTextValue);
        }
    }
}
