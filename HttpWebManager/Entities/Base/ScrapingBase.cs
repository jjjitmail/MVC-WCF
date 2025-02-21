using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;

namespace HttpWebManager
{
    public abstract class ScrapingBase
    {
        public string Mobile { get; set; }
        public string Sim { get; set; }
        public string RekeningNr { get; set; }
        public string Result { get; set; }
        public object Obj { get; set; }
        public static Telfort_Objects.UserBundelChoice UserBundelChoice { get; set; }
        public Telfort_Objects.Contract DealerContract { get; set; }
        public HttpWebResult HttpWebResult { get; set; }
                
        public event EventHandler<Telfort_Objects.UserBundelChoice> BundelEventHandler;
        public event Func<Telfort_Objects.Contract, Telfort_Objects.UserBundelChoice> BundelEvent;
        public event Func<Telfort_Objects.Contract, HttpWebResult> ContractHttpWebResult;
        
        public ScrapingBase()
        {
            Mobile = Mobile;
            Sim = Sim;
            Result = Result;
            Obj = Obj;
            DealerContract = DealerContract;
        }

        internal void InitContract(string _File)
        {
            SerializationManager<Telfort_Objects.Contract> _c = new SerializationManager<Telfort_Objects.Contract>() { FileName = _File };
            _c.Load();
            DealerContract = _c.Content;
        }
        
        internal void InvokeProcessContract()
        {
            DelegateHttpWebResult _DelegateHttpWebResult = TelfortKlantViewModel.ContractNaarTelfortSturen;
            AsyncCallback Asyncallback = GetResult;
            _DelegateHttpWebResult.BeginInvoke(DealerContract, Asyncallback, this.HttpWebResult);
        }

        internal static void GetResult(IAsyncResult result)
        {
            HttpWebResult state = (HttpWebResult)result.AsyncState;
            AsyncResult delegateResult = (AsyncResult)result;
            DelegateHttpWebResult _DelegateHttpWebResult = (DelegateHttpWebResult)delegateResult.AsyncDelegate;
            _DelegateHttpWebResult.EndInvoke(result);
        }

        public void ExecuteBundelEventHandler(Telfort_Objects.Contract _Contract)
        {
            BundelEventHandler.RaiseSafeEvent<Telfort_Objects.UserBundelChoice>(this, Using<Telfort_Objects.UserBundelChoice>(_Contract));
        }

        public void GetContractBundel()
        {
            if (BundelEvent != null)
                BundelEvent.GetInvocationList().ToList().ForEach(x => x.DynamicInvoke(DealerContract));
        }

        public ScrapingBase(Telfort_Objects.Contract _DealerContract)
        {
            this.DealerContract = _DealerContract;
        }

        public abstract void Run();

        public void InitControlsBinding()
        {
            Obj.BindAllControlsToObject(this, typeof(TextBox));
        }

        public void InvokeBezig()
        {
            if (Obj != null)
            {
                var lblStatus = ControlsManager.GetControls((Control)Obj, typeof(TextBox)).ToList()
                    .Where(x => x.Tag.ToString().ToLower().Equals("result")).FirstOrDefault();
                if (lblStatus != null)
                {
                    ((TextBox)lblStatus).Text = "Bezig...\r\n";
                    Application.DoEvents();
                }
            }
        }
       
        public void InvokeResult()
        {
            if (Obj != null)
            {
                var txtResult = ControlsManager.GetControls((Control)Obj, typeof(TextBox)).ToList()
                    .Where(x => x.Tag.ToString().ToLower().Equals("result")).FirstOrDefault();
                if (txtResult != null)
                    ((TextBox)txtResult).Text += !HttpWebResult.IsSuccess ? Environment.NewLine + HttpWebResult.ErrorMessage : Environment.NewLine + HttpWebResult.Message;
            }
        }

        protected static T Using<T>() where T : class
        {
            return (T)typeof(T).CreateInstance();
        }

        protected static T Using<T>(params object[] _Objects) where T : class
        {
            return (T)typeof(T).CreateInstance(_Objects);
        }
    }
}
