// <copyright file="OPCUAHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Workstation.ServiceModel.Ua;
    using Workstation.ServiceModel.Ua.Channels;

    public class OPCUAHelper
    {
        private OPCUAHelper()
        {
        }

        private const string EndpointUrl = "opc.tcp://localhost:26543"; // the endpoint of the Workstation.NodeServer.
        private const bool V = false;
        private static ApplicationDescription localDescription;
        private static ICertificateStore certificateStore;
        private static OPCUAHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRun = V;
        private static bool bIsCheck = V;
        private static UaTcpSessionChannel channel;

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
            bIsRun = true;
            localDescription = new ApplicationDescription
            {
                ApplicationName = "Workstation.UaClient.UnitTests",
                ApplicationUri = $"urn:{Dns.GetHostName()}:Workstation.UaClient.UnitTests",
                ApplicationType = ApplicationType.Client
            };

            certificateStore = new DirectoryStore(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Workstation.UaClient.UnitTests",
                    "pki"));

            Task.Run(async () => { await ConnectAsync(); });
        }

        public static void Stop()
        {
            bIsRun = V;
            if (channel != null)
            {
                Task.Run(async () => { await CloseAsync(); });
            }
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["OPCUAIsCheck"] != null)
            {
                bIsCheck = StringHelper.StrTobool(config["OPCUAIsCheck"].Value);
            }

            OPCUAHelper.mainform = mainform;
        }

        private static async Task ConnectAsync()
        {
           channel = new UaTcpSessionChannel(
                 localDescription,
                 certificateStore,
                 null,
                 EndpointUrl);

            await channel.OpenAsync();
        }

        private static async Task CloseAsync()
        {
            await channel.CloseAsync();
        }
    }
}