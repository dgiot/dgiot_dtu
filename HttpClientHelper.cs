// <copyright file="HttpClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
namespace Dgiot_dtu
{
    public class HttpClientHelper
    {
        private HttpClientHelper()
        {
        }

        private static HttpClientHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;

        public static HttpClientHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new HttpClientHelper();
            }

            return instance;
        }

        public static void Start(bool bIsRunning, MainForm mainform)
        {
            HttpClientHelper.bIsRunning = bIsRunning;
            HttpClientHelper.mainform = mainform;
        }

        public static void Stop()
        {
            HttpClientHelper.bIsRunning = false;
        }

        public static void Check(bool isCheck, MainForm mainform)
        {
            HttpClientHelper.bIsCheck = isCheck;
            HttpClientHelper.mainform = mainform;
        }
    }
}