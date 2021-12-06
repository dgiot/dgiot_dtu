// <copyright file="DgiotHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;

    public class DgiotHelper
    {
        public static List<string> GetIps()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> localIps = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    using (Ping p = new Ping())
                    {
                        PingReply pingReply = p.Send(ip, 100);
                        if (pingReply.Status == IPStatus.Success)
                        {
                            localIps.Add(ip.ToString());
                        }
                    }
                }
            }

            localIps.Add("127.0.0.1");
            return localIps;
        }

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

        /// <summary>
        /// 通过WMI读取系统信息里的网卡MAC
        /// </summary>
        /// <returns> list </returns>
        public static List<string> GetMacByWmi()
        {
            try
            {
                List<string> macs = new List<string>();
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        var mac = mo["MacAddress"].ToString();
                        macs.Add(mac);
                    }
                }

                return macs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Md5(string str)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(str);
                bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
                }

                str = sTemp.ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return str;
        }
    }
}
