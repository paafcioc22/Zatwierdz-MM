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
        public ICommand SearchCommand => new Command(Search);

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
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            FilterList = new Command(async () => await ExecuteLoadItemsCommand("bufor"));
        }


        async public void Search()
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

                var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'	select twr_kod PlaceOpis , PlaceName, sum(cast(placequantity as int)) PlaceQuantity
                            from cdn.pc_mspolozenie a
                            join cdn.TwrKarty on Twr_GIDNumer= placetwrnumer
                            where twr_kod like ''%{filtr}%'' or placename like ''%{filtr}%''
                            group by twr_kod , placeName
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
    }
}
