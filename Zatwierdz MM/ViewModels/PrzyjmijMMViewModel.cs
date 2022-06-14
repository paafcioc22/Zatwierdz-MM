using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.ViewModels
{

   

    public class PrzyjmijMMViewModel : BaseViewModel
    {
        ICommand tapCommand;

        public Command LoadItemsCommand { get; set; }
        public ICommand DeleteFromList { get; }
        public ICommand OpenWebCommand { get; }
        // public ICommand SearchCommand => new Command(Search);
        public ObservableCollection<DaneMMElem> Items { get; private set; }
        DaneMM dane;
        public ICommand TapCommand
        {
            get { return tapCommand; }
        }


        public PrzyjmijMMViewModel(DaneMM daneMM)
        {
            //Title = "Lista Zeskanowanych";
            Items = new ObservableCollection<DaneMMElem>();
            dane = daneMM;
            Title = daneMM.Trn_NrDokumentu;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            //OpenWebCommand = new Command<string>(async (string url) => await Browser.OpenAsync(url));
            tapCommand = new Command<string>(async (string url) => await OnTapped(url));
        }

        async Task OnTapped(string s)
        {
            
            await Browser.OpenAsync(s);
        }

        async Task<string> GetOpisFromRaport()
        {
            var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N' select top 1 MsR_Opis from[CDN].[PC_MsRaport]
            where[MsR_TrnNumer] = {dane.Trn_GidNumer}'";

            var opis= await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(sqlPobierzMMki);
           
            if (opis.Count > 0)
                return opis[0].MsR_Opis;
            return "";

        }
        async Task ExecuteLoadItemsCommand(string filtr = "")
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();

                this.Description =await  GetOpisFromRaport();

                var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'
    select *  from 
 (
	   select  TrN_DokumentObcy,min(TrE_GIDLp) TrE_GIDLp ,  Trn_Gidnumer, Twr_Gidnumer,Mag_GidNumer,twr_ean Ean,
	Twr_Kod,Twr_Nazwa,  Mag_Kod
	,cast(max(TwC_Wartosc) as float)Cena,cast(sum (tre_ilosc) as int)Ilosc  
	,cdn.PC_GetTwrUrl(Twr_Kod) as Url ,
isnull(left(atr_wartosc,1),'''') As IlKol
	from cdn.tranag
	join cdn.traelem on tre_gidnumer=trn_gidnumer and trn_gidtyp=TrN_GIDTyp
	JOIN cdn.twrkarty on twr_gidnumer=tre_twrnumer
left join cdn.atrybuty on twr_gidnumer=Atr_ObiNumer  and atr_atkid=24
	left join cdn.TwrCeny on Twr_GIDNumer=twc_twrnumer and TwC_TwrLp=2
	join cdn.magazyny on trn_magznumer=Mag_GIDNumer 
	where TrN_gidnumer={dane.Trn_GidNumer}
	group by TrN_DokumentObcy,    Trn_Gidnumer, Twr_Gidnumer,Mag_GidNumer,twr_ean,
	Twr_Kod,Twr_Nazwa,  Mag_Kod,atr_wartosc
)mm
left join
(
	select TwZ_TwrNumer, cast(sum(twz_ilosc)as int) StanMS 
	from cdn.TwrZasoby
	where TwZ_MagNumer=141
	group by TwZ_TwrNumer
)MS on mm.Twr_GIDNumer= MS.TwZ_TwrNumer  
 order by TrE_GIDLp'";


                var items = await App.TodoManager.PobierzDaneZWeb<DaneMMElem>(sqlPobierzMMki);
                if (!string.IsNullOrWhiteSpace(filtr))
                {
                    
                    foreach (var item in items)
                    {
                        if (item.TrN_DokumentObcy.Contains(filtr.ToUpper()))// || item.Fmm_NrlistuPaczka.Contains(filtr.ToUpper()))
                            Items.Add(item);
                    }

                }
                else
                {
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
