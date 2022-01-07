// <copyright file="SerialPortHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.IO.Ports;

    public class SerialPortHelper
    {
        private SerialPortHelper()
        {
        }

        private static SerialPort port = null;
        private static SerialPortHelper instance = null;
        private static bool bIsRunning = false;
        private static string portName;
        private static int baudRate;
        private static Parity parity;
        private static int dataBits;
        private static StopBits stopBits;

        public static SerialPortHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new SerialPortHelper();
            }

            return instance;
        }

        public static void Start()
        {
            Config();

            if (bIsRunning)
            {
                Stop();
            }

            try
            {
                port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                port.DataReceived += Received;
                port.ReceivedBytesThreshold = 1;
                port.Open();
                LogHelper.Log(@"Open open port " + portName);
            }
            catch (Exception)
            {
                LogHelper.Log(@"Couldn't open port " + portName);
                return;
            }

            bIsRunning = true;
        }

        public static void Stop()
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    bIsRunning = false;
                    port.Close();
                }
            }
        }

        public static void Config()
        {
            portName = ConfigHelper.GetConfig("SerialPort");
            baudRate = int.Parse(ConfigHelper.GetConfig("BaudRate"));
            dataBits = int.Parse(ConfigHelper.GetConfig("DataBits"));
            parity = StrToParity(ConfigHelper.GetConfig("Parity"));
            stopBits = StrToStopBits(ConfigHelper.GetConfig("StopBits"));
        }

        public static void Write(byte[] payload, int offset, int len)
        {
            if (port != null && port.IsOpen)
            {
                // LogHelper.Log("SerialPort Send: [" + LogHelper.Logdata(payload, 0, len) + "]");
                port.Write(payload, offset, len);
            }
        }

        private static void Received(object sender, SerialDataReceivedEventArgs e)
        {
            var rxlen = port.BytesToRead;
            var data = new byte[rxlen];
            port.Read(data, 0, rxlen);
            LogHelper.Log("SerialPort Recv: [" + LogHelper.Logdata(data, 0, rxlen) + "]");
            TcpClientHelper.Write(data, 0, rxlen);
        }

        private static StopBits StrToStopBits(string s)
        {
            if (s == "1")
            {
                return StopBits.One;
            }

            if (s == "2")
            {
                return StopBits.Two;
            }

            if (s == "1.5")
            {
                return StopBits.OnePointFive;
            }

            return StopBits.None;
        }

        private static Parity StrToParity(string s)
        {
            if (s == "None")
            {
                return Parity.None;
            }

            if (s == "Odd ")
            {
                return Parity.Odd;
            }

            if (s == "Even")
            {
                return Parity.Even;
            }

            if (s == "Mark")
            {
                return Parity.Mark;
            }

            if (s == "Space")
            {
                return Parity.Space;
            }

            return Parity.None;
        }

        public static string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}