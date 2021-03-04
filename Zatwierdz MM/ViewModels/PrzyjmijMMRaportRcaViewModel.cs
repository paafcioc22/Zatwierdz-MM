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
                //s.Url= viewModel.Items.GetEnumerator().Current
                Items.Add(s);
            }
        }
    }
}
