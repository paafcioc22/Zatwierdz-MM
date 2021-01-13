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
using System.Threading;

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
            entry_MM.Completed += Entry_MM_Completed;
           
        }

        private async void Entry_MM_Completed(object sender, EventArgs e)
        {
            try
            {
                entry_MM.Unfocus();
                await Task.Delay(5000);
                entry_MM.Focus();
            }
            catch (Exception s)
            {

                await DisplayAlert("Bład", s.Message, "OK");
            }
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
          //  Appearing += (object sender, System.EventArgs e) => entry_MM.Focus();
            //kodean.ReturnCommand = new Command(() => ilosc.Focus());

            if (viewModel.Items.Count == 0)
                viewModel.IsBusy = true;
            //entry_MM.Focus();
        }

        //private async void Entry_Completed(object sender, EventArgs e)
        //{
            
        //    await DisplayAlert("info", $"Dodano MM do listy : {entry_MM.Text}", "OK");
        //    entry_MM.Text = "";
        //}
    }
}