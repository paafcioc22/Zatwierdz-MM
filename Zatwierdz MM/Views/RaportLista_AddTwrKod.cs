using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Zatwierdz_MM.Models;
using Zatwierdz_MM.Services;
using Zatwierdz_MM.ViewModels;
using ZXing.Net.Mobile.Forms;
using static Xamarin.Forms.Button;
using static Xamarin.Forms.Button.ButtonContentLayout;

namespace Zatwierdz_MM.Views
{
    class RaportLista_AddTwrKod : ContentPage
    {

        private Label lbl_stan;
        private Label lbl_twrkod;
        private Label lbl_ean;
        private Label lbl_symbol;
        private Label lbl_nazwa;
        private Label lbl_cena;
        private Entry entry_kodean;
        private Entry entry_ilosc;
        private Entry entry_polozenie;
        private Image img_foto;

        private Button btn_AddEanPrefix;
        private Button btn_Zapisz;

        static PC_MsInwentory towar;
        PrzyjmijMMSkanowanieViewModel viewModel;


        string skanean;
        ZXingDefaultOverlay overlay;
        ZXing.Mobile.MobileBarcodeScanningOptions opts;
        ZXingScannerPage scanPage;


        string twrkod;
        string stan_szt;
        string twr_url;
        string twr_nazwa;
        string twr_symbol;
        string twr_ean;
        string twr_cena;
        string polozenie;

        Regex regex;
        public RaportLista_AddTwrKod(int gidnumer) : base() //dodawamoe pozycji
        {

            //if (SettingsPage.SelectedDeviceType == 1)
            //{
            //    WidokAparat(gidnumer);
            //}
            //else
            //{
            //    WidokSkaner(gidnumer);
            //}
            WidokSkaner(gidnumer);
            regex = new Regex(@"^[A-Z]\d{1,2}");
            viewModel = new PrzyjmijMMSkanowanieViewModel();
            entry_polozenie.Focused += Entry_polozenie_Focused;

        }

        private async Task<List<Place>> IsPlaceExists(int twr_Gidnumer, int trn_Gidnumer = 0)
        {
            var odp = await viewModel.IsPlaceExists(twr_Gidnumer, trn_Gidnumer);
            return odp.ToList();
        }

        private async void Entry_polozenie_Focused(object sender, FocusEventArgs e)
        {
             
            var isExists = await IsPlaceExists(towar.MsI_TwrNumer); // czy w ogóle kod dodany
            string odp3 = "";
            string odp = "";

                if (isExists.Count == 0)
                {
                    //var odp2 = await DisplayActionSheet($"Nie przypisano do miejsca :", "Utwórz nowe", "Anuluj", "");

                    //if (odp2 == "Utwórz nowe")
                    //{
                    //    //isExists = await IsPlaceExists( towar.MsI_TwrNumer, towar.MsI_TrnNumer); // czy istniej wpis z tej mmki 

                    //    string placeName = await DisplayPromptAsync("Tworzenie nowego wpisu", "Podaj pozycję np. A10..", "OK", "Anuluj", "", 3, keyboard: Keyboard.Create(KeyboardFlags.CapitalizeCharacter), "");


                    //    if (!await viewModel.IsPlaceEmpty(towar.MsI_TwrNumer, 0, placeName))
                    //        odp3 = await DisplayActionSheet($"Miejsce nie jest puste, odłożyć mimo to? :", "NIE", "TAK", "");


                    //    if (odp3 == "TAK" || string.IsNullOrEmpty(odp3))
                    //    {
                    //        if (!string.IsNullOrEmpty(placeName))
                    //        {
                    //            if (await viewModel.AddTowarToPlace(towar, placeName))
                    //            {
                    //                await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {placeName}", "OK");
                    //                foreach (var item in viewModel.Items)
                    //                {
                    //                    if (item.MsI_TwrNumer == towar.MsI_TwrNumer)
                    //                    {
                    //                        towar.Msi_IsPut = true;

                    //                    }
                    //                }
                    //            }
                    //            else
                    //                await DisplayAlert("info", "Pozycja z tej MM została już dodana", "OK");
                    //        }
                    //        else
                    //        {
                    //            await DisplayAlert("info", "Podaj lokalizacje", "OK");
                    //        }

                    //    }
                    //    //else
                    //    //{
                    //    //    await DisplayAlert("info", "To miejsce jest już zajęte", "OK");
                    //    //}

                     
                }
                else
                {
                    var places = isExists.Select(s => s.PlaceName).Distinct().ToArray();


                    var IsExistsFromThisMM = await IsPlaceExists(towar.MsI_TwrNumer, towar.MsI_TrnNumer);

                    if (IsExistsFromThisMM.Count > 0)
                    {
                        odp = await DisplayActionSheet($"Dodaj do istniejącego :", null, "Anuluj", places);//"Dodaj nowe położenie"

                    }


                if (odp == "Anuluj")
                {
                    return;
                }

                entry_polozenie.Text = odp;

                    //todo: czy dodawać nowe położenie??
                    //odp = await DisplayActionSheet($"Dodaj do istniejącego :",null,  "Anuluj", places);//""
                     

                }
             
        }

        private void WidokSkaner(int gidnumer)
        {

            StackLayout stack_dane = new StackLayout();
            AbsoluteLayout absoluteLayout = new AbsoluteLayout();

            var scrollView = new ScrollView();

            _gidnumer = gidnumer;


            NavigationPage.SetHasNavigationBar(this, false);



            Label lbl_naglowek = new Label()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Dodawanie towaru do położenia",
                FontSize = 20,
                TextColor = Color.Bisque,
                BackgroundColor = Color.Blue
            };
            AbsoluteLayout.SetLayoutBounds(lbl_naglowek, new Rectangle(0, 0, 1, .1));
            AbsoluteLayout.SetLayoutFlags(lbl_naglowek, AbsoluteLayoutFlags.All);




            img_foto = new Image();
            img_foto.Aspect = Aspect.AspectFill;
            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                Launcher.OpenAsync(twr_url.Replace("Miniatury/", "").Replace("small", "large"));
                
            };
            img_foto.GestureRecognizers.Add(tapGesture);
            AbsoluteLayout.SetLayoutBounds(img_foto, new Rectangle(0, 0.1, 1, .5));
            AbsoluteLayout.SetLayoutFlags(img_foto, AbsoluteLayoutFlags.All);



            lbl_stan = new Label();
            lbl_stan.HorizontalOptions = LayoutOptions.Center;

            lbl_nazwa = new Label();
            lbl_nazwa.HorizontalOptions = LayoutOptions.Center;

            lbl_ean = new Label();
            lbl_ean.HorizontalOptions = LayoutOptions.Center;

            lbl_symbol = new Label();
            lbl_symbol.HorizontalOptions = LayoutOptions.Center;

            lbl_cena = new Label();
            lbl_cena.HorizontalOptions = LayoutOptions.Center;


            entry_kodean = new Entry()
            {
                //Keyboard = Keyboard.Text,
                Placeholder = "Wpisz EAN/kod ręcznie lub skanuj",
                Keyboard = Keyboard.Plain,
                //ReturnCommand = new Command(() => entry_ilosc.Focus()) 
            };
            entry_kodean.Unfocused += Kodean_Unfocused;

            entry_ilosc = new Entry()
            {
                Placeholder = "Wpisz Ilość",
                Keyboard = Keyboard.Telephone,
                HorizontalOptions = LayoutOptions.Center,
                ReturnType = ReturnType.Go,
                HorizontalTextAlignment = TextAlignment.Center,

            };


            entry_polozenie = new Entry()
            {
                Placeholder = "Podaj położenie",
                Keyboard = Keyboard.Create(KeyboardFlags.Suggestions | KeyboardFlags.CapitalizeCharacter),
                HorizontalOptions = LayoutOptions.Center,
                ReturnType = ReturnType.Go,
                HorizontalTextAlignment = TextAlignment.Center,
                ClearButtonVisibility = ClearButtonVisibility.WhileEditing,

            };

            

            //,  @"^[A-Z]\d{1,2}";




        



            btn_Zapisz = new Button()
            {
                Text = "Dodaj do położenia",
                ImageSource = "save48x2.png",
                ContentLayout = new ButtonContentLayout(ImagePosition.Right, 10),
                BorderColor = Color.DarkCyan,
                BorderWidth = 2,
                CornerRadius = 10,
            };
            btn_Zapisz.Clicked += Btn_Zapisz_Clicked;
            AbsoluteLayout.SetLayoutBounds(btn_Zapisz, new Rectangle(0.5, 1, .9, 50));
            AbsoluteLayout.SetLayoutFlags(btn_Zapisz, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);



            btn_AddEanPrefix = new Button()
            {
                Text = "+2010000",
                BorderColor = Color.DarkCyan,
                BorderWidth = 2,
                CornerRadius = 20,
            };
            btn_AddEanPrefix.Clicked += Btn_AddEanPrefix_Clicked;
            AbsoluteLayout.SetLayoutBounds(btn_AddEanPrefix, new Rectangle(1, .62, .25, 50));
            AbsoluteLayout.SetLayoutFlags(btn_AddEanPrefix, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);



            stack_dane.Children.Add(lbl_nazwa);
            stack_dane.Children.Add(lbl_ean);
            stack_dane.Children.Add(lbl_cena);
            stack_dane.Children.Add(lbl_symbol);
            stack_dane.Children.Add(entry_kodean);
            stack_dane.Children.Add(entry_ilosc);
            stack_dane.Children.Add(entry_polozenie);
            AbsoluteLayout.SetLayoutBounds(stack_dane, new Rectangle(0, 1, 1, .45));
            AbsoluteLayout.SetLayoutFlags(stack_dane, AbsoluteLayoutFlags.All);

             

            absoluteLayout.Children.Add(img_foto);
            absoluteLayout.Children.Add(lbl_naglowek);
            absoluteLayout.Children.Add(stack_dane);
            absoluteLayout.Children.Add(btn_Zapisz);
            absoluteLayout.Children.Add(btn_AddEanPrefix);

            Content = absoluteLayout;


   

            //Appearing += (object sender, System.EventArgs e) => entry_kodean.Focus();
        }

        private void WidokAparat(int gidnumer)
        {

            SkanowanieEan();

            StackLayout stack_dane = new StackLayout();
            AbsoluteLayout absoluteLayout = new AbsoluteLayout();

            var scrollView = new ScrollView();

            _gidnumer = gidnumer;

            NavigationPage.SetHasNavigationBar(this, false);



            Label lbl_naglowek = new Label()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Dodawanie pozycji (raport przyjęć)",
                FontSize = 20,
                TextColor = Color.Bisque,
                BackgroundColor = Color.DarkCyan
            };
            AbsoluteLayout.SetLayoutBounds(lbl_naglowek, new Rectangle(0, 0, 1, .1));
            AbsoluteLayout.SetLayoutFlags(lbl_naglowek, AbsoluteLayoutFlags.All);




            img_foto = new Image();
            img_foto.Aspect = Aspect.AspectFill;
            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                Launcher.OpenAsync(twr_url.Replace("Miniatury/", "").Replace("small", "large"));
                //Launcher.OpenAsync(twr_url.Replace("Miniatury/", ""));
            };
            img_foto.GestureRecognizers.Add(tapGesture);
            AbsoluteLayout.SetLayoutBounds(img_foto, new Rectangle(0, 0.1, 1, .5));
            AbsoluteLayout.SetLayoutFlags(img_foto, AbsoluteLayoutFlags.All);



            lbl_stan = new Label();
            lbl_stan.HorizontalOptions = LayoutOptions.Center;

            lbl_nazwa = new Label();
            lbl_nazwa.HorizontalOptions = LayoutOptions.Center;

            lbl_ean = new Label();
            lbl_ean.HorizontalOptions = LayoutOptions.Center;

            lbl_symbol = new Label();
            lbl_symbol.HorizontalOptions = LayoutOptions.Center;

            lbl_cena = new Label();
            lbl_cena.HorizontalOptions = LayoutOptions.Center;


            entry_kodean = new Entry()
            {
                //Keyboard = Keyboard.Text,
                Placeholder = "Wpisz EAN/kod ręcznie lub skanuj",
                Keyboard = Keyboard.Plain,
                //ReturnCommand = new Command(() => entry_ilosc.Focus())
            };
            entry_kodean.Unfocused += Kodean_Unfocused;

            entry_ilosc = new Entry()
            {
                Placeholder = "Wpisz Ilość",
                Keyboard = Keyboard.Telephone,
                HorizontalOptions = LayoutOptions.Center,
                ReturnType = ReturnType.Go,
                HorizontalTextAlignment = TextAlignment.Center,

            };
            entry_ilosc.Completed += (object sender, EventArgs e) =>
            {
                Zapisz();
            };
            //entry_ilosc.Keyboard = Keyboard.Text;



            btn_Zapisz = new Button()
            {
                Text = "Zapisz pozycję",
                ImageSource = "save48x2.png",
                ContentLayout = new ButtonContentLayout(ImagePosition.Right, 10),
                BorderColor = Color.DarkCyan,
                BorderWidth = 2,
                CornerRadius = 10,
            };
            btn_Zapisz.Clicked += Btn_Zapisz_Clicked;
            AbsoluteLayout.SetLayoutBounds(btn_Zapisz, new Rectangle(0.5, 1, .9, 50));
            AbsoluteLayout.SetLayoutFlags(btn_Zapisz, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);



            btn_AddEanPrefix = new Button()
            {
                Text = "+2010000",
                BorderColor = Color.DarkCyan,
                BorderWidth = 2,
                CornerRadius = 20,
            };
            btn_AddEanPrefix.Clicked += Btn_AddEanPrefix_Clicked;
            AbsoluteLayout.SetLayoutBounds(btn_AddEanPrefix, new Rectangle(1, .8, .25, 50));
            AbsoluteLayout.SetLayoutFlags(btn_AddEanPrefix, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);



            stack_dane.Children.Add(lbl_nazwa);
            stack_dane.Children.Add(lbl_ean);
            stack_dane.Children.Add(lbl_cena);
            stack_dane.Children.Add(lbl_symbol);
            stack_dane.Children.Add(entry_ilosc);
            stack_dane.Children.Add(entry_kodean);
            AbsoluteLayout.SetLayoutBounds(stack_dane, new Rectangle(0, 1, 1, .45));
            AbsoluteLayout.SetLayoutFlags(stack_dane, AbsoluteLayoutFlags.All);




            //stackLayout.VerticalOptions = LayoutOptions.EndAndExpand; //Center
            //stackLayout.Padding = new Thickness(15, 0, 15, 0);

            absoluteLayout.Children.Add(img_foto);
            absoluteLayout.Children.Add(lbl_naglowek);
            absoluteLayout.Children.Add(stack_dane);
            absoluteLayout.Children.Add(btn_Zapisz);
            absoluteLayout.Children.Add(btn_AddEanPrefix);

            Content = absoluteLayout;


        }

        private void Btn_AddEanPrefix_Clicked(object sender, EventArgs e)
        {

            UstawFocus();
        }

        void UstawFocus()
        {
            entry_kodean.Focused += (sender, args) =>
            {
                entry_kodean.Keyboard = Keyboard.Telephone;
                entry_kodean.Text = "2010000";


            };
            entry_kodean.Focus();



        }

        //public RaportLista_AddTwrKod(DaneMMElem mmka) //edycja
        //{
        //    this.Title = "Dodaj MM";

        //    _MMElement = mmka;


        //    NavigationPage.SetHasNavigationBar(this, false);

        //    StackLayout stackLayout = new StackLayout();
        //    StackLayout stackLayout_gl = new StackLayout();
        //    StackLayout stack_naglowek = new StackLayout();

        //    Label lbl_naglowek = new Label();
        //    lbl_naglowek.HorizontalOptions = LayoutOptions.CenterAndExpand;
        //    lbl_naglowek.VerticalOptions = LayoutOptions.Start;
        //    lbl_naglowek.Text = "Szczegóły pozycji";
        //    lbl_naglowek.FontSize = 20;
        //    lbl_naglowek.TextColor = Color.Bisque;
        //    lbl_naglowek.BackgroundColor = Color.DarkCyan;

        //    stack_naglowek.HorizontalOptions = LayoutOptions.FillAndExpand;
        //    stack_naglowek.VerticalOptions = LayoutOptions.Start;
        //    stack_naglowek.BackgroundColor = Color.DarkCyan;
        //    stack_naglowek.Children.Add(lbl_naglowek);

        //    stackLayout_gl.Children.Add(stack_naglowek);



        //    img_foto = new Image();
        //    img_foto.Source = mmka.Url.Replace("Miniatury/", "");
        //    var tapGestureRecognizer = new TapGestureRecognizer();
        //    tapGestureRecognizer.Tapped += (s, e) =>
        //    {
        //        Launcher.OpenAsync(mmka.Url.Replace("Miniatury/", ""));
        //    };
        //    img_foto.GestureRecognizers.Add(tapGestureRecognizer);
        //    stackLayout.Children.Add(img_foto);
        //    //_gidnumer = mmka.gi;


        //    lbl_stan = new Label();
        //    lbl_stan.HorizontalOptions = LayoutOptions.Center;
        //    lbl_stan.Text = "Ilość : " + mmka.Ilosc + " szt";

        //    stackLayout.Children.Add(lbl_stan);

        //    lbl_twrkod = new Label();
        //    lbl_twrkod.HorizontalOptions = LayoutOptions.Center;
        //    lbl_twrkod.Text = "Kod towaru : " + mmka.Twr_Kod;

        //    lbl_ean = new Label();
        //    lbl_ean.HorizontalOptions = LayoutOptions.Center;
        //    lbl_ean.Text = "Ean : " + mmka.Ean;

        //    entry_kodean = new Entry()
        //    {
        //        HorizontalOptions = LayoutOptions.Center,
        //        Text = "Ean : " + mmka.Ean,

        //    };

        //    //lbl_symbol = new Label();
        //    //lbl_symbol.HorizontalOptions = LayoutOptions.Center;
        //    //lbl_symbol.Text = "Symbol : " + mmka.symbol;

        //    lbl_nazwa = new Label();
        //    lbl_nazwa.HorizontalOptions = LayoutOptions.Center;
        //    lbl_nazwa.Text = "Nazwa : " + mmka.Twr_Nazwa;

        //    lbl_cena = new Label();
        //    lbl_cena.HorizontalOptions = LayoutOptions.Center;
        //    lbl_cena.Text = "Cena : " + mmka.Cena + " Zł";


        //    Button open_url = new Button();
        //    open_url.Text = "Otwórz zdjęcie";
        //    open_url.CornerRadius = 15;

        //    open_url.Clicked += Open_url_Clicked;
        //    //open_url.BackgroundColor = Color.FromHex("#3CB371");
        //    //open_url.VerticalOptions = LayoutOptions.EndAndExpand;
        //    //open_url.Margin = new Thickness(15, 0, 15, 5);


        //    stackLayout.Children.Add(lbl_twrkod);
        //    stackLayout.Children.Add(lbl_nazwa);
        //    //stackLayout.Children.Add(lbl_ean);
        //    stackLayout.Children.Add(entry_kodean);
        //    //stackLayout.Children.Add(lbl_symbol);
        //    stackLayout.Children.Add(lbl_cena);


        //    stackLayout.VerticalOptions = LayoutOptions.Center;
        //    stackLayout.Padding = new Thickness(15, 0, 15, 0);
        //    stackLayout.Spacing = 8;
        //    stackLayout_gl.Children.Add(stackLayout);
        //    //stackLayout_gl.Children.Add(open_url);

        //    Content = stackLayout_gl;
        //    // GetDataFromTwrKod(mmka.twrkod);
        //    //entry_ilosc.Focus();
        //}


        public RaportLista_AddTwrKod(DaneMMElem akcje) //edycja
        {
            this.Title = "Dodaj MM";




            NavigationPage.SetHasNavigationBar(this, false);

            StackLayout stackLayout = new StackLayout();
            StackLayout stackLayout_gl = new StackLayout();
            StackLayout stack_naglowek = new StackLayout();

            Label lbl_naglowek = new Label();
            lbl_naglowek.HorizontalOptions = LayoutOptions.CenterAndExpand;
            lbl_naglowek.VerticalOptions = LayoutOptions.Start;
            lbl_naglowek.Text = "Szczegóły pozycji";
            lbl_naglowek.FontSize = 20;
            lbl_naglowek.TextColor = Color.Bisque;
            lbl_naglowek.BackgroundColor = Color.DarkCyan;

            stack_naglowek.HorizontalOptions = LayoutOptions.FillAndExpand;
            stack_naglowek.VerticalOptions = LayoutOptions.Start;
            stack_naglowek.BackgroundColor = Color.DarkCyan;
            stack_naglowek.Children.Add(lbl_naglowek);

            stackLayout_gl.Children.Add(stack_naglowek);



            img_foto = new Image();
            img_foto.Source = akcje.Url.Replace("Miniatury/", "").Replace("small", "home");
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                Launcher.OpenAsync(akcje.Url.Replace("Miniatury/", "").Replace("small", "large"));
            };
            img_foto.GestureRecognizers.Add(tapGestureRecognizer);
            stackLayout.Children.Add(img_foto);
            //_gidnumer = mmka.gi;


            lbl_stan = new Label();
            lbl_stan.HorizontalOptions = LayoutOptions.Center;
            lbl_stan.Text = "Stan : " + akcje.Ilosc + " szt";
            lbl_stan.FontAttributes = FontAttributes.Bold;

            stackLayout.Children.Add(lbl_stan);

            lbl_twrkod = new Label();
            lbl_twrkod.HorizontalOptions = LayoutOptions.Center;
            lbl_twrkod.Text = "Kod towaru : " + akcje.Twr_Kod;

            lbl_ean = new Label();
            lbl_ean.HorizontalOptions = LayoutOptions.Center;
            lbl_ean.Text = akcje.Ean;

            entry_kodean = new Entry()
            {
                HorizontalOptions = LayoutOptions.Center,
                Keyboard = Keyboard.Numeric,
                Text = akcje.Ilosc_Skan == 0 ? "" : akcje.Ilosc_Skan.ToString(),
                WidthRequest = 60,
                IsEnabled = false
            };

            //lbl_symbol = new Label();
            //lbl_symbol.HorizontalOptions = LayoutOptions.Center;
            //lbl_symbol.Text = "Symbol : " + akcje.TwrSymbol;

            lbl_nazwa = new Label();
            lbl_nazwa.HorizontalOptions = LayoutOptions.Center;
            lbl_nazwa.Text = "Nazwa : " + akcje.Twr_Nazwa;

            lbl_cena = new Label();
            lbl_cena.HorizontalOptions = LayoutOptions.Center;
            lbl_cena.Text = "Cena : " + akcje.Cena + " Zł";


            Button open_url = new Button();
            open_url.Text = "Zacznij skanowanie";
            open_url.CornerRadius = 15;

            //open_url.Clicked += Open_url_Clicked;
            overlay = new ZXingDefaultOverlay
            {
                TopText = $"Skanowany : {akcje.Twr_Kod}",
                BottomText = $"Zeskanowanych szt : {ile}",
                AutomationId = "zxingDefaultOverlay",


            };

            var torch = new Switch
            {
            };

            torch.Toggled += delegate
            {
                scanPage.ToggleTorch();
            };

            overlay.Children.Add(torch);
            open_url.Clicked += async delegate
            {
                scanPage = new ZXingScannerPage(
                    new ZXing.Mobile.MobileBarcodeScanningOptions { DelayBetweenContinuousScans = 3000 }, overlay);
                scanPage.DefaultOverlayShowFlashButton = true;
                scanPage.OnScanResult += (result) =>
                Device.BeginInvokeOnMainThread(() =>
                {
                    skanean = result.Text;

                    if (skanean == lbl_ean.Text)
                    {
                        ile += 1;
                        overlay.BottomText = $"Zeskanowanych szt : {ile}";
                        DisplayAlert(null, $"Zeskanowanych szt : {ile}", "OK");

                        entry_kodean.Text = ile.ToString();

                    }
                    else
                    {

                        DisplayAlert(null, "Probujesz zeskanować inny model..", "OK");
                    }
                });
                await Navigation.PushModalAsync(scanPage);
            };
            //open_url.BackgroundColor = Color.FromHex("#3CB371");
            open_url.VerticalOptions = LayoutOptions.EndAndExpand;
            //open_url.Margin = new Thickness(15, 0, 15, 5);


            stackLayout.Children.Add(lbl_twrkod);
            stackLayout.Children.Add(lbl_nazwa);
            stackLayout.Children.Add(lbl_ean);
            stackLayout.Children.Add(entry_kodean);
            stackLayout.Children.Add(lbl_symbol);
            stackLayout.Children.Add(lbl_cena);


            stackLayout.VerticalOptions = LayoutOptions.Center;
            stackLayout.Padding = new Thickness(15, 0, 15, 0);
            stackLayout.Spacing = 8;
            stackLayout_gl.Children.Add(stackLayout);
            stackLayout_gl.Children.Add(open_url);

            Content = stackLayout_gl;
            // GetDataFromTwrKod(mmka.twrkod);
            //entry_ilosc.Focus();
        }


        int ile = 0;
        private int _gidnumer;

        private void Open_url_Clicked(object sender, EventArgs e)
        {
            //Device.OpenUri(new Uri(_MMElement.url.Replace("Miniatury/", "")));
            //_akcja.TwrSkan = Convert.ToInt32(entry_kodean.Text);
            Navigation.PopModalAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            //if (ile > 0)
            //    _akcja.TwrSkan = ile;

            return base.OnBackButtonPressed();
        }

        private void Kodean_Unfocused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(entry_kodean.Text))
                pobierztwrkod(entry_kodean.Text);
            if (!string.IsNullOrEmpty(twrkod))
                entry_ilosc.Focus();
        }

        private void Btn_Update_Clicked(object sender, EventArgs e)
        {
            //if (entry_ilosc.Text != null && entry_kodean.Text != null)
            //{
            //    if (Int32.Parse(entry_ilosc.Text) > Int32.Parse(stan_szt))
            //    {
            //        DisplayAlert(null, "Wpisana ilość przekracza stan {}", "OK");
            //    }
            //    else
            //    {
            //        Model.DokMM dokMM = new Model.DokMM();
            //        dokMM.gidnumer = _gidnumer;
            //        dokMM.twrkod = entry_kodean.Text;
            //        dokMM.szt = Convert.ToInt32(entry_ilosc.Text);
            //        dokMM.UpdateElement(dokMM);
            //        //Model.DokMM.dokElementy.GetEnumerator();// (usunMM);
            //        dokMM.getElementy(_gidnumer);
            //        Navigation.PopModalAsync();
            //    }
            //}
            //else
            //{
            //    DisplayAlert("Uwaga", "Nie uzupełniono wszystkich pól!", "OK");
            //}
        }


        //public async Task ZapiszISkanujDalej()
        //{
        //    await Task.Run(() => //Task.Run automatically unwraps nested Task types!
        //    {
        //        //  Zapisz();
        //        Task.Delay(5000);
        //        DisplayAlert(null, "po zapisie", "ok");
        //        // SkanowanieEan();
        //    });

        //}


       

        public async void Zapisz(string placeName="")
        {
            short ilosc;
            string odp="";

            short.TryParse(entry_ilosc.Text, out ilosc);
            towar.MsI_TwrIloscSkan = ilosc;
            towar.MsI_TrnNumer = 1;

            if (regex.IsMatch(placeName))
            {
                if (!await viewModel.IsPlaceEmpty(towar.MsI_TwrNumer, 0, placeName))
                {

                     odp = await DisplayActionSheet($"Miejsce nie jest puste, odłożyć mimo to? :", "NIE", "TAK", "");
                     
                }


                if (odp == "TAK"||string.IsNullOrEmpty(odp))
                {
                    if (!string.IsNullOrEmpty(placeName))
                    {
                        if (await viewModel.AddTowarToPlace(towar, placeName))
                        {
                            await DisplayAlert("info", $"Dodano {towar.MsI_TwrIloscSkan} szt do {placeName}", "OK");
                            await Navigation.PopModalAsync();
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
            }

            else
                await DisplayAlert("uwaga", "nazwa połozenia zawiera błędy", "OK");
        }



        private void Btn_Zapisz_Clicked(object sender, EventArgs e)
        {
            Zapisz(entry_polozenie.Text);

        }

        private async void SkanowanieEan()
        {

            opts = new ZXing.Mobile.MobileBarcodeScanningOptions()
            {

                AutoRotate = false,
                PossibleFormats = new List<ZXing.BarcodeFormat>() {
                //ZXing.BarcodeFormat.EAN_8,
                ZXing.BarcodeFormat.EAN_13,
                //ZXing.BarcodeFormat.CODE_128,
                //ZXing.BarcodeFormat.CODABAR,
                ZXing.BarcodeFormat.CODE_39,
                },
                // CameraResolutionSelector = availableResolutions => {

                //     foreach (var ar in availableResolutions)
                //     {
                //         Console.WriteLine("Resolution: " + ar.Width + "x" + ar.Height);
                //     }
                //     return availableResolutions[0];
                // },
                //DelayBetweenContinuousScans=3000

            };

            opts.TryHarder = true;



            var torch = new Switch
            {

            };

            torch.Toggled += delegate
            {
                scanPage.ToggleTorch();

            };

            // scanPage.ToggleTorch();

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
            //customOverlay.Children.Add(btn_Manual);

            // var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            grid.Children.Add(Overlay);
            Overlay.Children.Add(torch);
            Overlay.BindingContext = Overlay;

            scanPage = new ZXingScannerPage(opts, customOverlay: Overlay)
            {
                DefaultOverlayTopText = "Zeskanuj kod ",
                //DefaultOverlayBottomText = " Skanuj kod ";
                DefaultOverlayShowFlashButton = true,
                IsTorchOn = true,  //////dodane

            };


            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                scanPage.AutoFocus();
                scanPage.IsTorchOn = true; //dodane
                scanPage.HasTorch = true; //dodane
                Device.BeginInvokeOnMainThread(() =>
            {

                Device.StartTimer(new TimeSpan(0, 0, 0, 2), () =>
                {
                    if (scanPage.IsScanning)
                    {
                        scanPage.AutoFocus();
                        scanPage.IsTorchOn = true;
                    }

                    return true;
                });
                Navigation.PopModalAsync();
                pobierztwrkod(result.Text);
                entry_ilosc.Focus();
            });
            };
            await Navigation.PushModalAsync(scanPage); /////!!!!!!!


            Device.StartTimer(new TimeSpan(0, 0, 0, 2), () =>
            {
                scanPage.IsTorchOn = true;
                torch.IsToggled = true;

                return false;
            });
        }




        private void Btn_Skanuj_Clicked(object sender, EventArgs e)
        {
            SkanowanieEan();
        }





        public async void pobierztwrkod(string _ean)
        {
            var app = Application.Current as App;
            

            if (!string.IsNullOrEmpty(_ean) || _ean != "2010000")
                try
                {


                    var Webquery = $@"cdn.PC_WykonajSelect N'Select Twr_Kod, Twr_Nazwa, Twr_Katalog Twr_Symbol, cast(twc_wartosc as decimal(5,2))Cena ,cast(sum(TwZ_Ilosc) as int)Ilosc, cdn.PC_GetTwrUrl(Twr_Kod) as Url ,Twr_Ean Ean , Twr_Gidnumer
		                from cdn.TwrKarty 
		                join cdn.TwrCeny on Twr_GIDNumer = TwC_TwrNumer and TwC_TwrLp = 2 
		                left join cdn.TwrZasoby on Twr_GIDNumer = TwZ_TwrNumer where twr_ean=''{_ean}'' or twr_kod=''{_ean}''
		                group by twr_kod, twr_nazwa, Twr_Katalog,twc_wartosc, twr_ean,Twr_Gidnumer'";



                    var dane = await App.TodoManager.PobierzDaneZWeb<DaneMMElem>(Webquery);

                    var karta = dane.FirstOrDefault(); 

                    towar = new PC_MsInwentory()
                    {
                        Cena=karta.Cena,
                        Ean= karta.Ean,
                        MsI_TwrNumer= karta.Twr_Gidnumer,
                        
                    };

                    if (dane.Count > 0)
                    {
                        twrkod = dane[0].Twr_Kod;
                        twr_url = dane[0].Url;
                        twr_nazwa = dane[0].Twr_Nazwa;
                        twr_ean = dane[0].Ean;
                        twr_cena = dane[0].Cena;

                    }
                    else

                        await DisplayAlert("Uwaga", "Kod nie istnieje!", "OK");



                    entry_kodean.Text = twrkod;
                    lbl_ean.Text = twr_ean;
                    lbl_symbol.Text = twr_symbol;
                    lbl_nazwa.Text = twr_nazwa;
                    lbl_cena.Text = twr_cena;
                    lbl_stan.Text = "Stan : " + stan_szt;
                    if (!string.IsNullOrEmpty(twr_url))
                        img_foto.Source = twr_url.Replace("Miniatury/", "").Replace("small", "home"); //twr_url;

                }
                catch (Exception)
                {
                    await DisplayAlert("Uwaga", "Nie znaleziono towaru", "OK");
                }


            //return twrkod;

        }


    }
}

