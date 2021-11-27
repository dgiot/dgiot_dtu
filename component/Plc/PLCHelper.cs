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

        public static void Start(bool bIsRunning)
        {
            PLCHelper.bIsRunning = bIsRunning;
        }

        public static void Stop()
        {
            PLCHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["PLCIsCheck"] != null)
            {
                bIsCheck = DgiotHelper.StrTobool(config["PLCIsCheck"].Value);
            }
        }

        public static void Check(bool isCheck)
        {
            bIsCheck = isCheck;
        }
    }
}