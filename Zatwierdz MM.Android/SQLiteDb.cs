using System; 
using System.IO; 
using SQLite;
using Xamarin.Forms;
using Zatwierdz_MM.Services;

[assembly: Dependency(typeof(Zatwierdz_MM.Droid.SQLiteDb))]
namespace Zatwierdz_MM.Droid
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "MySQLite.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}