using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// 转换帮助类
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// 转换Boolean
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ParseBoolean(object obj, bool defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            bool value;
            if (bool.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            else if (obj.ToString() == "1")
            {
                return true;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 转换Boolean
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ParseBoolean(object obj)
        {
            return ParseBoolean(obj, DbConvert.NullBoolean);
        }

        /// <summary>
        /// 转换Byte
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte ParseByte(object obj, byte defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            byte value;
            if (byte.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Byte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte ParseByte(object obj)
        {
            return ParseByte(obj, DbConvert.NullByte);
        }

        /// <summary>
        /// 转换DateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(object obj, DateTime defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            DateTime value;
            if (DateTime.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换DateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(object obj)
        {
            return ParseDateTime(obj, DbConvert.NullDateTime);
        }

        /// <summary>
        /// 转换Guid
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid ParseGuid(object obj, Guid defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            Guid value;
            if (Guid.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Guid
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid ParseGuid(object obj)
        {
            return ParseGuid(obj, DbConvert.NullGuid);
        }

        /// <summary>
        /// 转换Decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ParseDecimal(object obj, decimal defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            decimal value;
            if (decimal.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ParseDecimal(object obj)
        {
            return ParseDecimal(obj, DbConvert.NullDecimal);
        }

        /// <summary>
        /// 转换Double
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ParseDouble(object obj, double defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            double value;
            if (double.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ParseDouble(object obj)
        {
            return ParseDouble(obj, DbConvert.NullDouble);
        }

        /// <summary>
        /// 转换Int16
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ParseInt16(object obj, short defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            short value;
            if (short.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Int16
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static short ParseInt16(object obj)
        {
            return ParseInt16(obj, DbConvert.NullInt16);
        }

        /// <summary>
        /// 转换Int32
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ParseInt32(object obj, int defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            int value;
            if (int.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Int32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ParseInt32(object obj)
        {
            return ParseInt32(obj, DbConvert.NullInt32);
        }

        /// <summary>
        /// 转换Int64
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ParseInt64(object obj, long defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            long value;
            if (long.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Int64
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ParseInt64(object obj)
        {
            return ParseInt64(obj, DbConvert.NullInt64);
        }

        /// <summary>
        /// 转换SByte
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static sbyte ParseSByte(object obj, sbyte defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            sbyte value;
            if (sbyte.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换SByte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static sbyte ParseSByte(object obj)
        {
            return ParseSByte(obj, DbConvert.NullSByte);
        }

        /// <summary>
        /// 转换Float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ParseSingle(object obj, float defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            float value;
            if (float.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换Float
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float ParseSingle(object obj)
        {
            return ParseSingle(obj, DbConvert.NullSingle);
        }

        /// <summary>
        /// 转换TimeSpan
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TimeSpan ParseTimeSpan(object obj, TimeSpan defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            TimeSpan value;
            if (TimeSpan.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换TimeSpan
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TimeSpan ParseTimeSpan(object obj)
        {
            return ParseTimeSpan(obj, DbConvert.NullTimeSpan);
        }

        /// <summary>
        /// 转换UInt16
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ushort ParseUInt16(object obj, ushort defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            ushort value;
            if (ushort.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换UInt16
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ushort ParseUInt16(object obj)
        {
            return ParseUInt16(obj, DbConvert.NullUInt16);
        }

        /// <summary>
        /// 转换UInt32
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static uint ParseUInt32(object obj, uint defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            uint value;
            if (uint.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换UInt32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static uint ParseUInt32(object obj)
        {
            return ParseUInt32(obj, DbConvert.NullUInt32);
        }

        /// <summary>
        /// 转换UInt64
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ulong ParseUInt64(object obj, ulong defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            ulong value;
            if (ulong.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 转换UInt64
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ulong ParseUInt64(object obj)
        {
            return ParseUInt64(obj, DbConvert.NullUInt64);
        }
    }
}
