// <copyright file="UDPServerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.Threading;

    using LiteNetLib;
    using LiteNetLib.Utils;

    public class UDPServerHelper
    {
        private static UDPServerHelper instance;
        private static NetManager server = null;
        private static int port;
        private static bool bIsCheck = false;

        public static UDPServerHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UDPServerHelper();
                }

                return instance;
            }
        }

        public static void Start()
        {
            Config();
            if (bIsCheck)
            {
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
        }

        public static void Config()
        {
            port = int.Parse(ConfigHelper.GetConfig("DgiotPort"));
            if (DgiotHelper.StrTobool(ConfigHelper.GetConfig("UDPClient_Checked")) && DgiotHelper.StrTobool(ConfigHelper.GetConfig("Bridge_Checked")))
            {
                bIsCheck = true;
            }
            else
            {
                bIsCheck = false;
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