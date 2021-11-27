// <copyright file="TcpClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    public class TcpClientHelper
    {
        private TcpClientHelper()
        {
        }

        private static TcpClient client;
        private static TcpClientHelper instance;
        private static string login = string.Empty;
        private static NetworkStream stream;
        private static string server = "prod.iotn2n.com";
        private static int port;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
        private static bool bAutoReconnect = false;
        private static byte[] tcpdata = new byte[1024];

        public static TcpClientHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new TcpClientHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, bool bAutoReconnect)
        {
            Config(config);
            TcpClientHelper.bAutoReconnect = bAutoReconnect;
            if (bIsCheck)
            {
                CreateConnect();
            }
        }

        public static void Stop()
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    client.Close();
                }

                client = null;
            }
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["tcpClientServer"] != null)
            {
                server = (string)config["tcpClientServer"].Value;
            }

            if (config["tcpClientPort"] != null)
            {
                port = int.Parse((string)config["tcpClientPort"].Value);
            }

            if (config["tcpClientLogin"] != null)
            {
                login = config["tcpClientLogin"].Value;
            }

            if (config["tcpClientIsCheck"] != null)
            {
                bIsCheck = DgiotHelper.StrTobool(config["tcpClientIsCheck"].Value);
            }
        }

        public static void CreateConnect()
        {
            client = new TcpClient();
            client.BeginConnect(server, port, Connected, null);
        }

        private static void Connected(IAsyncResult result)
        {
            try
            {
                client.EndConnect(result);

                stream = client.GetStream();

                if (stream.CanWrite)
                {
                    Thread.Sleep(1000 * 1);

                    byte[] data = new byte[1024];

                    data = LogHelper.Payload(TcpClientHelper.login.ToCharArray());

                    LogHelper.Log("S->N: tcpClient login [" + LogHelper.Logdata(data, 0, data.Length) + "]");

                    stream.Write(data, 0, data.Length);
                }

                stream.BeginRead(tcpdata, 0, tcpdata.Length, Read, null);

                bIsRunning = true;
            }
            catch (Exception e)
            {
                LogHelper.Log("Couldn't connect: " + e.Message);
                OnConnectClosed();
            }
        }

        private static void Read(IAsyncResult ar)
        {
            try
            {
                var offset = 0;
                var rxbytes = stream.EndRead(ar);

                if (rxbytes > 0)
                {
                    stream.BeginRead(tcpdata, 0, tcpdata.Length, Read, null);

                    LogHelper.Log("N->S: tcpClient revc [" + LogHelper.Logdata(tcpdata, offset, rxbytes - offset) + "]");

                    TcpServerHelper.Write(tcpdata, offset, rxbytes - offset);

                    SerialPortHelper.Write(tcpdata, offset, rxbytes - offset);
                }

                if (rxbytes == 0)
                {
                    LogHelper.Log("Client closed");
                    OnConnectClosed();
                }
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    LogHelper.Log("Connection closed");
                }
                else if (e is IOException && e.Message.Contains("closed"))
                {
                    LogHelper.Log("Connection closed");
                }
                else
                {
                    LogHelper.Log("Exception: " + e.Message);
                }

                OnConnectClosed();
            }
        }

        private static void OnConnectClosed()
        {
            try
            {
                if (client != null)
                {
                    client.Close();
                    bIsRunning = false;
                    client = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("close client:" + ex.Message);
            }

            if (bAutoReconnect && bIsRunning)
            {
                try
                {
                    CreateConnect();
                }
                catch (Exception ex)
                {
                    LogHelper.Log("Problem reconnecting:" + ex.Message);
                }
            }
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
                if (stream.CanWrite)
                {
                    stream.Write(data, offset, len);
                }
            }
        }
    }
}