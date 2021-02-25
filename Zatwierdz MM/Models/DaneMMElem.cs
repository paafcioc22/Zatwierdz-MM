using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{
    [XmlType("Table")]
    public class DaneMMElem
    {
        //[PrimaryKey, AutoIncrement]
        //public int Lp { get; set; }
        //[Indexed]
        public int  Trn_Gidnumer{ get; set; } 
        public int  Twr_Gidnumer{ get; set; } 
        public int  Mag_GidNumer { get; set; } 
        public string TrN_DokumentObcy { get; set; }
        public string TrE_GIDLp { get; set; }
        public string Twr_Kod { get; set; }
        public string Twr_Nazwa { get; set; }
        public string Twr_Symbol { get; set; }
        public string Mag_Kod { get; set; }
        public string Opis { get; set; }
        public string Cena { get; set; }
        public string Url { get; set; }
        public string Ean { get; set; }
        public int  Ilosc{ get; set; } 
        public int  Ilosc_Skan{ get; set; } 
 
    }

 
 
}
