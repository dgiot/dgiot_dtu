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

        public static void Start(KeyValueConfigurationCollection config)
        {
            bIsRun = true;
            Config(config);
        }

        public static void Stop()
        {
            bIsRun = false;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["SqlServerIsCheck"] != null)
            {
                SqlServerHelper.bIsCheck = DgiotHelper.StrTobool(config["SqlServerIsCheck"].Value);
            }
        }
    }
}