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
    public class KlantGegevens : ScrapingBase
    {
        public KlantGegevens() : base() { }

        public override void Run()
        {
            //
        }

        private void Execute(Object sender, EventArgs e)
        {
            InitControlsBinding();

            InvokeBezig();

            string ThisMobileNumber = Mobile; //this.Contract.MobileNumber;
            string SIM = Sim;

            this.HttpWebResult = new HttpWebResult();
            LoginInfo _LoginInfo = new LoginInfo();
            string strLoginUserName = _LoginInfo.LoginName, strLoginPassword = _LoginInfo.Password;
            
            string strRetentionResult = string.Empty;
            DateTime dtEndDate = DateTime.MinValue;

            string strResponseData = string.Empty, strErrorMessage = string.Empty;
            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            CookieContainer cookies = null;
            string AuthKey = "", OperID = "";
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
                    using (UnitOfWork session1 = new UnitOfWork())
                    {
                        Klant _Klant_Telfort = new Klant(session1);

                        strResponseData = HttpWebManager.ScrapeHelper.DoLogin(
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

                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/boss/post.do"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //WriteBreakAndLine(strResponseData);

                        strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
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

                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                            new Uri("https://b2b.telfort.nl/ca/POST.do"),
                            cookies,
                            strResponseData,
                            dictPostData,
                            "",
                            "",
                            certificate
                            );

                        //WriteBreakAndLine(strResponseData);

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
                            //WriteBreakAndLine(strResponseData);
                            if (strResponseData.Contains("Dit account is aangemeld op een andere locatie"))
                            {
                                strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                    new Uri("https://b2b.telfort.nl/boss/repeatLogin.do?actionFlag=login&loginPage=Frameset.jsp&loginRepeatFlag=1&operId=" + strLoginUserName),
                                    cookies,
                                    "",
                                    "",
                                    certificate
                                    );
                                //WriteBreakAndLine(strResponseData);
                            }

                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                new Uri("https://b2b.telfort.nl/custcare/cc_common/commonLoginAction.do?method=initPage&loadmode=loginboss&lang=nl_NL"),
                                cookies,
                                "",
                                "",
                                certificate
                                );
                            //WriteBreakAndLine(strResponseData);
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
                                    AuthKey = strValue;
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
                                    //WriteBreakAndLine(strResponseData);
                                }
                            }

                            dictPostData.Clear();
                            dictPostData.Add("firstQuickLoginType", "1");
                            dictPostData.Add("firstLoginNumber", ThisMobileNumber);
                            dictPostData.Add("firstLoginType", "");
                            dictPostData.Add("secondQuickLoginType", "6"); //[{"label":"KvK nummer","value":"3"},{"label":"Bedrijfsnaam","value":"4"},{"label":"Bankrekeningnummer","value":"5"},{"label":"SIM kaart nummer","value":"6"},{"label":"Geboortedatum","value":"7"}]
                            dictPostData.Add("secondLoginNumber", SIM);
                            dictPostData.Add("secondLoginType", "");

                            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                                new Uri("https://b2b.telfort.nl/custcare/cc_common/commonLoginAction.do?method=showCustomerList4Dealer"),
                                cookies,
                                strResponseData,
                                dictPostData,
                                "",
                                "",
                                certificate
                                );
                            //WriteBreakAndLine(strResponseData);
                            string strCustomerId = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "loginBy('", "'");

                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                               new Uri(string.Format("https://b2b.telfort.nl/custcare/cc_common/CustomerInfoAction.do?act=qryCustomerInfo&ONLYLOGIN=onlyLogin&&loginLevel=1&checkPassword=1&authType=AuthTypeCustID&objectID={0}&loginMsisdn={1}", strCustomerId, ThisMobileNumber)),
                               cookies,
                               "",
                               "",
                               certificate
                               );
                            //WriteBreakAndLine(strResponseData);
                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                new Uri("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/showCustomerInfo.do?method=initForLogin"),
                                cookies,
                                "",
                                "",
                                certificate
                                );
                            //WriteBreakAndLine(strResponseData);
                            string strSubscriberId = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "top.subsID4IPCC = \"", "\"");

                            //Klantgegevens

                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/personCustomerInfoAction.do?method=showPersonInfo&showType=showDetail&custType=PersonCustomer&selectedCustId={0}&msisdns={1}&fromloginflag=true", strCustomerId, ThisMobileNumber)),
                                cookies,
                                "",
                                "",
                                certificate
                                );
                            //WriteBreakAndLine(strResponseData);
                            string strSegment = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_customerLevel\"", "</select>");
                            strSegment = HttpWebManager.ScrapeHelper.ExtractValue(strSegment, "selected=\"selected\">", "</option>");

                            //html strippen //string PUK2 = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "", "");
                            strResponseData = strResponseData.RemoveSpaceAndBreak();
                            string Voorletters = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_initials\" value=\"", "\" id=\"cust_initials\" style=\"width: 100%;\" class=\"readonly\">");
                            string Tussenvoegsel = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_middleName\" value=\"", "\" id=\"cust_middleName\" style=\"width: 100%;\" class=\"readonly\">");
                            string Geboortenaam = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_nameOfBirth\" value=\"", "\" id=\"cust_nameOfBirth\" style=\"width: 100%\" class=\"readonly\">");
                            string Middlenamebirth = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_middlename_brith\" value=\"", "\" id=\"middleNameBirth\" style=\"width: 100%\" class=\"readonly\">");
                            string Achternaam = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_lastname\" value=\"", "\" id=\"cust_lastname\" style=\"width: 100%\" class=\"readonly\">");

                            string Geslacht = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_gender\"", "</select>");
                            Geslacht = HttpWebManager.ScrapeHelper.ExtractValue(Geslacht, "selected=\"selected\">", "</option>");
                            Geslacht = Geslacht == "Vrouw" ? "0" : "1";

                            string Telefoonnummer = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_contactTel\" maxlength=\"10\" value=\"", "\" onblur=\"validateContactTel(this);\" id=\"cust_contactTel\" style=\"width: 100%\">");
                            string Emailadres = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_emailAddress\" value=\"", "\" onblur=\"chkemail(this)\" id=\"cust_emailAddress\" style=\"width: 100%\">");
                            string Geboortedatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_dateOfBirth\" value=\"", "\" id=\"cust_dateOfBirth\" style=\"width: 100%;color:#CCC;\" class=\"Wdate\" title=\"dd-MM-yyyy\">");
                            string Klantenservicewachtwoord = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_servicePassword\" value=\"", "\" readonly=\"readonly\" id=\"acct_servicePassword\" style=\"width: 100%; color:red;font-weight: bold;\" class=\"readonly\">");

                            string Status = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_status\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_KlantStatus>(Status);
                            Status = HttpWebManager.ScrapeHelper.ExtractValue(Status, "selected=\"selected\">", "</option>");
                            Status = Domain.GetComboxValue<Lookup_KlantStatus>(Status);

                            string Klantnummer = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_customerCode\" value=\"", "\" disabled=\"disabled\" id=\"acct_customerCode\" style=\"width: 100%\">");
                            string Dealercode = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_dealerCode\" value=\"", "\" disabled=\"disabled\" id=\"acct_dealerCode\" style=\"width: 100%\">");
                            string Registratiedatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"acct_registeredDate\" value=\"", "\" disabled=\"disabled\" id=\"acct_registeredDate\" style=\"width: 100%\">");

                            string Bedrijfsvorm = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_customerType\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_Bedrijfsvorm>(Bedrijfsvorm);
                            Bedrijfsvorm = HttpWebManager.ScrapeHelper.ExtractValue(Bedrijfsvorm, "selected=\"selected\">", "</option>");
                            Bedrijfsvorm = Domain.GetComboxValue<Lookup_Bedrijfsvorm>(Bedrijfsvorm);

                            string Klantniveau = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_customerLevel\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_Klantniveau>(Klantniveau);
                            Klantniveau = HttpWebManager.ScrapeHelper.ExtractValue(Klantniveau, "selected=\"selected\">", "</option>");
                            Klantniveau = Domain.GetComboxValue<Lookup_Klantniveau>(Klantniveau);

                            string IDtype = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_certificate\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_IDtype>(IDtype);
                            IDtype = HttpWebManager.ScrapeHelper.ExtractValue(IDtype, "selected=\"selected\">", "</option>");
                            IDtype = Domain.GetComboxValue<Lookup_IDtype>(IDtype);

                            string Afgegevenin = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_issuedIn\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_Afgegevenin>(Afgegevenin);
                            Afgegevenin = HttpWebManager.ScrapeHelper.ExtractValue(Afgegevenin, "selected=\"selected\">", "</option>");
                            Afgegevenin = Domain.GetComboxValue<Lookup_Afgegevenin>(Afgegevenin);

                            string IDnummer = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_idNumber\" maxlength=\"31\" value=\"", "\" onblur=");
                            string Afgiftedatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_issueDate\" value=\"", "\" onfocus=");
                            string Vervaldatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"cust_expirationDate\" value=\"", "\" onfocus=");

                            string Nationaliteit = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"cust_nationality\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_Nationaliteit>(Nationaliteit);
                            Nationaliteit = HttpWebManager.ScrapeHelper.ExtractValue(Nationaliteit, "selected=\"selected\">", "</option>");
                            Nationaliteit = Domain.GetComboxValue<Lookup_Nationaliteit>(Nationaliteit);

                            string Contactadres_Straat = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_street\" value=\"", "\" onblur=");
                            string Contactadres_Huisnummer = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_house\" maxlength=\"5\" value=\"", "\" onchange=");
                            string Contactadres_Toevoeging = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_suffix\" maxlength=\"6\" value=\"", "\" onblur=");
                            string Contactadres_Postcode = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_postcode\" maxlength=\"6\" value=\"", "\" onchange=");
                            string Contactadres_Plaats = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"contact_place\" value=\"", "\" onblur=");
                            string Postadres_Straat = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_street\" value=\"", "\" onblur=");
                            string Postadres_Huisnummer = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_house\" maxlength=\"5\" value=\"", "\" onchange=");
                            string Postadres_Toevoeging = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_suffix\" maxlength=\"6\" value=\"", "\" onblur=");
                            string Postadres_Postcode = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_postcode\" maxlength=\"6\" value=\"", "\" onchange=");
                            string Postadres_Plaats = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"post_place\" value=\"", "\" onblur=");

                            string Postadres_Adres_status = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"post_status\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_Postadres_Adres_status>(Postadres_Adres_status);
                            Postadres_Adres_status = HttpWebManager.ScrapeHelper.ExtractValue(Postadres_Adres_status, "selected=\"selected\">", "</option>");
                            Postadres_Adres_status = Domain.GetComboxValue<Lookup_Postadres_Adres_status>(Postadres_Adres_status);

                            string Postadres_is_contractadres = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"checkbox\" name=\"post_AddrSameAsContractAddr\" value=\"on\" checked=\"", "\" onclick=");
                            Postadres_is_contractadres = Postadres_is_contractadres == "checked" ? "true" : "false";

                            string Betaalwijze = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"pay_PaymentType\"", "</select>");
                            Domain.InitComboBoxValues<Lookup_Betaalwijze>(Betaalwijze);
                            Betaalwijze = HttpWebManager.ScrapeHelper.ExtractValue(Betaalwijze, "selected=\"selected\">", "</option>");
                            Betaalwijze = Domain.GetComboxValue<Lookup_Betaalwijze>(Betaalwijze);

                            string Rekeningnummer = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"text\" name=\"pay_Rekeningnummer\" maxlength=\"9\" value=\"", "\" onkeypress=");

                            // waarde in object gooien!
                            _Klant_Telfort.Voorletters = Voorletters;
                            _Klant_Telfort.Tussenvoegsel = Tussenvoegsel;
                            _Klant_Telfort.Geboortenaam = Geboortenaam;
                            _Klant_Telfort.Middlenamebirth = Middlenamebirth;
                            _Klant_Telfort.Achternaam = Achternaam;
                            _Klant_Telfort.Geslacht = Geslacht.ToInt16();
                            _Klant_Telfort.Telefoonnummer = Telefoonnummer;
                            _Klant_Telfort.Emailadres = Emailadres;
                            _Klant_Telfort.Geboortedatum = Geboortedatum.ToDateTime();
                            _Klant_Telfort.Klantenservicewachtwoord = Klantenservicewachtwoord;
                            _Klant_Telfort.Status = Status;
                            _Klant_Telfort.Klantnummer = Klantnummer;
                            _Klant_Telfort.Dealercode = Dealercode;
                            _Klant_Telfort.Registratiedatum = Registratiedatum.ToDateTime();
                            _Klant_Telfort.Bedrijfsvorm = Bedrijfsvorm;
                            _Klant_Telfort.Klantniveau = Klantniveau.ToInt16();
                            _Klant_Telfort.IDtype = IDtype;
                            _Klant_Telfort.Afgegevenin = Afgegevenin;
                            _Klant_Telfort.IDnummer = IDnummer;
                            _Klant_Telfort.Afgiftedatum = Afgiftedatum.ToDateTime();
                            _Klant_Telfort.Vervaldatum = Vervaldatum.ToDateTime();
                            _Klant_Telfort.Nationaliteit = Nationaliteit;

                            Adres _Adres = new Adres(session1);
                            _Adres.Type = Adres.AdresType.Woon.ToString();
                            _Adres.Straat = Contactadres_Straat;
                            _Adres.Huisnummer = Contactadres_Huisnummer;
                            _Adres.Toevoeging = Contactadres_Toevoeging;
                            _Adres.Postcode = Contactadres_Postcode;
                            _Adres.Plaats = Contactadres_Plaats;
                            _Klant_Telfort.AdresCollection.Add(_Adres);

                            _Adres = new Adres(session1);
                            _Adres.Type = Adres.AdresType.Post.ToString();
                            _Adres.Straat = Postadres_Straat;
                            _Adres.Huisnummer = Postadres_Huisnummer;
                            _Adres.Toevoeging = Postadres_Toevoeging;
                            _Adres.Postcode = Postadres_Postcode;
                            _Adres.Plaats = Postadres_Plaats;
                            _Adres.Status = Postadres_Adres_status;
                            _Klant_Telfort.AdresCollection.Add(_Adres);

                            _Klant_Telfort.Postadres_is_contractadres = Postadres_is_contractadres.IntToBoolean();

                            // Aansluiten - producten

                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/showSubscriberInfo.do?method=showSubInfoDetail&income=detail&act=productinfo&subscriberID={0}&fromloginflag=true&selectedCustId={1}", strSubscriberId, strCustomerId)),
                                cookies,
                                "",
                                "",
                                certificate
                                );
                            //WriteBreakAndLine(strResponseData);
                            //html strippen //string PUK2 = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "", "");
                            strResponseData = strResponseData.RemoveSpaceAndBreak();
                            string TelfortID = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<td class=\"labelProductInfo\">Telfort ID</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Huidige status");
                            string Huidige_status = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Huidige status</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Type aansluiting");
                            string Type_aansluiting = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Type aansluiting</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Product");
                            string Product = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Product</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Bundel");
                            string Bundel = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Bundel</td><td class=\"value\">", "&nbsp;</td></tr></table></div></div><!-- Product Info End -->");
                            string Contractperiode = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Contractperiode</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Startdatum");
                            string Startdatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Startdatum</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Einddatum");
                            string Einddatum = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Einddatum</td><td class=\"value\">", "&nbsp;</td></tr><tr><td class=\"labelProductInfo\">Afkoopsom");
                            string Afkoopsom = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "Afkoopsom(€)<sup>incl. BTW</sup></td><td class=\"value\">", "&nbsp;</td></tr><tr><td colspan=\"2\">&nbsp;</td></tr><tr><td colspan=\"2\">&nbsp;</td></tr><tr><td colspan=\"2\"><div style=\"float:right\" >");

                            AbonnementContract _Contract = new AbonnementContract(session1);
                            _Contract.Betaalwijze = Betaalwijze;
                            _Contract.Huidige_status = Huidige_status;
                            _Contract.Rekeningnummer = Rekeningnummer;
                            _Contract.TelfortID = TelfortID;
                            _Contract.Type_aansluiting = Type_aansluiting;
                            
                            Abonnement _Abonnement = new Abonnement(session1);
                            _Abonnement.Afkoopsom = Afkoopsom.ToDecimal();
                            _Abonnement.Contractperiode = Contractperiode.ToInt16();
                            _Abonnement.Startdatum = Startdatum.ToDateTime();
                            _Abonnement.Einddatum = Einddatum.ToDateTime();
                            _Abonnement.AbonnementNaam = Product;
                            _Abonnement.MobileNr = ThisMobileNumber;
                            _Abonnement.SimNr = Sim;

                            AbonnementProduct _AbonnementProduct = new AbonnementProduct(session1);
                            _AbonnementProduct.ProductNaam = Bundel;
                            
                            //Verlengen knopje

                            //----------------------------------------------
                            // SIM Gegevens ophalen

                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/showSubscriberInfo.do?method=getSubsAddedInfo&income=detail&act=showResouce&subscriberID={0}&solenumber={2}&queryType=0&fromloginflag=true&selectedCustId={1}", strSubscriberId, strCustomerId, ThisMobileNumber)),
                                cookies,
                                "",
                                "",
                                certificate
                                );
                            //WriteBreakAndLine(strResponseData);

                            //html strippen
                            strResponseData = strResponseData.RemoveSpaceAndBreak();
                            string PIN1 = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "PIN1</td><td class=\"value\">", "&nbsp;</td>");
                            string PIN2 = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "PIN2</td><td class=\"value\">", "&nbsp;</td>");
                            string PUK1 = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "PUK1</td><td class=\"value\">", "&nbsp;</td>");
                            string PUK2 = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "PUK2</td><td class=\"value\">", "&nbsp;</td>");
                            
                            // waarde in object gooien!
                            _Abonnement.PIN1 = PIN1;
                            _Abonnement.PIN2 = PIN2;
                            _Abonnement.PUK1 = PUK1;
                            _Abonnement.PUK2 = PUK2;

                            _Abonnement.AbonnementProductCollection.Add(_AbonnementProduct);
                            _Contract.AbonnementCollection.Add(_Abonnement);
                            _Klant_Telfort.ContractCollection.Add(_Contract);
                            //----------------------------------------------

                            //Opslaan
                            session1.CommitChanges();

                            HttpWebResult.IsSuccess = true;

                            DateTime dtStartDate;

                            string strStartDate = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<td class=\"labelProductInfo\">Startdatum</td>", "</tr>");
                            strStartDate = HttpWebManager.ScrapeHelper.ExtractValue(strStartDate, "<td class=\"value\">", "</td>");
                            string strEndDate = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<td class=\"labelProductInfo\">Einddatum</td>", "</tr>");
                            strEndDate = HttpWebManager.ScrapeHelper.ExtractValue(strEndDate, "<td class=\"value\">", "</td>");

                            strStartDate = strStartDate.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("&nbsp;", "");
                            strEndDate = strEndDate.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("&nbsp;", "");

                            DateTime.TryParse(strStartDate.Trim(), out dtStartDate);
                            DateTime.TryParse(strEndDate.Trim(), out dtEndDate);

                            if (dtStartDate != DateTime.MinValue && dtEndDate != DateTime.MinValue)
                            {
                                //strRetentionResult = string.Format("{0}Startdatum{1}{2}{4}{3}{0}Einddatum{1}{2}{5}{3}{0}VBR segment{1}{2}{6}{3}{0}Verlengdatum{1}{2}{7}{3}{0}Verlengbaar{1}{2}{8}{3}",
                                //    "<tr><td width=\"20%\">", "</td>", "<td width=\"80%\">", "</td></tr>", dtStartDate.ToString("dd-MM-yyyy"), dtEndDate.ToString("dd-MM-yyyy"), strSegment,
                                //    dtEndDate.AddMonths(objRetentionInfo.Network.RetentionInMonths * -1).ToString("dd-MM-yyyy"), dtEndDate.AddMonths(objRetentionInfo.Network.RetentionInMonths * -1) <= DateTime.Today ? "Ja" : "Nee");
                            }
                            else
                            {
                                strRetentionResult = "!!!";// NOT_RETAINABLE;
                            }
                        }
                        else
                        {
                            strRetentionResult = strErrorMessage;
                        }
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
