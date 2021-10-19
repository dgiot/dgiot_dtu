namespace Dgiot_dtu
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonStartStop = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkBoxReconnect = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayHex = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.groupBoxSerialPort = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.comboBoxDataBits = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.labelSerialPort = new System.Windows.Forms.Label();
            this.comboBoxSerialPort = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textPubTopic = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textSubTopic = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButtonMqtt = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.textlogin = new System.Windows.Forms.TextBox();
            this.textBoxReadOnlyPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTargetPort = new System.Windows.Forms.TextBox();
            this.labelTargetPort = new System.Windows.Forms.Label();
            this.textBoxIPAddress = new System.Windows.Forms.TextBox();
            this.checkBoxTelnet = new System.Windows.Forms.CheckBox();
            this.labelTargetIP = new System.Windows.Forms.Label();
            this.radioButtonServer = new System.Windows.Forms.RadioButton();
            this.radioButtonClient = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textCom = new System.Windows.Forms.TextBox();
            this.textNet = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.sendcom = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.sendnet = new System.Windows.Forms.Button();
            this.groupBoxSerialPort.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Location = new System.Drawing.Point(1130, 37);
            this.buttonStartStop.Margin = new System.Windows.Forms.Padding(4);
            this.buttonStartStop.Name = "buttonStartStop";
            this.buttonStartStop.Size = new System.Drawing.Size(112, 32);
            this.buttonStartStop.TabIndex = 4;
            this.buttonStartStop.Text = "Start";
            this.buttonStartStop.UseVisualStyleBackColor = true;
            this.buttonStartStop.Click += new System.EventHandler(this.ButtonStartStopClick);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(18, 359);
            this.textBoxLog.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(1286, 344);
            this.textBoxLog.TabIndex = 9;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.Silver;
            this.linkLabel1.Location = new System.Drawing.Point(550, 721);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(89, 18);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "dgiot Ltd";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1LinkClicked);
            // 
            // checkBoxReconnect
            // 
            this.checkBoxReconnect.AutoSize = true;
            this.checkBoxReconnect.Location = new System.Drawing.Point(1130, 141);
            this.checkBoxReconnect.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxReconnect.Name = "checkBoxReconnect";
            this.checkBoxReconnect.Size = new System.Drawing.Size(160, 22);
            this.checkBoxReconnect.TabIndex = 5;
            this.checkBoxReconnect.Text = "Auto Reconnect";
            this.checkBoxReconnect.UseVisualStyleBackColor = true;
            this.checkBoxReconnect.CheckedChanged += new System.EventHandler(this.CheckBoxReconnectCheckedChanged);
            // 
            // checkBoxDisplayHex
            // 
            this.checkBoxDisplayHex.AutoSize = true;
            this.checkBoxDisplayHex.Location = new System.Drawing.Point(140, 186);
            this.checkBoxDisplayHex.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxDisplayHex.Name = "checkBoxDisplayHex";
            this.checkBoxDisplayHex.Size = new System.Drawing.Size(106, 22);
            this.checkBoxDisplayHex.TabIndex = 11;
            this.checkBoxDisplayHex.Text = "十六进制";
            this.checkBoxDisplayHex.UseVisualStyleBackColor = true;
            this.checkBoxDisplayHex.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayHexCheckedChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(1130, 187);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(112, 32);
            this.buttonClear.TabIndex = 12;
            this.buttonClear.Text = "Clear Log";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClearClick);
            // 
            // groupBoxSerialPort
            // 
            this.groupBoxSerialPort.Controls.Add(this.label5);
            this.groupBoxSerialPort.Controls.Add(this.label4);
            this.groupBoxSerialPort.Controls.Add(this.label1);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxStopBits);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxDataBits);
            this.groupBoxSerialPort.Controls.Add(this.checkBoxDisplayHex);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxBaudRate);
            this.groupBoxSerialPort.Controls.Add(this.labelSerialPort);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxSerialPort);
            this.groupBoxSerialPort.Location = new System.Drawing.Point(18, 18);
            this.groupBoxSerialPort.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxSerialPort.Name = "groupBoxSerialPort";
            this.groupBoxSerialPort.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxSerialPort.Size = new System.Drawing.Size(246, 218);
            this.groupBoxSerialPort.TabIndex = 13;
            this.groupBoxSerialPort.TabStop = false;
            this.groupBoxSerialPort.Text = "Serial Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 146);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 18);
            this.label5.TabIndex = 12;
            this.label5.Text = "stopBits";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 106);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 18);
            this.label4.TabIndex = 12;
            this.label4.Text = "dataBits";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "Baud Rate";
            // 
            // comboBoxStopBits
            // 
            this.comboBoxStopBits.FormattingEnabled = true;
            this.comboBoxStopBits.Items.AddRange(new object[] {
            "1",
            "2",
            "1.5"});
            this.comboBoxStopBits.Location = new System.Drawing.Point(136, 141);
            this.comboBoxStopBits.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(98, 26);
            this.comboBoxStopBits.TabIndex = 11;
            // 
            // comboBoxDataBits
            // 
            this.comboBoxDataBits.FormattingEnabled = true;
            this.comboBoxDataBits.Items.AddRange(new object[] {
            "8",
            "7",
            "6",
            "5"});
            this.comboBoxDataBits.Location = new System.Drawing.Point(136, 102);
            this.comboBoxDataBits.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDataBits.Name = "comboBoxDataBits";
            this.comboBoxDataBits.Size = new System.Drawing.Size(98, 26);
            this.comboBoxDataBits.TabIndex = 11;
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(136, 63);
            this.comboBoxBaudRate.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(98, 26);
            this.comboBoxBaudRate.TabIndex = 11;
            // 
            // labelSerialPort
            // 
            this.labelSerialPort.AutoSize = true;
            this.labelSerialPort.Location = new System.Drawing.Point(20, 30);
            this.labelSerialPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort.Name = "labelSerialPort";
            this.labelSerialPort.Size = new System.Drawing.Size(44, 18);
            this.labelSerialPort.TabIndex = 10;
            this.labelSerialPort.Text = "Port";
            // 
            // comboBoxSerialPort
            // 
            this.comboBoxSerialPort.FormattingEnabled = true;
            this.comboBoxSerialPort.Location = new System.Drawing.Point(136, 26);
            this.comboBoxSerialPort.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxSerialPort.Name = "comboBoxSerialPort";
            this.comboBoxSerialPort.Size = new System.Drawing.Size(98, 26);
            this.comboBoxSerialPort.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textPubTopic);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textSubTopic);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textPassword);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.textUserName);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.radioButtonMqtt);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textlogin);
            this.groupBox1.Controls.Add(this.textBoxReadOnlyPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxTargetPort);
            this.groupBox1.Controls.Add(this.labelTargetPort);
            this.groupBox1.Controls.Add(this.textBoxIPAddress);
            this.groupBox1.Controls.Add(this.checkBoxTelnet);
            this.groupBox1.Controls.Add(this.labelTargetIP);
            this.groupBox1.Controls.Add(this.radioButtonServer);
            this.groupBox1.Controls.Add(this.radioButtonClient);
            this.groupBox1.Location = new System.Drawing.Point(282, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(818, 218);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TCP/IP";
            // 
            // textPubTopic
            // 
            this.textPubTopic.Location = new System.Drawing.Point(461, 175);
            this.textPubTopic.Margin = new System.Windows.Forms.Padding(4);
            this.textPubTopic.Name = "textPubTopic";
            this.textPubTopic.Size = new System.Drawing.Size(336, 28);
            this.textPubTopic.TabIndex = 22;
            this.textPubTopic.Text = "thing/dgiot/post/";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(358, 182);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 18);
            this.label12.TabIndex = 21;
            this.label12.Text = "PubTopic";
            // 
            // textSubTopic
            // 
            this.textSubTopic.Location = new System.Drawing.Point(461, 136);
            this.textSubTopic.Margin = new System.Windows.Forms.Padding(4);
            this.textSubTopic.Name = "textSubTopic";
            this.textSubTopic.Size = new System.Drawing.Size(336, 28);
            this.textSubTopic.TabIndex = 20;
            this.textSubTopic.Text = "thing/dgiot/";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(358, 141);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 18);
            this.label11.TabIndex = 19;
            this.label11.Text = "SubTopic";
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(461, 94);
            this.textPassword.Margin = new System.Windows.Forms.Padding(4);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(336, 28);
            this.textPassword.TabIndex = 18;
            this.textPassword.Text = "dgiot";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(358, 101);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 18);
            this.label10.TabIndex = 17;
            this.label10.Text = "PassWord";
            // 
            // textUserName
            // 
            this.textUserName.Location = new System.Drawing.Point(461, 58);
            this.textUserName.Margin = new System.Windows.Forms.Padding(4);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(336, 28);
            this.textUserName.TabIndex = 16;
            this.textUserName.Text = "dgiot";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(358, 65);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 18);
            this.label9.TabIndex = 15;
            this.label9.Text = "UserName";
            // 
            // radioButtonMqtt
            // 
            this.radioButtonMqtt.AutoSize = true;
            this.radioButtonMqtt.Location = new System.Drawing.Point(361, 30);
            this.radioButtonMqtt.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonMqtt.Name = "radioButtonMqtt";
            this.radioButtonMqtt.Size = new System.Drawing.Size(69, 22);
            this.radioButtonMqtt.TabIndex = 14;
            this.radioButtonMqtt.Text = "Mqtt";
            this.radioButtonMqtt.UseVisualStyleBackColor = true;
            this.radioButtonMqtt.CheckedChanged += new System.EventHandler(this.radioButtonMqtt_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 185);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(134, 18);
            this.label6.TabIndex = 13;
            this.label6.Text = "login/clientid";
            // 
            // textlogin
            // 
            this.textlogin.Location = new System.Drawing.Point(196, 182);
            this.textlogin.Margin = new System.Windows.Forms.Padding(4);
            this.textlogin.Name = "textlogin";
            this.textlogin.Size = new System.Drawing.Size(148, 28);
            this.textlogin.TabIndex = 12;
            this.textlogin.Text = "12345678";
            this.toolTip1.SetToolTip(this.textlogin, "Connect to this port then you can send and receive data to/from Serial port.\r\nThe" +
        "re can only be one connection at a time.");
            // 
            // textBoxReadOnlyPort
            // 
            this.textBoxReadOnlyPort.Location = new System.Drawing.Point(195, 144);
            this.textBoxReadOnlyPort.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxReadOnlyPort.Name = "textBoxReadOnlyPort";
            this.textBoxReadOnlyPort.Size = new System.Drawing.Size(148, 28);
            this.textBoxReadOnlyPort.TabIndex = 7;
            this.textBoxReadOnlyPort.Text = "8066";
            this.toolTip1.SetToolTip(this.textBoxReadOnlyPort, "Connect to this port then you can receive data from Serial port");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 148);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 18);
            this.label2.TabIndex = 8;
            this.label2.Text = "Read-olny Port";
            // 
            // textBoxTargetPort
            // 
            this.textBoxTargetPort.Location = new System.Drawing.Point(195, 104);
            this.textBoxTargetPort.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxTargetPort.Name = "textBoxTargetPort";
            this.textBoxTargetPort.Size = new System.Drawing.Size(148, 28);
            this.textBoxTargetPort.TabIndex = 7;
            this.textBoxTargetPort.Text = "1883";
            this.toolTip1.SetToolTip(this.textBoxTargetPort, "Connect to this port then you can send and receive data to/from Serial port.\r\nThe" +
        "re can only be one connection at a time.");
            // 
            // labelTargetPort
            // 
            this.labelTargetPort.AutoSize = true;
            this.labelTargetPort.Location = new System.Drawing.Point(22, 108);
            this.labelTargetPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTargetPort.Name = "labelTargetPort";
            this.labelTargetPort.Size = new System.Drawing.Size(44, 18);
            this.labelTargetPort.TabIndex = 8;
            this.labelTargetPort.Text = "Port";
            // 
            // textBoxIPAddress
            // 
            this.textBoxIPAddress.Location = new System.Drawing.Point(195, 63);
            this.textBoxIPAddress.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxIPAddress.Name = "textBoxIPAddress";
            this.textBoxIPAddress.Size = new System.Drawing.Size(148, 28);
            this.textBoxIPAddress.TabIndex = 5;
            this.textBoxIPAddress.Text = "prod.iotn2n.com";
            // 
            // checkBoxTelnet
            // 
            this.checkBoxTelnet.AutoSize = true;
            this.checkBoxTelnet.Checked = true;
            this.checkBoxTelnet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTelnet.Location = new System.Drawing.Point(208, 27);
            this.checkBoxTelnet.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxTelnet.Name = "checkBoxTelnet";
            this.checkBoxTelnet.Size = new System.Drawing.Size(88, 22);
            this.checkBoxTelnet.TabIndex = 11;
            this.checkBoxTelnet.Text = "Telnet";
            this.toolTip1.SetToolTip(this.checkBoxTelnet, "Process client data as Telnet protocol");
            this.checkBoxTelnet.UseVisualStyleBackColor = true;
            this.checkBoxTelnet.CheckedChanged += new System.EventHandler(this.checkBoxTelnet_CheckedChanged);
            // 
            // labelTargetIP
            // 
            this.labelTargetIP.AutoSize = true;
            this.labelTargetIP.Location = new System.Drawing.Point(21, 66);
            this.labelTargetIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTargetIP.Name = "labelTargetIP";
            this.labelTargetIP.Size = new System.Drawing.Size(161, 18);
            this.labelTargetIP.TabIndex = 6;
            this.labelTargetIP.Text = "Server IP address";
            // 
            // radioButtonServer
            // 
            this.radioButtonServer.AutoSize = true;
            this.radioButtonServer.Checked = true;
            this.radioButtonServer.Location = new System.Drawing.Point(111, 27);
            this.radioButtonServer.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonServer.Name = "radioButtonServer";
            this.radioButtonServer.Size = new System.Drawing.Size(87, 22);
            this.radioButtonServer.TabIndex = 1;
            this.radioButtonServer.TabStop = true;
            this.radioButtonServer.Text = "Server";
            this.radioButtonServer.UseVisualStyleBackColor = true;
            this.radioButtonServer.CheckedChanged += new System.EventHandler(this.radioButtonServer_CheckedChanged);
            // 
            // radioButtonClient
            // 
            this.radioButtonClient.AutoSize = true;
            this.radioButtonClient.Location = new System.Drawing.Point(26, 27);
            this.radioButtonClient.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonClient.Name = "radioButtonClient";
            this.radioButtonClient.Size = new System.Drawing.Size(87, 22);
            this.radioButtonClient.TabIndex = 0;
            this.radioButtonClient.Text = "Client";
            this.radioButtonClient.UseVisualStyleBackColor = true;
            this.radioButtonClient.CheckedChanged += new System.EventHandler(this.radioButtonClient_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Silver;
            this.label3.Location = new System.Drawing.Point(471, 721);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 18);
            this.label3.TabIndex = 15;
            this.label3.Text = "Author:";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.LinkColor = System.Drawing.Color.Silver;
            this.linkLabel2.Location = new System.Drawing.Point(740, 721);
            this.linkLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(62, 18);
            this.linkLabel2.TabIndex = 16;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "DG-IoT";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 200;
            this.toolTip1.ReshowDelay = 100;
            // 
            // textCom
            // 
            this.textCom.Location = new System.Drawing.Point(143, 265);
            this.textCom.Margin = new System.Windows.Forms.Padding(4);
            this.textCom.Name = "textCom";
            this.textCom.Size = new System.Drawing.Size(987, 28);
            this.textCom.TabIndex = 14;
            this.textCom.Text = "12345678";
            this.toolTip1.SetToolTip(this.textCom, "Connect to this port then you can send and receive data to/from Serial port.\r\nThe" +
        "re can only be one connection at a time.");
            this.textCom.TextChanged += new System.EventHandler(this.TextCom_TextChanged);
            // 
            // textNet
            // 
            this.textNet.Location = new System.Drawing.Point(143, 311);
            this.textNet.Margin = new System.Windows.Forms.Padding(4);
            this.textNet.Name = "textNet";
            this.textNet.Size = new System.Drawing.Size(987, 28);
            this.textNet.TabIndex = 19;
            this.textNet.Text = "12345678";
            this.toolTip1.SetToolTip(this.textNet, "Connect to this port then you can send and receive data to/from Serial port.\r\nThe" +
        "re can only be one connection at a time.");
            this.textNet.TextChanged += new System.EventHandler(this.TextNet_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 271);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 18);
            this.label7.TabIndex = 14;
            this.label7.Text = "发到串口";
            // 
            // sendcom
            // 
            this.sendcom.Location = new System.Drawing.Point(1152, 262);
            this.sendcom.Margin = new System.Windows.Forms.Padding(4);
            this.sendcom.Name = "sendcom";
            this.sendcom.Size = new System.Drawing.Size(112, 32);
            this.sendcom.TabIndex = 17;
            this.sendcom.Text = "send";
            this.sendcom.UseVisualStyleBackColor = true;
            this.sendcom.Click += new System.EventHandler(this.sendcom_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 316);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 18);
            this.label8.TabIndex = 18;
            this.label8.Text = "发到网络";
            // 
            // sendnet
            // 
            this.sendnet.Location = new System.Drawing.Point(1152, 308);
            this.sendnet.Margin = new System.Windows.Forms.Padding(4);
            this.sendnet.Name = "sendnet";
            this.sendnet.Size = new System.Drawing.Size(112, 32);
            this.sendnet.TabIndex = 20;
            this.sendnet.Text = "send";
            this.sendnet.UseVisualStyleBackColor = true;
            this.sendnet.Click += new System.EventHandler(this.sendnet_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 761);
            this.Controls.Add(this.sendnet);
            this.Controls.Add(this.textNet);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.sendcom);
            this.Controls.Add(this.textCom);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxSerialPort);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.checkBoxReconnect);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.buttonStartStop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "dgiot_dtu";
            this.groupBoxSerialPort.ResumeLayout(false);
            this.groupBoxSerialPort.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStartStop;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox checkBoxReconnect;
        private System.Windows.Forms.CheckBox checkBoxDisplayHex;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.GroupBox groupBoxSerialPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.Label labelSerialPort;
        private System.Windows.Forms.ComboBox comboBoxSerialPort;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxTargetPort;
        private System.Windows.Forms.Label labelTargetPort;
        private System.Windows.Forms.TextBox textBoxIPAddress;
        private System.Windows.Forms.Label labelTargetIP;
        private System.Windows.Forms.RadioButton radioButtonServer;
        private System.Windows.Forms.RadioButton radioButtonClient;
        private System.Windows.Forms.TextBox textBoxReadOnlyPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxDataBits;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxStopBits;
        private System.Windows.Forms.CheckBox checkBoxTelnet;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textlogin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textCom;
        private System.Windows.Forms.Button sendcom;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textNet;
        private System.Windows.Forms.Button sendnet;
        private System.Windows.Forms.RadioButton radioButtonMqtt;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textUserName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textSubTopic;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textPubTopic;
    }
}

