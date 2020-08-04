using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Zatwierdz_MM.Models;
using Zatwierdz_MM.Views;
using Zatwierdz_MM.ViewModels;

namespace Zatwierdz_MM.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SkanujPage : ContentPage
    {
        SkanujViewModel viewModel;

        public SkanujPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new SkanujViewModel();
        }

        async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject)sender;
            var item = (Item)layout.BindingContext;
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));
        }

      

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.IsBusy = true;
        }

        //private async void Entry_Completed(object sender, EventArgs e)
        //{
            
        //    await DisplayAlert("info", $"Dodano MM do listy : {entry_MM.Text}", "OK");
        //    entry_MM.Text = "";
        //}
    }
}