using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zatwierdz_MM.Services
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
