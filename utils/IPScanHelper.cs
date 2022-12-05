// <copyright file="DgiotHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    public class IPScanHelper
    {
        private static IPScanHelper instance = null;
        private IPScanHelper()
        {
        }
        public static IPScanHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new IPScanHelper();
            }

            return instance;
        }

        public static void IPScan()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> localIps = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    using (Ping p = new Ping())
                    {
                        PingReply pingReply = p.Send(ip, 100);
                        if (pingReply.Status == IPStatus.Success)
                        {
                            PingIP(ip);
                        }
                    }
                }
            }
        }


        private static void PingIP(IPAddress ipa)
        {
            byte[] ipByte = ipa.GetAddressBytes();
            string ipComm = ipByte[0] + "." + ipByte[1] + "." + ipByte[2] + ".";
            string pingIP = "";

            for (int lastByte = 0; lastByte <= 255; lastByte++)
            {
                Ping ping = new Ping();
                ping.PingCompleted += new PingCompletedEventHandler(PingComplete);
                pingIP = ipComm + lastByte;
                ping.SendAsync(pingIP, 2000, null);
            }
        }

        [DllImport(@"iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);
        private static void PingComplete(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                IPAddress ip = e.Reply.Address;
                // this.listBox.Items.Add("IP:" + ip.ToString());
                byte[] b = new byte[6];
                int len = b.Length;
                int r = SendARP(BitConverter.ToInt32(ip.GetAddressBytes(), 0), 0, b, ref len);
                string mac = BitConverter.ToString(b, 0, 6);
                // this.listBox.Items.Add("Mac:" + mac);
                LogHelper.Log(mac + "    " + ip.ToString());
            }
        }

    }
}
