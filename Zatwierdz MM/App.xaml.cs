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
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
