using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.ViewModels
{
    public class SkanujViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand InsertToBase { get; set; }

        //ZXing.Mobile.MobileBarcodeScanningOptions opts;
        //ZXingScannerPage scanPage;
        //ZXingScannerView zxing;

        public SkanujViewModel()
        {
            Title = "Skanuj";
            Items = new ObservableCollection<Item>();
          //  LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            InsertToBase = new Command<string>(async (string query) => await ExecInsertToBase(query));
          
           
        }

         

        async Task ExecInsertToBase(string nrmmki)
        {
            //await Application.Current.MainPage.DisplayAlert("info",$"Dodano do listy {nrmmki}", "OK");
            try
            {

                var select = $@"cdn.PC_WykonajSelect N'
                  
                     	declare @nrdok as varchar(13)  , @rok varchar(4),@seria varchar(4), @string as varchar(33)
					set @string=''{nrmmki}''
					set @string=substring(@string, patindex(''%[/0]%'',@string)+1, 30) 			 
					set @string=substring(@string, patindex(''%[^0]%'',@string), 30) 				 
					set @rok=right(@string,4)			 
					set @nrdok=substring(@string, 1,patindex(''%[/]%'',@string)-1 ) 			 
					set @seria=replace(REPLACE(REPLACE(@string,@nrdok,''''),@rok,''''),''/'','''')
 

				  if (ISNUMERIC(@nrdok)=1 and ISNUMERIC(@rok)=1	 )
				begin

				  select mmp.trn_gidnumer Trn_GidNumer,mmp.TrN_GIDTyp Trn_GidTyp,mmp.Trn_Stan  
                    ,cdn.nazwaobiektu(mmp.trn_gidtyp, mmp.trn_gidnumer,0,2)Trn_NrDokumentu
                  from cdn.tranag mmw   
                  join cdn.tranag mmp on mmp.trn_zwrnumer =mmw.trn_GIDNumer   
                  where 
				  mmw.TrN_GIDTyp = 1603 and 
				  mmw.TrN_TrNNumer=@nrdok and 
				  mmw.TrN_TrNSeria=@seria and
				  mmw.TrN_TrNRok=@rok
				  if @@ROWCOUNT=0 
					begin 
						select ''brak dokumnetu'' statuss
					end	
				end
				else
					select ''błędny ciąg'' statuss'              
        ";

                var mmka = await App.TodoManager.GetDataFromWeb(select);
                if (mmka.Trn_NrDokumentu != "not" && !string.IsNullOrEmpty(mmka.Trn_NrDokumentu) && mmka.Trn_NrDokumentu != "zatwierdzona")
                    DependencyService.Get<Services.IWebService>().ShowLong($"Dodano do listy {mmka.Trn_NrDokumentu}");
                else if (string.IsNullOrEmpty(mmka.Trn_NrDokumentu))
                    await Application.Current.MainPage.DisplayAlert("info", $"Brak dokumentu", "OK");
                else if (mmka.Trn_NrDokumentu == "zatwierdzona")
                    await Application.Current.MainPage.DisplayAlert("info", $"Dokument jest już zatwierdzony", "OK");
                else
                    await Application.Current.MainPage.DisplayAlert("info", $"Nie udał się dodać pozycji", "OK");


                NrMMki = "";
                //entry_MM.Focus();
                InsertToBase = new Command<View>((view) =>
                {
                    view?.Focus();
                });
            }
            catch (Exception s)
            {
                throw;
               
            }
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