using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace QSDMS.Util
{
    /// <summary>
    /// 序列化
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// 序列化XML文件
        /// </summary>
        public static bool SerializeXmlFile<T>(string filePath, T obj) where T : class
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                return true;
            }
        }

        /// <summary>
        /// 反序列化XML文件
        /// </summary>
        public static T DeserializeXmlFile<T>(string filePath) where T : class
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// 序列化XML对象
        /// </summary>
        public static string SerializeXml<T>(T obj) where T : class
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 反序列化XML字符串
        /// </summary>
        public static T DeserializeXml<T>(string str) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// 序列化Json对象
        /// <remarks>采用.NetFramework序列化类</remarks>
        /// </summary>
        public static string SerializeJson(object obj)
        {
            return SerializeJson<object>(obj);
        }

        /// <summary>
        /// 序列化Json对象
        /// <remarks>采用Newtonsoft序列化类</remarks>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignorenull">是否忽略空值属性</param>
        /// <returns></returns>
        public static string SerializeJson(object obj, bool ignorenull)
        {
            return SerializeJson<object>(obj, ignorenull);
        }

        /// <summary>
        /// 序列化Json对象
        /// <remarks>采用Newtonsoft序列化类</remarks>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignorenull">是否忽略空值属性</param>
        /// <param name="converters">自定义转换器</param>
        /// <returns></returns>
        public static string SerializeJson(object obj, bool ignorenull, IList<JsonConverter> converters)
        {
            return SerializeJson<object>(obj, ignorenull, converters);
        }

        /// <summary>
        /// 序列化Json对象
        /// <remarks>采用.NetFramework序列化类</remarks>
        /// </summary>
        public static string SerializeJson<T>(T obj) where T : class
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 序列化Json对象
        /// <remarks>采用Newtonsoft序列化类</remarks>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignorenull">是否忽略空值属性</param>
        /// <returns></returns>
        public static string SerializeJson<T>(T obj, bool ignorenull) where T : class
        {
            return SerializeJson<object>(obj, ignorenull, null);
        }

        /// <summary>
        /// 序列化Json对象
        /// <remarks>采用Newtonsoft序列化类</remarks>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignorenull">是否忽略空值属性</param>
        /// <param name="converters">自定义转换器</param>
        /// <returns></returns>
        public static string SerializeJson<T>(T obj, bool ignorenull, IList<JsonConverter> converters) where T : class
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = ignorenull ? NullValueHandling.Ignore : NullValueHandling.Include;
            setting.Converters = converters;
            return JsonConvert.SerializeObject(obj, Formatting.None, setting);
        }

        /// <summary>
        /// 反序列化Json对象
        /// <remarks>采用.NetFramework序列化类</remarks>
        /// </summary>
        public static T DeserializeJson<T>(string str) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// 反序列化Json对象
        /// <remarks>采用Newtonsoft序列化类</remarks>
        /// <param name="str"></param>
        /// <param name="ignorenull">是否忽略空值属性</param>
        /// </summary>
        public static T DeserializeJson<T>(string str, bool ignorenull) where T : class
        {
            return DeserializeJson<T>(str, ignorenull, null);
        }

        /// <summary>
        /// 反序列化Json对象
        /// <remarks>采用Newtonsoft序列化类</remarks>
        /// <param name="str"></param>
        /// <param name="ignorenull">是否忽略空值属性</param>
        /// <param name="converters">自定义转换器</param>
        /// </summary>
        public static T DeserializeJson<T>(string str, bool ignorenull, IList<JsonConverter> converters) where T : class
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = ignorenull ? NullValueHandling.Ignore : NullValueHandling.Include;
            setting.Converters = converters;
            return JsonConvert.DeserializeObject<T>(str, setting);
        }

        /// <summary>
        /// 序列化object对象
        /// </summary>
        public static string SerializeObject(object obj)
        {
            byte[] bytes = SerializeBinary(obj);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 反序列化object对象
        /// </summary>
        public static object DeserializeObject(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return DesrializeBinary(bytes);
        }

        /// <summary>
        /// 二进制序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeBinary(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter format = new BinaryFormatter();
                format.Serialize(stream, obj);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// 二进制反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static object DesrializeBinary(byte[] buffer)
        {
            BinaryFormatter format = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return format.Deserialize(stream);
            }
        }

        /// <summary>
        /// 二进制反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T DesrializeBinary<T>(byte[] buffer)
        {
            BinaryFormatter format = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return (T)format.Deserialize(stream);
            }
        }
    }
}
