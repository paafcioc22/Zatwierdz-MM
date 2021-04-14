using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{


    [XmlType("Table")]
    public class FotoBrowser
    {
        public string Kontrahent { get; set; }
        public string TwgKod { get; set; }
        public string TwrKod { get; set; }
        public string Url { get; set; }
        public string Twr_Ean { get; set; }
    }
}
