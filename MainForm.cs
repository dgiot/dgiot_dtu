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

        private LogHelper log = LogHelper.GetInstance();
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
        private bool bIsRunning = false;
        private string[] bridges = new string[]
        {
            "SerialPort",
            "TcpServer",
            "BACnet",
            "OPCDA_SCAN",
            "OPCDA_READ",
            "OPCDA_WRITE",
            "OPCUA_SCAN",
            "OPCUA_READ",
            "OPCUA_WRITE",
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

            LogHelper.Init(this);

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

            comboBoxDevAddr.Items.Clear();
            List<string> macAddrs = DgiotHelper.GetMacByWmi();
            foreach (var mac in macAddrs)
            {
                devaddr = Regex.Replace(mac, @":", "");
                comboBoxDevAddr.Items.Add(devaddr);
            }

            comboBoxDevAddr.SelectedIndex = 0;

            List<string> opcservers = OPCDAHelper.GetServer();
            foreach (var opcserver in opcservers)
            {
                comboBoxOpcServer.Items.Add(opcserver);
            }

            if (opcservers.Count > 0)
            {
                comboBoxOpcServer.SelectedIndex = 0;
            }

            List<string> loglevels = LogHelper.Levels();
            foreach (var level in loglevels)
            {
                comboBoxLogLevel.Items.Add(level);
            }

            if (loglevels.Count > 0)
            {
                comboBoxLogLevel.SelectedIndex = (int)LogHelper.Level.DEBUG;
            }

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
                    MqttServerHelper.Start(config.AppSettings.Settings);
                    MqttClientHelper.Start(config.AppSettings.Settings, bAutoReconnect);
                    TcpServerHelper.Start(config.AppSettings.Settings);
                    TcpClientHelper.Start(config.AppSettings.Settings, bAutoReconnect);
                    UDPServerHelper.Start(config.AppSettings.Settings);
                    UDPClientHelper.Start(config.AppSettings.Settings, bAutoReconnect);
                    OPCDAHelper.Start(config.AppSettings.Settings);
                    BACnetHelper.Start(config.AppSettings.Settings);
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
                textBoxMqttSever.Text = config.AppSettings.Settings["mqttServer"].Value;
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
                this.checkBoxTcpServer.Checked = DgiotHelper.StrTobool(config.AppSettings.Settings["tcpServerIsCheck"].Value);
            }

            Resh_topic();

            MqttClientHelper.Config(config.AppSettings.Settings);
            MqttServerHelper.Config(config.AppSettings.Settings);
            TcpClientHelper.Config(config.AppSettings.Settings);
            TcpServerHelper.Config(config.AppSettings.Settings);
            UDPClientHelper.Config(config.AppSettings.Settings);
            UDPServerHelper.Config(config.AppSettings.Settings);

            SerialPortHelper.Config(config.AppSettings.Settings);
            PLCHelper.Config(config.AppSettings.Settings);
            OPCDAHelper.Config(config.AppSettings.Settings);
            OPCUAHelper.Config(config.AppSettings.Settings);
            AccessHelper.Config(config.AppSettings.Settings);
            SqlServerHelper.Config(config.AppSettings.Settings);
        }

        private void Resh_topic()
        {
            devaddr = comboBoxDevAddr.Text;
            productid = textBoxMqttUserName.Text;
            clientid = DgiotHelper.Md5("Device" + this.textBoxMqttUserName.Text + devaddr).Substring(0, 10);
            textBoxMqttClientId.Text = clientid;
            textBoxMqttSubTopic.Text = "/" + productid + "/" + devaddr;
            textBoxMqttPubTopic.Text = "/" + productid + "/" + devaddr + "/properties/read/reply";
            textBoxAccessTopic.Text = "/" + productid + "/" + devaddr + "/child/mdb";

            // textBoxOPCDATopic.Text = "/" + productid + "/" + devaddr + "/child/opcda";
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

        private void SaveAppConfig()
        {
            if (config.AppSettings.Settings["tcpServerIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerIsCheck", DgiotHelper.BoolTostr(this.checkBoxTcpServer.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpServerIsCheck"].Value = DgiotHelper.BoolTostr(this.checkBoxTcpServer.Checked);
            }

            if (config.AppSettings.Settings["PLCTopic"] == null)
            {
                config.AppSettings.Settings.Add("PLCTopic", textBoxPLCTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["PLCTopic"].Value = textBoxPLCTopic.Text;
            }

            if (config.AppSettings.Settings["OpcServer"] == null)
            {
                config.AppSettings.Settings.Add("OpcServer", textBoxOpcServer.Text);
            }
            else
            {
                config.AppSettings.Settings["OpcServer"].Value = textBoxOpcServer.Text;
            }

            if (config.AppSettings.Settings["OPCUATopic"] == null)
            {
                config.AppSettings.Settings.Add("OPCUATopic", textBoxOPCUATopic.Text);
            }
            else
            {
                config.AppSettings.Settings["OPCUATopic"].Value = textBoxOPCUATopic.Text;
            }

            if (config.AppSettings.Settings["BACnetTopic"] == null)
            {
                config.AppSettings.Settings.Add("BACnetTopic", textBoxBACnetTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["BACnetTopic"].Value = textBoxBACnetTopic.Text;
            }

            if (config.AppSettings.Settings["ControlTopic"] == null)
            {
                config.AppSettings.Settings.Add("ControlTopic", textBoxControlTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["ControlTopic"].Value = textBoxControlTopic.Text;
            }

            if (config.AppSettings.Settings["AccessTopic"] == null)
            {
                config.AppSettings.Settings.Add("AccessTopic", textBoxAccessTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["AccessTopic"].Value = textBoxAccessTopic.Text;
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
                config.AppSettings.Settings.Add("mqttIsCheck", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["mqttIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonMqttClient.Checked);
            }

            if (config.AppSettings.Settings["tcpClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientIsCheck", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpClientIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonTcpClient.Checked);
            }

            if (config.AppSettings.Settings["UDPClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientIsCheck", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["UDPClientIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonTcpClient.Checked);
            }

            if (config.AppSettings.Settings["toPayload"] == null)
            {
                config.AppSettings.Settings.Add("toPayload", textToPayload.Text);
            }
            else
            {
                config.AppSettings.Settings["toPayload"].Value = textToPayload.Text;
            }

            if (config.AppSettings.Settings["mqttServer"] == null)
            {
                config.AppSettings.Settings.Add("mqttServer", textBoxMqttSever.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttServer"].Value = textBoxMqttSever.Text;
            }

            if (config.AppSettings.Settings["mqttPort"] == null)
            {
                config.AppSettings.Settings.Add("mqttPort", textBoxMqttPort.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPort"].Value = textBoxMqttPort.Text;
            }

            if (config.AppSettings.Settings["mqttClientId"] == null)
            {
                config.AppSettings.Settings.Add("mqttClientId", textBoxMqttClientId.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttClientId"].Value = textBoxMqttClientId.Text;
            }

            if (config.AppSettings.Settings["devAddr"] == null)
            {
                config.AppSettings.Settings.Add("devAddr", comboBoxDevAddr.Text);
            }
            else
            {
                config.AppSettings.Settings["devAddr"].Value = comboBoxDevAddr.Text;
            }

            if (config.AppSettings.Settings["mqttUserName"] == null)
            {
                config.AppSettings.Settings.Add("mqttUserName", textBoxMqttUserName.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttUserName"].Value = textBoxMqttUserName.Text;
            }

            if (config.AppSettings.Settings["mqttPassword"] == null)
            {
                config.AppSettings.Settings.Add("mqttPassword", textBoxMqttPassword.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPassword"].Value = textBoxMqttPassword.Text;
            }

            if (config.AppSettings.Settings["mqttSubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttSubTopic", textBoxMqttSubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttSubTopic"].Value = textBoxMqttSubTopic.Text;
            }

            if (config.AppSettings.Settings["mqttPubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttPubTopic", textBoxMqttPubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPubTopic"].Value = textBoxMqttPubTopic.Text;
            }

            if (config.AppSettings.Settings["tcpClientServer"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientServer", textBoxTcpClientServer.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientServer"].Value = textBoxTcpClientServer.Text;
            }

            if (config.AppSettings.Settings["tcpClientPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientPort", textBoxTcpClientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientPort"].Value = textBoxTcpClientPort.Text;
            }

            if (config.AppSettings.Settings["tcpClientLogin"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientLogin", textBoxTcpClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientLogin"].Value = textBoxTcpClientLogin.Text;
            }

            if (config.AppSettings.Settings["UDPClientServer"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientServer", textBoxUDPClientServer.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientServer"].Value = textBoxUDPClientServer.Text;
            }

            if (config.AppSettings.Settings["UDPClientPort"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientPort", textBoxUDPCLientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientPort"].Value = textBoxUDPCLientPort.Text;
            }

            if (config.AppSettings.Settings["UDPClientLogin"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientLogin", textBoxUDPClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientLogin"].Value = textBoxUDPClientLogin.Text;
            }

            if (config.AppSettings.Settings["tcpServerPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerPort", textBoxTcpServerPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpServerPort"].Value = textBoxTcpServerPort.Text;
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
            if (config.AppSettings.Settings["DisplayHex"] == null)
            {
                config.AppSettings.Settings.Add("DisplayHex", DgiotHelper.BoolTostr(checkBoxDisplayHex.Checked));
            }
            else
            {
                config.AppSettings.Settings["DisplayHex"].Value = DgiotHelper.BoolTostr(checkBoxDisplayHex.Checked);
            }

            LogHelper.Config(config.AppSettings.Settings);
        }

        private void CheckBoxTcpServer_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpServerIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerIsCheck", DgiotHelper.BoolTostr(checkBoxTcpServer.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpServerIsCheck"].Value = DgiotHelper.BoolTostr(checkBoxTcpServer.Checked);
            }

            TcpServerHelper.Config(config.AppSettings.Settings);
        }

        private void CheckBoxMqttBridge_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttbridgeIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("mqttbridgeIsCheck", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["mqttbridgeIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonMqttClient.Checked);
            }

            MqttServerHelper.Config(config.AppSettings.Settings);
        }

        private void CheckBoxUdpServer_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["udpbridgeIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("udpbridgeIsCheck", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["udpbridgeIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonMqttClient.Checked);
            }

            UDPClientHelper.Config(config.AppSettings.Settings);
        }

        private void RadioButtonMqttClient_CheckedChanged(object sender, EventArgs e)
        {
           if (config.AppSettings.Settings["mqttIsCheck"] == null)
           {
              config.AppSettings.Settings.Add("mqttIsCheck", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
           }
           else
           {
              config.AppSettings.Settings["mqttIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonMqttClient.Checked);
           }

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void RadioButtonTcpClient_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientIsCheck", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["tcpClientIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonTcpClient.Checked);
            }

           TcpClientHelper.Config(config.AppSettings.Settings);
        }

        private void RadioButtonUDPClient_CheckedChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["UDPClientIsCheck"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientIsCheck", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
            }
            else
            {
                config.AppSettings.Settings["UDPClientIsCheck"].Value = DgiotHelper.BoolTostr(radioButtonTcpClient.Checked);
            }

            UDPClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextToPayload_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["toPayload"] == null)
            {
                config.AppSettings.Settings.Add("toPayload", textToPayload.Text);
            }
            else
            {
                config.AppSettings.Settings["toPayload"].Value = textToPayload.Text;
            }
        }

        private void TextBoxMqttSever_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttServer"] == null)
            {
                config.AppSettings.Settings.Add("mqttServer", textBoxMqttSever.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttServer"].Value = textBoxMqttSever.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxMqttPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttPort"] == null)
            {
                config.AppSettings.Settings.Add("mqttPort", textBoxMqttPort.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPort"].Value = textBoxMqttPort.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxMqttClientId_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttClientId"] == null)
            {
                config.AppSettings.Settings.Add("mqttClientId", textBoxMqttClientId.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttClientId"].Value = textBoxMqttClientId.Text;
            }

            Resh_topic();

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxMqttUserName_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttUserName"] == null)
            {
                config.AppSettings.Settings.Add("mqttUserName", textBoxMqttUserName.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttUserName"].Value = textBoxMqttUserName.Text;
            }

            Resh_topic();

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxMqttPassword_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttPassword"] == null)
            {
                config.AppSettings.Settings.Add("mqttPassword", textBoxMqttPassword.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPassword"].Value = textBoxMqttPassword.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxMqttSubTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttSubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttSubTopic", textBoxMqttSubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttSubTopic"].Value = textBoxMqttSubTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxMqttPubTopic_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttPubTopic"] == null)
            {
                config.AppSettings.Settings.Add("mqttPubTopic", textBoxMqttPubTopic.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttPubTopic"].Value = textBoxMqttPubTopic.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxMqttServerPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["mqttServerPort"] == null)
            {
                config.AppSettings.Settings.Add("mqttServerPort", textBoxTcpClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["mqttServerPort"].Value = textBoxTcpClientLogin.Text;
            }

            MqttServerHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxTcpClientServer_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientServer"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientServer", textBoxTcpClientServer.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientServer"].Value = textBoxTcpClientServer.Text;
            }

            MqttClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxTcpClientPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientPort", textBoxTcpClientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientPort"].Value = textBoxTcpClientPort.Text;
            }

            TcpClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxTcpClientLogin_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpClientLogin"] == null)
            {
                config.AppSettings.Settings.Add("tcpClientLogin", textBoxTcpClientLogin.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpClientLogin"].Value = textBoxTcpClientLogin.Text;
            }

            TcpClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxTcpServerPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["tcpServerPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpServerPort", textBoxTcpServerPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpServerPort"].Value = textBoxTcpServerPort.Text;
            }

            TcpServerHelper.Config(config.AppSettings.Settings);
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

            UDPClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxUDPCLientPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["UDPClientPort"] == null)
            {
                config.AppSettings.Settings.Add("UDPClientPort", textBoxUDPCLientPort.Text);
            }
            else
            {
                config.AppSettings.Settings["UDPClientPort"].Value = textBoxUDPCLientPort.Text;
            }

            UDPClientHelper.Config(config.AppSettings.Settings);
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

            UDPClientHelper.Config(config.AppSettings.Settings);
        }

        private void TextBoxUdpServerPort_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["udpServerPort"] == null)
            {
                config.AppSettings.Settings.Add("udpServerPort", this.textBoxTcpServerPort.Text);
            }
            else
            {
                config.AppSettings.Settings["udpServerPort"].Value = textBoxTcpServerPort.Text;
            }

            UDPClientHelper.Config(config.AppSettings.Settings);
        }

        private void SendBridge_Click(object sender, EventArgs e)
        {
            byte[] payload = LogHelper.Payload(config.AppSettings.Settings["toPayload"].Value.ToCharArray());
            LogHelper.Log(bridges[comboBoxBridge.SelectedIndex] + " send  [" + LogHelper.Logdata(payload, 0, payload.Length) + "]");

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
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "OPCDA_SCAN")
            {
                OPCDAHelper.Scan();
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "OPCDA_READ")
            {
            }
            else if (bridges[this.comboBoxBridge.SelectedIndex] == "OPCDA_WRITE")
            {
                OPCDAHelper.Write();
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

            MqttClientHelper.Config(config.AppSettings.Settings);
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

            MqttClientHelper.Config(config.AppSettings.Settings);
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

            MqttClientHelper.Config(config.AppSettings.Settings);
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

            MqttClientHelper.Config(config.AppSettings.Settings);
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

            MqttClientHelper.Config(config.AppSettings.Settings);
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

            MqttClientHelper.Config(config.AppSettings.Settings);
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

            MqttClientHelper.Config(config.AppSettings.Settings);
            Resh_topic();
        }

        private void TextBoxOpcServer_TextChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["OpcServer"] == null)
            {
                config.AppSettings.Settings.Add("OpcServer", textBoxOpcServer.Text);
            }
            else
            {
                config.AppSettings.Settings["OpcServer"].Value = textBoxOpcServer.Text;
            }

            OPCDAHelper.Config(config.AppSettings.Settings);
        }

        private void ComboBoxOpcServer_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBoxLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (config.AppSettings.Settings["LogLevel"] == null)
            {
                config.AppSettings.Settings.Add("LogLevel", comboBoxLogLevel.Text);
            }
            else
            {
                config.AppSettings.Settings["LogLevel"].Value = comboBoxLogLevel.Text;
            }

            LogHelper.SetLevel(comboBoxLogLevel.SelectedIndex);
        }
    }
}
