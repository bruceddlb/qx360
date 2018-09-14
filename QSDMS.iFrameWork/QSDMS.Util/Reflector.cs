using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace QSDMS.Util
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class Reflector
    {
        /// <summary>
        /// 反射获取枚举的字段的自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="filedName"></param>
        /// <returns></returns>
        public static K GetEnumAttributeInfo<T, K>(string filedName)
            where K : Attribute
        {
            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException("T 必须是枚举");
            }

            FieldInfo fieldInfo = type.GetField(filedName);

            if (fieldInfo != null)
            {
                K res = GetAttributeObject<K>(fieldInfo);

                return res;
            }
            else
            {
                return default(K);
            }
        }

        /// <summary>
        /// 反射获取枚举值的自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static K GetEnumAttributeInfo<T, K>(T enumValue)
            where K : Attribute
        {
            if (enumValue == null)
            {
                throw new ArgumentNullException("参数不能为空");
            }

            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException("T 必须是枚举");
            }

            FieldInfo fieldInfo = type.GetField(Enum.GetName(typeof(T), enumValue));

            if (fieldInfo != null)
            {
                K res = GetAttributeObject<K>(fieldInfo);

                return res;
            }
            else
            {
                return default(K);
            }
        }

        /// <summary>
        /// 反射获取枚举值的自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static List<K> GetEnumAttributeInfos<T, K>(T enumValue)
            where K : Attribute
        {
            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException("T 必须是枚举");
            }

            FieldInfo fieldInfo = type.GetField(Enum.GetName(typeof(T), enumValue));

            List<K> lstRes = new List<K>();

            if (fieldInfo != null)
            {
                lstRes = GetAttributeObjects<K>(fieldInfo);
            }
            return lstRes;
        }

        /// <summary>
        /// 获取枚举的带有指定的Attribute的枚举项信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public static Dictionary<T, K> GetEnumAttributeInfos<T, K>()
            where T : struct
            where K : Attribute
        {
            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException("T 必须是枚举");
            }

            Dictionary<T, K> dicValue = new Dictionary<T, K>();

            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo fi in fields)
            {
                if (fi.DeclaringType == type)
                {
                    K attriObj = GetAttributeObject<K>(fi);

                    if (attriObj != default(K))
                    {
                        T v;
                        if (Enum.TryParse<T>(fi.Name, out v))
                        {
                            dicValue[v] = attriObj as K;
                        }
                    }
                }
            }
            return dicValue;
        }

        /// <summary>
        /// 获取字段的指定Attribute信息
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static K GetAttributeObject<K>(FieldInfo fieldInfo) where K : Attribute
        {
            K res = default(K);

            object[] oAttrs = fieldInfo.GetCustomAttributes(typeof(K), true);

            if (oAttrs.Length > 0)
            {
                foreach (object attr in oAttrs)
                {
                    if (attr is K)
                    {
                        res = attr as K;
                        break;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 获取字段的指定Attribute信息
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static List<K> GetAttributeObjects<K>(FieldInfo fieldInfo) where K : Attribute
        {
            List<K> lstRes = new List<K>();

            object[] oAttrs = fieldInfo.GetCustomAttributes(typeof(K), true);

            if (oAttrs.Length > 0)
            {
                foreach (object attr in oAttrs)
                {
                    if (attr is K)
                    {
                        lstRes.Add(attr as K);
                    }
                }
            }
            return lstRes;
        }
    }
}
