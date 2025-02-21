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
    // Vergelijk automatische Bundels van database met die van online versie, indien twee niet zelfde zijn, wordt oude bundels op "niet actief" gezet en nieuwe wordt bewaard.
    public class NetworkOperatorViewModel : ScrapingBase
    {
        private List<Lookup_Network_Operator> GetNetworkOperatorList()
        {
            List<Lookup_Network_Operator> _List = new List<Lookup_Network_Operator>();
            _List.Add(new Lookup_Network_Operator() { Name = "T-Mobile", Value = "BEN" });
            _List.Add(new Lookup_Network_Operator() { Name = "Ben PrePaid", Value = "BEPP" });
            _List.Add(new Lookup_Network_Operator() { Name = "Elephanttalk", Value = "ETMB" });
            _List.Add(new Lookup_Network_Operator() { Name = "KPN (Service Provider Mobiel)", Value = "GSM1" });
            _List.Add(new Lookup_Network_Operator() { Name = "Barablu Mobile", Value = "BMNL" });
            _List.Add(new Lookup_Network_Operator() { Name = "6GMOBILE netwerk", Value = "INMO" });
            _List.Add(new Lookup_Network_Operator() { Name = "Vodafone", Value = "LTEL" });
            _List.Add(new Lookup_Network_Operator() { Name = "Lyca", Value = "LYCA" });
            _List.Add(new Lookup_Network_Operator() { Name = "Tele2", Value = "TEL2" });
            _List.Add(new Lookup_Network_Operator() { Name = "Telfort", Value = "TLFM" });
            _List.Add(new Lookup_Network_Operator() { Name = "Teleena", Value = "TLNM" });
            return _List;
        }

        private Telfort_Objects.Contract GetContract()
        {
            Telfort_Objects.Contract _Contract = new Telfort_Objects.Contract();

            List<Telfort_Objects.Adres> _AdresList = new List<Telfort_Objects.Adres>();

            List<Telfort_Objects.AbonnementContract> _AbonnementContractList = new List<Telfort_Objects.AbonnementContract>();
            List<Telfort_Objects.Abonnement> _AbonnementList = new List<Telfort_Objects.Abonnement>();
            List<Telfort_Objects.AbonnementProduct> _AbonnementProductList = new List<Telfort_Objects.AbonnementProduct>();

            Telfort_Objects.Klant _Klant = new Telfort_Objects.Klant();
            _Klant.Voorletters = "W.J";
            _Klant.Middlenamebirth = "";
            _Klant.Tussenvoegsel = "";
            _Klant.Geboortenaam = "TAM";
            _Klant.Achternaam = "TAM";
            _Klant.Geslacht = 1;
            _Klant.Geboortedatum = "25-08-1972".ToDateTime();
            _Klant.Nationaliteit = "Netherlands";
            _Klant.IDtype = "HolandDriverIC";
            _Klant.IDnummer = "4478578108";
            _Klant.Afgegevenin = "NL";
            _Klant.Afgiftedatum = "18-03-2010".ToDateTime();
            _Klant.Vervaldatum = "18-03-2020".ToDateTime();
            _Klant.Dealercode = "Telecombinatie";
            //_Klant.TypeAccount = "actpNormal";
            //_Klant.FactuurType = "bfOnline";
            //_Klant.Betaalwijze = "sttpDirectDebit";

            Telfort_Objects.Adres _Adres = new Telfort_Objects.Adres() { Postcode = "2592VL", Huisnummer = "33", Straat = "Roggekamp", Plaats = "'S-GRAVENHAGE", Toevoeging = "" };
            _AdresList.Add(_Adres);

            Telfort_Objects.AbonnementContract _AbonnementContract = new Telfort_Objects.AbonnementContract();
            _AbonnementContract.Type_Contract = (int)Telfort_Objects.ContractType.Nieuw;// "Nieuw";
            _AbonnementContract.Rekeningnummer = "128693649";

            Telfort_Objects.Abonnement _Abonnement = new Telfort_Objects.Abonnement();
            _Abonnement.SimNr = "1006144039004";
            _Abonnement.MobileNr = "";
            _Abonnement.Startdatum = "30-1-2012".ToDateTime();
            _Abonnement.Contractperiode = "12".ToInt16();
            _Abonnement.AbonnementNaam = "Telfort sim only";
            _Abonnement.AbonnementId = "SIMOnly2010";
            _Abonnement.IsNummerPortering = true;
            //...


            Telfort_Objects.AbonnementProduct _AbonnementProduct1 = new Telfort_Objects.AbonnementProduct() { BundelId = "SO2010MR300BDVKD12" };
            _AbonnementProductList.Add(_AbonnementProduct1);
            Telfort_Objects.AbonnementProduct _AbonnementProduct2 = new Telfort_Objects.AbonnementProduct() { BundelId = "SO2010nationalSMSBundel100LKQMH12" };
            _AbonnementProductList.Add(_AbonnementProduct2);

            _Abonnement.AbonnementProductList = _AbonnementProductList;
            _AbonnementList.Add(_Abonnement);

            _AbonnementContract.AbonnementList = _AbonnementList;
            _AbonnementContractList.Add(_AbonnementContract);

            _Klant.AbonnementContractList = _AbonnementContractList;
            _Klant.AdresList = _AdresList;
            _Contract.Klant = _Klant;

            return _Contract;
        }

        public override void Run()
        {
            //
        }

        public static Telfort_Objects.Network_Operators Execute()
        {
            Domain.InitDBConnection();
            using (UnitOfWork session1 = new UnitOfWork())
            {
                return GetNetwork_Operators(session1);
            }
        }

        private static Telfort_Objects.Network_Operators GetNetwork_Operators(UnitOfWork session1)
        {
            Telfort_Objects.Network_Operators _Network_Operators = new Telfort_Objects.Network_Operators();
            var _Lookup_Network_OperatorList = new XPQuery<Lookup_Network_Operator>(session1).ToList();

            if (_Lookup_Network_OperatorList != null)
            {
                List<Telfort_Objects.Lookup_Network_Operator> _Network_OperatorList = new List<Telfort_Objects.Lookup_Network_Operator>();
                _Lookup_Network_OperatorList.ForEach(x => 
                {
                    Telfort_Objects.Lookup_Network_Operator _Lookup_Network_Operator = new Telfort_Objects.Lookup_Network_Operator();
                    AssemblyManager.ConvertObject(_Lookup_Network_Operator, x);

                    List<Telfort_Objects.Lookup_Service_Provider> _Lookup_Service_ProviderList = new List<Telfort_Objects.Lookup_Service_Provider>();
                    x.Lookup_Service_ProviderCollection.ToList().ForEach(y => 
                    {
                        Telfort_Objects.Lookup_Service_Provider _Lookup_Service_Provider = new Telfort_Objects.Lookup_Service_Provider();
                        AssemblyManager.ConvertObject(_Lookup_Service_Provider, y);
                        _Lookup_Service_ProviderList.Add(_Lookup_Service_Provider);
                    });
                    _Lookup_Network_Operator.Lookup_Service_ProviderList = _Lookup_Service_ProviderList;
                    _Network_OperatorList.Add(_Lookup_Network_Operator);
                });
                
                _Network_Operators.Lookup_Network_OperatorList = _Network_OperatorList;
            }

            return _Network_Operators;
        }

        private void Execute(Object sender, EventArgs e)
        {
            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            CookieContainer cookies = null;

            this.HttpWebResult = new HttpWebResult();
            LoginInfo _LoginInfo = new LoginInfo();
            string strLoginUserName = _LoginInfo.LoginName, strLoginPassword = _LoginInfo.Password;

            string strResponseData = string.Empty, strErrorMessage = string.Empty;
            string Mobiel = "";
            Sim = new LoginInfo().Sim;
            string SIM = Sim;
            
            Telfort_Objects.Contract _DealerContract = GetContract();

            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2();
            string AuthKey = "";
            if (1 == 1) //(Domain.DoLogin(ref AuthKey, ref strResponseData, ref strErrorMessage, ref certificate))
            {
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
                        //certificateOut = certificate;

                        //Klant_Telfort _Klant_Telfort = new Klant_Telfort();

                        strResponseData = ScrapeHelper.DoLogin(
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

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/boss/post.do"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        strResponseData = ScrapeHelper.DoGetPage(
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

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/ca/POST.do"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

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

                            if (strResponseData.Contains("Dit account is aangemeld op een andere locatie"))
                            {
                                strResponseData = ScrapeHelper.DoGetPage(
                                    new Uri("https://b2b.telfort.nl/boss/repeatLogin.do?actionFlag=login&loginPage=Frameset.jsp&loginRepeatFlag=1&operId=" + strLoginUserName),
                                    cookies,
                                    "",
                                    "",
                                    certificate
                                    );
                            }

                            strResponseData = ScrapeHelper.DoGetPage(
                                new Uri("https://b2b.telfort.nl/custcare/cc_common/commonLoginAction.do?method=initPage&loadmode=loginboss&lang=nl_NL"),
                                cookies,
                                "",
                                "",
                                certificate
                                );

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
                                }
                            }
                            ////------
                        }

                        // page 1 ophalen
                        dictPostData.Clear();
                        dictPostData.Add("act", "install");
                        dictPostData.Add("ClearSession", "true");
                        dictPostData.Add("sourceURL", "null");
                        dictPostData.Add("language", "nl_NL");
                        dictPostData.Add("custID4IPCC", "");
                        dictPostData.Add("subsID4IPCC", "");
                        dictPostData.Add("msisdn4IPCC", "");

                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/installPCAInfoAction.do?act=install&ClearSession=true&sourceURL=null&language=nl_NL&custID4IPCC=&subsID4IPCC=&msisdn4IPCC="),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        strResponseData = strResponseData.RemoveSpaceAndBreak();
                        // ComboBox values ophalen van page 1

                        //string SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name = \"SIMonly\" style=\"width: 100%\" id=\"SIMonly\"><option>Selecteer</option>", "</select>");
                        string org_apache_struts_taglib_html_TOKEN = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"org.apache.struts.taglib.html.TOKEN\" value=\"", "\"");

                        //XPCollection<Lookup_AboBundel> _Lookup_AboBundelList = new XPCollection<Lookup_AboBundel>(session1);

                        ////Domain.InitComboBoxValues<Lookup_TypeProduct>(SelectHTMLString);
                        //Domain.Init_Lookup_TypeProduct(SelectHTMLString);
                        // TUSSEN PROCESS
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
                        string Productkeuze = ((int)Lookup_AboBundel.ProductKeuze.SIMONLY).ToString();
                        dictPostData.Clear();
                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                        dictPostData.Add("initials", _DealerContract.Klant.Voorletters);
                        dictPostData.Add("middleNameBirth", _DealerContract.Klant.Middlenamebirth);
                        dictPostData.Add("birthName", _DealerContract.Klant.Geboortenaam);
                        dictPostData.Add("middleName", _DealerContract.Klant.Tussenvoegsel);
                        dictPostData.Add("lastName", _DealerContract.Klant.Achternaam);
                        dictPostData.Add("gender", _DealerContract.Klant.Geslacht.ToString());
                        dictPostData.Add("birthDate", _DealerContract.Klant.Geboortedatum.ToShortDateString());
                        dictPostData.Add("postCode", _DealerContract.Klant.AdresList.First().Postcode);
                        dictPostData.Add("houseNumber", _DealerContract.Klant.AdresList.First().Huisnummer);
                        dictPostData.Add("suffixNumber", _DealerContract.Klant.AdresList.First().Toevoeging);
                        dictPostData.Add("street", _DealerContract.Klant.AdresList.First().Straat);
                        dictPostData.Add("place", _DealerContract.Klant.AdresList.First().Plaats);
                        dictPostData.Add("nationality", _DealerContract.Klant.Nationaliteit);
                        dictPostData.Add("contactTelephone", "");
                        dictPostData.Add("email", _DealerContract.Klant.Emailadres);
                        dictPostData.Add("idType", _DealerContract.Klant.IDtype);
                        dictPostData.Add("countryOfIssue", _DealerContract.Klant.Afgegevenin);
                        dictPostData.Add("idNumber", _DealerContract.Klant.IDnummer);
                        dictPostData.Add("dateOfIssue", _DealerContract.Klant.Afgiftedatum.ToShortDateString());
                        dictPostData.Add("dateOfExpire", _DealerContract.Klant.Vervaldatum.ToShortDateString());
                        dictPostData.Add("dealerCode", _DealerContract.Klant.Dealercode);
                        dictPostData.Add("orgId", "");
                        dictPostData.Add("street2", "");
                        dictPostData.Add("place2", "");
                        dictPostData.Add("accountType", _DealerContract.Klant.TypeAccount);
                        dictPostData.Add("billFormat", _DealerContract.Klant.FactuurType);
                        dictPostData.Add("bankAccountNumber", _DealerContract.Klant.AbonnementContractList.First().Rekeningnummer);
                        dictPostData.Add("bankAccountType", _DealerContract.Klant.AbonnementContractList.First().Rekeningnummer.Length == 7 ? "Giro" : "Bank");
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

                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/installPCAInfoAction.do?act=nextPage&street=Roggekamp&place=%27S-GRAVENHAGE&useBillAddress=same&gender=1&nationality=Netherlands&idType=HolandDriverIC&countryOfIssue=NL&accountType=actpNormal&billFormat=bfOnline&paymentType=sttpDirectDebit&SIMonly=1&=&org.apache.struts.taglib.html.TOKEN=ec2b3de087437e6a1d67e5c2e4cded8f&initials=W.J.&middleNameBirth=&birthName=Tam&middleName=&lastName=Tam&birthDate=25-08-1972&postCode=2592VL&houseNumber=33&suffixNumber=&street=Roggekamp&place='S-GRAVENHAGE&contactTelephone=&email=&idNumber=4478578108&dateOfIssue=12-09-2007&dateOfExpire=12-09-2017&dealerCode=Telecombinatie&orgId=&postCode2=&houseNumber2=&suffixNumber2=&street2=&place2=&bankAccountNumber=128693649&bankAccountType=Bank&install=&needDecoder=true

                        dictPostData.Clear();
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/installPCAInfoAction.do?act=nextPage&street={0}&place={1}&useBillAddress=same&gender={2}&nationality={3}&idType={4}&countryOfIssue={5}&accountType={6}&billFormat={7}&paymentType={8}&SIMonly={9}&=&org.apache.struts.taglib.html.TOKEN={10}&initials={11}&middleNameBirth=&birthName={12}&middleName=&lastName={13}&birthDate={14}&postCode={15}&houseNumber={16}&suffixNumber=&street={17}&place={18}&contactTelephone=&email=&idNumber={19}&dateOfIssue={20}&dateOfExpire={21}&dealerCode={22}&orgId=&postCode2=&houseNumber2=&suffixNumber2=&street2=&place2=&bankAccountNumber={23}&bankAccountType={24}&install=&needDecoder=true",
                                _DealerContract.Klant.AdresList.First().Straat, _DealerContract.Klant.AdresList.First().Plaats, _DealerContract.Klant.Geslacht,
                                _DealerContract.Klant.Nationaliteit, _DealerContract.Klant.IDtype, _DealerContract.Klant.Afgegevenin,
                                _DealerContract.Klant.TypeAccount, _DealerContract.Klant.FactuurType, _DealerContract.Klant.Betaalwijze,
                                Productkeuze, org_apache_struts_taglib_html_TOKEN, _DealerContract.Klant.Voorletters, _DealerContract.Klant.Geboortenaam,
                                _DealerContract.Klant.Achternaam, _DealerContract.Klant.Geboortedatum, _DealerContract.Klant.AdresList.First().Postcode, _DealerContract.Klant.AdresList.First().Huisnummer,
                                _DealerContract.Klant.AdresList.First().Straat, _DealerContract.Klant.AdresList.First().Plaats, _DealerContract.Klant.IDnummer, _DealerContract.Klant.Afgiftedatum,
                                _DealerContract.Klant.Vervaldatum, _DealerContract.Klant.Dealercode, _DealerContract.Klant.AbonnementContractList.First().Rekeningnummer, _DealerContract.Klant.AbonnementContractList.First().Rekeningnummer.Length == 7 ? "Giro" : "Bank"
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
                        //Productkeuze = ((int)Lookup_AboBundel.ProductKeuze.SIMONLY).ToString();
                        dictPostData.Clear();
                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                        dictPostData.Add("initials", _DealerContract.Klant.Voorletters);
                        dictPostData.Add("middleNameBirth", _DealerContract.Klant.Middlenamebirth);
                        dictPostData.Add("birthName", _DealerContract.Klant.Geboortenaam);
                        dictPostData.Add("middleName", _DealerContract.Klant.Tussenvoegsel);
                        dictPostData.Add("lastName", _DealerContract.Klant.Achternaam);
                        dictPostData.Add("gender", _DealerContract.Klant.Geslacht.ToString());
                        dictPostData.Add("birthDate", _DealerContract.Klant.Geboortedatum.ToShortDateString());
                        dictPostData.Add("postCode", _DealerContract.Klant.AdresList.First().Postcode);
                        dictPostData.Add("houseNumber", _DealerContract.Klant.AdresList.First().Huisnummer);
                        dictPostData.Add("suffixNumber", _DealerContract.Klant.AdresList.First().Toevoeging);
                        dictPostData.Add("street", _DealerContract.Klant.AdresList.First().Straat);
                        dictPostData.Add("place", _DealerContract.Klant.AdresList.First().Plaats);
                        dictPostData.Add("nationality", _DealerContract.Klant.Nationaliteit);
                        dictPostData.Add("contactTelephone", "");
                        dictPostData.Add("email", _DealerContract.Klant.Emailadres);
                        dictPostData.Add("idType", _DealerContract.Klant.IDtype);
                        dictPostData.Add("countryOfIssue", _DealerContract.Klant.Afgegevenin);
                        dictPostData.Add("idNumber", _DealerContract.Klant.IDnummer);
                        dictPostData.Add("dateOfIssue", _DealerContract.Klant.Afgiftedatum.ToShortDateString());
                        dictPostData.Add("dateOfExpire", _DealerContract.Klant.Vervaldatum.ToShortDateString());
                        dictPostData.Add("dealerCode", _DealerContract.Klant.Dealercode);
                        dictPostData.Add("orgId", "");
                        dictPostData.Add("street2", "");
                        dictPostData.Add("place2", "");
                        dictPostData.Add("accountType", _DealerContract.Klant.TypeAccount);
                        dictPostData.Add("billFormat", _DealerContract.Klant.FactuurType);
                        dictPostData.Add("bankAccountNumber", _DealerContract.Klant.AbonnementContractList.First().Rekeningnummer);
                        dictPostData.Add("bankAccountType", _DealerContract.Klant.AbonnementContractList.First().Rekeningnummer.Length == 7 ? "Giro" : "Bank");
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

                        //popup - OPENEN MET  SIMonly = "1" select product
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=commonTree&recType=Install&catalogType=ROOT&productType=ProdType_Person&custType=PersonCustomer&recChannel=bsacHal&solutionID=&productID=&subs_relation_type=&main_or_subs_card=&DependOnID=&subSeq="),
                            cookies,
                            "",
                            "",
                            certificate
                            );

                        strResponseData = strResponseData.RemoveSpaceAndBreak();

                        //SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "d.add(\"ROOT\",\"\",\"Product\",\"\",\"_self\");", "d.oAll(true);").Replace("d.add(\"TlfPreFMRHSM", "").Replace("d.add(\"TlfPreFMRSOM", "").Replace(">Telfort prepaid", "").Replace(">Telfort Prepaid", "");

                        // XPCollection<Lookup_SubscriptionType> _stList = new XPCollection<Lookup_SubscriptionType>();

                        //Domain.InitRadioValues_Lookup_SubscriptionType(SelectHTMLString, ref _stList);

                        Thread.Sleep(1000);
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getNetType&productId={0}&productName={1}&RelationType=&StallType=0",
                            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam)),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        strResponseData = strResponseData.RemoveSpaceAndBreak();

                        string resType = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");
                        strResponseData = strResponseData.Replace("@_@" + resType, "");
                        string brandId = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");

                        // get phone nummers
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getMoreselectTelnums&productId={0}&brandId={1}&resType={2}",
                            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId, brandId, resType)),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        XPCollection<LookupBase> _PhoneList = new XPCollection<LookupBase>();
                        _PhoneList = Domain.ReturnComboBoxValues<LookupBase>(strResponseData);
                        //
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubsInfoForVasProduct&saveFlag=C&hidShowSubSeq=&contractPeriod={5}&productId={0}&hidProductName={1}&txtActivationDate={2}&txtMsisdn={3}&txtSsn={4}&txtImsi=&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false",
                            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId, _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum, _PhoneList.First().Value, SIM,
                            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode)),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        //Get network operator info begin

                        using (UnitOfWork session1 = new UnitOfWork())
                        {
                            //var Query = new XPQuery<Lookup_Network_Operator>(session1).ToList();
                            //if (Query.Count == 0)
                            //{
                            XPCollection<Lookup_Network_Operator> _Lookup_Network_OperatorXPOListDB = new XPCollection<Lookup_Network_Operator>(session1);

                                List<Lookup_Network_Operator> _Lookup_Network_OperatorList = GetNetworkOperatorList();

                                List<Telfort_Objects.Lookup_Network_Operator> _Lookup_Network_OperatorListTemp = new List<Telfort_Objects.Lookup_Network_Operator>();

                                _Lookup_Network_OperatorListTemp = Ult.ConvertTList<Telfort_Objects.Lookup_Network_Operator, Lookup_Network_Operator>(_Lookup_Network_OperatorList).ToList();

                                XPCollection<Lookup_Network_Operator> _Lookup_Network_OperatorXPOList = new XPCollection<Lookup_Network_Operator>(session1);
                                
                                _Lookup_Network_OperatorListTemp.ForEach(x =>
                                {
                                    Thread.Sleep(500);
                                    Lookup_Network_Operator _Lookup_Network_Operator = new Lookup_Network_Operator(session1) { Name = x.Name, Value = x.Value };
                                    dictPostData.Clear();
                                    strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                                    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryCurrentSPByCurrentCarrierId&CurrentCarrier={0}",
                                        x.Value)),
                                    cookies,
                                    strResponseData,
                                    dictPostData,
                                    "",
                                    "",
                                    certificate
                                    );
                                    XPCollection<Lookup_Service_Provider> _Lookup_Service_ProviderList = Domain.GetServiceProvider(strResponseData, session1);

                                    _Lookup_Network_Operator.Lookup_Service_ProviderCollection.AddRange(_Lookup_Service_ProviderList);

                                    _Lookup_Network_OperatorXPOList.Add(_Lookup_Network_Operator);
                                });

                                if (!IsZelfde(_Lookup_Network_OperatorXPOListDB, _Lookup_Network_OperatorXPOList))
                                {
                                    // oud data verwijderen en nieuwe data toevoegen
                                    session1.Delete(_Lookup_Network_OperatorXPOListDB);
                                    session1.CommitChanges();
                                }
                            //}
                        }

                        
                        HttpWebResult.IsSuccess = true;
                        //// einde
                    }
                }
                catch (Exception err)
                {
                    HttpWebResult.IsSuccess = false;
                    HttpWebResult.ErrorMessage = err.Message;
                }
                finally
                {
                    Domain.DoLogout(cookies, AuthKey, certificate);
                    InvokeResult();
                }
            }
            else
            {
                HttpWebResult.IsSuccess = false;
                HttpWebResult.ErrorMessage = "Login Failed.";
            }
        }

        private bool IsZelfde(XPCollection<Lookup_Network_Operator> _Lookup_Network_OperatorXPOListDB, XPCollection<Lookup_Network_Operator> _Lookup_Network_OperatorXPOList)
        {
            bool result = false;

            if (_Lookup_Network_OperatorXPOListDB.Count != _Lookup_Network_OperatorXPOList.Count)
                return false;

            _Lookup_Network_OperatorXPOList.ToList().ForEach(x => 
            {
                Lookup_Network_Operator _Lookup_Network_Operator = _Lookup_Network_OperatorXPOListDB.ToList()
                    .Where(z1 => z1.Name.ToLower().Equals(x.Name.ToLower()) 
                        && z1.Value.ToLower().Equals(x.Value.ToLower())).FirstOrDefault();
                if (_Lookup_Network_Operator == null)
                {
                    result = false; return;
                }

                x.Lookup_Service_ProviderCollection.ToList().ForEach(q => 
                {
                    if (_Lookup_Network_Operator.Lookup_Service_ProviderCollection
                        .Count(q1 => q1.Name.ToLower().Equals(q.Name.ToLower()) 
                            && q1.Value.ToLower().Equals(q.Value.ToLower())) < 1)
                    {
                        result = false; return;
                    }
                });
            });

            return result;
        }
    }
    
   
}
