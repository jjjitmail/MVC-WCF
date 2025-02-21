using System;
using System.Collections.Generic;
using System.Linq;
using Telfort_XPO_Objects;

namespace HttpWebManager
{
    public class ProcesLookupManager : ScrapingBase
    {
        public ProcesLookupManager() : base() { }

        public LookupManager GetLookupManager()
        {
            LookupManager _LookupManager = new LookupManager();

            //List<Lookup_Afgegevenin> _Lookup_AfgegeveninList = new List<Lookup_Afgegevenin>();
            //for (int i = 0; i < 10; i++)
            //{
            //    Lookup_Afgegevenin _Lookup_Afgegevenin = new Lookup_Afgegevenin();
            //    _Lookup_Afgegevenin.Name = "aaa" + i;
            //    _Lookup_AfgegeveninList.Add(_Lookup_Afgegevenin);
            //}
            //_LookupManager.Lookup_AfgegeveninList = _Lookup_AfgegeveninList;

            //List<Lookup_Bedrijfsvorm> _Lookup_BedrijfsvormList = new List<Lookup_Bedrijfsvorm>();
            //for (int i = 0; i < 10; i++)
            //{
            //    Lookup_Bedrijfsvorm _Lookup_Bedrijfsvorm = new Lookup_Bedrijfsvorm();
            //    _Lookup_Bedrijfsvorm.Name = "bbb" + i;
            //    _Lookup_BedrijfsvormList.Add(_Lookup_Bedrijfsvorm);
            //}
            //_LookupManager.Lookup_BedrijfsvormList = _Lookup_BedrijfsvormList;

            return _LookupManager;
        }

        public override void Run()
        {
            //
        }

        private void Execute(Object sender, EventArgs e)
        {
            InitControlsBinding();

            string strRetentionResult = string.Empty;
            DateTime dtEndDate = DateTime.MinValue;
            string ThisMobileNumber = Mobile;
            string SIM = Sim;

            InvokeBezig();

            LookupManager _LookupManager = new LookupManager();

            //List<Lookup_Afgegevenin> _Lookup_AfgegeveninList = new List<Lookup_Afgegevenin>();
            //for (int i = 0; i < 10; i++)
            //{
            //    Lookup_Afgegevenin _Lookup_Afgegevenin = new Lookup_Afgegevenin();
            //    _Lookup_Afgegevenin.Name = "aaa" + i;
            //    _Lookup_AfgegeveninList.Add(_Lookup_Afgegevenin);
            //}
            //_LookupManager.Lookup_AfgegeveninList = _Lookup_AfgegeveninList;

            //List<Lookup_Bedrijfsvorm> _Lookup_BedrijfsvormList = new List<Lookup_Bedrijfsvorm>();
            //for (int i = 0; i < 10; i++)
            //{
            //    Lookup_Bedrijfsvorm _Lookup_Bedrijfsvorm = new Lookup_Bedrijfsvorm();
            //    _Lookup_Bedrijfsvorm.Name = "bbb" + i;
            //    _Lookup_BedrijfsvormList.Add(_Lookup_Bedrijfsvorm);
            //}
            //_LookupManager.Lookup_BedrijfsvormList = _Lookup_BedrijfsvormList;

            SerializationManager<LookupManager> _SLookupManager = new SerializationManager<LookupManager>();
            _SLookupManager.Content = _LookupManager;
            _SLookupManager.Save();

            this.HttpWebResult = new HttpWebResult();
            HttpWebResult.IsSuccess = true;

            InvokeResult();

        }
    }
}
