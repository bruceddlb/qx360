using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CM = System.Configuration.ConfigurationManager;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using iFramework.Framework;

namespace QSDMS.Util
{
    /// <summary>
    /// 文本处理类
    /// </summary>
    public class TextHelper
    {
        private static Dictionary<string, string> mapping = new Dictionary<string, string>();
        private static bool IsDNAT = false;

        static TextHelper()
        {
            mapping.Add("/", "!-!");
            mapping.Add("=", "-!-");
            mapping.Add("+", "*!*");
        }

        #region 获取webconfig当中appSetting中的配置项
        /// <summary>
        /// 获取webconfig当中appSetting中的配置项
        /// </summary>
        /// <param name="key">配置项的键</param>
        /// <returns>配置项的值</returns>
        public static string GetConfigItem(string key)
        {
            string result = string.Empty;
            try
            {
                result = CM.AppSettings[key];
            }
            catch { }
            return result;
        }
        #endregion

        #region 获取webconfig当中appSetting中的配置项
        /// <summary>
        /// 获取webconfig当中appSetting中的配置项
        /// </summary>
        /// <param name="key">配置项的键</param>
        /// <returns>配置项的值</returns>
        public static T GetConfigItem<T>(string key)
        {
            try
            {
                return (T)Convert.ChangeType(CM.AppSettings[key], typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
        #endregion

        #region 获取数据库配置项
        /// <summary>
        /// 获取数据库配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetDataConfigItem(string key)
        {
            string result = string.Empty;
            try
            {
                result = CM.ConnectionStrings[key].ConnectionString;
            }
            catch { }
            return result;
        }
        #endregion

        #region 对指定的字符串,进行md5加密散列值计算
        /// <summary>
        /// 对指定的字符串,进行md5加密散列值计算
        /// </summary>
        /// <param name="source">需要md5加密的字符串值</param>
        /// <returns>md5值</returns>
        public static string MD5(string source)
        {
            byte[] result = Encoding.Default.GetBytes(source);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            return ByteArrayToHexString(md5.ComputeHash(result));
        }
        #endregion

        #region 把对应的字节值转换为对应的字符串
        /// <summary>
        /// 把对应的字节值转换为对应的字符串
        /// </summary>
        /// <param name="values">字节值</param>
        /// <returns>字符串</returns>
        private static string ByteArrayToHexString(byte[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte value in values)
            {
                sb.AppendFormat("{0:X2}", value);
            }
            return sb.ToString();
        }
        #endregion

        #region 字符串转成 column in ('','','','',)的条件字符串
        /// <summary>
        ///  字符串转成 column in ('','','','',)的条件字符串
        /// </summary>
        /// <param name="strings">字符串</param>
        /// <param name="split">拆分字符</param>
        /// <returns></returns>
        public static string ConvertStringArrayToWhere(string strings, string split)
        {
            string[] array = strings.Split(split.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return ConvertStringArrayToWhere(array);
        }

        /// <summary>
        /// 组合数组为('','','','',)的条件字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>query的条件</returns>
        public static string ConvertStringArrayToWhere(string[] array)
        {
            //组合guid为query的条件
            StringBuilder where = new StringBuilder();
            where.Append("('");
            where.Append(string.Join("','", array));
            where.Append("')");

            return where.ToString();
        }

        /// <summary>
        /// 组合数组为('','','','',)的条件字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>query的条件</returns>
        public static string ConvertStringArrayToWhere(IEnumerable<string> array)
        {
            return ConvertStringArrayToWhere(array.ToArray());
        }

        /// <summary>
        /// 组合数组为('','','','',)的条件字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>query的条件</returns>
        public static string ConvertIntArrayToWhere(IEnumerable<int> array)
        {
            //组合guid为query的条件
            StringBuilder where = new StringBuilder();
            where.Append("('");
            where.Append(string.Join("','", array));
            where.Append("')");

            return where.ToString();
        }
        /// <summary>
        /// 把字符串转换为ModID的条件字符串
        /// </summary>
        /// <param name="strings">品牌ID逗号分割字符串</param>
        /// <param name="split">分隔符</param>
        /// <param name="Mod">余数</param>
        /// <returns>把字符串转换为ModID的条件字符串</returns>
        public static string ConvertIntArrayToModIDWhere(string strings, string split, int Mod = 50)
        {
            string[] array = strings.Split(split.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return ConvertIntArrayToModIDWhere(array, Mod);
        }

        /// <summary>
        /// 组合数组为('','','','',)的条件字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="Mod">余数</param>
        /// <returns>query的条件</returns>
        public static string ConvertIntArrayToModIDWhere(IEnumerable<int> array, int Mod = 50)
        {
            //组合guid为query的条件
            List<int> temp = array.ToList();

            for (int i = 0; i < temp.Count; i++)
            {
                temp[i] = temp[i] % Mod;
            }

            StringBuilder where = new StringBuilder();
            where.Append("('");
            where.Append(string.Join("','", temp));
            where.Append("')");

            return where.ToString();
        }

        /// <summary>
        /// 组合数组为('','','','',)的条件字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="Mod">余数</param>
        /// <returns>query的条件</returns>
        public static string ConvertIntArrayToModIDWhere(IEnumerable<string> array, int Mod = 50)
        {
            //组合guid为query的条件
            List<int> temp = array.Select(p => Convert.ToInt32(p)).ToList();

            for (int i = 0; i < temp.Count; i++)
            {
                temp[i] = temp[i] % Mod;
            }

            StringBuilder where = new StringBuilder();
            where.Append("('");
            where.Append(string.Join("','", temp));
            where.Append("')");

            return where.ToString();
        }
        #endregion

        #region 转化为PinYin匹配的条件
        /// <summary>
        /// 查询关键字
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <returns></returns>
        public static string ConvertStringToPinYinWhereString(string searchTxt)
        {
            StringBuilder where = new StringBuilder();

            if (!string.IsNullOrEmpty(searchTxt))
            {
                char[] chararray = searchTxt.ToCharArray();

                where.Append("%");
                where.Append(string.Join("%", chararray));
                where.Append("%");
            }

            return where.ToString();
        }
        #endregion

        #region Base64加密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encode">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string EncodeBase64(Encoding encode, string source)
        {
            byte[] bytes = encode.GetBytes(source);
            string encodeString = "";
            try
            {
                encodeString = Convert.ToBase64String(bytes);
            }
            catch
            {
                encodeString = source;
            }
            return encodeString;
        }

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string EncodeBase64(string source)
        {
            return EncodeBase64(Encoding.UTF8, source);
        }
        #endregion

        #region Base64解密
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encode">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decodeString = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decodeString = encode.GetString(bytes);
            }
            catch
            {
                decodeString = result;
            }
            return decodeString;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }
        #endregion

        #region DES加密
        private string key = "teeinapi";
        private string iv = "request+";
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public string EncryptByDES(string sourceString)
        {
            byte[] btKey = Encoding.Default.GetBytes(key);
            byte[] btIV = Encoding.Default.GetBytes(iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Encoding.Default.GetBytes(sourceString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        #region Decrypt
        /// <summary>  
        /// 对DES加密后的字符串进行解密  
        /// </summary>  
        /// <param name="encryptedString">待解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public string Decrypt(string encryptedString)
        {
            byte[] btKey = Encoding.Default.GetBytes(key);
            byte[] btIV = Encoding.Default.GetBytes(iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    byte[] inData = Convert.FromBase64String(encryptedString);
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.Default.GetString(ms.ToArray());
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        #endregion

        #region 判断字符串是否是中文
        /// <summary>
        /// 判断字符串是否是中文
        /// </summary>
        /// <param name="chars">字符串</param>
        /// <param name="RegType">true:全部是中文；false:包含有中文</param>
        /// <returns></returns>
        private static bool IsChinese(string chars, bool RegType)
        {
            if (RegType)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(chars, @"^([\u4e00-\u9fa5]|[\uff01-\uff60]|\u3000){1,}$");
            }
            else
            {
                return System.Text.RegularExpressions.Regex.IsMatch(chars, @"([\u4e00-\u9fa5]|[\uff01-\uff60]|\u3000){1,}");
            }
        }
        #endregion

        #region 判断是否为手机号
        public static bool IsMobileNumber(string number)
        {
            return new System.Text.RegularExpressions.Regex("^(13[0-9]|14[0-9]|15[0-9]|18[0-9])\\d{8}$").IsMatch(number);
        }
        #endregion

        #region 截取字符串(按字节)

        /// <summary>
        /// /不按字节截取
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">截取的字符长度</param>
        /// <param name="postfix">追加字符</param>
        /// <returns></returns>
        public static string GetSubString(string str, int length, string postfix = "")
        {
            if (string.IsNullOrWhiteSpace(str) || length < 1)
            {
                return "";
            }
            return str.Length > length ? (str.Substring(0, length) + postfix) : postfix;
        }

        /// <summary>
        /// 截取字符串(按字节)
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="length">截取的字节长度</param>
        /// <returns></returns>
        public static string SubStr(string s, int length)
        {
            if (string.IsNullOrWhiteSpace(s) || length < 1)
            {
                return "";
            }
            if (Encoding.GetEncoding("GB2312").GetBytes(s).Length <= length)
            {
                return s;
            }
            // 全部是中文
            if (IsChinese(s, true))
            {
                return s.Substring(0, length / 2);
            }
            // 如果不含有中文
            if (!IsChinese(s, false))
            {
                return s.Substring(0, length);
            }
            string str = "";
            int num = length / 2;
            int num2 = length;
            while (true)
            {
                str = str + s.Substring(str.Length, num);
                num2 = length - Encoding.GetEncoding("GB2312").GetBytes(str).Length;
                if (num2 <= 1)
                {
                    if ((num2 == 1) && (Encoding.GetEncoding("GB2312").GetBytes(s.Substring(str.Length, 1)).Length == 1))
                    {
                        str = str + s.Substring(str.Length, 1);
                    }
                    return str;
                }
                num = num2 / 2;
            }
        }

        /// <summary>
        /// 截取指定长度的字节数，并在末尾追加指定字符，比如“...”
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <param name="tailString"></param>
        /// <returns></returns>
        public static string SubStr(string s, int length, string tailString)
        {
            if (string.IsNullOrWhiteSpace(s) || length < 1)
            {
                return "";
            }
            if (string.IsNullOrWhiteSpace(tailString))
            {
                tailString = "...";
            }
            if (Encoding.GetEncoding("GB2312").GetBytes(s).Length > length)
            {
                return SubStr(s, length) + tailString;
            }
            return s;
        }
        #endregion

        #region 转换为int
        /// <summary>
        /// 转换为int
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int ToInt(object o)
        {
            if (o != null)
                return ToInt(o.ToString());

            return 0;
        }
        #endregion

        #region 转换为int
        /// <summary>
        /// 转换为int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt(string s)
        {
            int result = 0;
            if (int.TryParse(s, out result))
                return result;

            return 0;
        }
        #endregion

        #region 转换为UnixTime
        /// <summary>
        /// 转换为UnixTime
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double ToUnixTimestamp(DateTime dt)
        {
            DateTime start = new DateTime(1970, 1, 1).ToLocalTime();
            return dt.Subtract(start).TotalMilliseconds;
        }
        #endregion

        #region 时间搓转换为时间
        /// <summary>
        /// 时间搓转换为时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDatetime(double timestamp)
        {
            DateTime start = new DateTime(1970, 1, 1).ToLocalTime();
            return start.AddMilliseconds(timestamp);
        }
        #endregion

        #region XmlDocument转换为字符串
        /// <summary>
        /// XmlDocument转换为字符串
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public static string XmlDocument2String(XmlDocument xmlDoc)
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, null);
            writer.Formatting = Formatting.Indented;
            xmlDoc.Save(writer);
            StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
            stream.Position = 0;
            string xmlString = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return xmlString;
        }
        #endregion

        #region KeyValue结构转换为Xml
        /// <summary>
        /// KeyValue结构转换为Xml
        /// </summary>
        /// <param name="node"></param>
        /// <param name="Source"></param>
        private static void KeyValue2Xml(XmlElement node, KeyValuePair<string, object> Source)
        {
            object kValue = Source.Value;
            if (kValue != null)
            {
                if (kValue.GetType() == typeof(Dictionary<string, object>))
                {
                    foreach (KeyValuePair<string, object> item in kValue as Dictionary<string, object>)
                    {
                        XmlElement element = node.OwnerDocument.CreateElement(item.Key);
                        KeyValue2Xml(element, item);
                        node.AppendChild(element);
                    }
                }
                else if (kValue.GetType() == typeof(object[]))
                {
                    object[] o = kValue as object[];
                    for (int i = 0; i < o.Length; i++)
                    {
                        XmlElement xitem = node.OwnerDocument.CreateElement("Item");
                        KeyValuePair<string, object> item = new KeyValuePair<string, object>("Item", o[i]);
                        KeyValue2Xml(xitem, item);
                        node.AppendChild(xitem);
                    }

                }
                else
                {
                    XmlText text = node.OwnerDocument.CreateTextNode(kValue.ToString());
                    node.AppendChild(text);
                }
            }
            else
            {
                XmlText text = node.OwnerDocument.CreateTextNode("");
                node.AppendChild(text);
            }
        }
        #endregion

        #region ========加密========
        private static string secret = "#!12^0#@";
        private static Byte[] _key;
        private static Byte[] IV = new Byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <returns></returns>
        public static string UrlEncrypt(string strToEncrypt)
        {
            return UrlEncrypt(strToEncrypt, secret);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="strEncryptKey">密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string UrlEncrypt(string strToEncrypt, string strEncryptKey)
        {
            if (!string.IsNullOrEmpty(strToEncrypt))
            {
                try
                {
                    _key = Encoding.UTF8.GetBytes(strEncryptKey.Substring(0, 8));
                    Byte[] inputByteArray = Encoding.UTF8.GetBytes(strToEncrypt);
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(_key, IV), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    string result = Convert.ToBase64String(ms.ToArray());

                    foreach (var item in mapping)
                    {
                        result = result.Replace(item.Key, item.Value);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    new ExceptionHelper().LogException(ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region ========解密========
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strToDecrypt"></param>
        /// <returns></returns>
        public static string UrlDecrypt(string strToDecrypt)
        {
            return UrlDecrypt(strToDecrypt, secret);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strToDecrypt">要解密的字符串</param>
        /// <param name="strEncryptKey">密钥，必须与加密的密钥相同</param>
        /// <returns>解密后的字符串</returns>
        public static string UrlDecrypt(string strToDecrypt, string strEncryptKey)
        {
            if (!string.IsNullOrEmpty(strToDecrypt))
            {
                foreach (var item in mapping)
                {
                    strToDecrypt = strToDecrypt.Replace(item.Value, item.Key);
                }
                //strToDecrypt = strToDecrypt.Replace(" ", "+");//如果去除此部分的代码就会出现上面出现所说的情况，出错或者解密出来的数据变成空值。
                try
                {
                    _key = Encoding.UTF8.GetBytes(strEncryptKey.Substring(0, 8));
                    Byte[] inputByteArray = Convert.FromBase64String(strToDecrypt);
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(_key, IV), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch (Exception ex)
                {
                    new ExceptionHelper().LogException(ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 获取post过来的xml参数
        /// <summary>
        /// 获取post过来的xml参数
        /// </summary>
        /// <returns></returns>
        public XDocument GetPostXmlData()
        {
            XDocument doc = null;
            if (System.Web.HttpContext.Current != null)
            {
                //读取参数
                using (StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Request.InputStream, Encoding.UTF8))
                {
                    try
                    {
                        string responseXml = sr.ReadToEnd();

                        //转换为XDocument
                        doc = XDocument.Parse(responseXml);

                    }
                    catch (Exception e)
                    {
                        new ExceptionHelper().LogException(e);
                    }
                }
            }

            return doc;
        }
        #endregion

        #region 根据当前的请求获取网站的跟路径
        /// <summary>
        /// 根据当前的请求获取网站的跟路径
        /// </summary>
        /// <returns></returns>
        public static string BaseUrl()
        {
            string baseUrl = string.Empty;
            if (System.Web.HttpContext.Current != null)
            {
                HttpRequest request = System.Web.HttpContext.Current.Request;
                if (string.IsNullOrWhiteSpace(request.ApplicationPath) || request.ApplicationPath.Equals("/"))
                {
                    if (request.Url.Port == 80 || IsDNAT)
                    {
                        baseUrl = string.Format("http://{0}/", request.Url.Host);
                    }
                    else
                    {
                        baseUrl = string.Format("http://{0}:{1}/", request.Url.Host, request.Url.Port);
                    }
                }
                else
                {
                    if (request.Url.Port == 80 || IsDNAT)
                    {
                        baseUrl = string.Format("http://{0}/{1}/", request.Url.Host, request.ApplicationPath.TrimStart('/'));
                    }
                    else
                    {
                        baseUrl = string.Format("http://{0}:{1}/{2}/", request.Url.Host, request.Url.Port, request.ApplicationPath.TrimStart('/'));
                    }
                }
            }
            return baseUrl;
        }
        #endregion

        #region 获取当前请求的域名
        /// <summary>
        /// 获取当前请求的域名
        /// </summary>
        /// <returns></returns>
        public static string BaseUrlDomain()
        {
            string baseUrl = string.Empty;
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (request.Url.Port == 80 || IsDNAT)
            {
                baseUrl = string.Format("http://{0}/", request.Url.Host);
            }
            else
            {
                baseUrl = string.Format("http://{0}:{1}/", request.Url.Host, request.Url.Port);
            }
            return baseUrl;
        }
        #endregion

        #region 调试模式再输出
        /// <summary>
        /// 调试模式再输出
        /// </summary>
        /// <param name="message">信息</param>
        public static void DebugWriteLine(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
        #endregion

        #region 根据当前的请求获取网站的跟路径
        /// <summary>
        /// 根据当前的请求获取网站的根路径
        /// </summary>
        /// <returns></returns>
        public static string BaseUrlPath()
        {
            string baseUrl = string.Empty;
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (string.IsNullOrWhiteSpace(request.ApplicationPath) || request.ApplicationPath.Equals("/"))
            {
                if (request.Url.Port == 80 || IsDNAT)
                {
                    baseUrl = string.Format("http://{0}/{1}", request.Url.Host, request.Url.LocalPath.TrimStart('/'));
                }
                else
                {
                    baseUrl = string.Format("http://{0}:{1}/{2}", request.Url.Host, request.Url.Port, request.Url.LocalPath.TrimStart('/'));
                }
            }
            else
            {
                if (request.Url.Port == 80 || IsDNAT)
                {
                    baseUrl = string.Format("http://{0}/{1}/{2}", request.Url.Host, request.ApplicationPath.TrimStart('/'), request.Url.LocalPath.TrimStart('/'));
                }
                else
                {
                    baseUrl = string.Format("http://{0}:{1}/{2}/{3}", request.Url.Host, request.Url.Port, request.ApplicationPath.TrimStart('/'), request.Url.LocalPath.TrimStart('/'));
                }
            }

            return baseUrl;
        }
        #endregion

        #region 相对路径转换为完整路径
        /// <summary>
        /// 相对路径转换为完整路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Content(string url)
        {
            return ContentComm(url);
        }
        /// <summary>
        /// 重载Content,当传入两个参数的时候匹配的Url.Action("action","controller")
        /// </summary>
        /// <param name="strAction"></param>
        /// <param name="strCon"></param>
        /// <returns></returns>
        public static string Content(string strCon, string strAction)
        {
            string url = string.Format("{0}/{1}", strAction, strCon);

            return ContentComm(url);
        }

        private static string ContentComm(string url)
        {
            string result = string.Empty;

            string baseUrl = BaseUrl();
            if (url.StartsWith("http://"))
            {
                if (IsDNAT)
                {
                    result = url.Replace(string.Format(":{0}", new System.Uri(url).Port), "");
                }
                else
                {
                    result = url;
                }
            }
            else
            {
                if (url.StartsWith("~/") || url.StartsWith("/"))
                {
                    url = url.TrimStart("~/".ToCharArray());
                }

                result = string.Format("{0}/{1}", baseUrl.TrimEnd('/'), url);
            }

            return result;
        }
        #endregion

        #region 获取随机字符串
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="length">长度,默认长度43</param>
        /// <returns></returns>
        public static string RandomString(int length = 43)
        {
            string source = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = string.Empty;
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(0, 42);
                result += source[index];
            }

            return result;
        }
        #endregion

        #region 判断是否是手机端访问
        /// <summary>
        /// 判断是否是手机端访问
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            return HttpContext.Current.Request.Browser.IsMobileDevice;
        }
        #endregion

        #region 把byte类型的数据转换为string
        /// <summary>
        /// 把byte类型的数据转换为string
        /// </summary>
        /// <param name="sidBytes">字节数组</param>
        /// <returns>转换以后的String值</returns>
        public static string ConvertByteToStringSid(Byte[] sidBytes)
        {
            short sSubAuthorityCount = 0;
            StringBuilder strSid = new StringBuilder();
            strSid.Append("S-");
            try
            {
                // Add SID revision.
                strSid.Append(sidBytes[0].ToString());

                sSubAuthorityCount = Convert.ToInt16(sidBytes[1]);

                // Next six bytes are SID authority value.
                if (sidBytes[2] != 0 || sidBytes[3] != 0)
                {
                    string strAuth = String.Format("{0}{1}{2}{3}{4}{5}",
                        (Int16)sidBytes[2] % 100,
                        (Int16)sidBytes[3] % 100,
                        (Int16)sidBytes[4] % 100,
                        (Int16)sidBytes[5] % 100,
                        (Int16)sidBytes[6] % 100,
                        (Int16)sidBytes[7] % 100);
                    strSid.Append("-");
                    strSid.Append(strAuth);
                }
                else
                {
                    Int64 iVal = (Int32)(sidBytes[7]) +
                        (Int32)(sidBytes[6] << 8) +
                        (Int32)(sidBytes[5] << 16) +
                        (Int32)(sidBytes[4] << 24);
                    strSid.Append("-");
                    strSid.Append(iVal.ToString());
                }

                // Get sub authority count...
                int idxAuth = 0;
                for (int i = 0; i < sSubAuthorityCount; i++)
                {
                    idxAuth = 8 + i * 4;
                    if (idxAuth < sidBytes.Count())
                    {
                        UInt32 iSubAuth = BitConverter.ToUInt32(sidBytes, idxAuth);
                        strSid.Append("-");
                        strSid.Append(iSubAuth.ToString());
                    }
                }
            }
            catch
            {
                return "";
            }
            return strSid.ToString();
        }
        #endregion

        #region 获取SID
        /// <summary>
        /// 获取SID
        /// </summary>
        /// <returns></returns>
        public static string CreateSID()
        {
            return ConvertByteToStringSid(Guid.NewGuid().ToByteArray());
        }
        #endregion

        #region 首字母转大写
        /// <summary>
        /// 将字符串转为驼峰命名 的字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToCamelName(string name)
        {

            // 快速检查
            if (string.IsNullOrWhiteSpace(name))
            {
                // 没必要转换
                return "";
            }
            else if (!name.Contains("_"))
            {
                // 不含下划线，仅将首字母小写
                return name.Substring(0, 1).ToUpper() + name.Substring(1);
            }
            // 用下划线将原始字符串分割
            string[] camels = name.Split(Convert.ToChar("_"));
            List<string> restList = new List<string>();
            foreach (var camel in camels)
            {
                // 跳过原始字符串中开头、结尾的下换线或双重下划线
                if (string.IsNullOrWhiteSpace(camel))
                {
                    continue;
                }
                restList.Add(ToCamelString(camel));
            }
            return string.Join("_", restList);
        }

        /// <summary>
        /// 首字母转大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            // 不含下划线，仅将首字母小写
            return str.Substring(0, 1).ToUpper() + str.Substring(1);

        }
        #endregion

        #region Xml2Object
        /// <summary>
        /// Xml2Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static T Xml2Object<T>(XmlNode node)
        {
            string jsonResult = Newtonsoft.Json.JsonConvert.SerializeXmlNode(node);

            jsonResult = jsonResult.Replace("{\"Results\":", "");
            jsonResult = jsonResult.Remove(jsonResult.Length - 1, 1);

            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonResult);
            return result;
        }
        #endregion

        #region 根据当前的url获取微信授权回调url
        /// <summary>
        /// 根据当前的url获取微信授权回调url
        /// </summary>
        /// <returns></returns>
        public static string GeneralWeiXinOAuthCallBackUrl()
        {
            string url = "";

            if (System.Web.HttpContext.Current != null)
            {
                url = System.Web.HttpContext.Current.Request.Url.ToString(); //当前地址
                int iPort = System.Web.HttpContext.Current.Request.Url.Port;//端口号               

                if (!url.Contains("?"))
                {
                    url += "?t=" + new Random().Next().ToString();
                }
                else
                {
                    url += "&t=" + new Random().Next().ToString();
                }
            }
            return url;
        }
        #endregion

        #region 创建16位GUID
        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <param name=\"guid\"></param>  
        /// <returns></returns>  
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        #endregion

        #region
        /// <summary>
        /// 专享价
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static decimal GetExclusivePrice(decimal price, int count)
        {
            decimal rate = 1;
            if (count > 60)
            {
                rate = 0.8M;
            }
            else if (count < 60 && count >= 36)
            {
                rate = 0.85M;
            }
            else if (count < 36 && count >= 12)
            {
                rate = 0.9M;
            }
            else if (count < 12 && count >= 6)
            {
                rate = 0.93M;
            }
            else if (count < 6 && count >= 2)
            {
                rate = 0.95M;
            }

            return (price) * rate;
        }
        #endregion

        /// <summary>
        ///     去除HTML标记
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <returns>已经去除后的文字</returns>
        public static string RemoveHtml(string htmlstring)
        {
            //删除脚本    
            htmlstring =
                Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>",
                              "", RegexOptions.IgnoreCase);
            //删除HTML    
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);


            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            htmlstring = htmlstring.Replace("\r\n", "");
            return htmlstring;
        }

        public static string GetFixedString(int count,string s){
            if(count<=0){
                return string.Empty;
            }

            StringBuilder sb=new StringBuilder();
            for(var i=0;i<count;i++){
                sb.Append(s);
            }

            return sb.ToString();
        }

        #region 密码加密算法
        public static string Md5Password(string password) 
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }
            else {
                return TextHelper.MD5(password.ToUpper() + "!!!!").ToUpper();
            }
        }

        public static string EncryptPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }
            else {
                return TextHelper.MD5(TextHelper.MD5(password).ToUpper() + "!!!!").ToUpper();
            }
        }
        #endregion

        public static string RemoveProvince(string data)
        {
            data = data.Replace("省", "");
            data = data.Replace("市", "");
            data = data.Length > 2 ? data.Substring(0, 2) : data;
            return data;
        }

        public static string RemoveCounty(string data) 
        {
            data = data.Replace("区", "");
            data = data.Replace("县", "");
            data = data.Length > 2 ? data.Substring(0, 2) : data;
            return data;
        }

        public static bool IsAndroid(string agent) 
        {
            if (!string.IsNullOrWhiteSpace(agent)) {
                return agent.ToUpper().Contains("okhttp".ToUpper());
            }
            return false;
        }

        public static string AppendFileSuffix(string fileName, int width, int height) 
        {
            return AppendFileSuffix(fileName, string.Format("_{0}_{1}", width, height));
        }

        public static string AppendFileSuffix(string fileName, ProductSizes sizes)
        {
            string suffix=string.Empty;
            string fixed_size_str="_{0}_{1}";
            switch(sizes){
                case ProductSizes.Size100:
                    suffix=string.Format(fixed_size_str,100,100);
                    break;
                case ProductSizes.Size300:
                    suffix=string.Format(fixed_size_str,300,300);
                    break;
                case ProductSizes.Size640:
                    suffix=string.Format(fixed_size_str,640,640);
                    break;
                default:
                    suffix=string.Empty;
                    break;
            }

            return AppendFileSuffix(fileName, suffix);
        }

        public static string AppendFileSuffix(string fileName, string suffix) 
        {
            if (string.IsNullOrWhiteSpace(fileName)) 
            {
                return fileName;
            }

            var index = fileName.LastIndexOf('.');
            if (index > -1) {
                return string.Format("{0}{1}{2}", fileName.Substring(0, index), suffix, fileName.Substring(index));
            }
            else{
                return fileName;
            }
        }

        #region xml对象序列化
        /// <summary>
        /// 将对象序列化为文件
        /// </summary>
        /// <param name="obj">操作的对象</param>
        /// <param name="path">保存路径</param>
        public static void SerilizeAnObject(object obj, string path)
        {
            System.IO.FileStream stream = new FileStream(path, FileMode.Create);
            try
            {
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(obj.GetType());
                serializer.Serialize(stream, obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SerilizeAnObject Exception: {0}", ex.Message);
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        /// <summary>
        /// 序列化XML文件
        /// </summary>
        /// <param name="type">序列化类型</param>
        /// <param name="path">文件路径</param>
        /// <returns>序列化后的类型</returns>
        public static object DeserilizeAnObject(Type type, string path)
        {
            object obj = null;
            System.IO.FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open);
                System.Xml.XmlReader reader = new XmlTextReader(stream);
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(type);
                obj = serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                new ExceptionHelper().LogException(ex);
                //Console.WriteLine("DeserilizeAnObject Exception: {0}", ex.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }

            return obj;
        }
        #endregion
    }
}
