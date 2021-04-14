using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Split;

            MenuPages.Add((int)MenuItemType.Skanuj, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Skanuj:
                        MenuPages.Add(id, new NavigationPage(new SkanujPage()));
                        break;
                    case (int)MenuItemType.Lista:
                        MenuPages.Add(id, new NavigationPage(new ListaZeskanowaychPage()));
                        break; 
                    case (int)MenuItemType.ListaNieSkanowanych:
                        MenuPages.Add(id, new NavigationPage(new ListaNieZeskanowaychPage()));
                        break;
                    case (int)MenuItemType.Mijesca:
                        MenuPages.Add(id, new NavigationPage(new PlacesPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.Ustawienia:
                        MenuPages.Add(id, new NavigationPage(new UstawieniaPage()));
                        break;
                    case (int)MenuItemType.FotoBrowser:
                        MenuPages.Add(id, new NavigationPage(new FotoBrowser()));
                        break;
                   
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}