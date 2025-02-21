using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Telfort_XPO_Objects;
using DevExpress.Xpo.DB;
using System.Net;
using HttpWebManager;
using System.Configuration;

namespace HttpWebManager
{
    public static class Domain
    {
        internal static void InitDBConnection()
        {
            if (string.IsNullOrEmpty(XpoDefault.Session.ConnectionString))
            {
                XpoDefault.Session.ConnectionString = ConfigurationManager.ConnectionStrings["TelfortConnection"].ToString();

                DevExpress.Xpo.XpoDefault.DataLayer = DevExpress.Xpo.XpoDefault.GetDataLayer(
                                                        XpoDefault.Session.ConnectionString,
                                                        AutoCreateOption.DatabaseAndSchema);
            }
        }
        internal static int CountWords(string text, string word)
        {
            return (text.Length - text.Replace(word, "").Length) / word.Length;
        }

        internal static void InitKlantStatus(string Status)
        {
            Status = Status.Replace(" selected=\"selected\"", "");

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _obj = new XPQuery<Lookup_KlantStatus>(session1).ToList();
                List<Lookup_KlantStatus> _list = new List<Lookup_KlantStatus>();
                
                if (_obj.Count() == 0)
                {
                    string value1 = "";
                    string value2 = "";
                    int x = CountWords(Status, "</option>");
                    for (int i = 0; i < x; i++)
                    {
                        value1 = HttpWebManager.ScrapeHelper.ExtractValue(Status, "<option value=\"", "\">");
                        value2 = HttpWebManager.ScrapeHelper.ExtractValue(Status, "<option value=\"" + value1 + "\">", "</option>");
                        _list.Add(new Lookup_KlantStatus(session1) { Value = value1, Name = value2 });
                        Status = Status.Replace("<option value=\"" + value1 + "\">" + value2 + "</option>", "");
                    }
                    session1.CommitChanges();
                }
            }
        }

        internal static void InitTypeContract()
        {
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _obj = new XPQuery<Lookup_TypeContract>(session1).ToList();
                XPCollection<Lookup_TypeContract> _list = new XPCollection<Lookup_TypeContract>(session1);

                if (_obj != null)
                {                    
                    _list.Add(new Lookup_TypeContract(session1) { Name = Telfort_Objects.ContractType.Nieuw.ToString(), Value = ((int)Telfort_Objects.ContractType.Nieuw).ToString() });
                    _list.Add(new Lookup_TypeContract(session1) { Name = Telfort_Objects.ContractType.Pre2Post.ToString(), Value = ((int)Telfort_Objects.ContractType.Pre2Post).ToString() });
                    _list.Add(new Lookup_TypeContract(session1) { Name = Telfort_Objects.ContractType.Verlengen.ToString(), Value = ((int)Telfort_Objects.ContractType.Verlengen).ToString() });
                }
                session1.CommitChanges();
            }
        }

        internal static void UpdateComboBoxValues<T>(string strResponseData)
        {
            string HTMLString = strResponseData;
            string quotation = "\"";
            if (HTMLString.Contains("value='"))
            {
                quotation = "'";
            }

            HTMLString = HTMLString.Replace(string.Format(" selected={0}selected{0}", quotation), "").Replace("selected", "");
            string value1 = "", tussenTekst = "", value2 = "";

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _obj = new XPQuery<T>(session1).ToList();
                XPCollection<T> _list = new XPCollection<T>(session1);

                XPCollection<T> _XPlist = new XPCollection<T>(session1);
                
                List<T> _dList = Ult.ConvertTList<T>((IEnumerable<object>)_obj);

                while (HTMLString.Contains("</option>"))
                {
                    value1 = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, string.Format("<option value={0}", quotation), quotation);
                    tussenTekst = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, string.Format("<option value={1}{0}{1}", value1, quotation), ">");
                    value2 = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, String.Format("<option value={2}{0}{2}{1}>", value1, tussenTekst, quotation), "</option>");

                    Object _Obj = Activator.CreateInstance(typeof(T), session1);
                    _Obj.GetType().GetProperty("Value").SetValue(_Obj, value1, null);
                    _Obj.GetType().GetProperty("Name").SetValue(_Obj, value2, null);
                    _list.Add((T)Convert.ChangeType(_Obj, typeof(T)));

                    HTMLString = HTMLString.Replace(string.Format("<option value={3}{0}{3}{1}>{2}</option>", value1, tussenTekst, value2, quotation), "");
                }

                if (_XPlist.Count() == 0)
                {                   
                    session1.CommitChanges();
                }
                else
                {
                    //Is collection zelfde?
                    if (_dList.Count == _list.Count)
                    {
                        _dList.ForEach(x =>
                        {
                            var name = _list.Where(q => q.GetType().GetProperty("Name").GetValue(x, null).
                                Equals(x.GetType().GetProperty("Name").GetValue(x, null))).FirstOrDefault();
                            var value = _list.Where(q => q.GetType().GetProperty("Value").GetValue(x, null).
                                Equals(x.GetType().GetProperty("Value").GetValue(x, null))).FirstOrDefault();

                            // geen data gevonden betekent: collection is niet zelfde
                            if (name == null || value == null)
                            {
                                // oud data verwijderen
                                session1.Delete(_XPlist);
                                session1.CommitChanges();

                                // nieuwe data toevoegen
                                _XPlist.AddRange(_list);
                                session1.CommitChanges();

                                return;
                            }
                        });
                    }
                }
            }
        }
       
        internal static void InitComboBoxValues<T>(string HTMLString)
        {
            HTMLString = HTMLString.Replace(" selected=\"selected\"", "").Replace("selected", "");
            string value1 = "", tussenTekst = "", value2 = "";

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _obj = new XPQuery<T>(session1).ToList();
                XPCollection<T> _list = new XPCollection<T>(session1);

                if (_obj.Count() == 0)
                {                    
                    //int x = CountWords(HTMLString, "</option>");
                    while (HTMLString.Contains("</option>"))
                    {                        
                        value1 = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, "<option value=\"", "\"");
                        tussenTekst = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, string.Format("<option value=\"{0}\"", value1), ">");
                        value2 = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, String.Format("<option value=\"{0}\"{1}>", value1, tussenTekst), "</option>");

                        Object _Obj = Activator.CreateInstance(typeof(T), session1);
                        _Obj.GetType().GetProperty("Value").SetValue(_Obj, value1, null);
                        _Obj.GetType().GetProperty("Name").SetValue(_Obj, value2, null);
                        _list.Add((T)Convert.ChangeType(_Obj, typeof(T)));

                        HTMLString = HTMLString.Replace(string.Format("<option value=\"{0}\"{1}>{2}</option>", value1, tussenTekst, value2), "");
                    }
                    session1.CommitChanges();
                }
            }
        }

        internal static void Init_Lookup_TypeProduct(string HTMLString)
        {
            HTMLString = HTMLString.Replace(" selected=\"selected\"", "").Replace("selected", "");
            string tussenTekst = "";

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _obj = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                XPCollection<Lookup_TypeProduct> _list = new XPCollection<Lookup_TypeProduct>(session1);

                if (_obj.Count() == 0)
                {
                    string value1 = "";
                    string value2 = "";
                    int x = CountWords(HTMLString, "</option>");
                    for (int i = 0; i < x; i++)
                    {
                        value1 = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, "<option value=\"", "\"");
                        tussenTekst = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, "<option value=\"" + value1 + "\"", ">");
                        tussenTekst = string.IsNullOrEmpty(tussenTekst) ? "" : tussenTekst;
                        value2 = HttpWebManager.ScrapeHelper.ExtractValue(HTMLString, String.Format("<option value=\"{0}\"{1}>", value1, tussenTekst), "</option>");

                        Lookup_TypeProduct _Lookup_TypeProduct = new Lookup_TypeProduct(session1);
                        _Lookup_TypeProduct.Value = value1;
                        _Lookup_TypeProduct.Name = value2;
                        _list.Add(_Lookup_TypeProduct);

                        HTMLString = HTMLString.Replace("<option value=\"" + value1 + "\"" + tussenTekst + ">" + value2 + "</option>", "");
                    }
                    session1.CommitChanges();
                }
            }
        }

        internal static XPCollection<T> InitComboBoxValues<T>(string SelectHTMLString, UnitOfWork session1)
        {
            string strSIMonly = "";
            string strSIMonlyValue = "";
            string tussenTekst = "";

            XPCollection<T> _list = new XPCollection<T>(session1);

            int x = Domain.CountWords(SelectHTMLString, "</option>");
            for (int i = 0; i < x; i++)
            {
                Object _Obj = Activator.CreateInstance(typeof(T), session1);

                strSIMonlyValue = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, "<option value=\"", "\"");
                tussenTekst = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value=\"{0}\"", strSIMonlyValue), ">");
                tussenTekst = string.IsNullOrEmpty(tussenTekst) ? "" : tussenTekst;
                strSIMonly = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value=\"{0}\"{1}>", strSIMonlyValue, tussenTekst), "</option>");

                _Obj.GetType().GetProperty("Name").SetValue(_Obj, strSIMonly, null);
                _Obj.GetType().GetProperty("Value").SetValue(_Obj, strSIMonlyValue, null);
                _list.Add((T)Convert.ChangeType(_Obj, typeof(T)));

                SelectHTMLString = SelectHTMLString.Replace(string.Format("<option value=\"{0}\"{1}>{2}</option>", strSIMonlyValue, tussenTekst, strSIMonly), "");
            }

            return _list;
        }

        internal static XPCollection<T> ReturnComboBoxValues<T>(string SelectHTMLString)
        {
            string strSIMonly = "";
            string strSIMonlyValue = "";
            string tussenTekst = "";

            XPCollection<T> _list = new XPCollection<T>();

            int x = Domain.CountWords(SelectHTMLString, "</option>");
            for (int i = 0; i < x; i++)
            {
                Object _Obj = Activator.CreateInstance(typeof(T));

                strSIMonlyValue = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, "<option value=\"", "\">");
                tussenTekst = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value=\"{0}\"", strSIMonlyValue), ">");
                tussenTekst = string.IsNullOrEmpty(tussenTekst) ? "" : tussenTekst;
                strSIMonly = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value=\"{0}\"{1}>", strSIMonlyValue, tussenTekst), "</option>");

                _Obj.GetType().GetProperty("Name").SetValue(_Obj, strSIMonly, null);
                _Obj.GetType().GetProperty("Value").SetValue(_Obj, strSIMonlyValue, null);
                _list.Add((T)Convert.ChangeType(_Obj, typeof(T)));

                SelectHTMLString = SelectHTMLString.Replace(string.Format("<option value=\"{0}\"{1}>{2}</option>", strSIMonlyValue, tussenTekst, strSIMonly), "");
            }

            return _list;
        }

        internal static void InitComboBoxValues_Lookup_TypeProduct(string SelectHTMLString, UnitOfWork session1, 
            ref XPCollection<Lookup_TypeProduct> _list)
        {
            string strSIMonly = "";
            string strSIMonlyValue = "";
            string tussenTekst = "";

            int x = Domain.CountWords(SelectHTMLString, "</option>");
            for (int i = 0; i < x; i++)
            {
                Lookup_TypeProduct _Lookup_TypeProduct = new Lookup_TypeProduct(session1);

                strSIMonlyValue = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, "<option value=\"", "\">");
                tussenTekst = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value=\"{0}\"", strSIMonly), ">");
                tussenTekst = string.IsNullOrEmpty(tussenTekst) ? "" : tussenTekst;
                strSIMonly = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value=\"{0}\"{1}>", strSIMonly, tussenTekst), "</option>");
                _Lookup_TypeProduct.Name = strSIMonly;
                _Lookup_TypeProduct.Value = strSIMonlyValue;
                _list.Add(_Lookup_TypeProduct);

                SelectHTMLString = SelectHTMLString.Replace(string.Format("<option value=\"{0}\"{1}>{2}</option>", strSIMonly, tussenTekst, strSIMonlyValue), "");
            }
        }
        
        internal static void InitAansluiting()
        {
            //using (UnitOfWork session1 = new UnitOfWork())
            //{
            //    var _obj = new XPQuery<Lookup_Aansluiting>(session1).ToList();
            //    List<Lookup_Aansluiting> _list = new List<Lookup_Aansluiting>();
            //    Lookup_Aansluiting _Lookup_Aansluiting = new Lookup_Aansluiting(session1);
            //    if (_obj.Count() == 0)
            //    {
            //        _Lookup_Aansluiting.Value = "BB2010";
            //        _Lookup_Aansluiting.Name = "Telfort abonnement";
            //        _list.Add(_Lookup_Aansluiting);

            //        _Lookup_Aansluiting = new Lookup_Aansluiting(session1);
            //        _Lookup_Aansluiting.Value = "SIMOnly2010";
            //        _Lookup_Aansluiting.Name = "Telfort sim only";
            //        _list.Add(_Lookup_Aansluiting);

            //        _Lookup_Aansluiting = new Lookup_Aansluiting(session1);
            //        _Lookup_Aansluiting.Value = "MIU2008";
            //        _Lookup_Aansluiting.Name = "Telfort mobiel internet";
            //        _list.Add(_Lookup_Aansluiting);
                   
            //        session1.CommitChanges();
            //    }
            //}
        }

        // wordt niet gebruikt, kan verwijderd worden
        internal static void InitSubscriptionPlanRenew()
        {
            // Vernieuw - Normal -
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                Lookup_SubscriptionType _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
                _Lookup_SubscriptionType.Name = "Telfort abonnement";
                _Lookup_SubscriptionType.Value = "BB2010";
                _Lookup_SubscriptionType.ParentId = _Search_Lookup_TypeProduct
                    .Where(x => x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())
                        && x.ParentId == _Search_Loopup_SortPlan
                        .Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid
                        && x.Value == "0").First().Oid;
                _list.Add(_Lookup_SubscriptionType);

                _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
                _Lookup_SubscriptionType.Name = "Telfort sim only";
                _Lookup_SubscriptionType.Value = "SIMOnly2010";
                _Lookup_SubscriptionType.ParentId = _Search_Lookup_TypeProduct
                    .Where(x => x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())
                        && x.ParentId == _Search_Loopup_SortPlan
                        .Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid
                        && x.Value == "0").First().Oid;
                _list.Add(_Lookup_SubscriptionType);

                _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
                _Lookup_SubscriptionType.Name = "Telfort mobiel internet";
                _Lookup_SubscriptionType.Value = "MIU2008";
                _Lookup_SubscriptionType.ParentId = _Search_Lookup_TypeProduct
                    .Where(x => x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())
                        && x.ParentId == _Search_Loopup_SortPlan
                        .Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid
                        && x.Value == "0").First().Oid;
                _list.Add(_Lookup_SubscriptionType);

                _Lookup_TypeProductList.Where(x => x.ParentId == _Search_Loopup_SortPlan
                    .Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid
                    && x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())).First().Lookup_SubscriptionTypeList = _list;

                session1.CommitChanges();
            }

            //// Vernieuw - Normal - Telfort abonnement
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_SubscriptionType = new XPQuery<Lookup_SubscriptionType>(session1).ToList();
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                //XPCollection<Lookup_SubscriptionType> _Lookup_SubscriptionTypeList = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                int ParentId = _Search_Lookup_SubscriptionType
                    .Where(x => x.ParentId == _Search_Lookup_TypeProduct
                        .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid).First().Oid
                        && x.Value == "BB2010").First().Oid;

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_Bundel> _blist = new XPCollection<Lookup_Bundel>(session1);

                _blist.Add(new Lookup_Bundel(session1) { Name = "100 minuten", Value = "0001.0101.0001", NameId = "0001.0101", ValueId = "0001.0101.0001", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "150 minuten", Value = "0001.0101.0002", NameId = "0001.0101", ValueId = "0001.0101.0002", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "200 minuten", Value = "0001.0101.0003", NameId = "0001.0101", ValueId = "0001.0101.0003", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "250 minuten", Value = "0001.0101.0004", NameId = "0001.0101", ValueId = "0001.0101.0004", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 minuten", Value = "0001.0101.0005", NameId = "0001.0101", ValueId = "0001.0101.0005", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "400 minuten", Value = "0001.0101.0006", NameId = "0001.0101", ValueId = "0001.0101.0006", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "500 minuten", Value = "0001.0101.0007", NameId = "0001.0101", ValueId = "0001.0101.0007", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "1000 minuten", Value = "0001.0101.0008", NameId = "0001.0101", ValueId = "0001.0101.0008", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "25 sms bundel", Value = "0001.0102.0001", NameId = "0001.0102", ValueId = "0001.0102.0001", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "100 sms bundel", Value = "0001.0102.0002", NameId = "0001.0102", ValueId = "0001.0102.0002", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 sms bundel", Value = "0001.0102.0003", NameId = "0001.0102", ValueId = "0001.0102.0003", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "1000 sms bundel", Value = "0001.0102.0004", NameId = "0001.0102", ValueId = "0001.0102.0004", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 200 MB", Value = "0001.0103.0001", NameId = "0001.0103", ValueId = "0001.0103.0001", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1 GB", Value = "0001.0103.0002", NameId = "0001.0103", ValueId = "0001.0103.0002", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1,5 GB", Value = "0001.0103.0003", NameId = "0001.0103", ValueId = "0001.0103.0003", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });

                _list.Where(x => x.Value == "BB2010" && x.ParentId == _Search_Lookup_TypeProduct
                    .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid).First().Oid).First().Lookup_BundelList = _blist;

                session1.CommitChanges();
            }
            // Vernieuw - Normal - Telfort sim only
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_SubscriptionType = new XPQuery<Lookup_SubscriptionType>(session1).ToList();
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                //XPCollection<Lookup_SubscriptionType> _Lookup_SubscriptionTypeList = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                int ParentId = _Search_Lookup_SubscriptionType
                    .Where(x => x.ParentId == _Search_Lookup_TypeProduct
                        .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid).First().Oid
                        && x.Value == "SIMOnly2010").First().Oid;

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_Bundel> _blist = new XPCollection<Lookup_Bundel>(session1);

                _blist.Add(new Lookup_Bundel(session1) { Name = "75 minuten", Value = "0001.0101.0001", NameId = "0001.0101", ValueId = "0001.0101.0001", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "200 minuten", Value = "0001.0101.0002", NameId = "0001.0101", ValueId = "0001.0101.0002", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 minuten", Value = "0001.0101.0003", NameId = "0001.0101", ValueId = "0001.0101.0003", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "400 minuten", Value = "0001.0101.0004", NameId = "0001.0101", ValueId = "0001.0101.0004", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "500 minuten", Value = "0001.0101.0005", NameId = "0001.0101", ValueId = "0001.0101.0005", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "750 minuten", Value = "0001.0101.0006", NameId = "0001.0101", ValueId = "0001.0101.0006", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "25 sms bundel", Value = "0001.0102.0001", NameId = "0001.0102", ValueId = "0001.0102.0001", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "100 sms bundel", Value = "0001.0102.0002", NameId = "0001.0102", ValueId = "0001.0102.0002", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 sms bundel", Value = "0001.0102.0003", NameId = "0001.0102", ValueId = "0001.0102.0003", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "1000 sms bundel", Value = "0001.0102.0004", NameId = "0001.0102", ValueId = "0001.0102.0004", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 200 MB", Value = "0001.0103.0001", NameId = "0001.0103", ValueId = "0001.0103.0001", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1 GB", Value = "0001.0103.0002", NameId = "0001.0103", ValueId = "0001.0103.0002", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1,5 GB", Value = "0001.0103.0003", NameId = "0001.0103", ValueId = "0001.0103.0003", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });

                _list.Where(x => x.Value == "SIMOnly2010" && x.ParentId == _Search_Lookup_TypeProduct
                    .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid).First().Oid).First().Lookup_BundelList = _blist;

                session1.CommitChanges();
            }
            // Vernieuw - Normal - Telfort mobiel internet
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_SubscriptionType = new XPQuery<Lookup_SubscriptionType>(session1).ToList();
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                //XPCollection<Lookup_SubscriptionType> _Lookup_SubscriptionTypeList = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                int ParentId = _Search_Lookup_SubscriptionType
                    .Where(x => x.ParentId == _Search_Lookup_TypeProduct
                        .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid).First().Oid
                        && x.Value == "MIU2008").First().Oid;

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_Bundel> _blist = new XPCollection<Lookup_Bundel>(session1);

                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 250 MB", Value = "0001.0101.0001", NameId = "0001.0101", ValueId = "0001.0101.0001", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 500 MB", Value = "0001.0101.0002", NameId = "0001.0101", ValueId = "0001.0101.0002", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 2 GB", Value = "0001.0101.0003", NameId = "0001.0101", ValueId = "0001.0101.0003", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 4 GB", Value = "0001.0101.0004", NameId = "0001.0101", ValueId = "0001.0101.0004", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });

                _list.Where(x => x.Value == "MIU2008" && x.ParentId == _Search_Lookup_TypeProduct
                    .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid).First().Oid).First().Lookup_BundelList = _blist;

                session1.CommitChanges();
            }
            //
        }
        // wordt niet gebruikt, kan verwijderd worden
        internal static void InitSubscriptionPlanNew()
        {
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _obj = new XPQuery<SubscriptionPlan>(session1).ToList();
                SubscriptionPlan _SubscriptionPlan = new SubscriptionPlan(session1);

                XPCollection<Loopup_SortPlan> _list = new XPCollection<Loopup_SortPlan>(session1);

                Loopup_SortPlan _Loopup_SortPlan = new Loopup_SortPlan(session1);
                _Loopup_SortPlan.Name = Loopup_SortPlan.PlanType.Nieuw.ToString();
                _list.Add(_Loopup_SortPlan);

                _Loopup_SortPlan = new Loopup_SortPlan(session1);
                _Loopup_SortPlan.Name = Loopup_SortPlan.PlanType.Vernieuwing.ToString();
                _list.Add(_Loopup_SortPlan);

                _SubscriptionPlan.Loopup_SortPlanList = _list;
                session1.CommitChanges();
            }

            // New & Vernieuw
            using (UnitOfWork session1 = new UnitOfWork())
            {                
                XPCollection<Lookup_TypeProduct> _list = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                Lookup_TypeProduct _Lookup_TypeProduct = new Lookup_TypeProduct(session1);
                _Lookup_TypeProduct.Name = Lookup_TypeProduct.ProductKeuze.NORMAL.ToString();
                _Lookup_TypeProduct.Value = "0";
                _Lookup_TypeProduct.ParentId = _Search_Loopup_SortPlan.Where(x => x.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid;
                _list.Add(_Lookup_TypeProduct);

                _Lookup_TypeProduct = new Lookup_TypeProduct(session1);
                _Lookup_TypeProduct.Name = Lookup_TypeProduct.ProductKeuze.SIMONLY.ToString();
                _Lookup_TypeProduct.Value = "1";
                _Lookup_TypeProduct.ParentId = _Search_Loopup_SortPlan.Where(x => x.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid;
                _list.Add(_Lookup_TypeProduct);

                _Loopup_SortPlanList.Where(x => x.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Lookup_TypeProductList = _list;

                XPCollection<Lookup_TypeProduct> _list2 = new XPCollection<Lookup_TypeProduct>(session1);

                _Lookup_TypeProduct = new Lookup_TypeProduct(session1);
                _Lookup_TypeProduct.Name = Lookup_TypeProduct.ProductKeuze.NORMAL.ToString();
                _Lookup_TypeProduct.Value = "0";
                _Lookup_TypeProduct.ParentId = _Search_Loopup_SortPlan.Where(x => x.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid;
                _list2.Add(_Lookup_TypeProduct);

                _Lookup_TypeProduct = new Lookup_TypeProduct(session1);
                _Lookup_TypeProduct.Name = Lookup_TypeProduct.ProductKeuze.SIMONLY.ToString();
                _Lookup_TypeProduct.Value = "1";
                _Lookup_TypeProduct.ParentId = _Search_Loopup_SortPlan.Where(x => x.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Oid;
                _list2.Add(_Lookup_TypeProduct);

                _Loopup_SortPlanList.Where(x => x.Name.Equals(Loopup_SortPlan.PlanType.Vernieuwing.ToString())).First().Lookup_TypeProductList = _list2;

                session1.CommitChanges();
                //
            }

            // New - simonly -
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);

                Lookup_SubscriptionType _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
                _Lookup_SubscriptionType.Name = "Telfort sim only";
                _Lookup_SubscriptionType.Value = "SIMOnly2010";
                int pid = _Search_Loopup_SortPlan.Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid;

                _Lookup_SubscriptionType.ParentId = _Search_Lookup_TypeProduct
                    .Where(x => x.Name.Equals(Lookup_TypeProduct.ProductKeuze.SIMONLY.ToString())
                        && x.ParentId == pid
                        && x.Value == "1" ).First().Oid;
                _list.Add(_Lookup_SubscriptionType);

                _Lookup_TypeProductList.Where(x => x.ParentId == _Search_Loopup_SortPlan
                    .Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid 
                    && x.Name.Equals(Lookup_TypeProduct.ProductKeuze.SIMONLY.ToString())).First().Lookup_SubscriptionTypeList = _list;

                session1.CommitChanges();
            }

            // New - Normal -
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                Lookup_SubscriptionType _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
                _Lookup_SubscriptionType.Name = "Telfort abonnement";
                _Lookup_SubscriptionType.Value = "BB2010";
                _Lookup_SubscriptionType.ParentId = _Search_Lookup_TypeProduct
                    .Where(x => x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())
                        && x.ParentId == _Search_Loopup_SortPlan.Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid
                        && x.Value == "0").First().Oid;
                _list.Add(_Lookup_SubscriptionType);

                _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
                _Lookup_SubscriptionType.Name = "Telfort sim only";
                _Lookup_SubscriptionType.Value = "SIMOnly2010";
                _Lookup_SubscriptionType.ParentId = _Search_Lookup_TypeProduct
                    .Where(x => x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())
                        && x.ParentId == _Search_Loopup_SortPlan.Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid
                        && x.Value == "0").First().Oid;
                _list.Add(_Lookup_SubscriptionType);

                _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
                _Lookup_SubscriptionType.Name = "Telfort mobiel internet";
                _Lookup_SubscriptionType.Value = "MIU2008";
                _Lookup_SubscriptionType.ParentId = _Search_Lookup_TypeProduct
                    .Where(x => x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())
                        && x.ParentId == _Search_Loopup_SortPlan.Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid
                        && x.Value == "0").First().Oid;
                _list.Add(_Lookup_SubscriptionType);

                _Lookup_TypeProductList.Where(x => x.ParentId == _Search_Loopup_SortPlan
                    .Where(y => y.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid
                    && x.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString())).First().Lookup_SubscriptionTypeList = _list;

                session1.CommitChanges();
            }

            // New - simonly - Telfort sim only
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_SubscriptionType = new XPQuery<Lookup_SubscriptionType>(session1).ToList();
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                //XPCollection<Lookup_SubscriptionType> _Lookup_SubscriptionTypeList = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                int ParentId = _Search_Lookup_SubscriptionType
                    .Where(x => x.ParentId == _Search_Lookup_TypeProduct
                        .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.SIMONLY.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid
                        && x.Value == "SIMOnly2010").First().Oid;

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_Bundel> _blist = new XPCollection<Lookup_Bundel>(session1);

                _blist.Add(new Lookup_Bundel(session1) { Name = "75 minuten", Value = "0001.0102.0001", NameId = "0001.0102", ValueId = "0001.0102.0001", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "200 minuten", Value = "0001.0102.0002", NameId = "0001.0102", ValueId = "0001.0102.0002", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 minuten", Value = "0001.0102.0003", NameId = "0001.0102", ValueId = "0001.0102.0003", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "400 minuten", Value = "0001.0102.0004", NameId = "0001.0102", ValueId = "0001.0102.0004", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "500 minuten", Value = "0001.0102.0005", NameId = "0001.0102", ValueId = "0001.0102.0005", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "750 minuten", Value = "0001.0102.0006", NameId = "0001.0102", ValueId = "0001.0102.0006", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "25 sms bundel", Value = "0001.0103.0001", NameId = "0001.0103", ValueId = "0001.0103.0001", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "100 sms bundel", Value = "0001.0103.0002", NameId = "0001.0103", ValueId = "0001.0103.0002", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 sms bundel", Value = "0001.0103.0003", NameId = "0001.0103", ValueId = "0001.0103.0003", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "1000 sms bundel", Value = "0001.0103.0004", NameId = "0001.0103", ValueId = "0001.0103.0004", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 200 MB", Value = "0001.0104.0001", NameId = "0001.0104", ValueId = "0001.0104.0001", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1 GB", Value = "0001.0104.0002", NameId = "0001.0104", ValueId = "0001.0104.0002", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1,5 GB", Value = "0001.0104.0003", NameId = "0001.0104", ValueId = "0001.0104.0003", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });

                _list.Where(x => x.Value == "SIMOnly2010" && x.ParentId == _Search_Lookup_TypeProduct
                    .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.SIMONLY.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid).First().Lookup_BundelList = _blist;

                session1.CommitChanges();
            }

            //// New - Normal - Telfort abonnement
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_SubscriptionType = new XPQuery<Lookup_SubscriptionType>(session1).ToList();
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                //XPCollection<Lookup_SubscriptionType> _Lookup_SubscriptionTypeList = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                int ParentId = _Search_Lookup_SubscriptionType
                    .Where(x => x.ParentId == _Search_Lookup_TypeProduct
                        .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid
                        && x.Value == "BB2010").First().Oid;

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_Bundel> _blist = new XPCollection<Lookup_Bundel>(session1);

                _blist.Add(new Lookup_Bundel(session1) { Name = "100 minuten", Value = "0001.0102.0001", NameId = "0001.0102", ValueId = "0001.0102.0001", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "150 minuten", Value = "0001.0102.0002", NameId = "0001.0102", ValueId = "0001.0102.0002", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "200 minuten", Value = "0001.0102.0003", NameId = "0001.0102", ValueId = "0001.0102.0003", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "250 minuten", Value = "0001.0102.0004", NameId = "0001.0102", ValueId = "0001.0102.0004", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 minuten", Value = "0001.0102.0005", NameId = "0001.0102", ValueId = "0001.0102.0005", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "400 minuten", Value = "0001.0102.0006", NameId = "0001.0102", ValueId = "0001.0102.0006", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "500 minuten", Value = "0001.0102.0007", NameId = "0001.0102", ValueId = "0001.0102.0007", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "1000 minuten", Value = "0001.0102.0008", NameId = "0001.0102", ValueId = "0001.0102.0008", GroupName = "Telfort abonnement", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "25 sms bundel", Value = "0001.0103.0001", NameId = "0001.0103", ValueId = "0001.0103.0001", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "100 sms bundel", Value = "0001.0103.0002", NameId = "0001.0103", ValueId = "0001.0103.0002", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 sms bundel", Value = "0001.0103.0003", NameId = "0001.0103", ValueId = "0001.0103.0003", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "1000 sms bundel", Value = "0001.0103.0004", NameId = "0001.0103", ValueId = "0001.0103.0004", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 200 MB", Value = "0001.0104.0001", NameId = "0001.0104", ValueId = "0001.0104.0001", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1 GB", Value = "0001.0104.0002", NameId = "0001.0104", ValueId = "0001.0104.0002", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1,5 GB", Value = "0001.0104.0003", NameId = "0001.0104", ValueId = "0001.0104.0003", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });

                _list.Where(x => x.Value == "BB2010" && x.ParentId == _Search_Lookup_TypeProduct
                    .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid).First().Lookup_BundelList = _blist;

                session1.CommitChanges();
            }
            // New - Normal - Telfort sim only
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_SubscriptionType = new XPQuery<Lookup_SubscriptionType>(session1).ToList();
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                //XPCollection<Lookup_SubscriptionType> _Lookup_SubscriptionTypeList = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                int ParentId = _Search_Lookup_SubscriptionType
                    .Where(x => x.ParentId == _Search_Lookup_TypeProduct
                        .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid
                        && x.Value == "SIMOnly2010").First().Oid;

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_Bundel> _blist = new XPCollection<Lookup_Bundel>(session1);

                _blist.Add(new Lookup_Bundel(session1) { Name = "75 minuten", Value = "0001.0102.0001", NameId = "0001.0102", ValueId = "0001.0102.0001", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "200 minuten", Value = "0001.0102.0002", NameId = "0001.0102", ValueId = "0001.0102.0002", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 minuten", Value = "0001.0102.0003", NameId = "0001.0102", ValueId = "0001.0102.0003", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "400 minuten", Value = "0001.0102.0004", NameId = "0001.0102", ValueId = "0001.0102.0004", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "500 minuten", Value = "0001.0102.0005", NameId = "0001.0102", ValueId = "0001.0102.0005", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "750 minuten", Value = "0001.0102.0006", NameId = "0001.0102", ValueId = "0001.0102.0006", GroupName = "Telfort sim only", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "25 sms bundel", Value = "0001.0103.0001", NameId = "0001.0103", ValueId = "0001.0103.0001", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "100 sms bundel", Value = "0001.0103.0002", NameId = "0001.0103", ValueId = "0001.0103.0002", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "300 sms bundel", Value = "0001.0103.0003", NameId = "0001.0103", ValueId = "0001.0103.0003", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "1000 sms bundel", Value = "0001.0103.0004", NameId = "0001.0103", ValueId = "0001.0103.0004", GroupName = "Telfort sms bundel", GroupValue = "0001", ParentId = ParentId });

                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 200 MB", Value = "0001.0104.0001", NameId = "0001.0104", ValueId = "0001.0104.0001", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1 GB", Value = "0001.0104.0002", NameId = "0001.0104", ValueId = "0001.0104.0002", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "surf&mail 1,5 GB", Value = "0001.0104.0003", NameId = "0001.0104", ValueId = "0001.0104.0003", GroupName = "surf&mail", GroupValue = "0001", ParentId = ParentId });

                _list.Where(x => x.Value == "SIMOnly2010" && x.ParentId == _Search_Lookup_TypeProduct
                    .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid).First().Lookup_BundelList = _blist;

                session1.CommitChanges();
            }

            // New - Normal - Telfort mobiel internet
            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Search_Lookup_SubscriptionType = new XPQuery<Lookup_SubscriptionType>(session1).ToList();
                var _Search_Lookup_TypeProduct = new XPQuery<Lookup_TypeProduct>(session1).ToList();
                var _Search_Loopup_SortPlan = new XPQuery<Loopup_SortPlan>(session1).ToList();

                //XPCollection<Lookup_SubscriptionType> _Lookup_SubscriptionTypeList = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_TypeProduct> _Lookup_TypeProductList = new XPCollection<Lookup_TypeProduct>(session1);
                XPCollection<Loopup_SortPlan> _Loopup_SortPlanList = new XPCollection<Loopup_SortPlan>(session1);

                int ParentId = _Search_Lookup_SubscriptionType
                    .Where(x => x.ParentId == _Search_Lookup_TypeProduct
                        .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid
                        && x.Value == "MIU2008").First().Oid;

                XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);
                XPCollection<Lookup_Bundel> _blist = new XPCollection<Lookup_Bundel>(session1);

                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 250 MB", Value = "0001.0102.0001", NameId = "0001.0102", ValueId = "0001.0102.0001", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 500 MB", Value = "0001.0102.0002", NameId = "0001.0102", ValueId = "0001.0102.0002", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 2 GB", Value = "0001.0102.0003", NameId = "0001.0102", ValueId = "0001.0102.0003", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });
                _blist.Add(new Lookup_Bundel(session1) { Name = "mobiel internet 4 GB", Value = "0001.0102.0004", NameId = "0001.0102", ValueId = "0001.0102.0004", GroupName = "Telfort mobiel internet", GroupValue = "0001", ParentId = ParentId });
                
                _list.Where(x => x.Value == "MIU2008" && x.ParentId == _Search_Lookup_TypeProduct
                    .Where(y => y.Name.Equals(Lookup_TypeProduct.ProductKeuze.NORMAL.ToString()) && y.ParentId == _Search_Loopup_SortPlan
                            .Where(y1 => y1.Name.Equals(Loopup_SortPlan.PlanType.Nieuw.ToString())).First().Oid).First().Oid).First().Lookup_BundelList = _blist;

                session1.CommitChanges();
            }
        }
        // wordt niet gebruikt, kan verwijderd worden
        internal static void InitSubscriptionPlan2()
        {
            //using (UnitOfWork session1 = new UnitOfWork())
            //{
            //    var _obj = new XPQuery<SubscriptionPlan>(session1).ToList();
            //    SubscriptionPlan _SubscriptionPlan = new SubscriptionPlan(session1);
            //    XPCollection<Lookup_SubscriptionType> _list = new XPCollection<Lookup_SubscriptionType>(session1);

            //    Lookup_SubscriptionType _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
            //    XPCollection<Lookup_SubscriptionBundel> _blist = new XPCollection<Lookup_SubscriptionBundel>(session1);

            //    if (_obj.Count() == 0)
            //    {
            //        //Telfort sim only
            //        _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
            //        _Lookup_SubscriptionType.Name = "Telfort sim only";
            //        _Lookup_SubscriptionType.NameId = "SIMOnly2010";
            //        _Lookup_SubscriptionType.Value = "0001.0101";

            //        _blist = new XPCollection<Lookup_SubscriptionBundel>(session1);
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "75 minuten", Value = "0001.0101.0001" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "200 minuten", Value = "0001.0101.0002" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "300 minuten", Value = "0001.0101.0003" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "400 minuten", Value = "0001.0101.0004" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "500 minuten", Value = "0001.0101.0005" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "750 minuten", Value = "0001.0101.0006" });
            //        _Lookup_SubscriptionType.Lookup_SubscriptionBundelList = _blist;

            //        _list.Add(_Lookup_SubscriptionType);

            //        //Telfort sms bundel
            //        _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
            //        _Lookup_SubscriptionType.Name = "Telfort sms bundel";
            //        _Lookup_SubscriptionType.NameId = "SOSMSCLbundel2010";
            //        _Lookup_SubscriptionType.Value = "0001.0102";

            //        _blist = new XPCollection<Lookup_SubscriptionBundel>(session1);
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "25 sms bundel", Value = "0001.0102.0001" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "100 sms bundel", Value = "0001.0102.0002" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "300 sms bundel", Value = "0001.0102.0003" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "1000 sms bundel", Value = "0001.0102.0004" });

            //        _Lookup_SubscriptionType.Lookup_SubscriptionBundelList = _blist;
            //        _list.Add(_Lookup_SubscriptionType);

            //        //surf&mail
            //        _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);
            //        _Lookup_SubscriptionType.Name = "surf&mail";
            //        _Lookup_SubscriptionType.NameId = "surfmail2010";
            //        _Lookup_SubscriptionType.Value = "0001.0103";

            //        _blist = new XPCollection<Lookup_SubscriptionBundel>(session1);
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "surf&mail 200 MB", Value = "0001.0103.0001" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "surf&mail 1 GB", Value = "0001.0103.0002" });
            //        _blist.Add(new Lookup_SubscriptionBundel(session1) { Name = "surf&mail 1,5 GB", Value = "0001.0103.0003" });

            //        _Lookup_SubscriptionType.Lookup_SubscriptionBundelList = _blist;
            //        _list.Add(_Lookup_SubscriptionType);

            //        ///
            //        _SubscriptionPlan.Lookup_SubscriptionTypeList = _list;

            //        session1.CommitChanges();
            //    }
            //}
        }

        internal static string GetComboxValue<T>(string value)
        {
            using (Session session1 = new Session())
            {
                XPCollection<T> _list = new XPCollection<T>(session1);
                var _obj = _list.Where(s => s.GetType().GetProperty("Name").GetValue(s, null).ToString().ToLower() == value.ToLower()).FirstOrDefault();

                if ((_obj != null))
                    return _obj.GetType().GetProperty("Value").GetValue(_obj, null).ToString();

                return "0";
            }
        }

        internal static void InitRadioValues_Lookup_SubscriptionType(string SelectHTMLString, UnitOfWork session1,
            ref XPCollection<Lookup_SubscriptionType> _list)
        {
            string strSIMonly = "";
            string strSIMonlyValue = "";

            int x = Domain.CountWords(SelectHTMLString, "d.add(");
            for (int i = 0; i < x; i++)
            {
                Lookup_SubscriptionType _Lookup_SubscriptionType = new Lookup_SubscriptionType(session1);

                strSIMonlyValue = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, "d.add(\"", "\"");
                strSIMonly = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, ">", "\"");
                SelectHTMLString = SelectHTMLString.Replace(string.Format("d.add(\"{0}", strSIMonlyValue), "");
                SelectHTMLString = SelectHTMLString.Replace(string.Format(">{0}", strSIMonly), "");

                _Lookup_SubscriptionType.Name = strSIMonly;
                _Lookup_SubscriptionType.Value = strSIMonlyValue;

                _list.Add(_Lookup_SubscriptionType);            
            }
        }

        internal static void InitRadioValues_Lookup_SubscriptionType(string SelectHTMLString,
            ref XPCollection<Lookup_SubscriptionType> _list)
        {
            string strSIMonly = "";
            string strSIMonlyValue = "";

            int x = Domain.CountWords(SelectHTMLString, "d.add(");
            for (int i = 0; i < x; i++)
            {
                Lookup_SubscriptionType _Lookup_SubscriptionType = new Lookup_SubscriptionType();

                strSIMonlyValue = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, "d.add(\"", "\"");
                strSIMonly = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, ">", "\"");
                SelectHTMLString = SelectHTMLString.Replace(string.Format("d.add(\"{0}", strSIMonlyValue), "");
                SelectHTMLString = SelectHTMLString.Replace(string.Format(">{0}", strSIMonly), "");

                _Lookup_SubscriptionType.Name = strSIMonly;
                _Lookup_SubscriptionType.Value = strSIMonlyValue;

                _list.Add(_Lookup_SubscriptionType);
            }
        }

        internal static bool DoLogin(CookieContainer cookies, string strLoginUserName, string strLoginPassword, ref string AuthKey,
            ref string strResponseData, ref string strErrorMessage, ref System.Security.Cryptography.X509Certificates.X509Certificate2 certificateOut)
        {
            bool IsSuccess = false;

            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            //CookieContainer cookies = null;

            System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store("TrustedPeople",
                                System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);
            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly | System.Security.Cryptography.X509Certificates.OpenFlags.OpenExistingOnly);

            System.Security.Cryptography.X509Certificates.X509Certificate2Collection certificateCollection = store.Certificates.Find(System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "tft.notify@prolocation.net", false);

            if (certificateCollection.Count > 0)
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = certificateCollection[0];
                certificateOut = certificate;

                //Klant_Telfort _Klant_Telfort = new Klant_Telfort();

                strResponseData = ScrapeHelper.DoLogin(
                    new Uri("https://b2b.telfort.nl/boss/"),
                    "",
                    "",
                    ref cookies,
                    certificate
                    );

                //WriteBreakAndLine(strResponseData);

                dictPostData.Add("SAMLRequest", "");
                dictPostData.Add("RelayState", "");
                dictPostData.Add("RelayStateEshop", "");
                dictPostData.Add("loginType", "");

                strResponseData = ScrapeHelper.DoPostPage(
                    new Uri("https://b2b.telfort.nl/boss/post.do"),
                    cookies,
                    strResponseData,
                    dictPostData,
                    "",
                    "",
                    certificate
                    );

                //WriteBreakAndLine(strResponseData);

                strResponseData = ScrapeHelper.DoGetPage(
                    new Uri("https://b2b.telfort.nl/ca/loginbox.do?service=https%3A%2F%2Fmijn.telfort.nl%2Fboss&continue=Frameset.jsp"),
                    cookies,
                    "",
                    "",
                    certificate
                    );

                //WriteBreakAndLine(strResponseData);

                dictPostData.Clear();
                dictPostData.Add("SAMLRequest", "");
                dictPostData.Add("RelayState", "");
                dictPostData.Add("mobile", strLoginUserName);
                dictPostData.Add("password", strLoginPassword);

                strResponseData = ScrapeHelper.DoPostPage(
                    new Uri("https://b2b.telfort.nl/ca/POST.do"),
                    cookies,
                    strResponseData,
                    dictPostData,
                    "",
                    "",
                    certificate
                    );

                //WriteBreakAndLine(strResponseData);

                strErrorMessage = ScrapeHelper.ExtractValue(strResponseData, "<p class=\"errormessage\">", "</p>");

                if (String.IsNullOrEmpty(strErrorMessage))
                {
                    dictPostData.Clear();
                    dictPostData.Add("loginType", "");
                    dictPostData.Add("RelayState", "");
                    dictPostData.Add("SAMLart", "");

                    strResponseData = ScrapeHelper.DoPostPage(
                        new Uri("https://b2b.telfort.nl/boss/post.do"),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );
                    //WriteBreakAndLine(strResponseData);
                    if (strResponseData.Contains("Dit account is aangemeld op een andere locatie"))
                    {
                        strResponseData = ScrapeHelper.DoGetPage(
                            new Uri("https://b2b.telfort.nl/boss/repeatLogin.do?actionFlag=login&loginPage=Frameset.jsp&loginRepeatFlag=1&operId=" + strLoginUserName),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //WriteBreakAndLine(strResponseData);
                    }

                    strResponseData = ScrapeHelper.DoGetPage(
                        new Uri("https://b2b.telfort.nl/custcare/cc_common/commonLoginAction.do?method=initPage&loadmode=loginboss&lang=nl_NL"),
                        cookies,
                        "",
                        "",
                        certificate
                        );
                    //WriteBreakAndLine(strResponseData);
                    string strUrl = ScrapeHelper.ExtractValue(strResponseData, "action = \"", "\"");

                    if (!String.IsNullOrEmpty(strUrl))
                    {
                        strUrl = "https://b2b.telfort.nl" + strUrl;

                        if (Uri.IsWellFormedUriString(strUrl, UriKind.RelativeOrAbsolute))
                        {
                            dictPostData.Clear();

                            string strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"OperID\" value=\"", "\"");
                            dictPostData.Add("OperID", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"OperName\" value=\"", "\"");
                            dictPostData.Add("OperName", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"Region\" value=\"", "\"");
                            dictPostData.Add("Region", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"OrgID\" value=\"", "\"");
                            dictPostData.Add("OrgID", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"QueueID\" value=\"", "\"");
                            dictPostData.Add("QueueID", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"IPAddress\" value=\"", "\"");
                            dictPostData.Add("IPAddress", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"MacAddress\" value=\"", "\"");
                            dictPostData.Add("MacAddress", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"RoamOrgID\" value=\"", "\"");
                            dictPostData.Add("RoamOrgID", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"RoamRegion\" value=\"", "\"");
                            dictPostData.Add("RoamRegion", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"LoginAuthKey\" value=\"", "\"");
                            AuthKey = strValue;
                            dictPostData.Add("LoginAuthKey", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"bossJSessionID\" value=\"", "\"");
                            dictPostData.Add("bossJSessionID", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"Language\" value=\"", "\"");
                            dictPostData.Add("Language", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"sid\" value=\"", "\"");
                            dictPostData.Add("sid", strValue);
                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=hidden name=\"targetURI\"", " />");
                            strValue = ScrapeHelper.ExtractValue(strValue, "value=\"", "\"");
                            dictPostData.Add("targetURI", strValue);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(strUrl),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            IsSuccess = true;
                            //WriteBreakAndLine(strResponseData);
                        }                        
                    }
                    ////------
                }
            }
            return IsSuccess;
        }

        internal static void DoLogout(CookieContainer cookies, string authKey, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            string strResponseData;
            //CookieContainer cookies = null;
            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            string Random = Ult.CreateRandomString(16);

            LoginInfo _LoginInfo = new LoginInfo();
            string operID = _LoginInfo.LoginName;

            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                new Uri(string.Format("https://b2b.telfort.nl/boss/repeatLogin.do?actionFlag=exit&operId={0}&random=0.{1}", operID, Random)),
                cookies,
                "",
                "",
                certificate
                );

            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                new Uri(string.Format("https://b2b.telfort.nl/boss/Logout.jsp?operID={0}&authKey={1}", operID, authKey)),
                cookies,
                "",
                "",
                certificate
                );

            dictPostData.Clear();
            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
            new Uri(string.Format("https://b2b.telfort.nl/ca/logout.do?service=https%3A%2F%2Fmijn.telfort.nl%2Fboss&continue=https%3A%2F%2Fb2b.telfort.nl%2Fboss%2Flogout.do%3Fcontinue%3D%2Fboss")),
            cookies,
            strResponseData,
            dictPostData,
            "",
            "",
            certificate
            );
            //
            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                new Uri(string.Format("https://b2b.telfort.nl/custcare/BossLogoutClient?loginAuthKey={0}", authKey)),
                cookies,
                "",
                "",
                certificate
                );
            //
            dictPostData.Clear();
            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
            new Uri(string.Format("https://b2b.telfort.nl/channel/loginAction.do?step=quit&loginAuthKey={0}", authKey)),
            cookies,
            strResponseData,
            dictPostData,
            "",
            "",
            certificate
            );
            //
            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
               new Uri(string.Format("https://b2b.telfort.nl/RenascenceWeb/jsp/Logout.jsp?staffNo={0}&cityID={1}&LoginAuthKey={2}", operID, operID, authKey)),
               cookies,
               "",
               "",
               certificate
               );
            //
            //
            dictPostData.Clear();
            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
            new Uri(string.Format("https://b2b.telfort.nl/boss/logout.do?continue=/boss")),
            cookies,
            strResponseData,
            dictPostData,
            "",
            "",
            certificate
            );
            //
            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                new Uri(string.Format("https://b2b.telfort.nl/sessionLogout.do?operstaffno={0}&loginAuthKey={1}", operID, authKey)),
                cookies,
                "",
                "",
                certificate
                );
            //
            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
               new Uri(string.Format("https://b2b.telfort.nl/report/BossExitServlet?isBossExit=true&loginAuthKey={0}", authKey)),
               cookies,
               "",
               "",
               certificate
               );
            //
            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                new Uri(string.Format("https://b2b.telfort.nl/ca/loginbox.do?service=https%3A%2F%2Fmijn.telfort.nl%2Fboss&continue=Frameset.jsp")),
                cookies,
                "",
                "",
                certificate
                );
            //
            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                new Uri(string.Format("https://b2b.telfort.nl/email/BossExitServlet?isBossExit=true&loginAuthKey={0}", authKey)),
                cookies,
                "",
                "",
                certificate
                );
            //
            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                new Uri(string.Format("https://b2b.telfort.nl/Inventory/BossLogoutClient?loginAuthKey={0}", authKey)),
                cookies,
                "",
                "",
                certificate
                );

            //////////////////////

        }

        internal static List<LookupBase> InitContractPeriode(string strResponseData)
        {
            string strMaand = "";
            string strMaandValue = "";
            List<LookupBase> _list = new List<LookupBase>();

            int x = Domain.CountWords(strResponseData, "@_@");
            for (int i = 0; i < x; i++)
            {
                LookupBase _LookupBase = new LookupBase();

                strMaandValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, ",\"", "@_@");
                strMaand = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "\"]");
                strResponseData = strResponseData.Replace(string.Format(",\"{0}", strMaandValue), "");
                strResponseData = strResponseData.Replace(string.Format("@_@{0}\"]", strMaand), "");

                _LookupBase.Name = strMaand;
                _LookupBase.Value = strMaandValue;

                _list.Add(_LookupBase);
            }
            return _list;
        }

        internal static List<LookupBase> InitProducts(string SelectHTMLString)
        {
            string strSIMonly = "";
            string strSIMonlyValue = "";
            List<LookupBase> _list = new List<LookupBase>();
            
            int x = Domain.CountWords(SelectHTMLString, "d.add(");
            for (int i = 0; i < x; i++)
            {
                LookupBase _LookupBase = new LookupBase();

                strSIMonlyValue = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, "d.add(\"", "\"");
                strSIMonly = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, ">", "\"");
                SelectHTMLString = SelectHTMLString.Replace(string.Format("d.add(\"{0}", strSIMonlyValue), "");
                SelectHTMLString = SelectHTMLString.Replace(string.Format(">{0}", strSIMonly), "");

                _LookupBase.Name = strSIMonly;
                _LookupBase.Value = strSIMonlyValue;

                _list.Add(_LookupBase);
            }
            return _list;
        }

        internal static List<LookupBase> InitContractPeriodeVerlengen(string strResponseData)
        {
            string strMaand = "";
            string strMaandValue = "";
            List<LookupBase> _list = new List<LookupBase>();

            int x = Domain.CountWords(strResponseData, "<itemId>");
            for (int i = 0; i < x; i++)
            {
                LookupBase _LookupBase = new LookupBase();

                strMaandValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<itemId>", "</itemId>");
                strMaand = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<itemName>", "</itemName>");
                strResponseData = strResponseData.Replace(string.Format("<itemId>{0}</itemId>", strMaandValue), "");
                strResponseData = strResponseData.Replace(string.Format("<itemName>{0}</itemName>", strMaand), "");

                _LookupBase.Name = strMaand;
                _LookupBase.Value = strMaandValue;

                _list.Add(_LookupBase);
            }
            return _list;
            //throw new NotImplementedException();
        }

        internal static XPCollection<Lookup_Service_Provider> GetServiceProvider(string strResponseData, UnitOfWork session1)
        {
            string Data = strResponseData;
            string naam = "", value = "";
            XPCollection<Lookup_Service_Provider> _List = new XPCollection<Lookup_Service_Provider>(session1);
            while (Data.Contains("@_@"))
            {
                value = HttpWebManager.ScrapeHelper.ExtractValue(Data, "[\"1\",\"", "@_@");
                naam = HttpWebManager.ScrapeHelper.ExtractValue(Data, "@_@", "\"");
                _List.Add(new Lookup_Service_Provider(session1) { Name = naam, Value = value });
                Data = Data.Replace(string.Format("[\"1\",\"{0}@_@{1}", value, naam), "");
            }
            return _List;
        }

        internal static List<string> ReturnPhoneListValues(string SelectHTMLString)
        {
            string strSIMonly = "";
            string strSIMonlyValue = "";
            string tussenTekst = "";

            List<string> _list = new List<string>();

            while (SelectHTMLString.Contains("</option>"))
            {
                strSIMonlyValue = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, "<option value='", "'>");
                tussenTekst = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value='{0}'", strSIMonlyValue), ">");
                strSIMonly = HttpWebManager.ScrapeHelper.ExtractValue(SelectHTMLString, String.Format("<option value='{0}'{1}>", strSIMonlyValue, tussenTekst), "</option>");
                                
                _list.Add(strSIMonlyValue);

                SelectHTMLString = SelectHTMLString.Replace(string.Format("<option value='{0}'{1}>{2}</option>", strSIMonlyValue, tussenTekst, strSIMonly), "");
            }

            return _list;
        }

        internal static bool ScrapeError(string strResponseData)
        {
            return strResponseData.Contains("<TITLE>redirect kicked</TITLE>");
        }
    }
}
