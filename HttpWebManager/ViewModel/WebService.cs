using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpWebManager.AAS_TelfortService;
using Telfort_Objects;


namespace HttpWebManager
{
    public class WebService : ScrapingBase
    {
        private void Execute(Object sender, EventArgs e)
        {
            AasTelfortClient _AasTelfortClient = new AasTelfortClient();

            LookupManager _LookupManager = _AasTelfortClient.GetLookupManager();

            SerializationManager<LookupManager> _SLookupManager = new SerializationManager<LookupManager>();
            _SLookupManager.Content = _LookupManager;
            _SLookupManager.Save();

            this.HttpWebResult = new HttpWebResult();
            HttpWebResult.IsSuccess = true;

            InvokeResult();
        }

        public override void Run()
        {
            //
        }
    }
}
