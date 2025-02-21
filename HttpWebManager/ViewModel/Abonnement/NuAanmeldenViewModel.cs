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
using System.Runtime.Remoting.Messaging;

namespace HttpWebManager
{
    [Telfort_Objects.LookupViewModel(ContractType = (int)Telfort_Objects.ContractType.Nieuw)]
    public class NuAanmeldenViewModel : ScrapingBase
    {
        public NuAanmeldenViewModel() : base() { }

        private XPCollection<Lookup_Network_Operator> GetNetworkOperatorList(UnitOfWork session1)
        {
            XPCollection<Lookup_Network_Operator> _List = new XPCollection<Lookup_Network_Operator>(session1);
            _List.Add(new Lookup_Network_Operator(session1) { Name = "T-Mobile", Value = "BEN" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "Ben PrePaid", Value = "BEPP" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "Elephanttalk", Value = "ETMB" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "KPN (Service Provider Mobiel)", Value = "GSM1" });
            //_List.Add(new Lookup_Network_Operator() { Name = "6GMOBILE (TRING) overgenomen door SIMYO", Value = "INMO" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "Vodafone", Value = "LTEL" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "Lyca", Value = "LYCA" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "Tele2", Value = "TEL2" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "Telfort", Value = "TLFM" });
            _List.Add(new Lookup_Network_Operator(session1) { Name = "Teleena", Value = "TLNM" });
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
            _Abonnement.NP_TypeAansluiting = "1";
            _Abonnement.NP_TypeKlant = "0";
            _Abonnement.NP_MobielNr = "0640533244";
            _Abonnement.NP_SIM = "8931440000447169349";
            _Abonnement.NP_NetwerkOperator = "LTEL";
            _Abonnement.NP_ServiceProvider = "LIFN";
            _Abonnement.NP_EindDatum = "05-02-2012".ToDateTime();
            _Abonnement.NP_Wensdatum = "30-12-2011".ToDateTime();
            _Abonnement.NP_TypeKlant = "0"; //particulier
            _Abonnement.NP_TypeAansluiting = "1"; // postpaid
            _Abonnement.NP_Opzegtermijn = 1;

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

        private void Execute(Object sender, EventArgs e)
        {
            InitContract("Nieuw_Laanen.xml");
            NuAanmelden();
        }

        public override void Run()
        {
            Inschieten();
        }
        
        internal void NuAanmelden()
        {
            InvokeProcessContract();
        }
                
        internal void Inschieten()
        {
            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            CookieContainer cookies = null;

            bool Saved = false;
            this.HttpWebResult = new HttpWebResult();
            //LoginInfo _LoginInfo = new LoginInfo();

            string strLoginUserName = DealerContract.Klant.Gebruikersnaam, strLoginPassword = DealerContract.Klant.Wachtwoord;
            //unitedconsumers
            //108400503401
            //9ExGmMtj1
            
            string strResponseData = string.Empty, strErrorMessage = string.Empty;

            string SIM = "893126" + DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr;

            // bundel van deze contract samenstellen
            UserBundelChoiceViewModel _UserBundelChoiceViewModel = Using<UserBundelChoiceViewModel>(DealerContract);
            _UserBundelChoiceViewModel.BundelEvent += UserBundelChoiceViewModel.GetUserBundelChoiceCollection;            
            _UserBundelChoiceViewModel.GetContractBundel();

            string Privilege = UserBundelChoice.Privilege, AppendProduct = UserBundelChoice.AppendProduct, ids = UserBundelChoice.Ids;
            string GekozenSimOnly = UserBundelChoice.GekozenSimOnly, GekozenSmsBundel = UserBundelChoice.GekozenSmsBundel, GekozenSimOnly_GekozenSmsBundel = UserBundelChoice.GekozenSimOnly_GekozenSmsBundel, GekozenSmsBundelOptie = UserBundelChoice.GekozenSmsBundelOptie;
            string Surf_Mail = UserBundelChoice.Surf_Mail, Surf_Mail_Optie = UserBundelChoice.Surf_Mail_Optie, Surf_Mail_SimOnly = UserBundelChoice.Surf_Mail_SimOnly;
            
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
                            if (Domain.ScrapeError(strResponseData))
                                goto FinischWithError;

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

                        // Alle ComboValue controleren en updaten

                        string SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"gender\" id=\"gender\" style=\"width: 100% \">", "</select>");
                        Domain.InitComboBoxValues<Lookup_Geslacht>(SelectHTMLString);


                        // TUSSEN PROCESS
                        string dt = DateTime.Now.ToString();
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

                        //legitimatiebewijs check
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

                        strResponseData = strResponseData.RemoveSpaceAndBreak();
                        strResponseData = strResponseData.Replace("var activeDate = '';", "");
                        string activatieDate = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "activeDate = '", "'");

                        activatieDate = string.IsNullOrEmpty(activatieDate) ? DateTime.Today.AddDays(60).ToShortDateString() : activatieDate;

                        if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                        {
                            if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_TypeAansluiting.IntToBoolean()) // is postpaid?
                            {
                                //DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum = activatieDate.ToDateTime();
                            }
                            else
                            {
                                activatieDate = Ult.GetNummerPorteringPrepaidActivationDay().ToShortDateString();
                            }
                        }
                        else
                        {
                            activatieDate = DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString();
                        }

                        //https://b2b.telfort.nl/custcare/js/custsvc/basebusiness/installScript.jsp
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                          new Uri("https://b2b.telfort.nl/custcare/js/custsvc/basebusiness/installScript.jsp"),
                          cookies,
                          "",
                          "",
                          certificate
                          );
                        
                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                          new Uri("https://b2b.telfort.nl/custcare/js/custsvc/basebusiness/ReceptionHelper.jsp"),
                          cookies,
                          "",
                          "",
                          certificate
                          );
                        //

                        dt = DateTime.Now.ToString();
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
                        // result moet ongeveer 14 new Array(["1",""]) 0  zijn

                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                           new Uri("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=frmJsp&recType=Install&catalogType=ROOT&productType=ProdType_Person&custType=PersonCustomer&recChannel=bsacHal&subSeq="),
                           cookies,
                           "",
                           "",
                           certificate
                           );

                        ////???????

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

                        //maanden
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
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=unPickSelectTelnums&selectTel&unPickSelectTelnums=")),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        // get phone nummers
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getMoreselectTelnums&productId={0}&brandId={1}&resType={2}",
                            DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId, brandId, resType)),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        List<string> _PhoneList = new List<string>();

                        _PhoneList = Domain.ReturnPhoneListValues(strResponseData);

                        //get bundel page
                        DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr = _PhoneList[0];

                        //////////-------------------------
                        if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                        {
                            //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getNetWorkOperatorByTel&CurrentMsisdn=0650870015
                            dictPostData.Clear();
                            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getNetWorkOperatorByTel&CurrentMsisdn={0}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr)),
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
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryCurrentSPByCurrentCarrierId&CurrentCarrier={0}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_NetwerkOperator)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                            //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryNpSsnPrefixByCurrentCarrierId&CurrentCarrier=LTEL&currentSP=LIFN
                            dictPostData.Clear();
                            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryNpSsnPrefixByCurrentCarrierId&CurrentCarrier={0}&currentSP={1}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_NetwerkOperator,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_ServiceProvider)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                            //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryNpSsnPrefixByCurrentCarrierId&CurrentCarrier=LTEL&currentSP=LIFN
                            dictPostData.Clear();
                            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryNpSsnPrefixByCurrentCarrierId&CurrentCarrier={0}&currentSP={1}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_NetwerkOperator,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_ServiceProvider)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                            //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getWishDate&noticePeriod=1&ContractDate=31-12-2011&subscriberType=1
                            dictPostData.Clear();
                            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getWishDate&noticePeriod={0}&ContractDate={1}&subscriberType={2}",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_Opzegtermijn.ToString(),
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_EindDatum.ToShortDateString(),
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_TypeAansluiting)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        }
                        //////////-------------------------
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActivationDateValid&ActivationDate={0}",
                            activatieDate)),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                        {
                            dictPostData.Clear();
                            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActivationDateValid&ActivationDate={0}",
                                activatieDate)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        }
                        //
                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubsInfoForVasProduct&saveFlag=C&hidShowSubSeq=&contractPeriod=12&productId=SIMOnly2010&hidProductName=Telfort%20sim%20only&txtActivationDate=28-02-2012&txtMsisdn=0633089035&txtSsn=8931261109052073344&txtImsi=&premiumSMS=0&premiumDeBlock=0&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false
                        dictPostData.Clear();
                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubsInfoForVasProduct&saveFlag=C&hidShowSubSeq=&contractPeriod={0}&productId={1}&hidProductName={2}&txtActivationDate={3}&txtMsisdn={4}&txtSsn={5}&txtImsi=&premiumSMS=0&premiumDeBlock=0&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false",
                            DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                            DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId, DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                            activatieDate, _PhoneList[0], SIM
                            )),
                        cookies,
                        strResponseData,
                        dictPostData,
                        "",
                        "",
                        certificate
                        );

                        //dictPostData.Clear();
                        //strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                        //new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubsInfoForVasProduct&saveFlag=C&hidShowSubSeq=&contractPeriod={0}&productId={1}&hidProductName={2}&txtActivationDate={3}&txtMsisdn={4}&txtSsn={5}&txtImsi=&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false",
                        //    DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId, DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                        //    activatieDate, _PhoneList[0], SIM,
                        //    DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode)),
                        //cookies,
                        //strResponseData,
                        //dictPostData,
                        //"",
                        //"",
                        //certificate
                        //);

                        //https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=select&isHasCautioner=&curProcductID=SIMOnly2010
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
                        // begin bundel kiezen & save

                        dt = DateTime.Now.ToString();
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
                        dictPostData.Clear();
                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=prodRecCheck
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=prodRecCheck")),
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

                        Thread.Sleep(5000);
                        // Result tonen begin
                        //
                        dictPostData.Clear();
                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getVasProductList&curProcductID=SIMOnly2010&updateFlag=undefined&hidShowSubSeq=&contractPeriod=12&productId=SIMOnly2010&hidProductName=Telfort%20sim%20only&txtActivationDate=28-02-2012&txtMsisdn=0633089035&txtSsn=8931261109052073344&txtImsi=&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false&saveFlag=C&isImport=undefined

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getVasProductList&curProcductID={0}&updateFlag=undefined&hidShowSubSeq=&contractPeriod={1}&productId={2}&hidProductName={3}&txtActivationDate={4}&txtMsisdn={5}&txtSsn={6}&txtImsi=&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false&saveFlag=C&isImport=undefined",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                                activatieDate,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr, SIM)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        // Result tonen end

                        //
                        dictPostData.Clear();

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActivationDateValid&ActivationDate={0}", activatieDate)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        // BEGIN OPSLAAN 121

                        string txtImsi = "";

                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkNoCard&phoneNum=0619787137&productId=SIMOnly2010&hasLockOrNot=1&oldsimNum=&simNum=8931261006144039004&selectMsisdnType=1
                        dictPostData.Clear();
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkNoCard&phoneNum={0}&productId={1}&hasLockOrNot=1&oldsimNum=&simNum={2}&selectMsisdnType=1",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                SIM)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        txtImsi = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "\"],[\"2\",\"", "\"");
                        //
                        dictPostData.Clear();

                        _PhoneList.RemoveAt(0);
                        string unPickSelectTelnums = string.Join(",", _PhoneList.ToArray()) + ",";

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=unPickSelectTelnums&selectTel&unPickSelectTelnums={0}",
                                unPickSelectTelnums)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        dictPostData.Clear();
                        string _Random = Ult.CreateRandomString(16);
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=false&random=0.{0}",
                                _Random)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        Thread.Sleep(1000);
                        //
                        dictPostData.Clear();
                        _Random = Ult.CreateRandomString(16);
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=signed&value=false&random=0.{0}",
                                _Random)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //Nummerportering begin
                        string NP_SIMPrefix = "";
                        if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                        {
                            // NP Prefix
                            dictPostData.Clear();
                            strResponseData = ScrapeHelper.DoPostPage(
                               new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryNpSsnPrefixByCurrentCarrierId&CurrentCarrier={0}&currentSP={1}",
                                   DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_NetwerkOperator,
                                   DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_ServiceProvider)),
                               cookies,
                               strResponseData,
                               dictPostData,
                               "",
                               "",
                               certificate
                               );
                            NP_SIMPrefix = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "\",\"", "@_@");
                        }

                        //Nummerportering end

                        //


                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubInfo&saveFlag=C&hidShowSubSeq=&contractPeriod=24&productId=SIMOnly2010&hidProductName=Telfort%20sim%20only&txtActivationDate=07-02-2012&txtMsisdn=0619787137&txtSsn=8931261006144039004&txtImsi=204121900864059&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=true&simType=&simTypeDesc=undefined
                        dictPostData.Clear();

                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN); //c046b31f1763bf41e0ef39925bc8b133
                        dictPostData.Add("hidShowSubSeq", "");
                        dictPostData.Add("hidProductId", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                        dictPostData.Add("hidProductName", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam);
                        dictPostData.Add("hidNetType", GSM);
                        dictPostData.Add("hidPrdBrand", brandId);
                        dictPostData.Add("hidPhoneResType", resType);
                        dictPostData.Add("hidPaymodeType", "");
                        dictPostData.Add("chkGetVasProdFlag", "on");
                        dictPostData.Add("hidTxtImsi", txtImsi);
                        dictPostData.Add("hidOldMisidn", "");
                        dictPostData.Add("hidOldIccid", "");
                        dictPostData.Add("hidNpSsnPrefix", NP_SIMPrefix);
                        if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                        {
                            dictPostData.Add("currentCarrier", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_NetwerkOperator);//
                        }
                        dictPostData.Add("ssnPrefix", "893126");
                        dictPostData.Add("errorMsg", "");
                        dictPostData.Add("productId", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                        dictPostData.Add("contractPeriod", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode.ToString());
                        dictPostData.Add("txtMsisdn", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                        dictPostData.Add("selectTelNum", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                        dictPostData.Add("ssnSuffix", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr);
                        dictPostData.Add("txtSsn", "893126" + DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr);
                        dictPostData.Add("numberConcealed", "false");
                        dictPostData.Add("numberInclusion", "NN");
                        dictPostData.Add("canDoCustRec", "on");
                        dictPostData.Add("imei", "");
                        dictPostData.Add("setPremium", "0");
                        if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                        {
                            dictPostData.Add("chkNP", "on");//
                            dictPostData.Add("txtNPCurrentMsisdn", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_MobielNr);//
                            dictPostData.Add("currentSp", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_ServiceProvider);//
                            dictPostData.Add("customerType", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_TypeKlant);//
                            dictPostData.Add("txtSsnCustomerNo", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_SIM.StartsWith(NP_SIMPrefix) ? DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_SIM.Replace(NP_SIMPrefix, "") : DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_SIM);//
                            dictPostData.Add("subscriberType", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_TypeAansluiting);//
                            dictPostData.Add("txtContractDate", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_EindDatum.ToShortDateString());//
                        }
                        
                        dictPostData.Add("oldIndex", "");
                        if (DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                        {
                            dictPostData.Add("txtNoticePeriod", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_Opzegtermijn.ToString());//
                            dictPostData.Add("txtWishDate", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_Wensdatum.ToShortDateString());
                        }

                        dictPostData.Add("hiddenWishDate", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering ? DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_Wensdatum.ToShortDateString() : "");
                        dictPostData.Add("hiddenMaxWishDate", activatieDate);

                        //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubInfo&saveFlag=C&hidShowSubSeq=&contractPeriod=24&productId=SIMOnly2010&hidProductName=Telfort%20sim%20only&txtActivationDate=27-12-2011&txtMsisdn=0619657199&txtSsn=8931261108105030079&txtImsi=204127000312058&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=true&simType=&simTypeDesc=undefined&premiumSMS=0&premiumDeBlock=0

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubInfo&saveFlag=C&hidShowSubSeq=&contractPeriod={0}&productId={1}&hidProductName={2}&txtActivationDate={3}&txtMsisdn={4}&txtSsn={5}&txtImsi={6}&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=true&simType=&simTypeDesc=undefined&premiumSMS=0&premiumDeBlock=0",
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                                activatieDate,
                                DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr,
                                SIM,
                                txtImsi)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //string Startdatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"txtActivationDate\" maxlength=\"50\" value=\"", "\"");

                        //DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum = Startdatum.ToDateTime();

                        ///https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=next

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
                        dictPostData.Add("hidTxtImsi", txtImsi);
                        dictPostData.Add("hidOldMisidn", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                        dictPostData.Add("hidOldIccid", "893126" + DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr);
                        dictPostData.Add("hidNpSsnPrefix", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering ? NP_SIMPrefix : "");
                        dictPostData.Add("ssnPrefix", "893126");
                        dictPostData.Add("errorMsg", "");
                        dictPostData.Add("mainProductID", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                        dictPostData.Add("serviceNumber", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                        dictPostData.Add("productId", DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering ? DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId : "");
                        dictPostData.Add("oldIndex", "");
                        dictPostData.Add("hiddenWishDate", "");
                        dictPostData.Add("hiddenMaxWishDate", activatieDate);

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=next")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );
                        strResponseData = strResponseData.RemoveSpaceAndBreak();

                        string Token = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"token\" id=\"token\" value=\"", "\"");
                        string formnum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"formnum\" id=\"formnum\" value=\"", "\"");
                        string hasFee = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"hasFee\" id=\"hasFee\" value=\"", "\"");
                        string paytype = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"radio\" name=\"paytype\" id=\"paytype\" class=\"INPUT_Border0\" value=\"", "\"");
                        string nextBillvalue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"nextBillvalue\" id=\"nextBillvalue\" value=\"", "\"");

                        dictPostData.Clear();
                        _Random = Ult.CreateRandomString(16);
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=false&random=0.{0}",
                                _Random)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //
                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=printFra&printPriView=true&hasFee={0}&org.apache.struts.taglib.html.TOKEN={1}&paytype={2}&nextBillvalue={3}&preStep=Vorige&printPriBT=Contract%20tonen&formnum={4}&urlPath=null&token={5}",
                                hasFee, org_apache_struts_taglib_html_TOKEN, paytype, nextBillvalue, formnum, Token)),
                            cookies,
                            "",
                            "",
                            certificate
                            );

                        //
                        dictPostData.Clear();
                        _Random = Ult.CreateRandomString(16);
                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=true&random=0.{0}",
                                _Random)),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //PDF   https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=proPrint

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

                            // inschieten begin!!!
                            ////
                            Thread.Sleep(1000);
                            dictPostData.Clear();
                            _Random = Ult.CreateRandomString(16);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=signed&value=true&random=0.{0}",
                                    _Random)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=get&random=0.9172070751072618
                            Thread.Sleep(1000);
                            dictPostData.Clear();
                            _Random = Ult.CreateRandomString(16);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=get&random=0.{0}",
                                    _Random)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=get&random=0.7466306082807984
                            Thread.Sleep(1000);
                            dictPostData.Clear();
                            _Random = Ult.CreateRandomString(16);
                            strResponseData = ScrapeHelper.DoPostPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=get&random=0.{0}",
                                    _Random)),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //hasFee, org_apache_struts_taglib_html_TOKEN, paytype, nextBillvalue, formnum, Token
                            //https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=commitRequest
                            Thread.Sleep(1000);
                            dictPostData.Clear();
                            dictPostData.Add("hasFee", hasFee);
                            dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                            dictPostData.Add("paytype", paytype);
                            dictPostData.Add("nextBillvalue", nextBillvalue);
                            dictPostData.Add("chkGetSignedContract", "1");
                            dictPostData.Add("formnum", formnum);
                            dictPostData.Add("urlPath", "null");
                            dictPostData.Add("token", Token);
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
                                DealerContract.Klant.AbonnementContractList.First().IsOpen = false;
                            }
                            else
                            {
                                if (strResponseData.Contains("<td>Foutmelding&nbsp;</td>"))
                                {
                                    strResponseData = strResponseData.RemoveSpaceAndBreak();
                                    HttpWebResult.ErrorMessage = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<textarea id=\"errorId[2]\" readonly=\"readonly\" style=\"width: 100%;overflow: visible;\" class=\"plan\" type=\"_moz\">", "</textarea>");
                                }
                                else
                                {
                                    HttpWebResult.ErrorMessage = strResponseData;
                                }
                                HttpWebResult.IsSuccess = false;
                            }

                            //Opslaan in database

                            HttpWebResult _HttpWebResult = TelfortKlantViewModel.Set(DealerContract);

                            if (_HttpWebResult.IsSuccess)
                            {
                                goto FinischWithSuccess;
                            }
                            else
                            {
                                HttpWebResult.IsSuccess = false;
                                HttpWebResult.ErrorMessage += _HttpWebResult.ErrorMessage;
                                goto FinischWithError;
                            }
                        }

                    FinischWithError:
                        HttpWebResult.IsSuccess = false;

                    FinischWithSuccess:
                        HttpWebResult.IsSuccess = Saved;
                       
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
    }
}
