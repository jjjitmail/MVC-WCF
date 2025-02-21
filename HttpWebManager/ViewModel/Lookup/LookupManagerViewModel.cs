using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Telfort_XPO_Objects;
using System.Reflection;
using System.Net;

namespace HttpWebManager
{
    public class LookupManagerViewModel : ScrapingBase
    {
        public static Telfort_Objects.LookupManager Execute()
        {
            Domain.InitDBConnection();
            using (UnitOfWork session1 = new UnitOfWork())
            {
                return GetLookupManager(session1);
            }
            return null;
        }

        public override void Run()
        {
            //
        }
             
        // LookupManager items checken/updaten
        private void Execute(Object sender, EventArgs e)
        {
            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            CookieContainer cookies = null;

            bool Saved = false;
            this.HttpWebResult = new HttpWebResult();
            LoginInfo _LoginInfo = new LoginInfo();
            string strLoginUserName = _LoginInfo.LoginName, strLoginPassword = _LoginInfo.Password;
            //string strLoginUserName = "108400503401", strLoginPassword = "9ExGmMtj1";
            //unitedconsumers
            //108400503401
            //9ExGmMtj1

            Telfort_Objects.Contract _DealerContract = new Telfort_Objects.Contract();
            SerializationManager<Telfort_Objects.Contract> _c = new SerializationManager<Telfort_Objects.Contract>() { FileName = "NieuwAanmelden.xml" };
            _c.Load();
            _DealerContract = _c.Content;

            string strResponseData = string.Empty, strErrorMessage = string.Empty;
            string Mobiel = "";
            //Sim = new LoginInfo().Sim;
            string SIM = "893126" + _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr;

            //if (_DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
            //    _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum = Ult.GetNummerPorteringActivationDay(_DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_TypeAansluiting.IntToBoolean());

            string Privilege = "", AppendProduct = "", ids = "";
            string GekozenSimOnly = "", GekozenSmsBundel = "", GekozenSimOnly_GekozenSmsBundel = "", GekozenSmsBundelOptie = "";
            string Surf_Mail = "", Surf_Mail_Optie = "", Surf_Mail_SimOnly = "";

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Lookup_AboBundelList = new XPQuery<Lookup_AboBundel>(session1).ToList().Where(x => x.Actief == true).ToList();
                _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementProductList
                .ForEach(x =>
                {
                    string aboId = x.BundelId;
                    var Lookup_AboBundel = _Lookup_AboBundelList.Where(y => y.BundelId == x.BundelId).FirstOrDefault();
                    if (Lookup_AboBundel.ValueId == "0001.0102")
                    {
                        Privilege = String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        Privilege += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                        GekozenSimOnly = Lookup_AboBundel.Value;
                    }
                    else
                    {
                        if (Lookup_AboBundel.ValueId == "0001.0103")
                        {
                            GekozenSmsBundel = Lookup_AboBundel.ValueId;
                            GekozenSimOnly_GekozenSmsBundel = string.Format("@{0}@{1}@", GekozenSimOnly, Lookup_AboBundel.Value);
                            GekozenSmsBundelOptie = Lookup_AboBundel.Value;
                        }
                        if (Lookup_AboBundel.ValueId == "0001.0104")
                        {
                            Surf_Mail = Lookup_AboBundel.ValueId;
                            Surf_Mail_Optie = Lookup_AboBundel.Value;
                            Surf_Mail_SimOnly = string.Format("@{0}@", GekozenSimOnly);
                            Surf_Mail_SimOnly += !string.IsNullOrEmpty(GekozenSmsBundel) ? GekozenSmsBundelOptie + "@" : Lookup_AboBundel.Value + "@";
                        }

                        AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                    }
                });
                ids = String.Format(",,,0001,0001.0101;{0}{1}", Privilege, AppendProduct);

                string sd = ids;
            }


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
                        Domain.UpdateComboBoxValues<Lookup_Geslacht>(SelectHTMLString);
                        //
                        SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"nationality\" id=\"nationality\" style=\"width: 100% \">", "</select>");
                        Domain.UpdateComboBoxValues<Lookup_Nationaliteit>(SelectHTMLString);

                        SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"idType\" onchange=\"cleanIDNumber(this, document.all.countryOfIssue)\" id=\"idType\" style=\"width: 100% \">", "</select>");
                        Domain.UpdateComboBoxValues<Lookup_IDtype>(SelectHTMLString);
                        
                        //

                        dictPostData.Clear();

                        strResponseData = ScrapeHelper.DoPostPage(
                            new Uri(string.Format("https://b2b.telfort.nl/custcare/cc_common/util/ajaxCommonAction.do?act=qryIssuedCountryByCertType&certTypeId=EUPassport")),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        Domain.UpdateComboBoxValues<Lookup_Afgegevenin>(strResponseData);

                        
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

                        SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"customerType\" onchange=\"displaySsnOrCustNoByCustType();getWishDate();clearWishDate();\" id=\"customerType\" style=\"width:80%\">", "</select>");
                        Domain.UpdateComboBoxValues<Lookup_TypeKlant>(SelectHTMLString);

                        SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"subscriberType\" onchange=\"displayWishDateOrContractEndDate();getWishDate();setMinDate(this,'txtWishDate');\" id=\"subscriberType\" style=\"width:80%\">", "</select>");
                        Domain.UpdateComboBoxValues<Lookup_TypeAansluiting>(SelectHTMLString);

                        //klaar hier!!!!!!!

                    //    dt = new DateTime().ToString();
                    //    dictPostData.Clear();
                    //    dictPostData.Add("date", dt);

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/boss/heartThrob.jsp?date={0}", dt)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //popup - OPENEN MET  SIMonly = "1" select product
                    //    strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                    //        new Uri("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=commonTree&recType=Install&catalogType=ROOT&productType=ProdType_Person&custType=PersonCustomer&recChannel=bsacHal&solutionID=&productID=&subs_relation_type=&main_or_subs_card=&DependOnID=&subSeq="),
                    //        cookies,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    strResponseData = strResponseData.RemoveSpaceAndBreak();

                    //    //SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "d.add(\"ROOT\",\"\",\"Product\",\"\",\"_self\");", "d.oAll(true);").Replace("d.add(\"TlfPreFMRHSM", "").Replace("d.add(\"TlfPreFMRSOM", "").Replace(">Telfort prepaid", "").Replace(">Telfort Prepaid", "");

                    //    // XPCollection<Lookup_SubscriptionType> _stList = new XPCollection<Lookup_SubscriptionType>();

                    //    //Domain.InitRadioValues_Lookup_SubscriptionType(SelectHTMLString, ref _stList);

                    //    Thread.Sleep(1000);
                    //    dictPostData.Clear();
                    //    strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                    //    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getNetType&productId={0}&productName={1}&RelationType=&StallType=0",
                    //        _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                    //        _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam)),
                    //    cookies,
                    //    strResponseData,
                    //    dictPostData,
                    //    "",
                    //    "",
                    //    certificate
                    //    );

                    //    strResponseData = strResponseData.RemoveSpaceAndBreak();

                    //    string GSM = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "\",\"", "@_@");
                    //    string resType = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");
                    //    strResponseData = strResponseData.Replace("@_@" + resType, "");
                    //    string brandId = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "@_@", "@_@");

                    //    // get phone nummers
                    //    dictPostData.Clear();
                    //    strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                    //    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getMoreselectTelnums&productId={0}&brandId={1}&resType={2}",
                    //        _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId, brandId, resType)),
                    //    cookies,
                    //    strResponseData,
                    //    dictPostData,
                    //    "",
                    //    "",
                    //    certificate
                    //    );

                    //    List<string> _PhoneList = new List<string>();

                    //    _PhoneList = Domain.ReturnPhoneListValues(strResponseData);

                    //    //get bundel page
                    //    _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr = _PhoneList[0];

                    //    dictPostData.Clear();
                    //    strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                    //    new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubsInfoForVasProduct&saveFlag=C&hidShowSubSeq=&contractPeriod={5}&productId={0}&hidProductName={1}&txtActivationDate={2}&txtMsisdn={3}&txtSsn={4}&txtImsi=&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false",
                    //        _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId, _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                    //        _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString(), _PhoneList[0], SIM,
                    //        _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode)),
                    //    cookies,
                    //    strResponseData,
                    //    dictPostData,
                    //    "",
                    //    "",
                    //    certificate
                    //    );

                    //    //
                    //    strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=selectProduct&cmd=select&isHasCautioner=&isSimple=&curProcductID={0}",
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId)),
                    //        cookies,
                    //        "",
                    //        "",
                    //        certificate
                    //        );
                    //    // begin bundel kiezen & save

                    //    dt = new DateTime().ToString();
                    //    dictPostData.Clear();
                    //    dictPostData.Add("date", dt);

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/boss/heartThrob.jsp?date={0}", dt)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //520 bundel selecteren

                    //    dictPostData.Clear();
                    //    dictPostData.Add("catatype", "Privilege");
                    //    dictPostData.Add("objectID", GekozenSimOnly);
                    //    dictPostData.Add("cmd", "addNode");
                    //    dictPostData.Add("radios", String.Format("@{0}@", GekozenSimOnly));

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );
                    //    //521
                    //    if (!string.IsNullOrEmpty(GekozenSmsBundel))
                    //    {
                    //        dictPostData.Clear();
                    //        dictPostData.Add("catatype", "AppendProduct");
                    //        dictPostData.Add("objectID", GekozenSmsBundel);
                    //        dictPostData.Add("cmd", "addNode");
                    //        dictPostData.Add("radios", GekozenSimOnly_GekozenSmsBundel);

                    //        strResponseData = ScrapeHelper.DoPostPage(
                    //            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                    //            cookies,
                    //            strResponseData,
                    //            dictPostData,
                    //            "",
                    //            "",
                    //            certificate
                    //            );

                    //        dictPostData.Clear();
                    //        dictPostData.Add("catatype", "Privilege");
                    //        dictPostData.Add("objectID", GekozenSmsBundelOptie);
                    //        dictPostData.Add("cmd", "addNode");
                    //        dictPostData.Add("radios", GekozenSimOnly_GekozenSmsBundel);

                    //        strResponseData = ScrapeHelper.DoPostPage(
                    //            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                    //            cookies,
                    //            strResponseData,
                    //            dictPostData,
                    //            "",
                    //            "",
                    //            certificate
                    //            );
                    //    }
                    //    if (!string.IsNullOrEmpty(Surf_Mail)) //string Surf_Mail = "", Surf_Mail_SimOnly = "";
                    //    {
                    //        dictPostData.Clear();
                    //        dictPostData.Add("catatype", "AppendProduct");
                    //        dictPostData.Add("objectID", Surf_Mail);
                    //        dictPostData.Add("cmd", "addNode");
                    //        dictPostData.Add("radios", Surf_Mail_SimOnly);

                    //        strResponseData = ScrapeHelper.DoPostPage(
                    //            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                    //            cookies,
                    //            strResponseData,
                    //            dictPostData,
                    //            "",
                    //            "",
                    //            certificate
                    //            );

                    //        dictPostData.Clear();
                    //        dictPostData.Add("catatype", "Privilege");
                    //        dictPostData.Add("objectID", Surf_Mail_Optie);
                    //        dictPostData.Add("cmd", "addNode");
                    //        dictPostData.Add("radios", Surf_Mail_SimOnly);

                    //        strResponseData = ScrapeHelper.DoPostPage(
                    //            new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=adduser")),
                    //            cookies,
                    //            strResponseData,
                    //            dictPostData,
                    //            "",
                    //            "",
                    //            certificate
                    //            );
                    //    }
                    //    //532

                    //    dictPostData.Clear();
                    //    dictPostData.Add("ids", ids);

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=calcBuyOffFee")),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );
                    //    //533
                    //    dictPostData.Clear();
                    //    dictPostData.Add("pids", "");
                    //    dictPostData.Add("buyOffFlag", "0");

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=commitSave")),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    // end bundel kiezen

                    //    //
                    //    dictPostData.Clear();

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=clearSessionForMainProduct")),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    // Result tonen begin
                    //    //
                    //    dictPostData.Clear();

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getVasProductList&curProcductID={0}&updateFlag=undefined&hidShowSubSeq=&contractPeriod={1}&productId={2}&hidProductName={3}&txtActivationDate={4}&txtMsisdn={5}&txtSsn={6}&txtImsi=&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=false&saveFlag=C&isImport=undefined",
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString(),
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr, SIM)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    // Result tonen end

                    //    //
                    //    dictPostData.Clear();

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkActivationDateValid&ActivationDate={0}", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString())),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    // BEGIN OPSLAAN 121

                    //    string txtImsi = "";

                    //    //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkNoCard&phoneNum=0619787137&productId=SIMOnly2010&hasLockOrNot=1&oldsimNum=&simNum=8931261006144039004&selectMsisdnType=1
                    //    dictPostData.Clear();
                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=checkNoCard&phoneNum={0}&productId={1}&hasLockOrNot=1&oldsimNum=&simNum={2}&selectMsisdnType=1",
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                    //            SIM)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );
                    //    txtImsi = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "\"],[\"2\",\"", "\"");
                    //    //
                    //    dictPostData.Clear();

                    //    _PhoneList.RemoveAt(0);
                    //    string unPickSelectTelnums = string.Join(",", _PhoneList.ToArray()) + ",";

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=unPickSelectTelnums&selectTel&unPickSelectTelnums={0}",
                    //            unPickSelectTelnums)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //
                    //    dictPostData.Clear();
                    //    string _Random = Ult.CreateRandomString(16);
                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=false&random=0.{0}",
                    //            _Random)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );
                    //    Thread.Sleep(1000);
                    //    //
                    //    dictPostData.Clear();
                    //    _Random = Ult.CreateRandomString(16);
                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=signed&value=false&random=0.{0}",
                    //            _Random)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //Nummerportering begin
                    //    string NP_SIMPrefix = ""; // <--------------- Network Operator
                    //    if (_DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                    //    {
                    //        // NP Prefix
                    //        dictPostData.Clear();
                    //        strResponseData = ScrapeHelper.DoPostPage(
                    //           new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=qryNpSsnPrefixByCurrentCarrierId&CurrentCarrier={0}&currentSP={1}",
                    //               _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_NetwerkOperator,
                    //               _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_ServiceProvider)),
                    //           cookies,
                    //           strResponseData,
                    //           dictPostData,
                    //           "",
                    //           "",
                    //           certificate
                    //           );
                    //        NP_SIMPrefix = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "\",\"", "@_@");
                    //    }

                    //    //Nummerportering end

                    //    //


                    //    //https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubInfo&saveFlag=C&hidShowSubSeq=&contractPeriod=24&productId=SIMOnly2010&hidProductName=Telfort%20sim%20only&txtActivationDate=07-02-2012&txtMsisdn=0619787137&txtSsn=8931261006144039004&txtImsi=204121900864059&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=true&simType=&simTypeDesc=undefined
                    //    dictPostData.Clear();

                    //    dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN); //c046b31f1763bf41e0ef39925bc8b133
                    //    dictPostData.Add("hidShowSubSeq", "");
                    //    dictPostData.Add("hidProductId", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                    //    dictPostData.Add("hidProductName", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam);
                    //    dictPostData.Add("hidNetType", GSM);
                    //    dictPostData.Add("hidPrdBrand", brandId);
                    //    dictPostData.Add("hidPhoneResType", resType);
                    //    dictPostData.Add("hidPaymodeType", "");
                    //    dictPostData.Add("chkGetVasProdFlag", "on");
                    //    dictPostData.Add("hidTxtImsi", txtImsi);
                    //    dictPostData.Add("hidOldMisidn", "");
                    //    dictPostData.Add("hidOldIccid", "");
                    //    dictPostData.Add("hidNpSsnPrefix", NP_SIMPrefix);
                    //    if (_DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                    //    {
                    //        dictPostData.Add("currentCarrier", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_NetwerkOperator);//
                    //    }
                    //    dictPostData.Add("ssnPrefix", "893126");
                    //    dictPostData.Add("errorMsg", "");
                    //    dictPostData.Add("productId", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                    //    dictPostData.Add("contractPeriod", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode);
                    //    dictPostData.Add("txtMsisdn", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                    //    dictPostData.Add("selectTelNum", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                    //    dictPostData.Add("ssnSuffix", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr);
                    //    dictPostData.Add("txtSsn", "893126" + _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr);
                    //    dictPostData.Add("numberConcealed", "false");
                    //    dictPostData.Add("numberInclusion", "NN");
                    //    dictPostData.Add("canDoCustRec", "on");
                    //    dictPostData.Add("imei", "");
                    //    if (_DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                    //    {
                    //        dictPostData.Add("chkNP", "on");//
                    //        dictPostData.Add("txtNPCurrentMsisdn", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_MobielNr);//
                    //        dictPostData.Add("currentSp", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_ServiceProvider);//
                    //        dictPostData.Add("customerType", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_TypeKlant);//
                    //        dictPostData.Add("txtSsnCustomerNo", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_SIM.StartsWith(NP_SIMPrefix) ? _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_SIM.Replace(NP_SIMPrefix, "") : _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_SIM);//
                    //        dictPostData.Add("subscriberType", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_TypeAansluiting);//
                    //        dictPostData.Add("txtContractDate", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_EindDatum.ToShortDateString());//
                    //    }
                    //    dictPostData.Add("oldIndex", "");
                    //    if (_DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering)
                    //    {
                    //        dictPostData.Add("txtNoticePeriod", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_Opzegtermijn.ToString());//
                    //        dictPostData.Add("txtWishDate", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_Wensdatum.ToShortDateString());
                    //    }

                    //    dictPostData.Add("hiddenWishDate", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering ? _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().NP_EindDatum.AddDays(1).ToShortDateString() : "");
                    //    dictPostData.Add("hiddenMaxWishDate", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString());

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubInfo&saveFlag=C&hidShowSubSeq=&contractPeriod={0}&productId={1}&hidProductName={2}&txtActivationDate={3}&txtMsisdn={4}&txtSsn={5}&txtImsi={6}&numberInclusion=NN&numberConcealed=false&canDoCustRec=true&chkGetVasProdFlag=true&simType=&simTypeDesc=undefined",
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Contractperiode,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam,
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString(),
                    //            _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr,
                    //            SIM,
                    //            txtImsi)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //string Startdatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"txtActivationDate\" maxlength=\"50\" value=\"", "\"");

                    //    //_DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum = Startdatum.ToDateTime();

                    //    ///https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=next

                    //    dictPostData.Clear();
                    //    dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                    //    dictPostData.Add("hidShowSubSeq", "0");
                    //    dictPostData.Add("hidProductId", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                    //    dictPostData.Add("hidProductName", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementNaam);
                    //    dictPostData.Add("hidNetType", GSM);
                    //    dictPostData.Add("hidPrdBrand", brandId);
                    //    dictPostData.Add("hidPhoneResType", resType);
                    //    dictPostData.Add("hidPaymodeType", "");
                    //    dictPostData.Add("chkGetVasProdFlag", "on");
                    //    dictPostData.Add("hidTxtImsi", txtImsi);
                    //    dictPostData.Add("hidOldMisidn", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                    //    dictPostData.Add("hidOldIccid", "893126" + _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().SimNr);
                    //    dictPostData.Add("hidNpSsnPrefix", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering ? NP_SIMPrefix : "");
                    //    dictPostData.Add("ssnPrefix", "893126");
                    //    dictPostData.Add("errorMsg", "");
                    //    dictPostData.Add("mainProductID", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId);
                    //    dictPostData.Add("serviceNumber", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().MobileNr);
                    //    dictPostData.Add("productId", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().IsNummerPortering ? _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementId : "");
                    //    dictPostData.Add("oldIndex", "");
                    //    dictPostData.Add("hiddenWishDate", "");
                    //    dictPostData.Add("hiddenMaxWishDate", _DealerContract.Klant.AbonnementContractList.First().AbonnementList.First().Startdatum.ToShortDateString());

                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=next")),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );
                    //    strResponseData = strResponseData.RemoveSpaceAndBreak();

                    //    string Token = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"token\" id=\"token\" value=\"", "\"");
                    //    string formnum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"formnum\" id=\"formnum\" value=\"", "\"");
                    //    string hasFee = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"hasFee\" id=\"hasFee\" value=\"", "\"");
                    //    string paytype = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"radio\" name=\"paytype\" id=\"paytype\" class=\"INPUT_Border0\" value=\"", "\"");
                    //    string nextBillvalue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"nextBillvalue\" id=\"nextBillvalue\" value=\"", "\"");

                    //    dictPostData.Clear();
                    //    _Random = Ult.CreateRandomString(16);
                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=false&random=0.{0}",
                    //            _Random)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //
                    //    strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=printFra&printPriView=true&hasFee={0}&org.apache.struts.taglib.html.TOKEN={1}&paytype={2}&nextBillvalue={3}&preStep=Vorige&printPriBT=Contract%20tonen&formnum={4}&urlPath=null&token={5}",
                    //            hasFee, org_apache_struts_taglib_html_TOKEN, paytype, nextBillvalue, formnum, Token)),
                    //        cookies,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //
                    //    dictPostData.Clear();
                    //    _Random = Ult.CreateRandomString(16);
                    //    strResponseData = ScrapeHelper.DoPostPage(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=receptionRemoteStoreService&action=set&key=printed&value=true&random=0.{0}",
                    //            _Random)),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate
                    //        );

                    //    //PDF   https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=proPrint

                    //    dictPostData.Clear();
                    //    Saved = ScrapeHelper.DoPostPageSavePDF(
                    //        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/receptionCommitAction.do?method=proPrint")),
                    //        cookies,
                    //        strResponseData,
                    //        dictPostData,
                    //        "",
                    //        "",
                    //        certificate,
                    //        formnum
                    //        );

                    //    if (Saved)
                    //    {
                    //        goto FinischWithSuccess;
                    //    }

                    FinischWithError:
                        HttpWebResult.IsSuccess = false;

                    //FinischWithSuccess:
                    //    HttpWebResult.IsSuccess = Saved;
                        // END OPSLAAN

                        //strResponseData = strResponseData.RemoveSpaceAndBreak();

                        //strResponseData = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<body onLoad=\"doinit();", "</tbody>");
                        //strResponseData = strResponseData.RemoveSpaceAndBreak();
                        //string GroupName = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<tr basenode=\"1\" id=\"", "\"");





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

        private static Telfort_Objects.LookupManager GetLookupManager(UnitOfWork session1)
        {
            Telfort_Objects.LookupManager _LookupManager = new Telfort_Objects.LookupManager();

            //InitObjectProperties(_LookupManager, session1);

            // container object type aanmaken
            Type _Type = typeof(Telfort_Objects.LookupManager);

            // properties van container object doorlopen
            _LookupManager.GetType().GetProperties()
                .ToList().ForEach(x =>
                {
                    // property info ophalen
                    PropertyInfo prop = _Type.GetProperty(x.Name);

                    // LookupType attibute van betreffende property ophalen
                    Attribute attribute = (Attribute)prop.GetCustomAttributes(typeof(Attribute), true)
                                                    .Where(y => y.GetType().Name.Equals("LookupType")).FirstOrDefault();

                    // property en LookupType attribute niet leeg?
                    if (prop != null && attribute != null)
                    {
                        // "T" property van LookupType attribute ophalen
                        Type _ObjectT = (Type)attribute.GetType().GetProperty("T").GetValue(attribute, null);

                        // Type van betreffende "T" property ophalen
                        Type _XPO_Object_Type = AssemblyManager.Telfort_XPO_Objects_Type(x.ReflectedType.Name, _ObjectT.Name);

                        // XPO collection ophalen en convert naar een WCF compatible type collection
                        var _Collection = ConvertXPOCollection(session1, _ObjectT, _XPO_Object_Type);

                        // Convert de collection naar een collection type die bij property van container object past.
                        object list = _Collection.ConvertToGenericCollection(_ObjectT);

                        // Daarna betreffende property (collection) vullen met juiste collection
                        _LookupManager.GetType().GetProperty(x.Name).SetValue(_LookupManager, Convert.ChangeType(list, prop.PropertyType), null);
                    }
                });
            return _LookupManager;
        }
        
        private static void InitObjectProperties(object _Obj, UnitOfWork session1)
        {
            Type _Type = _Obj.GetType();
            
            // properties van container object doorlopen
            _Obj.GetType().GetProperties()
                .ToList().ForEach(x =>
            {
                // property info ophalen
                PropertyInfo prop = _Type.GetProperty(x.Name);

                // LookupType attibute van betreffende property ophalen
                Attribute attribute = (Attribute)prop.GetCustomAttributes(typeof(Attribute), true)
                                                .Where(y => y.GetType().Name.Equals("LookupType")).FirstOrDefault();

                // property en LookupType attribute niet leeg?
                if (prop != null && attribute != null)
                {
                    // "T" property van LookupType attribute ophalen
                    Type _ObjectT = (Type)attribute.GetType().GetProperty("T").GetValue(attribute, null);

                    // Type van betreffende "T" property ophalen
                    Type _XPO_Object_Type = AssemblyManager.Telfort_XPO_Objects_Type("LookupManager", _ObjectT.Name);//x.ReflectedType.Name

                    // XPO collection ophalen en convert naar een WCF compatible type collection
                    var _Collection = ConvertXPOCollection(session1, _ObjectT, _XPO_Object_Type);

                    // Convert de collection naar een collection type die bij property van container object past.
                    object list = _Collection.ConvertToGenericCollection(_ObjectT);

                    // Daarna betreffende property (collection) vullen met juiste collection
                    _Obj.GetType().GetProperty(x.Name).SetValue(_Obj, Convert.ChangeType(list, prop.PropertyType), null);

                    //object _ThisObj = Activator.CreateInstance(_ObjectT);
                    //InitObjectProperties(_ThisObj, session1);
                }
            });
        }
        
        private static List<T> GetCollection<T, U>(UnitOfWork session1)
        {
            return Ult.ConvertTList<T, U>(new XPQuery<U>(session1).ToList()).ToList();
        }

        private static List<object> ConvertXPOCollection(UnitOfWork session1, Type _Object_Type, Type _XPO_Object_Type)
        {
            XPCollection _XPList = new XPCollection(session1, _XPO_Object_Type);

            return Ult.ConvertTList(_XPList, _Object_Type);
        }

        private static List<object> ConvertXPOCollection_X(UnitOfWork session1, Type _Object_Type, Type _XPO_Object_Type)
        {
            XPCollection _XPList = new XPCollection(session1, _XPO_Object_Type);

            return Ult.ConvertTList(_XPList, _Object_Type);
        }

    }
}
