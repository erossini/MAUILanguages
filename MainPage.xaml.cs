using CommunityToolkit.Maui.Converters;
using MauiLanguages.Helpers;
using MauiLanguages.Models;
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

            this.pickerFilter.SelectedIndex = 0;
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            vm.FilterItems(e.NewTextValue);
        }

        private void pickerFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            FilterModel? filterModel = picker.SelectedItem as FilterModel;

            vm.IsLoading = true;

            if (selectedIndex > 0 && filterModel != null)
            {
                vm.ShowGroups = false;

                this.listLanguages.IsGroupingEnabled = vm.ShowGroups;

                if (filterModel.Value == "Supported")
                {
                    this.listLanguages.SetBinding(ListView.ItemsSourceProperty, nameof(vm.SupportedCultures));
                    vm.IsEmpty = !vm.SupportedCultures.Any();
                }
                else if (filterModel.Value == "Recent")
                {
                    this.listLanguages.SetBinding(ListView.ItemsSourceProperty, nameof(vm.RecentCultures));
                    vm.IsEmpty = !vm.RecentCultures.Any();
                }
            }
            else
            {
                vm.ShowGroups = true;

                this.listLanguages.IsGroupingEnabled = vm.ShowGroups;
                this.listLanguages.SetBinding(ListView.ItemsSourceProperty, nameof(vm.FilteredCulturesGroups));
                vm.IsEmpty = !vm.FilteredCulturesGroups.Any();
            }

            vm.IsLoading = false;
        }

        private void listLanguages_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            LanguageModel? languageModel = null;
            if(e.SelectedItem is LanguageModel)
                languageModel = (LanguageModel)e.SelectedItem;
        }
    }
}