// <copyright file="UDPClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
using LiteNetLib;
using LiteNetLib.Utils;
using PortListener.Core.Utilities;
using System;
using System.Configuration;
using System.Threading;

namespace Dgiot_dtu
{
    public class UDPServerHelper
    {
        private static UDPServerHelper instance;
        private static NetManager server = null;
        private static MainForm mainform = null;
        private static int port;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
        private static bool bAutoReconnect = false;

        public static UDPServerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new UDPServerHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, MainForm mainform)
        {
            Config(config, mainform);
            UDPServerHelper.mainform = mainform;

            if (server == null)
            {
                EventBasedNetListener listener = new EventBasedNetListener();
                server = new NetManager(listener);
                server.Start(port /* port */);
                listener.ConnectionRequestEvent += request =>
                {
                    if (server.ConnectedPeersCount < 10 /* max connections */)
                    {
                        request.AcceptIfKey("SomeConnectionKey");
                    }
                    else
                    {
                        request.Reject();
                    }
                };

                listener.PeerConnectedEvent += peer =>
                {
                    Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
                    NetDataWriter writer = new NetDataWriter();                 // Create writer class
                    writer.Put("Hello client!");                                // Put some string
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
                };
            }

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
        }

        public static void Stop()
        {
            if (server != null)
            {
                if (server.IsRunning)
                {
                    server.Stop();
                }
            }

            UDPServerHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["updServerPort"] != null)
            {
                UDPServerHelper.port = int.Parse((string)config["UDPClientPort"].Value);
            }

            if (config["updbridgeIsCheck"] != null)
            {
                UDPServerHelper.bIsCheck = StringHelper.StrTobool(config["updbridgeIsCheck"].Value);
            }

            UDPServerHelper.mainform = mainform;
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
            }
        }
    }
 }