// <copyright file="StringHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PortListener.Core.Utilities
{
    using System.Configuration;
    using System.Text;

    public class StringHelper
    {
        public static string ToHexString(string s)
        {
            return ToHexString(Encoding.ASCII.GetBytes(s));
        }

        public static string ToHexString(byte[] arrBytes)
        {
            return ToHexString(arrBytes, 0, arrBytes.Length);
        }

        public static string ToHexString(byte[] arrBytes, int iOffset, int iLength)
        {
            if (arrBytes.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(string.Empty);

            var i = 0;
            for (; i < iLength - 1; i++)
            {
                sb.AppendFormat("{0:X2} ", arrBytes[i]);
            }

            sb.AppendFormat("{0:X2}", arrBytes[i]);

            return sb.ToString();
        }

        public static string ToHexString(byte data)
        {
            return string.Format("{0:X2}", data);
        }

        public static byte[] ToHexBinary(byte[] hexString)
        {
            if (hexString == null)
            {
                return null;
            }

            int length = hexString.Length / 2;

            byte[] bytes = new byte[length];
            string hexDigits = "0123456789abcdef";
            for (int i = 0; i < length; i++)
            {
                int pos = i * 2; // 两个字符对应一个byte
                int h = hexDigits.IndexOf((char)hexString[pos]) << 4; // 注1
                int l = hexDigits.IndexOf((char)hexString[pos + 1]); // 注2
                if (h == -1 || l == -1)
                { // 非16进制字符
                    return null;
                }

                bytes[i] = (byte)(h | l);
            }

            return bytes;
        }

        public static bool StrTobool(string s)
        {
            if (s == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string BoolTostr(bool isTrue)
        {
            if (isTrue)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }
    }
}
