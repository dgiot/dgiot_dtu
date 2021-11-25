// <copyright file="MainForm.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Management;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private delegate void LogHandler(string text);

        private MqttClientHelper mqtt = MqttClientHelper.GetInstance();
        private TcpClientHelper tcpclient = TcpClientHelper.GetInstance();
        private UDPClientHelper udplient = UDPClientHelper.GetInstance();

        private SerialPortHelper serialport = SerialPortHelper.GetInstance();
        private TcpServerHelper tcpserver = TcpServerHelper.GetInstance();
        private PLCHelper plc = PLCHelper.GetInstance();
        private OPCDAHelper opcda = OPCDAHelper.GetInstance();
        private OPCUAHelper opcua = OPCUAHelper.GetInstance();
        private BACnetHelper bacnet = BACnetHelper.GetInstance();
        private AccessHelper access = AccessHelper.GetInstance();
        private SqlServerHelper sqlserver = SqlServerHelper.GetInstance();

        private static string clientid = Guid.NewGuid().ToString().Substring(0, 10);
        private static string productid = Guid.NewGuid().ToString().Substring(0, 10);
        private static string devaddr = Guid.NewGuid().ToString().Substring(0, 10);

        private bool bAutoReconnect = false;
        private bool bDisplayHex = false;
        private bool bIsRunning = false;
        private string[] bridges = new string[]
        {
            "SerialPort",
            "TcpServer",
            "BACnet",
            "OPCDA",
            "OPCUA",
            "MqttClient",
            "MqttServer",
            "TcpClient",
            "TcpServer",
            "UdpClient",
            "UdpServer",
            "PLC",
            "Control",
            "Access",
            "SqlServer",
        };

        private Configuration config;

        public MainForm()
        {
            InitializeComponent();

            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            Text += " v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            var arrPorts = SerialPortHelper.GetPorts();
            comboBoxSerialPort.Items.Clear();
            foreach (var port in arrPorts)
            {
                comboBoxSerialPort.Items.Add(port);
            }

            if (arrPorts.Length > 0)
            {
                comboBoxSerialPort.SelectedIndex = 0;
            }

            comboBoxBaudRate.SelectedIndex = 7;
            comboBoxDataBits.SelectedIndex = 0;
            comboBoxStopBits.SelectedIndex = 0;
            comboBoxParity.SelectedIndex = 0;
            comboBoxCmdProdxy.SelectedIndex = 0;

            comboBoxBridge.Items.Clear();

            foreach (var bridge in bridges)
            {
                comboBoxBridge.Items.Add(bridge);
            }

            if (bridges.Length > 0)
            {
                comboBoxBridge.SelectedIndex = 0;
            }

            bAutoReconnect = this.checkBoxReconnect.Checked;
            bDisplayHex = this.checkBoxDisplayHex.Checked;

            comboBoxDevAddr.Items.Clear();
            List<string> macAddrs = GetMacByWmi();
            foreach (var mac in macAddrs)
            {
                devaddr = Regex.Replace(mac, @":", "");
                comboBoxDevAddr.Items.Add(devaddr);
            }

            comboBoxDevAddr.SelectedIndex = 0;

            try
            {
                ResotreConfig(config);
                SaveAppConfig();
            }
            catch (Exception ex)
            {
                Log("read config exception: " + ex.Message);
            }
        }

        private void ToStop()
        {
            try
            {
                buttonStartStop.Text = @"Start";
                bIsRunning = false;
                MqttClientHelper.Stop();
                MqttServerHelper.Stop();
                SerialPortHelper.Stop();
                TcpClientHelper.Stop();
                TcpServerHelper.Stop();
                UDPClientHelper.Stop();
                UDPServerHelper.Stop();
                BACnetHelper.Stop();
            }
            catch (Exception e)
            {
                Log("stop server exception:" + e.Message);
                return;
            }
        }

        private void ButtonStartStopClick(object sender, EventArgs e)
        {
            if (!bIsRunning)
            {
                bIsRunning = true;
                try
                {
                    MqttServerHelper.Start(config.AppSettings.Settings, this);
                    MqttClientHelper.Start(config.AppSettings.Settings, bAutoReconnect, this);
                    TcpServerHelper.Start(config.AppSettings.Settings, this);
                    TcpClientHelper.Start(config.AppSettings.Settings, bAutoReconnect, this);
                    UDPServerHelper.Start(config.AppSettings.Settings, this);
                    UDPClientHelper.Start(config.AppSettings.Settings, bAutoReconnect, this);
                    OPCDAHelper.Start(config.AppSettings.Settings, this);
                    BACnetHelper.Start(config.AppSettings.Settings, this);
                }
                catch (Exception error)
                {
                    Log("star error " + error.ToString());
                    return;
                }

                buttonStartStop.Text = @"Stop";

                SaveAppConfig();
            }
            else
            {
                ToStop();
            }
        }

        public void Log(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new LogHandler(Log), new object[] { text });
                return;
            }

            // Truncate
            if (textBoxLog.Text.Length > 4096)
            {
                textBoxLog.Text = textBoxLog.Text.Substring(textBoxLog.Text.Length - 4096);
            }

            textBoxLog.Text += text + "\r\n";
            textBoxLog.SelectionStart = textBoxLog.Text.Length - 1;
            textBoxLog.ScrollToCaret();
        }

        public string Logdata(byte[] data, int offset, int len)
        {
            var line = bDisplayHex ? StringHelper.ToHexString(data, offset, len) : System.Text.Encoding.ASCII.GetString(data, offset, len);
            if (line.EndsWith("\r\n"))
            {
                line = line.Substring(0, line.Length - 2);
            }

            return line;
        }

        public byte[] Payload(char[] data)
        {
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(data);
            if (bDisplayHex)
            {
                byte[] hexPayload = StringHelper.ToHexBinary(payload);
                return hexPayload;
            }
            else
            {
                return payload;
            }
        }

        public byte[] Payload(byte[] payload)
        {
            if (bDisplayHex)
            {
                byte[] hexPayload = StringHelper.ToHexBinary(payload);
                return hexPayload;
            }
            else
            {
                return payload;
            }
        }

        private void ResotreConfig(Configuration config)
        {
            if (config.AppSettings.Settings["portName"] != null)
            {
                var tmp = config.AppSettings.Settings["portName"].Value;
                comboBoxSerialPort.SelectedIndex = comboBoxSerialPort.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["BaudRate"] != null)
            {
                var tmp = config.AppSettings.Settings["BaudRate"].Value;
                comboBoxBaudRate.SelectedIndex = comboBoxBaudRate.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["DataBits"] != null)
            {
                var tmp = config.AppSettings.Settings["DataBits"].Value;
                comboBoxDataBits.SelectedIndex = comboBoxDataBits.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["Parity"] != null)
            {
                var tmp = config.AppSettings.Settings["Parity"].Value;
                comboBoxParity.SelectedIndex = comboBoxParity.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["StopBits"] != null)
            {
                var tmp = config.AppSettings.Settings["StopBits"].Value;
                comboBoxStopBits.SelectedIndex = comboBoxStopBits.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["serialPortIsCheck"] != null)
            {
                comboBoxSerialPort.Text = config.AppSettings.Settings["serialPortIsCheck"].Value;
            }

            if (config.AppSettings.Settings["cmdProdxy"] != null)
            {
                comboBoxCmdProdxy.Text = config.AppSettings.Settings["cmdProdxy"].Value;
            }

            if (config.AppSettings.Settings["toPayload"] != null)
            {
                this.textToPayload.Text = config.AppSettings.Settings["toPayload"].Value;
            }

            if (config.AppSettings.Settings["mqttServer"] != null)
            {
                this.textBoxMqttSever.Text = config.AppSettings.Settings["mqttServer"].Value;
            }

            if (config.AppSettings.Settings["mqttPort"] != null)
            {
                this.textBoxMqttPort.Text = config.AppSettings.Settings["mqttPort"].Value;
            }

            if (config.AppSettings.Settings["mqttUserName"] != null)
            {
                this.textBoxMqttUserName.Text = config.AppSettings.Settings["mqttUserName"].Value;
            }

            if (config.AppSettings.Settings["devAddr"] != null)
            {
                var tmp = config.AppSettings.Settings["devAddr"].Value;
                comboBoxDevAddr.SelectedIndex = comboBoxDevAddr.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["mqttPassword"] != null)
            {
                this.textBoxMqttPassword.Text = config.AppSettings.Settings["mqttPassword"].Value;
            }

            if (config.AppSettings.Settings["mqttPubTopic"] != null)
            {
                this.textBoxMqttPubTopic.Text = config.AppSettings.Settings["mqttPubTopic"].Value;
            }

            if (config.AppSettings.Settings["tcpClientServer"] != null)
            {
                this.textBoxTcpClientServer.Text = config.AppSettings.Settings["tcpClientServer"].Value;
            }

            if (config.AppSettings.Settings["tcpClientPort"] != null)
            {
                this.textBoxTcpClientPort.Text = config.AppSettings.Settings["tcpClientPort"].Value;
            }

            if (config.AppSettings.Settings["tcpClientLogin"] != null)
            {
                this.textBoxTcpClientLogin.Text = config.AppSettings.Settings["tcpClientLogin"].Value;
            }

            if (config.AppSettings.Settings["UDPClientServer"] != null)
            {
                this.textBoxUDPClientServer.Text = config.AppSettings.Settings["UDPClientServer"].Value;
            }

            if (config.AppSettings.Settings["UDPClientPort"] != null)
            {
                this.textBoxUDPCLientPort.Text = config.AppSettings.Settings["UDPClientPort"].Value;
            }

            if (config.AppSettings.Settings["tcpServerPort"] != null)
            {
                this.textBoxTcpServerPort.Text = config.AppSettings.Settings["tcpServerPort"].Value;
            }

            if (config.AppSettings.Settings["tcpServerIsCheck"] != null)
            {
                this.checkBoxTcpServer.Checked = StringHelper.StrTobool(config.AppSettings.Settings["tcpServerIsCheck"].Value);
            }

            Resh_topic();

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            MqttServerHelper.Config(config.AppSettings.Settings, this);
            TcpClientHelper.Config(config.AppSettings.Settings, this);
            TcpServerHelper.Config(config.AppSettings.Settings, this);
            UDPClientHelper.Config(config.AppSettings.Settings, this);
            UDPServerHelper.Config(config.AppSettings.Settings, this);

            SerialPortHelper.Config(config.AppSettings.Settings, this);
            PLCHelper.Config(config.AppSettings.Settings, this);
            OPCDAHelper.Config(config.AppSettings.Settings, this);
            OPCUAHelper.Config(config.AppSettings.Settings, this);
            AccessHelper.Config(config.AppSettings.Settings, this);
            SqlServerHelper.Config(config.AppSettings.Settings, this);
        }

        private void Resh_topic()
        {
            devaddr = comboBoxDevAddr.Text;
            productid = textBoxMqttUserName.Text;
            clientid = Md5("Device" + this.textBoxMqttUserName.Text + devaddr).Substring(0, 10);
            textBoxMqttClientId.Text = clientid;
            textBoxMqttSubTopic.Text = "/" + productid + "/" + devaddr;
            textBoxMqttPubTopic.Text = "/" + productid + "/" + devaddr + "/properties/read/reply";
            textBoxAccessTopic.Text = "/" + productid + "/" + devaddr + "/child/mdb";
            textBoxOPCDATopic.Text = "/" + productid + "/" + devaddr + "/child/opcda";
            textBoxOPCUATopic.Text = "/" + productid + "/" + devaddr + "/child/opcua";
            textBoxPLCTopic.Text = "/" + productid + "/" + devaddr + "/child/plc";
            textBoxBACnetTopic.Text = "/" + productid + "/" + devaddr + "/child/bacnet";
            textBoxControlTopic.Text = "/" + productid + "/" + devaddr + "/child/control";
            textBoxSqlServerTopic.Text = "/" + productid + "/" + devaddr + "/child/sqlserver";
        }

        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/dgiot/dgiot_dtu");
        }

        private void ButtonClearClick(object sender, EventArgs e)
        {
            textBoxLog.Text = "";
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/dgiot/dgiot_dtu");
        }

        /// <summary>
        /// 通过WMI读取系统信息里的网卡MAC
        /// </summary>
        /// <returns> </returns>
        public static List<string> GetMacByWmi()
        {
            try
            {
                List<string> macs = new List<string>();
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        var mac = mo["MacAddress"].ToString();
                        macs.Add(mac);
                    }
                }

                return macs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Md5(string str)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(str);
                bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
                }

                str = sTemp.ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return str;
        }

        private void SaveAppConfig()
        {
            if (config.AppSettings.Settings["tcpServerIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerIsCheck", StringHelper.BoolTostr(this.checkBoxTcpServer.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxTcpServer.Checked);
            }

            if (config.AppSettings.Settings["serialPortIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("serialPortIsCheck", StringHelper.BoolTostr(this.checkBoxSerialPort.Checked));
            }
            else
            {
                config.AppSettings.Settings["serialPortIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSerialPort.Checked);
            }

            if (config.AppSettings.Settings["PLCIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("PLCIsCheck", StringHelper.BoolTostr(this.checkBoxPLC.Checked));
            }
            else
            {
                config.AppSettings.Settings["PLCIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxPLC.Checked);
            }

            if (config.AppSettings.Settings["PLCTopic"] == null)
            {
                config.AppSettings.Settings.Add("PLCTopic", textBoxPLCTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["PLCTopic"].Value = textBoxPLCTopic.Text;
            }

            if (config.AppSettings.Settings["OPCDAIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("OPCDAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCDA.Checked));
            }
            else
            {
                config.AppSettings.Settings["OPCDAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCDA.Checked);
            }

            if (config.AppSettings.Settings["OPCDATopic"] == null)
            {
                config.AppSettings.Settings.Add("OPCDATopic", textBoxOPCDATopic.Text);
            }
            else
            {
                config.AppSettings.Settings["OPCDATopic"].Value = textBoxOPCDATopic.Text;
            }

            if (config.AppSettings.Settings["OPCUAIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("OPCUAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCUA.Checked));
            }
            else
            {
                config.AppSettings.Settings["OPCUAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCUA.Checked);
            }

            if (config.AppSettings.Settings["OPCUATopic"] == null)
            {
                config.AppSettings.Settings.Add("OPCUATopic", textBoxOPCUATopic.Text);
            }
            else
            {
                config.AppSettings.Settings["OPCUATopic"].Value = textBoxOPCUATopic.Text;
            }

            if (config.AppSettings.Settings["BACnetIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("BACnetIsCheck", StringHelper.BoolTostr(this.checkBoxBACnet.Checked));
            }
            else
            {
                config.AppSettings.Settings["BACnetIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxBACnet.Checked);
            }

            if (config.AppSettings.Settings["BACnetTopic"] == null)
            {
                config.AppSettings.Settings.Add("BACnetTopic", textBoxBACnetTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["BACnetTopic"].Value = textBoxBACnetTopic.Text;
            }

            if (config.AppSettings.Settings["ControlIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("ControlIsCheck", StringHelper.BoolTostr(this.checkBoxControl.Checked));
            }
            else
            {
                config.AppSettings.Settings["ControlIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxControl.Checked);
            }

            if (config.AppSettings.Settings["ControlTopic"] == null)
            {
                config.AppSettings.Settings.Add("ControlTopic", textBoxControlTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["ControlTopic"].Value = textBoxControlTopic.Text;
            }

            if (config.AppSettings.Settings["AccessIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("AccessIsCheck", StringHelper.BoolTostr(this.checkBoxAccess.Checked));
            }
            else
            {
                config.AppSettings.Settings["AccessIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxAccess.Checked);
            }

            if (config.AppSettings.Settings["AccessTopic"] == null)
            {
                config.AppSettings.Settings.Add("AccessTopic", textBoxAccessTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["AccessTopic"].Value = textBoxAccessTopic.Text;
            }

            if (config.AppSettings.Settings["SqlServerIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("SqlServerIsCheck", StringHelper.BoolTostr(this.checkBoxSqlServer.Checked));
            }
            else
            {
                config.AppSettings.Settings["SqlServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSqlServer.Checked);
            }

            if (config.AppSettings.Settings["SqlServerTopic"] == null)
            {
                config.AppSettings.Settings.Add("SqlServerTopic", textBoxSqlServerTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["SqlServerTopic"].Value = textBoxSqlServerTopic.Text;
            }

            if (config.AppSettings.Settings["mqttIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("mqttIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["mqttIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
            }

            if (config.AppSettings.Settings["tcpClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

            if (config.AppSettings.Settings["UDPClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["UDPClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

            if (config.AppSettings.Settings["toPayload"] == null)
            {
                config.AppSettings.Settings.Add("toPayload", this.textToPayload.Text);
            }
            else
            {
                config.AppSettings.Settings["toPayload"].Value = this.textToPayload.Text;
            }

            if (config.AppSettings.Settings["mqttServer"] == null)
            {
                config.AppSettings.Settings.Add("mqttServer", this.textBoxMqttSever.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttServer"].Value = this.textBoxMqttSever.Text;
            }

            if (config.AppSettings.Settings["mqttPort"] == null)
            {
                config.AppSettings.Settings.Add("mqttPort", this.textBoxMqttPort.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPort"].Value = this.textBoxMqttPort.Text;
            }

            if (config.AppSettings.Settings["mqttClientId"] == null)
            {
                config.AppSettings.Settings.Add("mqttClientId", this.textBoxMqttClientId.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttClientId"].Value = this.textBoxMqttClientId.Text;
            }

            if (config.AppSettings.Settings["devAddr"] == null)
            {
                config.AppSettings.Settings.Add("devAddr", this.comboBoxDevAddr.Text);
            }
            else
            {
                config.AppSettings.Settings["devAddr"].Value = this.comboBoxDevAddr.Text;
            }

            if (config.AppSettings.Settings["mqttUserName"] == null)
            {
                config.AppSettings.Settings.Add("mqttUserName", this.textBoxMqttUserName.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttUserName"].Value = this.textBoxMqttUserName.Text;
            }

            if (config.AppSettings.Settings["mqttPassword"] == null)
            {
                config.AppSettings.Settings.Add("mqttPassword", this.textBoxMqttPassword.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPassword"].Value = this.textBoxMqttPassword.Text;
            }

            if (config.AppSettings.Settings["mqttSubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttSubTopic", this.textBoxMqttSubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttSubTopic"].Value = this.textBoxMqttSubTopic.Text;
            }

            if (config.AppSettings.Settings["mqttPubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttPubTopic", this.textBoxMqttPubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPubTopic"].Value = this.textBoxMqttPubTopic.Text;
            }

            if (config.AppSettings.Settings["tcpClientServer"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientServer", this.textBoxTcpClientServer.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientServer"].Value = this.textBoxTcpClientServer.Text;
            }

            if (config.AppSettings.Settings["tcpClientPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientPort", this.textBoxTcpClientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientPort"].Value = this.textBoxTcpClientPort.Text;
            }

            if (config.AppSettings.Settings["tcpClientLogin"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientLogin", this.textBoxTcpClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientLogin"].Value = this.textBoxTcpClientLogin.Text;
            }

            if (config.AppSettings.Settings["UDPClientServer"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientServer", this.textBoxUDPClientServer.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientServer"].Value = this.textBoxUDPClientServer.Text;
            }

            if (config.AppSettings.Settings["UDPClientPort"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientPort", this.textBoxUDPCLientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientPort"].Value = this.textBoxUDPCLientPort.Text;
            }

            if (config.AppSettings.Settings["UDPClientLogin"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientLogin", this.textBoxUDPClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientLogin"].Value = this.textBoxUDPClientLogin.Text;
            }

            if (config.AppSettings.Settings["tcpServerPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerPort", this.textBoxTcpServerPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpServerPort"].Value = this.textBoxTcpServerPort.Text;
            }

            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void CheckBoxReconnectCheckedChanged(object sender, EventArgs e)
        {
            bAutoReconnect = checkBoxReconnect.Checked;
        }

        private void CheckBoxDisplayHexCheckedChanged(object sender, EventArgs e)
        {
            bDisplayHex = checkBoxDisplayHex.Checked;
        }

        private void CheckBoxTcpServer_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpServerIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerIsCheck", StringHelper.BoolTostr(this.checkBoxTcpServer.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxTcpServer.Checked);
            }

            TcpServerHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxSerialPort_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["serialPortIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("serialPortIsCheck", StringHelper.BoolTostr(this.checkBoxSerialPort.Checked));
            }
            else
            {
                config.AppSettings.Settings["serialPortIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSerialPort.Checked);
            }

            SerialPortHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxPLC_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["PLCIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("PLCIsCheck", StringHelper.BoolTostr(this.checkBoxPLC.Checked));
            }
            else
            {
                config.AppSettings.Settings["PLCIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxPLC.Checked);
            }

            PLCHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxOPCDA_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["OPCDAIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("OPCDAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCDA.Checked));
            }
            else
            {
                config.AppSettings.Settings["OPCDAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCDA.Checked);
            }

            OPCDAHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxOPCUA_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["OPCUAIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("OPCUAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCUA.Checked));
            }
            else
            {
                config.AppSettings.Settings["OPCUAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCUA.Checked);
            }

            OPCUAHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxBAnet_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["BACnetIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("BACnetIsCheck", StringHelper.BoolTostr(this.checkBoxBACnet.Checked));
            }
            else
            {
                config.AppSettings.Settings["BACnetIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxBACnet.Checked);
            }

            BACnetHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxControl_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["ControlIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("ControlIsCheck", StringHelper.BoolTostr(this.checkBoxControl.Checked));
            }
            else
            {
                config.AppSettings.Settings["ControlIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxControl.Checked);
            }

            ControlHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxSqlServer_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["SqlServerIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("SqlServerIsCheck", StringHelper.BoolTostr(this.checkBoxSqlServer.Checked));
            }
            else
            {
                config.AppSettings.Settings["SqlServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSqlServer.Checked);
            }

            SqlServerHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxAccess_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["AccessIsCheck"] == null)
            {
               config.AppSettings.Settings.Add("AccessIsCheck", StringHelper.BoolTostr(this.checkBoxAccess.Checked));
            }
            else
            {
                config.AppSettings.Settings["AccessIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxAccess.Checked);
            }

            AccessHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxMqttBridge_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttbridgeIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("mqttbridgeIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["mqttbridgeIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
            }

            MqttServerHelper.Config(config.AppSettings.Settings, this);
        }

        private void CheckBoxUdpServer_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["udpbridgeIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("udpbridgeIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["udpbridgeIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
            }

            UDPClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void RadioButtonMqttClient_CheckedChanged(object sender, EventArgs e)
        {
           if (config.AppSettings.Settings["mqttIsCheck"] == null)
           {
              config.AppSettings.Settings.Add("mqttIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
           }
           else
           {
              config.AppSettings.Settings["mqttIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
           }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void RadioButtonTcpClient_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

           TcpClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void RadioButtonUDPClient_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["UDPClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["UDPClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

            UDPClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextToPayload_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["toPayload"] == null)
            {
                config.AppSettings.Settings.Add("toPayload", this.textToPayload.Text);
            }
            else
            {
                config.AppSettings.Settings["toPayload"].Value = this.textToPayload.Text;
            }
        }

        private void TextBoxMqttSever_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttServer"] == null)
            {
                config.AppSettings.Settings.Add("mqttServer", this.textBoxMqttSever.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttServer"].Value = this.textBoxMqttSever.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxMqttPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttPort"] == null)
            {
                config.AppSettings.Settings.Add("mqttPort", this.textBoxMqttPort.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPort"].Value = this.textBoxMqttPort.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxMqttClientId_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttClientId"] == null)
            {
                config.AppSettings.Settings.Add("mqttClientId", this.textBoxMqttClientId.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttClientId"].Value = this.textBoxMqttClientId.Text;
            }

            Resh_topic();

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxMqttUserName_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttUserName"] == null)
            {
                config.AppSettings.Settings.Add("mqttUserName", this.textBoxMqttUserName.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttUserName"].Value = this.textBoxMqttUserName.Text;
            }

            Resh_topic();

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxMqttPassword_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttPassword"] == null)
            {
                config.AppSettings.Settings.Add("mqttPassword", this.textBoxMqttPassword.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPassword"].Value = this.textBoxMqttPassword.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxMqttSubTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttSubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttSubTopic", this.textBoxMqttSubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttSubTopic"].Value = this.textBoxMqttSubTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxMqttPubTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttPubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttPubTopic", this.textBoxMqttPubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPubTopic"].Value = this.textBoxMqttPubTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxMqttServerPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttServerPort"] == null)
            {
                config.AppSettings.Settings.Add("mqttServerPort", this.textBoxTcpClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttServerPort"].Value = this.textBoxTcpClientLogin.Text;
            }

            MqttServerHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxTcpClientServer_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientServer"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientServer", this.textBoxTcpClientServer.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientServer"].Value = this.textBoxTcpClientServer.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxTcpClientPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientPort", this.textBoxTcpClientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientPort"].Value = this.textBoxTcpClientPort.Text;
            }

            TcpClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxTcpClientLogin_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientLogin"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientLogin", this.textBoxTcpClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientLogin"].Value = this.textBoxTcpClientLogin.Text;
            }

            TcpClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxTcpServerPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpServerPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerPort", this.textBoxTcpServerPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpServerPort"].Value = this.textBoxTcpServerPort.Text;
            }

            TcpServerHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxUDPClientServer_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["UDPClientServer"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientServer", this.textBoxUDPClientServer.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientServer"].Value = this.textBoxUDPClientServer.Text;
            }

            UDPClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxUDPCLientPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["UDPClientPort"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientPort", this.textBoxUDPCLientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientPort"].Value = this.textBoxUDPCLientPort.Text;
            }

            UDPClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxUDPClientLogin_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["UDPClientLogin"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientLogin", this.textBoxUDPClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientLogin"].Value = this.textBoxUDPClientLogin.Text;
            }

            UDPClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void TextBoxUdpServerPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["udpServerPort"] == null)
            {
                config.AppSettings.Settings.Add("udpServerPort", this.textBoxTcpServerPort.Text);
            }
            else
            {
                config.AppSettings.Settings["udpServerPort"].Value = this.textBoxTcpServerPort.Text;
            }

            UDPClientHelper.Config(config.AppSettings.Settings, this);
        }

        private void SendBridge_Click(object sender, EventArgs e)
        {
            byte[] payload = Payload(config.AppSettings.Settings["toPayload"].Value.ToCharArray());
            Log(bridges[comboBoxBridge.SelectedIndex] + " send  [" + Logdata(payload, 0, payload.Length) + "]");

            if (bridges[comboBoxBridge.SelectedIndex] == "SerialPort")
            {
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "TcpServer")
            {
                TcpServerHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "PLC")
            {
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "OPCDA")
            {
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "OPCUA")
            {
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "BACnet")
            {
                BACnetHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "Control")
            {
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "Access")
            {
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "SqlServer")
            {
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "MqttClient")
            {
                MqttClientHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "MqttServer")
            {
                MqttServerHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "TcpClient")
            {
                TcpClientHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "UdpClient")
            {
            }
            else
            {
            }
        }

        private void ComboBoxCmdProdxy_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBoxStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBoxParity_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBoxDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBoxDevAddr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["devAddr"] != null)
            {
               config.AppSettings.Settings["devAddr"].Value = this.comboBoxDevAddr.Text;
            }
            else
            {
                config.AppSettings.Settings.Add("devAddr", this.comboBoxDevAddr.Text);
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }

        private void TextBoxPLCTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["PLCTopic"] == null)
            {
                config.AppSettings.Settings.Add("PLCTopic", textBoxPLCTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["PLCTopic"].Value = textBoxPLCTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }

        private void TextBoxOPCDATopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["OPCDATopic"] == null)
            {
                config.AppSettings.Settings.Add("OPCDATopic", textBoxOPCDATopic.Text);
            }
            else
            {
                config.AppSettings.Settings["OPCDATopic"].Value = textBoxOPCDATopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }

        private void TextBoxOPCUATopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["OPCUATopic"] == null)
            {
                config.AppSettings.Settings.Add("OPCUATopic", textBoxOPCUATopic.Text);
            }
            else
            {
                config.AppSettings.Settings["OPCUATopic"].Value = textBoxOPCUATopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }

        private void TextBoxBACnetTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["BACnetTopic"] == null)
            {
                config.AppSettings.Settings.Add("BACnetTopic", textBoxBACnetTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["BACnetTopic"].Value = textBoxBACnetTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }

        private void TextBoxControlTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["ControlTopic"] == null)
            {
                config.AppSettings.Settings.Add("ControlTopic", textBoxControlTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["ControlTopic"].Value = textBoxControlTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }

        private void TextBoxAccessTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["AccessTopic"] == null)
            {
                config.AppSettings.Settings.Add("AccessTopic", textBoxAccessTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["AccessTopic"].Value = textBoxAccessTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }

        private void TextBoxSqlServerTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["SqlServerTopic"] == null)
            {
                config.AppSettings.Settings.Add("SqlServerTopic", textBoxSqlServerTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["SqlServerTopic"].Value = textBoxSqlServerTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings, this);
            Resh_topic();
        }
    }
}
