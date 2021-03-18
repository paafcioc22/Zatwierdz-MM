using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.ViewModels
{
    public class PrzyjmijMMRaportRcaViewModel : BaseViewModel
    {
        public ObservableCollection<PC_MsInwentory> Items { get; private set; }

        private PrzyjmijMMSkanowanieViewModel viewModel;

        public Command LoadRaport { get; set; }
        List<DaneMMElem> daneMMs;

        public PrzyjmijMMRaportRcaViewModel(PrzyjmijMMSkanowanieViewModel viewModel)
        {

            Items = new ObservableCollection<PC_MsInwentory>();
            var gidnr = viewModel.Trn_Gidnumer;
            Title = viewModel.Title;
            this.viewModel = viewModel;
            LoadRaport = new Command(async () => await ExecuteLoadItemsCommand(gidnr));
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
 ,replace(twr_url,Twr_Kod+''.JPG'',''Miniatury/''+Twr_kod+''.JPG'') Url
                         from
                         (
	                         select typ, MsI_TwrNumer,isnull(cast(ilosc as int),0)ilosc,msi_trnnumer ,  MsI_MagNumer, twr_kod, twr_url
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

            foreach (var s in items)
            {
                //s.Url= viewModel.Items.
                //foreach (var x in viewModel.Items)
                //{
                //    if (s.Twr_Kod.Equals(x.Twr_Kod))
                //    {
                //        s.Url = x.Url;
                //    }
                //}

                    
                Items.Add(s);
            }
        }


        private async Task<int> IsRaportExists(  int TrnGidnumer   )
        {
           


            var Webquery = $@"cdn.PC_WykonajSelect N'Select * from cdn.PC_MsRaport where    MsR_TrnNumer= {TrnGidnumer} '";
            var dane = await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(Webquery);


            return dane.Count;

        }



        public  async Task<bool> SaveRaportToBase( )
        {
            bool isSaveOk = false;
            var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "Insert into cdn.PC_MsRaport values ";

            int trnNumer = Items[0].MsI_TrnNumer;
            int ilosc = 0;
            int docType = 0;
            string insert = "";

            if (await IsRaportExists(trnNumer) == 0)
            {
                foreach (var i in Items)
                {
                    ilosc = (i.MsI_TwrIloscSkan - i.MsI_TwrIloscMM);
                    docType = ilosc < 0 ? 1603 : 1617;

                    insert += $"({i.MsI_MagNumer},{i.MsI_TrnNumer},{i.MsI_TwrNumer},{Math.Abs(ilosc)},{docType},0,''{data}'',0),";//+ Environment.NewLine;
                }

                sql += insert.Substring(0, insert.Length - 1);


                var sqlInsert = $@"cdn.PC_WykonajSelect N'{sql}  Select * from cdn.PC_MsRaport where MsR_TrnNumer={trnNumer}  '";


                var response = await App.TodoManager.PobierzDaneZWeb<PC_MsRaport>(sqlInsert);

                if (response.Count == Items.Count)
                    isSaveOk = true;
            }
            else
            {
                
            }
                  

            return isSaveOk;
        }
    }
}
