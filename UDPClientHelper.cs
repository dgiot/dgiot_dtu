// <copyright file="UDPClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
using LiteNetLib;
using PortListener.Core.Utilities;
using System;
using System.Configuration;
using System.Threading;

namespace Dgiot_dtu
{
    public class UDPClientHelper
    {
        private static UDPClientHelper instance;
        private static MainForm mainform = null;
        private static NetManager client = null;
        private static string server = "prod.iotn2n.com";
        private static int port;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
        private static bool bAutoReconnect = false;

        public static UDPClientHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new UDPClientHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, bool bAutoReconnect, MainForm mainform)
        {
            UDPClientHelper.bAutoReconnect = bAutoReconnect;
            bIsRunning = true;
            Config(config, mainform);

            if (bIsCheck)
            {
                if (client == null)
                {
                    EventBasedNetListener listener = new EventBasedNetListener();
                    client = new NetManager(listener);
                    client.Start();
                    client.Connect(server /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
                    listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
                    {
                        Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
                        dataReader.Recycle();
                    };
                }

                while (!Console.KeyAvailable)
                {
                    client.PollEvents();
                    Thread.Sleep(15);
                }
            }
        }

        public static void Stop()
        {
            if (client != null)
            {
                if (client.IsRunning)
                {
                    client.Stop();
                }
            }

            UDPClientHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["UDPClientServer"] != null)
            {
                UDPClientHelper.server = (string)config["UDPClientServer"].Value;
            }

            if (config["UDPClientPort"] != null)
            {
                UDPClientHelper.port = int.Parse((string)config["UDPClientPort"].Value);
            }

            if (config["UDPClientIsCheck"] != null)
            {
                UDPClientHelper.bIsCheck = StringHelper.StrTobool(config["UDPClientIsCheck"].Value);
            }

            UDPClientHelper.mainform = mainform;
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
            }
        }
    }
 }