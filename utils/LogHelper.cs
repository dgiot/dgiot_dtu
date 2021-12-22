// <copyright file="LogHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Configuration;

namespace Dgiot_dtu
{
    public class LogHelper
    {
        private LogHelper()
        {
        }

        public enum Level
        {
            DEBUG,
            INFO,
            NOTICE,
            WARN,
            ERROR,
            CRITICAL,
            ALERT,
        }

        private static List<string> levelstring = new List<string>
        {
            "DEBUG",
            "INFO",
            "NOTICE",
            "WARN",
            "ERROR",
            "CRITICAL",
            "ALERT"
        };

        private static bool bDisplayHex = false;
        private static int loglevel = (int)Level.DEBUG;
        private static LogHelper instance;
        private static string login = string.Empty;
        private static MainForm mainform = null;

        public static LogHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new LogHelper();
            }

            return instance;
        }

        public static void Init(MainForm mainform, int level = 0)
        {
            LogHelper.mainform = mainform;
            loglevel = level;
        }

        public static List<string> Levels()
        {
            return levelstring;
        }

        public static void SetLevel(int level = 0)
        {
            loglevel = level;
        }

        public static void Log(string text, int level = 0)
        {
            if (level >= loglevel)
            {
                mainform.Log(text);
            }
        }

        public static void Config()
        {
             bDisplayHex = DgiotHelper.StrTobool(ConfigHelper.GetConfig("DisplayHex"));
        }

        public static string Logdata(byte[] data, int offset, int len)
        {
            var line = bDisplayHex ? DgiotHelper.ToHexString(data, offset, len) : System.Text.Encoding.ASCII.GetString(data, offset, len);
            if (line.EndsWith("\r\n"))
            {
                line = line.Substring(0, line.Length - 2);
            }

            return line;
        }

        public static byte[] Payload(char[] data)
        {
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(data);
            if (bDisplayHex)
            {
                byte[] hexPayload = DgiotHelper.ToHexBinary(payload);
                return hexPayload;
            }
            else
            {
                return payload;
            }
        }
    }
}