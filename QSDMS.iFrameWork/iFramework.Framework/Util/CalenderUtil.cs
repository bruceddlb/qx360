using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace iFramework.Framework.Util
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    public static class CalenderUtil
    {
        /// <summary>
        /// 获取年中周
        /// <remarks>星期一为周的第一天</remarks>
        /// <remarks>星期一所在周为第一周</remarks>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(DateTime date)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            return GetWeekOfYear(date, culture);
        }

        /// <summary>
        /// 获取年中周
        /// <remarks>星期一为周的第一天</remarks>
        /// <remarks>星期一所在周为第一周</remarks>
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(DateTime date, CultureInfo culture)
        {
            return culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// 获取输入时间点归属周的星期一
        /// <remarks>星期一为周的第一天</remarks>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetMonday(DateTime date)
        {
            return GetWeekDay(date, DayOfWeek.Monday);
        }

        /// <summary>
        /// 获取输入时间点归属周的星期几
        /// <remarks>星期一为周的第一天</remarks>
        /// </summary>
        /// <param name="date"></param>
        /// <param name="weekday">输出星期几</param>
        /// <returns></returns>
        public static DateTime GetWeekDay(DateTime date, DayOfWeek weekday)
        {
            int week1 = (int)weekday;
            if (week1 == 0)
            {
                // 特殊处理星期天
                week1 = 7;
            }

            int week2 = (int)(date.DayOfWeek);
            if (week2 == 0)
            {
                // 特殊处理星期天
                week2 = 7;
            }

            date = date.AddDays(week1 - week2);
            return date;
        }
    }
}
