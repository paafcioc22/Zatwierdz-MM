using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zatwierdz_MM.Models;
using Zatwierdz_MM.Services;
using Zatwierdz_MM.ViewModels;

namespace Zatwierdz_MM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrzyjmijMMSkanowanie : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        PrzyjmijMMSkanowanieViewModel viewModel;
        public PrzyjmijMMSkanowanie(PrzyjmijMMSkanowanieViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
            entry_MM.Completed += Entry_MM_Completed; ;
        }

        private async void Entry_MM_Completed(object sender, EventArgs e)
        {
            try
            {
                entry_MM.Unfocus();
                await Task.Delay(1000);
                entry_MM.Focus();
            }
            catch (Exception s)
            {

                await DisplayAlert("Bład", s.Message, "OK");
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            List<string> opcje = new List<string> {"Odłóż na miejsce odkładcze", "Edytuj", "Pokaż wszystkie wpisy dla modelu"};

            var towar = e.Item as PC_MsInwentory;

            var odp =await  DisplayActionSheet($"Wybierz :", null, "Anuluj", opcje.ToArray());
            if (odp == "Odłóż na miejsce odkładcze")
            {
               var isExists= await  IsPlaceExists(towar.MsI_TwrNumer); // czy w ogóle kod dodany


                if (isExists.Count == 0)
                {
                    var odp2=await DisplayActionSheet($"Nie przypisano do miejsca :", "Utwórz nowe", "Anuluj", "");

                    if(odp2== "Utwórz nowe")
                    {
                        //isExists = await IsPlaceExists( towar.MsI_TwrNumer, towar.MsI_TrnNumer); // czy istniej wpis z tej mmki 

                        string placeName = await DisplayPromptAsync("Tworzenie nowego wpisu","Podaj pozycję np. A10..", "OK", "Anuluj", "", 3, keyboard: Keyboard.Create(KeyboardFlags.CapitalizeCharacter), "");


                        if(  await viewModel.IsPlaceEmpty(towar.MsI_TwrNumer,0,placeName))
                        {
                            if (await AddTowarToPlace(towar, placeName))
                            {
                                await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {placeName}", "OK");
                                foreach (var item in viewModel.Items)
                                {
                                    if (item.MsI_TwrNumer == towar.MsI_TwrNumer)
                                    {
                                        towar.Msi_IsPut = true;

                                    }
                                }
                            }
                            else
                                await DisplayAlert("info", "Pozycja z tej MM została już dodana", "OK");
                        }
                        else
                        {
                            await DisplayAlert("info", "To miejsce jest już zajęte", "OK");
                        }
                        
                    }else if (odp2 == "Anuluj")
                    {
                        ((ListView)sender).SelectedItem = null;
                        return;
                    }
                }
                else
                {
                    var places = isExists.Select(s => s.PlaceName).Distinct().ToArray();
                    //var quantSum = from aa in isExists
                    //               group aa by aa.PlaceTwrNumer into g
                    //               select new Place
                    //               {
                    //                   PlaceQuantity = g.Sum(s => s.PlaceQuantity)
                    //               };



                    odp = await DisplayActionSheet($"Dodaj do istniejącego :", "Dodaj nowe położenie", "Anuluj", places);
                    if(odp!= "Dodaj nowe położenie" && odp !="Anuluj")
                    {
                        if(await AddTowarToPlace(towar, odp))
                        {
                            await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {odp}", "OK");
                            foreach (var item in viewModel.Items)
                            {
                                if (item.MsI_TwrNumer == towar.MsI_TwrNumer)
                                {
                                    towar.Msi_IsPut = true;

                                }
                            }
                        }
                              
                        else
                             
                        if (await DisplayAlert("info", $"{towar.Twr_Kod} z tej MM został już dodany\n Chcesz zaktualizować wpis?", "Tak", "Nie"))
                        {
                            if(await viewModel.UpdateModelInPlace(towar, odp))
                                await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {odp}", "OK");
                        }
                    }
                    else if (odp == "Anuluj")
                    {
                        ((ListView)sender).SelectedItem = null;
                        return;
                    }
                    else
                    {
                        string placeName = await DisplayPromptAsync("Tworzenie nowego wpisu", "Podaj pozycję np. A10..", 
                            "OK", "Anuluj", "", 3, keyboard: Keyboard.Create(KeyboardFlags.CapitalizeCharacter), "");

                        //if(await AddTowarToPlace(towar, placeName))
                        //    await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {placeName}", "OK");
                        //else
                        //    await DisplayAlert("info", "Pozycja z tej MM została już dodana", "OK");


                        if (await viewModel.IsPlaceEmpty(towar.MsI_TwrNumer, 0, placeName))
                        {
                            if (await AddTowarToPlace(towar, placeName))
                            {
                                await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {placeName}", "OK");
                                foreach (var item in viewModel.Items)
                                {
                                    if (item.MsI_TwrNumer == towar.MsI_TwrNumer)
                                    {
                                        towar.Msi_IsPut = true;

                                    }
                                }
                            }
                            else
                                await DisplayAlert("info", "Pozycja z tej MM została już dodana", "OK");
                        }
                        else
                        {
                            await DisplayAlert("info", "To miejsce jest już zajęte", "OK");
                        }

                    }

                }
            }else if (odp == "Anuluj")
            {
                ((ListView)sender).SelectedItem = null;
                return;
            }else if (odp== "Pokaż wszystkie wpisy dla modelu")
            {

                var sql = $@"cdn.PC_WykonajSelect N'select PlaceName, cdn.NazwaObiektu(1603, placetrnnumer,0,2) PlaceOpis, PlaceQuantity  
                            from  cdn.pc_mspolozenie
                            where placetwrnumer={towar.MsI_TwrNumer}'";

                var listaplace=await App.TodoManager.PobierzDaneZWeb<Place>(sql);

                string wpisy="";
                List<string> wpisies = new List<string>();

                foreach (var i in listaplace)
                {
                    wpisies.Add($@"{i.PlaceName} - {i.PlaceOpis} :  {i.PlaceQuantity} szt");
                }

                ;
                await DisplayActionSheet($"Lista wpisów dla \n{towar.Twr_Kod} : suma {listaplace.Sum(s=>s.PlaceQuantity)} szt", "", "Anuluj", wpisies.ToArray());

            }
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async Task<bool> AddTowarToPlace(PC_MsInwentory pC_MsInwentory, string placeName)
        {
            return  await viewModel.AddTowarToPlace(pC_MsInwentory, placeName);
        }

        private async Task<List<Place>> IsPlaceExists( int twr_Gidnumer, int trn_Gidnumer=0)
        {
            var odp= await viewModel.IsPlaceExists(twr_Gidnumer, trn_Gidnumer) ;
            return odp.ToList();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.LoadItemsCommand.Execute(null);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
             await Navigation.PushModalAsync(new RaportLista_AddTwrKod(viewModel.Trn_Gidnumer));
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrzyjmijMMRaportRoznic(new PrzyjmijMMRaportRcaViewModel(viewModel)));
        }
    }
}
