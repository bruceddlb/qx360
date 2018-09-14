using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 获取32位长度的小写Guid字符串
        /// </summary>
        /// <returns></returns>
        public static string NewLowerGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 获取32位长度的Guid字符串
        /// </summary>
        /// <returns></returns>
        public static string NewUpperGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }
    }
}
