using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        private bool isRaportExists;

        public PrzyjmijMMSkanowanie(PrzyjmijMMSkanowanieViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
           
            entry_MM.Completed += Entry_MM_Completed; ;
            //
        }

        private async void Entry_MM_Completed(object sender, EventArgs e)
        {
            try
            {
                entry_MM.Unfocus();
                //var objj = viewModel.Items.Where(s => s.Ean == viewModel.EanSkan).FirstOrDefault();
                //MyListView.ScrollTo(objj, ScrollToPosition.Center, true);
                await Task.Delay(1500);
                //if(viewModel.twrkarta!=null)
                //viewModel.SelectItem = viewModel.Items.Where(s => s.Ean == viewModel.EanSkan).FirstOrDefault();
                
                //Task.Delay(2000).Wait();
                //MyListView.SelectedItem = ;
                //viewModel.SelectItem = 2;
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
            string odp3 = "";

            List<string> opcje = null;
            string deleteButton = null;
            //todo: dodaj w menu zdjćeice
            if (isRaportExists)
            {
                opcje = new List<string> { "Pokaż wszystkie wpisy dla modelu" };
            }
            else
            {
                opcje = new List<string> { "Odłóż na miejsce odkładcze", "Edytuj", "Pokaż wszystkie wpisy dla modelu","Pokaż zdjęcie" };
                deleteButton = "Usuń";
            }

            var towar = e.Item as PC_MsInwentory;

            var odp = await DisplayActionSheet($"Wybierz :", "Anuluj", deleteButton, opcje.ToArray());
            if (odp == "Odłóż na miejsce odkładcze")
            {
                var isExists = await IsPlaceExists(towar.MsI_TwrNumer); // czy w ogóle kod dodany

            
                if (isExists.Count == 0)
                {
                    var odp2 = await DisplayActionSheet($"Nie przypisano do miejsca :", "Utwórz nowe", "Anuluj", "");

                    if (odp2 == "Utwórz nowe")
                    {
                        //isExists = await IsPlaceExists( towar.MsI_TwrNumer, towar.MsI_TrnNumer); // czy istniej wpis z tej mmki 

                        string placeName = await DisplayPromptAsync("Tworzenie nowego wpisu", "Podaj pozycję np. A10..", "OK", "Anuluj", "", 3, keyboard: Keyboard.Create(KeyboardFlags.CapitalizeCharacter), "");


                        if (!await viewModel.IsPlaceEmpty(towar.MsI_TwrNumer, 0, placeName))
                            odp3 = await DisplayActionSheet($"Miejsce nie jest puste, odłożyć mimo to? :", "NIE", "TAK", "");


                        if (odp3 == "TAK" || string.IsNullOrEmpty(odp3))
                        {
                            if (!string.IsNullOrEmpty(placeName))
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
                                await DisplayAlert("info", "Podaj lokalizacje", "OK");
                            }

                        }
                        //else
                        //{
                        //    await DisplayAlert("info", "To miejsce jest już zajęte", "OK");
                        //}

                    }
                    else if (odp2 == "Anuluj")
                    {
                        ((ListView)sender).SelectedItem = null;
                        return;
                    }
                }
                else
                {
                    var places = isExists.Select(s => s.PlaceName).Distinct().ToArray();


                    var IsExistsFromThisMM = await IsPlaceExists(towar.MsI_TwrNumer, towar.MsI_TrnNumer);

                    if (IsExistsFromThisMM.Count > 0)
                    {
                        odp = await DisplayActionSheet($"Dodaj do istniejącego :", null, "Anuluj", places);//"Dodaj nowe położenie"

                    }
                    else
                    {
                        odp = await DisplayActionSheet($"Dodaj do istniejącego :", "Dodaj nowe położenie", "Anuluj", places);//"Dodaj nowe położenie"

                    }

                    //todo: czy dodawać nowe położenie??
                    //odp = await DisplayActionSheet($"Dodaj do istniejącego :",null,  "Anuluj", places);//""
                    if (odp != "Dodaj nowe położenie" && odp != "Anuluj")
                    {

                        if (!string.IsNullOrEmpty(odp))
                        {
                            if (await AddTowarToPlace(towar, odp))
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
                            else if (await DisplayAlert("info", $"{towar.Twr_Kod} z tej MM został już dodany\n Chcesz zaktualizować wpis?", "Tak", "Nie"))
                            {
                                if (await viewModel.UpdateModelInPlace(towar, odp))
                                    await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {odp}", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("info", "Nazwa miejsca nie może być pusta", "OK");
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


                        if (!await viewModel.IsPlaceEmpty(towar.MsI_TwrNumer, 0, placeName))
                            odp3 = await DisplayActionSheet($"Miejsce nie jest puste, odłożyć mimo to? :", "NIE", "TAK", "");


                        if (odp3 == "TAK" || string.IsNullOrEmpty(odp3))
                        {
                            if (!string.IsNullOrEmpty(placeName))
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
                                {
                                    await DisplayAlert("info", "Pozycja z tej MM została już dodana", "OK");
                                }
                            }
                            else
                            {
                                await DisplayAlert("info", "Podaj lokalizacje", "OK");
                            }

                        }

                        //if (await viewModel.IsPlaceEmpty(towar.MsI_TwrNumer, 0, placeName))
                        //{
                        //    if (!string.IsNullOrEmpty(placeName))
                        //    {
                        //        if (await AddTowarToPlace(towar, placeName))
                        //        {
                        //            await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {placeName}", "OK");
                        //            foreach (var item in viewModel.Items)
                        //            {
                        //                if (item.MsI_TwrNumer == towar.MsI_TwrNumer)
                        //                {
                        //                    towar.Msi_IsPut = true;

                        //                }
                        //            }
                        //        }
                        //        else
                        //            await DisplayAlert("info", "Pozycja z tej MM została już dodana", "OK");
                        //    }
                        //    else
                        //    {
                        //        await DisplayAlert("info", "Nazwa miejsca nie może być pusta", "OK");
                        //    }
                        //}
                        //else
                        //{
                        //    await DisplayAlert("info", "To miejsce jest już zajęte", "OK");
                        //}

                    }

                }
            }
            else if (odp == "Anuluj")
            {
                ((ListView)sender).SelectedItem = null;
                return;
            }
            else if (odp == "Pokaż wszystkie wpisy dla modelu")
            {

                var sql = $@"cdn.PC_WykonajSelect N'select PlaceName, cdn.NazwaObiektu(1603, placetrnnumer,0,2) PlaceOpis, PlaceQuantity  
                            from  cdn.pc_mspolozenie
                            where placetwrnumer={towar.MsI_TwrNumer}'";

                var listaplace = await App.TodoManager.PobierzDaneZWeb<Place>(sql);

                string wpisy = "";
                List<string> wpisies = new List<string>();

                foreach (var i in listaplace)
                {
                    wpisies.Add($@"{i.PlaceName} - {i.PlaceOpis} :  {i.PlaceQuantity} szt");
                }

               ;
                await DisplayActionSheet($"Lista wpisów dla \n{towar.Twr_Kod} : suma {listaplace.Sum(s => s.PlaceQuantity)} szt", "", "Anuluj", wpisies.ToArray());

            }
            else if (odp == "Edytuj")
            {
                short nowa;
                string nowailosc = await DisplayPromptAsync("Edycja wpisu", $"Podaj nową wartość dla {towar.Twr_Kod}", "OK", "Anuluj", "", 3, keyboard: Keyboard.Numeric, "");


                if (!string.IsNullOrEmpty(nowailosc))
                {
                    if (short.TryParse(nowailosc, out nowa))
                    {
                        towar.MsI_TwrIloscSkan = nowa;
                        await viewModel.UpdateModelInInventory(towar);
                        await DisplayAlert("uwaga!", "Ponownie odłóż tą wartość na miejsce odkładcze", "OK");
                    }
                    else
                    {
                        await DisplayAlert("uwaga", "To nie liczba", "OK");
                    }
                }



            }
            else if (odp == "Usuń")
            {
                var odpDelete=await DisplayAlert("uwaga", "Czy chcesz usunąć pozycję?\nWpis zostanie usunięty również z położenia", "TAK", "NIE");
                if(odpDelete)
                {
                    var response = await viewModel.DeleteFromSkanAndPolozenie(towar.MsI_TrnNumer, towar.Twr_Gidnumer);
                    if (response)
                    {
                        var obj = viewModel.Items.Where(s => s.MsI_TrnNumer == towar.MsI_TrnNumer && s.MsI_TwrNumer == towar.Twr_Gidnumer).FirstOrDefault();
                        viewModel.Items.Remove(obj);
                       await DisplayAlert("info", "Wpis usunięto", "OK");
                    }
                }
            }else if(odp== "Pokaż zdjęcie")
            {
                //var karta = e.Item as DaneMMElem;

                await Launcher.OpenAsync(new Uri(towar.Url.Replace("Miniatury/", "").Replace("small", "large")));
            }


                //Deselect Item
                ((ListView)sender).SelectedItem = null;
        }

        private async Task<bool> AddTowarToPlace(PC_MsInwentory pC_MsInwentory, string placeName)
        {
            return await viewModel.AddTowarToPlace(pC_MsInwentory, placeName);
        }

        private async Task<List<Place>> IsPlaceExists(int twr_Gidnumer, int trn_Gidnumer = 0)
        {
            var odp = await viewModel.IsPlaceExists(twr_Gidnumer, trn_Gidnumer);
            return odp.ToList();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.LoadItemsCommand.Execute(null);
            isRaportExists = Task.Run(() => viewModel.IsRaportExists(viewModel.Trn_Gidnumer)).Result;
            
            if (!isRaportExists)
            entry_MM.IsEnabled = !isRaportExists;

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RaportLista_AddTwrKod(viewModel.Trn_Gidnumer));
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrzyjmijMMRaportRoznic(new PrzyjmijMMRaportRcaViewModel(viewModel)));
        }

        private void entry_MM_Focused(object sender, FocusEventArgs e)
        {
            entry_MM.IsEnabled = !isRaportExists;
            if (isRaportExists)
            {
                DisplayAlert("uwaga", "Raport wysłany - edycja niemożliwa", "OK");
            }
        }
    }
}
