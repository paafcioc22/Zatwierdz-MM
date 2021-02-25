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

            List<string> kolory = new List<string> {"Odłóż na miejsce odkładcze", "Edytuj"};

            var towar = e.Item as PC_MsInwentory;

            var odp =await  DisplayActionSheet($"Wybierz :", "OK", "Anuluj", kolory.ToArray());
            if (odp == "Odłóż na miejsce odkładcze")
            {
               var isExists= await  IsPlaceExists(towar.MsI_TrnNumer, towar.MsI_TwrNumer);


                if (isExists.Count == 0)
                {
                    var odp2=await DisplayActionSheet($"Nie przypisano do miejsca :", "Utwórz nowe", "Anuluj", "");

                    if(odp2== "Utwórz nowe")
                    {

                    }
                }
            }
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async Task<List<Place>> IsPlaceExists(int trn_Gidnumer, int twr_Gidnumer)
        {
            var odp= await viewModel.IsPlaceExists(trn_Gidnumer, twr_Gidnumer) ;
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
    }
}
