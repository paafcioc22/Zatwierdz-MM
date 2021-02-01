using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Zatwierdz_MM.Droid;
using Zatwierdz_MM.Models;
using Zatwierdz_MM.Services;



[assembly: Dependency(typeof(WebServiceSzacho))]
namespace Zatwierdz_MM.Droid
{
    public class WebServiceSzacho : IWebService
    {
        public WebSzacho.CDNOffLineSrv client;
        private List<DaneMM> items;
        DaneMM daneMM;
        string odp;
        public WebServiceSzacho()
        {
            client = new WebSzacho.CDNOffLineSrv();
        }


        public void ShowLong(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }


        public async Task<IEnumerable<DaneMM>> GetItemsAsync(string filtr = "")
        {
            items = new List<DaneMM>();

            var query = $@"cdn.PC_WykonajSelect N'select mm.Trn_GidNumer,mm.Trn_GidTyp,mm.Trn_NrDokumentu,mm.Trn_DataSkan,mm.Trn_DataZatwierdz,mm.Trn_StanMM,trn.TrN_Stan Trn_Stan , mag_kod DclMagKod 
                        from cdn.PC_ZatwierdzoneMM mm
                        join cdn.tranag trn on trn.TrN_GIDNumer=mm.Trn_GidNumer and trn.TrN_GIDTyp=mm.Trn_GidTyp
                        join cdn.Magazyny on trn.TrN_MagDNumer = MAG_GIDNumer
                        order by mm.Trn_DataSkan desc '";

            var respone = client.ExecuteSQLCommand(query);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(respone);

            TextReader reader = new StringReader(respone);

            //typeof(ObservableCollection<Allegro>), new XmlRootAttribute("ROOT")
            XmlSerializer serializer = new XmlSerializer(typeof(List<DaneMM>), new XmlRootAttribute("ROOT"));
            List<DaneMM> mm = (List<DaneMM>)serializer.Deserialize(reader);




            foreach (var i in mm)
            {
                i.Trn_DataSkan = "Data skanowania : " + DateTimeOffset.Parse(i.Trn_DataSkan).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

                switch (i.Trn_Stan)
                {
                    case "1":
                        i.Trn_Stan = "Bufor";
                        break;

                    case "2":
                        i.Trn_Stan = "Bufor";
                        break;

                    case "5":
                        i.Trn_Stan = "Zatwierdzona";
                        break;

                    default:
                        break;
                }

                items.Add(i);
            }

            return await Task.FromResult(items);
        }

        public async Task<string> InsertMM(DaneMM item)
        {
            try
            {
                return await Task.Run(() =>
                {

                    var InsertString = $@"cdn.PC_InsertOrderNag                                 ";

                    var respone = client.ExecuteSQLCommand(InsertString);

                    odp = respone;
                    odp = odp.Replace("<ROOT>\r\n  <Table>\r\n    <statuss>", "").Replace("</statuss>\r\n  </Table>\r\n</ROOT>", "");

                    return odp;
                });
            }
            catch (Exception s)
            {

                odp = s.Message;
                return odp;
            }
        }

        public async Task<DaneMM> SelectMM(string query)
        {
            return await Task.Run(() =>
            {
                daneMM = new DaneMM();

                var respone = client.ExecuteSQLCommand(query);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(respone);

                TextReader reader = new StringReader(respone);

                //typeof(ObservableCollection<Allegro>), new XmlRootAttribute("ROOT")
                XmlSerializer serializer = new XmlSerializer(typeof(List<DaneMM>), new XmlRootAttribute("ROOT"));
                List<DaneMM> mm = (List<DaneMM>)serializer.Deserialize(reader);



                try
                {
                    if (mm.Count > 0)
                    {
                        if (mm[0].Trn_GidNumer != null)
                        {

                            daneMM = mm[0];
                            if (daneMM.Trn_Stan == "5")
                            {
                                daneMM.Trn_NrDokumentu = "zatwierdzona";
                            }
                            else
                            {
                                var czas = DateTime.Now.ToLocalTime();//.ToString("yyyy-MM-dd hh:mm:ss.000");


                                //.ToString("yyyy-MM-dd HH:mm:ss")

                                var InsertString = $@"cdn.PC_WykonajSelect N' 
                                if not exists(select *  from cdn.pc_zatwierdzonemm where trn_gidnumer={daneMM.Trn_GidNumer})
                            begin                            
                            insert into cdn.pc_zatwierdzonemm
                                                values ({daneMM.Trn_GidNumer},
                                                {daneMM.Trn_GidTyp},''{daneMM.Trn_NrDokumentu}'',{daneMM.Trn_Stan},''{czas.ToString("yyyy-MM-dd HH:mm:ss")}'',null,0,''{daneMM.Fmm_NrListu}'',''{daneMM.Fmm_NrlistuPaczka}'')
                                                select ''OK'' as statuss

                            end else
                            begin
                                select ''not'' as statuss
                            end'
                            ";

                                respone = client.ExecuteSQLCommand(InsertString);
                                respone = respone.Replace("<ROOT>\r\n  <Table>\r\n    <statuss>", "").Replace("</statuss>\r\n  </Table>\r\n</ROOT>", "");

                                if (respone == "not")
                                    daneMM.Trn_NrDokumentu = "not";
                            }

                        }
                    }

                }
                catch (Exception)
                {

                    throw;
                }



                return daneMM;
            });
        }
        public async Task<IList<T>> PobierzDaneZWeb<T>(string query)
        {

            try
            {
                return await Task.Run(() =>
                  {
                      var respone = client.ExecuteSQLCommand(query);

                      return DeserializeFromXml<T>(respone);
                  });
            }
            catch (Exception)
            {

                throw;
            }
        }





        public static IList<T> DeserializeFromXml<T>(string xml)
        {
            List<T> result;
            //Type type = result.GetType();

            XmlSerializer ser = new XmlSerializer(typeof(List<T>), new XmlRootAttribute("ROOT"));
            using (TextReader tr = new StringReader(xml))
            {
                result = (List<T>)ser.Deserialize(tr);
            }
            return result;
        }


    }
}