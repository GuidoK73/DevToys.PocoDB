using System;
using System.Security;

namespace DevToys.PocoDB.Encryption
{ 
    internal static class FieldEncryption
    {
        public static object Decrypt(object value, SecureString password, bool Decrypt)
        {
            if (value == null)
                return DBNull.Value;

            if (value == DBNull.Value)
                return DBNull.Value;

            if (password.Length > 0 && Decrypt == true)
            {
                if (value.GetType() == typeof(string))
                {
                    string _value = Convert.ToString(value);
                    if (string.IsNullOrEmpty(_value))
                        return _value;

                    return StringCipher.Decrypt(_value, password);
                }
                if (value.GetType() == typeof(byte[]))
                {
                    byte[] binaryData = (byte[])value;
                    if (binaryData.Length == 0)
                        return binaryData;

                    return StringCipher.Decrypt(binaryData, password);
                }
            }
            return value;
        }

        public static object Encrypt(object value, SecureString password, bool Encrypt)
        {
            if (value == null)
                return DBNull.Value;

            if (password.Length > 0 && Encrypt == true)
            {
                if (value.GetType() == typeof(string))
                {
                    string _value = Convert.ToString(value);
                    if (string.IsNullOrEmpty(_value))
                        return _value;

                    return StringCipher.Encrypt(_value, password);
                }
                if (value.GetType() == typeof(byte[]))
                {
                    byte[] binaryData = (byte[])value;
                    if (binaryData.Length == 0)
                        return binaryData;

                    return StringCipher.Encrypt(binaryData, password);
                }
            }
            return value;
        }
    }
}