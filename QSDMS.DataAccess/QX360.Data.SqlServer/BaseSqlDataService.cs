using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.SqlServer
{
    /// <summary>
    /// 基类
    /// </summary>
    public class BaseSqlDataService
    {
        /// <summary>
        /// 格式化数据库查询参数 防止SQL注入攻击
        /// </summary>
        /// <param name="format"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        protected string FormatParameter(string format, params object[] objs)
        {
            if (objs.Length > 0)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i] != null)
                    {
                        objs[i] = RepairParameter(objs[i]);
                    }
                }
            }
            return string.Format(" " + format + " ", objs);
        }

        /// <summary>
        /// 替换SQL参数 不进行防止SQL注入攻击操作
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ReplaceParameter(string commandText, string key, string value)
        {
            // 由于存在整个参数替换 故不进行单引号的替换

            if (commandText == null)
            {
                return null;
            }

            return commandText.Replace(key, value);
        }
        /// <summary>
        /// 格式化参数 防止SQL注入攻击
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        protected object RepairParameter(object para)
        {
            if (para == null || para == DBNull.Value)
            {
                return para;
            }

            string str = para.ToString();

            // 替换单引号为空
            if (str.IndexOf('\'') != -1)
            {
                return str.Replace("\'", "");
            }

            return para;
        }

    }
}
