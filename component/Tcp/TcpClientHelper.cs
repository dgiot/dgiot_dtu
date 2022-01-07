// <copyright file="TcpClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
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

        public static void Start()
        {
            Config();
            LogHelper.Log("TcpClient_Checked " + ConfigHelper.GetConfig("TcpClient_Checked"));
            if (DgiotHelper.StrTobool(ConfigHelper.GetConfig("TcpClient_Checked")))
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

        public static void Config()
        {
            server = ConfigHelper.GetConfig("DgiotSever");
            port = int.Parse(ConfigHelper.GetConfig("DgiotPort"));
            bAutoReconnect = DgiotHelper.StrTobool(ConfigHelper.GetConfig("ReconnectChecked"));
            bIsCheck = DgiotHelper.StrTobool(ConfigHelper.GetConfig("TcpClient_Checked"));
            login = ConfigHelper.GetConfig("TcpClientLogin");
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

                    data = System.Text.Encoding.UTF8.GetBytes(login.ToCharArray());

                    LogHelper.Log("TcpClient: login [" + login + "]");

                    stream.Write(data, 0, data.Length);
                }

                stream.BeginRead(tcpdata, 0, tcpdata.Length, Read, null);

                bIsRunning = true;
            }
            catch (Exception e)
            {
                LogHelper.Log("TcpClient Couldn't connect: " + e.Message);
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

                    LogHelper.Log("TcpClient Recv: [" + LogHelper.Logdata(tcpdata, offset, rxbytes - offset) + "]");

                    TcpServerHelper.Write(tcpdata, offset, rxbytes - offset);

                    SerialPortHelper.Write(tcpdata, offset, rxbytes - offset);
                }

                if (rxbytes == 0)
                {
                    LogHelper.Log("TcpClient Client closed");
                    OnConnectClosed();
                }
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    LogHelper.Log("TcpClient Connection closed");
                }
                else if (e is IOException && e.Message.Contains("closed"))
                {
                    LogHelper.Log("TcpClient Connection closed");
                }
                else
                {
                    LogHelper.Log("TcpClient Exception: " + e.Message);
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
                LogHelper.Log("TcpClient close:" + ex.Message);
            }

            if (bAutoReconnect && bIsRunning)
            {
                try
                {
                    CreateConnect();
                }
                catch (Exception ex)
                {
                    LogHelper.Log("TcpClient Problem reconnecting:" + ex.Message);
                }
            }
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
                if (stream.CanWrite)
                {
                    LogHelper.Log("TcpClient Send: [" + LogHelper.Logdata(data, 0, len) + "]");
                    stream.Write(data, offset, len);
                }
            }
        }
    }
}