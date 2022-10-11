// <copyright file="DgiotHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    public class DgiotHelper
    {

        private static DgiotHelper instance = null;

        private DgiotHelper()
        {
        }

        public static DgiotHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new DgiotHelper();
            }

            return instance;
        }

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
            //localIps.Clear();
            //localIps.Add("127.0.0.1");
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
                int h = hexDigits.IndexOf(GetLow((char)hexString[pos])) << 4; // 注1
                int l = hexDigits.IndexOf(GetLow((char)hexString[pos + 1])); // 注2
                if (h == -1 || l == -1)
                { // 非16进制字符
                    return null;
                }

                bytes[i] = (byte)(h | l);
            }

            return bytes;
        }

        public static char GetLow(char var_ch)
        {
            char result = var_ch;
            if (var_ch >= 'a' && var_ch <= 'z')
            {
                result = (char)(var_ch - 32);
            }
            else if (var_ch >= 'A' && var_ch <= 'Z')
            {
                result = (char)(var_ch + 32);
            }

            return result;
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
                bytValue = Encoding.UTF8.GetBytes(str);
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

        private static readonly DateTime BaseTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将unixtime转换为.NET的DateTime
        /// </summary>
        /// <param name="timeStamp">秒数</param>
        /// <returns>转换后的时间</returns>
        public static DateTime FromUnixTime(long timeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime((timeStamp * 10000000) + BaseTime.Ticks));
        }

        /// <summary>
        /// 将.NET的DateTime转换为unix time
        /// </summary>
        /// <param name="dateTime">待转换的时间</param>
        /// <returns>转换后的unix time</returns>
        public static long FromDateTime(DateTime dateTime)
        {
            return (TimeZone.CurrentTimeZone.ToUniversalTime(dateTime).Ticks - BaseTime.Ticks) / 10000000;
        }

        public static long Now()
        {
            return (TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now).Ticks - BaseTime.Ticks) / 10000000;
        }

        public static long Ms()
        {
            return (TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now).Ticks - BaseTime.Ticks) / 10000;
        }

        public static void DgiotHub(string cmdType)
        {
            Process[] process1 = Process.GetProcesses();
            foreach (Process prc in process1)
            {
                if (prc.ProcessName == "erl")
                    prc.Kill();
            }

            foreach (Process prc in process1)
            {
                if (prc.ProcessName == "node")
                    prc.Kill();
            }

            LogHelper.Log("DgiotHub: " + cmdType);

            string AppPath = System.Environment.CurrentDirectory;
            string PgCmd = AppPath + "/postgres/bin/pg_ctl.exe";
            string ParseCmd = AppPath + "/node/pm2.cmd";
            string NSSMCmd = AppPath + "/node/nssm.exe";

            switch (cmdType)
            {
                case "install":
                    InstallPostgres(AppPath);
                    break;
                case "start":
                    // net start pgsql
                    string[] pgStartArgs = new string[] { "start", "\"pgsql\"" };
                    StartProcess("net", pgStartArgs);

                    //nssm install gofastd ./datacenter/file/file.exe
                    string[] filesInstallArgs = new string[] { "install", "\"gofastd\"", AppPath + "/datacenter/file/file.exe" };
                    StartProcess(NSSMCmd, filesInstallArgs);

                    // net start gofastd 
                    string[] filesStartArgs = new string[] { "start", "\"gofastd\"" };
                    StartProcess("net", filesStartArgs);

                    // pm2 start ./parse/server/index.js 
                    string[] parseStartArgs = new string[] { "start", AppPath + "/parse/server/index.js" };
                    StartProcess(ParseCmd, parseStartArgs);

                    // string FilePath = AppPath + "/emqx/bin/emqx.cmd";
                    // StartProcess(FilePath, args);

                    // System.Diagnostics.Process.Start("http://127.0.0.1:5080");
                    break;

                case "stop":

                    //pm2 delete index 
                    string[] parseStopArgs = new string[] { "delete", "index" };
                    StartProcess(ParseCmd, parseStopArgs);

                    //net stop gofastd 
                    string[] filesStopArgs = new string[] { "stop", "\"gofastd\"" };
                    StartProcess("net", filesStopArgs);

                    //net stop pgsql 
                    string[] pgStopArgs = new string[] { "stop", "\"pgsql\"" };
                    StartProcess("net", pgStopArgs);

                    //nssm remove gofastd confirm
                    // string[] filesRemoveArgs = new string[] { "remove", "\"gofastd\"", "confirm" };
                    // StartProcess(NSSMCmd, filesRemoveArgs);

                    //pg_ctl unregister -N pgsql
                    // string[] pgUnregisterArgs = new string[] { "unregister", "-N", "\"pgsql\"" };
                    // StartProcess(PgCmd, pgUnregisterArgs);

                    break;
                default:
                    break;
            }
        }

        static public void InstallPostgres(string AppPath)
        {
            string PgData = AppPath + "/datacenter/pgdata";
            string ParseSql = AppPath + "/datacenter/parsesql/parse_4.0.sql";
            string PgInit = AppPath + "/postgres/bin/initdb.exe";
            string PgCmd = AppPath + "/postgres/bin/pg_ctl.exe";
            string PgDump = AppPath + "/postgres/bin/pg_dump.exe";
            string PgSql = AppPath + "/postgres/bin/psql.exe";

            string strFilePath = AppPath + "/parse/script/.env";

            List<string> Ips = GetIps();
            String Ip = Ips[0].ToString();
            // parse .env 替换本地ip
            FileHelper.ReplaceValue(strFilePath, "SERVER_DOMAIN", "http://"+ Ip + ":1337");
            FileHelper.ReplaceValue(strFilePath, "SERVER_PUBLIC", "http://"+ Ip + ":1337");
            FileHelper.ReplaceValue(strFilePath, "GRAPHQL_PATH", "http://"+ Ip + ":1337/graphql");

            if (Directory.Exists(PgData) == false)
            {
                // initdb -D /data/dgiot/dgiot_pg_writer/data/
                string[] pgInitArgs = new string[] { "-D", PgData, "-E", "UTF8", "-U", "postgres" };
                StartProcess(PgInit, pgInitArgs);
            };

            Thread.Sleep(6000);

            // pg_ctl register  -N pgsql  -D ./datacenter/pgdata
            string[] pgInstallArgs = new string[] { "register", "-N", "\"pgsql\"", "-D", PgData };
            StartProcess(PgCmd, pgInstallArgs);

            // net start pgsql
            string[] pgStartArgs = new string[] { "start", "\"pgsql\"" };
            StartProcess("net", pgStartArgs);

            //psql - U postgres - c "ALTER USER postgres WITH PASSWORD '${pg_pwd}';"
            string[] pgchangePwdArgs = new string[] { "-U", "postgres", "-c", "\"ALTER USER postgres WITH PASSWORD \"dgiot1344\"\"" };
            StartProcess(PgSql, pgchangePwdArgs);

            //psql - U postgres - c "CREATE DATABASE parse;"
            string[] CreateDataBaseArgs = new string[] { "-U", "postgres", "-c", "\"CREATE DATABASE parse;\"" };
            StartProcess(PgSql, CreateDataBaseArgs);

            //pg_dump - F p - f  ${ backup_dir}/ dgiot_pg_writer / parse_4.0_backup.sql - C - E  UTF8 - h 127.0.0.1 - U postgres parse

            //psql - U postgres -f /datacenter/parsesql/parse_4.0.sql parse
            string[] ImportSqlArgs = new string[] { "-U", "postgres", "-f",  ParseSql, "parse"};
            StartProcess(PgSql, ImportSqlArgs);
        }
        
        static public bool StartProcess(string filename, string[] args)
        {
            Thread.Sleep(1000);
            try
            {
                string s = "";
                foreach (string arg in args)
                {
                    s = s + arg + " ";
                }
                s = s.Trim();
                Process myprocess = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo(filename, s);
                myprocess.StartInfo = startInfo;

                //通过以下参数可以控制exe的启动方式，具体参照 myprocess.StartInfo.下面的参数，如以无界面方式启动exe等
                myprocess.StartInfo.UseShellExecute = false;
                myprocess.Start();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log("启动应用程序时出错！原因：" + ex.Message);
            }
            return false;
        }
    }
}
