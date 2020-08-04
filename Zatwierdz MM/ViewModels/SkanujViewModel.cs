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
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            InsertToBase = new Command<string>(async (string query) => await ExecIsartToBase(query));

        }

       

        async Task ExecIsartToBase(object nrmmki)
        {
            await Application.Current.MainPage.DisplayAlert("info",$"Dodano do listy {nrmmki}", "OK");
             

        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
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
    }
}