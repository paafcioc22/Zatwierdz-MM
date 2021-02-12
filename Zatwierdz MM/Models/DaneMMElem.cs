using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{
    [XmlType("Table")]
    public class DaneMMElem
    {
        public string TrN_DokumentObcy { get; set; }
        public string TrE_GIDLp { get; set; }
        public string Twr_Kod { get; set; }
        public string Twr_Nazwa { get; set; }
        public string Mag_Kod { get; set; }
        public string Opis { get; set; }
        public string Cena { get; set; }
        public string Url { get; set; }
        public int  Ilosc{ get; set; } 
 
    }

 
 
}
