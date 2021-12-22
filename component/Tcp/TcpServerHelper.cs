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

        public static void Start()
        {
            bIsRunning = true;
            Config();
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

        public static void Config()
        {
            if (DgiotHelper.StrTobool(ConfigHelper.GetConfig("TcpClient_Checked")) && DgiotHelper.StrTobool(ConfigHelper.GetConfig("Bridge_Checked")))
            {
                bIsCheck = true;
            }
            else
            {
                bIsCheck = false;
            }

            port = int.Parse(ConfigHelper.GetConfig("BridgePort"));
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
                    LogHelper.Log("TcpConnectedIn: server shutdown");
                    goto end;
                }

                TcpClient tmp_client = server.EndAcceptTcpClient(result);
                NetworkStream tmp_stream = tmp_client.GetStream();

                if (client != null)
                {
                    LogHelper.Log("tcpServer Already in use, close connected from: " + tmp_client.Client.RemoteEndPoint);
                    byte[] reject = System.Text.Encoding.ASCII.GetBytes("Already in use!\r\n");
                    tmp_stream.Write(reject, 0, reject.Length);
                    tmp_stream.Close();
                    tmp_client.Close();
                    goto end;
                }

                client = tmp_client;
                stream = tmp_stream;

                LogHelper.Log("tcpServer Client Connected: " + client.Client.LocalEndPoint + "<==>" + client.Client.RemoteEndPoint);
                stream.BeginRead(tcpdata, 0, tcpdata.Length, Read, null);

                bIsRunning = true;
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                {
                    LogHelper.Log("TCP Server Connection shutdown");
                }
                else
                {
                    LogHelper.Log("TCP Server Connection exception: " + e.Message);
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
                LogHelper.Log("TCP Server exception: " + e.Message);
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
                    LogHelper.Log("N->S: tcpServer recv [ " + LogHelper.Logdata(tcpdata, offset, rxbytes - offset) + "]");
                    TcpClientHelper.Write(tcpdata, offset, rxbytes - offset);

                    // Write(tcpdata, offset, rxbytes - offset);
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