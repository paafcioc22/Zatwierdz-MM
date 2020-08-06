using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{

	[XmlType("Table")]
	public class DaneMM
	{
	    public string Trn_Gidnumer { get; set; } 
		public string Trn_Gidtyp { get; set; } 
		public string TrN_Stan { get; set; }
		public string Trn_NrDokumentu { get; set; }
	}
 
}
