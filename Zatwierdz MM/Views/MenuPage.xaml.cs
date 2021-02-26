using Zatwierdz_MM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zatwierdz_MM.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();
            BindingContext = this;

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Skanuj, Title="Skanuj" },
                new HomeMenuItem {Id = MenuItemType.Lista, Title="Lista zeskanowanych" },
                new HomeMenuItem {Id = MenuItemType.Ustawienia, Title="Ustawienia" },
                //new HomeMenuItem {Id = MenuItemType.PrzyjmijMM, Title="Przyjmij MM" },
                new HomeMenuItem {Id = MenuItemType.About, Title="Info" }
            };

            var version = DependencyService.Get<Models.IAppVersionProvider>();
            wersja.Text = $"wersja :{version.AppVersion}";


            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;

                if (IsBusy)
                    return;

                IsBusy = true;
                await RootPage.NavigateFromMenu(id);
                IsBusy = false;
            };
        }
    }
}