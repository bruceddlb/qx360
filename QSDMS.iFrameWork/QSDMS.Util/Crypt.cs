using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace QSDMS.Util
{
    /// <summary>
    /// 加密解密类
    /// </summary>
    public static class Crypt
    {
        /// <summary>
        /// 根据HASH值后8位字节倒序生成LONG型数字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static long HashInt64(string text)
        {
            byte[] bytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// 不可逆加密(MD5)
        /// </summary>
        /// <param name="text">需要加密的字符串</param>
        /// <returns></returns>
        public static string Hash(string text)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5");

            #region 备份代码

            //算法来源于 FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5");
            //if (text == null)
            //{
            //    throw new ArgumentNullException();
            //}
            //byte[] buffer = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            //char[] acharval = new char[16];
            //for (int i = 16; --i >= 0; )
            //{
            //    if (i < 10)
            //    {
            //        acharval[i] = (char)('0' + i);
            //    }
            //    else
            //    {
            //        acharval[i] = (char)('A' + (i - 10));
            //    }
            //}
            //int iLength = buffer.Length;
            //char[] chars = new char[iLength * 2];
            //fixed (char* fc = chars, fcharval = acharval)
            //{
            //    fixed (byte* fb = buffer)
            //    {
            //        char* pc;
            //        byte* pb;
            //        pc = fc;
            //        pb = fb;
            //        while (--iLength >= 0)
            //        {
            //            *pc++ = fcharval[(*pb & 0xf0) >> 4];
            //            *pc++ = fcharval[*pb & 0x0f];
            //            pb++;
            //        }
            //    }
            //}
            //return new string(chars);

            #endregion
        }

        /// <summary>
        /// 可逆加密(DES)
        /// </summary>
        /// <param name="text">需要加密的字符串</param>
        /// <param name="key">密钥<para>长度必须为8位</para></param>
        /// <returns></returns>
        public static string Encrypt(string text, string key)
        {
            if (text == null || key == null)
            {
                throw new ArgumentNullException();
            }

            if (key.Length != 8)
            {
                throw new ArgumentException();
            }

            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                provider.Key = ASCIIEncoding.ASCII.GetBytes(key);
                provider.IV = ASCIIEncoding.ASCII.GetBytes(key);

                byte[] buffer = Encoding.UTF8.GetBytes(text);
                MemoryStream stream = new MemoryStream();

                using (CryptoStream cs = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }

                text = Convert.ToBase64String(stream.ToArray());
                stream.Close();
                return text;
            }
        }

        /// <summary>
        /// 可逆解密(DES)
        /// </summary>
        /// <param name="text">需要解密密的字符串</param>
        /// <param name="key">密钥<para>长度必须为8位</para></param>
        /// <returns></returns>
        public static string Decrypt(string text, string key)
        {
            if (text == null || key == null)
            {
                throw new ArgumentNullException();
            }

            if (key.Length != 8)
            {
                throw new ArgumentException();
            }

            byte[] buffer = Convert.FromBase64String(text);

            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                provider.Key = ASCIIEncoding.ASCII.GetBytes(key);
                provider.IV = ASCIIEncoding.ASCII.GetBytes(key);

                MemoryStream stream = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }

                text = Encoding.UTF8.GetString(stream.ToArray());
                stream.Close();
                return text;
            }
        }
    }
}
