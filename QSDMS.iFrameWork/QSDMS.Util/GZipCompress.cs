using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace QSDMS.Util
{
    /// <summary>
    /// GZip压缩工具
    /// </summary>
    public class GZipCompress
    {
        /// <summary>
        /// 压缩字符串
        /// </summary>
        public string Compress(string str)
        {
            if (str == null)
            {
                return null;
            }
            return Convert.ToBase64String(Compress(Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(str)))));
        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        public string Decompress(string str)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(Decompress(Convert.FromBase64String(str)));
        }

        /// <summary>
        /// 压缩字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>Base64编码</returns>
        public byte[] Compress(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
                {
                    zip.Write(bytes, 0, bytes.Length);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 解压字节
        /// </summary>
        /// <param name="bytes">Base64编码</param>
        /// <returns></returns>
        public byte[] Decompress(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            using (MemoryStream output = new MemoryStream())
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        zip.CopyTo(output);
                    }
                }
                return output.ToArray();
            }
        }
    }
}
