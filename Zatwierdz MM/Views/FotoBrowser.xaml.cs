using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zatwierdz_MM.ViewModels;

namespace Zatwierdz_MM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FotoBrowser : ContentPage
    {
        private readonly FotoBrowserViewModel viewModel;

        public List<string> KontrahentList { get; set; }
        public List<string> GrupaList { get; set; }
         

        public FotoBrowser()
        {
            InitializeComponent();
            BindingContext = viewModel = new FotoBrowserViewModel();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void grupaSuggetstBox_TextChanged(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxTextChangedEventArgs e)
        {
            if (GrupaList != null)
            {
                grupaSuggetstBox.ItemsSource = string.IsNullOrWhiteSpace(grupaSuggetstBox.Text)
               ? null
               : GrupaList.Where(filter => filter.StartsWith(grupaSuggetstBox.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

        }

        private void kotrahentSugestsBox_TextChanged(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxTextChangedEventArgs e)
        {
            try
            {
                if (KontrahentList!=null)
                {
                    kotrahentSugestsBox.ItemsSource = string.IsNullOrWhiteSpace(kotrahentSugestsBox.Text)
                      ? null
                      : KontrahentList.Where(filter => filter.StartsWith(kotrahentSugestsBox.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }
               
            }
            catch (Exception)
            {

                throw;
            }
                   
        }

        private  void kotrahentSugestsBox_QuerySubmitted(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxQuerySubmittedEventArgs e)
        {
             
        }

        private async void grupaSuggetstBox_QuerySubmitted(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            await viewModel.GetDataWithFoto(grupaSuggetstBox.Text, kotrahentSugestsBox.Text);
        }

        private async void kotrahentSugestsBox_Focused(object sender, FocusEventArgs e)
        {
             KontrahentList= await viewModel.GetKontrahents(grupaSuggetstBox.Text, kotrahentSugestsBox.Text);
        }

        private async void grupaSuggetstBox_Focused(object sender, FocusEventArgs e)
        {
            GrupaList=  await viewModel.GetMainGropup(kotrahentSugestsBox.Text);
        }

       
    }
}
