using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// Float扩展类
    /// </summary>
    public static class FloatHelper
    {
        /// <summary>
        /// 判断数值是否为0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsZero(this float obj)
        {
            return Math.Abs(obj) < float.Epsilon;
        }
    }
}
