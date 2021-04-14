using System;
using System.Collections.Generic;
using System.Text;

namespace Zatwierdz_MM.Models
{
    public enum MenuItemType
    {
        Skanuj,
        Lista,
        ListaNieSkanowanych,
        Mijesca,
        About,
        Ustawienia,
        PrzyjmijMM,
        FotoBrowser
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
