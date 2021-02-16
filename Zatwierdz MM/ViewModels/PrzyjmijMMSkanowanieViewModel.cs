using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Zatwierdz_MM.Models;
using Zatwierdz_MM.Services;

namespace Zatwierdz_MM.ViewModels
{
    public class PrzyjmijMMSkanowanieViewModel: BaseViewModel
    {

        public Command LoadItemsCommand { get; set; }
        SQLiteAsyncConnection _connection;

        public PrzyjmijMMSkanowanieViewModel(DaneMMElem daneMMElem)
        {
            Title = daneMMElem.TrN_DokumentObcy;

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        private Task ExecuteLoadItemsCommand()
        {
            throw new NotImplementedException();
        }
    }
}
