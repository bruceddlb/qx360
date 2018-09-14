using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QSDMS.Util
{
    public class HttpUtil
    {
        public static string GetNoncestr()
        {
            Random random = new Random();
            return GetMD5(random.Next(1000).ToString(), "GBK");
        }
        public static string GetTimestamp()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
        }
        public static string UrlEncode(string instr, string charset)
        {
            if (instr == null || instr.Trim() == "")
            {
                return "";
            }
            string result;
            try
            {
                result = HttpUtility.UrlEncode(instr, Encoding.GetEncoding(charset));
            }
            catch (Exception)
            {
                result = HttpUtility.UrlEncode(instr, Encoding.GetEncoding("GB2312"));
            }
            return result;
        }
        public static string UrlDecode(string instr, string charset)
        {
            if (instr == null || instr.Trim() == "")
            {
                return "";
            }
            string result;
            try
            {
                result = HttpUtility.UrlDecode(instr, Encoding.GetEncoding(charset));
            }
            catch (Exception)
            {
                result = HttpUtility.UrlDecode(instr, Encoding.GetEncoding("GB2312"));
            }
            return result;
        }
        public static uint UnixStamp()
        {
            return Convert.ToUInt32((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds);
        }
        public static string BuildRandomStr(int length)
        {
            Random random = new Random();
            string text = random.Next().ToString();
            if (text.Length > length)
            {
                text = text.Substring(0, length);
            }
            else
            {
                if (text.Length < length)
                {
                    for (int i = length - text.Length; i > 0; i--)
                    {
                        text.Insert(0, "0");
                    }
                }
            }
            return text;
        }

        public static string CreateMd5Sign(string key,string value, Dictionary<string,string> hash,Encoding encoding)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ArrayList arrayList = new ArrayList(hash.Keys);
            arrayList.Sort();
            foreach (string text in arrayList)
            {
                string text2 = (string)hash[text];
                if (text2 != null && "".CompareTo(text2) != 0 && "sign".CompareTo(text) != 0 && "key".CompareTo(text) != 0)
                {
                    stringBuilder.Append(text + "=" + text2 + "&");
                }
            }

            stringBuilder.Append(key + "=" + value);
            return GetMD5(stringBuilder.ToString(), encoding.BodyName).ToUpper();
        }

        public static string GetMD5(string encypStr, string charset)
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] bytes;
            try
            {
                bytes = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception)
            {
                bytes = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            byte[] value = mD5CryptoServiceProvider.ComputeHash(bytes);
            string text = BitConverter.ToString(value);
            return text.Replace("-", "").ToUpper();
        }

        public static string ParseXML(Dictionary<string,string> hash)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<xml>");
            foreach (string text in hash.Keys)
            {
                string text2 = (string)hash[text];
                if (System.Text.RegularExpressions.Regex.IsMatch(text2, "^[0-9.]$"))
                {
                    stringBuilder.Append(string.Concat(new string[]
			{
				"<",
				text,
				">",
				text2,
				"</",
				text,
				">"
			}));
                }
                else
                {
                    stringBuilder.Append(string.Concat(new string[]
			{
				"<",
				text,
				"><![CDATA[",
				text2,
				"]]></",
				text,
				">"
			}));
                }
            }
            stringBuilder.Append("</xml>");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="noncestr">随机字符串</param>
        /// <param name="url">当前网页url</param>
        /// <param name="jsapi_ticket"></param>
        /// <returns></returns>
        public static string GetJsApiTicketSignature(string timestamp, string noncestr, string url = null, string jsapi_ticket = null)
        {
            var arr = new[] { "jsapi_ticket=" + jsapi_ticket, "noncestr=" + noncestr, "timestamp=" + timestamp, "url=" + url }.OrderBy(Z => Z).ToArray();
            var arrString = string.Join("&", arr);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
            var enText = new StringBuilder();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }
            return enText.ToString();
        }

        /**/
        /// <summary> 
        /// 反序列化为对象 
        /// </summary> 
        /// <param name=\"type\">对象类型</param> 
        /// <param name=\"s\">对象序列化后的Xml字符串</param> 
        /// <returns></returns> 
        public static object Deserialize(Type type, string s) {
            using (System.IO.StringReader sr = new System.IO.StringReader(s))
            {
                System.Xml.Serialization.XmlSerializer xz = new System.Xml.Serialization.XmlSerializer(type); 
                return xz.Deserialize(sr); 
            } 
        } 
    }
}
