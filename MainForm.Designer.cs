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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonStartStop = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkBoxReconnect = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayHex = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.groupBoxSerialPort = new System.Windows.Forms.GroupBox();
            this.comboBoxParity = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.comboBoxDataBits = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.labelSerialPort = new System.Windows.Forms.Label();
            this.comboBoxSerialPort = new System.Windows.Forms.ComboBox();
            this.textBoxTcpServerPort = new System.Windows.Forms.TextBox();
            this.labelTargetPort = new System.Windows.Forms.Label();
            this.textBoxMqttPubTopic = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxMqttSubTopic = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxMqttPassword = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxMqttUserName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.textToPayload = new System.Windows.Forms.TextBox();
            this.textBoxTcpClientLogin = new System.Windows.Forms.TextBox();
            this.textBoxTcpClientPort = new System.Windows.Forms.TextBox();
            this.textBoxMqttClientId = new System.Windows.Forms.TextBox();
            this.textBoxMqttPort = new System.Windows.Forms.TextBox();
            this.textBoxUDPClientLogin = new System.Windows.Forms.TextBox();
            this.textBoxUDPCLientPort = new System.Windows.Forms.TextBox();
            this.sendBridge = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxTcpClientServer = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkBoxTcpServer = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label29 = new System.Windows.Forms.Label();
            this.comboBoxCmdProdxy = new System.Windows.Forms.ComboBox();
            this.checkBoxMqttBridge = new System.Windows.Forms.CheckBox();
            this.textBoxMqttServerPort = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.textBoxMqttSever = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.checkBoxSerialPort = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxOPCDATopic = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.checkBoxOPCDA = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxOPCUATopic = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.checkBoxOPCUA = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxBACnetTopic = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textBoxContolTopic = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxBAnet = new System.Windows.Forms.CheckBox();
            this.checkBoxControl = new System.Windows.Forms.CheckBox();
            this.comboBoxBridge = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.textBoxMDBTopic = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.textBoxSqlServerTopic = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.checkBoxAccess = new System.Windows.Forms.CheckBox();
            this.checkBoxSqlServer = new System.Windows.Forms.CheckBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.checkBoxUdpServer = new System.Windows.Forms.CheckBox();
            this.label28 = new System.Windows.Forms.Label();
            this.textBoxUdpServerPort = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.textBoxUDPClientServer = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.radioButtonMqttClient = new System.Windows.Forms.RadioButton();
            this.radioButtonTcpClient = new System.Windows.Forms.RadioButton();
            this.radioButtonUDPClient = new System.Windows.Forms.RadioButton();
            this.checkBoxPLC = new System.Windows.Forms.CheckBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.textBoxPLCTopic = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBoxSerialPort.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Location = new System.Drawing.Point(509, 529);
            this.buttonStartStop.Name = "buttonStartStop";
            this.buttonStartStop.Size = new System.Drawing.Size(53, 21);
            this.buttonStartStop.TabIndex = 4;
            this.buttonStartStop.Text = "Start";
            this.buttonStartStop.UseVisualStyleBackColor = true;
            this.buttonStartStop.Click += new System.EventHandler(this.ButtonStartStopClick);
            // 
            // textBoxLog
            // 
            this.textBoxLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(170)))), ((int)(((byte)(55)))));
            this.textBoxLog.Location = new System.Drawing.Point(9, 15);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(670, 512);
            this.textBoxLog.TabIndex = 9;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.Silver;
            this.linkLabel1.Location = new System.Drawing.Point(739, 531);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(59, 12);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "dgiot Ltd";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1LinkClicked);
            // 
            // checkBoxReconnect
            // 
            this.checkBoxReconnect.AutoSize = true;
            this.checkBoxReconnect.Location = new System.Drawing.Point(568, 534);
            this.checkBoxReconnect.Name = "checkBoxReconnect";
            this.checkBoxReconnect.Size = new System.Drawing.Size(108, 16);
            this.checkBoxReconnect.TabIndex = 5;
            this.checkBoxReconnect.Text = "Auto Reconnect";
            this.checkBoxReconnect.UseVisualStyleBackColor = true;
            this.checkBoxReconnect.CheckedChanged += new System.EventHandler(this.CheckBoxReconnectCheckedChanged);
            // 
            // checkBoxDisplayHex
            // 
            this.checkBoxDisplayHex.AutoSize = true;
            this.checkBoxDisplayHex.Location = new System.Drawing.Point(129, 533);
            this.checkBoxDisplayHex.Name = "checkBoxDisplayHex";
            this.checkBoxDisplayHex.Size = new System.Drawing.Size(42, 16);
            this.checkBoxDisplayHex.TabIndex = 11;
            this.checkBoxDisplayHex.Text = "Hex";
            this.checkBoxDisplayHex.UseVisualStyleBackColor = true;
            this.checkBoxDisplayHex.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayHexCheckedChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(455, 531);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(48, 21);
            this.buttonClear.TabIndex = 12;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClearClick);
            // 
            // groupBoxSerialPort
            // 
            this.groupBoxSerialPort.Controls.Add(this.comboBoxParity);
            this.groupBoxSerialPort.Controls.Add(this.label13);
            this.groupBoxSerialPort.Controls.Add(this.label5);
            this.groupBoxSerialPort.Controls.Add(this.label4);
            this.groupBoxSerialPort.Controls.Add(this.label1);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxStopBits);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxDataBits);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxBaudRate);
            this.groupBoxSerialPort.Controls.Add(this.labelSerialPort);
            this.groupBoxSerialPort.Controls.Add(this.comboBoxSerialPort);
            this.groupBoxSerialPort.Location = new System.Drawing.Point(682, 20);
            this.groupBoxSerialPort.Name = "groupBoxSerialPort";
            this.groupBoxSerialPort.Size = new System.Drawing.Size(195, 135);
            this.groupBoxSerialPort.TabIndex = 13;
            this.groupBoxSerialPort.TabStop = false;
            this.groupBoxSerialPort.Text = "Serial Port Captrue";
            // 
            // comboBoxParity
            // 
            this.comboBoxParity.FormattingEnabled = true;
            this.comboBoxParity.Items.AddRange(new object[] {
            "NONE",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.comboBoxParity.Location = new System.Drawing.Point(91, 83);
            this.comboBoxParity.Name = "comboBoxParity";
            this.comboBoxParity.Size = new System.Drawing.Size(101, 20);
            this.comboBoxParity.TabIndex = 14;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 90);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 12);
            this.label13.TabIndex = 13;
            this.label13.Text = "Parity";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "stopBits";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "dataBits";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
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
            this.comboBoxStopBits.Location = new System.Drawing.Point(91, 108);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(101, 20);
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
            this.comboBoxDataBits.Location = new System.Drawing.Point(91, 59);
            this.comboBoxDataBits.Name = "comboBoxDataBits";
            this.comboBoxDataBits.Size = new System.Drawing.Size(101, 20);
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
            this.comboBoxBaudRate.Location = new System.Drawing.Point(91, 36);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(101, 20);
            this.comboBoxBaudRate.TabIndex = 11;
            // 
            // labelSerialPort
            // 
            this.labelSerialPort.AutoSize = true;
            this.labelSerialPort.Location = new System.Drawing.Point(13, 16);
            this.labelSerialPort.Name = "labelSerialPort";
            this.labelSerialPort.Size = new System.Drawing.Size(29, 12);
            this.labelSerialPort.TabIndex = 10;
            this.labelSerialPort.Text = "Port";
            // 
            // comboBoxSerialPort
            // 
            this.comboBoxSerialPort.FormattingEnabled = true;
            this.comboBoxSerialPort.Location = new System.Drawing.Point(91, 14);
            this.comboBoxSerialPort.Name = "comboBoxSerialPort";
            this.comboBoxSerialPort.Size = new System.Drawing.Size(101, 20);
            this.comboBoxSerialPort.TabIndex = 9;
            // 
            // textBoxTcpServerPort
            // 
            this.textBoxTcpServerPort.Location = new System.Drawing.Point(74, 98);
            this.textBoxTcpServerPort.Name = "textBoxTcpServerPort";
            this.textBoxTcpServerPort.Size = new System.Drawing.Size(136, 21);
            this.textBoxTcpServerPort.TabIndex = 7;
            this.textBoxTcpServerPort.Text = "5080";
            this.textBoxTcpServerPort.TextChanged += new System.EventHandler(this.TextBoxTcpServerPort_TextChanged);
            // 
            // labelTargetPort
            // 
            this.labelTargetPort.AutoSize = true;
            this.labelTargetPort.Location = new System.Drawing.Point(19, 100);
            this.labelTargetPort.Name = "labelTargetPort";
            this.labelTargetPort.Size = new System.Drawing.Size(41, 12);
            this.labelTargetPort.TabIndex = 8;
            this.labelTargetPort.Text = "bridge";
            // 
            // textBoxMqttPubTopic
            // 
            this.textBoxMqttPubTopic.Location = new System.Drawing.Point(73, 181);
            this.textBoxMqttPubTopic.Name = "textBoxMqttPubTopic";
            this.textBoxMqttPubTopic.Size = new System.Drawing.Size(137, 21);
            this.textBoxMqttPubTopic.TabIndex = 22;
            this.textBoxMqttPubTopic.Text = "thing/dgiot/post";
            this.textBoxMqttPubTopic.TextChanged += new System.EventHandler(this.TextBoxMqttPubTopic_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 185);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 21;
            this.label12.Text = "PubTopic";
            // 
            // textBoxMqttSubTopic
            // 
            this.textBoxMqttSubTopic.Location = new System.Drawing.Point(74, 153);
            this.textBoxMqttSubTopic.Name = "textBoxMqttSubTopic";
            this.textBoxMqttSubTopic.Size = new System.Drawing.Size(134, 21);
            this.textBoxMqttSubTopic.TabIndex = 20;
            this.textBoxMqttSubTopic.Text = "thing/dgiot";
            this.textBoxMqttSubTopic.TextChanged += new System.EventHandler(this.TextBoxMqttSubTopic_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 159);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 19;
            this.label11.Text = "SubTopic";
            // 
            // textBoxMqttPassword
            // 
            this.textBoxMqttPassword.Location = new System.Drawing.Point(73, 126);
            this.textBoxMqttPassword.Name = "textBoxMqttPassword";
            this.textBoxMqttPassword.Size = new System.Drawing.Size(136, 21);
            this.textBoxMqttPassword.TabIndex = 18;
            this.textBoxMqttPassword.Text = "dgiot";
            this.textBoxMqttPassword.TextChanged += new System.EventHandler(this.TextBoxMqttPassword_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 130);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "PassWord";
            // 
            // textBoxMqttUserName
            // 
            this.textBoxMqttUserName.Location = new System.Drawing.Point(75, 97);
            this.textBoxMqttUserName.Name = "textBoxMqttUserName";
            this.textBoxMqttUserName.Size = new System.Drawing.Size(134, 21);
            this.textBoxMqttUserName.TabIndex = 16;
            this.textBoxMqttUserName.Text = "dgiot";
            this.textBoxMqttUserName.TextChanged += new System.EventHandler(this.TextBoxMqttUserName_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 102);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 15;
            this.label9.Text = "UserName";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Silver;
            this.label3.Location = new System.Drawing.Point(690, 532);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "Author:";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.LinkColor = System.Drawing.Color.Silver;
            this.linkLabel2.Location = new System.Drawing.Point(838, 530);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(41, 12);
            this.linkLabel2.TabIndex = 16;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "DG-IoT";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel2_LinkClicked);
            // 
            // textToPayload
            // 
            this.textToPayload.Location = new System.Drawing.Point(177, 532);
            this.textToPayload.Name = "textToPayload";
            this.textToPayload.Size = new System.Drawing.Size(219, 21);
            this.textToPayload.TabIndex = 14;
            this.textToPayload.Text = "12345678";
            this.textToPayload.TextChanged += new System.EventHandler(this.TextToPayload_TextChanged);
            // 
            // textBoxTcpClientLogin
            // 
            this.textBoxTcpClientLogin.Location = new System.Drawing.Point(75, 72);
            this.textBoxTcpClientLogin.Name = "textBoxTcpClientLogin";
            this.textBoxTcpClientLogin.Size = new System.Drawing.Size(134, 21);
            this.textBoxTcpClientLogin.TabIndex = 12;
            this.textBoxTcpClientLogin.Text = "login";
            this.textBoxTcpClientLogin.TextChanged += new System.EventHandler(this.TextBoxTcpClientLogin_TextChanged);
            // 
            // textBoxTcpClientPort
            // 
            this.textBoxTcpClientPort.Location = new System.Drawing.Point(73, 45);
            this.textBoxTcpClientPort.Name = "textBoxTcpClientPort";
            this.textBoxTcpClientPort.Size = new System.Drawing.Size(136, 21);
            this.textBoxTcpClientPort.TabIndex = 7;
            this.textBoxTcpClientPort.Text = "5080";
            this.textBoxTcpClientPort.TextChanged += new System.EventHandler(this.TextBoxTcpClientPort_TextChanged);
            // 
            // textBoxMqttClientId
            // 
            this.textBoxMqttClientId.Location = new System.Drawing.Point(75, 70);
            this.textBoxMqttClientId.Name = "textBoxMqttClientId";
            this.textBoxMqttClientId.Size = new System.Drawing.Size(134, 21);
            this.textBoxMqttClientId.TabIndex = 12;
            this.textBoxMqttClientId.Text = "login";
            this.textBoxMqttClientId.TextChanged += new System.EventHandler(this.TextBoxMqttClientId_TextChanged);
            // 
            // textBoxMqttPort
            // 
            this.textBoxMqttPort.Location = new System.Drawing.Point(73, 44);
            this.textBoxMqttPort.Name = "textBoxMqttPort";
            this.textBoxMqttPort.Size = new System.Drawing.Size(136, 21);
            this.textBoxMqttPort.TabIndex = 7;
            this.textBoxMqttPort.Text = "1883";
            this.textBoxMqttPort.TextChanged += new System.EventHandler(this.TextBoxMqttPort_TextChanged);
            // 
            // textBoxUDPClientLogin
            // 
            this.textBoxUDPClientLogin.Location = new System.Drawing.Point(77, 70);
            this.textBoxUDPClientLogin.Name = "textBoxUDPClientLogin";
            this.textBoxUDPClientLogin.Size = new System.Drawing.Size(134, 21);
            this.textBoxUDPClientLogin.TabIndex = 12;
            this.textBoxUDPClientLogin.Text = "login";
            this.textBoxUDPClientLogin.TextChanged += new System.EventHandler(this.TextBoxUDPClientLogin_TextChanged);
            // 
            // textBoxUDPCLientPort
            // 
            this.textBoxUDPCLientPort.Location = new System.Drawing.Point(76, 43);
            this.textBoxUDPCLientPort.Name = "textBoxUDPCLientPort";
            this.textBoxUDPCLientPort.Size = new System.Drawing.Size(136, 21);
            this.textBoxUDPCLientPort.TabIndex = 7;
            this.textBoxUDPCLientPort.Text = "6080";
            this.textBoxUDPCLientPort.TextChanged += new System.EventHandler(this.TextBoxUDPCLientPort_TextChanged);
            // 
            // sendBridge
            // 
            this.sendBridge.Location = new System.Drawing.Point(402, 531);
            this.sendBridge.Name = "sendBridge";
            this.sendBridge.Size = new System.Drawing.Size(47, 21);
            this.sendBridge.TabIndex = 17;
            this.sendBridge.Text = "send";
            this.sendBridge.UseVisualStyleBackColor = true;
            this.sendBridge.Click += new System.EventHandler(this.SendBridge_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelTargetPort);
            this.groupBox2.Controls.Add(this.textBoxTcpServerPort);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.textBoxTcpClientLogin);
            this.groupBox2.Controls.Add(this.textBoxTcpClientPort);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.textBoxTcpClientServer);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.checkBoxTcpServer);
            this.groupBox2.Location = new System.Drawing.Point(1021, 281);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(223, 127);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "TCP Client Channel";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(17, 73);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(35, 12);
            this.label16.TabIndex = 13;
            this.label16.Text = "login";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(17, 49);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 12);
            this.label17.TabIndex = 8;
            this.label17.Text = "Port";
            // 
            // textBoxTcpClientServer
            // 
            this.textBoxTcpClientServer.Location = new System.Drawing.Point(73, 20);
            this.textBoxTcpClientServer.Name = "textBoxTcpClientServer";
            this.textBoxTcpClientServer.Size = new System.Drawing.Size(136, 21);
            this.textBoxTcpClientServer.TabIndex = 5;
            this.textBoxTcpClientServer.Text = "prod.iotn2n.com";
            this.textBoxTcpClientServer.TextChanged += new System.EventHandler(this.TextBoxTcpClientServer_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(17, 23);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(41, 12);
            this.label18.TabIndex = 6;
            this.label18.Text = "Server";
            // 
            // checkBoxTcpServer
            // 
            this.checkBoxTcpServer.AutoSize = true;
            this.checkBoxTcpServer.Location = new System.Drawing.Point(5, 99);
            this.checkBoxTcpServer.Name = "checkBoxTcpServer";
            this.checkBoxTcpServer.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTcpServer.TabIndex = 23;
            this.checkBoxTcpServer.UseVisualStyleBackColor = true;
            this.checkBoxTcpServer.CheckedChanged += new System.EventHandler(this.CheckBoxTcpServer_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label29);
            this.groupBox3.Controls.Add(this.comboBoxCmdProdxy);
            this.groupBox3.Controls.Add(this.checkBoxMqttBridge);
            this.groupBox3.Controls.Add(this.textBoxMqttServerPort);
            this.groupBox3.Controls.Add(this.label27);
            this.groupBox3.Controls.Add(this.textBoxMqttPubTopic);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.textBoxMqttClientId);
            this.groupBox3.Controls.Add(this.textBoxMqttSubTopic);
            this.groupBox3.Controls.Add(this.textBoxMqttPort);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.textBoxMqttSever);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.textBoxMqttUserName);
            this.groupBox3.Controls.Add(this.textBoxMqttPassword);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(1021, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(217, 259);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mqtt Client Channel";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(12, 211);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(59, 12);
            this.label29.TabIndex = 37;
            this.label29.Text = "cmd proxy";
            // 
            // comboBoxCmdProdxy
            // 
            this.comboBoxCmdProdxy.FormattingEnabled = true;
            this.comboBoxCmdProdxy.Items.AddRange(new object[] {
            "ffmpeg"});
            this.comboBoxCmdProdxy.Location = new System.Drawing.Point(74, 208);
            this.comboBoxCmdProdxy.Name = "comboBoxCmdProdxy";
            this.comboBoxCmdProdxy.Size = new System.Drawing.Size(134, 20);
            this.comboBoxCmdProdxy.TabIndex = 36;
            this.comboBoxCmdProdxy.SelectedIndexChanged += new System.EventHandler(this.ComboBoxCmdProdxy_SelectedIndexChanged);
            // 
            // checkBoxMqttBridge
            // 
            this.checkBoxMqttBridge.AutoSize = true;
            this.checkBoxMqttBridge.Location = new System.Drawing.Point(5, 232);
            this.checkBoxMqttBridge.Name = "checkBoxMqttBridge";
            this.checkBoxMqttBridge.Size = new System.Drawing.Size(15, 14);
            this.checkBoxMqttBridge.TabIndex = 36;
            this.checkBoxMqttBridge.UseVisualStyleBackColor = true;
            this.checkBoxMqttBridge.CheckedChanged += new System.EventHandler(this.CheckBoxMqttBridge_CheckedChanged);
            // 
            // textBoxMqttServerPort
            // 
            this.textBoxMqttServerPort.Location = new System.Drawing.Point(75, 232);
            this.textBoxMqttServerPort.Name = "textBoxMqttServerPort";
            this.textBoxMqttServerPort.Size = new System.Drawing.Size(136, 21);
            this.textBoxMqttServerPort.TabIndex = 24;
            this.textBoxMqttServerPort.Text = "1883";
            this.textBoxMqttServerPort.TextChanged += new System.EventHandler(this.TextBoxMqttServerPort_TextChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(24, 233);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(41, 12);
            this.label27.TabIndex = 23;
            this.label27.Text = "bridge";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(12, 75);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 12);
            this.label20.TabIndex = 13;
            this.label20.Text = "clientid";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(13, 48);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(29, 12);
            this.label21.TabIndex = 8;
            this.label21.Text = "Port";
            // 
            // textBoxMqttSever
            // 
            this.textBoxMqttSever.Location = new System.Drawing.Point(75, 19);
            this.textBoxMqttSever.Name = "textBoxMqttSever";
            this.textBoxMqttSever.Size = new System.Drawing.Size(134, 21);
            this.textBoxMqttSever.TabIndex = 5;
            this.textBoxMqttSever.Text = "prod.iotn2n.com";
            this.textBoxMqttSever.TextChanged += new System.EventHandler(this.TextBoxMqttSever_TextChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(13, 22);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(41, 12);
            this.label22.TabIndex = 6;
            this.label22.Text = "Server";
            // 
            // checkBoxSerialPort
            // 
            this.checkBoxSerialPort.AutoSize = true;
            this.checkBoxSerialPort.Location = new System.Drawing.Point(884, 44);
            this.checkBoxSerialPort.Name = "checkBoxSerialPort";
            this.checkBoxSerialPort.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSerialPort.TabIndex = 22;
            this.checkBoxSerialPort.UseVisualStyleBackColor = true;
            this.checkBoxSerialPort.CheckedChanged += new System.EventHandler(this.CheckBoxSerialPort_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxOPCDATopic);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Location = new System.Drawing.Point(686, 212);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(195, 49);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "OPC_DA Capture";
            // 
            // textBoxOPCDATopic
            // 
            this.textBoxOPCDATopic.Location = new System.Drawing.Point(35, 10);
            this.textBoxOPCDATopic.Name = "textBoxOPCDATopic";
            this.textBoxOPCDATopic.ReadOnly = true;
            this.textBoxOPCDATopic.Size = new System.Drawing.Size(155, 21);
            this.textBoxOPCDATopic.TabIndex = 5;
            this.textBoxOPCDATopic.Text = "thing/opdda/clientid";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(7, 15);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(17, 12);
            this.label25.TabIndex = 6;
            this.label25.Text = "To";
            // 
            // checkBoxOPCDA
            // 
            this.checkBoxOPCDA.AutoSize = true;
            this.checkBoxOPCDA.Location = new System.Drawing.Point(884, 215);
            this.checkBoxOPCDA.Name = "checkBoxOPCDA";
            this.checkBoxOPCDA.Size = new System.Drawing.Size(15, 14);
            this.checkBoxOPCDA.TabIndex = 24;
            this.checkBoxOPCDA.UseVisualStyleBackColor = true;
            this.checkBoxOPCDA.CheckedChanged += new System.EventHandler(this.CheckBoxOPCDA_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxOPCUATopic);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Location = new System.Drawing.Point(686, 261);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(195, 49);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "OPC_UA Capture";
            // 
            // textBoxOPCUATopic
            // 
            this.textBoxOPCUATopic.Location = new System.Drawing.Point(35, 16);
            this.textBoxOPCUATopic.Name = "textBoxOPCUATopic";
            this.textBoxOPCUATopic.ReadOnly = true;
            this.textBoxOPCUATopic.Size = new System.Drawing.Size(156, 21);
            this.textBoxOPCUATopic.TabIndex = 5;
            this.textBoxOPCUATopic.Text = "thing/opdua/clientid";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(4, 19);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 12);
            this.label19.TabIndex = 6;
            this.label19.Text = "To";
            // 
            // checkBoxOPCUA
            // 
            this.checkBoxOPCUA.AutoSize = true;
            this.checkBoxOPCUA.Location = new System.Drawing.Point(883, 266);
            this.checkBoxOPCUA.Name = "checkBoxOPCUA";
            this.checkBoxOPCUA.Size = new System.Drawing.Size(15, 14);
            this.checkBoxOPCUA.TabIndex = 25;
            this.checkBoxOPCUA.UseVisualStyleBackColor = true;
            this.checkBoxOPCUA.CheckedChanged += new System.EventHandler(this.CheckBoxOPCUA_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBoxBACnetTopic);
            this.groupBox6.Controls.Add(this.label23);
            this.groupBox6.Location = new System.Drawing.Point(685, 311);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(195, 49);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "BACnet Capture";
            // 
            // textBoxBACnetTopic
            // 
            this.textBoxBACnetTopic.Location = new System.Drawing.Point(35, 15);
            this.textBoxBACnetTopic.Name = "textBoxBACnetTopic";
            this.textBoxBACnetTopic.ReadOnly = true;
            this.textBoxBACnetTopic.Size = new System.Drawing.Size(154, 21);
            this.textBoxBACnetTopic.TabIndex = 5;
            this.textBoxBACnetTopic.Text = "thing/bacnet/clientid";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(9, 18);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(17, 12);
            this.label23.TabIndex = 6;
            this.label23.Text = "To";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.textBoxContolTopic);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Location = new System.Drawing.Point(684, 371);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(195, 49);
            this.groupBox7.TabIndex = 21;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Control Capture";
            // 
            // textBoxContolTopic
            // 
            this.textBoxContolTopic.Location = new System.Drawing.Point(36, 15);
            this.textBoxContolTopic.Name = "textBoxContolTopic";
            this.textBoxContolTopic.ReadOnly = true;
            this.textBoxContolTopic.Size = new System.Drawing.Size(155, 21);
            this.textBoxContolTopic.TabIndex = 5;
            this.textBoxContolTopic.Text = "thing/control/clientid";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 6;
            this.label8.Text = "To";
            // 
            // checkBoxBAnet
            // 
            this.checkBoxBAnet.AutoSize = true;
            this.checkBoxBAnet.Location = new System.Drawing.Point(882, 326);
            this.checkBoxBAnet.Name = "checkBoxBAnet";
            this.checkBoxBAnet.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBAnet.TabIndex = 26;
            this.checkBoxBAnet.UseVisualStyleBackColor = true;
            this.checkBoxBAnet.CheckedChanged += new System.EventHandler(this.CheckBoxBAnet_CheckedChanged);
            // 
            // checkBoxControl
            // 
            this.checkBoxControl.AutoSize = true;
            this.checkBoxControl.Location = new System.Drawing.Point(881, 388);
            this.checkBoxControl.Name = "checkBoxControl";
            this.checkBoxControl.Size = new System.Drawing.Size(15, 14);
            this.checkBoxControl.TabIndex = 27;
            this.checkBoxControl.UseVisualStyleBackColor = true;
            this.checkBoxControl.CheckedChanged += new System.EventHandler(this.CheckBoxControl_CheckedChanged);
            // 
            // comboBoxBridge
            // 
            this.comboBoxBridge.FormattingEnabled = true;
            this.comboBoxBridge.Location = new System.Drawing.Point(28, 531);
            this.comboBoxBridge.Name = "comboBoxBridge";
            this.comboBoxBridge.Size = new System.Drawing.Size(95, 20);
            this.comboBoxBridge.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 534);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 18;
            this.label7.Text = "To";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.textBoxMDBTopic);
            this.groupBox8.Controls.Add(this.label6);
            this.groupBox8.Location = new System.Drawing.Point(684, 425);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(195, 49);
            this.groupBox8.TabIndex = 22;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Access Capture";
            // 
            // textBoxMDBTopic
            // 
            this.textBoxMDBTopic.Location = new System.Drawing.Point(35, 17);
            this.textBoxMDBTopic.Name = "textBoxMDBTopic";
            this.textBoxMDBTopic.ReadOnly = true;
            this.textBoxMDBTopic.Size = new System.Drawing.Size(154, 21);
            this.textBoxMDBTopic.TabIndex = 5;
            this.textBoxMDBTopic.Text = "thing/mdb/clientid";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "To";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.textBoxSqlServerTopic);
            this.groupBox9.Controls.Add(this.label14);
            this.groupBox9.Location = new System.Drawing.Point(685, 478);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(195, 49);
            this.groupBox9.TabIndex = 23;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Sql Server Capture";
            // 
            // textBoxSqlServerTopic
            // 
            this.textBoxSqlServerTopic.Location = new System.Drawing.Point(35, 15);
            this.textBoxSqlServerTopic.Name = "textBoxSqlServerTopic";
            this.textBoxSqlServerTopic.ReadOnly = true;
            this.textBoxSqlServerTopic.Size = new System.Drawing.Size(155, 21);
            this.textBoxSqlServerTopic.TabIndex = 5;
            this.textBoxSqlServerTopic.Text = "thing/sqlserver/clientid";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 12);
            this.label14.TabIndex = 6;
            this.label14.Text = "To";
            // 
            // checkBoxAccess
            // 
            this.checkBoxAccess.AutoSize = true;
            this.checkBoxAccess.Location = new System.Drawing.Point(882, 438);
            this.checkBoxAccess.Name = "checkBoxAccess";
            this.checkBoxAccess.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAccess.TabIndex = 28;
            this.checkBoxAccess.UseVisualStyleBackColor = true;
            this.checkBoxAccess.CheckedChanged += new System.EventHandler(this.CheckBoxAccess_CheckedChanged);
            // 
            // checkBoxSqlServer
            // 
            this.checkBoxSqlServer.AutoSize = true;
            this.checkBoxSqlServer.Location = new System.Drawing.Point(882, 489);
            this.checkBoxSqlServer.Name = "checkBoxSqlServer";
            this.checkBoxSqlServer.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSqlServer.TabIndex = 29;
            this.checkBoxSqlServer.UseVisualStyleBackColor = true;
            this.checkBoxSqlServer.CheckedChanged += new System.EventHandler(this.CheckBoxSqlServer_CheckedChanged);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.checkBoxUdpServer);
            this.groupBox10.Controls.Add(this.label28);
            this.groupBox10.Controls.Add(this.textBoxUdpServerPort);
            this.groupBox10.Controls.Add(this.label15);
            this.groupBox10.Controls.Add(this.textBoxUDPClientLogin);
            this.groupBox10.Controls.Add(this.textBoxUDPCLientPort);
            this.groupBox10.Controls.Add(this.label24);
            this.groupBox10.Controls.Add(this.textBoxUDPClientServer);
            this.groupBox10.Controls.Add(this.label26);
            this.groupBox10.Location = new System.Drawing.Point(1019, 416);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(223, 129);
            this.groupBox10.TabIndex = 19;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "UDP Client Channel";
            // 
            // checkBoxUdpServer
            // 
            this.checkBoxUdpServer.AutoSize = true;
            this.checkBoxUdpServer.Location = new System.Drawing.Point(5, 101);
            this.checkBoxUdpServer.Name = "checkBoxUdpServer";
            this.checkBoxUdpServer.Size = new System.Drawing.Size(15, 14);
            this.checkBoxUdpServer.TabIndex = 24;
            this.checkBoxUdpServer.UseVisualStyleBackColor = true;
            this.checkBoxUdpServer.CheckedChanged += new System.EventHandler(this.CheckBoxUdpServer_CheckedChanged);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(21, 101);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(41, 12);
            this.label28.TabIndex = 24;
            this.label28.Text = "bridge";
            // 
            // textBoxUdpServerPort
            // 
            this.textBoxUdpServerPort.Location = new System.Drawing.Point(76, 99);
            this.textBoxUdpServerPort.Name = "textBoxUdpServerPort";
            this.textBoxUdpServerPort.Size = new System.Drawing.Size(136, 21);
            this.textBoxUdpServerPort.TabIndex = 24;
            this.textBoxUdpServerPort.Text = "6080";
            this.textBoxUdpServerPort.TextChanged += new System.EventHandler(this.TextBoxUdpServerPort_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 72);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 12);
            this.label15.TabIndex = 13;
            this.label15.Text = "login";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(18, 47);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 12);
            this.label24.TabIndex = 8;
            this.label24.Text = "Port";
            // 
            // textBoxUDPClientServer
            // 
            this.textBoxUDPClientServer.Location = new System.Drawing.Point(77, 15);
            this.textBoxUDPClientServer.Name = "textBoxUDPClientServer";
            this.textBoxUDPClientServer.Size = new System.Drawing.Size(134, 21);
            this.textBoxUDPClientServer.TabIndex = 5;
            this.textBoxUDPClientServer.Text = "prod.iotn2n.com";
            this.textBoxUDPClientServer.TextChanged += new System.EventHandler(this.TextBoxUDPClientServer_TextChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(18, 21);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(41, 12);
            this.label26.TabIndex = 6;
            this.label26.Text = "Server";
            // 
            // radioButtonMqttClient
            // 
            this.radioButtonMqttClient.AutoSize = true;
            this.radioButtonMqttClient.Checked = true;
            this.radioButtonMqttClient.Location = new System.Drawing.Point(997, 38);
            this.radioButtonMqttClient.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonMqttClient.Name = "radioButtonMqttClient";
            this.radioButtonMqttClient.Size = new System.Drawing.Size(14, 13);
            this.radioButtonMqttClient.TabIndex = 30;
            this.radioButtonMqttClient.TabStop = true;
            this.radioButtonMqttClient.UseVisualStyleBackColor = true;
            this.radioButtonMqttClient.CheckedChanged += new System.EventHandler(this.RadioButtonMqttClient_CheckedChanged);
            // 
            // radioButtonTcpClient
            // 
            this.radioButtonTcpClient.AutoSize = true;
            this.radioButtonTcpClient.Location = new System.Drawing.Point(997, 278);
            this.radioButtonTcpClient.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonTcpClient.Name = "radioButtonTcpClient";
            this.radioButtonTcpClient.Size = new System.Drawing.Size(14, 13);
            this.radioButtonTcpClient.TabIndex = 31;
            this.radioButtonTcpClient.UseVisualStyleBackColor = true;
            this.radioButtonTcpClient.CheckedChanged += new System.EventHandler(this.RadioButtonTcpClient_CheckedChanged);
            // 
            // radioButtonUDPClient
            // 
            this.radioButtonUDPClient.AutoSize = true;
            this.radioButtonUDPClient.Location = new System.Drawing.Point(997, 423);
            this.radioButtonUDPClient.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonUDPClient.Name = "radioButtonUDPClient";
            this.radioButtonUDPClient.Size = new System.Drawing.Size(14, 13);
            this.radioButtonUDPClient.TabIndex = 32;
            this.radioButtonUDPClient.UseVisualStyleBackColor = true;
            this.radioButtonUDPClient.CheckedChanged += new System.EventHandler(this.RadioButtonUDPClient_CheckedChanged);
            // 
            // checkBoxPLC
            // 
            this.checkBoxPLC.AutoSize = true;
            this.checkBoxPLC.Location = new System.Drawing.Point(882, 169);
            this.checkBoxPLC.Name = "checkBoxPLC";
            this.checkBoxPLC.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPLC.TabIndex = 34;
            this.checkBoxPLC.UseVisualStyleBackColor = true;
            this.checkBoxPLC.CheckedChanged += new System.EventHandler(this.CheckBoxPLC_CheckedChanged);
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.textBoxPLCTopic);
            this.groupBox12.Controls.Add(this.label2);
            this.groupBox12.Location = new System.Drawing.Point(683, 159);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(195, 49);
            this.groupBox12.TabIndex = 19;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "PLC Capture";
            // 
            // textBoxPLCTopic
            // 
            this.textBoxPLCTopic.Location = new System.Drawing.Point(35, 15);
            this.textBoxPLCTopic.Name = "textBoxPLCTopic";
            this.textBoxPLCTopic.ReadOnly = true;
            this.textBoxPLCTopic.Size = new System.Drawing.Size(157, 21);
            this.textBoxPLCTopic.TabIndex = 5;
            this.textBoxPLCTopic.Text = "thing/plc/clientid";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "To";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(897, 21);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(99, 517);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 35;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(143)))), ((int)(((byte)(178)))));
            this.ClientSize = new System.Drawing.Size(1246, 557);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox12);
            this.Controls.Add(this.checkBoxPLC);
            this.Controls.Add(this.radioButtonUDPClient);
            this.Controls.Add(this.radioButtonTcpClient);
            this.Controls.Add(this.radioButtonMqttClient);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.checkBoxSqlServer);
            this.Controls.Add(this.checkBoxAccess);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comboBoxBridge);
            this.Controls.Add(this.checkBoxControl);
            this.Controls.Add(this.checkBoxBAnet);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.checkBoxOPCUA);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.checkBoxOPCDA);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBoxSerialPort);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxDisplayHex);
            this.Controls.Add(this.sendBridge);
            this.Controls.Add(this.textToPayload);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBoxSerialPort);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.checkBoxReconnect);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.buttonStartStop);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "dgiot_dtu";
            this.groupBoxSerialPort.ResumeLayout(false);
            this.groupBoxSerialPort.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.TextBox textBoxTcpServerPort;
        private System.Windows.Forms.Label labelTargetPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxDataBits;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxStopBits;
        private System.Windows.Forms.TextBox textToPayload;
        private System.Windows.Forms.Button sendBridge;
        private System.Windows.Forms.TextBox textBoxMqttPassword;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxMqttUserName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxMqttSubTopic;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxMqttPubTopic;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxTcpClientLogin;
        private System.Windows.Forms.TextBox textBoxTcpClientPort;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxTcpClientServer;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBoxMqttClientId;
        private System.Windows.Forms.TextBox textBoxMqttPort;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBoxMqttSever;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox checkBoxSerialPort;
        private System.Windows.Forms.CheckBox checkBoxTcpServer;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxOPCDATopic;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckBox checkBoxOPCDA;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxOPCUATopic;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox checkBoxOPCUA;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxBACnetTopic;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox textBoxContolTopic;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxBAnet;
        private System.Windows.Forms.CheckBox checkBoxControl;
        private System.Windows.Forms.ComboBox comboBoxBridge;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox textBoxMDBTopic;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox textBoxSqlServerTopic;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBoxAccess;
        private System.Windows.Forms.CheckBox checkBoxSqlServer;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxUDPClientLogin;
        private System.Windows.Forms.TextBox textBoxUDPCLientPort;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox textBoxUDPClientServer;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.RadioButton radioButtonMqttClient;
        private System.Windows.Forms.RadioButton radioButtonTcpClient;
        private System.Windows.Forms.RadioButton radioButtonUDPClient;
        private System.Windows.Forms.CheckBox checkBoxPLC;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.TextBox textBoxPLCTopic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox comboBoxParity;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBoxMqttBridge;
        private System.Windows.Forms.TextBox textBoxMqttServerPort;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox textBoxUdpServerPort;
        private System.Windows.Forms.CheckBox checkBoxUdpServer;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.ComboBox comboBoxCmdProdxy;
    }
}

