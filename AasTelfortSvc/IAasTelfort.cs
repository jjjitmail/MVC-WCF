using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NNN.Aas.Telfort.Service.Contracts;
using Telfort_Objects;
using HttpWebManager;

namespace NNN.Aas.Telfort.Service
{
    [ServiceContract]
    public interface IAasTelfort
    {
        [OperationContract]
        void ExecuteContract( string loginName, string password, ExecuteContractData data );

        [OperationContract]
        LookupManager GetLookupManager(Authorization _Authorization);

        [OperationContract]
        TelfortBundels GetTelfortBundels(Authorization _Authorization);

        [OperationContract]
        Klant GetKlant(string MobielNr, string SimOfRekeningNr, Authorization _Authorization);

        [OperationContract]
        Network_Operators GetNetworkOperators(Authorization _Authorization);

        [OperationContract]
        HttpWebResult AddContract(Contract _Contract);

    }
}
