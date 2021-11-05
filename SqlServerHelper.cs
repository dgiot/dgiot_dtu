// <copyright file="SqlServerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
using PortListener.Core.Utilities;
using System.Configuration;

namespace Dgiot_dtu
{
    public class SqlServerHelper
    {
        private SqlServerHelper()
        {
        }

        private static SqlServerHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRunning = false;
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
            SqlServerHelper.bIsRunning = true;
            Config(config, mainform);
            SqlServerHelper.mainform = mainform;
        }

        public static void Stop()
        {
            SqlServerHelper.bIsRunning = false;
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