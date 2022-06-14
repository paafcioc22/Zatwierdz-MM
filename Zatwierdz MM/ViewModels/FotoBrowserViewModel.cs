using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.ViewModels
{
    public class FotoBrowserViewModel : BaseViewModel
    {
        public ICommand ClickCommand { get; private set; }
        INavigation Navigation;
        private List<string> kontrahentList;
        private List<string> GrupaList;
        public ObservableCollection<FotoBrowser> FotoBrowserList { get; set; }

        public FotoBrowserViewModel()
        {
            kontrahentList = new List<string>();
            Title = "Przeglądarka zdjęć";
            GrupaList = new List<string>();
            FotoBrowserList = new ObservableCollection<FotoBrowser>();
            ClickCommand = new Command<FotoBrowser>(async (FotoBrowser url) => await OpenUrl(url));
        }

        private async Task OpenUrl(FotoBrowser karta)
        {
            if (karta != null)
            {
                await Browser.OpenAsync(karta.Url.Replace("Miniatury/", "").Replace("small", "large"));

                await Clipboard.SetTextAsync(karta.Twr_Ean);
                DependencyService.Get<Services.IWebService>().ShowLong("Skopiowano Ean");
            }
         
        }

        internal async Task<List<string>> GetKontrahents(string grupa, string kontrahent)
        {

                kontrahentList.Clear();
            try
            {

                string Webquery = $@"cdn.PC_WykonajSelect N'      
                select    Knt_Akronim as Kontrahent 
                from cdn.twrkarty t 
	                join cdn.TraElem on TrE_TwrNumer=t.Twr_GIDNumer
	                join cdn.tranag a on a.TrN_GIDNumer=TrE_GIDNumer and a.trn_gidtyp=TrE_GIDTyp 
	                left join cdn.KntKarty on a.TrN_KntNumer = Knt_GIDNumer
                    where   tre_gidtyp in(1489,1490,1497,1498)  
                    
					and TrN_VatRok>YEAR(getdate())-6
					group by Knt_Akronim'";
                // var twrdane = await App.TodoManager.GetDataFromWeb(Webquery);


                var kontahenci = await App.TodoManager.PobierzDaneZWeb<FotoBrowser>(Webquery);

                if (kontahenci != null)
                {
                    foreach (var i in kontahenci)
                    {
                        kontrahentList.Add(i.Kontrahent);
                    }

                    return kontrahentList;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
             
        }

        internal async Task<List<string>> GetMainGropup(string kontrahent)
        {
            string filtrKnt = "";


            if (!string.IsNullOrEmpty(kontrahent))
            {
                filtrKnt = $"and Knt_Akronim like ''%{ kontrahent}%''";
            }


                GrupaList.Clear();
            try
            {

                string Webquery = $@"cdn.PC_WykonajSelect N' 
                select   twg_kod  TwgKod 
                from cdn.twrkarty t
				JOIN CDN.TwrGrupyDom ON t.Twr_GIDTyp = TGD_GIDTyp AND t.Twr_GIDNumer = TGD_GIDNumer 
				JOIN CDN.TwrGrupy ON TGD_GrOTyp = TwG_GIDTyp AND TGD_GrONumer = TwG_GIDNumer
	                join cdn.TraElem on TrE_TwrNumer=t.Twr_GIDNumer
	                join cdn.tranag a on a.TrN_GIDNumer=TrE_GIDNumer and a.trn_gidtyp=TrE_GIDTyp 
	                left join cdn.KntKarty on a.TrN_KntNumer = Knt_GIDNumer
                     where   tre_gidtyp in(1489,1490,1497,1498)   
                        {filtrKnt}
						and TrN_VatRok>YEAR(getdate())-6
						group by  twg_kod 
						 '";
                // var twrdane = await App.TodoManager.GetDataFromWeb(Webquery);


                var grupy = await App.TodoManager.PobierzDaneZWeb<FotoBrowser>(Webquery);

                foreach (var i in grupy)
                {
                    GrupaList.Add(i.TwgKod);
                }

                return GrupaList;
            }
            catch (Exception)
            {

                throw;
            }

        }




        internal async Task<ObservableCollection<FotoBrowser>> GetDataWithFoto(string grupa, string kontrahent)
        {

            IsBusy = true;

            FotoBrowserList.Clear();
            try
            {

                string Webquery = $@"cdn.PC_WykonajSelect N' 
                select   t.twr_kod TwrKod, twg_kod TwgKod,Knt_Akronim as Kontrahent, max(trn_gidnumer)gg 
	,CDN.PC_GetTwrUrl3(t.twr_gidnumer,1) as Url ,Twr_Ean
                from cdn.twrkarty t
				JOIN CDN.TwrGrupyDom ON t.Twr_GIDTyp = TGD_GIDTyp AND t.Twr_GIDNumer = TGD_GIDNumer 
				JOIN CDN.TwrGrupy ON TGD_GrOTyp = TwG_GIDTyp AND TGD_GrONumer = TwG_GIDNumer
	                join cdn.TraElem on TrE_TwrNumer=t.Twr_GIDNumer
	                join cdn.tranag a on a.TrN_GIDNumer=TrE_GIDNumer and a.trn_gidtyp=TrE_GIDTyp 
	                join cdn.KntKarty on a.TrN_KntNumer = Knt_GIDNumer
                     where   tre_gidtyp in(1489,1490,1497,1498)  and
                         twg_kod like ''{grupa}%''
                        and Knt_Akronim like ''%{kontrahent}%''
						and TrN_VatRok>YEAR(getdate())-6
						group by t.twr_kod, twg_kod,Knt_Akronim,t.twr_gidnumer,	Twr_Ean
						order by gg desc '";



                var grupy = await App.TodoManager.PobierzDaneZWeb<FotoBrowser>(Webquery);

                if (grupy.Count > 0)
                {
                    foreach (var i in grupy)
                    {
                        //i.Url = ConvertUrlToOtherSize(i.Url, i.TwrKod, OtherSize.small);
                        FotoBrowserList.Add(i);
                    }
                }

              

                IsBusy = false;
                return FotoBrowserList;
            }
            catch (Exception)
            {

                throw;
            }

        }


       


    }
}
