using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{
	[XmlType("Table")]
	public class PC_MsInwentory: DaneMMElem ,INotifyPropertyChanged
	{
        private bool msi_isput;

        public short MsI_TrnTyp { get; set; }
		public int MsI_TrnNumer { get; set; }
		public int MsI_TwrNumer { get; set; }
	
		public short MsI_TwrIloscMM { get; set; }
		public short MsI_TwrIloscSkan { get; set; }
		public short MsI_Rca { get; set; }
		public DateTime Msi_DataSkan { get; set; }
		public short MsI_MagNumer { get; set; }
		 
		//public bool Msi_IsPut { get; set; }
        public bool Msi_IsPut
        {
            get { return msi_isput; }
            set { SetProperty(ref msi_isput, value); }
        }




        protected bool SetProperty<T>(ref T backingStore, T value,
          [CallerMemberName] string propertyName = "",
          Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

         
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public DaneMMElem DaneMMElem { get; set; }
    }
}
