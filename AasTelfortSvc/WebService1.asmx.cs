using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Telfort_Objects;
using HttpWebManager;

namespace NNN.Aas.Telfort.Service
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        [WebMethod]
        public LookupManager GetLookupManager()
        {
            return new WebServiceController<LookupManager>().Execute();
        }
        [WebMethod]
        public TelfortBundels GetTelfortBundels()
        {
            return new WebServiceController<TelfortBundels>().Execute();
        }
        [WebMethod]
        public Network_Operators GetNetworkOperators()
        {
            return new WebServiceController<Network_Operators>().Execute();
        }
        [WebMethod]
        public Klant GetKlant(string MobielNr, string SimOfRekeningNr)
        {
            return new WebServiceController<Klant, TelfortKlantViewModel>().Get(MobielNr, SimOfRekeningNr);
        }
        [WebMethod]
        public HttpWebResult AddContract()
        {
            Telfort_Objects.Contract _DealerContract = new Telfort_Objects.Contract();
            SerializationManager<Telfort_Objects.Contract> _c = new SerializationManager<Telfort_Objects.Contract>() { FileName = "NieuwAanmelden.xml" };
            _c.Load();
            _DealerContract = _c.Content;

            return new WebServiceController<HttpWebResult, TelfortKlantViewModel>().Set(_DealerContract);
        }
    }
}
