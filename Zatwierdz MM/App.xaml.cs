using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zatwierdz_MM.Services;
using Zatwierdz_MM.Views;

namespace Zatwierdz_MM
{
    public partial class App : Application
    {
        public static WebMenager TodoManager { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
