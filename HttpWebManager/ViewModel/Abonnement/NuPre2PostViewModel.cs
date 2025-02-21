using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using XpoObjects;
using DevExpress.Xpo;
using Telfort_XPO_Objects;
using DevExpress.Xpo.DB;
using System.Configuration;
using System.Web;
using System.Threading;
using System.Windows.Forms;

namespace HttpWebManager
{
    [Telfort_Objects.LookupViewModel(ContractType = (int)Telfort_Objects.ContractType.Pre2Post)]
    public class NuPre2PostViewModel : ScrapingBase
    {
        public NuPre2PostViewModel() : base() { }

        public override void Run()
        {
            Inschieten();
        }

        private void Execute(Object sender, EventArgs e)
        {
            NuPre2Post();
        }

        private void NuPre2Post()
        {
            SerializationManager<Telfort_Objects.Contract> _c = new SerializationManager<Telfort_Objects.Contract>() { FileName = "Pre2Post.xml" };
            _c.Load();
            DealerContract = _c.Content;

            Func<Telfort_Objects.Contract, HttpWebResult> _HttpWebResult = TelfortKlantViewModel.ContractNaarTelfortSturen;
            IAsyncResult ar = _HttpWebResult.BeginInvoke(DealerContract, null, null);
            this.HttpWebResult = _HttpWebResult.EndInvoke(ar);
        }

        internal void Inschieten()
        {
            InitControlsBinding();
            InvokeBezig();
            string strRetentionResult = string.Empty;
            DateTime dtEndDate = DateTime.MinValue;
            //string ThisMobileNumber = Mobile;

            this.HttpWebResult = new HttpWebResult();
            LoginInfo _LoginInfo = new LoginInfo();
            string strLoginUserName = _LoginInfo.LoginName, strLoginPassword = _LoginInfo.Password;

            string strResponseData = string.Empty, strErrorMessage = string.Empty;

            //string Activatiedatum = DateTime.Today.AddDays(1).ToShortDateString();

            //Telfort_Objects.Contract _DealerContract = new Telfort_Objects.Contract();
            //SerializationManager<Telfort_Objects.Contract> _c = new SerializationManager<Telfort_Objects.Contract>() { FileName = "Pre2Post.xml" };
            //_c.Load();
            //_DealerContract = _c.Content;

            //HttpWebResult _HttpWebResult2 = TelfortKlantViewModel.Set(_DealerContract);

            string SIM = "893126" + DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr;

            string authKey = "", OperID = "";

            UserBundelChoiceViewModel _UserBundelChoiceViewModel = Using<UserBundelChoiceViewModel>(DealerContract);
            _UserBundelChoiceViewModel.BundelEvent += UserBundelChoiceViewModel.GetUserBundelChoiceCollection;
            _UserBundelChoiceViewModel.GetContractBundel();

            string Privilege = UserBundelChoice.Privilege, AppendProduct = UserBundelChoice.AppendProduct, ids = UserBundelChoice.Ids;
            string GekozenSimOnly = UserBundelChoice.GekozenSimOnly, GekozenSmsBundel = UserBundelChoice.GekozenSmsBundel, GekozenSimOnly_GekozenSmsBundel = UserBundelChoice.GekozenSimOnly_GekozenSmsBundel, GekozenSmsBundelOptie = UserBundelChoice.GekozenSmsBundelOptie;
            string Surf_Mail = UserBundelChoice.Surf_Mail, Surf_Mail_Optie = UserBundelChoice.Surf_Mail_Optie, Surf_Mail_SimOnly = UserBundelChoice.Surf_Mail_SimOnly;

            //string Privilege = "";
            //string AppendProduct = "";
            //string ids = "";
            //string GekozenSimOnly = "", GekozenSmsBundel = "", GekozenSimOnly_GekozenSmsBundel = "", GekozenSmsBundelOptie = "";
            //string Surf_Mail = "", Surf_Mail_Optie = "", Surf_Mail_SimOnly = "";

            //using (UnitOfWork session1 = new UnitOfWork())
            //{
            //    var _Lookup_AboBundelList = new XPQuery<Lookup_AboBundel>(session1).ToList().Where(x => x.Actief == true).ToList();
            //    _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementProductList
            //    .ForEach(x =>
            //    {
            //        string aboId = x.BundelId;
            //        var Lookup_AboBundel = _Lookup_AboBundelList.Where(y => y.BundelId == x.BundelId).FirstOrDefault();
            //        if (Lookup_AboBundel.ValueId == "0001.0101")
            //        {
            //            Privilege = String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
            //            Privilege += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
            //            GekozenSimOnly = Lookup_AboBundel.Value;
            //        }
            //        else
            //        {
            //            if (Lookup_AboBundel.ValueId == "0001.0102")
            //            {
            //                GekozenSmsBundel = Lookup_AboBundel.ValueId;
            //                GekozenSimOnly_GekozenSmsBundel = string.Format("@{0}@{1}@", GekozenSimOnly, Lookup_AboBundel.Value);
            //                GekozenSmsBundelOptie = Lookup_AboBundel.Value;
            //            }
            //            if (Lookup_AboBundel.ValueId == "0001.0103")
            //            {
            //                Surf_Mail = Lookup_AboBundel.ValueId;
            //                Surf_Mail_Optie = Lookup_AboBundel.Value;
            //                Surf_Mail_SimOnly = string.Format("@{0}@", GekozenSimOnly);
            //                Surf_Mail_SimOnly += !string.IsNullOrEmpty(GekozenSmsBundel) ? GekozenSmsBundelOptie + "@" : Lookup_AboBundel.Value + "@";
            //            }

            //            AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
            //            AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
            //        }
            //    });
            //    ids = Privilege + AppendProduct;

            //    //,,,0001,0001.0101;,,,0001.0101,0001.0101.0002;,,,0001,0001.0102;,,,0001.0102,0001.0102.0002;,,,0001,0001.0103;,,,0001.0103,0001.0103.0002;
            //}

            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            CookieContainer cookies = null;
            bool Saved = false;
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2();

            try
            {

                System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store("TrustedPeople",
                                    System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);
                store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly | System.Security.Cryptography.X509Certificates.OpenFlags.OpenExistingOnly);

                System.Security.Cryptography.X509Certificates.X509Certificate2Collection certificateCollection = store.Certificates.Find(System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "tft.notify@prolocation.net", false);

                if (certificateCollection.Count > 0)
                {
                    //System.Security.Cryptography.X509Certificates.X509Certificate2 
                    certificate = certificateCollection[0];
                    //using (UnitOfWork session1 = new UnitOfWork())
                    //{
                    //XPCollection<Lookup_AboBundel> _Lookup_AboBundelList = new XPCollection<Lookup_AboBundel>(session1);

                    //--login begin
                    strResponseData = HttpWebManager.ScrapeHelper.DoLogin(
                        new Uri("https://b2b.telfort.nl/boss/"),
                        "",
                        "",
                        ref cookies,
                        certificate
                        );



                    dictPostData.Add("SAMLRequest", "");
                    dictPostData.Add("RelayState", "");
                    dictPostData.Add("RelayStateEshop", "");
                    dictPostData.Add("loginType", "");

                    strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri("https://b2b.telfort.nl/boss/post.do"),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );



                    strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                        new Uri("https://b2b.telfort.nl/ca/loginbox.do?service=https%3A%2F%2Fmijn.telfort.nl%2Fboss&continue=Frameset.jsp"),
                        cookies,
                        "",
                        "",
                        certificate
                        );



                    dictPostData.Clear();
                    dictPostData.Add("SAMLRequest", "");
                    dictPostData.Add("RelayState", "");
                    dictPostData.Add("mobile", strLoginUserName);
                    dictPostData.Add("password", strLoginPassword);

                    strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri("https://b2b.telfort.nl/ca/POST.do"),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );



                    strErrorMessage = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<p class=\"errormessage\">", "</p>");

                    if (String.IsNullOrEmpty(strErrorMessage))
                    {
                        dictPostData.Clear();
                        dictPostData.Add("loginType", "");
                        dictPostData.Add("RelayState", "");
                        dictPostData.Add("SAMLart", "");

                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/boss/post.do"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        if (strResponseData.Contains("Dit account is aangemeld op een andere locatie"))
                        {
                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                new Uri("https://b2b.telfort.nl/boss/repeatLogin.do?actionFlag=login&loginPage=Frameset.jsp&loginRepeatFlag=1&operId=" + strLoginUserName),
                                cookies,
                                "",
                                "",
                                certificate
                                );

                        }

                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri("https://b2b.telfort.nl/custcare/cc_common/commonLoginAction.do?method=initPage&loadmode=loginboss&lang=nl_NL"),
                            cookies,
                            "",
                            "",
                            certificate
                            );

                        string strUrl = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "action = \"", "\"");

                        if (!String.IsNullOrEmpty(strUrl))
                        {
                            strUrl = "https://b2b.telfort.nl" + strUrl;

                            if (Uri.IsWellFormedUriString(strUrl, UriKind.RelativeOrAbsolute))
                            {
                                dictPostData.Clear();

                                string strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"OperID\" value=\"", "\"");
                                OperID = strValue;
                                dictPostData.Add("OperID", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"OperName\" value=\"", "\"");
                                dictPostData.Add("OperName", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"Region\" value=\"", "\"");
                                dictPostData.Add("Region", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"OrgID\" value=\"", "\"");
                                dictPostData.Add("OrgID", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"QueueID\" value=\"", "\"");
                                dictPostData.Add("QueueID", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"IPAddress\" value=\"", "\"");
                                dictPostData.Add("IPAddress", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"MacAddress\" value=\"", "\"");
                                dictPostData.Add("MacAddress", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"RoamOrgID\" value=\"", "\"");
                                dictPostData.Add("RoamOrgID", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"RoamRegion\" value=\"", "\"");
                                dictPostData.Add("RoamRegion", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"LoginAuthKey\" value=\"", "\"");
                                authKey = strValue;
                                dictPostData.Add("LoginAuthKey", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"bossJSessionID\" value=\"", "\"");
                                dictPostData.Add("bossJSessionID", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"Language\" value=\"", "\"");
                                dictPostData.Add("Language", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"sid\" value=\"", "\"");
                                dictPostData.Add("sid", strValue);
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=hidden name=\"targetURI\"", " />");
                                strValue = HttpWebManager.ScrapeHelper.ExtractValue(strValue, "value=\"", "\"");
                                dictPostData.Add("targetURI", strValue);

                                strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                                    new Uri(strUrl),
                                    cookies,
                                    strResponseData,
                                    dictPostData,
                                    "",
                                    "",
                                    certificate
                                    );

                            }
                        }
                        //login eind
                        List<LookupBase> _plist = new List<LookupBase>();
                        //_plist.Add(new LookupBase() { Name = Lookup_AboBundel.ProductKeuze.SIMONLY.ToString(), Value = ((int)Lookup_AboBundel.ProductKeuze.SIMONLY).ToString() });
                        _plist.Add(new LookupBase() { Name = Lookup_AboBundel.ProductKeuze.NORMAL.ToString(), Value = ((int)Lookup_AboBundel.ProductKeuze.NORMAL).ToString() });

                        //_plist.ForEach(l =>
                        //{
                        dictPostData.Clear();

                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/cc_common/CustomerInfoAction.do?act=setMenuId&activeMenuId=Pre2postMigrationPerson_WEB"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dictPostData.Clear();

                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/cc_common/MenuLogAction.do?act=activeMenuLog&activeMenuId=Pre2postMigrationPerson_WEB"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dictPostData.Clear();
                        dictPostData.Add("undefined", "");
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/boss/common/activeSessionAction.do"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/custsvc/basebusiness/migration/pre2PostMigrationAction.do?method=init&recType=Pre2Post&custType=PersonCustomer&ClearSession=true&sourceURL=null&language=nl_NL&custID4IPCC=&subsID4IPCC=&msisdn4IPCC="),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        string dt = new DateTime().ToString();
                        dictPostData.Clear();
                        dictPostData.Add("date", dt);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/boss/heartThrob.jsp?date={0}", dt)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dictPostData.Clear();

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/migration/pre2PostMigrationAction.do?method=checkCust&servNumber={0}&SSN={1}&custType=PersonCustomer",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr, SIM)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        strResponseData = strResponseData.RemoveSpaceAndBreak();
                        string custId = ScrapeHelper.ExtractValue(strResponseData, ":", "\n");
                        //
                        dictPostData.Clear();
                        dictPostData.Add("ssnPrefix", "893126");
                        dictPostData.Add("m_SSN", SIM);
                        dictPostData.Add("custType", "PersonCustomer");
                        dictPostData.Add("custid", custId);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/migration/pre2PostMigrationAction.do?method=queryCustInfo&custid={0}&servNumber={1}&SSN={2}&custType=PersonCustomer",
                                custId, DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr, SIM)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        string org_apache_struts_taglib_html_TOKEN = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"org.apache.struts.taglib.html.TOKEN\" value=\"", "\"");

                        //
                        dictPostData.Clear();
                        dictPostData.Add("date", dt);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/boss/heartThrob.jsp?date={0}", dt)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dictPostData.Clear();
                        string Productkeuze = ((int)Lookup_AboBundel.ProductKeuze.NORMAL).ToString();
                        dictPostData.Clear();
                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                        dictPostData.Add("initials", DealerContract.Klant.Voorletters);
                        dictPostData.Add("middleNameBirth", DealerContract.Klant.Middlenamebirth);
                        dictPostData.Add("birthName", DealerContract.Klant.Geboortenaam);
                        dictPostData.Add("middleName", DealerContract.Klant.Tussenvoegsel);
                        dictPostData.Add("lastName", DealerContract.Klant.Achternaam);
                        dictPostData.Add("gender", DealerContract.Klant.Geslacht.ToString());
                        dictPostData.Add("birthDate", DealerContract.Klant.Geboortedatum.ToShortDateString());
                        dictPostData.Add("postCode", DealerContract.Klant.AdresList.First().Postcode);
                        dictPostData.Add("houseNumber", DealerContract.Klant.AdresList.First().Huisnummer);
                        dictPostData.Add("suffixNumber", DealerContract.Klant.AdresList.First().Toevoeging);
                        dictPostData.Add("street", DealerContract.Klant.AdresList.First().Straat);
                        dictPostData.Add("place", DealerContract.Klant.AdresList.First().Plaats);
                        dictPostData.Add("nationality", DealerContract.Klant.Nationaliteit);
                        dictPostData.Add("contactTelephone", "");
                        dictPostData.Add("email", DealerContract.Klant.Emailadres);
                        dictPostData.Add("idType", DealerContract.Klant.IDtype);
                        dictPostData.Add("countryOfIssue", DealerContract.Klant.Afgegevenin);
                        dictPostData.Add("idNumber", DealerContract.Klant.IDnummer);
                        dictPostData.Add("dateOfIssue", DealerContract.Klant.Afgiftedatum.ToShortDateString());
                        dictPostData.Add("dateOfExpire", DealerContract.Klant.Vervaldatum.ToShortDateString());
                        dictPostData.Add("dealerCode", DealerContract.Klant.Dealercode);
                        dictPostData.Add("orgId", "");
                        dictPostData.Add("street2", DealerContract.Klant.AdresList.First().Straat);
                        dictPostData.Add("place2", DealerContract.Klant.AdresList.First().Plaats);
                        dictPostData.Add("accountType", DealerContract.Klant.TypeAccount);
                        dictPostData.Add("billFormat", DealerContract.Klant.FactuurType);
                        dictPostData.Add("bankAccountNumber", DealerContract.Klant.AbonnementContractList.First().Rekeningnummer);
                        dictPostData.Add("bankAccountType", DealerContract.Klant.AbonnementContractList.First().Rekeningnummer.Length == 7 ? "Giro" : "Bank");
                        dictPostData.Add("SIMonly", Productkeuze);
                        dictPostData.Add("install", "");

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/queryOPCAction.do?act=qryOldCustList&maxUserCountsForConsumer=3")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/installPCAInfoAction.do?act=nextPage&street={0}&place={1}&useBillAddress=same&gender={2}&nationality={3}&idType={4}&countryOfIssue={5}&accountType={6}&billFormat={7}&paymentType={8}&SIMonly={9}&=&org.apache.struts.taglib.html.TOKEN={10}&initials={11}&middleNameBirth=&birthName={12}&middleName=&lastName={13}&birthDate={14}&postCode={15}&houseNumber={16}&suffixNumber=&street={17}&place={18}&contactTelephone=&email=&idNumber={19}&dateOfIssue={20}&dateOfExpire={21}&dealerCode={22}&orgId=&postCode2=&houseNumber2=&suffixNumber2=&street2=&place2=&bankAccountNumber={23}&bankAccountType={24}&install=&needDecoder=true",
                                DealerContract.Klant.AdresList.First().Straat, DealerContract.Klant.AdresList.First().Plaats, DealerContract.Klant.Geslacht,
                                DealerContract.Klant.Nationaliteit, DealerContract.Klant.IDtype, DealerContract.Klant.Afgegevenin,
                                DealerContract.Klant.TypeAccount, DealerContract.Klant.FactuurType, DealerContract.Klant.Betaalwijze,
                                Productkeuze, org_apache_struts_taglib_html_TOKEN, DealerContract.Klant.Voorletters, DealerContract.Klant.Geboortenaam,
                                DealerContract.Klant.Achternaam, DealerContract.Klant.Geboortedatum, DealerContract.Klant.AdresList.First().Postcode, DealerContract.Klant.AdresList.First().Huisnummer,
                                DealerContract.Klant.AdresList.First().Straat, DealerContract.Klant.AdresList.First().Plaats, DealerContract.Klant.IDnummer, DealerContract.Klant.Afgiftedatum,
                                DealerContract.Klant.Vervaldatum, DealerContract.Klant.Dealercode, DealerContract.Klant.AbonnementContractList.First().Rekeningnummer, DealerContract.Klant.AbonnementContractList.First().Rekeningnummer.Length == 7 ? "Giro" : "Bank"
                                )),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dictPostData.Clear();
                        dictPostData.Add("method", "cartCheckScore");
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=cartCheckScore")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        // post page 1
                        Productkeuze = ((int)Lookup_AboBundel.ProductKeuze.NORMAL).ToString();
                        dictPostData.Clear();
                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                        dictPostData.Add("initials", DealerContract.Klant.Voorletters);
                        dictPostData.Add("middleNameBirth", DealerContract.Klant.Middlenamebirth);
                        dictPostData.Add("birthName", DealerContract.Klant.Geboortenaam);
                        dictPostData.Add("middleName", DealerContract.Klant.Tussenvoegsel);
                        dictPostData.Add("lastName", DealerContract.Klant.Achternaam);
                        dictPostData.Add("gender", DealerContract.Klant.Geslacht.ToString());
                        dictPostData.Add("birthDate", DealerContract.Klant.Geboortedatum.ToShortDateString());
                        dictPostData.Add("postCode", DealerContract.Klant.AdresList.First().Postcode);
                        dictPostData.Add("houseNumber", DealerContract.Klant.AdresList.First().Huisnummer);
                        dictPostData.Add("suffixNumber", DealerContract.Klant.AdresList.First().Toevoeging);
                        dictPostData.Add("street", DealerContract.Klant.AdresList.First().Straat);
                        dictPostData.Add("place", DealerContract.Klant.AdresList.First().Plaats);
                        dictPostData.Add("nationality", DealerContract.Klant.Nationaliteit);
                        dictPostData.Add("contactTelephone", "");
                        dictPostData.Add("email", DealerContract.Klant.Emailadres);
                        dictPostData.Add("idType", DealerContract.Klant.IDtype);
                        dictPostData.Add("countryOfIssue", DealerContract.Klant.Afgegevenin);
                        dictPostData.Add("idNumber", DealerContract.Klant.IDnummer);
                        dictPostData.Add("dateOfIssue", DealerContract.Klant.Afgiftedatum.ToShortDateString());
                        dictPostData.Add("dateOfExpire", DealerContract.Klant.Vervaldatum.ToShortDateString());
                        dictPostData.Add("dealerCode", DealerContract.Klant.Dealercode);
                        dictPostData.Add("orgId", "");
                        dictPostData.Add("street2", "");
                        dictPostData.Add("place2", "");
                        dictPostData.Add("accountType", DealerContract.Klant.TypeAccount);
                        dictPostData.Add("billFormat", DealerContract.Klant.FactuurType);
                        dictPostData.Add("bankAccountNumber", DealerContract.Klant.AbonnementContractList.First().Rekeningnummer);
                        dictPostData.Add("bankAccountType", DealerContract.Klant.AbonnementContractList.First().Rekeningnummer.Length == 7 ? "Giro" : "Bank");
                        dictPostData.Add("SIMonly", Productkeuze);
                        dictPostData.Add("install", "");

                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=init"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dt = new DateTime().ToString();
                        dictPostData.Clear();
                        dictPostData.Add("date", dt);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/boss/heartThrob.jsp?date={0}", dt)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        dictPostData.Clear();
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=clearSessionForMainProduct")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=frmJsp&recType=Pre2Post&catalogType=ROOT&productType=ProdType_Person&custType=PersonCustomer&recChannel=bsacHal&subSeq=0"),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=commonTree&recType=Pre2Post&catalogType=ROOT&productType=ProdType_Person&custType=PersonCustomer&recChannel=bsacHal&solutionID=&productID=&subs_relation_type=&main_or_subs_card=&DependOnID=&subSeq=0"),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //
                        strResponseData = strResponseData.RemoveSpaceAndBreak();

                        string SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "d.add(\"ROOT\",\"\",\"Product\",\"\",\"_self\");", "d.oAll(true);").Replace("d.add(\"TlfPreFMRHSM", "").Replace("d.add(\"TlfPreFMRSOM", "").Replace(">Telfort prepaid", "").Replace(">Telfort Prepaid", "");

                        /////////-------------------------------------------------------
                        //Domain.InitProducts(SelectHTMLString).ForEach(item =>
                        //{
                        Thread.Sleep(1000);
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getNetType&productId={0}&productName={1}&RelationType=&StallType=0",
                            DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                            DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam)),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        strResponseData = strResponseData.RemoveSpaceAndBreak();

                        string GSM = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "\",\"", "@_@");
                        string resType = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");
                        strResponseData = strResponseData.Replace("@_@" + resType, "");
                        string brandId = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");

                        //string resType = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");
                        //strResponseData = strResponseData.Replace("@_@" + resType, "");
                        //string brandId = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");
                        //
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getContractPeriodByProductId&productId={0}",
                            DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId)),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        //
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActivationDateValid&ActivationDate={0}",
                            DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString())),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        //strResponseData = strResponseData.RemoveSpaceAndBreak();

                        //Domain.InitContractPeriode(strResponseData).ForEach(m =>
                        //{
                        Thread.Sleep(1000);
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubsInfoForVasProduct&saveFlag=M&hidShowSubSeq=0&contractPeriod={0}&productId={1}&hidProductName={2}&txtActivationDate={3}&txtMsisdn={4}&txtSsn={5}&txtImsi=&numberInclusion=NN&numberConcealed=undefined&canDoCustRec=false&chkGetVasProdFlag=false",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString(),
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr,
                                SIM)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                        );
                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=select&isHasCautioner=&curProcductID={0}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId)),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=selectProduct&cmd=select&isHasCautioner=&isSimple=&curProcductID={0}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId)),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //

                        //
                        dt = new DateTime().ToString();
                        dictPostData.Clear();
                        dictPostData.Add("date", dt);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/boss/heartThrob.jsp?date={0}", dt)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );


                        dictPostData.Clear();
                        dictPostData.Add("catatype", "Privilege");
                        dictPostData.Add("objectID", GekozenSimOnly);
                        dictPostData.Add("cmd", "addNode");
                        dictPostData.Add("radios", String.Format("@{0}@", GekozenSimOnly));

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //521
                        if (!string.IsNullOrEmpty(GekozenSmsBundel))
                        {
                            dictPostData.Clear();
                            dictPostData.Add("catatype", "AppendProduct");
                            dictPostData.Add("objectID", GekozenSmsBundel);
                            dictPostData.Add("cmd", "addNode");
                            dictPostData.Add("radios", GekozenSimOnly_GekozenSmsBundel);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            dictPostData.Clear();
                            dictPostData.Add("catatype", "Privilege");
                            dictPostData.Add("objectID", GekozenSmsBundelOptie);
                            dictPostData.Add("cmd", "addNode");
                            dictPostData.Add("radios", GekozenSimOnly_GekozenSmsBundel);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                        }
                        if (!string.IsNullOrEmpty(Surf_Mail)) //string Surf_Mail = "", Surf_Mail_SimOnly = "";
                        {
                            dictPostData.Clear();
                            dictPostData.Add("catatype", "AppendProduct");
                            dictPostData.Add("objectID", Surf_Mail);
                            dictPostData.Add("cmd", "addNode");
                            dictPostData.Add("radios", Surf_Mail_SimOnly);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            dictPostData.Clear();
                            dictPostData.Add("catatype", "Privilege");
                            dictPostData.Add("objectID", Surf_Mail_Optie);
                            dictPostData.Add("cmd", "addNode");
                            dictPostData.Add("radios", Surf_Mail_SimOnly);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                        }
                        //532

                        dictPostData.Clear();
                        dictPostData.Add("ids", ids);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=calcBuyOffFee")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //533
                        dictPostData.Clear();
                        dictPostData.Add("pids", "");
                        dictPostData.Add("buyOffFlag", "0");

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=commitSave")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        // end bundel kiezen

                        //
                        dictPostData.Clear();

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=clearSessionForMainProduct")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        // Result tonen begin
                        //
                        dictPostData.Clear();

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getVasProductList&curProcductID={0}&updateFlag=undefined&hidShowSubSeq=&contractPeriod={1}&productId={2}&hidProductName={3}&txtActivationDate={4}&txtMsisdn={5}&txtSsn={6}&txtImsi=&numberInclusion=NN&numberConcealed=undefined&canDoCustRec=false&chkGetVasProdFlag=false&saveFlag=M&isImport=undefined",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString(),
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr, SIM)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        dictPostData.Clear();

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActivationDateValid&ActivationDate={0}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString())),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        dictPostData.Clear();

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getVasProductList&curProcductID={0}&updateFlag=updateUserProd&hidShowSubSeq=0&contractPeriod={1}&productId={2}&hidProductName={3}&txtActivationDate={4}&txtMsisdn={5}&txtSsn={6}&txtImsi=&numberInclusion=NN&numberConcealed=undefined&canDoCustRec=false&chkGetVasProdFlag=true&saveFlag=M&isImport=undefined",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString(),
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr, SIM)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActiveDateValidDay&ActivationDate={0}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString())),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActivationDateValid&ActivationDate={0}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString())),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        //
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkZAZIProdClass")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        dictPostData.Clear();
                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                        dictPostData.Add("hidShowSubSeq", "0");
                        dictPostData.Add("hidProductId", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                        dictPostData.Add("hidProductName", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam);
                        dictPostData.Add("hidNetType", GSM);
                        dictPostData.Add("hidPrdBrand", brandId);
                        dictPostData.Add("hidPhoneResType", resType);
                        dictPostData.Add("hidPaymodeType", "");
                        dictPostData.Add("chkGetVasProdFlag", "on");
                        dictPostData.Add("hidTxtImsi", "");
                        dictPostData.Add("hidContractPeriod", "");
                        dictPostData.Add("errorMsg", "");
                        dictPostData.Add("productId", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                        dictPostData.Add("txtActivationDate", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString());
                        dictPostData.Add("contractPeriod", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode.ToString());
                        dictPostData.Add("txtMsisdn", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                        dictPostData.Add("ssnSuffix", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr);
                        dictPostData.Add("txtSsn", SIM);
                        dictPostData.Add("numberConcealed", "false");
                        dictPostData.Add("numberInclusion", "NN");
                        dictPostData.Add("imei", "");

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubInfoForPre2Post")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //// actie hier...token
                        string Token = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"token\" id=\"token\" value=\"", "\"");
                        string formnum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"formnum\" id=\"formnum\" value=\"", "\"");
                        string urlPath = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"urlPath\" id=\"urlPath\" value=\"", "\"");
                        string hasFee = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"hasFee\" id=\"hasFee\" value=\"", "\"");
                        string paytype = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"radio\" name=\"paytype\" id=\"paytype\" class=\"INPUT_Border0\" value=\"", "\"");
                        string nextBillvalue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"nextBillvalue\" id=\"nextBillvalue\" value=\"", "\"");


                        Thread.Sleep(1000);
                        dt = new DateTime().ToString();
                        dictPostData.Clear();
                        dictPostData.Add("date", dt);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/boss/heartThrob.jsp?date={0}", dt)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        string Random = Ult.CreateRandomString(16);
                        dictPostData.Clear();
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=false&random=0.{0}",
                                Random)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=printFra&printPriView=true&hasFee={0}&org.apache.struts.taglib.html.TOKEN={1}&paytype={2}&nextBillvalue={3}&preStep=Vorige&printPriBT=Contract%20tonen&formnum={4}&urlPath={5}&token={6}",
                                hasFee,
                                org_apache_struts_taglib_html_TOKEN,
                                paytype,
                                nextBillvalue,
                                formnum,
                                urlPath,
                                Token
                                )),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //

                        dictPostData.Clear();
                        Random = Ult.CreateRandomString(16);
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=true&random=0.{0}",
                                Random)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        dictPostData.Clear();
                        Saved = ScrapeHelper.DoPostPageSavePDF(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=proPrint")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate,
                            formnum
                            );

                        if (Saved)
                        {
                            byte[] Overeenkomst = null;
                            dictPostData.Clear();
                            Overeenkomst = ScrapeHelper.DoPostPagePDF(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=proPrint")),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            if (Overeenkomst != null)
                            {
                                DealerContract.Klant.AbonnementContractList.First().Document = Overeenkomst;
                            }

                            // inschieten hier!!!
                            //.....

                            HttpWebResult _HttpWebResult = TelfortKlantViewModel.Set(DealerContract);

                            if (_HttpWebResult.IsSuccess)
                            {
                                goto FinischWithSuccess;
                            }
                            else
                            {
                                HttpWebResult.IsSuccess = false;
                                HttpWebResult.ErrorMessage = _HttpWebResult.ErrorMessage;
                                goto FinischWithError;
                            }
                        }

                    FinischWithError:
                        HttpWebResult.IsSuccess = false;

                    FinischWithSuccess:
                        HttpWebResult.IsSuccess = Saved;

                        //
                        //});
                        //});
                        //});

                        //try
                        //{
                        //    // klaar met bundel data ophalen
                        //    //session1.CommitChanges();
                        //    HttpWebResult.IsSuccess = true;
                        //}
                        //catch (Exception err)
                        //{
                        //    HttpWebResult.IsSuccess = false;
                        //    HttpWebResult.ErrorMessage = err.Message;
                        //    //SerializationManager<XPCollection<Lookup_AboBundel>> _xlist = new SerializationManager<XPCollection<Lookup_AboBundel>>();
                        //    //_xlist.Content = _Lookup_AboBundelList;
                        //    //_xlist.Save();
                        //}

                    }
                    //}
                }
            }
            catch (Exception err)
            {
                HttpWebResult.IsSuccess = false;
                HttpWebResult.ErrorMessage = err.Message;
            }
            finally
            {
                Domain.DoLogout(cookies, authKey, certificate);
                InvokeResult();
            }
            //HttpWebResult.IsSuccess = true;
            //InvokeResult();
        }
    }
}
