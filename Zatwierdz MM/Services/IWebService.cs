using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.Services
{
    public interface IWebService
    {
        Task<string> InsertMM(DaneMM item); 
        Task<DaneMM> SelectMM(string query);

        Task<IEnumerable<DaneMM>> GetItemsAsync(bool forceRefresh = false);

    }
}
