using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zatwierdz_MM.Models;
using Zatwierdz_MM.ViewModels;

namespace Zatwierdz_MM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlacesPage : ContentPage
    {
        private PlacesViewModel zatwierdzonevm;
        private PrzyjmijMMSkanowanieViewModel viewModel;

        public PlacesPage()
        {
            InitializeComponent();

            BindingContext = zatwierdzonevm = new PlacesViewModel();
            viewModel = new PrzyjmijMMSkanowanieViewModel();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            List<string> opcje = new List<string> { "Zmień położenie (nazwę)", "Pokaż wszystkie wpisy dla modelu" };

            var place = e.Item as Place;

            var odp = await DisplayActionSheet($"Wybierz :", null, "Anuluj", opcje.ToArray());
            string odp2 = "";

            if(odp== "Zmień położenie (nazwę)")
            {
                string placeName = await DisplayPromptAsync("Tworzenie nowego wpisu", "Podaj nową pozycję", "OK", "Anuluj", "", 3, 
                    keyboard: Keyboard.Create(KeyboardFlags.CapitalizeCharacter), "");

                if (!string.IsNullOrEmpty(placeName))
                {
                    if (!await viewModel.IsPlaceEmpty(place.PlaceTwrNumer, 0, placeName))
                        odp2 = await DisplayActionSheet($"Miejsce nie jest puste, odłożyć mimo to? :", "NIE", "TAK", "");

                    if (odp2 == "TAK" || string.IsNullOrEmpty(odp2))
                    {
                        if (!string.IsNullOrEmpty(placeName))
                        {
                            if (await zatwierdzonevm.UpdatePlaceName(place, placeName))
                            {
                                await DisplayAlert("info", $"Zmieniono położenie na {placeName}", "OK");
                                //todo : pobierz nowa liste bo następuje agregacja
                                place.PlaceName = placeName;
                            }

                        }
                        else
                        {
                            await DisplayAlert("info", "Podaj lokalizacje", "OK");
                        }
                    } 
                }
                
                   

            }else if(odp== "Pokaż wszystkie wpisy dla modelu")
            {
                await DisplayAlert("info", "JEszcze nie dziala", "OK");
            }


            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //if (zatwierdzonevm.Items.Count == 0)
            zatwierdzonevm.LoadItemsCommand.Execute(null);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RaportLista_AddTwrKod(0));

        }
    }
}
