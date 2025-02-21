using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace HttpWebManager
{
    public class RetentionCheckerInfo
    {
        public XpoObjects.Network Network { get; set; }
        public string MobileNumber { get; set; }
        public string SIMNumber { get; set; }
        public DateTime BirthDay { get; set; }

        public RetentionCheckerInfo(XpoObjects.Network Network, string MobileNumber, string SIMNumber, DateTime BirthDay)
        {
            this.Network = Network;
            this.MobileNumber = MobileNumber;
            this.SIMNumber = SIMNumber;
            this.BirthDay = BirthDay;
        }
    }

    public static class RetentionChecker
    {
        private const string NOT_RETAINABLE = "Het is niet te bepalen of het mobiele nummer te verlengen is";

        public static string GetRetentionResult(RetentionCheckerInfo objRetentionInfo, out DateTime dtEndDate)
        {
            return GetRetentionInfo(objRetentionInfo, out dtEndDate);
        }
        private static string GetRetentionInfo(RetentionCheckerInfo objRetentionInfo, out DateTime dtEndDate)
        {
            string strRetentionResult = string.Empty;
            dtEndDate = DateTime.MinValue;

            string strLoginUserName = string.Empty, strLoginPassword = string.Empty;
            string strResponseData = string.Empty, strErrorMessage = string.Empty;
            System.Collections.Generic.Dictionary<string, string> dictPostData = new System.Collections.Generic.Dictionary<string, string>();
            CookieContainer cookies = null;

            if (objRetentionInfo.Network != null)
            {
                strLoginUserName = objRetentionInfo.Network.RetentionCheckUserName;
                strLoginPassword = objRetentionInfo.Network.RetentionCheckPassword;

                switch (objRetentionInfo.Network.Name)
                {                    
                    case "Telfort":
                        try
                        {
                            System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store("TrustedPeople",
                                System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);
                            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly | System.Security.Cryptography.X509Certificates.OpenFlags.OpenExistingOnly);

                            System.Security.Cryptography.X509Certificates.X509Certificate2Collection certificateCollection = store.Certificates.Find(System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "tft.notify@prolocation.net", false);

                            if (certificateCollection.Count > 0)
                            {
                                System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = certificateCollection[0];

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

                                    strResponseData = ScrapeHelper.DoGetPage(
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
                                            dictPostData.Add("LoginAuthKey", strValue);
                                            strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"bossJSessionID\" value=\"", "\"");
                                            dictPostData.Add("bossJSessionID", strValue);
                                            strValue = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"Language\" value=\"", "\"");
                                            dictPostData.Add("Language", strValue);
                                            strValue = ScrapeHelper.ExtractValue(strResponseData, "<input type=\"hidden\" name=\"sid\" value=\"", "\"");
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
                                    dictPostData.Add("firstLoginNumber", objRetentionInfo.MobileNumber);
                                    dictPostData.Add("firstLoginType", "");
                                    dictPostData.Add("secondQuickLoginType", "6");
                                    dictPostData.Add("secondLoginNumber", objRetentionInfo.SIMNumber);
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

                                    string strCustomerId = ScrapeHelper.ExtractValue(strResponseData, "loginBy('", "'");

                                    strResponseData = ScrapeHelper.DoGetPage(
                                       new Uri(string.Format("https://b2b.telfort.nl/custcare/cc_common/CustomerInfoAction.do?act=qryCustomerInfo&ONLYLOGIN=onlyLogin&&loginLevel=1&checkPassword=1&authType=AuthTypeCustID&objectID={0}&loginMsisdn={1}", strCustomerId, objRetentionInfo.MobileNumber)),
                                       cookies,
                                       "",
                                       "",
                                       certificate
                                       );

                                    strResponseData = ScrapeHelper.DoGetPage(
                                        new Uri("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/showCustomerInfo.do?method=initForLogin"),
                                        cookies,
                                        "",
                                        "",
                                        certificate
                                        );

                                    string strSubscriberId = ScrapeHelper.ExtractValue(strResponseData, "top.subsID4IPCC = \"", "\"");

                                    strResponseData = ScrapeHelper.DoGetPage(
                                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/personCustomerInfoAction.do?method=showPersonInfo&showType=showDetail&custType=PersonCustomer&selectedCustId={0}&msisdns={1}&fromloginflag=true", strCustomerId, objRetentionInfo.MobileNumber)),
                                        cookies,
                                        "",
                                        "",
                                        certificate
                                        );

                                    string strSegment = ScrapeHelper.ExtractValue(strResponseData, "<select name=\"acct_customerLevel\"", "</select>");
                                    strSegment = ScrapeHelper.ExtractValue(strSegment, "selected=\"selected\">", "</option>");

                                    strResponseData = ScrapeHelper.DoGetPage(
                                        new Uri(string.Format("https://b2b.telfort.nl/custcare/custsvc/servicequery/customerInfoQuery/showSubscriberInfo.do?method=showSubInfoDetail&income=detail&act=productinfo&subscriberID={0}&fromloginflag=true&selectedCustId={1}", strSubscriberId, strCustomerId)),
                                        cookies,
                                        "",
                                        "",
                                        certificate
                                        );

                                    DateTime dtStartDate;

                                    string strStartDate = ScrapeHelper.ExtractValue(strResponseData, "<td class=\"labelProductInfo\">Startdatum</td>", "</tr>");
                                    strStartDate = ScrapeHelper.ExtractValue(strStartDate, "<td class=\"value\">", "</td>");
                                    string strEndDate = ScrapeHelper.ExtractValue(strResponseData, "<td class=\"labelProductInfo\">Einddatum</td>", "</tr>");
                                    strEndDate = ScrapeHelper.ExtractValue(strEndDate, "<td class=\"value\">", "</td>");

                                    strStartDate = strStartDate.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("&nbsp;", "");
                                    strEndDate = strEndDate.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("&nbsp;", "");

                                    DateTime.TryParse(strStartDate.Trim(), out dtStartDate);
                                    DateTime.TryParse(strEndDate.Trim(), out dtEndDate);

                                    if (dtStartDate != DateTime.MinValue && dtEndDate != DateTime.MinValue)
                                    {
                                        strRetentionResult = string.Format("{0}Startdatum{1}{2}{4}{3}{0}Einddatum{1}{2}{5}{3}{0}VBR segment{1}{2}{6}{3}{0}Verlengdatum{1}{2}{7}{3}{0}Verlengbaar{1}{2}{8}{3}",
                                            "<tr><td width=\"20%\">", "</td>", "<td width=\"80%\">", "</td></tr>", dtStartDate.ToString("dd-MM-yyyy"), dtEndDate.ToString("dd-MM-yyyy"), strSegment,
                                            dtEndDate.AddMonths(objRetentionInfo.Network.RetentionInMonths * -1).ToString("dd-MM-yyyy"), dtEndDate.AddMonths(objRetentionInfo.Network.RetentionInMonths * -1) <= DateTime.Today ? "Ja" : "Nee");
                                    }
                                    else
                                    {
                                        strRetentionResult = NOT_RETAINABLE;
                                    }
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
                        catch (Exception ex)
                        {
                            strRetentionResult = "<span style=\"color: red;\">Er is een fout opgetreden: " + ex.Message + "</span>";

                            //Utils.SiteHelper.SendErrorEmail("Verlengingschecker (" + objRetentionInfo.Network.Name + ", " + objRetentionInfo.MobileNumber + ", " + objRetentionInfo.SIMNumber + ")", ex, HttpContext.Current.User.Identity.Name);
                        }
                        break;
                }
            }

            return strRetentionResult;
        }
    }
}
