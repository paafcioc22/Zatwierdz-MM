using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        internal string EanSkan;

        private PC_MsInwentory selectItem;

        public PC_MsInwentory SelectItem
        {
            get { return selectItem; } 
            set { SetProperty(ref selectItem, value); }
        }


        public ICommand LoadRaport { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand InsertToBase { get; set; }
        public int Trn_Gidnumer { get; set; }

        
        SQLiteAsyncConnection _connection;
        public ObservableCollection<PC_MsInwentory> Items { get; private set; }
        List<DaneMMElem> daneMMElem;

        public PrzyjmijMMSkanowanieViewModel(List<DaneMMElem> daneMMElem= null)
        {
            if(daneMMElem!=null)
            {
                Title = daneMMElem[0].TrN_DokumentObcy;
                Items = new ObservableCollection<PC_MsInwentory>();
                Trn_Gidnumer = daneMMElem[0].Trn_Gidnumer;
                this.daneMMElem = daneMMElem;
            }
        

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            InsertToBase = new Command<string>(async (string ean) => await ExecInsertToBase(ean));
            LoadRaport = new Command(async () => await ExecuteLoadItems());
        }

        private async Task ExecuteLoadItems( )
        {
            
             
            var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'
                         select 
                         *,  MsI_TwrIloscSkan-MsI_TwrIloscMM as MsI_Rca
                         from
                         (
                         select MsI_TrnNumer ,  MsI_MagNumer,p.MsI_TwrNumer, Twr_Kod, isnull(mm,0)MsI_TwrIloscMM, isnull(skan,0) MsI_TwrIloscSkan  
                         from
                         (
	                         select typ, MsI_TwrNumer,isnull(cast(ilosc as int),0)ilosc,msi_trnnumer ,  MsI_MagNumer, twr_kod, twr_url
	                         from(
			                        select ''skan''typ,msi_twrnumer MsI_TwrNumer, MsI_TwrIloscSkan ilosc , MsI_TrnNumer ,  MsI_MagNumer
			                        from cdn.PC_MsInwentory
			                        where msi_trnnumer={daneMMElem[0].Trn_Gidnumer}
		                          union all
			                        select ''mm''typ, tre_twrnumer,tre_ilosc,tre_gidnumer, TrN_MagZNumer
			                        from cdn.TraElem
			                        join cdn.TraNag on TrE_GIDTyp=TrN_GIDTyp and TrE_GIDNumer=TrN_GIDNumer
			                        where tre_gidnumer={daneMMElem[0].Trn_Gidnumer}
	                          )a
	                          join cdn.twrkarty on a.MsI_TwrNumer=Twr_GIDNumer
                          )d
                          pivot( sum(ilosc) for typ in ([skan],[mm]))as p
                          )a
                          where  MsI_TwrIloscSkan-MsI_TwrIloscMM   <>0
                            '";

            var items = await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlPobierzMMki);

             
        }

        internal DaneMMElem twrkarta;
        private async Task ExecInsertToBase(string ean)
        {
            EanSkan = ean;
            twrkarta = await Pobierztwrkod(ean);
            int ilosZmm = 0;
            var quantityFromMM = daneMMElem.Where(x => x.Twr_Gidnumer == twrkarta.Twr_Gidnumer).SingleOrDefault();

            var obj = this.Items.Where(s => s.Twr_Gidnumer == twrkarta.Twr_Gidnumer).FirstOrDefault();

            this.SelectItem = this.Items.Where(s => s.Ean == this.EanSkan).FirstOrDefault();


            if (quantityFromMM != null)
            {
                ilosZmm = quantityFromMM.Ilosc;
            }


            // MyListView.ScrollTo(obj, ScrollToPosition.Center, true);

            await Task.Delay(500);

            if (twrkarta.Twr_Gidnumer!=0)
            {
                var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var sqlInsert = $@"cdn.PC_WykonajSelect N' 
                         update   cdn.PC_MsInwentory
						 set MsI_TwrIloscSkan+=1 where  MsI_TrnNumer={Trn_Gidnumer}  and MsI_TwrNumer={twrkarta.Twr_Gidnumer}
                        
                        if @@ROWCOUNT =0
                        begin
                             Insert into cdn.PC_MsInwentory values (1604,{Trn_Gidnumer},{twrkarta.Twr_Gidnumer},{ilosZmm},1,''{data}'',{this.daneMMElem[0].Mag_GidNumer})
                                if @@ROWCOUNT >0
                                    Select ''insert'' as Twr_Symbol
                        end 
else 
begin 
select ''update'' as Twr_Symbol
end'";
                //todo : popraw ilosc 


                var items = await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlInsert);

                if(items.Count>0)
                {
                    if(items[0].Twr_Symbol== "update")
                    {
                        obj.MsI_TwrIloscSkan += 1;
                    }
                    else
                    {
                        await ExecuteLoadItemsCommand();
                        this.SelectItem = this.Items.Where(s => s.Ean == this.EanSkan).FirstOrDefault();

                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("info", $"Nie znaleziono towaru..", "OK");
            }

            NrMMki = "";
            //entry_MM.Focus();
            InsertToBase = new Command<View>((view) =>
            {
                view?.Focus();
            });

            //await ExecuteLoadItemsCommand();
        }

        internal async Task<bool> IsRaportExists(int TrnGidnumer)
        {

            var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsRaport where    MsR_TrnNumer= {TrnGidnumer} '";
            var dane = await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(Webquery);

            if (dane.Count > 0)
                return true;
            return false;

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
		                else twr_kod end as Url ,Twr_Ean Ean ,
(case when (select count(*) from cdn.pc_mspolozenie where placetwrnumer=Twr_GIDNumer and placetrnnumer={daneMMElem[0].Trn_Gidnumer} )>=1 then 1 else 0 end)Msi_IsPut
                        from cdn.PC_MsInwentory msi
                        join cdn.twrkarty  on twr_gidnumer=msi_twrnumer
                        join cdn.TwrCeny on Twr_GIDNumer = TwC_TwrNumer and TwC_TwrLp = 2 
                        where msi_trnnumer={daneMMElem[0].Trn_Gidnumer} order by Msi_DataSkan'";


                var items = await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlPobierzMMki);
                if (!string.IsNullOrWhiteSpace(filtr))
                {

                    foreach (var item in items)
                    {
                        var karta = await Pobierztwrkod(daneMMElem[0].Twr_Gidnumer);
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

        internal async Task<bool> UpdateModelInPlace(PC_MsInwentory towar, string place)
        {


            // $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={msi.Twr_Gidnumer} and  PlaceTrnNumer= {msi.MsI_TrnNumer}'";
            bool done = false;
            string sqlInsert = "";
                try
                {
                    var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (towar.MsI_TrnNumer == 1)
                    {
                        sqlInsert = $@"cdn.PC_WykonajSelect N'Update cdn.PC_MsPolozenie set PlaceQuantity+= {towar.MsI_TwrIloscSkan}, PlaceTime=''{data}''
                                                               where PlaceTrnNumer={towar.MsI_TrnNumer} and PlaceTwrNumer={towar.MsI_TwrNumer} and PlaceName= ''{place} ''
                                '";
                    }
                    else
                    {
                          sqlInsert = $@"cdn.PC_WykonajSelect N'Update cdn.PC_MsPolozenie set PlaceQuantity= {towar.MsI_TwrIloscSkan}, PlaceTime=''{data}''
                                                               where PlaceTrnNumer={towar.MsI_TrnNumer} and PlaceTwrNumer={towar.MsI_TwrNumer} and PlaceName= ''{place} ''
                                '";
                    }
                    

                    await App.TodoManager.PobierzDaneZWeb<Place>(sqlInsert);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    done= true;
                }


            //var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={msi.Twr_Gidnumer}  '";

            //var dane = await App.TodoManager.PobierzDaneZWeb<Place>(Webquery);

            //return IsAddRow;
            return done;
             
        }

        internal async Task<bool> UpdateModelInInventory(PC_MsInwentory towar)
        {


            // $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={msi.Twr_Gidnumer} and  PlaceTrnNumer= {msi.MsI_TrnNumer}'";
            bool done = false;

            try
            {
                var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var sqlInsert = $@"cdn.PC_WykonajSelect N'Update [CDN].[PC_MsInwentory] set MsI_TwrIloscSkan= {towar.MsI_TwrIloscSkan}
                                                           where MsI_TrnNumer={towar.MsI_TrnNumer} and MsI_TwrNumer={towar.MsI_TwrNumer}  
                            '";

                await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlInsert);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                done = true;
            }


            //var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={msi.Twr_Gidnumer}  '";

            //var dane = await App.TodoManager.PobierzDaneZWeb<Place>(Webquery);

            //return IsAddRow;
            return done;

        }

        //        select(placeName+' - '+  cdn.NazwaObiektu(1604, placetrnnumer,0,2) +' - ' + placeQuantity +'szt')as sss
        //from  cdn.pc_mspolozenie
        //where placetwrnumer=65839

        internal async Task<bool> AddTowarToPlace(PC_MsInwentory msi, string placeName)
        {
            try
            {
                bool IsAddRow = true;
                var IsAddedRow = await IsPlaceExists(msi.MsI_TwrNumer, msi.MsI_TrnNumer, placeName);

                // $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={msi.Twr_Gidnumer} and  PlaceTrnNumer= {msi.MsI_TrnNumer}'";

                if (IsAddedRow.Count != 0)
                {
                    if (msi.MsI_TrnNumer == 1)
                    {
                        await UpdateModelInPlace(msi, placeName);
                        IsAddRow = true;
                    }
                    else
                    {
                        IsAddRow = false;
                    }
                }
                else
                {
                    var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var sqlInsert = $@"cdn.PC_WykonajSelect N'insert into cdn.PC_MsPolozenie values (''{placeName}'','''',{msi.MsI_TwrNumer},{msi.MsI_TrnNumer},{msi.MsI_MagNumer},{msi.MsI_TwrIloscSkan},''{data}'') '";

                    await App.TodoManager.PobierzDaneZWeb<Place>(sqlInsert);
                }

                //var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={msi.Twr_Gidnumer}  '";

                //var dane = await App.TodoManager.PobierzDaneZWeb<Place>(Webquery);

                return IsAddRow;
            }
            catch (Exception ) 
            {

                throw;
            }
        }

        public async Task<bool> IsPlaceEmpty(int TwrGidnumer, int TrnGidnumer, string placname = "")
        {
            //Place polozenie= new Place();  
            bool rt=true;
           
            var filtrPlace = string.IsNullOrEmpty(placname) ? "" : $" and PlaceName =''{ placname}''";


            var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer<>{TwrGidnumer}  {filtrPlace} '";
            var dane = await App.TodoManager.PobierzDaneZWeb<Place>(Webquery);
            if (dane.Count > 0)
                rt = false;
            return rt;

        }


        public async Task<IList<Place>> IsPlaceExists( int TwrGidnumer, int TrnGidnumer,string placname="")
        {
            //Place polozenie= new Place();  

            var filtr = TrnGidnumer == 0?"":$" and PlaceTrnNumer ={ TrnGidnumer}";
            var filtrPlace = string.IsNullOrEmpty(placname) ?"":$" and PlaceName =''{ placname}''";


            var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={TwrGidnumer} {filtr} {filtrPlace} '";
            var dane = await App.TodoManager.PobierzDaneZWeb<Place>(Webquery);

            return dane;

        }
        //   -- , isnull(Mpe_Quantity-LAG(Mpe_Quantity) over (order by mpe_twrnumer, mpe_data),0) PlaceDifrent
        internal async Task<List<string>> GetListaZmianPolozenia(int twrnumer)
        {
            List<string> places = new List<string>();

            try
            {
     
                var sqlInsert = $@"cdn.PC_WykonajSelect N' 
                                    select   
                                      Mpe_Quantity PlaceQuantity
, isnull(Mpe_Quantity-LAG(Mpe_Quantity) over (order by mpe_twrnumer, mpe_data),0) PlaceDifrent
	                                  ,Mpe_Data PlaceTime
                                  FROM CDN.PC_MsPolozenieElem
                                  where mpe_twrnumer={twrnumer}
                            '";

                var lista = await App.TodoManager.PobierzDaneZWeb<Place>(sqlInsert);
                if (lista != null)
                {
                    foreach (var item in lista)
                    {
                        places.Add(string.Concat(item.PlaceTime, " ; ", item.PlaceQuantity, " ; ", item.PlaceDifrent));
                    }
                     
                }
                return places;

            }
            catch (Exception)
            {

                throw;
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


                    var Webquery = $@"cdn.PC_WykonajSelect N'Select Twr_Gidnumer, Twr_Kod, Twr_Nazwa, Twr_Katalog Twr_Symbol, cast(twc_wartosc as decimal(5,2))Cena ,
                        cast(sum(TwZ_Ilosc) as int)Ilosc, case when len(twr_kod) > 5 and len(twr_url)> 5 
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


        public async Task<bool> DeleteFromSkanAndPolozenie(int TrnGidnumer, int twrnumer)
        {
            DaneMMElem daneMMElem = new DaneMMElem();


            try
            {


                var Webquery = $@"cdn.PC_WykonajSelect N' delete from [CDN].[PC_MsInwentory] 
                    where MsI_TrnNumer =  {TrnGidnumer} and MsI_TwrNumer={twrnumer} if @@ROWCOUNT>0
 select MsI_TwrNumer=1'";



                var dane = await App.TodoManager.PobierzDaneZWeb<DaneMMElem>(Webquery);
                if (dane.Count > 0)
                {
                    daneMMElem = dane[0];
                    return true;
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


            return false;

        }


    }
}
