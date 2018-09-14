using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// 数据转换类
    /// </summary>
    public static class DbConvert
    {
        /// <summary>
        /// 转换为数据库不可为空得可识别对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object ToDbValue(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                if (obj is bool)
                {
                    return (object)NullBoolean;
                }
                if (obj is byte)
                {
                    return (object)NullByte;
                }
                if (obj is char)
                {
                    return (object)NullChar;
                }
                if (obj is DateTime)
                {
                    return (object)NullDateTime;
                }
                if (obj is decimal)
                {
                    return (object)NullDecimal;
                }
                if (obj is double)
                {
                    return (object)NullDouble;
                }
                if (obj is Guid)
                {
                    return (object)NullGuid;
                }
                if (obj is short)
                {
                    return (object)NullInt16;
                }
                if (obj is int)
                {
                    return (object)NullInt32;
                }
                if (obj is long)
                {
                    return (object)NullInt64;
                }
                if (obj is IntPtr)
                {
                    return (object)NullIntPtr;
                }
                if (obj is SByte)
                {
                    return (object)NullSByte;
                }
                if (obj is float)
                {
                    return (object)NullSingle;
                }
                if (obj is TimeSpan)
                {
                    return (object)NullTimeSpan;
                }
                if (obj is ushort)
                {
                    return (object)NullUInt16;
                }
                if (obj is uint)
                {
                    return (object)NullUInt32;
                }
                if (obj is ulong)
                {
                    return (object)NullUInt64;
                }
                if (obj is UIntPtr)
                {
                    return (object)NullUIntPtr;
                }
                throw new ArgumentException();
            }

            return obj;
        }

        /// <summary>
        /// 转换为数据库可为空的可识别对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object ToDbValueNullable(object obj)
        {
            if (obj == null)
            {
                return DBNull.Value;
            }
            return obj;
        }

        /// <summary>
        /// 数据库转换为Boolean值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetBoolean(object obj, bool defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToBoolean(obj);
        }

        /// <summary>
        /// 数据库转换为Boolean值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetBoolean(object obj)
        {
            return GetBoolean(obj, NullBoolean);
        }

        /// <summary>
        /// 数据库转换为Boolean值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool? GetBooleanNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToBoolean(obj);
        }

        /// <summary>
        /// 数据库转换为Byte值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte GetByte(object obj, byte defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToByte(obj);
        }

        /// <summary>
        /// 数据库转换为Byte值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte GetByte(object obj)
        {
            return GetByte(obj, NullByte);
        }

        /// <summary>
        /// 数据库转换为Byte值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte? GetByteNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToByte(obj);
        }

        /// <summary>
        /// 数据库转换为Bytes值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte[] GetBytes(object obj, byte[] defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return (byte[])(obj);
        }

        /// <summary>
        /// 数据库转换为Bytes值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetBytes(object obj)
        {
            return GetBytes(obj, null);
        }

        /// <summary>
        /// 数据库转换为Char值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static char GetChar(object obj, char defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToChar(obj);
        }

        /// <summary>
        /// 数据库转换为Char值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static char GetChar(object obj)
        {
            return GetChar(obj, NullChar);
        }

        /// <summary>
        /// 数据库转换为Char值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static char? GetCharNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToChar(obj);
        }

        /// <summary>
        /// 数据库转换为DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defalutValue"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(object obj, DateTime defalutValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defalutValue;
            }
            return Convert.ToDateTime(obj);
        }

        /// <summary>
        /// 数据库转换为DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(object obj)
        {
            return GetDateTime(obj, NullDateTime);
        }

        /// <summary>
        /// 数据库转换为DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDateTime(obj);
        }

        /// <summary>
        /// 数据库转换为Decimal值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal GetDecimal(object obj, decimal defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToDecimal(obj);
        }

        /// <summary>
        /// 数据库转换为Decimal值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal GetDecimal(object obj)
        {
            return GetDecimal(obj, NullDecimal);
        }

        /// <summary>
        /// 数据库转换为Decimal值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? GetDecimalNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(obj);
        }

        /// <summary>
        /// 数据库转换为Double值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double GetDouble(object obj, double defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToDouble(obj);
        }

        /// <summary>
        /// 数据库转换为Double值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetDouble(object obj)
        {
            return GetDouble(obj, NullDouble);
        }

        /// <summary>
        /// 数据库转换为Double值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double? GetDoubleNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDouble(obj);
        }

        /// <summary>
        /// 数据库转换为Guid值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid GetGuid(object obj, Guid defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return (Guid)(obj);
        }

        /// <summary>
        /// 数据库转换为Guid值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid GetGuid(object obj)
        {
            return GetGuid(obj, NullGuid);
        }

        /// <summary>
        /// 数据库转换为Guid值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid? GetGuidNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return (Guid)(obj);
        }

        /// <summary>
        /// 数据库转换为Int16值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short GetInt16(object obj, short defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToInt16(obj);
        }

        /// <summary>
        /// 数据库转换为Int16值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static short GetInt16(object obj)
        {
            return GetInt16(obj, NullInt16);
        }

        /// <summary>
        /// 数据库转换为Int16值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static short? GetInt16Nullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt16(obj);
        }

        /// <summary>
        /// 数据库转换为Int32值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt32(object obj, int defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 数据库转换为Int32值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetInt32(object obj)
        {
            return GetInt32(obj, NullInt32);
        }

        /// <summary>
        /// 数据库转换为Int32值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? GetInt32Nullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 数据库转换为Int64值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long GetInt64(object obj, long defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToInt64(obj);
        }

        /// <summary>
        /// 数据库转换为Int64值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetInt64(object obj)
        {
            return GetInt64(obj, NullInt64);
        }

        /// <summary>
        /// 数据库转换为Int64值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long? GetInt64Nullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt64(obj);
        }

        /// <summary>
        /// 数据库转换为IntPtr值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static IntPtr GetIntPtr(object obj, IntPtr defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return (IntPtr)(obj);
        }

        /// <summary>
        /// 数据库转换为IntPtr值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IntPtr GetIntPtr(object obj)
        {
            return GetIntPtr(obj, NullIntPtr);
        }

        /// <summary>
        /// 数据库转换为IntPtr值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IntPtr? GetIntPtrNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return (IntPtr)(obj);
        }

        /// <summary>
        /// 数据库转换为SByte值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static sbyte GetSByte(object obj, sbyte defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToSByte(obj);
        }

        /// <summary>
        /// 数据库转换为SByte值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static sbyte GetSByte(object obj)
        {
            return GetSByte(obj, NullSByte);
        }

        /// <summary>
        /// 数据库转换为SByte值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static sbyte? GetSByteNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToSByte(obj);
        }

        /// <summary>
        /// 数据库转换为Single值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetSingle(object obj, float defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToSingle(obj);
        }

        /// <summary>
        /// 数据库转换为Single值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetSingle(object obj)
        {
            return GetSingle(obj, NullSingle);
        }

        /// <summary>
        /// 数据库转换为Single值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float? GetSingleNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToSingle(obj);
        }

        /// <summary>
        /// 数据库转换为String值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString(object obj, string defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToString(obj);
        }

        /// <summary>
        /// 数据库转换为String值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetString(object obj)
        {
            return GetString(obj, null);
        }

        /// <summary>
        /// 数据库转换为TimeSpan值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(object obj, TimeSpan defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return (TimeSpan)(obj);
        }

        /// <summary>
        /// 数据库转换为TimeSpan值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(object obj)
        {
            return GetTimeSpan(obj, NullTimeSpan);
        }

        /// <summary>
        /// 数据库转换为TimeSpan值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TimeSpan? GetTimeSpanNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return (TimeSpan)(obj);
        }

        /// <summary>
        /// 数据库转换为UInt16值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ushort GetUInt16(object obj, ushort defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToUInt16(obj);
        }

        /// <summary>
        /// 数据库转换为UInt16值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ushort GetUInt16(object obj)
        {
            return GetUInt16(obj, NullUInt16);
        }

        /// <summary>
        /// 数据库转换为UInt16值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ushort? GetUInt16Nullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToUInt16(obj);
        }

        /// <summary>
        /// 数据库转换为UInt32值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static uint GetUInt32(object obj, uint defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToUInt32(obj);
        }

        /// <summary>
        /// 数据库转换为UInt32值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static uint GetUInt32(object obj)
        {
            return GetUInt32(obj, NullUInt32);
        }

        /// <summary>
        /// 数据库转换为UInt32值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static uint? GetUInt32Nullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToUInt32(obj);
        }

        /// <summary>
        /// 数据库转换为UInt64值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ulong GetUInt64(object obj, ulong defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return Convert.ToUInt64(obj);
        }

        /// <summary>
        /// 数据库转换为UInt64值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ulong GetUInt64(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return NullUInt64;
            }
            return GetUInt64(obj, NullUInt64);
        }

        /// <summary>
        /// 数据库转换为UInt64值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ulong? GetUInt64Nullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return Convert.ToUInt64(obj);
        }

        /// <summary>
        /// 数据库转换为UIntPtr值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static UIntPtr GetUIntPtr(object obj, UIntPtr defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }
            return (UIntPtr)(obj);
        }

        /// <summary>
        /// 数据库转换为UIntPtr值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UIntPtr GetUIntPtr(object obj)
        {
            return GetUIntPtr(obj, NullUIntPtr);
        }

        /// <summary>
        /// 数据库转换为UIntPtr值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UIntPtr? GetUIntPtrNullable(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            return (UIntPtr)(obj);
        }

        /// <summary>
        /// 判断是否为空值
        /// </summary>
        /// <param name="obj">供判断的值</param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                return true;
            }

            if (obj is bool)
            {
                return obj.Equals(NullBoolean);
            }
            if (obj is byte)
            {
                return obj.Equals(NullByte);
            }
            if (obj is char)
            {
                return obj.Equals(NullChar);
            }
            if (obj is DateTime)
            {
                return obj.Equals(NullDateTime);
            }
            if (obj is decimal)
            {
                return obj.Equals(NullDecimal);
            }
            if (obj is double)
            {
                return obj.Equals(NullDouble);
            }
            if (obj is Guid)
            {
                return obj.Equals(NullGuid);
            }
            if (obj is short)
            {
                return obj.Equals(NullInt16);
            }
            if (obj is int)
            {
                return obj.Equals(NullInt32);
            }
            if (obj is long)
            {
                return obj.Equals(NullInt64);
            }
            if (obj is IntPtr)
            {
                return obj.Equals(NullIntPtr);
            }
            if (obj is SByte)
            {
                return obj.Equals(NullSByte);
            }
            if (obj is float)
            {
                return obj.Equals(NullSingle);
            }
            if (obj is TimeSpan)
            {
                return obj.Equals(NullTimeSpan);
            }
            if (obj is ushort)
            {
                return obj.Equals(NullUInt16);
            }
            if (obj is uint)
            {
                return obj.Equals(NullUInt32);
            }
            if (obj is ulong)
            {
                return obj.Equals(NullUInt64);
            }
            if (obj is UIntPtr)
            {
                return obj.Equals(NullUIntPtr);
            }

            return false;
        }

        /// <summary>
        /// 判断是否为空值
        /// </summary>
        /// <param name="obj">供判断的值</param>
        /// <returns></returns>
        public static bool IsDbNull(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                return true;
            }

            return false;
        }

        #region NULL值

        /// <summary>
        /// Boolean空值
        /// </summary>
        public const bool NullBoolean = false;

        /// <summary>
        /// Byte空值
        /// </summary>
        public const byte NullByte = 0;

        /// <summary>
        /// Char空值
        /// </summary>
        public const char NullChar = Char.MinValue;

        /// <summary>
        /// Date空值
        /// </summary>
        public static readonly DateTime NullDateTime = DateTime.MinValue;

        /// <summary>
        /// Decimal空值
        /// </summary>
        public const decimal NullDecimal = 0;

        /// <summary>
        /// Double空值
        /// </summary>
        public const double NullDouble = 0;

        /// <summary>
        /// Single空值
        /// </summary>
        public const float NullSingle = 0;

        /// <summary>
        /// Guid空值
        /// </summary>
        public static readonly Guid NullGuid = Guid.Empty;

        /// <summary>
        /// Int16空值
        /// </summary>
        public const short NullInt16 = 0;

        /// <summary>
        /// Int32空值
        /// </summary>
        public const int NullInt32 = 0;

        /// <summary>
        /// Int64空值
        /// </summary>
        public const int NullInt64 = 0;

        /// <summary>
        /// IntPtr空值
        /// </summary>
        public static readonly IntPtr NullIntPtr = IntPtr.Zero;

        /// <summary>
        /// SByte空值
        /// </summary>
        public const sbyte NullSByte = 0;

        /// <summary>
        /// TimeSpan空值
        /// </summary>
        public static readonly TimeSpan NullTimeSpan = TimeSpan.MinValue;

        /// <summary>
        /// UInt16空值
        /// </summary>
        public const ushort NullUInt16 = 0;

        /// <summary>
        /// UInt32空值
        /// </summary>
        public const uint NullUInt32 = 0;

        /// <summary>
        /// UInt64空值
        /// </summary>
        public const ulong NullUInt64 = 0;

        /// <summary>
        /// UIntPtr空值
        /// </summary>
        public static readonly UIntPtr NullUIntPtr = UIntPtr.Zero;

        #endregion
    }
}
