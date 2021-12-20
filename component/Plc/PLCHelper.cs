// <copyright file="PLCHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://www.cnblogs.com/dathlin/p/8685855.html
// https://github.com/dathlin/HslCommunication
namespace Dgiot_dtu
{
    using System.Configuration;
    using HslCommunication.Profinet.Siemens;

    public class PLCHelper
    {
        private PLCHelper()
        {
        }

        private static PLCHelper instance;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
        private static SiemensS7Net siemensTcpNet = null;

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
            siemensTcpNet = new SiemensS7Net(SiemensPLCS.S1200, "127.0.0.1")
            {
                ConnectTimeOut = 5000
            };
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

        public static void Write(byte[] data, int offset, int len)
        {
        }
    }
}