// <copyright file="SqlServerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Configuration;

namespace Dgiot_dtu
{
    public class SqlServerHelper
    {
        private SqlServerHelper()
        {
        }

        private const bool V = false;
        private static SqlServerHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRun = V;
        private static bool bIsCheck = false;

        public static SqlServerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new SqlServerHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config,  MainForm mainform)
        {
            bIsRun = true;
            Config(config, mainform);
            SqlServerHelper.mainform = mainform;
        }

        public static void Stop()
        {
            bIsRun = false;
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["SqlServerIsCheck"] != null)
            {
                SqlServerHelper.bIsCheck = StringHelper.StrTobool(config["SqlServerIsCheck"].Value);
            }

            SqlServerHelper.mainform = mainform;
        }
    }
}