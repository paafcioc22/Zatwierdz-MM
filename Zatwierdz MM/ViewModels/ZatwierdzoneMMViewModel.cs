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
        public  ICommand SearchCommand => new Command(async ()=>await Search());

        private string _filter;

        public ZatwierdzoneMMViewModel()
        {
            Title = "Lista Zeskanowanych";
            Items = new ObservableCollection<DaneMM>();
          //  LoadItemsCommand = new Command<string>(async (string filtrr) => await ExecuteLoadItemsCommand(filtrr));
            LoadItemsCommand = new Command(async () => await Search());
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

                        //todo: !!!Popraw to 
                        var query = $@" cdn.PC_WykonajSelect ' select * from cdn.PC_ZatwierdzoneMM where Fmm_NrlistuPaczka=''{Items[0].Fmm_NrlistuPaczka}''' ";
                        //var query = $@" cdn.PC_WykonajSelect ' delete from cdn.PC_ZatwierdzoneMM where Fmm_NrlistuPaczka=''{Items[0].Fmm_NrlistuPaczka}''' ";


                        var usun = await App.TodoManager.PobierzDaneZWeb<DaneMM>(query);
                        //LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
                        await Application.Current.MainPage.DisplayAlert("info", $"Wpisy usunięte", "OK");
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

        public  string Filter
        {
            get { return _filter; }
            set
            {
                SetProperty(ref _filter, value);
                if(string.IsNullOrEmpty(_filter))
                Search();
            }
        }

        async public Task Search()
        {
            if (string.IsNullOrWhiteSpace(_filter))
            {
                 await ExecuteLoadItemsCommand();
                    

                //OrderList = GetOrders;// ItemData.Items;
            }
            else
            {
                
                //var nowa = daneMMs.Where(s => s.Trn_NrDokumentu.Contains(_filter.ToUpper()) || s.Fmm_NrlistuPaczka.Contains(_filter.ToUpper())).ToList();

                //Filtruj(nowa);
                
                //item.Trn_NrDokumentu.Contains(filtr.ToUpper()) || item.Fmm_NrlistuPaczka.Contains(filtr.ToUpper()
                await ExecuteLoadItemsCommand(_filter);
            }

        }

        private void Filtruj(IEnumerable<DaneMM> nowa)
        {
            Items.Clear();
            foreach (var i in nowa)
            {
                Items.Add(i);
            }
        }

        static ObservableCollection<DaneMM> daneMMs = new ObservableCollection<DaneMM>();

        async Task ExecuteLoadItemsCommand(string filtr ="")
        {
            if (IsBusy)
                return;
            IsBusy = true;


            if (filtr.Contains("MM/"))
            {
                filtr = filtr.Replace("MM/", "");

                var tmp = filtr.Split('/');
                int intt = Convert.ToInt32(tmp[0]);
                filtr = $@"{intt}/{tmp[1]}/{tmp[2]}";
            }

            // var daneMMs = await App.TodoManager.GetItemsAsync();
            try
            {
                Items.Clear();
             
                var items = await App.TodoManager.GetItemsAsync(filtr);
                if(!string.IsNullOrWhiteSpace(filtr))
                {

                    foreach (var item in items)
                    {
                        if (item.Trn_NrDokumentu.Contains(filtr.ToUpper()) || item.Fmm_NrlistuPaczka.Contains(filtr.ToUpper()))
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
            daneMMs = Items;
        }


        public static ObservableCollection<T> Convert2<T>(IList<T> original)
        {
            return new ObservableCollection<T>(original);
        }
    }
}
