using System;
using System.Collections.Generic;
using System.Text;

namespace Zatwierdz_MM.Extensions
{
    public static class Helpers
    {
        public static string ConvertUrlToOtherSize(string url, string twrkod, OtherSize otherSize, bool onlyFromPresta = false)
        {
            string rtrnUrl = "";
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Contains("http://szachownica.com"))
                {
                    rtrnUrl = url.Replace("small", otherSize.ToString());
                }
                else
                {
                    //replace(twr_url,replace(twr_kod,'/','-')+'.JPG','')+'Miniatury/'+replace(twr_kod,'/','-')+'.JPG' as Twr_Url_Small
                    if (onlyFromPresta)
                    {
                        return url;
                    }
                    else
                    {
                        rtrnUrl = url.Replace(twrkod.Replace('/', '-') + ".JPG", "") + string.Concat("Miniatury/", twrkod.Replace('/', '-'), ".JPG");
                    }
                }
            }

            return rtrnUrl;
        }

        public enum OtherSize
        {
            large,
            home,
            small
        }
    }
}
