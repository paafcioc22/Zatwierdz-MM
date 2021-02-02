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
        public ICommand DeleteFromList { get; }
        public  ICommand SearchCommand => new Command(Search);

        private string _filter;

        public ZatwierdzoneMMViewModel()
        {
            Title = "Lista Zeskanowanych";
            Items = new ObservableCollection<DaneMM>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            DeleteFromList = new Command(async () => await UsunZlistCommand());
        }

        private async  Task UsunZlistCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var mmki = new List<string>();
            try
            {
                foreach (var item in Items)
                {
                    Debug.WriteLine($"Usuwam mm {item.Trn_GidNumer}");
                    mmki.Add(item.Trn_NrDokumentu);
                }
                var odp =await Application.Current.MainPage.DisplayActionSheet($"Usunąć poniższe mmki z paczki {Items[0].Fmm_NrlistuPaczka}?", "OK", "Anuluj", mmki.ToArray());
                if (odp == "OK")
                {
                    var czyTylkoJeden = Items.Select(x => x.Fmm_NrlistuPaczka).Distinct().Count();
                    if (czyTylkoJeden==1)
                    {
                        var query = $@" cdn.PC_WykonajSelect ' delete from cdn.PC_ZatwierdzoneMM where Fmm_NrlistuPaczka=''{Items[0].Fmm_NrlistuPaczka}''' ";

                        var usun = await App.TodoManager.PobierzDaneZWeb<DaneMM>(query);
                        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("info", $"Na liście znajduje się więcej niż jeden nr listu", "OK");
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

        public string Filter
        {
            get { return _filter; }
            set
            {
                SetProperty(ref _filter, value);
                Search();
            }
        }

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
                      if(item.Trn_NrDokumentu.Contains(filtr.ToUpper())|| item.Fmm_NrlistuPaczka.Contains(filtr.ToUpper()))
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
