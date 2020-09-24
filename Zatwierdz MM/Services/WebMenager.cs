using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zatwierdz_MM.Models;

namespace Zatwierdz_MM.Services
{
    
    public class WebMenager
    {
        IWebService soapService;
        public WebMenager(IWebService service)
        {
            soapService = service;
        }


        public Task<string> InsertDaneMM(DaneMM query)
        {
            return soapService.InsertMM(query);
        }

        public Task<DaneMM> GetDataFromWeb(string query)
        {
            return soapService.SelectMM(query);
        }


        public Task<IEnumerable<DaneMM>> GetItemsAsync(string filtr="")
        {
            return soapService.GetItemsAsync();
        }
       

    }
        
}
