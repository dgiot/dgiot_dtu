// <copyright file="TcpServerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    public class TcpServerHelper
    {
        private TcpServerHelper()
        {
        }

        private static TcpListener server;
        private static TcpClient client;
        private static NetworkStream stream;
        private static byte[] tcpdata = new byte[1024];
        private static TcpServerHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
        private static int port;

        public static TcpServerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new TcpServerHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, MainForm mainform)
        {
            bIsRunning = true;
            Config(config, mainform);
            if (bIsCheck)
            {
                CreateConnect();
            }
        }

        public static void Stop()
        {
            if (server != null)
            {
                TcpServerHelper.bIsRunning = false;
                server.Stop();
                server = null;
            }
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["tcpServerIsCheck"] != null)
            {
                TcpServerHelper.bIsCheck = StringHelper.StrTobool(config["tcpServerIsCheck"].Value);
            }

            if (config["tcpServerPort"] != null)
            {
                TcpServerHelper.port = int.Parse((string)config["tcpServerPort"].Value);
            }

            TcpServerHelper.mainform = mainform;
        }

        public static void CreateConnect()
        {
            server = new TcpListener(IPAddress.Any, TcpServerHelper.port);
            server.Start();
            server.BeginAcceptTcpClient(Connected, null);
        }

        private static void Connected(IAsyncResult result)
        {
            try
            {
                if (!bIsRunning)
                {
                    mainform.Log("TcpConnectedIn: server shutdown");
                    goto end;
                }

                TcpClient tmp_client = server.EndAcceptTcpClient(result);
                NetworkStream tmp_stream = tmp_client.GetStream();

                if (client != null)
                {
                    mainform.Log("tcpServer Already in use, close connected from: " + tmp_client.Client.RemoteEndPoint);
                    byte[] reject = System.Text.Encoding.ASCII.GetBytes("Already in use!\r\n");
                    tmp_stream.Write(reject, 0, reject.Length);
                    tmp_stream.Close();
                    tmp_client.Close();
                    goto end;
                }

                client = tmp_client;
                stream = tmp_stream;

                mainform.Log("tcpServer Client Connected: " + client.Client.LocalEndPoint + "<==>" + client.Client.RemoteEndPoint);
                stream.BeginRead(tcpdata, 0, tcpdata.Length, Read, null);

                bIsRunning = true;
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    mainform.Log("TCP Server Connection shutdown");
                }
                else
                {
                    mainform.Log("TCP Server Connection exception: " + e.Message);
                }
            }

        end:
            try
            {
                if (bIsRunning)
                {
                    /* accept other connection again */
                    server.BeginAcceptTcpClient(Connected, null);
                }
            }
            catch (Exception e)
            {
                mainform.Log("TCP Server exception: " + e.Message);
            }
        }

        private static void Read(IAsyncResult ar)
        {
            try
            {
                var rxbytes = stream.EndRead(ar);
                if (rxbytes > 0)
                {
                    var offset = 0;
                    stream.BeginRead(tcpdata, 0, tcpdata.Length, Read, null);
                    mainform.Log("N->S: tcpServer recv [ " + mainform.Logdata(tcpdata, offset, rxbytes - offset) + "]");
                    TcpClientHelper.Write(tcpdata, offset, rxbytes - offset);

                    // Write(tcpdata, offset, rxbytes - offset);
                }

                if (rxbytes == 0)
                {
                    mainform.Log("Client closed");
                    OnConnectClosed();
                }
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    mainform.Log("Connection closed");
                }
                else if (e is IOException && e.Message.Contains("closed"))
                {
                    mainform.Log("Connection closed");
                }
                else
                {
                    mainform.Log("Exception: " + e.Message);
                }

                OnConnectClosed();
            }
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
                if (stream != null)
                {
                    stream.Write(data, offset, len);
                }
            }
        }

        private static void OnConnectClosed()
        {
            try
            {
                client.Close();
                client = null;
            }
            catch
            {
            }
        }
    }
}