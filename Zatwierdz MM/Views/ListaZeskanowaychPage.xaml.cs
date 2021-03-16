using System;
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
    public partial class ListaZeskanowaychPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        ZatwierdzoneMMViewModel zatwierdzonevm;

        public ListaZeskanowaychPage()
        {
            InitializeComponent();
            BindingContext = zatwierdzonevm = new ZatwierdzoneMMViewModel();


        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            var klik = e.Item as DaneMM;



            //await DisplayAlert("Otwieram", $"MM numer {klik.Trn_GidNumer}.", "OK");

            await Navigation.PushAsync(new PrzyjmijMMPage(new PrzyjmijMMViewModel(klik)));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //if (zatwierdzonevm.Items.Count == 0)
            if(!string.IsNullOrEmpty(zatwierdzonevm.Filter))
                zatwierdzonevm.LoadItemsCommand.Execute(zatwierdzonevm.Filter);
            else
                zatwierdzonevm.LoadItemsCommand.Execute(null);

        }
    }
}
