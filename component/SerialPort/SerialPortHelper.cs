// <copyright file="SerialPortHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.IO.Ports;

    public class SerialPortHelper
    {
        private SerialPortHelper()
        {
        }

        private static SerialPort port = null;
        private static SerialPortHelper instance = null;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
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

        public static void Start(KeyValueConfigurationCollection config)
        {
            Config(config);
            bIsRunning = true;
            if (!bIsRunning)
            {
                try
                {
                    port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                    port.DataReceived += Received;
                    port.ReceivedBytesThreshold = 1;
                    port.Open();
                }
                catch (Exception)
                {
                    LogHelper.Log(@"Couldn't open port " + portName);
                    return;
                }

                bIsRunning = true;
            }
        }

        public static void Stop()
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["portName"] != null)
            {
                portName = (string)config["portName"].Value;
            }

            if (config["BaudRate"] != null)
            {
                baudRate = int.Parse((string)config["BaudRate"].Value);
            }

            if (config["DataBits"] != null)
            {
                dataBits = int.Parse((string)config["DataBits"].Value);
            }

            if (config["Parity"] != null)
            {
                parity = (Parity)StrToParity(config["Parity"].Value);
            }

            if (config["StopBits"] != null)
            {
                stopBits = StrToStopBits(config["StopBits"].Value);
            }

            if (config["SerialPortIsCheck"] != null)
            {
                bIsCheck = DgiotHelper.StrTobool(config["SerialPortIsCheck"].Value);
            }
        }

        public static void Write(byte[] payload, int offset, int len)
        {
            if (port != null && port.IsOpen)
            {
                LogHelper.Log("S->N: " + LogHelper.Logdata(payload, 0, payload.Length));
                port.Write(payload, offset, len);
            }
        }

        private static void Received(object sender, SerialDataReceivedEventArgs e)
        {
            var rxlen = port.BytesToRead;
            var data = new byte[rxlen];
            port.Read(data, 0, rxlen);
            LogHelper.Log("S->N: " + LogHelper.Logdata(data, 0, rxlen));
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