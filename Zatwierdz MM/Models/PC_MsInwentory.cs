using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{
	[XmlType("Table")]
	public class PC_MsInwentory: DaneMMElem
	{
		public short MsI_TrnTyp { get; set; }
		public int MsI_TrnNumer { get; set; }
		public int MsI_TwrNumer { get; set; }
		public short MsI_TwrIloscMM { get; set; }
		public short MsI_TwrIloscSkan { get; set; }
		public DateTime Msi_DataSkan { get; set; }
		public short MsI_MagNumer { get; set; }
		//public DaneMMElem DaneMMElem { get; set; }
	}
}
