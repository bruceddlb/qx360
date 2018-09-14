using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// 字符串忽略大小写比较器
    /// </summary>
    public class StringEqualityComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// 比较两个指定的String对象
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(string x, string y)
        {
            return (string.Compare(x, y, true) == 0);
        }

        /// <summary>
        /// 返回该字符串的Hash代码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(string obj)
        {
            return obj.ToUpper().GetHashCode();
        }
    }
}
