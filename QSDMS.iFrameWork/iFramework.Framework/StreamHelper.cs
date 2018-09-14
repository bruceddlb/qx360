using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace iFramework.Framework
{
    /// <summary>
    /// Stream扩展类
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <returns></returns>
        public static string HashMD5(this Stream stream)
        {
            byte[] bytes = MD5.Create().ComputeHash(stream);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <returns></returns>
        public static string HashSHA1(this Stream stream)
        {
            byte[] bytes = SHA1.Create().ComputeHash(stream);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// 把流转换为字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream can't read.");
            }

            byte[] buffer = new byte[4096];

            int index = 0;
            int count;

            while ((count = stream.Read(buffer, index, buffer.Length - index)) > 0)
            {
                index += count;
                if (index == buffer.Length)
                {
                    int next = stream.ReadByte();
                    if (next != -1)
                    {
                        byte[] temp = new byte[buffer.Length * 2];
                        Buffer.BlockCopy(buffer, 0, temp, 0, buffer.Length);
                        Buffer.SetByte(temp, index, (byte)next);
                        buffer = temp;
                        index++;
                    }
                }
            }

            if (index == 0)
            {
                return null;
            }

            if (buffer.Length != index)
            {
                byte[] fruit = new byte[index];
                Buffer.BlockCopy(buffer, 0, fruit, 0, index);
                return fruit;
            }

            return buffer;
        }
    }
}
