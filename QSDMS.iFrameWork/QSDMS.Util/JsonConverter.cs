using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// Int64的Json转换器
    /// <remarks>转换Int64类型数据以字符串方式输出</remarks>
    /// </summary>
    public class Int64JsonConverter : JsonConverter
    {
        /// <summary>
        /// 类型判断
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Int64);
        }

        /// <summary>
        /// 读
        /// <remarks>反序列化时使用</remarks>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
            {
                return 0;
            }

            string str = existingValue.ToString().Trim();
            if (str.Length == 0)
            {
                return 0;
            }

            return long.Parse(str);
        }

        /// <summary>
        /// 写
        /// <remarks>序列化时使用</remarks>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    /// <summary>
    /// 枚举的Json转换器
    /// <remarks>转换枚举类型数据以值方式输出</remarks>
    /// </summary>
    public class EnumJsonConverter : JsonConverter
    {
        /// <summary>
        /// 类型判断
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        /// <summary>
        /// 读
        /// <remarks>反序列化时使用</remarks>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue != null && objectType.IsEnumDefined(existingValue))
            {
                return Enum.Parse(objectType, existingValue.ToString());
            }
            else
            {
                return Activator.CreateInstance(objectType);
            }
        }

        /// <summary>
        /// 写
        /// <remarks>序列化时使用</remarks>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToInt32(value).ToString());
        }
    }
}
