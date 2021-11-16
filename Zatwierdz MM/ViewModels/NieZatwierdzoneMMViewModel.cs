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
    public class NieZatwierdzoneMMViewModel : BaseViewModel
    {
        public ObservableCollection<DaneMM> Items { get; private set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand FilterList { get; }
        public  ICommand SearchCommand => new Command(Search);

        private string _filter;

        public NieZatwierdzoneMMViewModel()
        {
            Title = "Lista Nie Zeskanowanych";
            Items = new ObservableCollection<DaneMM>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            FilterList = new Command(async () => await ExecuteLoadItemsCommand("bufor"));
     
        }

         

        public string Filter
        {
            get { return _filter; }
            set
            {
                SetProperty(ref _filter, value);
                if(string.IsNullOrEmpty(_filter))
                Search();
            }
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

            string ff = "";

            if (filtr.Contains("MM/"))
            {
                filtr = filtr.Replace("MM/", "");

                var tmp = filtr.Split('/');
                int intt = Convert.ToInt32(tmp[0]);
                filtr = $@"{intt}/{tmp[1]}/{tmp[2]}";

                ff = $"and t.TrN_DokumentObcy  like ''%{filtr}%'' and t.trn_trnnumer={intt} ";
                filtr = "";
            }
            else
            if (filtr.Contains("bufor"))
            {
                filtr = $" and mmp.trn_stan in (1,2)";

            }
            else
            {
                ff = $"and t.TrN_DokumentObcy  like ''%{filtr}%''";
                filtr = "";
            }

            // var daneMMs = await App.TodoManager.GetItemsAsync();
            try
            {
                Items.Clear();

                var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'select 
	        t.Trn_GidNumer,
	        t.Trn_GidTyp,
	        t.TrN_DokumentObcy Trn_NrDokumentu , 
	        cast(dateadd(d,t.trn_data3,''18001228'') as date) Trn_DataSkan,
	        mmp.trn_stan Trn_Stan   
        from cdn.tranag t
            join cdn.tranag mmp on mmp.trn_zwrnumer =t.trn_GIDNumer 
        join cdn.magazyny on mag_gidnumer=t.trn_magdnumer
        where t.trn_magdnumer=141 {filtr} {ff}
        and t.trn_data3 > datediff(d,''18001228'',getdate()-460)
        and t.trn_gidtyp=1603
        and not exists (select * from cdn.pc_zatwierdzonemm where mmp.trn_gidnumer=trn_gidnumer)
        order by 1'";


                var items = await App.TodoManager.PobierzDaneZWeb<DaneMM>(sqlPobierzMMki);  

            
                if(!string.IsNullOrWhiteSpace(filtr))
                {

                    foreach (var i in items)
                    {
                        
                            i.Trn_DataSkan = "Data dokumentu : " + DateTimeOffset.Parse(i.Trn_DataSkan).ToLocalTime().ToString("yyyy-MM-dd ");

                            switch (i.Trn_Stan)
                            {
                                case "1":
                                    i.Trn_Stan = "Bufor";
                                    break;

                                case "2":
                                    i.Trn_Stan = "Bufor";
                                    break;

                                case "5":
                                    i.Trn_Stan = "Zatwierdzona";
                                    break;

                                default:
                                    break;
                            }
                        
                            Items.Add(i);
                    }

                }
                else
                {
                    foreach (var i in items)
                    {
                        i.Trn_DataSkan = "Data dokumentu : " + DateTimeOffset.Parse(i.Trn_DataSkan).ToLocalTime().ToString("yyyy-MM-dd");

                        switch (i.Trn_Stan)
                        {
                            case "1":
                                i.Trn_Stan = "Bufor";
                                break;

                            case "2":
                                i.Trn_Stan = "Bufor";
                                break;

                            case "5":
                                i.Trn_Stan = "Zatwierdzona";
                                break;

                            default:
                                break;
                        }
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
            daneMMs = Items;
        }


        public static ObservableCollection<T> Convert2<T>(IList<T> original)
        {
            return new ObservableCollection<T>(original);
        }
    }
}
