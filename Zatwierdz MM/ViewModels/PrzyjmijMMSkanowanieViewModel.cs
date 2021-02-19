using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Zatwierdz_MM.Models;
using Zatwierdz_MM.Services;

namespace Zatwierdz_MM.ViewModels
{
    public class PrzyjmijMMSkanowanieViewModel : BaseViewModel
    {

        public Command LoadItemsCommand { get; set; }
        public ICommand InsertToBase { get; set; }
        public int Trn_Gidnumer { get; set; }

        private DaneMMElem daneMMElem;
        SQLiteAsyncConnection _connection;
        public ObservableCollection<PC_MsInwentory> Items { get; private set; }

        public PrzyjmijMMSkanowanieViewModel(DaneMMElem daneMMElem)
        {
            Title = daneMMElem.TrN_DokumentObcy;
            Items = new ObservableCollection<PC_MsInwentory>();
            Trn_Gidnumer = daneMMElem.Trn_Gidnumer;
            this.daneMMElem = daneMMElem;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            InsertToBase = new Command<string>(async (string ean) => await ExecInsertToBase(ean));
        }

        private async Task ExecInsertToBase(string ean)
        {
            var twrkarta = await Pobierztwrkod(ean);
            var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var sqlInsert = $@"cdn.PC_WykonajSelect N' 
                         update   cdn.PC_MsInwentory
						 set MsI_TwrIloscSkan+=1 where  MsI_TrnNumer={Trn_Gidnumer}  and MsI_TwrNumer={twrkarta.Twr_Gidnumer}
                        
                        if @@ROWCOUNT =0
                        begin
                             Insert into cdn.PC_MsInwentory values (1604,{Trn_Gidnumer},{twrkarta.Twr_Gidnumer},{daneMMElem.Ilosc},1,''{data}'',11)
                        end'";
           //todo : popraw ilosc 


            var items = await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlInsert);

            NrMMki = "";
            //entry_MM.Focus();
            InsertToBase = new Command<View>((view) =>
            {
                view?.Focus();
            });

            await ExecuteLoadItemsCommand();
        }

        private async  Task ExecuteLoadItemsCommand(string filtr = "")
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();


                var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'select  msi.*,Twr_Gidnumer, Twr_Kod, Twr_Nazwa, Twr_Katalog Twr_Symbol, cast(twc_wartosc as decimal(5,2))Cena 
                , case when len(twr_kod) > 5 and len(twr_url)> 5 
		                then replace(twr_url, substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4),  substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4) + ''Miniatury/'') 
		                else twr_kod end as Url ,Twr_Ean Ean 
                        from cdn.PC_MsInwentory msi
                        join cdn.twrkarty  on twr_gidnumer=msi_twrnumer
                        join cdn.TwrCeny on Twr_GIDNumer = TwC_TwrNumer and TwC_TwrLp = 2 
                        where msi_trnnumer={daneMMElem.Trn_Gidnumer} order by Msi_DataSkan'";


                var items = await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlPobierzMMki);
                if (!string.IsNullOrWhiteSpace(filtr))
                {

                    foreach (var item in items)
                    {
                        var karta = await Pobierztwrkod(daneMMElem.Twr_Gidnumer);
                        //item.DaneMMElem = karta;
                        Items.Add(item);
                        
                    }

                }
                else
                {
                    foreach (var item in items)
                    {
                        var karta = await Pobierztwrkod(item.MsI_TwrNumer);
                        //item.DaneMMElem = karta;
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



        public async Task<DaneMMElem> Pobierztwrkod(string _ean)
        {
            DaneMMElem daneMMElem = new DaneMMElem();

            if (!string.IsNullOrEmpty(_ean) || _ean != "2010000")
                try
                {


                    var Webquery = $@"cdn.PC_WykonajSelect N'Select Twr_Gidnumer, Twr_Kod, Twr_Nazwa, Twr_Katalog Twr_Symbol, cast(twc_wartosc as decimal(5,2))Cena ,cast(sum(TwZ_Ilosc) as int)Ilosc, case when len(twr_kod) > 5 and len(twr_url)> 5 
		                then replace(twr_url, substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4),  substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4) + ''Miniatury/'') 
		                else twr_kod end as Url ,Twr_Ean Ean 
		                from cdn.TwrKarty 
		                join cdn.TwrCeny on Twr_GIDNumer = TwC_TwrNumer and TwC_TwrLp = 2 
		                left join cdn.TwrZasoby on Twr_GIDNumer = TwZ_TwrNumer 
                        where twr_ean=''{_ean}'' or twr_kod=''{_ean}''
		                group by Twr_Gidnumer,twr_kod, twr_nazwa, Twr_Katalog,twc_wartosc, twr_url,twr_ean'";



                    var dane = await App.TodoManager.PobierzDaneZWeb<DaneMMElem>(Webquery);
                    if (dane.Count > 0)
                    {
                        daneMMElem = dane[0];

                    }
                    

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }


             return daneMMElem;

        }

        public async Task<DaneMMElem> Pobierztwrkod(int twrnumer)
        {
            DaneMMElem daneMMElem = new DaneMMElem();

     
                try
                {


                    var Webquery = $@"cdn.PC_WykonajSelect N'Select Twr_Gidnumer, Twr_Kod, Twr_Nazwa, Twr_Katalog Twr_Symbol, cast(twc_wartosc as decimal(5,2))Cena ,cast(sum(TwZ_Ilosc) as int)Ilosc, case when len(twr_kod) > 5 and len(twr_url)> 5 
		                then replace(twr_url, substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4),  substring(twr_url, 1, len(twr_url) - len(twr_kod) - 4) + ''Miniatury/'') 
		                else twr_kod end as Url ,Twr_Ean Ean 
		                from cdn.TwrKarty 
		                join cdn.TwrCeny on Twr_GIDNumer = TwC_TwrNumer and TwC_TwrLp = 2 
		                left join cdn.TwrZasoby on Twr_GIDNumer = TwZ_TwrNumer where Twr_Gidnumer={twrnumer}
		                group by Twr_Gidnumer,twr_kod, twr_nazwa, Twr_Katalog,twc_wartosc, twr_url,twr_ean'";



                    var dane = await App.TodoManager.PobierzDaneZWeb<DaneMMElem>(Webquery);
                    if (dane.Count > 0)
                    {
                        daneMMElem = dane[0];

                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }


            return daneMMElem;

        }
    }
}
