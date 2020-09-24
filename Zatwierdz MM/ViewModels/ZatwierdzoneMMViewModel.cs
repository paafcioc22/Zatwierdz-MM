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

namespace Zatwierdz_MM.ViewModels
{
    public class ZatwierdzoneMMViewModel : BaseViewModel
    {
        public ObservableCollection<DaneMM> Items { get; private set; }
        public Command LoadItemsCommand { get; set; }
        private string _filter;

        public ZatwierdzoneMMViewModel()
        {
            Title = "Lista Zeskanowanych";
            Items = new ObservableCollection<DaneMM>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                SetProperty(ref _filter, value);
                Search();
            }
        }


        public  ICommand SearchCommand => new Command(Search);
        async public void Search()
        {
            if (string.IsNullOrWhiteSpace(_filter))
            {
                 await ExecuteLoadItemsCommand();
                //OrderList = GetOrders;// ItemData.Items;
            }
            else
            {
                await ExecuteLoadItemsCommand(_filter);
            }

        }

        async Task ExecuteLoadItemsCommand(string filtr ="")
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await App.TodoManager.GetItemsAsync();
                if(!string.IsNullOrWhiteSpace(filtr))
                {
                    //var tmp = items.Where(c => c.Trn_NrDokumentu.ToString().Contains(filtr)).ToList();
                    foreach (var item in items)
                    {
                      if(item.Trn_NrDokumentu.Contains(filtr.ToUpper()))
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


        public static ObservableCollection<T> Convert2<T>(IList<T> original)
        {
            return new ObservableCollection<T>(original);
        }
    }
}
