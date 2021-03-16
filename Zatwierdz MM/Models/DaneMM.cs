using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{

	[XmlType("Table")]
	public class DaneMM
	{
	    public string Trn_GidNumer { get; set; } 
		public string Trn_GidTyp { get; set; } 
		public string Trn_Stan { get; set; }
		public string Trn_NrDokumentu { get; set; }
		public string Trn_DataSkan { get; set; }
		public string DclMagKod { get; set; }
		public string Fmm_NrListu { get; set; }
		public string Fmm_NrlistuPaczka { get; set; }
		public string Trn_Opis{ get; set; }
	}
 
}
