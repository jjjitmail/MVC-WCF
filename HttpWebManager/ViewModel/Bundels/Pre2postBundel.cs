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
    public class Pre2postBundel : ScrapingBase
    {
        public Pre2postBundel() : base() {  }

        public override void Run()
        {
            //
        }

        private void Execute(Object sender, EventArgs e)
        {
            InitControlsBinding();
            InvokeBezig();
            string strRetentionResult = string.Empty;
            DateTime dtEndDate = DateTime.MinValue;            
            string ThisMobileNumber = Mobile;
            string SIM = Sim;
            this.HttpWebResult = new HttpWebResult();
            LoginInfo _LoginInfo = new LoginInfo();
            string strLoginUserName = _LoginInfo.LoginName, strLoginPassword = _LoginInfo.Password;

            string strResponseData = string.Empty, strErrorMessage = string.Empty;
            
            string Activatiedatum = DateTime.Today.AddDays(1).ToShortDateString();

            PersoonAanmelden _PersoonAanmelden = new PersoonAanmelden()
            {
                Initials = "W.J",
                MiddleNameBirth = "",
                BirthName = "Tam",
                MiddleName = "",
                LastName = "Tam",
                Gender = "1",
                BirthDate = "25-08-1972",
                PostCode = "2592VL",
                HouseNumber = "33",
                SuffixNumber = "",
                Street = "Roggekamp",
                Place = "'S-GRAVENHAGE",
                Nationality = "Netherlands",
                ContactTelephone = "",
                Email = "",
                IdType = "HolandDriverIC",
                CountryOfIssue = "NL",
                IdNumber = "4478578108",
                DateOfIssue = "12-09-2007",
                DateOfExpire = "12-09-2017",
                DealerCode = "Telecombinatie",
                OrgId = "",
                Street2 = "",
                Place2 = "",
                AccountType = "actpNormal",
                BillFormat = "bfOnline",
                BankAccountNumber = "128693649",
                BankAccountType = "Bank",
                PaymentType = "sttpDirectDebit"
            };

            string authKey = "", OperID = "";

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
                    using (UnitOfWork session1 = new UnitOfWork())
                    {
                        XPCollection<Lookup_AboBundel> _Lookup_AboBundelList = new XPCollection<Lookup_AboBundel>(session1);

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

                            _plist.ForEach(l =>
                                {
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
                                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/migration/pre2PostMigrationAction.do?method=checkCust&servNumber={0}&SSN=893126{1}&custType=PersonCustomer", ThisMobileNumber, SIM)),
                                        cookies,
                                        strResponseData,
                                        dictPostData,
                                        "",
                                        "",
                                        certificate
                                        );
                                    strResponseData = strResponseData.RemoveSpaceAndBreak();
                                    string custId = ScrapeHelper.ExtractValue(strResponseData, ":", " ");
                                    //
                                    dictPostData.Clear();
                                    dictPostData.Add("ssnPrefix", "893126");
                                    dictPostData.Add("m_SSN", string.Format("893126{0}", SIM));
                                    dictPostData.Add("custType", "PersonCustomer");
                                    dictPostData.Add("custid", custId);

                                    strResponseData = ScrapeHelper.DoPostPage(
                                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/migration/pre2PostMigrationAction.do?method=queryCustInfo&custid={0}&servNumber={1}&SSN=893126{2}&custType=PersonCustomer", custId, ThisMobileNumber, SIM)),
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
                                    string Productkeuze = l.Value;// ((int)Lookup_AboBundel.ProductKeuze.SIMONLY).ToString();
                                    dictPostData.Clear();
                                    dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                                    dictPostData.Add("initials", _PersoonAanmelden.Initials);
                                    dictPostData.Add("middleNameBirth", _PersoonAanmelden.MiddleNameBirth);
                                    dictPostData.Add("birthName", _PersoonAanmelden.BirthName);
                                    dictPostData.Add("middleName", _PersoonAanmelden.MiddleName);
                                    dictPostData.Add("lastName", _PersoonAanmelden.LastName);
                                    dictPostData.Add("gender", _PersoonAanmelden.Gender);
                                    dictPostData.Add("birthDate", _PersoonAanmelden.BirthDate);
                                    dictPostData.Add("postCode", _PersoonAanmelden.PostCode);
                                    dictPostData.Add("houseNumber", _PersoonAanmelden.HouseNumber);
                                    dictPostData.Add("suffixNumber", _PersoonAanmelden.SuffixNumber);
                                    dictPostData.Add("street", _PersoonAanmelden.Street);
                                    dictPostData.Add("place", _PersoonAanmelden.Place);
                                    dictPostData.Add("nationality", _PersoonAanmelden.Nationality);
                                    dictPostData.Add("contactTelephone", _PersoonAanmelden.ContactTelephone);
                                    dictPostData.Add("email", _PersoonAanmelden.Email);
                                    dictPostData.Add("idType", _PersoonAanmelden.IdType);
                                    dictPostData.Add("countryOfIssue", _PersoonAanmelden.CountryOfIssue);
                                    dictPostData.Add("idNumber", _PersoonAanmelden.IdNumber);
                                    dictPostData.Add("dateOfIssue", _PersoonAanmelden.DateOfIssue);
                                    dictPostData.Add("dateOfExpire", _PersoonAanmelden.DateOfExpire);
                                    dictPostData.Add("dealerCode", _PersoonAanmelden.DealerCode);
                                    dictPostData.Add("orgId", _PersoonAanmelden.OrgId);
                                    dictPostData.Add("street2", _PersoonAanmelden.Street2);
                                    dictPostData.Add("place2", _PersoonAanmelden.Place2);
                                    dictPostData.Add("accountType", _PersoonAanmelden.AccountType);
                                    dictPostData.Add("billFormat", _PersoonAanmelden.BillFormat);
                                    dictPostData.Add("bankAccountNumber", _PersoonAanmelden.BankAccountNumber);
                                    dictPostData.Add("bankAccountType", _PersoonAanmelden.BankAccountType);
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
                                            _PersoonAanmelden.Street, _PersoonAanmelden.Place, _PersoonAanmelden.Gender, _PersoonAanmelden.Nationality, _PersoonAanmelden.IdType, _PersoonAanmelden.CountryOfIssue, _PersoonAanmelden.AccountType, _PersoonAanmelden.BillFormat, _PersoonAanmelden.PaymentType, Productkeuze, org_apache_struts_taglib_html_TOKEN, _PersoonAanmelden.Initials, _PersoonAanmelden.BirthName, _PersoonAanmelden.LastName, _PersoonAanmelden.BirthDate, _PersoonAanmelden.PostCode, _PersoonAanmelden.HouseNumber, _PersoonAanmelden.Street, _PersoonAanmelden.Place, _PersoonAanmelden.IdNumber, _PersoonAanmelden.DateOfIssue, _PersoonAanmelden.DateOfExpire, _PersoonAanmelden.DealerCode, _PersoonAanmelden.BankAccountNumber, _PersoonAanmelden.BankAccountType)),
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
                                    Productkeuze = l.Value;// ((int)Lookup_AboBundel.ProductKeuze.SIMONLY).ToString();
                                    dictPostData.Clear();
                                    dictPostData.Add("org.apache.struts.taglib.html.TOKEN", org_apache_struts_taglib_html_TOKEN);
                                    dictPostData.Add("initials", _PersoonAanmelden.Initials);
                                    dictPostData.Add("middleNameBirth", _PersoonAanmelden.MiddleNameBirth);
                                    dictPostData.Add("birthName", _PersoonAanmelden.BirthName);
                                    dictPostData.Add("middleName", _PersoonAanmelden.MiddleName);
                                    dictPostData.Add("lastName", _PersoonAanmelden.LastName);
                                    dictPostData.Add("gender", _PersoonAanmelden.Gender);
                                    dictPostData.Add("birthDate", _PersoonAanmelden.BirthDate);
                                    dictPostData.Add("postCode", _PersoonAanmelden.PostCode);
                                    dictPostData.Add("houseNumber", _PersoonAanmelden.HouseNumber);
                                    dictPostData.Add("suffixNumber", _PersoonAanmelden.SuffixNumber);
                                    dictPostData.Add("street", _PersoonAanmelden.Street);
                                    dictPostData.Add("place", _PersoonAanmelden.Place);
                                    dictPostData.Add("nationality", _PersoonAanmelden.Nationality);
                                    dictPostData.Add("contactTelephone", _PersoonAanmelden.ContactTelephone);
                                    dictPostData.Add("email", _PersoonAanmelden.Email);
                                    dictPostData.Add("idType", _PersoonAanmelden.IdType);
                                    dictPostData.Add("countryOfIssue", _PersoonAanmelden.CountryOfIssue);
                                    dictPostData.Add("idNumber", _PersoonAanmelden.IdNumber);
                                    dictPostData.Add("dateOfIssue", _PersoonAanmelden.DateOfIssue);
                                    dictPostData.Add("dateOfExpire", _PersoonAanmelden.DateOfExpire);
                                    dictPostData.Add("dealerCode", _PersoonAanmelden.DealerCode);
                                    dictPostData.Add("orgId", _PersoonAanmelden.OrgId);
                                    dictPostData.Add("street2", _PersoonAanmelden.Street2);
                                    dictPostData.Add("place2", _PersoonAanmelden.Place2);
                                    dictPostData.Add("accountType", _PersoonAanmelden.AccountType);
                                    dictPostData.Add("billFormat", _PersoonAanmelden.BillFormat);
                                    dictPostData.Add("bankAccountNumber", _PersoonAanmelden.BankAccountNumber);
                                    dictPostData.Add("bankAccountType", _PersoonAanmelden.BankAccountType);
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
                                    Domain.InitProducts(SelectHTMLString).ForEach(item =>
                                    {
                                        Thread.Sleep(1000);
                                        dictPostData.Clear();
                                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getNetType&productId={0}&productName={1}&RelationType=&StallType=0", item.Value, item.Name)),
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
                                        //
                                        dictPostData.Clear();
                                        strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=getContractPeriodByProductId&productId={0}", item.Value)),
                                        cookies,
                                        strResponseData,
                                        dictPostData,
                                        "",
                                        "",
                                        certificate
                                        );

                                        strResponseData = strResponseData.RemoveSpaceAndBreak();

                                        Domain.InitContractPeriode(strResponseData).ForEach(m =>
                                        {
                                            Thread.Sleep(1000);
                                            dictPostData.Clear();
                                            strResponseData = HttpWebManager.ScrapeHelper.DoPostPage(
                                                new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/basebusiness/install/InstallSubsInitAction.do?method=saveSubsInfoForVasProduct&saveFlag=M&hidShowSubSeq=0&contractPeriod={0}&productId={1}&hidProductName={2}&txtActivationDate={3}&txtMsisdn={4}&txtSsn=893126{5}&txtImsi=&numberInclusion=NN&numberConcealed=undefined&canDoCustRec=false&chkGetVasProdFlag=false", m.Value, item.Value, item.Name, Activatiedatum, ThisMobileNumber, SIM)),
                                                cookies,
                                                strResponseData,
                                                dictPostData,
                                                "",
                                                "",
                                                certificate
                                            );
                                            //
                                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                                new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=select&isHasCautioner=&curProcductID={0}", item.Value)),
                                                cookies,
                                                "",
                                                "",
                                                certificate
                                                );
                                            //
                                            strResponseData = HttpWebManager.ScrapeHelper.DoGetPage(
                                                new Uri(string.Format("https://b2b.telfort.nl/custcare/product/selectProductAction.do?actionType=selectProduct&cmd=select&isHasCautioner=&isSimple=&curProcductID={0}", item.Value)),
                                                cookies,
                                                "",
                                                "",
                                                certificate
                                                );
                                            //

                                            //bundel scraping hier!!!
                                            strResponseData = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<body onLoad=\"doinit();", "</tbody>");
                                            strResponseData = strResponseData.RemoveSpaceAndBreak();
                                            string GroupName = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<tr basenode=\"1\" id=\"", "\"");

                                            string ProductGroupName = "", ProductGroupId = "";

                                            // bundel opties scraping
                                            while (strResponseData.Contains("<tr basenode=\"1\""))
                                            {
                                                string CheckNode = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<tr basenode=\"1\"", "</tr>");
                                                string CurrentEndNode = CheckNode.Contains("Dependent") ? "...</td></tr>" : "</tr>";
                                                string HtmlNode = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<tr basenode=\"1\"", CurrentEndNode);

                                                if ((HtmlNode.Contains("<input  type=\"checkbox\" needCount=true ")))
                                                {
                                                    ProductGroupId = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode, "<FONT class='blueTxt'><span id=\"corep\" title=\"", "\"");
                                                    ProductGroupName = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode, string.Format("<FONT class='blueTxt'><span id=\"corep\" title=\"{0}\">", ProductGroupId), "</span>");
                                                }
                                                if (HtmlNode.Contains("radio"))
                                                {
                                                    Thread.Sleep(1000);
                                                    string productID = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode, "productID=\"", "\"");
                                                    string id = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode, "id=\"", "\"");
                                                    string pid = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode, "pid=\"", "\"");
                                                    string nm = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode, productID + "\">", "</span>");
                                                    string info = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode + "</end>", "showtag", "</end>");

                                                    string tarief = HttpWebManager.ScrapeHelper.ExtractValue(info, "<td", "</td>");
                                                    string tariefValue = (tarief.Contains("document.write")) ? HttpWebManager.ScrapeHelper.ExtractValue(info, "document.write('", "'") : "";
                                                    info = info.Replace(string.Format("<td{0}</td>", tarief), "");

                                                    string promotieProcent = HttpWebManager.ScrapeHelper.ExtractValue(info, "<td", "</td>");
                                                    string promotieProcentValue = (promotieProcent.Contains("document.write")) ? HttpWebManager.ScrapeHelper.ExtractValue(info, "document.write('", "'") : "";
                                                    info = info.Replace(string.Format("<td{0}</td>", promotieProcent), "");

                                                    string tariefPromotie = HttpWebManager.ScrapeHelper.ExtractValue(info, "<td", "</td>");
                                                    string tariefPromotieValue = (tariefPromotie.Contains("document.write")) ? HttpWebManager.ScrapeHelper.ExtractValue(info, "document.write('", "'") : "";
                                                    info = info.Replace(string.Format("<td{0}</td>", tariefPromotie), "");

                                                    string kortingsperiode = HttpWebManager.ScrapeHelper.ExtractValue(info, "<td", "</td>");
                                                    string kortingsperiodeValue = HttpWebManager.ScrapeHelper.ExtractValue(info, ">&nbsp;", "<");
                                                    kortingsperiodeValue = string.IsNullOrEmpty(kortingsperiodeValue.Trim()) ? "0" : kortingsperiodeValue;
                                                    info = info.Replace(string.Format("<td{0}</td>", kortingsperiode), "");

                                                    string AdditionelePromotie = "";
                                                    string AdditionelePromotieValue = "";
                                                    if (HtmlNode.Contains("onmouseover=\"ddrivetip('<table><tr><td nowrap><nobr>"))
                                                    {
                                                        AdditionelePromotie = HttpWebManager.ScrapeHelper.ExtractValue(HtmlNode, "onmouseover=\"ddrivetip('<table><tr><td nowrap><nobr>", "', '','0');\" onmouseout=\"hideddrivetip();");
                                                        if (AdditionelePromotie != "")
                                                        {
                                                            AdditionelePromotie = string.Format("<table><tr><td nowrap><nobr>{0}", AdditionelePromotie);
                                                            AdditionelePromotieValue = Ult.AdditionelePromotieTekst(AdditionelePromotie);
                                                        }
                                                    }

                                                    // bundel opties verwerken
                                                    Lookup_AboBundel _Lookup_AboBundel = new Lookup_AboBundel(session1);
                                                    _Lookup_AboBundel.KeyCombo = string.Format("Pre2post;{0};{1};{2};{3}", item.Name, ProductGroupName, nm, m.Value);
                                                    _Lookup_AboBundel.BundelId = productID + Ult.CreateRandomCharacter(5) + m.Value;
                                                    _Lookup_AboBundel.IsNieuw = true;
                                                    _Lookup_AboBundel.ProductType = l.Value.ToInt16();// (int)Lookup_AboBundel.ProductKeuze.SIMONLY;
                                                    _Lookup_AboBundel.AboType = item.Name;
                                                    _Lookup_AboBundel.AboTypeValue = item.Value;
                                                    _Lookup_AboBundel.Name = nm;
                                                    _Lookup_AboBundel.NameId = productID;
                                                    _Lookup_AboBundel.Value = id;
                                                    _Lookup_AboBundel.ValueId = pid;
                                                    _Lookup_AboBundel.GroupName = GroupName;
                                                    _Lookup_AboBundel.ProductGroupName = ProductGroupName;
                                                    _Lookup_AboBundel.ProductGroupId = ProductGroupId;
                                                    _Lookup_AboBundel.Tarief = tariefValue;
                                                    _Lookup_AboBundel.PromotieProcent = promotieProcentValue;
                                                    _Lookup_AboBundel.TariefPromotie = tariefPromotieValue;
                                                    _Lookup_AboBundel.AdditionelePromotie = AdditionelePromotieValue; 
                                                    _Lookup_AboBundel.Kortingsperiode = kortingsperiodeValue.ToInt16();
                                                    _Lookup_AboBundel.AantalMaanden = m.Value.ToInt16();

                                                    _Lookup_AboBundel.Actief = true;

                                                    _Lookup_AboBundelList.Add(_Lookup_AboBundel);
                                                    //
                                                }
                                                strResponseData = strResponseData.Replace(string.Format("<tr basenode=\"1\"{0}{1}", HtmlNode, CurrentEndNode), "");
                                                //strResponseData = strResponseData.Replace(string.Format("<tr basenode=\"1\"{0}{1}", HtmlNode, strResponseData.Contains("Dependent") ? "...</td></tr>" : "</tr>"), "");
                                            }
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
                                        });
                                    });
                                });

                            try
                            {
                                // klaar met bundel data ophalen
                                session1.CommitChanges();
                                HttpWebResult.IsSuccess = true; 
                            }
                            catch (Exception err)
                            {
                                HttpWebResult.IsSuccess = false;
                                HttpWebResult.ErrorMessage = err.Message;
                                SerializationManager<XPCollection<Lookup_AboBundel>> _xlist = new SerializationManager<XPCollection<Lookup_AboBundel>>();
                                _xlist.Content = _Lookup_AboBundelList;
                                _xlist.Save();
                            }
                                                       
                        }
                    }
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
