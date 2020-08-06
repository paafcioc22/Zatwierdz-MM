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
using Zatwierdz_MM.Models;
using Zatwierdz_MM.Services;

namespace Zatwierdz_MM.Droid
{
    public class WebServiceSzacho : IWebService
    {
        public WebSzacho.CDNOffLineSrv client;
        private  List<DaneMM> items;
        DaneMM daneMM;
        string odp;
        public WebServiceSzacho()
        {
            client = new WebSzacho.CDNOffLineSrv();
        }

        public async Task<IEnumerable<DaneMM>> GetItemsAsync(bool forceRefresh = false)
        {
            items = new List<DaneMM>();

            var query = $@"cdn.PC_WykonajSelect N'select * from cdn.pc_zatwierdzonemm '";

            var respone = client.ExecuteSQLCommand(query);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(respone);

            TextReader reader = new StringReader(respone);

            //typeof(ObservableCollection<Allegro>), new XmlRootAttribute("ROOT")
            XmlSerializer serializer = new XmlSerializer(typeof(List<DaneMM>), new XmlRootAttribute("ROOT"));
            List<DaneMM> mm = (List<DaneMM>)serializer.Deserialize(reader);


            foreach (var i in mm)
            {
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
                        daneMM.Trn_NrDokumentu = mm[0].Trn_NrDokumentu;
                        daneMM.Trn_Gidnumer = mm[0].Trn_Gidnumer;
                        daneMM.Trn_Gidtyp = mm[0].Trn_Gidtyp;
                        daneMM.TrN_Stan = mm[0].TrN_Stan;

                        var InsertString = $@"cdn.PC_WykonajSelect N' 
                            if not exists(select *  from cdn.pc_zatwierdzonemm where trn_gidnumer={daneMM.Trn_Gidnumer})
                        begin                            
                        insert into cdn.pc_zatwierdzonemm 
                                            values ({daneMM.Trn_Gidnumer},
                                            {daneMM.Trn_Gidtyp},''{daneMM.Trn_NrDokumentu}'',{daneMM.TrN_Stan},null,null)
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
                catch (Exception)
                {

                    throw;
                }
                


                return daneMM;
            });
        }
    }
}