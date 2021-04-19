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
    public class PlacesViewModel:BaseViewModel
    {
        private string _filter;

        public ObservableCollection<Place> Items { get; }
        public Command LoadItemsCommand { get; set; }
        public Command FilterList { get; }
        public ICommand SearchCommand => new Command(async ()=> await Search());

        public string Filter
        {
            get { return _filter; }
            set
            {
                SetProperty(ref _filter, value);
            }
                
        }

        public PlacesViewModel()
        {
            Title = "Miejsca odkładcze";
            Items = new ObservableCollection<Place>();
            LoadItemsCommand = new Command(async () => await Search());
            FilterList = new Command(async () => await ExecuteLoadItemsCommand("bufor"));
        }


        async public Task Search()
        {
            if (string.IsNullOrWhiteSpace(_filter))
            {
                await ExecuteLoadItemsCommand();
            }
            else
            {
                await ExecuteLoadItemsCommand(_filter);
            }

        }

        private async Task ExecuteLoadItemsCommand(string filtr="")
        {
            if (IsBusy)
                return;
            IsBusy = true;
             
            try
            {
                Items.Clear();

                var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'	select twr_kod PlaceOpis , PlaceName, sum(cast(placequantity as int)) PlaceQuantity, PlaceTwrNumer
                            from cdn.pc_mspolozenie a
                            join cdn.TwrKarty on Twr_GIDNumer= placetwrnumer
                            where twr_kod like ''%{filtr}%'' or placename like ''%{filtr}%''
                            group by twr_kod , placeName,PlaceTwrNumer
                            order by 2'";


                var items = await App.TodoManager.PobierzDaneZWeb<Place>(sqlPobierzMMki);


                if (!string.IsNullOrWhiteSpace(filtr))
                {

                    foreach (var i in items)
                    {
                         
                        Items.Add(i);
                    }

                }
                else
                {
                    foreach (var i in items)
                    {
                         
                        Items.Add(i);
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


        


        internal async Task<bool> UpdatePlaceName(Place towar, string place)
        {


            // $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsPolozenie where  PlaceTwrNumer={msi.Twr_Gidnumer} and  PlaceTrnNumer= {msi.MsI_TrnNumer}'";
            bool done = false;

            try
            {
                var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var sqlInsert = $@"cdn.PC_WykonajSelect N'Update cdn.PC_MsPolozenie 
                                    set PlaceName= ''{place}'', PlaceTime=''{data}''
                                                           where  PlaceTwrNumer={towar.PlaceTwrNumer} and PlaceName= ''{towar.PlaceName}''
                            '";

                await App.TodoManager.PobierzDaneZWeb<Place>(sqlInsert);
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





    }
}
