// <copyright file="MainForm.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private delegate void LogHandler(string text);

        private static string clientid = Guid.NewGuid().ToString().Substring(0, 10);
        private static string productid = Guid.NewGuid().ToString().Substring(0, 10);
        private static string devaddr = Guid.NewGuid().ToString().Substring(0, 10);
        private static OPCDAHelper oPCDAHelper = OPCDAHelper.GetInstance();
        private static OPCDAViewHelper oPCDAViewHelper = OPCDAViewHelper.GetInstance();
        private bool bAutoReconnect = false;
        private bool bIsRunning = false;
        private readonly string[] bridges = new string[]
        {
            "SerialPort",
            "TcpServer",
            "BACnet",
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

            LogHelper.Init(this);
            TreeViewHelper.Init(treeView);
            FileHelper.Init(openFileDialog);
            SetComboBox();

            Text += " v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            try
            {
                RestoreConfigs(config);
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

                DgiotHelper.GetIps();
                SaveAppConfig();
            }
            else
            {
                ToStop();
            }
        }

        private void SetComboBox()
        {
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

            bAutoReconnect = checkBoxReconnect.Checked;

            comboBoxDevAddr.Items.Clear();
            List<string> macAddrs = DgiotHelper.GetMacByWmi();
            foreach (var mac in macAddrs)
            {
                devaddr = Regex.Replace(mac, @":", "");
                comboBoxDevAddr.Items.Add(devaddr);
            }

            comboBoxDevAddr.SelectedIndex = 0;

            List<string> loglevels = LogHelper.Levels();
            foreach (var level in loglevels)
            {
                comboBoxLogLevel.Items.Add(level);
            }

            if (loglevels.Count > 0)
            {
                comboBoxLogLevel.SelectedIndex = (int)LogHelper.Level.DEBUG;
            }

            comboBoxLan.Items.Add("简体中文");
            comboBoxLan.Items.Add("English");
            comboBoxLan.SelectedIndex = 0;
        }

        private void RestoreConfigs(Configuration config)
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
                textBoxMqttPort.Text = config.AppSettings.Settings["mqttPort"].Value;
            }

            if (config.AppSettings.Settings["mqttUserName"] != null)
            {
                textBoxMqttUserName.Text = config.AppSettings.Settings["mqttUserName"].Value;
            }

            if (config.AppSettings.Settings["devAddr"] != null)
            {
                var tmp = config.AppSettings.Settings["devAddr"].Value;
                comboBoxDevAddr.SelectedIndex = comboBoxDevAddr.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["mqttPassword"] != null)
            {
                textBoxMqttPassword.Text = config.AppSettings.Settings["mqttPassword"].Value;
            }

            if (config.AppSettings.Settings["mqttPubTopic"] != null)
            {
                textBoxMqttPubTopic.Text = config.AppSettings.Settings["mqttPubTopic"].Value;
            }

            if (config.AppSettings.Settings["tcpClientServer"] != null)
            {
                textBoxTcpClientServer.Text = config.AppSettings.Settings["tcpClientServer"].Value;
            }

            if (config.AppSettings.Settings["tcpClientPort"] != null)
            {
                textBoxTcpClientPort.Text = config.AppSettings.Settings["tcpClientPort"].Value;
            }

            if (config.AppSettings.Settings["tcpClientLogin"] != null)
            {
                textBoxTcpClientLogin.Text = config.AppSettings.Settings["tcpClientLogin"].Value;
            }

            if (config.AppSettings.Settings["UDPClientServer"] != null)
            {
                textBoxUDPClientServer.Text = config.AppSettings.Settings["UDPClientServer"].Value;
            }

            if (config.AppSettings.Settings["UDPClientPort"] != null)
            {
                textBoxUDPCLientPort.Text = config.AppSettings.Settings["UDPClientPort"].Value;
            }

            if (config.AppSettings.Settings["tcpServerPort"] != null)
            {
               textBoxTcpServerPort.Text = config.AppSettings.Settings["tcpServerPort"].Value;
            }

            if (config.AppSettings.Settings["tcpServerIsCheck"] != null)
            {
                checkBoxTcpBridge.Checked = DgiotHelper.StrTobool(config.AppSettings.Settings["tcpServerIsCheck"].Value);
            }

            if (config.AppSettings.Settings["LogLevel"] != null)
            {
                var tmp = config.AppSettings.Settings["LogLevel"].Value;
                comboBoxLogLevel.SelectedIndex = comboBoxLogLevel.Items.IndexOf(tmp);
            }

            if (config.AppSettings.Settings["OPCDAInterval"] != null)
            {
                textBoxOPCDAInterval.Text = config.AppSettings.Settings["OPCDAInterval"].Value;
            }

            if (config.AppSettings.Settings["OPCDACheck"] != null)
            {
                checkBoxOPCDA.Text = config.AppSettings.Settings["OPCDACheck"].Value;
            }

            Resh_Topic();
            Resh_Config();
        }

        private void SetConfig(string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }

            Resh_Config();
            Resh_Topic();
        }

        private void Resh_Config()
        {
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

        private void Resh_Topic()
        {
            devaddr = comboBoxDevAddr.Text;
            productid = textBoxMqttUserName.Text;
            clientid = DgiotHelper.Md5("Device" + this.textBoxMqttUserName.Text + devaddr).Substring(0, 10);
            textBoxMqttClientId.Text = clientid;
            textBoxMqttSubTopic.Text = "/" + productid + "/" + devaddr;
            textBoxMqttPubTopic.Text = "/" + productid + "/" + devaddr + "/properties/read/reply";
            textBoxAccessTopic.Text = "/" + productid + "/" + devaddr + "/scan/mdb";

            textBoxOPCDATopic.Text = "/" + productid + "/" + devaddr + "/scan/opcda/reply";
            textBoxOPCUATopic.Text = "/" + productid + "/" + devaddr + "/scan/opcua/reply";
            textBoxPLCTopic.Text = "/" + productid + "/" + devaddr + "/scan/plc/reply";
            textBoxBACnetTopic.Text = "/" + productid + "/" + devaddr + "/scan/bacnet/reply";
            textBoxControlTopic.Text = "/" + productid + "/" + devaddr + "/scan/control/reply";
            textBoxSqlServerTopic.Text = "/" + productid + "/" + devaddr + "/scan/sqlserver/reply";
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
            SetConfig("tcpServerIsCheck", DgiotHelper.BoolTostr(checkBoxTcpBridge.Checked));
            SetConfig("PLCTopic", textBoxPLCTopic.Text);
            SetConfig("OPCDATopic", textBoxOPCDATopic.Text);
            SetConfig("OPCUATopic", textBoxOPCUATopic.Text);
            SetConfig("BACnetTopic", textBoxBACnetTopic.Text);
            SetConfig("ControlTopic", textBoxControlTopic.Text);
            SetConfig("AccessTopic", textBoxAccessTopic.Text);
            SetConfig("SqlServerTopic", textBoxSqlServerTopic.Text);
            SetConfig("mqttIsCheck", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
            SetConfig("tcpClientIsCheck", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
            SetConfig("UDPClientIsCheck", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
            SetConfig("toPayload", textToPayload.Text);
            SetConfig("mqttServer", textBoxMqttSever.Text);
            SetConfig("mqttPort", textBoxMqttPort.Text);
            SetConfig("mqttClientId", textBoxMqttClientId.Text);
            SetConfig("devAddr", comboBoxDevAddr.Text);
            SetConfig("mqttUserName", textBoxMqttUserName.Text);
            SetConfig("mqttPassword", textBoxMqttPassword.Text);
            SetConfig("mqttSubTopic", textBoxMqttSubTopic.Text);
            SetConfig("mqttPubTopic", textBoxMqttPubTopic.Text);
            SetConfig("tcpClientServer", textBoxTcpClientServer.Text);
            SetConfig("tcpClientPort", textBoxTcpClientPort.Text);
            SetConfig("tcpClientLogin", textBoxTcpClientLogin.Text);
            SetConfig("UDPClientServer", textBoxUDPClientServer.Text);
            SetConfig("UDPClientPort", textBoxUDPCLientPort.Text);
            SetConfig("UDPClientLogin", textBoxUDPClientLogin.Text);
            SetConfig("tcpServerPort", textBoxTcpServerPort.Text);
            SetConfig("LogLevel", comboBoxLogLevel.Text);
            SetConfig("OPCDAInterval", textBoxOPCDAInterval.Text);
            SetConfig("OPCDACheck", checkBoxOPCDA.Text);

            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void CheckBoxReconnectCheckedChanged(object sender, EventArgs e)
        {
            bAutoReconnect = checkBoxReconnect.Checked;
        }

        private void CheckBoxDisplayHexCheckedChanged(object sender, EventArgs e)
        {
            SetConfig("DisplayHex", DgiotHelper.BoolTostr(checkBoxDisplayHex.Checked));
        }

        private void CheckBoxMqttBridge_CheckedChanged(object sender, EventArgs e)
        {
            SetConfig("mqttbridgeIsCheck", DgiotHelper.BoolTostr(checkBoxMqttBridge.Checked));
        }

        private void CheckBoxTcpBridge_CheckedChanged(object sender, EventArgs e)
        {
            SetConfig("tcpBridgeIsCheck", DgiotHelper.BoolTostr(checkBoxTcpBridge.Checked));
        }

        private void CheckBoxUdpBridge_CheckedChanged(object sender, EventArgs e)
        {
            SetConfig("udpBridgeIsCheck", DgiotHelper.BoolTostr(checkBoxUdpBridge.Checked));
        }

        private void RadioButtonMqttClient_CheckedChanged(object sender, EventArgs e)
        {
            SetConfig("mqttIsCheck", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
        }

        private void RadioButtonTcpClient_CheckedChanged(object sender, EventArgs e)
        {
           SetConfig("tcpClientIsCheck", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
        }

        private void TextToPayload_TextChanged(object sender, EventArgs e)
        {
            SetConfig("toPayload", textToPayload.Text);
        }

        private void TextBoxMqttSever_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttServer", textBoxMqttSever.Text);
        }

        private void TextBoxMqttPort_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttPort", textBoxMqttPort.Text);
        }

        private void TextBoxMqttClientId_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttClientId", textBoxMqttClientId.Text);
        }

        private void TextBoxMqttUserName_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttUserName", textBoxMqttUserName.Text);
        }

        private void TextBoxMqttPassword_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttPassword", textBoxMqttPassword.Text);
        }

        private void TextBoxMqttSubTopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttSubTopic", textBoxMqttSubTopic.Text);
        }

        private void TextBoxMqttPubTopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttPubTopic", textBoxMqttPubTopic.Text);
        }

        private void TextBoxMqttServerPort_TextChanged(object sender, EventArgs e)
        {
            SetConfig("mqttServerPort", textBoxTcpClientLogin.Text);
        }

        private void TextBoxTcpClientServer_TextChanged(object sender, EventArgs e)
        {
            SetConfig("tcpClientServer", textBoxTcpClientServer.Text);
        }

        private void TextBoxTcpClientPort_TextChanged(object sender, EventArgs e)
        {
            SetConfig("tcpClientPort", textBoxTcpClientPort.Text);
        }

        private void TextBoxTcpClientLogin_TextChanged(object sender, EventArgs e)
        {
            SetConfig("tcpClientLogin", textBoxTcpClientLogin.Text);
        }

        private void TextBoxTcpServerPort_TextChanged(object sender, EventArgs e)
        {
            SetConfig("tcpServerPort", textBoxTcpServerPort.Text);
        }

        private void TextBoxUDPClientServer_TextChanged(object sender, EventArgs e)
        {
            SetConfig("UDPClientServer", textBoxUDPClientServer.Text);
        }

        private void TextBoxUDPCLientPort_TextChanged(object sender, EventArgs e)
        {
            SetConfig("UDPClientPort", textBoxUDPCLientPort.Text);
        }

        private void TextBoxUDPClientLogin_TextChanged(object sender, EventArgs e)
        {
            SetConfig("UDPClientLogin", textBoxUDPClientLogin.Text);
        }

        private void TextBoxUdpServerPort_TextChanged(object sender, EventArgs e)
        {
            SetConfig("udpServerPort", textBoxUdpServerPort.Text);
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
            SetConfig("devAddr", comboBoxDevAddr.Text);
        }

        private void TextBoxPLCTopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("PLCTopic", textBoxPLCTopic.Text);
        }

        private void TextBoxOPCDATopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("OPCDATopic", textBoxOPCDATopic.Text);
        }

        private void TextBoxOPCUATopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("OPCUATopic", textBoxOPCUATopic.Text);
        }

        private void TextBoxBACnetTopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("BACnetTopic", textBoxBACnetTopic.Text);
        }

        private void TextBoxControlTopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("ControlTopic", textBoxControlTopic.Text);
        }

        private void TextBoxAccessTopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("AccessTopic", textBoxAccessTopic.Text);
        }

        private void TextBoxSqlServerTopic_TextChanged(object sender, EventArgs e)
        {
            SetConfig("SqlServerTopic", textBoxSqlServerTopic.Text);
        }

        private void ComboBoxLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetConfig("LogLevel", comboBoxLogLevel.Text);
            LogHelper.SetLevel(comboBoxLogLevel.SelectedIndex);
        }

        private void ComboBoxBridge_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetConfig("Bridge", comboBoxBridge.Text);
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
            else if (bridges[comboBoxBridge.SelectedIndex] == "OPCUA")
            {
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "BACnet")
            {
                BACnetHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "Control")
            {
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "Access")
            {
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "SqlServer")
            {
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "MqttClient")
            {
                MqttClientHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "MqttServer")
            {
                MqttServerHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "TcpClient")
            {
                TcpClientHelper.Write(payload, 0, payload.Length);
            }
            else if (bridges[comboBoxBridge.SelectedIndex] == "UdpClient")
            {
            }
            else
            {
            }
        }

        private void ComboBoxLan_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language(comboBoxLan.SelectedItem.ToString());
        }

        private void Language(String lan)
        {
            if (lan == "English")
            {
                En();
            }
            else
            {
                Zh();
            }
        }

        private void Zh()
        {
            sendBridge.Text = "发送";
            buttonClear.Text = "清除";
            buttonStartStop.Text = "开始";

            checkBoxReconnect.Text = "自动重连";
            label3.Text = "作者：";
            label33.Text = "语言";
            label32.Text = "日志级别";

            label7.Text = "发至";
            label2.Text = "发至";
            labelopcua.Text = "发至";
            label23.Text = "发至";
            label8.Text = "发至";
            label6.Text = "发至";
            label14.Text = "发至";
            labelopcda.Text = "发至";

            groupBoxSerialPort.Text = "串口捕获";
            groupBox12.Text = "PLC 捕获";
            groupBox4.Text = "OPC_DA 捕获";
            groupBox5.Text = "OPC_UA 捕获";
            groupBox6.Text = "BACnet 捕获";
            groupBox7.Text = "控制捕获";
            groupBox8.Text = "访问捕获";
            groupBox9.Text = "Sql Server 捕获";

            labelSerialPort.Text = "端口";
            label1.Text = "波特率";
            label4.Text = "数据位";
            label13.Text = "校验位";
            label5.Text = "停止位";

            groupBox3.Text = "Mqtt 客户端通道";
            label22.Text = "服务";
            label21.Text = "端口";
            label9.Text = "用户名";
            label10.Text = "密码";
            label30.Text = "开发地址";
            label20.Text = "客户编号";
            label11.Text = "订阅主题";
            label12.Text = "发布主题";
            label29.Text = "命令行代理";
            label27.Text = "桥接";

            groupBox2.Text = "TCP 客户端通道";
            label18.Text = "服务";
            label17.Text = "端口";
            label16.Text = "登录";
            labelTargetPort.Text = "桥接";

            groupBox10.Text = "UDP 客户端通道";
            label26.Text = "服务";
            label24.Text = "端口";
            label15.Text = "登录";
            label28.Text = "桥接";

            label_devicelog.Text = "设备日志";
            label_devcietree.Text = "设备树";
            checkBoxOPCDA.Text = "主动上报";
            labelOPCDAMonitor.Text = "采集间隔";
            labelSecond.Text = "秒";
        }

        private void En()
        {
            sendBridge.Text = "Send";
            buttonClear.Text = "Clear";
            buttonStartStop.Text = "Start";
            label32.Text = "Level";

            checkBoxReconnect.Text = "Auto Reconnect";
            label3.Text = "Author:";
            label33.Text = "Language";

            label7.Text = "To";
            label2.Text = "To";
            labelopcua.Text = "To";
            label23.Text = "To";
            label8.Text = "To";
            label6.Text = "To";
            label14.Text = "To";
            labelopcda.Text = "To";

            groupBoxSerialPort.Text = "Serial Port Capture";
            groupBox12.Text = "PLC Capture";
            groupBox4.Text = "OPC_DA Capture";
            groupBox5.Text = "OPC_UA Capture";
            groupBox6.Text = "BACnet Capture";
            groupBox7.Text = "Control Capture";
            groupBox8.Text = "Access Capture";
            groupBox9.Text = "Sql Server Capture";

            labelSerialPort.Text = "Port";
            label1.Text = "Baud Rate";
            label4.Text = "dataBits";
            label13.Text = "Parity";
            label5.Text = "stopBits";
            labelopcda.Text = "Server";

            groupBox3.Text = "Mqtt Client Channel";
            label22.Text = "Server";
            label21.Text = "Port";
            label9.Text = "UserName";
            label10.Text = "PassWord";
            label30.Text = "DevAddr";
            label20.Text = "Clientid";
            label11.Text = "SubTopic";
            label12.Text = "PubTopic";
            label29.Text = "cmd proxy";
            label27.Text = "bridge";

            groupBox2.Text = "TCP Client Channel";
            label18.Text = "Server";
            label17.Text = "Port";
            label16.Text = "login";
            labelTargetPort.Text = "bridge";

            groupBox10.Text = "UDP Client Channel";
            label26.Text = "Server";
            label24.Text = "Port";
            label15.Text = "login";
            label28.Text = "bridge";

            label_devicelog.Text = "DeviceLog";
            label_devcietree.Text = "DeviceTree";

            checkBoxOPCDA.Text = "Monitor";
            labelOPCDAMonitor.Text = "Interval";
            labelSecond.Text = "Second";
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeViewHelper.TreeView_AfterSelect(e.Action, e.Node);
        }

        private void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeViewHelper.TreeView_AfterCheck(e.Action, e.Node);
        }

        private void NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeViewHelper.NodeMouseDoubleClick(e.Button, e.Node);
        }

        private void AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (TreeViewHelper.CheckLabelEdit(e.Label, e.Node))
                    {
                        // Stop editing without canceling the label change.
                        e.Node.EndEdit(false);
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    e.Node.BeginEdit();
                }
            }
        }

        private void CheckBoxOPCDA_CheckedChanged(object sender, EventArgs e)
        {
            LogHelper.Log("checkBoxOPCDA " + DgiotHelper.BoolTostr(checkBoxOPCDA.Checked));
            SetConfig("OPCDACheck", DgiotHelper.BoolTostr(checkBoxOPCDA.Checked));
            OPCDAHelper.StartMonitor();
        }

        private void TextBoxOPCDAInterval_TextChanged(object sender, EventArgs e)
        {
            SetConfig("OPCDAInterval", textBoxOPCDAInterval.Text);
        }
    }
}
