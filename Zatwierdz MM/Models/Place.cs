using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{
    [XmlType("Table")]
    public class Place
    {
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string PlaceOpis { get; set; }
        public int PlaceTwrNumer { get; set; }
        public int PlaceTrnNumer { get; set; }
        public int PlaceMagZrd { get; set; }
        public int PlaceQuantity { get; set; }
        public DateTime PlaceTime { get; set; }
    }
}
