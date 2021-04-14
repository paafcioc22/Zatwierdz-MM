using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zatwierdz_MM.ViewModels;

namespace Zatwierdz_MM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrzyjmijMMRaportRoznic : ContentPage
    {
        private PrzyjmijMMRaportRcaViewModel viewModel;

        public ObservableCollection<string> Items { get; set; }

        public PrzyjmijMMRaportRoznic(ViewModels.PrzyjmijMMRaportRcaViewModel przyjmijMMRaportRcaViewModel)
        {
            InitializeComponent();
            BindingContext = this.viewModel = przyjmijMMRaportRcaViewModel;

            MyListView.IsVisible = viewModel.Items.Any();
            notFound.IsVisible = !MyListView.IsVisible;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //if (zatwierdzonevm.Items.Count == 0)
            viewModel.LoadRaport.Execute(null);
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

           // await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async void btn_finishRaport_Clicked(object sender, EventArgs e)
        {
            string opisRaport = await DisplayPromptAsync("Zapisywanie raportu", "Dodaj ewentualny opis", "OK", "Anuluj", "", 50,
                    keyboard: Keyboard.Create(KeyboardFlags.CapitalizeCharacter), "");

            if (opisRaport!=null)
            {
                if (await viewModel.SaveRaportToBase(opisRaport))
                {
                    await DisplayAlert("Info", "Raport zapisany pomyślnie", "OK");
                }
                else
                {
                    await DisplayAlert("Info", "Raport już istnieje", "OK");
                }
            }
            
        }
    }
}
