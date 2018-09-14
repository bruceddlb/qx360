using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// IEnumerable扩展类
    /// </summary>
    public static class IEnumerableHelper
    {
        /// <summary>
        /// 复制集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> items) where T : ICloneable
        {
            List<T> fruit = new List<T>();
            foreach (T item in items)
            {
                fruit.Add((T)(item.Clone()));
            }
            return fruit;
        }

        /// <summary>
        /// 检查集合是否为空或者长度为0
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNothing(this IList o)
        {
            return o == null || o.Count == 0;
        }

        /// <summary>
        /// 检查集合是否为空或者长度为0
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [Obsolete("Please use the IsNothing method.")]
        public static bool IsNullOrZeroLength(this IList o)
        {
            return IsNothing(o);
        }

        /// <summary>
        /// 遍历循环枚举器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="action"></param>
        public static void Foreach<T>(this IEnumerable<T> o, Action<T> action)
        {
            foreach (var obj in o)
            {
                action(obj);
            }
        }
    }
}
