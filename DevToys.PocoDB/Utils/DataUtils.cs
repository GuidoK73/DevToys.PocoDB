using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DevToys.PocoDB
{
    internal static class DataUtils
    {
        public const char _DOUBLEQUOTE = '"';
        public const char _SINGLEQUOTE = '\'';

        public enum NetType
        {
            String = 1,
            Guid = 2,
            Boolean = 3,
            DateTime = 4,
            DateTimeOffset = 5,
            TimeSpan = 6,
            Byte = 7,
            SByte = 8,
            Int16 = 9,
            Int32 = 10,
            Int64 = 11,
            Single = 12,
            Decimal = 13,
            Double = 14,
            UInt16 = 15,
            UInt32 = 16,
            UInt64 = 17,
            Enum = 18
        }

        public static string CamelCaseUpper(string name)
        {
            StringBuilder _result = new StringBuilder();
            char[] chars = name.ToCharArray();
            bool nextCap = true;
            foreach (char c in chars)
            {
                if (char.IsLetter(c))
                {
                    if (nextCap)
                    {
                        _result.Append(char.ToUpper(c));
                        nextCap = false;
                    }
                    else
                        _result.Append(c);
                }
                else
                    nextCap = true;
                if (nextCap)
                    continue;
            }
            return _result.ToString();
        }

        /// <summary>
        /// Removes all non alpha except for '_', replaces space with '_'.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CleanString(string name)
        {
            StringBuilder _result = new StringBuilder();
            char[] chars = name.ToCharArray();

            foreach (char c in chars)
                if (char.IsLetter(c) || c == '_' || c == ' ')
                    _result.Append(c == ' ' ? '_' : c);

            return _result.ToString();
        }

        /// <summary>
        /// Return Max Value for a Numeric / DateTime / TimeSpan
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetMaxValue(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(IntPtr))
                return IntPtr.Size;

            if (type == typeof(UIntPtr))
                return UIntPtr.Size;

            if (type == typeof(Int16))
                return Int16.MaxValue;

            if (type == typeof(Int32))
                return Int32.MaxValue;

            if (type == typeof(Int64))
                return Int64.MaxValue;

            if (type == typeof(UInt16))
                return UInt16.MaxValue;

            if (type == typeof(UInt32))
                return UInt32.MaxValue;

            if (type == typeof(UInt64))
                return UInt64.MaxValue;

            if (type == typeof(Double))
                return Double.MaxValue;

            if (type == typeof(Decimal))
                return Decimal.MaxValue;

            if (type == typeof(Byte))
                return Byte.MaxValue;

            if (type == typeof(SByte))
                return SByte.MaxValue;

            if (type == typeof(DateTime))
                return DateTime.MaxValue;

            if (type == typeof(TimeSpan))
                return TimeSpan.MaxValue;

            return null;
        }

        /// <summary>
        /// Return Min Value for a Numeric / DateTime / TimeSpan
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetMinValue(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(IntPtr))
                return IntPtr.Zero;

            if (type == typeof(UIntPtr))
                return UIntPtr.Zero;

            if (type == typeof(Int16))
                return Int16.MinValue;

            if (type == typeof(Int32))
                return Int32.MinValue;

            if (type == typeof(Int64))
                return Int64.MinValue;

            if (type == typeof(UInt16))
                return UInt16.MinValue;

            if (type == typeof(UInt32))
                return UInt32.MinValue;

            if (type == typeof(UInt64))
                return UInt64.MinValue;

            if (type == typeof(Double))
                return Double.MinValue;

            if (type == typeof(Decimal))
                return Decimal.MinValue;

            if (type == typeof(Byte))
                return Byte.MinValue;

            if (type == typeof(SByte))
                return SByte.MinValue;

            if (type == typeof(DateTime))
                return DateTime.MinValue;

            if (type == typeof(TimeSpan))
                return TimeSpan.MinValue;

            return null;
        }

        public static NetType GetNetType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsEnum)
                return NetType.Enum;

            if (type == typeof(String))
                return NetType.String; // Alternative: char[], char, NText, NChar, NText, NVarChar, Varchar

            if (type == typeof(Guid))
                return NetType.Guid;

            if (type == typeof(Boolean))
                return NetType.Boolean;

            if (type == typeof(DateTime))
                return NetType.DateTime; // Alternative: SmallDateTime or DateTime2

            if (type == typeof(DateTimeOffset))
                return NetType.DateTimeOffset;

            if (type == typeof(TimeSpan))
                return NetType.TimeSpan;

            if (type == typeof(Byte))
                return NetType.Byte;

            if (type == typeof(SByte))
                return NetType.SByte;

            if (type == typeof(Int16))
                return NetType.Int16;

            if (type == typeof(Int32))
                return NetType.Int32;

            if (type == typeof(Int64))
                return NetType.Int64;

            if (type == typeof(UInt16))
                return NetType.UInt16; // one larger to fit unsigned maximum

            if (type == typeof(UInt32))
                return NetType.UInt32; // one larger to fit unsigned maximum

            if (type == typeof(UInt64))
                return NetType.UInt64; // one larger to fit unsigned maximum

            if (type == typeof(Single))
                return NetType.Single;

            if (type == typeof(Decimal))
                return NetType.Decimal; // Alternative: Money, SmallMoney

            if (type == typeof(Double))
                return NetType.Double;

            return NetType.String;
        }

        public static string[] GetReaderColumns(IDataReader reader)
        {
            string[] _result = new string[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
                _result[i] = reader.GetName(i).ToLower();

            return _result;
        }

        /// <summary>
        /// Determines whether a type is simple like string or int, etc.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(Byte[])
                || (type == typeof(String)
                || type == typeof(Boolean)
                || type == typeof(Byte)
                || type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64))
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(IntPtr)
                || type == typeof(UIntPtr)
                || type == typeof(Single)
                || type == typeof(Double)
                || type == typeof(Decimal)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan))
                return true;

            return false;
        }

        public static string NormalizeQuotes(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            char[] array = value.ToCharArray();
            for (int ii = 0; ii < array.Length; ii++)
            {
                if (array[ii] == '‘' || array[ii] == '’' || array[ii] == '‚')
                    array[ii] = _SINGLEQUOTE;

                if (array[ii] == '“' || array[ii] == '”' || array[ii] == '„' || array[ii] == '¨')
                    array[ii] = _DOUBLEQUOTE;
            }
            string s = new string(array);
            return s.Replace("'", "''");
        }

        internal static bool DerivesFromBaseType(Type type, Type basetype)
        {
            if (type == basetype)
                return true;

            if (type == null)
                return false;

            if (type.BaseType != null)
            {
                if (type.BaseType == basetype)
                    return true;

                return DerivesFromBaseType(type.BaseType, basetype);
            }
            return false;
        }
    }
}