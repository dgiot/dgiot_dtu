// <copyright file="UDPClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.Threading;
    using LiteNetLib;

    public class UDPClientHelper
    {
        private const bool V = false;
        private static UDPClientHelper instance;
        private static NetManager client = null;
        private static string server = "prod.iotn2n.com";
        private static int port = 9050;
        private static bool bIsRun = V;
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

        public static void Start(KeyValueConfigurationCollection config, bool bAutoReconnect)
        {
            UDPClientHelper.bAutoReconnect = bAutoReconnect;
            bIsRun = true;
            Config(config);

            if (bIsCheck)
            {
                if (client == null)
                {
                    EventBasedNetListener listener = new EventBasedNetListener();
                    client = new NetManager(listener);
                    client.Start();
                    client.Connect(server /* host ip or name */, port /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
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

            bIsRun = false;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["UDPClientServer"] != null)
            {
               server = (string)config["UDPClientServer"].Value;
            }

            if (config["UDPClientPort"] != null)
            {
                port = int.Parse((string)config["UDPClientPort"].Value);
            }

            if (config["UDPClientIsCheck"] != null)
            {
                bIsCheck = DgiotHelper.StrTobool(config["UDPClientIsCheck"].Value);
            }
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
            }
        }
    }
 }