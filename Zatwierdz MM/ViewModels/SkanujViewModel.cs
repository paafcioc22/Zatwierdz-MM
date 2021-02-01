using System;
using System.Collections.Generic;
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

         

        public async Task ExecInsertToBase(string nrmmki)
        {
 
             
            try
            {

                if (nrmmki.IndexOf("/") > 0)
                {
                    var select = GetDataMMString(nrmmki);
                     

                    var mmka = await App.TodoManager.GetDataFromWeb(select);
                    if (mmka.Trn_NrDokumentu != "not" && !string.IsNullOrEmpty(mmka.Trn_NrDokumentu) && mmka.Trn_NrDokumentu != "zatwierdzona")
                        DependencyService.Get<Services.IWebService>().ShowLong($"Dodano do listy {mmka.Trn_NrDokumentu}");
                    else if (string.IsNullOrEmpty(mmka.Trn_NrDokumentu))
                        await Application.Current.MainPage.DisplayAlert("info", $"Brak dokumentu", "OK");
                    else if (mmka.Trn_NrDokumentu == "zatwierdzona")
                        await Application.Current.MainPage.DisplayAlert("info", $"Dokument jest już zatwierdzony", "OK");
                    else
                        await Application.Current.MainPage.DisplayAlert("info", $"Nie udał się dodać pozycji- paczka już zeskanowana?", "OK");


                
                }
                else
                {
                    var IsScanBeforeSql = $@"cdn.PC_WykonajSelect N' 
                     select * from cdn.PC_ZatwierdzoneMM
                     where Fmm_NrlistuPaczka=''{nrmmki}''  '";

                    var IsScanBefore = await App.TodoManager.PobierzDaneZWeb<DaneMM>(IsScanBeforeSql);

                    if (IsScanBefore.Count == 0)
                    {
                        string query = $@"cdn.PC_WykonajSelect N' select * from cdn.PC_FedexMM
					    where Fmm_NrPaczki=''{nrmmki}'' order by Fmm_NazwaPaczki'";

                        var fedex = await App.TodoManager.PobierzDaneZWeb<FedexPaczka>(query);

                        if (fedex.Count > 0)
                        {

                            var mmki = new List<string>();

                            foreach (var i in fedex)
                            {
                                var select = GetDataMMString(i.Fmm_NazwaPaczki, i.Fmm_NrListu, i.Fmm_NrPaczki);

                                mmki.Add(i.Fmm_NazwaPaczki);
                                mmki.Sort((s1, s2) => s1.CompareTo(s2));

                                var mmka = await App.TodoManager.GetDataFromWeb(select);
                                if (mmka.Trn_NrDokumentu != "not" && !string.IsNullOrEmpty(mmka.Trn_NrDokumentu) && mmka.Trn_NrDokumentu != "zatwierdzona")
                                    DependencyService.Get<Services.IWebService>().ShowLong($"Dodano do listy {mmka.Trn_NrDokumentu}");
                                else if (string.IsNullOrEmpty(mmka.Trn_NrDokumentu))
                                    await Application.Current.MainPage.DisplayAlert("info", $"Brak dokumentu", "OK");
                                else if (mmka.Trn_NrDokumentu == "zatwierdzona")
                                    await Application.Current.MainPage.DisplayAlert("info", $"Dokument jest już zatwierdzony", "OK");
                                else
                                    await Application.Current.MainPage.DisplayAlert("info", $"Nie udał się dodać pozycji- paczka już zeskanowana?", "OK");
                            }
                            if (fedex.Count > 1)
                                await Application.Current.MainPage.DisplayActionSheet($"Mmki w paczce {fedex[0].Fmm_NrPaczki}:", "OK", null, mmki.ToArray());
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("info", $"Nie znaleziono nr fedex lub nie zawiera MM", "OK");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("info", $"Paczka była już skanowana", "OK");
                    }
                    
                }


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


        string GetDataMMString(string nrmmki, string fmm_nrlist="",string fmm_nrpaczki="")
        {

            var addfedex = string.IsNullOrEmpty(fmm_nrlist) ? "" : $" ,''{fmm_nrlist}'' Fmm_NrListu, ''{fmm_nrpaczki}'' Fmm_NrlistuPaczka ";

            var select = $@"cdn.PC_WykonajSelect N' 
                declare @nrdok as int  , @rok varchar(4)  , @seria varchar(4)  
		        	 
			set @nrdok=PARSENAME(REPLACE(''{nrmmki}'',''/'',''.''), 3) 
			set @seria=PARSENAME(REPLACE(''{nrmmki}'',''/'',''.''), 2) 
			set @rok=PARSENAME(REPLACE(''{nrmmki}'',''/'',''.''), 1) 
 
			if(ISNUMERIC(@seria)=1 )
				begin
					set @rok=PARSENAME(REPLACE(''{nrmmki}'',''/'',''.''), 2)  
					set @seria=PARSENAME(REPLACE(''{nrmmki}'',''/'',''.''), 1) 
				end 

				  if (ISNUMERIC(@nrdok)=1 and ISNUMERIC(@rok)=1	 )
				begin

				  select mmp.trn_gidnumer Trn_GidNumer,mmp.TrN_GIDTyp Trn_GidTyp,mmp.Trn_Stan  
                    ,cdn.nazwaobiektu(mmp.trn_gidtyp, mmp.trn_gidnumer,0,2)Trn_NrDokumentu {addfedex}
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

            return select;
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