using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zatwierdz_MM.Models
{
    public interface IAppVersionProvider
    {
        string AppVersion { get; }
        int BuildVersion { get; }
        Task OpenAppInStore();
        
    }
}
