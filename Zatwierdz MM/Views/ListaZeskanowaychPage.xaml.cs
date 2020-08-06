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

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (zatwierdzonevm.Items.Count == 0)
                zatwierdzonevm.LoadItemsCommand.Execute(null);
        }
    }
}
