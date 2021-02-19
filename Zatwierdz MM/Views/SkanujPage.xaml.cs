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
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;

namespace Zatwierdz_MM.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SkanujPage : ContentPage
    {
        SkanujViewModel viewModel;
        //ZXingDefaultOverlay overlay;
        ZXingScannerPage scanPage;
        //string EANKodMM;
        ZXingScannerView zxing;
        MobileBarcodeScanningOptions opts;

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
                await Task.Delay(300);
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



        private async void Button_Clicked(object sender, EventArgs e)
        {

            opts = new ZXing.Mobile.MobileBarcodeScanningOptions()
            {
                AutoRotate = false,
                PossibleFormats = new List<ZXing.BarcodeFormat>() {

                ZXing.BarcodeFormat.CODE_128,
                ZXing.BarcodeFormat.CODABAR,
                ZXing.BarcodeFormat.CODE_39,

                }

            };

            opts.TryHarder = true;

            zxing = new ZXingScannerView
            {

                IsScanning = false,
                IsTorchOn = false,
                IsAnalyzing = false,
                AutomationId = "zxingDefaultOverlay",//zxingScannerView
                Opacity = 22,
                Options = opts
            };

            var torch = new Switch
            {
            };

            torch.Toggled += delegate
            {
                scanPage.ToggleTorch();
            };

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };

            var Overlay = new ZXingDefaultOverlay
            {
                TopText = "Włącz latarkę",
                BottomText = "Skanowanie rozpocznie się automatycznie",
                ShowFlashButton = true,
                AutomationId = "zxingDefaultOverlay",

            };

            var customOverlay = new StackLayout
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand
            };

            grid.Children.Add(Overlay);
            Overlay.Children.Add(torch);
            Overlay.BindingContext = Overlay;

            scanPage = new ZXingScannerPage(opts, customOverlay: Overlay)
            {
                DefaultOverlayTopText = "Zeskanuj kod ",
                //DefaultOverlayBottomText = " Skanuj kod ";
                DefaultOverlayShowFlashButton = true

            };
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                scanPage.AutoFocus();
                Device.BeginInvokeOnMainThread(() =>
                {

                    Device.StartTimer(new TimeSpan(0, 0, 0, 2), () =>
                    {
                        if (scanPage.IsScanning) scanPage.AutoFocus();
                        return true;
                    });
                    Navigation.PopModalAsync();
                    entry_MM.Text= result.Text;
                    viewModel.ExecInsertToBase(result.Text);

                });
            };
            await Navigation.PushModalAsync(scanPage);
        }



    }
}