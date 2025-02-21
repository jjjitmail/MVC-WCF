using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NNN.Aas.Telfort.Service.Contracts
{
    [DataContract]
    public class ExecuteContractData
    {
        int ctrContractOid = 0;

        [DataMember]
        public int CtrContractOid
        {
            get
            {
                return ctrContractOid;
            }
            set
            {
                ctrContractOid = value;
            }
        }

    }
}
