using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using Zatwierdz_MM.Models;
using Zatwierdz_MM.Views;

namespace Zatwierdz_MM.ViewModels
{
    public class SkanujViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand InsertToBase { get; set; }

        public SkanujViewModel()
        {
            Title = "Skanuj";
            Items = new ObservableCollection<Item>();
          //  LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            InsertToBase = new Command<string>(async (string query) => await ExecIsartToBase(query));
          
           
    }

         

        async Task ExecIsartToBase(object nrmmki)
        {
            //await Application.Current.MainPage.DisplayAlert("info",$"Dodano do listy {nrmmki}", "OK");

            var select = $@"cdn.PC_WykonajSelect N'
                  declare @nrdok as varchar(33)  
                  set @nrdok=''{nrmmki}''
                  set @nrdok=substring(@nrdok, patindex(''%[/0]%'',@nrdok)+1, 33)  
                  set @nrdok=substring(@nrdok, patindex(''%[^0]%'',@nrdok), 33)  
                  set @nrdok=''mmw-''+@nrdok  
                  select mmp.trn_gidnumer Trn_Gidnumer,mmp.TrN_GIDTyp Trn_Gidtyp,mmp.TrN_Stan  
                    ,cdn.nazwaobiektu(mmp.trn_gidtyp, mmp.trn_gidnumer,0,2)Trn_NrDokumentu
                    from cdn.tranag mmw   
                  join cdn.tranag mmp on mmp.trn_zwrnumer =mmw.trn_GIDNumer   
                  where mmw.TrN_GIDTyp = 1603  and mmw.TrN_DokumentObcy like   @nrdok'              
        ";

            var mmka = await App.TodoManager.GetDataFromWeb(select);
            if(mmka.Trn_NrDokumentu!="not")
                await Application.Current.MainPage.DisplayAlert("info",$"Dodano do listy {mmka.Trn_NrDokumentu}", "OK");
            else
                await Application.Current.MainPage.DisplayAlert("info", $"Nie udał się dodać pozycji", "OK");


            NrMMki = "";
            //InsertToBase = new Command<View>((view) =>
            //{
            //    view?.Focus();
            //} ) ;
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                 
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