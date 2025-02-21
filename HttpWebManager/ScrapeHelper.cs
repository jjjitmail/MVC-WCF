using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace HttpWebManager
{
    public class ScrapeHelper
    {
        private static string USER_AGENT = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; iOpus-I-M; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729";


        public static void DownloadPDF(string Url, string OrderId)
        {
            string FileName = String.Format(@"C:\Users\juntam\Desktop\Telfort\PDF\{0}.pdf", OrderId);
            using (var wc = new System.Net.WebClient())
            {
                wc.DownloadFile(Url, FileName);
            }
        }

        private static string ExtractViewState(string strHTML)
        {
            string viewStateNameDelimiter = "__VIEWSTATE";
            string valueDelimiter = "value=\"";

            int viewStateNamePosition = strHTML.IndexOf(viewStateNameDelimiter);

            if (viewStateNamePosition >= 0)
            {
                int viewStateValuePosition = strHTML.IndexOf(
                      valueDelimiter, viewStateNamePosition
                   );

                int viewStateStartPosition = viewStateValuePosition +
                                             valueDelimiter.Length;
                int viewStateEndPosition = strHTML.IndexOf("\"", viewStateStartPosition);

                return HttpUtility.UrlEncodeUnicode(
                         strHTML.Substring(
                            viewStateStartPosition,
                            viewStateEndPosition - viewStateStartPosition
                         )
                      );
            }

            return "";
        }
        private static string ExtractEventValidation(string strHTML)
        {
            string viewStateNameDelimiter = "__EVENTVALIDATION";
            string valueDelimiter = "value=\"";

            int viewStateNamePosition = strHTML.IndexOf(viewStateNameDelimiter);

            if (viewStateNamePosition >= 0)
            {
                int viewStateValuePosition = strHTML.IndexOf(
                      valueDelimiter, viewStateNamePosition
                   );

                int viewStateStartPosition = viewStateValuePosition +
                                             valueDelimiter.Length;
                int viewStateEndPosition = strHTML.IndexOf("\"", viewStateStartPosition);

                return HttpUtility.UrlEncodeUnicode(
                        strHTML.Substring(
                            viewStateStartPosition,
                            viewStateEndPosition - viewStateStartPosition
                        )
                    );
            }

            return "";
        }
        public static string ExtractValue(string strSearchString, string strValueToSearch, string strEndOfSearchString)
        {
            if (String.IsNullOrEmpty(strSearchString))
                return string.Empty;

            return ExtractValue(strSearchString, strValueToSearch, strEndOfSearchString, true);
        }
        public static string ExtractValue(string strSearchString, string strValueToSearch, string strEndOfSearchString, bool bExcludeSearchValue)
        {
            string strResult = string.Empty;

            if (String.IsNullOrEmpty(strSearchString))
                return strResult;

            int iIndexStart = strSearchString.IndexOf(strValueToSearch);
            int iIndexEnd = -1;

            if (iIndexStart >= 0)
            {
                if (bExcludeSearchValue)
                {
                    iIndexStart += strValueToSearch.Length;
                }

                iIndexEnd = strSearchString.IndexOf(strEndOfSearchString, iIndexStart);

                if (!bExcludeSearchValue)
                {
                    iIndexEnd += strEndOfSearchString.Length;
                }
            }

            if (iIndexEnd >= 0)
            {
                strResult = strSearchString.Substring(iIndexStart, iIndexEnd - iIndexStart).Replace("&amp;", "&");
            }

            return strResult;
        }
        internal static int CountStringOccurrences(string strText, string strPattern)
        {
            int count = 0;
            int i = 0;

            while ((i = strText.IndexOf(strPattern, i)) != -1)
            {
                i += strPattern.Length;
                count++;
            }

            return count;
        }
        public static string DoLogin(Uri loginUri, string strLogin, string strPassword, ref CookieContainer authCookies)
        {
            return DoLogin(loginUri, strLogin, strPassword, ref authCookies, null);
        }
        public static string DoLogin(Uri loginUri, string strLogin, string strPassword, ref CookieContainer authCookies, System.Security.Cryptography.X509Certificates.X509Certificate certificateToUse)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(Object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                System.Security.Cryptography.X509Certificates.X509Chain chain,
                                System.Net.Security.SslPolicyErrors errors)
            {
                return true;
            };

            authCookies = new CookieContainer();
            string strResponseData = string.Empty;

            HttpWebRequest webRequest = WebRequest.Create(loginUri.AbsoluteUri) as HttpWebRequest;
            webRequest.UserAgent = USER_AGENT;
            webRequest.CookieContainer = authCookies;

            if (certificateToUse != null)
            {
                webRequest.ClientCertificates.Add(certificateToUse);
            }

            if (!String.IsNullOrEmpty(strLogin) && !String.IsNullOrEmpty(strPassword))
            {
                webRequest.Credentials = new NetworkCredential(strLogin, strPassword);
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader responseReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    strResponseData = responseReader.ReadToEnd();

                    responseReader.Close();
                }
            }

            return strResponseData;
        }
        public static string DoGetPage(Uri pageUri, CookieContainer authCookies, string strLogin, string strPassword)
        {
            return DoGetPage(pageUri, authCookies, strLogin, strPassword, null);
        }
        public static string DoGetPage(Uri pageUri, CookieContainer authCookies, string strLogin, string strPassword, System.Security.Cryptography.X509Certificates.X509Certificate certificateToUse)
        {
            string strResponseData = string.Empty;

            HttpWebRequest webRequest = WebRequest.Create(pageUri.AbsoluteUri) as HttpWebRequest;
            webRequest.UserAgent = USER_AGENT;
            webRequest.CookieContainer = authCookies;

            if (certificateToUse != null)
            {
                webRequest.ClientCertificates.Add(certificateToUse);
            }

            if (!String.IsNullOrEmpty(strLogin) && !String.IsNullOrEmpty(strPassword))
            {
                webRequest.Credentials = new NetworkCredential(strLogin, strPassword);
            }

            using (HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    strResponseData = responseReader.ReadToEnd();

                    responseReader.Close();
                }
            }

            return strResponseData;
        }
        public static string DoPostPage(Uri postUri, CookieContainer authCookies, string strResponseData, Dictionary<string, string> dictPostData, string strLogin, string strPassword)
        {
            return DoPostPage(postUri, authCookies, strResponseData, dictPostData, strLogin, strPassword, null);
        }
        public static string DoPostPage(Uri postUri, CookieContainer authCookies, string strResponseData, Dictionary<string, string> dictPostData, string strLogin, string strPassword, System.Security.Cryptography.X509Certificates.X509Certificate certificateToUse)
        {
            StringBuilder sbPostData = new StringBuilder();
            string strPostData = string.Empty;

            foreach (KeyValuePair<string, string> kvpPostData in dictPostData)
            {
                sbPostData.Append(kvpPostData.Key);
                sbPostData.Append("=");

                if (kvpPostData.Key.ToUpper().Equals("__VIEWSTATE"))
                {
                    sbPostData.Append(ExtractViewState(strResponseData));
                }
                else if (kvpPostData.Key.ToUpper().Equals("__EVENTVALIDATION"))
                {
                    sbPostData.Append(ExtractEventValidation(strResponseData));
                }
                else if (kvpPostData.Key.Equals("SAMLRequest"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""SAMLRequest", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = HttpUtility.UrlEncodeUnicode(ExtractValue(strPostData, "value=\"", "\" />"));
                    }

                    sbPostData.Append(strPostData);
                }
                else if (kvpPostData.Key.Equals("RelayState"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""RelayState", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = ExtractValue(strPostData, "value=\"", "\" />");
                    }

                    sbPostData.Append(strPostData);
                }
                else if (kvpPostData.Key.Equals("SAMLart"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""SAMLart", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = ExtractValue(strPostData, "value=\"", "\" />");
                    }

                    sbPostData.Append(strPostData);
                }
                else
                {
                    sbPostData.Append(kvpPostData.Value);
                }

                sbPostData.Append("&");
            }

            if (sbPostData.Length > 0)
            {
                sbPostData.Remove(sbPostData.Length - 1, 1);
            }
                        
            HttpWebRequest webRequest = WebRequest.Create(postUri.AbsoluteUri) as HttpWebRequest;
            webRequest.UserAgent = USER_AGENT;
            webRequest.AllowAutoRedirect = true;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = authCookies;
            
            if (certificateToUse != null)
            {
                webRequest.ClientCertificates.Add(certificateToUse);
            }

            if (!String.IsNullOrEmpty(strLogin) && !String.IsNullOrEmpty(strPassword))
            {
                webRequest.Credentials = new NetworkCredential(strLogin, strPassword);
            }

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] requestData = encoding.GetBytes(sbPostData.ToString());
            webRequest.ContentLength = requestData.Length;

            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(requestData, 0, requestData.Length);
                requestStream.Close();
            }

            using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                strResponseData = responseReader.ReadToEnd();
                responseReader.Close();
            }

            return strResponseData;
        }

        public static bool DoPostPageSavePDF(Uri postUri, CookieContainer authCookies, string strResponseData, Dictionary<string, string> dictPostData,
            string strLogin, string strPassword, System.Security.Cryptography.X509Certificates.X509Certificate certificateToUse, string FileName)
        {
            StringBuilder sbPostData = new StringBuilder();
            string strPostData = string.Empty;
            bool Result = false;

            foreach (KeyValuePair<string, string> kvpPostData in dictPostData)
            {
                sbPostData.Append(kvpPostData.Key);
                sbPostData.Append("=");

                if (kvpPostData.Key.ToUpper().Equals("__VIEWSTATE"))
                {
                    sbPostData.Append(ExtractViewState(strResponseData));
                }
                else if (kvpPostData.Key.ToUpper().Equals("__EVENTVALIDATION"))
                {
                    sbPostData.Append(ExtractEventValidation(strResponseData));
                }
                else if (kvpPostData.Key.Equals("SAMLRequest"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""SAMLRequest", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = HttpUtility.UrlEncodeUnicode(ExtractValue(strPostData, "value=\"", "\" />"));
                    }

                    sbPostData.Append(strPostData);
                }
                else if (kvpPostData.Key.Equals("RelayState"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""RelayState", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = ExtractValue(strPostData, "value=\"", "\" />");
                    }

                    sbPostData.Append(strPostData);
                }
                else if (kvpPostData.Key.Equals("SAMLart"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""SAMLart", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = ExtractValue(strPostData, "value=\"", "\" />");
                    }

                    sbPostData.Append(strPostData);
                }
                else
                {
                    sbPostData.Append(kvpPostData.Value);
                }

                sbPostData.Append("&");
            }

            if (sbPostData.Length > 0)
            {
                sbPostData.Remove(sbPostData.Length - 1, 1);
            }
            
            HttpWebRequest webRequest = WebRequest.Create(postUri.AbsoluteUri) as HttpWebRequest;
            webRequest.UserAgent = USER_AGENT;
            webRequest.AllowAutoRedirect = true;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/pdf";
            webRequest.CookieContainer = authCookies;

            if (certificateToUse != null)
            {
                webRequest.ClientCertificates.Add(certificateToUse);
            }

            if (!String.IsNullOrEmpty(strLogin) && !String.IsNullOrEmpty(strPassword))
            {
                webRequest.Credentials = new NetworkCredential(strLogin, strPassword);
            }

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] requestData = encoding.GetBytes(sbPostData.ToString());
            webRequest.ContentLength = requestData.Length;
            
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(requestData, 0, requestData.Length);
                requestStream.Close();
            }

            try
            {
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseReader.SaveAsPdf(FileName);
                    //strResponseData = responseReader.ReadToEnd();
                    responseReader.Close();
                    Result = true;
                }
            }
            catch (Exception err)
            {
                string HH = err.Message;
            }
            return Result;
        }

        public static byte[] DoPostPagePDF(Uri postUri, CookieContainer authCookies, string strResponseData, Dictionary<string, string> dictPostData,
            string strLogin, string strPassword, System.Security.Cryptography.X509Certificates.X509Certificate certificateToUse)
        {
            StringBuilder sbPostData = new StringBuilder();
            string strPostData = string.Empty;

            foreach (KeyValuePair<string, string> kvpPostData in dictPostData)
            {
                sbPostData.Append(kvpPostData.Key);
                sbPostData.Append("=");

                if (kvpPostData.Key.ToUpper().Equals("__VIEWSTATE"))
                {
                    sbPostData.Append(ExtractViewState(strResponseData));
                }
                else if (kvpPostData.Key.ToUpper().Equals("__EVENTVALIDATION"))
                {
                    sbPostData.Append(ExtractEventValidation(strResponseData));
                }
                else if (kvpPostData.Key.Equals("SAMLRequest"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""SAMLRequest", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = HttpUtility.UrlEncodeUnicode(ExtractValue(strPostData, "value=\"", "\" />"));
                    }

                    sbPostData.Append(strPostData);
                }
                else if (kvpPostData.Key.Equals("RelayState"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""RelayState", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = ExtractValue(strPostData, "value=\"", "\" />");
                    }

                    sbPostData.Append(strPostData);
                }
                else if (kvpPostData.Key.Equals("SAMLart"))
                {
                    strPostData = ExtractValue(strResponseData, @"<input type=""hidden"" name=""SAMLart", "<");

                    if (!String.IsNullOrEmpty(strPostData))
                    {
                        strPostData = ExtractValue(strPostData, "value=\"", "\" />");
                    }

                    sbPostData.Append(strPostData);
                }
                else
                {
                    sbPostData.Append(kvpPostData.Value);
                }

                sbPostData.Append("&");
            }

            if (sbPostData.Length > 0)
            {
                sbPostData.Remove(sbPostData.Length - 1, 1);
            }

            HttpWebRequest webRequest = WebRequest.Create(postUri.AbsoluteUri) as HttpWebRequest;
            webRequest.UserAgent = USER_AGENT;
            webRequest.AllowAutoRedirect = true;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/pdf";
            webRequest.CookieContainer = authCookies;

            if (certificateToUse != null)
            {
                webRequest.ClientCertificates.Add(certificateToUse);
            }

            if (!String.IsNullOrEmpty(strLogin) && !String.IsNullOrEmpty(strPassword))
            {
                webRequest.Credentials = new NetworkCredential(strLogin, strPassword);
            }

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] requestData = encoding.GetBytes(sbPostData.ToString());
            webRequest.ContentLength = requestData.Length;

            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(requestData, 0, requestData.Length);
                requestStream.Close();
            }
            byte[] Overeenkomst = null;
            try
            {
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    Overeenkomst = responseReader.ConvertToByteArray();
                    responseReader.Close();
                }
            }
            catch (Exception err)
            {
                //
            }           

            return Overeenkomst;
        }
        
        private static void SaveMemoryStream(StreamReader responseReader)
        {
            var bytes = default(byte[]);
            using (var memstream = new MemoryStream())
            {
                responseReader.BaseStream.CopyTo(memstream);
                bytes = memstream.ToArray();
            }
            MemoryStream ms = new MemoryStream(bytes);

            FileStream outStream = File.OpenWrite(@"C:\Users\juntam\Desktop\Telfort\Verlengen\Data\testData\xxx2.pdf");
            ms.WriteTo(outStream);
            outStream.Flush();
            outStream.Close();
        }

        private static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}
