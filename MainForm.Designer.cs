namespace dgiot_dtu
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
            this.groupBoxSerialPort.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Location = new System.Drawing.Point(693, 38);
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
            this.textBoxLog.Location = new System.Drawing.Point(18, 255);
            this.textBoxLog.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(835, 372);
            this.textBoxLog.TabIndex = 9;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.Silver;
            this.linkLabel1.Location = new System.Drawing.Point(550, 633);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(179, 18);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Dynamic Devices Ltd";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1LinkClicked);
            // 
            // checkBoxReconnect
            // 
            this.checkBoxReconnect.AutoSize = true;
            this.checkBoxReconnect.Location = new System.Drawing.Point(693, 88);
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
            this.checkBoxDisplayHex.Location = new System.Drawing.Point(693, 124);
            this.checkBoxDisplayHex.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxDisplayHex.Name = "checkBoxDisplayHex";
            this.checkBoxDisplayHex.Size = new System.Drawing.Size(133, 22);
            this.checkBoxDisplayHex.TabIndex = 11;
            this.checkBoxDisplayHex.Text = "Display Hex";
            this.checkBoxDisplayHex.UseVisualStyleBackColor = true;
            this.checkBoxDisplayHex.CheckedChanged += new System.EventHandler(this.CheckBoxDisplayHexCheckedChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(693, 178);
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
            this.groupBox1.Location = new System.Drawing.Point(291, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(369, 218);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TCP/IP";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 185);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 18);
            this.label6.TabIndex = 13;
            this.label6.Text = "login payload";
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
            this.textBoxTargetPort.Text = "8067";
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
            this.textBoxIPAddress.Text = "127.0.0.1";
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
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Silver;
            this.label3.Location = new System.Drawing.Point(471, 633);
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
            this.linkLabel2.Location = new System.Drawing.Point(740, 633);
            this.linkLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(89, 18);
            this.linkLabel2.TabIndex = 16;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "YeLincoln";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 200;
            this.toolTip1.ReshowDelay = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 663);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxSerialPort);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.checkBoxDisplayHex);
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
    }
}

