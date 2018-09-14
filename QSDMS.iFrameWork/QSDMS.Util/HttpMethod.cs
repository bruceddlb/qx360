using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Drawing;
using System.Collections.Specialized;
using System.Web;
using ICSharpCode.SharpZipLib.GZip;
using iFramework.Framework;

namespace QSDMS.Util
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpMethod
    {
        /// <summary>
        /// 
        /// </summary>
        public string DownLoadFile(string url, string filePath, string cookie)
        {
            HttpWebRequest request = null;
            HttpWebResponse oWebResp = null;
            StreamReader oStream = null;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Accept = "*/*";
                request.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                request.Headers["Accept-Language"] = "zh-cn";
                request.KeepAlive = false;

                request.CookieContainer = new CookieContainer();
                if (!string.IsNullOrEmpty(cookie))
                {
                    request.CookieContainer.SetCookies(request.RequestUri, cookie);
                }
                request.Timeout = 60000;

                oWebResp = (HttpWebResponse)request.GetResponse();

                CookieCollection tmpCookieCollection = oWebResp.Cookies;
                foreach (Cookie ck in tmpCookieCollection)
                {
                    cookie += ck.Name + "=" + ck.Value + ",";
                }

                Stream s = null;
                s = oWebResp.GetResponseStream();

                Image img = Image.FromStream(s);
                if (filePath.StartsWith("http://"))
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] byteImage = ms.ToArray();
                    return Convert.ToBase64String(byteImage);
                }
                else
                {
                    img.Save(filePath);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oStream != null)
                {
                    oStream.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HttpGet(string url, Encoding codeName, ref string cookie, bool isGzip = false, string referer = "", string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest request = null;
            HttpWebResponse oWebResp = null;
            StreamReader oStream = null;
            string sResp = "";
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Accept = "*/*";
                request.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                request.Headers["Accept-Language"] = "zh-cn";
                request.KeepAlive = false;

                if (referer != null)
                {
                    request.Referer = referer;
                }
                if (contentType != null)
                {
                    request.ContentType = contentType;
                }
                request.CookieContainer = new CookieContainer();
                if (!string.IsNullOrEmpty(cookie))
                {
                    request.CookieContainer.SetCookies(request.RequestUri, cookie);
                }
                if (isGzip)
                {
                    request.Headers["Accept-Encoding"] = "gzip, deflate";
                }
                request.Timeout = 60000;

                oWebResp = (HttpWebResponse)request.GetResponse();

                CookieCollection tmpCookieCollection = oWebResp.Cookies;
                foreach (Cookie ck in tmpCookieCollection)
                {
                    cookie += ck.Name + "=" + ck.Value + ",";
                }

                Stream s = null;
                if (isGzip)
                {
                    s = new GZipInputStream(oWebResp.GetResponseStream());
                }
                else
                {
                    s = oWebResp.GetResponseStream();
                }
                oStream = new StreamReader(s, codeName);
                sResp = oStream.ReadToEnd();
                oStream.Close();
                return sResp;
            }
            catch (Exception ex)
            {
                new ExceptionHelper().LogException(ex);
                return "";
            }
            finally
            {
                if (oStream != null)
                {
                    oStream.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HttpGet(string url, Encoding codeName, ref string cookie, ref MemoryStream sr, bool isGzip = false, string referer = "", string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest request = null;
            HttpWebResponse oWebResp = null;
            StreamReader oStream = null;
            string sResp = "";
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Accept = "*/*";
                request.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                request.Headers["Accept-Language"] = "zh-cn";
                request.KeepAlive = false;

                if (referer != null)
                {
                    request.Referer = referer;
                }
                if (contentType != null)
                {
                    request.ContentType = contentType;
                }
                request.CookieContainer = new CookieContainer();
                if (!string.IsNullOrEmpty(cookie))
                {
                    request.CookieContainer.SetCookies(request.RequestUri, cookie);
                }
                if (isGzip)
                {
                    request.Headers["Accept-Encoding"] = "gzip, deflate";
                }
                request.Timeout = 60000;

                oWebResp = (HttpWebResponse)request.GetResponse();

                CookieCollection tmpCookieCollection = oWebResp.Cookies;
                foreach (Cookie ck in tmpCookieCollection)
                {
                    cookie += ck.Name + "=" + ck.Value + ",";
                }

                Stream s = null;
                if (isGzip)
                {
                    s = new GZipInputStream(oWebResp.GetResponseStream());
                }
                else
                {
                    s = oWebResp.GetResponseStream();
                }
                oStream = new StreamReader(s, codeName);
                CopyStream(s, sr);
                sResp = oStream.ReadToEnd();
                oStream.Close();
                return sResp;
            }
            catch (Exception ex)
            {
                new ExceptionHelper().LogException(ex);
                return "";
            }
            finally
            {
                if (oStream != null)
                {
                    oStream.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CopyStream(Stream input, Stream output)
        {
            int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    return;
                }
                output.Write(buffer, 0, read);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="codeName"></param>
        /// <param name="cookie"></param>
        /// <param name="isGzip"></param>
        /// <param name="referer"></param>
        /// <param name="contentType"></param>
        /// <param name="certName">证书文件名称</param>
        /// <returns></returns>
        public string HttpPost(string url, string postData, Encoding codeName, ref string cookie, bool isGzip = false, string referer = "", string contentType = "application/x-www-form-urlencoded", string certName = "weixin.qq.com.cer")
        {
            HttpWebRequest myHttpWebRequest = null;
            Stream myRequestStream = null;
            StreamWriter myStreamWriter = null;
            HttpWebResponse myHttpWebResponse = null;
            string ContentType = contentType;

            try
            {
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.ContentType = ContentType;
                myHttpWebRequest.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                myHttpWebRequest.Accept = "*/*";

                //if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                //{
                //    try
                //    {
                //        myHttpWebRequest.Credentials = CredentialCache.DefaultCredentials;

                //        try
                //        {
                //            if (!string.IsNullOrWhiteSpace(certName))
                //            {
                //                string pfxPath = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\cert\\" + certName;
                //                if (File.Exists(pfxPath)) {
                //                    myHttpWebRequest.ClientCertificates.Add(new X509Certificate(pfxPath));
                //                }
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            new ExceptionHelper().LogException(ex);
                //        }
                //    }
                //    catch (Exception exx)
                //    {
                //        new ExceptionHelper().LogException(exx);
                //    }
                //}

                myHttpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                myHttpWebRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                myHttpWebRequest.ContentLength = Encoding.UTF8.GetBytes(postData).Length;
                myHttpWebRequest.Timeout = 30000;
                if (!string.IsNullOrEmpty(referer))
                {
                    myHttpWebRequest.Referer = referer;
                }
                if (isGzip)
                {
                    myHttpWebRequest.Headers["Accept-Encoding"] = "gzip, deflate";
                }
                myHttpWebRequest.CookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer.PerDomainCapacity = 100;
                if (!string.IsNullOrEmpty(cookie))
                {
                    myHttpWebRequest.CookieContainer.SetCookies(myHttpWebRequest.RequestUri, cookie);
                }
                myHttpWebRequest.Method = "POST";

                if (isGzip)
                {
                    myRequestStream = new GZipInputStream(myHttpWebRequest.GetRequestStream());
                }
                else
                {
                    myRequestStream = myHttpWebRequest.GetRequestStream();
                }

                myStreamWriter = new StreamWriter(myRequestStream);
                myStreamWriter.Write(postData);
                myStreamWriter.Close();
                myRequestStream.Close();

                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                CookieCollection tmpCookieCollection = myHttpWebResponse.Cookies;
                foreach (Cookie ck in tmpCookieCollection)
                {
                    cookie += ck.Name + "=" + ck.Value + ",";
                }
                if (isGzip)
                {
                    myRequestStream = new GZipStream(myHttpWebResponse.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    myRequestStream = myHttpWebResponse.GetResponseStream();
                }

                string result = new StreamReader(myRequestStream, codeName).ReadToEnd();

                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (myRequestStream != null)
                {
                    myRequestStream.Close();
                }
            }
        }

        public string HttpPost(string url,string fileName, byte[] fileBytes, string postData, ref string cookie, Encoding codeName, string contentType = "application/x-www-form-urlencoded") {
            Stream myRequestStream = null;
            var ContentType = contentType;
            string responseData = string.Empty; ;
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.ContentType = ContentType;
                myHttpWebRequest.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                myHttpWebRequest.Accept = "*/*";

                var webRequest = WebRequest.Create(url) as HttpWebRequest;
                if (webRequest == null) return string.Empty;

                webRequest.Method = "POST";
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 30000;
                webRequest.KeepAlive = true;

                NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                webRequest.PreAuthenticate = true;
                webRequest.AllowWriteStreamBuffering = true;

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] boundarybytes1 = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

                webRequest.ContentType = "multipart/form-data;boundary=" + boundary;

                Stream requestStream = webRequest.GetRequestStream();

                const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                MultipartformBody(qs, boundarybytes, boundarybytes1, requestStream, formdataTemplate);

                // Write file type to head
                const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = "";
                string fileType = System.IO.Path.GetExtension(fileName);
                if (string.Equals(fileType, ".jpg", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".jpeg", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".bmp", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".png", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".gif", StringComparison.CurrentCultureIgnoreCase))
                {
                    header = string.Format(headerTemplate, "media", string.Format("{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss")), "image/pjpeg");
                }

                byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                requestStream.Write(headerbytes, 0, headerbytes.Length);

                if (fileBytes != null&&fileBytes.Length>0) 
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                }
                // The trailer data
                byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                requestStream.Write(trailer, 0, trailer.Length);
                requestStream.Close();

                try
                {
                    using (WebResponse wr = webRequest.GetResponse())
                    {
                        Stream s = wr.GetResponseStream();
                        StreamReader sr = new StreamReader(s, codeName);
                        responseData = sr.ReadToEnd();
                    }
                }
                catch (WebException we)
                {
                    Stream s = we.Response.GetResponseStream();
                    StreamReader sr = new StreamReader(s, codeName);
                    responseData = sr.ReadToEnd();
                }
                catch
                {

                }
                finally
                {
                }

                return responseData;
            }
            catch
            {
                //throw ex;
                return "";
            }
            finally
            {
                if (myRequestStream != null)
                {
                    myRequestStream.Close();
                }
            }
        }

        /// <summary>
        /// 上传多媒体文件
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="filePath">文件名称(完整路径)</param>
        /// <param name="postData">post数据</param>
        /// <param name="cookie"></param>
        /// <param name="codeName">编码</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public string HttpPost(string url, string filePath, string postData, ref string cookie, Encoding codeName, string contentType = "application/x-www-form-urlencoded")
        {
            Stream myRequestStream = null;
            var ContentType = contentType;
            string responseData = string.Empty; ;
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.ContentType = ContentType;
                myHttpWebRequest.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                myHttpWebRequest.Accept = "*/*";

                var webRequest = WebRequest.Create(url) as HttpWebRequest;
                if (webRequest == null) return string.Empty;

                webRequest.Method = "POST";
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 30000;
                webRequest.KeepAlive = true;

                NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                webRequest.PreAuthenticate = true;
                webRequest.AllowWriteStreamBuffering = true;

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] boundarybytes1 = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

                webRequest.ContentType = "multipart/form-data;boundary=" + boundary;

                Stream requestStream = webRequest.GetRequestStream();

                const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                MultipartformBody(qs, boundarybytes, boundarybytes1, requestStream, formdataTemplate);

                // Write file type to head
                const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = "";
                string fileType = System.IO.Path.GetExtension(filePath);
                if (string.Equals(fileType, ".jpg", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".jpeg", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".bmp", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".png", StringComparison.CurrentCultureIgnoreCase) || string.Equals(fileType, ".gif", StringComparison.CurrentCultureIgnoreCase))
                {
                    header = string.Format(headerTemplate, "media", string.Format("{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss")), "image/pjpeg");
                }

                byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                requestStream.Write(headerbytes, 0, headerbytes.Length);

                // Write picture file binary to post data
                if (!string.IsNullOrWhiteSpace(filePath)) {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                // The trailer data
                byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                requestStream.Write(trailer, 0, trailer.Length);
                requestStream.Close();

                try
                {
                    using (WebResponse wr = webRequest.GetResponse())
                    {
                        Stream s = wr.GetResponseStream();
                        StreamReader sr = new StreamReader(s, codeName);
                        responseData = sr.ReadToEnd();
                    }
                }
                catch (WebException we)
                {
                    Stream s = we.Response.GetResponseStream();
                    StreamReader sr = new StreamReader(s, codeName);
                    responseData = sr.ReadToEnd();
                }
                catch
                {

                }
                finally
                {
                }

                return responseData;
            }
            catch
            {
                //throw ex;
                return "";
            }
            finally
            {
                if (myRequestStream != null)
                {
                    myRequestStream.Close();
                }
            }
        }

/// <summary>
        /// post方式，带上传文件功能 add by bruced
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="filePath">文件名称(完整路径)</param>
        /// <param name="postData">post数据</param>
        /// <param name="cookie"></param>
        /// <param name="codeName">编码</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public string HttpPost(string url, HttpPostedFileBase postedFile, Dictionary<string, object> parameters, Encoding codeName, string contentType = "application/x-www-form-urlencoded")
        {
            string responseData = string.Empty;
            //1>创建请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //2>Cookie容器
            //request.CookieContainer = cookies;
            request.Method = "POST";
            request.Timeout = 100000;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.KeepAlive = true;
            request.ContentType = contentType;
            request.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
            request.Accept = "*/*";
            //分界线
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            //内容类型
            request.ContentType = "multipart/form-data; boundary=" + boundary; ;

            //3>表单数据模板
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            try
            {
                using (Stream stream = request.GetRequestStream())
                {                    
                    //写入请求流
                    if (null != parameters)
                    {
                        foreach (KeyValuePair<string, object> item in parameters)
                        {
                            stream.Write(boundaryBytes, 0, boundaryBytes.Length);//写入分界线
                            byte[] formBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(formdataTemplate, item.Key, item.Value));
                            stream.Write(formBytes, 0, formBytes.Length);
                        }
                    }
                    //6.0>分界线============================================注意：缺少次步骤，可能导致远程服务器无法获取Request.Files集合
                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    //文件流
                    if (postedFile != null)
                    {                        
                        //4>读取流
                        byte[] buffer = new byte[postedFile.ContentLength];
                        postedFile.InputStream.Read(buffer, 0, buffer.Length);

                        //5>写入请求流数据
                        string strHeader = "Content-Disposition:application/x-www-form-urlencoded; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
                        strHeader = string.Format(strHeader,
                                                 "media",
                                                 postedFile.FileName,
                                                 postedFile.ContentType);
                        //6>HTTP请求头
                        byte[] byteHeader = System.Text.ASCIIEncoding.ASCII.GetBytes(strHeader);
                        //6.1>请求头
                        stream.Write(byteHeader, 0, byteHeader.Length);
                        //6.2>把文件流写入请求流
                        stream.Write(buffer, 0, buffer.Length);
                    }

                    //6.3>写入分隔流
                    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    stream.Write(trailer, 0, trailer.Length);
                    //6.4>关闭流
                    stream.Close();
                }               
                using (WebResponse wr = request.GetResponse())
                {
                    Stream s = wr.GetResponseStream();
                    StreamReader sr = new StreamReader(s, codeName);
                    responseData = sr.ReadToEnd();
                    sr.Dispose();
                    s.Dispose();
                }
            }
            catch (Exception ex)
            {                
                //throw new Exception("上传文件时远程服务器发生异常！", ex);
            }

            return responseData;
        }


        /// <summary>
        /// 退款申请专用方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="codeName"></param>
        /// <param name="pfxPath">证书路径</param>
        /// <param name="pfxPwd">证书密码</param>
        /// <param name="refund"></param>
        /// <param name="cookie"></param>
        /// <param name="isGzip"></param>
        /// <param name="referer"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public string HttpPost(string url, string postData, Encoding codeName, string pfxPath, string pfxPwd, bool refund, ref string cookie, bool isGzip = false, string referer = "", string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest myHttpWebRequest = null;
            Stream myRequestStream = null;
            StreamWriter myStreamWriter = null;
            HttpWebResponse myHttpWebResponse = null;
            string ContentType = contentType;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.ContentType = ContentType;
                myHttpWebRequest.UserAgent = "Mozilla-Firefox-Spider(Wenanry)";
                myHttpWebRequest.Accept = "*/*";
                myHttpWebRequest.ServicePoint.Expect100Continue = false;

                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        myHttpWebRequest.Credentials = CredentialCache.DefaultCredentials;

                        try
                        {

                            if (!string.IsNullOrEmpty(pfxPath) && !string.IsNullOrEmpty(pfxPwd))
                            {
                                X509Certificate cerCaiShang = new X509Certificate(pfxPath, pfxPwd);
                                myHttpWebRequest.ClientCertificates.Add(cerCaiShang);
                            }
                        }
                        catch (Exception ex)
                        {
                            new ExceptionHelper().LogException(ex);
                        }
                    }
                    catch (Exception exx)
                    {
                        new ExceptionHelper().LogException(exx);
                    }
                }

                myHttpWebRequest.Headers.Add("Origin", "https://mp.weixin.qq.com");
                myHttpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                myHttpWebRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=GBK";
                myHttpWebRequest.Timeout = 30000;
                if (!string.IsNullOrEmpty(referer))
                {
                    myHttpWebRequest.Referer = referer;
                }
                if (isGzip)
                {
                    myHttpWebRequest.Headers["Accept-Encoding"] = "gzip, deflate";
                }
                myHttpWebRequest.CookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer.PerDomainCapacity = 100;
                if (!string.IsNullOrEmpty(cookie))
                {
                    myHttpWebRequest.CookieContainer.SetCookies(myHttpWebRequest.RequestUri, cookie);
                }
                myHttpWebRequest.Method = "POST";

                if (isGzip)
                {
                    myRequestStream = new GZipInputStream(myHttpWebRequest.GetRequestStream());
                }
                else
                {
                    myRequestStream = myHttpWebRequest.GetRequestStream();
                }

                myStreamWriter = new StreamWriter(myRequestStream);
                myStreamWriter.Write(postData);
                myStreamWriter.Close();
                myRequestStream.Close();

                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                CookieCollection tmpCookieCollection = myHttpWebResponse.Cookies;
                foreach (Cookie ck in tmpCookieCollection)
                {
                    cookie += ck.Name + "=" + ck.Value + ",";
                }
                //这个地方需要采用Gizp解压
                //当通过退款通道请求的时候,必须采用Gizp解压
                if (!isGzip)
                {
                    myRequestStream = new GZipStream(myHttpWebResponse.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    myRequestStream = myHttpWebResponse.GetResponseStream();
                }

                string result = new StreamReader(myRequestStream, codeName).ReadToEnd();

                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (myRequestStream != null)
                {
                    myRequestStream.Close();
                }
            }
        }

        /// <summary>
        /// 订单查询专用,不可修改
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="codeName"></param>
        /// <param name="pfxPath">证书路径</param>
        /// <param name="pfxPwd">证书密码</param>
        /// <param name="cookie"></param>
        /// <param name="isGzip"></param>
        /// <param name="referer"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public string HttpPost(string url, string postData, Encoding codeName, string pfxPath, string pfxPwd, ref string cookie, bool isGzip = false, string referer = "", string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest myHttpWebRequest = null;
            Stream myRequestStream = null;
            StreamWriter myStreamWriter = null;
            HttpWebResponse myHttpWebResponse = null;
            string ContentType = contentType;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.ContentType = ContentType;
                //myHttpWebRequest.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                myHttpWebRequest.UserAgent = "Mozilla-Firefox-Spider(Wenanry)";
                myHttpWebRequest.Accept = "*/*";
                myHttpWebRequest.ServicePoint.Expect100Continue = false;

                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        myHttpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                        try
                        {
                            if (!string.IsNullOrEmpty(pfxPath) && !string.IsNullOrEmpty(pfxPwd))
                            {
                                X509Certificate cerCaiShang = new X509Certificate(pfxPath, pfxPwd);
                                myHttpWebRequest.ClientCertificates.Add(cerCaiShang);
                            }
                        }
                        catch (Exception ex)
                        {
                            new ExceptionHelper().LogException(ex);
                        }
                    }
                    catch (Exception exx)
                    {
                        new ExceptionHelper().LogException(exx);
                    }
                }

                myHttpWebRequest.Headers.Add("Origin", "https://mp.weixin.qq.com");
                myHttpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                myHttpWebRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=GBK";
                myHttpWebRequest.Timeout = 30000;
                if (!string.IsNullOrEmpty(referer))
                {
                    myHttpWebRequest.Referer = referer;
                }
                if (isGzip)
                {
                    myHttpWebRequest.Headers["Accept-Encoding"] = "gzip, deflate";
                }
                myHttpWebRequest.CookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer.PerDomainCapacity = 100;
                if (!string.IsNullOrEmpty(cookie))
                {
                    myHttpWebRequest.CookieContainer.SetCookies(myHttpWebRequest.RequestUri, cookie);
                }
                myHttpWebRequest.Method = "POST";

                if (isGzip)
                {
                    myRequestStream = new GZipInputStream(myHttpWebRequest.GetRequestStream());
                }
                else
                {
                    myRequestStream = myHttpWebRequest.GetRequestStream();
                }

                myStreamWriter = new StreamWriter(myRequestStream);
                myStreamWriter.Write(postData);
                myStreamWriter.Close();
                myRequestStream.Close();

                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                CookieCollection tmpCookieCollection = myHttpWebResponse.Cookies;
                foreach (Cookie ck in tmpCookieCollection)
                {
                    cookie += ck.Name + "=" + ck.Value + ",";
                }

                if (isGzip)
                {
                    myRequestStream = new GZipStream(myHttpWebResponse.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    myRequestStream = myHttpWebResponse.GetResponseStream();
                }

                string result = new StreamReader(myRequestStream, codeName).ReadToEnd();

                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (myRequestStream != null)
                {
                    myRequestStream.Close();
                }
            }
        }

        /// <summary>
        /// 对账单专用
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="codeName"></param>
        /// <param name="isSoai">是否由对账单转入</param>
        /// <param name="cookie"></param>
        /// <param name="isGzip"></param>
        /// <param name="referer"></param>
        /// <param name="contentType"></param>
        /// <param name="certName">证书路径</param>
        /// <returns></returns>
        public string HttpPost(string url, string postData, Encoding codeName, bool isSoai, ref string cookie, bool isGzip = false, string referer = "", string contentType = "application/x-www-form-urlencoded", string certName = "weixin.qq.com.cer")
        {
            HttpWebRequest myHttpWebRequest = null;
            Stream myRequestStream = null;
            StreamWriter myStreamWriter = null;
            HttpWebResponse myHttpWebResponse = null;
            string ContentType = contentType;

            try
            {
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.ContentType = ContentType;
                myHttpWebRequest.UserAgent = "Mozilla/5.2 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                myHttpWebRequest.Accept = "*/*";

                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        myHttpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(certName))
                            {
                                string pfxPath = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\cert\\" + certName;
                                myHttpWebRequest.ClientCertificates.Add(new X509Certificate(pfxPath));
                            }
                        }
                        catch (Exception ex)
                        {
                            new ExceptionHelper().LogException(ex);
                        }
                    }
                    catch (Exception exx)
                    {
                        new ExceptionHelper().LogException(exx);
                    }
                }

                myHttpWebRequest.Headers.Add("Origin", "https://mp.weixin.qq.com");
                myHttpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                myHttpWebRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"; //对账单必须为utf-8
                myHttpWebRequest.Timeout = 30000;
                if (!string.IsNullOrEmpty(referer))
                {
                    myHttpWebRequest.Referer = referer;
                }
                if (isGzip)
                {
                    myHttpWebRequest.Headers["Accept-Encoding"] = "gzip, deflate";
                }
                myHttpWebRequest.CookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer.PerDomainCapacity = 100;
                if (!string.IsNullOrEmpty(cookie))
                {
                    myHttpWebRequest.CookieContainer.SetCookies(myHttpWebRequest.RequestUri, cookie);
                }
                myHttpWebRequest.Method = "POST";

                //对账单处理
                if (!isGzip)
                {
                    myRequestStream = new GZipInputStream(myHttpWebRequest.GetRequestStream());
                }
                else
                {
                    myRequestStream = myHttpWebRequest.GetRequestStream();
                }

                myStreamWriter = new StreamWriter(myRequestStream);
                myStreamWriter.Write(postData);
                myStreamWriter.Close();
                myRequestStream.Close();

                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                CookieCollection tmpCookieCollection = myHttpWebResponse.Cookies;
                foreach (Cookie ck in tmpCookieCollection)
                {
                    cookie += ck.Name + "=" + ck.Value + ",";
                }

                if (string.IsNullOrWhiteSpace(myHttpWebResponse.ContentEncoding))
                {
                    myRequestStream = myHttpWebResponse.GetResponseStream();
                }
                else
                {
                    if (isGzip)
                    {
                        myRequestStream = new GZipStream(myHttpWebResponse.GetResponseStream(), CompressionMode.Decompress);
                    }
                    else
                    {
                        myRequestStream = myHttpWebResponse.GetResponseStream();
                    }
                }

                //解码时用gbk
                string result = new StreamReader(myRequestStream, codeName).ReadToEnd();

                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (myRequestStream != null)
                {
                    myRequestStream.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void MultipartformBody(NameValueCollection qs, byte[] boundarybytes, byte[] boundarybytes1, Stream requestStream, string formdataTemplate)
        {
            requestStream.Write(boundarybytes1, 0, boundarybytes1.Length);
            string key = "status";
            string formitem = string.Format(formdataTemplate, key, qs[key]);
            byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
            requestStream.Write(formitembytes, 0, formitembytes.Length);
            requestStream.Write(boundarybytes, 0, boundarybytes.Length);
            string formitem1 = string.Format(formdataTemplate, "source", qs["source"]);
            byte[] formitembytes1 = Encoding.UTF8.GetBytes(formitem1);
            requestStream.Write(formitembytes1, 0, formitembytes1.Length);
            requestStream.Write(boundarybytes, 0, boundarybytes.Length);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    class CertPolicy : ICertificatePolicy
    {
        /// <summary>
        /// 
        /// </summary>
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }
}
