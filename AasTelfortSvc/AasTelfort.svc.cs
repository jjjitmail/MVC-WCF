using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using NNN.Aas.Telfort.Service.Contracts;
using NNN.Aas.Telfort;
using Telfort_Objects;
using HttpWebManager;

namespace NNN.Aas.Telfort.Service
{
    public class AasTelfort : IAasTelfort
    {
        private string _ipClient()
        {
            // ToDo: Spoof detection.
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[ RemoteEndpointMessageProperty.Name ] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;

            // Do some mapping
            if ( string.Equals( ip, "::1", StringComparison.InvariantCultureIgnoreCase ) )
            {
                // This IP address is retrieved when we develop on the VS ASP.NET Development Server. 
                // Map it to a localhost IP address.
                ip = "127.0.0.1";
            }
            return ip;
        }

        public void ExecuteContract( string loginName, string password, ExecuteContractData data )
        {
            AasTelfortAppCredentials credentials = new AasTelfortAppCredentials()
            {
                LoginName = loginName,
                Password = password,
                IpAddress = this._ipClient()
            };
            AasTelfortApp app = new AasTelfortApp( credentials  );
            app.ExecuteContract( data.CtrContractOid );
        }

        //public Klant IsVerlengbaar(string MobielNr, string SimOfRekeningNr, Authorization _Authorization)
        //{
        //    return new WebServiceController<Klant, IsVerlengbaarViewModel>().Get(MobielNr, SimOfRekeningNr);
        //}

        public LookupManager GetLookupManager(Authorization _Authorization)
        {
            return new WebServiceController<LookupManager>().Execute();
        }

        public TelfortBundels GetTelfortBundels(Authorization _Authorization)
        {
            return new WebServiceController<TelfortBundels>().Execute();
        }

        public Klant GetKlant(string MobielNr, string SimOfRekeningNr, Authorization _Authorization)
        {
            return new WebServiceController<Klant, TelfortKlantViewModel>().Get(MobielNr, SimOfRekeningNr);
        }

        public Network_Operators GetNetworkOperators(Authorization _Authorization)
        {
            return new WebServiceController<Network_Operators>().Execute();
        }

        public HttpWebResult AddContract(Telfort_Objects.Contract _Contract)
        {
            return new WebServiceController<HttpWebResult, TelfortKlantViewModel>().Set(_Contract);
        }

    }
}
