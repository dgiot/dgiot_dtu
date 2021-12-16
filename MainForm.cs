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
            ConfigHelper.Init(config);
            TreeViewHelper.Init(treeView);
            LogHelper.Init(this);
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
                    MqttServerHelper.Start();
                    MqttClientHelper.Start();
                    TcpServerHelper.Start(config.AppSettings.Settings);
                    TcpClientHelper.Start();
                    UDPServerHelper.Start(config.AppSettings.Settings);
                    UDPClientHelper.Start();
                    OPCDAHelper.Start();
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

            comboBoxDtuAddr.Items.Clear();
            List<string> macAddrs = DgiotHelper.GetMacByWmi();
            foreach (var mac in macAddrs)
            {
                devaddr = Regex.Replace(mac, @":", "");
                comboBoxDtuAddr.Items.Add(devaddr);
            }

            comboBoxDtuAddr.SelectedIndex = 0;

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
            RestoreMqtt();
            RestoreTcp();
            RestoreUDP();
            RestoreCommonConfig();
            RestoreSerialPort();
            RestorePLC();
            RestoreOPCDA();
            RestoreOPCUA();
            RestoreBACnet();
            RestoreControl();
            RestoreAccess();
            Resh_Topic();
            Resh_Config();
        }

        private void RestoreCommonConfig()
        {
            if (ConfigHelper.Check("DtuAddr"))
            {
                var tmp = ConfigHelper.GetConfig("DtuAddr");
                comboBoxDtuAddr.SelectedIndex = comboBoxDtuAddr.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("DtuAddr", comboBoxDtuAddr.Text);
            }

            if (ConfigHelper.Check("DgiotSever"))
            {
                textBoxDgiotSever.Text = ConfigHelper.GetConfig("DgiotSever");
            }
            else
            {
                ConfigHelper.SetConfig("DgiotSever", textBoxDgiotSever.Text);
            }

            if (ConfigHelper.Check("DgiotPort"))
            {
                textBoxDgiotPort.Text = ConfigHelper.GetConfig("DgiotPort");
            }
            else
            {
                ConfigHelper.SetConfig("DgiotPort", textBoxDgiotPort.Text);
            }

            if (ConfigHelper.Check("BridgePort"))
            {
                textBoxBridgePort.Text = ConfigHelper.GetConfig("BridgePort");
            }
            else
            {
                ConfigHelper.SetConfig("BridgePort", textBoxBridgePort.Text);
            }

            if (ConfigHelper.Check("ReconnectChecked"))
            {
                checkBoxReconnect.Checked = DgiotHelper.StrTobool(ConfigHelper.GetConfig("ReconnectChecked"));
            }
            else
            {
                ConfigHelper.SetConfig("ReconnectChecked", DgiotHelper.BoolTostr(checkBoxReconnect.Checked));
            }

            if (ConfigHelper.Check("LogLevel"))
            {
                var tmp = ConfigHelper.GetConfig("LogLevel");
                comboBoxLogLevel.SelectedIndex = comboBoxLogLevel.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("LogLevel", comboBoxLogLevel.Text);
            }

            if (ConfigHelper.Check("ToPayload"))
            {
                textToPayload.Text = ConfigHelper.GetConfig("ToPayload");
            }
            else
            {
                ConfigHelper.SetConfig("ToPayload", textToPayload.Text);
            }

            if (ConfigHelper.Check("DisplayHex"))
            {
                checkBoxDisplayHex.Checked = DgiotHelper.StrTobool(ConfigHelper.GetConfig("DisplayHex"));
            }
            else
            {
                ConfigHelper.SetConfig("DisplayHex", DgiotHelper.BoolTostr(checkBoxDisplayHex.Checked));
            }

            if (ConfigHelper.Check("Bridge_Checked"))
            {
                checkBoxBridge.Checked = DgiotHelper.StrTobool(ConfigHelper.GetConfig("Bridge_Checked"));
            }
            else
            {
                ConfigHelper.SetConfig("Bridge_Checked", DgiotHelper.BoolTostr(checkBoxBridge.Checked));
            }
        }

        private void RestoreMqtt()
        {
            if (ConfigHelper.Check("MqttUserName"))
            {
                textBoxMqttUserName.Text = ConfigHelper.GetConfig("MqttUserName");
            }
            else
            {
                ConfigHelper.SetConfig("MqttUserName", textBoxMqttUserName.Text);
            }

            if (ConfigHelper.Check("MqttPassword"))
            {
                textBoxMqttPassword.Text = ConfigHelper.GetConfig("MqttPassword");
            }
            else
            {
                ConfigHelper.SetConfig("MqttPassword", textBoxMqttPassword.Text);
            }

            if (ConfigHelper.Check("MqttClientId"))
            {
                textBoxMqttClientId.Text = ConfigHelper.GetConfig("MqttClientId");
            }
            else
            {
                ConfigHelper.SetConfig("MqttClientId", textBoxMqttClientId.Text);
            }

            if (ConfigHelper.Check("MqttPubTopic"))
            {
                textBoxMqttPubTopic.Text = ConfigHelper.GetConfig("MqttPubTopic");
            }
            else
            {
                ConfigHelper.SetConfig("MqttPubTopic", textBoxMqttPubTopic.Text);
            }

            if (ConfigHelper.Check("MqttSubTopic"))
            {
                textBoxMqttSubTopic.Text = ConfigHelper.GetConfig("MqttSubTopic");
            }
            else
            {
                ConfigHelper.SetConfig("MqttSubTopic", textBoxMqttSubTopic.Text);
            }

            if (ConfigHelper.Check("CmdProdxy"))
            {
                var tmp = ConfigHelper.GetConfig("CmdProdxy");
                comboBoxCmdProdxy.SelectedIndex = comboBoxCmdProdxy.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("CmdProdxy", comboBoxCmdProdxy.Text);
            }

            if (ConfigHelper.Check("MqttClient_Checked"))
            {
                radioButtonMqttClient.Checked = DgiotHelper.StrTobool(ConfigHelper.GetConfig("MqttClient_Checked"));
            }
            else
            {
                ConfigHelper.SetConfig("MqttClient_Checked", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
            }
        }

        private void RestoreTcp()
        {
            if (ConfigHelper.Check("TcpClientLogin"))
            {
                textBoxTcpClientLogin.Text = ConfigHelper.GetConfig("TcpClientLogin");
            }
            else
            {
                ConfigHelper.SetConfig("TcpClientLogin", textBoxTcpClientLogin.Text);
            }

            if (ConfigHelper.Check("TcpClient_Checked"))
            {
                radioButtonTcpClient.Checked = DgiotHelper.StrTobool(ConfigHelper.GetConfig("TcpClient_Checked"));
            }
            else
            {
                ConfigHelper.SetConfig("TcpClient_Checked", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
            }
        }

        private void RestoreUDP()
        {
            if (ConfigHelper.Check("UDPClientLogin"))
            {
                textBoxUDPClientLogin.Text = ConfigHelper.GetConfig("UDPClientLogin");
            }
            else
            {
                ConfigHelper.SetConfig("UDPClientLogin", textBoxUDPClientLogin.Text);
            }

            if (ConfigHelper.Check("UDPClient_Checked"))
            {
                radioButtonUDPClient.Checked = DgiotHelper.StrTobool(ConfigHelper.GetConfig("UDPClient_Checked"));
            }
            else
            {
                ConfigHelper.SetConfig("UDPClient_Checked", DgiotHelper.BoolTostr(radioButtonUDPClient.Checked));
            }
        }

        private void RestoreSerialPort()
        {
            if (ConfigHelper.Check("SerialPort"))
            {
                var tmp = ConfigHelper.GetConfig("SerialPort");
                comboBoxSerialPort.SelectedIndex = comboBoxSerialPort.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("SerialPort", comboBoxSerialPort.Text);
            }

            if (ConfigHelper.Check("BaudRate"))
            {
                var tmp = ConfigHelper.GetConfig("BaudRate");
                comboBoxBaudRate.SelectedIndex = comboBoxBaudRate.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("SerialPort", comboBoxBaudRate.Text);
            }

            if (ConfigHelper.Check("DataBits"))
            {
                var tmp = ConfigHelper.GetConfig("DataBits");
                comboBoxDataBits.SelectedIndex = comboBoxDataBits.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("DataBits", comboBoxDataBits.Text);
            }

            if (ConfigHelper.Check("Parity"))
            {
                var tmp = ConfigHelper.GetConfig("Parity");
                comboBoxParity.SelectedIndex = comboBoxParity.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("Parity", comboBoxParity.Text);
            }

            if (ConfigHelper.Check("StopBits"))
            {
                var tmp = ConfigHelper.GetConfig("StopBits");
                comboBoxStopBits.SelectedIndex = comboBoxStopBits.Items.IndexOf(tmp);
            }
            else
            {
                ConfigHelper.SetConfig("StopBits", comboBoxStopBits.Text);
            }
        }

        private void RestorePLC()
        {
            if (ConfigHelper.Check("PLCTopic"))
            {
                textBoxPLCTopic.Text = ConfigHelper.GetConfig("PLCTopic");
            }
            else
            {
                ConfigHelper.SetConfig("PLCTopic", textBoxPLCTopic.Text);
            }
        }

        private void RestoreOPCDA()
        {
            if (ConfigHelper.Check("OPCDAHost"))
            {
                textBoxOPCDAHost.Text = ConfigHelper.GetConfig("OPCDAHost");
            }
            else
            {
                ConfigHelper.SetConfig("OPCDAHost", textBoxOPCDAHost.Text);
            }

            if (ConfigHelper.Check("OPCDAInterval"))
            {
                textBoxOPCDAInterval.Text = ConfigHelper.GetConfig("OPCDAInterval");
            }
            else
            {
                ConfigHelper.SetConfig("OPCDAInterval", textBoxOPCDAInterval.Text);
            }

            if (ConfigHelper.Check("OPCDACheck"))
            {
                checkBoxOPCDA.Text = ConfigHelper.GetConfig("OPCDACheck");
            }
            else
            {
                ConfigHelper.SetConfig("OPCDACheck", checkBoxOPCDA.Text);
            }
        }

        private void RestoreOPCUA()
        {
            if (ConfigHelper.Check("OPCUATopic"))
            {
                textBoxOPCUATopic.Text = ConfigHelper.GetConfig("OPCUATopic");
            }
            else
            {
                ConfigHelper.SetConfig("OPCUATopic", textBoxOPCUATopic.Text);
            }
        }

        private void RestoreBACnet()
        {
            if (ConfigHelper.Check("BACnetTopic"))
            {
                textBoxBACnetTopic.Text = ConfigHelper.GetConfig("BACnetTopic");
            }
            else
            {
                ConfigHelper.SetConfig("BACnetTopic", textBoxBACnetTopic.Text);
            }
        }

        private void RestoreControl()
        {
            if (ConfigHelper.Check("ControlTopic"))
            {
                textBoxControlTopic.Text = ConfigHelper.GetConfig("ControlTopic");
            }
            else
            {
                ConfigHelper.SetConfig("ControlTopic", textBoxControlTopic.Text);
            }
        }

        private void RestoreAccess()
        {
            if (ConfigHelper.Check("AccessTopic"))
            {
                textBoxAccessTopic.Text = ConfigHelper.GetConfig("AccessTopic");
            }
            else
            {
                ConfigHelper.SetConfig("AccessTopic", textBoxAccessTopic.Text);
            }
        }

        private void Resh_Config()
        {
            MqttClientHelper.Config();
            MqttServerHelper.Config();
            TcpClientHelper.Config();
            TcpServerHelper.Config(config.AppSettings.Settings);
            UDPClientHelper.Config();
            UDPServerHelper.Config(config.AppSettings.Settings);

            SerialPortHelper.Config(config.AppSettings.Settings);
            PLCHelper.Config(config.AppSettings.Settings);
            OPCDAHelper.Config();
            OPCUAHelper.Config(config.AppSettings.Settings);
            AccessHelper.Config(config.AppSettings.Settings);
            SqlServerHelper.Config(config.AppSettings.Settings);
        }

        private void Resh_Topic()
        {
            devaddr = comboBoxDtuAddr.Text;
            productid = textBoxMqttUserName.Text;
            clientid = DgiotHelper.Md5("Device" + this.textBoxMqttUserName.Text + devaddr).Substring(0, 10);
            textBoxMqttClientId.Text = clientid;
            textBoxMqttSubTopic.Text = "/" + productid + "/" + devaddr;
            textBoxMqttPubTopic.Text = "/" + productid + "/" + devaddr + "/properties/read/reply";
            textBoxAccessTopic.Text = "/" + productid + "/" + devaddr + "/scan/mdb";

            textBoxOPCUATopic.Text = "/" + productid + "/" + devaddr + "/scan/opcua/reply";
            textBoxPLCTopic.Text = "/" + productid + "/" + devaddr + "/scan/plc/reply";
            textBoxBACnetTopic.Text = "/" + productid + "/" + devaddr + "/scan/bacnet/reply";
            textBoxControlTopic.Text = "/" + productid + "/" + devaddr + "/scan/control/reply";
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
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void CheckBoxReconnectCheckedChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("ReconnectChecked", DgiotHelper.BoolTostr(checkBoxReconnect.Checked));
        }

        private void CheckBoxDisplayHexCheckedChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("DisplayHex", DgiotHelper.BoolTostr(checkBoxDisplayHex.Checked));
        }

        private void RadioButtonMqttClient_CheckedChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("MqttClient_Checked", DgiotHelper.BoolTostr(radioButtonMqttClient.Checked));
        }

        private void RadioButtonTcpClient_CheckedChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("TcpClient_Checked", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
        }

        private void RadioButtonUDPClient_CheckedChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("UDPClient_Checked", DgiotHelper.BoolTostr(radioButtonTcpClient.Checked));
        }

        private void TextToPayload_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("ToPayload", textToPayload.Text);
        }

        private void TextBoxMqttClientId_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("MqttClientId", textBoxMqttClientId.Text);
        }

        private void TextBoxMqttUserName_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("MqttUserName", textBoxMqttUserName.Text);
        }

        private void TextBoxMqttPassword_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("MqttPassword", textBoxMqttPassword.Text);
        }

        private void TextBoxMqttSubTopic_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("MqttSubTopic", textBoxMqttSubTopic.Text);
        }

        private void TextBoxMqttPubTopic_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("MqttPubTopic", textBoxMqttPubTopic.Text);
        }

        private void TextBoxMqttServerPort_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("MqttServerPort", textBoxTcpClientLogin.Text);
        }

        private void TextBoxTcpClientLogin_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("TcpClientLogin", textBoxTcpClientLogin.Text);
        }

        private void TextBoxUDPClientLogin_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("UDPClientLogin", textBoxUDPClientLogin.Text);
        }

        private void ComboBoxCmdProdxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("CmdProdxy", comboBoxCmdProdxy.Text);
        }

        private void ComboBoxStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("StopBits", comboBoxStopBits.Text);
        }

        private void ComboBoxParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("Parity", comboBoxParity.Text);
        }

        private void ComboBoxDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("DataBits", comboBoxDataBits.Text);
        }

        private void TextBoxPLCTopic_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("PLCTopic", textBoxPLCTopic.Text);
        }

        private void CheckBoxOPCDA_CheckedChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("OPCDACheck", DgiotHelper.BoolTostr(checkBoxOPCDA.Checked));
            OPCDAHelper.StartMonitor();
        }

        private void TextBoxOPCDAInterval_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("OPCDAInterval", textBoxOPCDAInterval.Text);
        }

        private void TextBoxOPCDAHost_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("OPCDAHost", textBoxOPCDAHost.Text);
        }

        private void TextBoxOPCUATopic_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("OPCUATopic", textBoxOPCUATopic.Text);
        }

        private void TextBoxBACnetTopic_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("BACnetTopic", textBoxBACnetTopic.Text);
        }

        private void TextBoxControlTopic_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("ControlTopic", textBoxControlTopic.Text);
        }

        private void TextBoxAccessTopic_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("AccessTopic", textBoxAccessTopic.Text);
        }

        private void ComboBoxLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("LogLevel", comboBoxLogLevel.Text);
            LogHelper.SetLevel(comboBoxLogLevel.SelectedIndex);
        }

        private void ComboBoxBridge_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("Bridge", comboBoxBridge.Text);
        }

        private void TextBoxDgiotSever_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("DgiotSever", textBoxDgiotSever.Text);
        }

        private void TextBoxDgiotPort_TextChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("DgiotPort", textBoxDgiotPort.Text);
        }

        private void ComboBoxDtuAddr_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("DtuAddr", comboBoxDtuAddr.Text);
        }

        private void CheckBoxBridge_CheckedChanged(object sender, EventArgs e)
        {
            ConfigHelper.SetConfig("Bridge_Checked", DgiotHelper.BoolTostr(checkBoxBridge.Checked));
        }

        private void SendBridge_Click(object sender, EventArgs e)
        {
            byte[] payload = LogHelper.Payload(textToPayload.Text.ToCharArray());
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

        private void Language(string lan)
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
            label33.Text = "语言";
            label32.Text = "日志级别";

            label7.Text = "发至";
            label2.Text = "发至";
            labelopcua.Text = "发至";
            label23.Text = "发至";
            label8.Text = "发至";
            label6.Text = "发至";
            labelOPCDAHost.Text = "主机";

            groupBoxSerialPort.Text = "串口扫描";
            groupBox12.Text = "PLC扫描";
            groupBox4.Text = "OPC_DA扫描";
            groupBox5.Text = "OPC_UA扫描";
            groupBox6.Text = "BACnet扫描";
            groupBox7.Text = "窗体扫描";
            groupBox8.Text = "Access扫描";

            labelSerialPort.Text = "端口";
            label1.Text = "波特率";
            label4.Text = "数据位";
            label13.Text = "校验位";
            label5.Text = "停止位";

            groupBox3.Text = "Mqtt 客户端通道";
            label22.Text = "服务器地址";
            label21.Text = "服务器端口";
            label9.Text = "登录用户";
            label10.Text = "登录密码";
            label30.Text = "网关地址";
            label20.Text = "客户编号";
            label11.Text = "订阅主题";
            label12.Text = "发布主题";
            label29.Text = "命令代理";

            groupBox2.Text = "TCP 客户端通道";
            label16.Text = "登录报文";

            groupBox10.Text = "UDP 客户端通道";
            label15.Text = "登录报文";

            label_devcietree.Text = "设备树";
            checkBoxOPCDA.Text = "主动上报";
            labelOPCDAMonitor.Text = "采集间隔";
            labelSecond.Text = "秒";
            checkBoxBridge.Text = "桥接端口";
        }

        private void En()
        {
            sendBridge.Text = "Send";
            buttonClear.Text = "Clear";
            buttonStartStop.Text = "Start";
            label32.Text = "Level";

            checkBoxReconnect.Text = "Auto Reconnect";
            label33.Text = "Language";

            label7.Text = "To";
            label2.Text = "To";
            labelopcua.Text = "To";
            label23.Text = "To";
            label8.Text = "To";
            label6.Text = "To";
            labelOPCDAHost.Text = "Host";

            groupBoxSerialPort.Text = "Serial Port Capture";
            groupBox12.Text = "PLC Capture";
            groupBox4.Text = "OPC_DA Capture";
            groupBox5.Text = "OPC_UA Capture";
            groupBox6.Text = "BACnet Capture";
            groupBox7.Text = "Control Capture";
            groupBox8.Text = "Access Capture";

            labelSerialPort.Text = "Port";
            label1.Text = "Baud Rate";
            label4.Text = "dataBits";
            label13.Text = "Parity";
            label5.Text = "stopBits";
            labelOPCDAHost.Text = "Host";

            groupBox3.Text = "Mqtt Client Channel";
            label22.Text = "Server";
            label21.Text = "Port";
            label9.Text = "UserName";
            label10.Text = "PassWord";
            label30.Text = "DtuAddr";
            label20.Text = "Clientid";
            label11.Text = "SubTopic";
            label12.Text = "PubTopic";
            label29.Text = "cmd proxy";

            groupBox2.Text = "TCP Client Channel";
            label16.Text = "login";

            groupBox10.Text = "UDP Client Channel";
            label15.Text = "login";

            label_devcietree.Text = "DeviceTree";

            checkBoxOPCDA.Text = "Monitor";
            labelOPCDAMonitor.Text = "Interval";
            labelSecond.Text = "Second";
            checkBoxBridge.Text = "Bridge Port";
        }
    }
}
