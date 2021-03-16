using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zatwierdz_MM.Models;
using Zatwierdz_MM.ViewModels;

namespace Zatwierdz_MM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrzyjmijMMPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        PrzyjmijMMViewModel viewModel;
       



        public PrzyjmijMMPage(PrzyjmijMMViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = this.viewModel = viewModel; 

        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var karta = e.Item as DaneMMElem; 

            await Launcher.OpenAsync(new Uri(karta.Url.Replace("Miniatury/", "")));

            await Clipboard.SetTextAsync(karta.Ean);
            DependencyService.Get<Services.IWebService> ().ShowLong("Skopiowano Ean");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();

            //if (zatwierdzonevm.Items.Count == 0)
            viewModel.LoadItemsCommand.Execute(null);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrzyjmijMMSkanowanie(new PrzyjmijMMSkanowanieViewModel(viewModel.Items.ToList())));
        }

        
    }
}
