using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.ViewModels
{
    public class PrzyjmijMMRaportRcaViewModel : BaseViewModel
    {
        public ObservableCollection<PC_MsInwentory> Items { get; private set; }

        public readonly int gidnr;
        private PrzyjmijMMSkanowanieViewModel viewModel;

        public ICommand DeleteRaport { get; }
       
        public Command LoadRaport { get; set; }
        List<DaneMMElem> daneMMs;

        public PrzyjmijMMRaportRcaViewModel(PrzyjmijMMSkanowanieViewModel viewModel)
        {

            Items = new ObservableCollection<PC_MsInwentory>();
            gidnr = viewModel.Trn_Gidnumer;
            Title = viewModel.Title;
            this.viewModel = viewModel;
            LoadRaport = new Command(  () =>   Task.Run(()=> ExecuteLoadItemsCommand(gidnr)));
            DeleteRaport = new Command(async () => await DeleteCommand(gidnr));

        }

        private async Task DeleteCommand(int gidnumer)
        {
            var odp = await Application.Current.MainPage.DisplayAlert($"Pytanie..","Usunąć raport z tej mmki?", "OK", "Anuluj");
            if (odp )
            {
                var deletRaport = $@"cdn.PC_WykonajSelect N'

Delete from cdn.PC_MsRaport where MsR_TrnNumer={gidnumer} 
if @@ROWCOUNT>0
 select MsR_StanDok=1
'";

                var response=await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(deletRaport);

                if(response.Count>0)
                    if(response[0].MsR_StanDok == 1)
                    await Application.Current.MainPage.DisplayAlert($"Info..", "Raport usunięto", "OK");

            }
        }

        private async Task ExecuteLoadItemsCommand(int trn_Gidnumer)
        {
            Items.Clear();
            var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'
                         select 
                         *,  MsI_TwrIloscSkan-MsI_TwrIloscMM as MsI_Rca
                         from
                         (
                         select MsI_TrnNumer ,  MsI_MagNumer,p.MsI_TwrNumer, Twr_Kod,  isnull(mm,0)MsI_TwrIloscMM, isnull(skan,0)MsI_TwrIloscSkan  
 ,twr_url as  Url
                         from
                         (
	                         select typ, MsI_TwrNumer,isnull(cast(ilosc as int),0)ilosc,msi_trnnumer ,  MsI_MagNumer, twr_kod, CDN.PC_GetTwrUrl(twr_kod) as twr_url
	                         from(
			                        select ''skan''typ,msi_twrnumer MsI_TwrNumer, MsI_TwrIloscSkan ilosc , MsI_TrnNumer ,  MsI_MagNumer
			                        from cdn.PC_MsInwentory
			                        where msi_trnnumer={ trn_Gidnumer}
		                          union all
			                        select ''mm''typ, tre_twrnumer,tre_ilosc,tre_gidnumer, TrN_MagZNumer
			                        from cdn.TraElem
			                        join cdn.TraNag on TrE_GIDTyp=TrN_GIDTyp and TrE_GIDNumer=TrN_GIDNumer
			                        where tre_gidnumer={trn_Gidnumer}
	                          )a
	                          join cdn.twrkarty on a.MsI_TwrNumer=Twr_GIDNumer
                          )d
                          pivot( sum(ilosc) for typ in ([skan],[mm]))as p
                          )a
                          where MsI_TwrIloscSkan-MsI_TwrIloscMM <>0
                            '";

            var items = await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlPobierzMMki);
            //var items = Task.Run(()=>  App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlPobierzMMki)).Result;

            


            foreach (var s in items)
            {
                     
                Items.Add(s);
            }
             
        }


        internal async Task<ObservableCollection<PC_MsInwentory>> GetPC_MsInwentories(int trn_Gidnumer)
        {
            Items.Clear();
            var sqlPobierzMMki = $@"cdn.PC_WykonajSelect N'
                         select 
                         *,  MsI_TwrIloscSkan-MsI_TwrIloscMM as MsI_Rca
                         from
                         (
                         select MsI_TrnNumer ,  MsI_MagNumer,p.MsI_TwrNumer, Twr_Kod,  isnull(mm,0)MsI_TwrIloscMM, isnull(skan,0)MsI_TwrIloscSkan  
 ,twr_url as Url
                         from
                         (
	                         select typ, MsI_TwrNumer,isnull(cast(ilosc as int),0)ilosc,msi_trnnumer ,  MsI_MagNumer, twr_kod,cdn.PC_GetTwrUrl(Twr_Kod) as twr_url
	                         from(
			                        select ''skan''typ,msi_twrnumer MsI_TwrNumer, MsI_TwrIloscSkan ilosc , MsI_TrnNumer ,  MsI_MagNumer
			                        from cdn.PC_MsInwentory
			                        where msi_trnnumer={ trn_Gidnumer}
		                          union all
			                        select ''mm''typ, tre_twrnumer,tre_ilosc,tre_gidnumer, TrN_MagZNumer
			                        from cdn.TraElem
			                        join cdn.TraNag on TrE_GIDTyp=TrN_GIDTyp and TrE_GIDNumer=TrN_GIDNumer
			                        where tre_gidnumer={trn_Gidnumer}
	                          )a
	                          join cdn.twrkarty on a.MsI_TwrNumer=Twr_GIDNumer
                          )d
                          pivot( sum(ilosc) for typ in ([skan],[mm]))as p
                          )a
                          where MsI_TwrIloscSkan-MsI_TwrIloscMM <>0
                            '";

            var items = await App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlPobierzMMki);
            //var items = Task.Run(()=>  App.TodoManager.PobierzDaneZWeb<PC_MsInwentory>(sqlPobierzMMki)).Result;




            foreach (var s in items)
            {

                Items.Add(s);
            }

            return Items;
        }




        private async Task<int> IsRaportExists(  int TrnGidnumer   )
        {
           


            var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsRaport where    MsR_TrnNumer= {TrnGidnumer} '";
            var dane = await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(Webquery);


            return dane.Count;

        }



        public  async Task<bool> SaveRaportToBase(string opis )
        {
            bool isSaveOk = false;
            var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "Insert into cdn.PC_MsRaport values ";

            int trnNumer = viewModel.Trn_Gidnumer;// Items[0].MsI_TrnNumer;
           // int magNumer = viewModel.
            int ilosc = 0;
            int docType = 0;
            string insert = "";

            if (await IsRaportExists(trnNumer) == 0)
            {

                if (Items.Count > 0)
                {
                    foreach (var i in Items)
                    {
                        ilosc = (i.MsI_TwrIloscSkan - i.MsI_TwrIloscMM);
                        docType = ilosc < 0 ? 1603 : 1617;

                        insert += $"({i.MsI_MagNumer},{i.MsI_TrnNumer},{i.MsI_TwrNumer},{Math.Abs(ilosc)},{docType},0,''{data}'',0,''{opis}''),";//+ Environment.NewLine;
                    }

                    sql += insert.Substring(0, insert.Length - 1);


                    var sqlInsert = $@"cdn.PC_WykonajSelect N'{sql}  Select * from cdn.PC_MsRaport where MsR_TrnNumer={trnNumer}  '";


                    var response = await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(sqlInsert);

                    if (response.Count == Items.Count)
                        isSaveOk = true;
                }
                else
                {
                    sql = $"Insert into cdn.PC_MsRaport values (0,{trnNumer},0,0,0,0,''{ data}'',0,''{opis}'')";
                    insert = $@"cdn.PC_WykonajSelect N'{sql}  Select * from cdn.PC_MsRaport where MsR_TrnNumer={trnNumer}  '";

                    var response = await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(insert);
                    if (response.Count == 1)
                        isSaveOk = true;
                }
                
            }
            else
            {

            }
                  

            return isSaveOk;
        }
    }
}
