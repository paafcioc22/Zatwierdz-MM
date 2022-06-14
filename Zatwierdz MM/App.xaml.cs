using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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
        private const string password = "password";
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
        }

        public string Password
        {
            get
            {
                if (Properties.ContainsKey(password))
                    return Properties[password].ToString();
                return "";
            }
            set
            {
                Properties[password] = value;
            }

        }


        protected override void OnStart()
        {
            AppCenter.Start("android=9f53c6c9-16f0-4e8f-ba7f-360de4964c97;" +
                "uwp={Your UWP App secret here};" +
                "ios={Your iOS App secret here};" +
                "macos={Your macOS App secret here};",
                typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
