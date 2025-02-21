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
    public class IsVerlengbaar : ScrapingBase
    {
        public IsVerlengbaar() : base() { }

        public override void Run()
        {
            //
        }

        private void Execute(Object sender, EventArgs e)
        {
            string strRetentionResult = string.Empty;
            DateTime dtEndDate = DateTime.MinValue;
            InitControlsBinding();
            InvokeBezig();
            string ThisMobileNumber = Mobile; //this.Contract.MobileNumber;
            string SIM = Sim;
            string OfRekeningNr = RekeningNr;

            this.HttpWebResult = new HttpWebResult();
            LoginInfo _LoginInfo = new LoginInfo();
            string strLoginUserName = _LoginInfo.LoginName, strLoginPassword = _LoginInfo.Password;

            //string expiryDate = "10-03-2014";

            string gekozenProduct = "SIMOnly2010"; //<------------------------------------------------Mapping!!!!!!!
            string gekozenProductPeriode = "12"; //<------------------------------------------------Mapping!!!!!!!

            string GekozenSimOnly = "0001.0101.0002";
            string GekozenSmsBundel = "0001.0102";
            string GekozenSmsBundelOptie = "0001.0102.0002";

            string eindDatum = "";
            string ids = ",,,0001,0001.0101;,,,0001.0101,0001.0101.0002;,,,0001,0001.0102;,,,0001.0102,0001.0102.0002;";
            string AuthKey = "";
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
                                //WriteBreakAndLine(strResponseData);
                            }
                        }

                        dictPostData.Clear();
                        dictPostData.Add("firstQuickLoginType", "1");
                        dictPostData.Add("firstLoginNumber", ThisMobileNumber);
                        dictPostData.Add("firstLoginType", "");
                        //dictPostData.Add("secondQuickLoginType", "6"); //[{"label":"KvK nummer","value":"3"},{"label":"Bedrijfsnaam","value":"4"},{"label":"Bankrekeningnummer","value":"5"},{"label":"SIM kaart nummer","value":"6"},{"label":"Geboortedatum","value":"7"}]
                        //dictPostData.Add("secondLoginNumber", SIM);
                        
                        if (string.IsNullOrEmpty(SIM))
                        {
                            dictPostData.Add("secondQuickLoginType", "5"); //[{"label":"KvK nummer","value":"3"},{"label":"Bedrijfsnaam","value":"4"},{"label":"Bankrekeningnummer","value":"5"},{"label":"SIM kaart nummer","value":"6"},{"label":"Geboortedatum","value":"7"}]
                            dictPostData.Add("secondLoginNumber", OfRekeningNr);
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

                        dictPostData.Clear();
                        dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                        dictPostData.Add("custType", "PersonCustomer");
                        dictPostData.Add("custId", strCustomerId);
                        dictPostData.Add("linkManId", "");
                        dictPostData.Add("firstName", "");
                        dictPostData.Add("initials", Voorletters);
                        dictPostData.Add("middleName", "");
                        dictPostData.Add("birthName", Geboortenaam);
                        dictPostData.Add("gender", Geslacht);
                        dictPostData.Add("birthDate", Geboortedatum);
                        dictPostData.Add("lastName", Achternaam);
                        dictPostData.Add("custPostCode", Contactadres_Postcode);
                        dictPostData.Add("custHouseNum", Contactadres_Huisnummer);
                        dictPostData.Add("custSuffixNum", "");
                        dictPostData.Add("custStreet", Contactadres_Straat);
                        dictPostData.Add("custPlace", Contactadres_Plaats);
                        dictPostData.Add("nationality", Nationaliteit);
                        dictPostData.Add("contactTel", "");
                        dictPostData.Add("email", "");
                        dictPostData.Add("idType", IDtype);
                        dictPostData.Add("issueCountry", Afgegevenin);
                        dictPostData.Add("idNumber", IDnummer);
                        dictPostData.Add("issueDate", Afgiftedatum);
                        dictPostData.Add("expiryDate", Vervaldatum);
                        dictPostData.Add("dealerId", dealerId);
                        dictPostData.Add("dealerCode", dealerCode);
                        dictPostData.Add("diffCustAddrFlag", "true");
                        dictPostData.Add("acctPostCode", Contactadres_Postcode);
                        dictPostData.Add("acctHouseNum", Contactadres_Huisnummer);
                        dictPostData.Add("acctSuffixNum", "");
                        dictPostData.Add("acctStreet", Contactadres_Straat);
                        dictPostData.Add("acctPlace", Contactadres_Plaats);
                        dictPostData.Add("acctId", acctId);
                        dictPostData.Add("billFormat", "bfOnline"); //bfConDetail
                        dictPostData.Add("bankAccount", Rekeningnummer);
                        dictPostData.Add("bankType", (Rekeningnummer.Length == 7) ? "Giro" : "Bank");
                        dictPostData.Add("payType", Betaalwijze);

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
                            HttpWebResult.IsSuccess = true;                            
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
    }
}
