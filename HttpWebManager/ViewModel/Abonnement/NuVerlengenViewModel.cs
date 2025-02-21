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

namespace HttpWebManager
{
    [Telfort_Objects.LookupViewModel(ContractType = (int)Telfort_Objects.ContractType.Verlengen)]
    public class NuVerlengenViewModel : ScrapingBase
    {
        public NuVerlengenViewModel() : base() { }

        private Telfort_Objects.Contract GetContract()
        {
            Telfort_Objects.Contract _Contract = new Telfort_Objects.Contract();

            List<Telfort_Objects.Adres> _AdresList = new List<Telfort_Objects.Adres>();

            List<Telfort_Objects.AbonnementContract> _AbonnementContractList = new List<Telfort_Objects.AbonnementContract>();
            List<Telfort_Objects.Abonnement> _AbonnementList = new List<Telfort_Objects.Abonnement>();
            List<Telfort_Objects.AbonnementProduct> _AbonnementProductList = new List<Telfort_Objects.AbonnementProduct>();

            Telfort_Objects.Klant _Klant = new Telfort_Objects.Klant();
            _Klant.IDtype = "HolandDriverIC";
            _Klant.IDnummer = "4777277602";
            _Klant.Afgiftedatum = "18-03-2010".ToDateTime();
            _Klant.Vervaldatum = "18-03-2020".ToDateTime();
            _Klant.Dealercode = "1084005034";

            Telfort_Objects.Adres _Adres = new Telfort_Objects.Adres() { Postcode = "9693CR", Huisnummer = "8" };
            _AdresList.Add(_Adres);

            Telfort_Objects.AbonnementContract _AbonnementContract = new Telfort_Objects.AbonnementContract();
            _AbonnementContract.Type_Contract = (int)Telfort_Objects.ContractType.Verlengen;// "Verlengen";
            _AbonnementContract.Rekeningnummer = "4235284";
                        
            Telfort_Objects.Abonnement _Abonnement = new Telfort_Objects.Abonnement();
            _Abonnement.SimNr = "0907143033982";
            _Abonnement.MobileNr = "0649943925";
            _Abonnement.Startdatum = "15-1-2012".ToDateTime();
            _Abonnement.Contractperiode = "12".ToInt16();
            _Abonnement.AbonnementNaam = "Telfort sim only";
            _Abonnement.AbonnementId = "SIMOnly2010";

            Telfort_Objects.AbonnementProduct _AbonnementProduct1 = new Telfort_Objects.AbonnementProduct() { BundelId = "SO2010MR300BDVKD12" };
            _AbonnementProductList.Add(_AbonnementProduct1);
            Telfort_Objects.AbonnementProduct _AbonnementProduct2 = new Telfort_Objects.AbonnementProduct() { BundelId = "SO2010nationalSMSBundel100LKQMH12" };
            _AbonnementProductList.Add(_AbonnementProduct2);

            _Abonnement.AbonnementProductList = _AbonnementProductList;
            _AbonnementList.Add(_Abonnement);

            _AbonnementContract.AbonnementList = _AbonnementList;
            _AbonnementContractList.Add(_AbonnementContract);

            _Klant.AbonnementContractList = _AbonnementContractList;
            //_Klant.AdresList
            _Contract.Klant = _Klant;

            return _Contract;
            
        }

        public override void Run()
        {
            Inschieten();
        }

        private void Execute(Object sender, EventArgs e)
        {
            InitContract("Verlengen_Kroft.xml");
            NuVerlengen();
        }

        private void NuVerlengen()
        {
            Func<Telfort_Objects.Contract, HttpWebResult> _HttpWebResult = TelfortKlantViewModel.ContractNaarTelfortSturen;
            IAsyncResult ar = _HttpWebResult.BeginInvoke(DealerContract, null, null);
            this.HttpWebResult = _HttpWebResult.EndInvoke(ar);
        }
        
        internal void Inschieten()
        {
            string strRetentionResult = string.Empty;
            DateTime dtEndDate = DateTime.MinValue;

            InitControlsBinding();
            InvokeBezig();
                        
            string ThisMobileNumber = DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr;
            string SIM = DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr;
            string BankRekeningNr = DealerContract.Klant.AbonnementContractList.First().Rekeningnummer;

            this.HttpWebResult = new HttpWebResult();
            LoginInfo _LoginInfo = new LoginInfo();
            //string strLoginUserName = _LoginInfo.LoginName, strLoginPassword = _LoginInfo.Password;

            string strLoginUserName = "108400503401", strLoginPassword = "9ExGmMtj1";
            //unitedconsumers
            //108400503401
            //9ExGmMtj1

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
            //    DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementProductList
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

            //    //,,,0001,0001.0101;,,,0001.0101,0001.0101.0003;,,,0001,0001.0102;,,,0001.0102,0001.0102.0002;,,,0001,0001.0103;,,,0001.0103,0001.0103.0002;

            //    //,,,0001,0001.0101;,,,0001.0101,0001.0101.0003;,,,0001,0001.0102;,,,0001.0102,0001.0102.0002;

            //    //,,,0001,0001.0101;,,,0001.0101,0001.0101.0003;
            //}

            //string expiryDate = "10-03-2014";

            string gekozenProduct = DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId;// "SIMOnly2010"; //<------------------------------------------------Mapping!!!!!!!
            string gekozenProductPeriode = DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode.ToString(); // "12"; //<------------------------------------------------Mapping!!!!!!!


            string eindDatum = "";
            string authKey = "", OperID = "";
            //
            string strResponseData = string.Empty, strErrorMessage = string.Empty;
            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            CookieContainer cookies = null;
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

                        if (Domain.ScrapeError(strResponseData))
                            goto FinischWithError;

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
                        dictPostData.Clear();
                        dictPostData.Add("firstQuickLoginType", "1");
                        dictPostData.Add("firstLoginNumber", ThisMobileNumber);
                        dictPostData.Add("firstLoginType", "");
                        if (string.IsNullOrEmpty(SIM))
                        {
                            dictPostData.Add("secondQuickLoginType", "5"); //[{"label":"KvK nummer","value":"3"},{"label":"Bedrijfsnaam","value":"4"},{"label":"Bankrekeningnummer","value":"5"},{"label":"SIM kaart nummer","value":"6"},{"label":"Geboortedatum","value":"7"}]
                            dictPostData.Add("secondLoginNumber", BankRekeningNr);
                        }
                        else
                        {
                            dictPostData.Add("secondQuickLoginType", "6"); //[{"label":"KvK nummer","value":"3"},{"label":"Bedrijfsnaam","value":"4"},{"label":"Bankrekeningnummer","value":"5"},{"label":"SIM kaart nummer","value":"6"},{"label":"Geboortedatum","value":"7"}]
                            dictPostData.Add("secondLoginNumber", SIM);
                        }
                        dictPostData.Add("secondLoginType", "");

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/cc_common/commonLoginAction.do?method=showCustomerList4Dealer"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        if (!strResponseData.Contains("1 klant/order gevonden"))
                        {
                            //goto FinischWithError;
                        }
                        //1 klant/order gevonden
                        //WriteBreakAndLine(strResponseData);
                        string strCustomerId = ScrapeHelper.ExtractValue(strResponseData, "loginBy('", "'");

                        strResponseData = ScrapeHelper.DoGetPage(
                           new Uri(string.Format("https://b2b.telfort.nl/custcare/cc_common/CustomerInfoAction.do?act=qryCustomerInfo&ONLYLOGIN=onlyLogin&&loginLevel=1&checkPassword=1&authType=AuthTypeCustID&objectID={0}&loginMsisdn={1}", strCustomerId, ThisMobileNumber)),
                           cookies,
                           "",
                           "",
                           certificate
                           );
                        //WriteBreakAndLine(strResponseData);
                        strResponseData = ScrapeHelper.DoGetPage(
                            new Uri("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/showCustomerInfo.do?method=initForLogin"),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //WriteBreakAndLine(strResponseData);
                        string strSubscriberId = ScrapeHelper.ExtractValue(strResponseData, "top.subsID4IPCC = \"", "\"");

                        //Klantgegevens

                        strResponseData = ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/personCustomerInfoAction.do?method=showPersonInfo&showType=showDetail&custType=PersonCustomer&selectedCustId={0}&msisdns={1}&fromloginflag=true", strCustomerId, ThisMobileNumber)),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //WriteBreakAndLine(strResponseData);
                        string org_apache_struts_taglib_html_TOKEN = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"org.apache.struts.taglib.html.TOKEN\" value=\"", "\"");

                        string strSegment = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_customerLevel\"", "</select>");
                        strSegment = ScrapeHelper.ExtractValue(strSegment, "selected=\"selected\">", "</option>");

                        //html strippen //string PUK2 = ScrapeHelper.ExtractValue(strResponseData, "", "");
                        strResponseData = strResponseData.RemoveSpaceAndBreak();
                        string Voorletters = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_initials\" value=\"", "\" id=\"cust_initials\" style=\"width: 100%;\" class=\"readonly\">");
                        string Tussenvoegsel = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_middleName\" value=\"", "\" id=\"cust_middleName\" style=\"width: 100%;\" class=\"readonly\">");
                        string Geboortenaam = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_nameOfBirth\" value=\"", "\" id=\"cust_nameOfBirth\" style=\"width: 100%\" class=\"readonly\">");
                        string Middlenamebirth = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_middlename_brith\" value=\"", "\" id=\"middleNameBirth\" style=\"width: 100%\" class=\"readonly\">");
                        string Achternaam = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_lastname\" value=\"", "\" id=\"cust_lastname\" style=\"width: 100%\" class=\"readonly\">");

                        string Geslacht = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_gender\"", "</select>");
                        Geslacht = ScrapeHelper.ExtractValue(Geslacht, "selected=\"selected\">", "</option>");
                        Geslacht = Geslacht == "Vrouw" ? "0" : "1";

                        string Telefoonnummer = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_contactTel\" maxlength=\"10\" value=\"", "\" onblur=\"validateContactTel(this);\" id=\"cust_contactTel\" style=\"width: 100%\">");
                        string Emailadres = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_emailAddress\" value=\"", "\" onblur=\"chkemail(this)\" id=\"cust_emailAddress\" style=\"width: 100%\">");
                        string Geboortedatum = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_dateOfBirth\" value=\"", "\" id=\"cust_dateOfBirth\" style=\"width: 100%;color:#CCC;\" class=\"Wdate\" title=\"dd-MM-yyyy\">");
                        string Klantenservicewachtwoord = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_servicePassword\" value=\"", "\" readonly=\"readonly\" id=\"acct_servicePassword\" style=\"width: 100%; color:red;font-weight: bold;\" class=\"readonly\">");

                        string Status = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_status\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_KlantStatus>(Status);
                        Status = ScrapeHelper.ExtractValue(Status, "selected=\"selected\">", "</option>");
                        //Status = Domain.GetComboxValue<Lookup_KlantStatus>(Status);

                        string Klantnummer = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_customerCode\" value=\"", "\" disabled=\"disabled\" id=\"acct_customerCode\" style=\"width: 100%\">");
                        string Dealercode = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_dealerCode\" value=\"", "\" disabled=\"disabled\" id=\"acct_dealerCode\" style=\"width: 100%\">");
                        string Registratiedatum = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_registeredDate\" value=\"", "\" disabled=\"disabled\" id=\"acct_registeredDate\" style=\"width: 100%\">");

                        string Bedrijfsvorm = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_customerType\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_Bedrijfsvorm>(Bedrijfsvorm);
                        Bedrijfsvorm = ScrapeHelper.ExtractValue(Bedrijfsvorm, "selected=\"selected\">", "</option>");
                        //Bedrijfsvorm = Domain.GetComboxValue<Lookup_Bedrijfsvorm>(Bedrijfsvorm);

                        string Klantniveau = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_customerLevel\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_Klantniveau>(Klantniveau);
                        Klantniveau = ScrapeHelper.ExtractValue(Klantniveau, "selected=\"selected\">", "</option>");
                        //Klantniveau = Domain.GetComboxValue<Lookup_Klantniveau>(Klantniveau);

                        string IDtype = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_certificate\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_IDtype>(IDtype);
                        IDtype = ScrapeHelper.ExtractValue(IDtype, "selected=\"selected\">", "</option>");
                        //IDtype = Domain.GetComboxValue<Lookup_IDtype>(IDtype);

                        string Afgegevenin = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_issuedIn\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_Afgegevenin>(Afgegevenin);
                        Afgegevenin = ScrapeHelper.ExtractValue(Afgegevenin, "selected=\"selected\">", "</option>");
                        //Afgegevenin = Domain.GetComboxValue<Lookup_Afgegevenin>(Afgegevenin);

                        string IDnummer = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_idNumber\" maxlength=\"31\" value=\"", "\" onblur=");
                        string Afgiftedatum = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_issueDate\" value=\"", "\" onfocus=");
                        string Vervaldatum = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_expirationDate\" value=\"", "\" onfocus=");

                        string Nationaliteit = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_nationality\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_Nationaliteit>(Nationaliteit);
                        Nationaliteit = ScrapeHelper.ExtractValue(Nationaliteit, "selected=\"selected\">", "</option>");
                        //Nationaliteit = Domain.GetComboxValue<Lookup_Nationaliteit>(Nationaliteit);

                        string Contactadres_Straat = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_street\" value=\"", "\" onblur=");
                        string Contactadres_Huisnummer = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_house\" maxlength=\"5\" value=\"", "\" onchange=");
                        string Contactadres_Toevoeging = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_suffix\" maxlength=\"6\" value=\"", "\" onblur=");
                        string Contactadres_Postcode = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_postcode\" maxlength=\"6\" value=\"", "\" onchange=");
                        string Contactadres_Plaats = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_place\" value=\"", "\" onblur=");
                        string Postadres_Straat = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_street\" value=\"", "\" onblur=");
                        string Postadres_Huisnummer = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_house\" maxlength=\"5\" value=\"", "\" onchange=");
                        string Postadres_Toevoeging = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_suffix\" maxlength=\"6\" value=\"", "\" onblur=");
                        string Postadres_Postcode = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_postcode\" maxlength=\"6\" value=\"", "\" onchange=");
                        string Postadres_Plaats = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_place\" value=\"", "\" onblur=");

                        string Postadres_Adres_status = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"post_status\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_Postadres_Adres_status>(Postadres_Adres_status);
                        Postadres_Adres_status = ScrapeHelper.ExtractValue(Postadres_Adres_status, "selected=\"selected\">", "</option>");
                        //Postadres_Adres_status = Domain.GetComboxValue<Lookup_Postadres_Adres_status>(Postadres_Adres_status);

                        string Postadres_is_contractadres = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"checkbox\" name=\"post_AddrSameAsContractAddr\" value=\"on\" checked=\"", "\" onclick=");
                        Postadres_is_contractadres = Postadres_is_contractadres == "checked" ? "true" : "false";

                        string Betaalwijze = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"pay_PaymentType\"", "</select>");
                        //Domain.InitComboBoxValues<Lookup_Betaalwijze>(Betaalwijze);
                        Betaalwijze = ScrapeHelper.ExtractValue(Betaalwijze, "selected=\"selected\">", "</option>");
                        //Betaalwijze = Domain.GetComboxValue<Lookup_Betaalwijze>(Betaalwijze);

                        string Rekeningnummer = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"pay_Rekeningnummer\" maxlength=\"9\" value=\"", "\" onkeypress=");

                        // Aansluiten - producten

                        strResponseData = ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/showSubscriberInfo.do?method=showSubInfoDetail&income=detail&act=productinfo&subscriberID={0}&fromloginflag=true&selectedCustId={1}", strSubscriberId, strCustomerId)),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                        //WriteBreakAndLine(strResponseData);
                        //html strippen //string PUK2 = ScrapeHelper.ExtractValue(strResponseData, "", "");
                        strResponseData = strResponseData.RemoveSpaceAndBreak();
                        string TelfortID = ScrapeHelper.ExtractValue(strResponseData, "<td class=\"labelProductInfo\">Telfort ID</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Huidige status");
                        string Huidige_status = ScrapeHelper.ExtractValue(strResponseData, "Huidige status</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Type aansluiting");
                        string Type_aansluiting = ScrapeHelper.ExtractValue(strResponseData, "Type aansluiting</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Product");
                        string Product = ScrapeHelper.ExtractValue(strResponseData, "Product</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Bundel");
                        string Bundel = ScrapeHelper.ExtractValue(strResponseData, "Bundel</td><td class=\"value\">", "&nbsp;</td></tr></table></div></div><!-- Product Info End -->");
                        string Contractperiode = ScrapeHelper.ExtractValue(strResponseData, "Contractperiode</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Startdatum");
                        string Startdatum = ScrapeHelper.ExtractValue(strResponseData, "Startdatum</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Einddatum");
                        string Einddatum = ScrapeHelper.ExtractValue(strResponseData, "Einddatum</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Afkoopsom");
                        string Afkoopsom = ScrapeHelper.ExtractValue(strResponseData, "Afkoopsom(€)<sup>incl. BTW</sup></td><td class=\"value\">", "&nbsp;</td></tr><tr><td colspan=\"2\">&nbsp;</td></tr><tr><td colspan=\"2\">&nbsp;</td></tr><tr><td colspan=\"2\"><div style=\"float:right\" >");


                        // knopje "Contract Verlengen"
                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=init&act=first&rectype=ContractRetention&ClearSession=true&sourceURL=null&custID4IPCC=3101005851561&subsID4IPCC=3101006127365&msisdn4IPCC=0619254007&random=0.23223304057735444

                        strResponseData = ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=init&act=first&rectype=ContractRetention&ClearSession=true&sourceURL=null&custID4IPCC={0}&subsID4IPCC={1}&msisdn4IPCC={2}&random=0.{3}", strCustomerId, strSubscriberId, ThisMobileNumber, Ult.CreateRandomString(16))),
                            cookies,
                            "",
                            "",
                            certificate
                            );

                        dictPostData.Clear();
                        //dictPostData.Add("date", "Fri%20Oct%2028%2012:23:33%20UTC+0200%202011");
                        string dt = new DateTime().ToString();
                        dictPostData.Add("date", dt);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/boss/heartThrob.jsp?date=" + dt),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        strResponseData = ScrapeHelper.DoGetPage(
                           new Uri("https://b2b.telfort.nl/custcare/cc_common/CustomerInfoAction.do?act=setReceptionToken"),
                           cookies,
                           "",
                           "",
                           certificate
                           );

                        dictPostData.Clear();
                        dictPostData.Add("act", "refreshCustomerInfo");

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/cc_common/CustomerInfoAction.do?act=refreshCustomerInfo"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        strResponseData = ScrapeHelper.DoGetPage(
                           new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=init&act=normal&random=0.{0}&sourceURL=null&menuid=ContractRetention_WEB&receptionId=ContractRetention", Ult.CreateRandomString(16))),
                           cookies,
                           "",
                           "",
                           certificate
                           );
                        string acctId = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" property=\"acctId\" name=\"acctId\" id=\"acctId\" value=\"", "\"");

                        string dealerCode = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" id=\"dealerCode\" style=\"width: 90%\" maxlength=\"128\" onclick=\"showTree(this,this,'Org');\" value=\"", "\""); ;
                        string dealerId = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"dealerId\" value=\"", "\""); ;

                        /// Check plaats en straat - begin                        


                        //dictPostData.Clear();
                        //strResponseData = ScrapeHelper.DoPostPage(
                        //    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/getStreetPlaceAction.do?act=getStreetPlace&POSTCODE={0}&HOUSENUMBER={1}",
                        //        DealerContract.Klant.AdresList.First().Postcode, 
                        //        DealerContract.Klant.AdresList.First().Huisnummer)),
                        //    cookies,
                        //    strResponseData,
                        //    dictPostData,
                        //    "",
                        //    "",
                        //    certificate
                        //    );

                        ////strResponseData = strResponseData.RemoveSpaceAndBreak();
                        //string DataToRemove = ScrapeHelper.ExtractValue(strResponseData, "HTTP", "chunked");
                        //strResponseData = strResponseData.Replace(DataToRemove, "");

                        /// Check plaats en straat - end

                        //Contactadres_Postcode = string.IsNullOrEmpty(DealerContract.Klant.AdresList.First().Postcode)? Contactadres_Postcode : DealerContract.Klant.AdresList.First().Postcode;
                        //Contactadres_Huisnummer = string.IsNullOrEmpty(DealerContract.Klant.AdresList.First().Huisnummer)? Contactadres_Huisnummer : DealerContract.Klant.AdresList.First().Huisnummer;
                        //Contactadres_Straat = string.IsNullOrEmpty(DealerContract.Klant.AdresList.First().Straat) ? Contactadres_Straat : DealerContract.Klant.AdresList.First().Straat;
                        //Contactadres_Plaats = string.IsNullOrEmpty(DealerContract.Klant.AdresList.First().Plaats) ? Contactadres_Plaats : DealerContract.Klant.AdresList.First().Plaats;
                        Rekeningnummer = string.IsNullOrEmpty(DealerContract.Klant.AbonnementContractList.First().Rekeningnummer) ? Rekeningnummer : DealerContract.Klant.AbonnementContractList.First().Rekeningnummer;

                        Afgiftedatum = DealerContract.Klant.Afgiftedatum.ToShortDateString();
                        Vervaldatum = DealerContract.Klant.Vervaldatum.ToShortDateString();

                        dictPostData.Clear();
                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                        dictPostData.Add("custType", "PersonCustomer");
                        dictPostData.Add("custId", strCustomerId);
                        dictPostData.Add("linkManId", "");
                        dictPostData.Add("firstName", "");
                        dictPostData.Add("initials", DealerContract.Klant.Voorletters);
                        dictPostData.Add("middleName", "");
                        dictPostData.Add("birthName", DealerContract.Klant.Geboortenaam);
                        dictPostData.Add("gender", Geslacht);
                        dictPostData.Add("birthDate", DealerContract.Klant.Geboortedatum.ToShortDateString());
                        dictPostData.Add("lastName", DealerContract.Klant.Achternaam);
                        dictPostData.Add("custPostCode", DealerContract.Klant.AdresList.First().Postcode);
                        dictPostData.Add("custHouseNum", DealerContract.Klant.AdresList.First().Huisnummer);
                        dictPostData.Add("custSuffixNum", DealerContract.Klant.AdresList.First().Toevoeging);
                        dictPostData.Add("custStreet", DealerContract.Klant.AdresList.First().Straat);
                        dictPostData.Add("custPlace", DealerContract.Klant.AdresList.First().Plaats);
                        dictPostData.Add("nationality", DealerContract.Klant.Nationaliteit);
                        dictPostData.Add("contactTel", "");
                        dictPostData.Add("email", "");
                        dictPostData.Add("idType", DealerContract.Klant.IDtype);
                        dictPostData.Add("issueCountry", DealerContract.Klant.Afgegevenin);
                        dictPostData.Add("idNumber", DealerContract.Klant.IDnummer);
                        dictPostData.Add("issueDate", DealerContract.Klant.Afgiftedatum.ToShortDateString());
                        dictPostData.Add("expiryDate", DealerContract.Klant.Vervaldatum.ToShortDateString());
                        dictPostData.Add("dealerId", dealerId);
                        dictPostData.Add("dealerCode", DealerContract.Klant.Dealercode);
                        dictPostData.Add("diffCustAddrFlag", "true");
                        dictPostData.Add("acctPostCode", DealerContract.Klant.AdresList.First().Postcode);
                        dictPostData.Add("acctHouseNum", DealerContract.Klant.AdresList.First().Huisnummer);
                        dictPostData.Add("acctSuffixNum", DealerContract.Klant.AdresList.First().Toevoeging);
                        dictPostData.Add("acctStreet", DealerContract.Klant.AdresList.First().Straat);
                        dictPostData.Add("acctPlace", DealerContract.Klant.AdresList.First().Plaats);
                        dictPostData.Add("acctId", acctId);
                        dictPostData.Add("billFormat", DealerContract.Klant.FactuurType); //bfConDetail
                        dictPostData.Add("bankAccount", DealerContract.Klant.AbonnementContractList.First().Rekeningnummer);
                        dictPostData.Add("bankType", DealerContract.Klant.AbonnementContractList.First().Rekeningnummer.Length == 7 ? "Giro" : "Bank");
                        dictPostData.Add("payType", DealerContract.Klant.AbonnementContractList.First().Betaalwijze);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=saveCustInfo"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        dt = new DateTime().ToString();
                        dictPostData.Add("date", dt);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/boss/heartThrob.jsp?date=" + dt),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        dictPostData.Clear();
                        dictPostData.Add("act", "cacheData");
                        dictPostData.Add("execute", "get");
                        dictPostData.Add("random", "0." + Ult.CreateRandomString(16));
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/subscriberList.do"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        strResponseData = ScrapeHelper.DoGetPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/subscriberList.do?act=query&custId={0}&page=&rectype=ContractRetention&serviceNum={1}&random=0.{2}", strCustomerId, ThisMobileNumber, Ult.CreateRandomString(16))),
                        cookies,
                        "",
                        "",
                        certificate
                        );
                        //actie!!!!!!!!!!!!!!!!!!!!!!!!!!!!448
                        string dataPost = strResponseData;

                        if (strResponseData.Contains("total\":0}")) //The customer has no subscriber
                        {
                            HttpWebResult.IsSuccess = false;
                            HttpWebResult.ErrorMessage = "The customer has no subscriber";
                        }
                        else
                        {
                            string gekozenSMS = ScrapeHelper.ExtractValue(strResponseData, "\"smsBundel\":\"", "\""); //"10000";
                            string OldvoiceMinuteBundel = ScrapeHelper.ExtractValue(strResponseData, "\"voiceMinuteBundel\":\"", "\""); //"30000";
                            string statusID = ScrapeHelper.ExtractValue(strResponseData, "\"statusID\":\"", "\""); //"US10";
                            string contractStartDate = ScrapeHelper.ExtractValue(strResponseData, "\"contractStartDate\":\"", "\""); //"01-10-2010";

                            string OldProdId = ScrapeHelper.ExtractValue(strResponseData, "\"proId\":\"", "\"");

                            string contractPeriod = ScrapeHelper.ExtractValue(strResponseData, "\"contractPeriod\":\"", "\""); //contractPeriod":"12"
                            string contractPeriodName = ScrapeHelper.ExtractValue(strResponseData, "\"contractPeriodName\":\"", "\""); //contractPeriodName":"12 maanden"
                            string contractPlanEndDate = ScrapeHelper.ExtractValue(strResponseData, "\"contractPlanEndDate\":\"", "\""); //contractPlanEndDate":"02-12-2011"
                            string dataBundel = ScrapeHelper.ExtractValue(strResponseData, "\"dataBundel\":\"", "\""); //dataBundel":"no"
                            string externalProdName = ScrapeHelper.ExtractValue(strResponseData, "\"externalProdName\":\"", "\""); //externalProdName":"Telfort sim only"
                            string mainProd = ScrapeHelper.ExtractValue(strResponseData, "\"mainProd\":\"", "\""); //mainProd":"Sim only 2008 MR"
                            string reisBundel = ScrapeHelper.ExtractValue(strResponseData, "\"reisBundel\":\"", "\""); //reisBundel":""
                            string subStatus = ScrapeHelper.ExtractValue(strResponseData, "\"subStatus\":\"", "\""); //subStatus":"Actief zonder barring"
                            string Status_Verlengen = ScrapeHelper.ExtractValue(strResponseData, "\"status\":\"", "\""); //subStatus":"Actief zonder barring"

                            dictPostData.Clear();
                            dictPostData.Add("method", "checkSubsValid");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("recType", "ContractRetention");
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=checkSubsValid&subsId={0}&recType=ContractRetention", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            //
                            if (strResponseData.Contains("Aansluiting verlengen pending"))
                            {
                                HttpWebResult.IsSuccess = false;
                                HttpWebResult.ErrorMessage = strResponseData;// "Transactie niet toegestaan, met als reden The subscriber has the Aansluiting verlengen pending order.";
                                goto FinischWithError;
                            }

                            dictPostData.Clear();
                            dictPostData.Add("method", "checkSubsValid");
                            dictPostData.Add("subsId", strSubscriberId);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=addSub&subsId={0}", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            dictPostData.Clear();
                            dictPostData.Add("method", "getContractPeriodByProductId");
                            dictPostData.Add("productId", OldProdId);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=getContractPeriodByProductId&productId={0}", OldProdId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //

                            dictPostData.Clear();
                            dictPostData.Add("method", "initBundelSizeInfo");
                            dictPostData.Add("subsId", strSubscriberId);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=initBundelSizeInfo&subsId={0}", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //
                            dictPostData.Clear();
                            dictPostData.Add("method", "updateSession");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("key", "product_ID");
                            dictPostData.Add("value", OldProdId);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=updateSession&subsId={0}&key=product_ID&value={1}", strSubscriberId, OldProdId)),
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
                            dictPostData.Add("method", "checkProduct");
                            dictPostData.Add("prodid", OldProdId);
                            dictPostData.Add("subsID", strSubscriberId);
                            dictPostData.Add("random", "0." + Random);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=checkProduct&prodid={0}&subsID={1}&random=0.{2}", OldProdId, strSubscriberId, Random)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //
                            dictPostData.Clear();
                            dictPostData.Add("method", "initSubsInfo4MainProduct");
                            dictPostData.Add("subsId", strSubscriberId);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=initSubsInfo4MainProduct&subsId={0}", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //
                            strResponseData = ScrapeHelper.DoGetPage(
                             new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=frmJsp&recChannel=bsacHal&catalogType=ROOT&productType=ProdType_Person&solutionID=&recType=ContractRetention&productID={0}&custType=PersonCustomer&subSeq={1}", OldProdId, strSubscriberId)),
                             cookies,
                             "",
                             "",
                             certificate
                             );
                            //
                            strResponseData = ScrapeHelper.DoGetPage(
                             new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=commonTree&recType=ContractRetention&catalogType=ROOT&productType=ProdType_Person&custType=PersonCustomer&recChannel=bsacHal&solutionID=&productID={0}&subs_relation_type=&main_or_subs_card=&DependOnID=&subSeq={1}", OldProdId, strSubscriberId)),
                             cookies,
                             "",
                             "",
                             certificate
                             );
                            //
                            //string gekozenProduct = "SIMOnly2010"; //<------------------------------------------------Mapping!!!!!!!
                            Random = Ult.CreateRandomString(16);
                            dictPostData.Clear();
                            dictPostData.Add("method", "checkProduct");
                            dictPostData.Add("prodid", gekozenProduct);
                            dictPostData.Add("subsID", strSubscriberId);
                            dictPostData.Add("random", "0." + Random);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=checkProduct&prodid={0}&subsID={1}&random=0.{2}", gekozenProduct, strSubscriberId, Random)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //476
                            dictPostData.Clear();
                            dictPostData.Add("method", "getContractPeriodByProductId");
                            dictPostData.Add("productId", gekozenProduct);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=getContractPeriodByProductId&productId={0}", gekozenProduct)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //477
                            dictPostData.Clear();
                            dictPostData.Add("method", "clearSubsInfo");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("Bundelflag", "0");

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=clearSubsInfo&subsId={0}&Bundelflag=0", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //478
                            dictPostData.Clear();
                            dictPostData.Add("method", "updateSession");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("key", "product_ID");
                            dictPostData.Add("value", gekozenProduct);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=updateSession&subsId={0}&key=product_ID&value={1}", strSubscriberId, gekozenProduct)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            // Aantal maanden selecteren begin
                            //
                            dictPostData.Clear();
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=clearSubsInfo&subsId={0}&Bundelflag=0", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            dictPostData.Clear();
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=updateSession&subsId={0}&key=contractPeriod&value={1}", strSubscriberId, gekozenProductPeriode)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //
                            // Aantal maanden selecteren end

                            //479
                            dictPostData.Clear();
                            dictPostData.Add("method", "initSubsInfo4VasProduct");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("productId", gekozenProduct);
                            dictPostData.Add("contractPeriod", gekozenProductPeriode);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=initSubsInfo4VasProduct&subsId={0}&productId={1}&contractPeriod={2}", strSubscriberId, gekozenProduct, gekozenProductPeriode)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //480
                            strResponseData = ScrapeHelper.DoGetPage(
                             new Uri("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=userBusiness"),
                             cookies,
                             "",
                             "",
                             certificate
                             );
                            //486
                            strResponseData = ScrapeHelper.DoGetPage(
                            new Uri("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=selectProduct&cmd=userBusiness&isHasCautioner=&isSimple=&curProcductID="),
                            cookies,
                            "",
                            "",
                            certificate
                            );

                            //???

                            //514
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

                            //520 bundel selecteren
                            //string GekozenSimOnly = "0001.0101.0002";
                            //string GekozenSmsBundel = "0001.0102";
                            //string GekozenSmsBundelOptie = "0001.0102.0002";
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
                            //string ids = ",,,0001,0001.0101;,,,0001.0101,0001.0101.0002;,,,0001,0001.0102;,,,0001.0102,0001.0102.0002;";
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
                            //534
                            dictPostData.Clear();
                            dictPostData.Add("method", "clearTempInfo4VasProduct");
                            dictPostData.Add("subsId", strSubscriberId);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=clearTempInfo4VasProduct&subsId={0}", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //535
                            dictPostData.Clear();
                            dictPostData.Add("method", "saveSubProductInfo");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("hasSelectTree", "1");

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=saveSubProductInfo&subsId={0}&hasSelectTree=1", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //537!!!!!!!!!!!!!!!!!!!!!!!!!!
                            dictPostData.Clear();
                            dictPostData.Add("method", "initSubsInfo4VasProduct");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("productId", gekozenProduct);
                            dictPostData.Add("contractPeriod", gekozenProductPeriode);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=initSubsInfo4VasProduct&subsId={0}&productId={1}&contractPeriod={2}", strSubscriberId, gekozenProduct, gekozenProductPeriode)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //538
                            dictPostData.Clear();
                            dictPostData.Add("method", "ajax");
                            dictPostData.Add("actionType", "selectProduct");
                            dictPostData.Add("cmd", "userBusiness");
                            dictPostData.Add("recType", "ContractRetention");
                            dictPostData.Add("catalogType", "ROOT");
                            dictPostData.Add("productType", "ProdType_Person");
                            dictPostData.Add("subsid", strSubscriberId);
                            dictPostData.Add("custType", "PersonCustomer");
                            dictPostData.Add("recChannel", "bsacHal");
                            dictPostData.Add("curProcductID", gekozenProduct);
                            dictPostData.Add("msisdn", ThisMobileNumber);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?method=ajax&actionType=selectProduct&cmd=userBusiness&recType=ContractRetention&catalogType=ROOT&productType=ProdType_Person&subsid={0}&custType=PersonCustomer&recChannel=bsacHal&curProcductID={1}&msisdn={2}", strSubscriberId, gekozenProduct, ThisMobileNumber)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            // 539
                            dictPostData.Clear();
                            dictPostData.Add("method", "clearTempInfo4VasProduct");
                            dictPostData.Add("subsId", strSubscriberId);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=clearTempInfo4VasProduct&subsId={0}", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //540
                            dictPostData.Clear();
                            dictPostData.Add("method", "checkSubsProduct");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("recType", "ContractRetention");
                            dictPostData.Add("productId", gekozenProduct);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=checkSubsProduct&subsId={0}&recType=ContractRetention&productId={1}", strSubscriberId, gekozenProduct)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //541
                            dictPostData.Clear();
                            dictPostData.Add("method", "initSubsInfo4VasProduct");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("productId", gekozenProduct);
                            dictPostData.Add("contractPeriod", gekozenProductPeriode);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=initSubsInfo4VasProduct&subsId={0}&productId={1}&contractPeriod={2}", strSubscriberId, gekozenProduct, gekozenProductPeriode)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //542
                            dictPostData.Clear();
                            dictPostData.Add("method", "setSubsProductInfoDefault");
                            dictPostData.Add("subsId", strSubscriberId);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=setSubsProductInfoDefault&subsId={0}", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            // 543
                            dictPostData.Clear();
                            dictPostData.Add("method", "clearTempInfo4VasProduct");
                            dictPostData.Add("subsId", strSubscriberId);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=setSubsProductInfoDefault&subsId={0}", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //544
                            dictPostData.Clear();
                            dictPostData.Add("method", "saveSubProductInfo");
                            dictPostData.Add("subsId", strSubscriberId);
                            dictPostData.Add("hasSelectTree", "0");

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=saveSubProductInfo&subsId={0}&hasSelectTree=0", strSubscriberId)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //545 - is dat een popup? moet nog even nakijken!!!
                            strResponseData = ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/changeProductAction.do?method=showProduct&random=0.{0}&ids={1}", Ult.CreateRandomString(16), strSubscriberId)),
                            cookies,
                            "",
                            "",
                            certificate
                            );
                            //567
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
                            //578
                            dataPost = Uri.EscapeUriString(dataPost).Replace(",", "%2C").Replace(":", "%3A");

                            Random = Ult.CreateRandomString(16);
                            dictPostData.Clear();
                            dictPostData.Add("act", "cacheData");
                            dictPostData.Add("execute", "save");
                            dictPostData.Add("dataStr", dataPost);
                            dictPostData.Add("random", "0." + Random);

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/subscriberList.do")),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            //572 prefinal - https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=next
                            dictPostData.Clear();
                            dictPostData.Add("custType", "PersonCustomer");
                            dictPostData.Add("custId", acctId);
                            dictPostData.Add("expiryDate", Vervaldatum);//
                            dictPostData.Add("txtSearchServiceNum", "");
                            dictPostData.Add("_SubsBox", strSubscriberId);
                            dictPostData.Add("Telfort sim only", gekozenProduct);
                            dictPostData.Add("SIMCardType", "");
                            dictPostData.Add("acctNo", acctId);
                            dictPostData.Add("actKey", "");
                            dictPostData.Add("airtimeExpiry", "");
                            dictPostData.Add("authenMode", "");
                            dictPostData.Add("balance", "");
                            dictPostData.Add("bedrijfsBundel", "");
                            dictPostData.Add("buitenlandBundel", "");
                            dictPostData.Add("buyOffFee", "");
                            dictPostData.Add("commitmentPrice", "");
                            dictPostData.Add("commitmentPriceTotal", "");
                            dictPostData.Add("contractEndDate", "");
                            dictPostData.Add("contractPeriod", gekozenProductPeriode);///
                            dictPostData.Add("contractPeriodName", contractPeriodName);
                            dictPostData.Add("contractPlanEndDate", contractPlanEndDate);
                            dictPostData.Add("contractStartDate", contractStartDate);
                            dictPostData.Add("dataBundel", dataBundel);
                            dictPostData.Add("deactKey", "");
                            dictPostData.Add("externalProdName", externalProdName);
                            dictPostData.Add("fobidBusinessList", "");
                            dictPostData.Add("iccid", "");
                            dictPostData.Add("imei", "");
                            dictPostData.Add("imsi", "");
                            dictPostData.Add("mainProd", mainProd); ///<---------------------------------------nakijken
                            dictPostData.Add("marketCode", "");
                            dictPostData.Add("minPeriod", "");
                            dictPostData.Add("selectedServiceNum", ThisMobileNumber);
                            dictPostData.Add("netType", "Mobile");
                            dictPostData.Add("notifyTemplate", "");
                            dictPostData.Add("onderBundel", "");
                            dictPostData.Add("payPlanList", "");
                            dictPostData.Add("penaltyRule", "");
                            dictPostData.Add("pricePlan", "");
                            dictPostData.Add("privilInfoList", "");
                            dictPostData.Add("proId", gekozenProduct);
                            dictPostData.Add("processMethod", "");
                            dictPostData.Add("prodType", "Postpaid");
                            dictPostData.Add("productList", "");
                            dictPostData.Add("registChannel", "");
                            dictPostData.Add("registDate", "");
                            dictPostData.Add("registOrganization", "");
                            dictPostData.Add("reisBundel", reisBundel);
                            dictPostData.Add("resourceList", "");
                            dictPostData.Add("serverInfoList", "");
                            dictPostData.Add("smsBundel", gekozenSMS);
                            dictPostData.Add("status", Status_Verlengen);
                            dictPostData.Add("statusDate", "");
                            dictPostData.Add("statusID", statusID);
                            dictPostData.Add("stopKey", "");
                            dictPostData.Add("selectedSubsId", strSubscriberId);
                            dictPostData.Add("subStatus", subStatus);
                            dictPostData.Add("subsName", "");
                            dictPostData.Add("telfortBundel", "");
                            dictPostData.Add("tempContractPeriod", OldProdId);
                            dictPostData.Add("tempProdName", "");
                            dictPostData.Add("userId", "");
                            dictPostData.Add("voiceMinuteBundel", OldvoiceMinuteBundel);
                            dictPostData.Add("zaGroup", "");

                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/contractRetentionAction.do?method=next")),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            org_apache_struts_taglib_html_TOKEN = ScrapeHelper.ExtractValue(strResponseData, "<input type='hidden' name='org.apache.struts.taglib.html.TOKEN' id=org.apache.struts.taglib.html.TOKEN' value='", "'"); ;
                            string paytype = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"radio\" name=\"paytype\" id=\"paytype\" class=\"INPUT_Border0\" value=\"", "\"");
                            string nextBillvalue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"nextBillvalue\" id=\"nextBillvalue\" value=\"", "\"");
                            string chkGetSignedContract = "1";
                            string formnum = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"formnum\" id=\"formnum\" value=\"", "\"");
                            string urlPath = "null";
                            string token = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"token\" id=\"token\" value=\"", "\"");

                            // refresh
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


                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=false&random=0.{0}", Ult.CreateRandomString(16))),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );

                            // PDF ophalen
                            strResponseData = ScrapeHelper.DoGetPage(
                               new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=printFra&printPriView=true&org.apache.struts.taglib.html.TOKEN={0}&paytype={1}&nextBillvalue={2}&preStep=Vorige&printPriBT=Contract%20tonen&formnum={3}&urlPath={4}&token={5}", org_apache_struts_taglib_html_TOKEN, paytype, nextBillvalue, formnum, urlPath, token)),
                               cookies,
                               "",
                               "",
                               certificate
                               );

                            ////
                            dictPostData.Clear();
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=true&random=0.{0}", Ult.CreateRandomString(16))),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );


                            strResponseData = ScrapeHelper.DoGetPage(
                               new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=proPrint")),
                               cookies,
                               "",
                               "",
                               certificate
                               );

                            bool Saved = false;
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

                                //// begin afronden // inschieten hier!!!
                                dictPostData.Clear();
                                strResponseData = ScrapeHelper.DoPostPage(
                                    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=signed&value=true&random=0.{0}", Ult.CreateRandomString(16))),
                                    cookies,
                                    strResponseData,
                                    dictPostData,
                                    "",
                                    "",
                                    certificate
                                    );
                                Thread.Sleep(500);
                                dictPostData.Clear();
                                strResponseData = ScrapeHelper.DoPostPage(
                                    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=get&random=0.{0}", Ult.CreateRandomString(16))),
                                    cookies,
                                    strResponseData,
                                    dictPostData,
                                    "",
                                    "",
                                    certificate
                                    );
                                Thread.Sleep(500);
                                dictPostData.Clear();
                                dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                                dictPostData.Add("paytype", paytype);
                                dictPostData.Add("nextBillvalue", nextBillvalue);
                                dictPostData.Add("chkGetSignedContract", chkGetSignedContract);
                                dictPostData.Add("formnum", formnum);
                                dictPostData.Add("urlPath", urlPath);
                                dictPostData.Add("token", token);

                                strResponseData = ScrapeHelper.DoPostPage(
                                    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=commitRequest")),
                                    cookies,
                                    strResponseData,
                                    dictPostData,
                                    "",
                                    "",
                                    certificate
                                    );

                                if (strResponseData.Contains("Order ID:" + formnum))
                                {
                                    HttpWebResult.IsSuccess = true;
                                }
                                else
                                {
                                    HttpWebResult.IsSuccess = false;
                                }

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

                        FinischWithSuccess:
                            HttpWebResult.IsSuccess = Saved;

                            ///--- eind pdf ophalen

                        }
                        //
                    }
                    else
                    {
                        strRetentionResult = strErrorMessage;
                    }
                }
                else
                {
                    strRetentionResult = "<span style=\"color: red;\">Er is geen certificaat gevonden</span>";
                }
            FinischWithError:
                HttpWebResult.IsSuccess = false;
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
        }
    }
}
