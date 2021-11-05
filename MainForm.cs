// <copyright file="MainForm.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using PortListener.Core.Utilities;

    public partial class MainForm : Form
    {
        private delegate void LogHandler(string text);

        private MqttHelper mqtt = MqttHelper.GetInstance();
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

        private bool bAutoReconnect = false;
        private bool bDisplayHex = false;
        private bool bIsRunning = false;
        private string[] bridges = new string[]
        {
            "SerialPort",
            "TcpServer",
            "PLC",
            "OPCDA",
            "OPCUA",
            "BAnet",
            "Control",
            "Access",
            "SqlServer",
            "MqttClient",
            "TcpClient",
            "UdpClient"
        };

        private Configuration config;

        public MainForm()
        {
            this.InitializeComponent();

            this.Text += " v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            var arrPorts = SerialPortHelper.GetPorts();
            this.comboBoxSerialPort.Items.Clear();
            foreach (var port in arrPorts)
            {
                this.comboBoxSerialPort.Items.Add(port);
            }

            if (arrPorts.Length > 0)
            {
                this.comboBoxSerialPort.SelectedIndex = 0;
            }

            this.comboBoxBaudRate.SelectedIndex = 7;
            this.comboBoxDataBits.SelectedIndex = 0;
            this.comboBoxStopBits.SelectedIndex = 0;

            this.comboBoxBridge.Items.Clear();
              foreach (var bridge in bridges)
            {
                this.comboBoxBridge.Items.Add(bridge);
            }

            if (bridges.Length > 0)
            {
                this.comboBoxBridge.SelectedIndex = 0;
            }

            this.bAutoReconnect = this.checkBoxReconnect.Checked;
            this.bDisplayHex = this.checkBoxDisplayHex.Checked;

            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
                MqttHelper.Stop();
                SerialPortHelper.Stop();
                TcpClientHelper.Stop();
                TcpServerHelper.Stop();
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
                    MqttHelper.Start(config.AppSettings.Settings, bAutoReconnect, this);
                    TcpClientHelper.Start(config.AppSettings.Settings, bAutoReconnect, this);
                    UDPClientHelper.Start(config.AppSettings.Settings, bAutoReconnect, this);
                    TcpServerHelper.Start(config.AppSettings.Settings, this);
                    Log("BACnetHelper.Start");
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
            if (this.textBoxLog.Text.Length > 4096)
            {
                this.textBoxLog.Text = this.textBoxLog.Text.Substring(this.textBoxLog.Text.Length - 4096);
            }

            this.textBoxLog.Text += text + "\r\n";
            this.textBoxLog.SelectionStart = this.textBoxLog.Text.Length - 1;
            this.textBoxLog.ScrollToCaret();
        }

        public string Logdata(byte[] data, int offset, int len)
        {
            var line = this.bDisplayHex ? StringHelper.ToHexString(data, offset, len) : System.Text.Encoding.ASCII.GetString(data, offset, len);
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
                var tmp = config.AppSettings.Settings["serialPortIsCheck"].Value;
                comboBoxStopBits.SelectedIndex = comboBoxStopBits.Items.IndexOf(tmp);
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

            if (config.AppSettings.Settings["mqttClientId"] != null)
            {
                this.textBoxMqttClientId.Text = config.AppSettings.Settings["mqttClientId"].Value;
            }

            if (config.AppSettings.Settings["mqttUserName"] != null)
            {
                this.textBoxMqttUserName.Text = config.AppSettings.Settings["mqttUserName"].Value;
            }

            if (config.AppSettings.Settings["mqttPassword"] != null)
            {
                this.textBoxMqttPassword.Text = config.AppSettings.Settings["mqttPassword"].Value;
            }

            if (config.AppSettings.Settings["mqttSubTopic"] != null)
            {
                this.textBoxMqttSubTopic.Text = config.AppSettings.Settings["mqttSubTopic"].Value;
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

            MqttHelper.Config(config.AppSettings.Settings, this);
            TcpClientHelper.Config(config.AppSettings.Settings, this);
            UDPClientHelper.Config(config.AppSettings.Settings, this);

            SerialPortHelper.Config(config.AppSettings.Settings, this);
            PLCHelper.Config(config.AppSettings.Settings, this);
            OPCDAHelper.Config(config.AppSettings.Settings, this);
            OPCUAHelper.Config(config.AppSettings.Settings, this);
            AccessHelper.Config(config.AppSettings.Settings, this);
            SqlServerHelper.Config(config.AppSettings.Settings, this);
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
            if (this.config.AppSettings.Settings["tcpServerIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpServerIsCheck", StringHelper.BoolTostr(this.checkBoxTcpServer.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["tcpServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxTcpServer.Checked);
            }

            if (this.config.AppSettings.Settings["serialPortIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("serialPortIsCheck", StringHelper.BoolTostr(this.checkBoxSerialPort.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["serialPortIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSerialPort.Checked);
            }

            if (this.config.AppSettings.Settings["PLCIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("PLCIsCheck", StringHelper.BoolTostr(this.checkBoxPLC.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["PLCIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxPLC.Checked);
            }

            if (this.config.AppSettings.Settings["OPCDAIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("OPCDAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCDA.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["OPCDAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCDA.Checked);
            }

            if (this.config.AppSettings.Settings["OPCUAIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("OPCUAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCUA.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["OPCUAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCUA.Checked);
            }

            if (this.config.AppSettings.Settings["BACnetIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("BACnetIsCheck", StringHelper.BoolTostr(this.checkBoxBAnet.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["BACnetIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxBAnet.Checked);
            }

            if (this.config.AppSettings.Settings["ControlIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("ControlIsCheck", StringHelper.BoolTostr(this.checkBoxControl.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["ControlIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxControl.Checked);
            }

            if (this.config.AppSettings.Settings["AccessIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("AccessIsCheck", StringHelper.BoolTostr(this.checkBoxAccess.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["AccessIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxAccess.Checked);
            }

            if (this.config.AppSettings.Settings["SqlServerIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("SqlServerIsCheck", StringHelper.BoolTostr(this.checkBoxSqlServer.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["SqlServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSqlServer.Checked);
            }

            if (this.config.AppSettings.Settings["mqttIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["mqttIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
            }

            if (this.config.AppSettings.Settings["tcpClientIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

            if (this.config.AppSettings.Settings["UDPClientIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

            if (this.config.AppSettings.Settings["toPayload"] == null)
            {
                this.config.AppSettings.Settings.Add("toPayload", this.textToPayload.Text);
            }
            else
            {
                this.config.AppSettings.Settings["toPayload"].Value = this.textToPayload.Text;
            }

            if (this.config.AppSettings.Settings["mqttServer"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttServer", this.textBoxMqttSever.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttServer"].Value = this.textBoxMqttSever.Text;
            }

            if (this.config.AppSettings.Settings["mqttPort"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttPort", this.textBoxMqttPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttPort"].Value = this.textBoxMqttPort.Text;
            }

            if (this.config.AppSettings.Settings["mqttClientId"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttClientId", this.textBoxMqttClientId.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttClientId"].Value = this.textBoxMqttClientId.Text;
            }

            if (this.config.AppSettings.Settings["mqttUserName"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttUserName", this.textBoxMqttUserName.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttUserName"].Value = this.textBoxMqttUserName.Text;
            }

            if (this.config.AppSettings.Settings["mqttPassword"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttPassword", this.textBoxMqttPassword.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttPassword"].Value = this.textBoxMqttPassword.Text;
            }

            if (this.config.AppSettings.Settings["mqttSubTopic"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttSubTopic", this.textBoxMqttSubTopic.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttSubTopic"].Value = this.textBoxMqttSubTopic.Text;
            }

            if (this.config.AppSettings.Settings["mqttPubTopic"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttPubTopic", this.textBoxMqttPubTopic.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttPubTopic"].Value = this.textBoxMqttPubTopic.Text;
            }

            if (this.config.AppSettings.Settings["tcpClientServer"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientServer", this.textBoxTcpClientServer.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientServer"].Value = this.textBoxTcpClientServer.Text;
            }

            if (this.config.AppSettings.Settings["tcpClientPort"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientPort", this.textBoxTcpClientPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientPort"].Value = this.textBoxTcpClientPort.Text;
            }

            if (this.config.AppSettings.Settings["tcpClientLogin"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientLogin", this.textBoxTcpClientLogin.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientLogin"].Value = this.textBoxTcpClientLogin.Text;
            }

            if (this.config.AppSettings.Settings["UDPClientServer"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientServer", this.textBoxUDPClientServer.Text);
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientServer"].Value = this.textBoxUDPClientServer.Text;
            }

            if (this.config.AppSettings.Settings["UDPClientPort"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientPort", this.textBoxUDPCLientPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientPort"].Value = this.textBoxUDPCLientPort.Text;
            }

            if (this.config.AppSettings.Settings["UDPClientLogin"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientLogin", this.textBoxUDPClientLogin.Text);
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientLogin"].Value = this.textBoxUDPClientLogin.Text;
            }

            if (this.config.AppSettings.Settings["tcpServerPort"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpServerPort", this.textBoxTcpServerPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpServerPort"].Value = this.textBoxTcpServerPort.Text;
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
            if (this.config.AppSettings.Settings["tcpServerIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpServerIsCheck", StringHelper.BoolTostr(this.checkBoxTcpServer.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["tcpServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxTcpServer.Checked);
            }

            TcpServerHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxSerialPort_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["serialPortIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("serialPortIsCheck", StringHelper.BoolTostr(this.checkBoxSerialPort.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["serialPortIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSerialPort.Checked);
            }

            SerialPortHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxPLC_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["PLCIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("PLCIsCheck", StringHelper.BoolTostr(this.checkBoxPLC.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["PLCIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxPLC.Checked);
            }

            PLCHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxOPCDA_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["OPCDAIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("OPCDAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCDA.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["OPCDAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCDA.Checked);
            }

            OPCDAHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxOPCUA_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["OPCUAIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("OPCUAIsCheck", StringHelper.BoolTostr(this.checkBoxOPCUA.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["OPCUAIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxOPCUA.Checked);
            }

            OPCUAHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxBAnet_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["BACnetIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("BACnetIsCheck", StringHelper.BoolTostr(this.checkBoxBAnet.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["BACnetIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxBAnet.Checked);
            }

            BACnetHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxControl_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["ControlIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("ControlIsCheck", StringHelper.BoolTostr(this.checkBoxControl.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["ControlIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxControl.Checked);
            }

            ControlHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxSqlServer_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["SqlServerIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("SqlServerIsCheck", StringHelper.BoolTostr(this.checkBoxSqlServer.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["SqlServerIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxSqlServer.Checked);
            }

            SqlServerHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxAccess_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["AccessIsCheck"] == null)
            {
               this.config.AppSettings.Settings.Add("AccessIsCheck", StringHelper.BoolTostr(this.checkBoxAccess.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["AccessIsCheck"].Value = StringHelper.BoolTostr(this.checkBoxAccess.Checked);
            }

            AccessHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxMqttBridge_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttbridgeIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttbridgeIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["mqttbridgeIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void CheckBoxUdpServer_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["udpbridgeIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("udpbridgeIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["udpbridgeIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
            }

            UDPClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void RadioButtonMqttClient_CheckedChanged(object sender, EventArgs e)
        {
           if (this.config.AppSettings.Settings["mqttIsCheck"] == null)
           {
              this.config.AppSettings.Settings.Add("mqttIsCheck", StringHelper.BoolTostr(this.radioButtonMqttClient.Checked));
           }
           else
           {
              this.config.AppSettings.Settings["mqttIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonMqttClient.Checked);
           }

          MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void RadioButtonTcpClient_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["tcpClientIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

           TcpClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void RadioButtonUDPClient_CheckedChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["UDPClientIsCheck"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientIsCheck", StringHelper.BoolTostr(this.radioButtonTcpClient.Checked));
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientIsCheck"].Value = StringHelper.BoolTostr(this.radioButtonTcpClient.Checked);
            }

            UDPClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextToPayload_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["toPayload"] == null)
            {
                this.config.AppSettings.Settings.Add("toPayload", this.textToPayload.Text);
            }
            else
            {
                this.config.AppSettings.Settings["toPayload"].Value = this.textToPayload.Text;
            }
        }

        private void TextBoxMqttSever_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttServer"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttServer", this.textBoxMqttSever.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttServer"].Value = this.textBoxMqttSever.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxMqttPort_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttPort"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttPort", this.textBoxMqttPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttPort"].Value = this.textBoxMqttPort.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxMqttClientId_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttClientId"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttClientId", this.textBoxMqttClientId.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttClientId"].Value = this.textBoxMqttClientId.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxMqttUserName_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttUserName"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttUserName", this.textBoxMqttUserName.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttUserName"].Value = this.textBoxMqttUserName.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxMqttPassword_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttPassword"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttPassword", this.textBoxMqttPassword.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttPassword"].Value = this.textBoxMqttPassword.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxMqttSubTopic_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttSubTopic"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttSubTopic", this.textBoxMqttSubTopic.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttSubTopic"].Value = this.textBoxMqttSubTopic.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxMqttPubTopic_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttPubTopic"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttPubTopic", this.textBoxMqttPubTopic.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttPubTopic"].Value = this.textBoxMqttPubTopic.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxMqttServerPort_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["mqttServerPort"] == null)
            {
                this.config.AppSettings.Settings.Add("mqttServerPort", this.textBoxTcpClientLogin.Text);
            }
            else
            {
                this.config.AppSettings.Settings["mqttServerPort"].Value = this.textBoxTcpClientLogin.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxTcpClientServer_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["tcpClientServer"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientServer", this.textBoxTcpClientServer.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientServer"].Value = this.textBoxTcpClientServer.Text;
            }

            MqttHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxTcpClientPort_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["tcpClientPort"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientPort", this.textBoxTcpClientPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientPort"].Value = this.textBoxTcpClientPort.Text;
            }

            TcpClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxTcpClientLogin_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["tcpClientLogin"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpClientLogin", this.textBoxTcpClientLogin.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpClientLogin"].Value = this.textBoxTcpClientLogin.Text;
            }

            TcpClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxTcpServerPort_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["tcpServerPort"] == null)
            {
                this.config.AppSettings.Settings.Add("tcpServerPort", this.textBoxTcpServerPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["tcpServerPort"].Value = this.textBoxTcpServerPort.Text;
            }

            TcpServerHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxUDPClientServer_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["UDPClientServer"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientServer", this.textBoxUDPClientServer.Text);
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientServer"].Value = this.textBoxUDPClientServer.Text;
            }

            UDPClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxUDPCLientPort_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["UDPClientPort"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientPort", this.textBoxUDPCLientPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientPort"].Value = this.textBoxUDPCLientPort.Text;
            }

            UDPClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxUDPClientLogin_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["UDPClientLogin"] == null)
            {
                this.config.AppSettings.Settings.Add("UDPClientLogin", this.textBoxUDPClientLogin.Text);
            }
            else
            {
                this.config.AppSettings.Settings["UDPClientLogin"].Value = this.textBoxUDPClientLogin.Text;
            }

            UDPClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void TextBoxUdpServerPort_TextChanged(object sender, EventArgs e)
        {
            if (this.config.AppSettings.Settings["udpServerPort"] == null)
            {
                this.config.AppSettings.Settings.Add("udpServerPort", this.textBoxTcpServerPort.Text);
            }
            else
            {
                this.config.AppSettings.Settings["udpServerPort"].Value = this.textBoxTcpServerPort.Text;
            }

            UDPClientHelper.Config(this.config.AppSettings.Settings, this);
        }

        private void SendBridge_Click(object sender, EventArgs e)
        {
            byte[] payload = Payload(config.AppSettings.Settings["toPayload"].Value.ToCharArray());
            Log(this.bridges[this.comboBoxBridge.SelectedIndex] + "send [" + Logdata(payload, 0, payload.Length) + "]");

            if (this.bridges[this.comboBoxBridge.SelectedIndex] == "SerialPort")
            {
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "TcpServer")
            {
                TcpServerHelper.Write(payload, 0, payload.Length);
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "PLC")
            {
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "OPCDA")
            {
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "OPCUA")
            {
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "BAnet")
            {
                BACnetHelper.Write(payload, 0, payload.Length);
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "Control")
            {
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "Access")
            {
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "SqlServer")
            {
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "MqttClient")
            {
                MqttHelper.Write(payload, 0, payload.Length);
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "TcpClient")
            {
                TcpClientHelper.Write(payload, 0, payload.Length);
            }
            else if (this.bridges[this.comboBoxBridge.SelectedIndex] == "UdpClient")
            {
            }
            else
            {
            }
        }

        private void ComboBoxCmdProdxy_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
