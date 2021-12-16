// <copyright file="ConfigHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Configuration;

namespace Dgiot_dtu
{
    public class ConfigHelper
    {
        private ConfigHelper()
        {
        }

        private static ConfigHelper instance;
        private static Configuration config;

        public static ConfigHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new ConfigHelper();
            }

            return instance;
        }

        public static void Init(Configuration config)
        {
            ConfigHelper.config = config;
        }

        public static bool Check(string key)
        {
            return config.AppSettings.Settings[key] != null;
        }

        public static void SetConfig(string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
        }

        public static string GetConfig(string key)
        {
            if (Check(key))
            {
                return config.AppSettings.Settings[key].Value;
            }

            return "";
        }
     }
}