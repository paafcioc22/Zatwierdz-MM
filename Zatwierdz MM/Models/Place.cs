using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace Zatwierdz_MM.Models
{
    [XmlType("Table")]
    public class Place: INotifyPropertyChanged
    {
        private string placeName;

        public int PlaceId { get; set; }
        public string PlaceName 
        {
           
            get {return placeName; } 
            set { SetProperty(ref placeName, value); } 
        }
        public string PlaceOpis { get; set; }
        public int PlaceTwrNumer { get; set; }
        public int PlaceTrnNumer { get; set; }
        public int PlaceMagZrd { get; set; }
        public int PlaceQuantity { get; set; }
        public DateTime PlaceTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


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
         
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
