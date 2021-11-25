// <copyright file="PLCHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Configuration;

namespace Dgiot_dtu
{
    public class PLCHelper
    {
        private PLCHelper()
        {
        }

        private static PLCHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;

        public static PLCHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new PLCHelper();
            }

            return instance;
        }

        public static void Start(bool bIsRunning, MainForm mainform)
        {
            PLCHelper.bIsRunning = bIsRunning;
            PLCHelper.mainform = mainform;
        }

        public static void Stop()
        {
            PLCHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["PLCIsCheck"] != null)
            {
                PLCHelper.bIsCheck = StringHelper.StrTobool(config["PLCIsCheck"].Value);
            }

            PLCHelper.mainform = mainform;
        }

        public static void Check(bool isCheck, MainForm mainform)
        {
            PLCHelper.bIsCheck = isCheck;
            PLCHelper.mainform = mainform;
        }
    }
}