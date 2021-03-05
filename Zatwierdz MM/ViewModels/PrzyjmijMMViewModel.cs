using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.ViewModels
{

   

    public class PrzyjmijMMViewModel : BaseViewModel
    {
        public Command LoadItemsCommand { get; set; }
        public ICommand DeleteFromList { get; }
       // public ICommand SearchCommand => new Command(Search);
        public ObservableCollection<DaneMMElem> Items { get; private set; }
        DaneMM dane;
         



        public PrzyjmijMMViewModel(DaneMM daneMM)
        {
            //Title = "Lista Zeskanowanych";
            Items = new ObservableCollection<DaneMMElem>();
            dane = daneMM;
            Title = daneMM.Trn_NrDokumentu;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }



        async Task ExecuteLoadItemsCommand(string filtr = "")
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();


                var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'
   select  TrN_DokumentObcy,TrE_GIDLp,   Trn_Gidnumer, Twr_Gidnumer,Mag_GidNumer,
Twr_Kod,Twr_Nazwa,  Mag_Kod
,cast(max(TwC_Wartosc) as float)Cena,cast( (tre_ilosc) as int)Ilosc,
  replace(twr_url, substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4),
 substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4) + ''Miniatury/'') Url, cast(sum(twz_ilosc)as int) StanMS
from cdn.tranag
join cdn.traelem on tre_gidnumer=trn_gidnumer and trn_gidtyp=TrN_GIDTyp
JOIN cdn.twrkarty on twr_gidnumer=tre_twrnumer
left join cdn.TwrCeny on Twr_GIDNumer=twc_twrnumer and TwC_TwrLp=2
join cdn.magazyny on trn_magznumer=Mag_GIDNumer
left join cdn.TwrZasoby on TwZ_TwrNumer=Twr_GIDNumer and TwZ_MagNumer=141
where TrN_gidnumer={dane.Trn_GidNumer}
group by TrN_DokumentObcy,TrE_GIDLp,   Trn_Gidnumer, Twr_Gidnumer,Mag_GidNumer,
Twr_Kod,Twr_Nazwa,  Mag_Kod,twr_url,tre_ilosc'";


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
