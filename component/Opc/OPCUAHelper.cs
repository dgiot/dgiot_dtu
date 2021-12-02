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
        private static ApplicationDescription localDescription;
        private static ICertificateStore certificateStore;
        private static OPCUAHelper instance;
        private static UaTcpSessionChannel channel;

        public static OPCUAHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new OPCUAHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config)
        {
            Config(config);
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
            if (channel != null)
            {
                Task.Run(async () => { await CloseAsync(); });
            }
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
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