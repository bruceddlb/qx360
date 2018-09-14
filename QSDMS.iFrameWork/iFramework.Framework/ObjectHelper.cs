using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// Object扩展类
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// 复制对象
        /// </summary>
        /// <returns></returns>
        public static object Copy(this object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Copy<T>(this T obj) where T : class
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                object o = formatter.Deserialize(stream);
                if (o is T)
                {
                    return o as T;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
