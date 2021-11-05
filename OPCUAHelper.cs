// <copyright file="OPCUAHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
using PortListener.Core.Utilities;
using System.Configuration;

namespace Dgiot_dtu
{
    public class OPCUAHelper
    {
        private OPCUAHelper()
        {
        }

        private static OPCUAHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;

        public static OPCUAHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new OPCUAHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, MainForm mainform)
        {
            Config(config, mainform);
            OPCUAHelper.bIsRunning = true;
        }

        public static void Stop()
        {
            OPCUAHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["OPCUAIsCheck"] != null)
            {
                OPCUAHelper.bIsCheck = StringHelper.StrTobool(config["OPCUAIsCheck"].Value);
            }

            OPCUAHelper.mainform = mainform;
        }
    }
}