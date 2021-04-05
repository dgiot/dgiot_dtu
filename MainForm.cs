using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using PortListener.Core.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace dgiot_dtu
{
    public enum TelnetCommand : byte
    {
        SE = 240,               // End of subnegotiation parameters.
        NOP = 241,              // No operation.
        DataMark = 242,         // The data stream portion of a Synch.
        // This should always be accompanied
        // by a TCP Urgent notification.
        Break = 243,            // NVT character BRK.
        InterruptProcess = 244, // The function IP.
        AbortOutput = 245,      // The function AO.
        AreYouThere = 246,      // The function AYT.
        EraseCharacter = 247,   //The function EC.
        EraseLine = 248,        // The function EL.
        GoAhead = 249,          // The GA signal.
        SB = 250,               // Indicates that what follows is subnegotiation of the indicated option.
        WILL = 251,             // Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
        WONT = 252,             // Indicates the refusal to perform, or continue performing, the indicated option.
        DO = 253,               // Indicates the request that the other party perform, or confirmation that you are expecting
        // the other party to perform, the indicated option.
        DONT = 254,             // Indicates the demand that the other party stop performing, or confirmation that you are no
        // longer expecting the other party to perform, the indicated option.
        IAC = 255,              // Data Byte 255.
    }

    public enum TelnetOption : byte
    {
        ECHO = 1,
        NO_GO_AHEAD = 3,
        LINEMODE = 34,
    }

    public partial class MainForm : Form
    {
        private delegate void LogHandler(string text);

        private TcpClient _client;
        private TcpListener _server;
        private TcpListener ro_server;
        private List<TcpClient> ro_clientList = new List<TcpClient>();
       
        private SerialPort _port;
        private NetworkStream _stream;
        private readonly byte[] _tcpdata = new byte[1024];

        private bool _bAutoReconnect;
        private bool _bDisplayHex;
        private bool _bIsRunning;
        private bool _bTelnet;

        private Configuration config;

        enum ConnectionMode
        {
            CLIENT,
            SERVER,
        }

        private ConnectionMode _eConnectionMode = ConnectionMode.SERVER;

        public MainForm()
        {
            InitializeComponent();

            Text += " v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            var arrPorts = SerialPort.GetPortNames();
            comboBoxSerialPort.Items.Clear();
            foreach (var port in arrPorts)
                comboBoxSerialPort.Items.Add(port);
            if (arrPorts.Length > 0)
                comboBoxSerialPort.SelectedIndex = 0;

            comboBoxBaudRate.SelectedIndex = 7;
            comboBoxDataBits.SelectedIndex = 0;
            comboBoxStopBits.SelectedIndex = 0;

            _bAutoReconnect = checkBoxReconnect.Checked;
            _bDisplayHex = checkBoxDisplayHex.Checked;

            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["SerialPort"] != null)
                {
                    var tmp = config.AppSettings.Settings["SerialPort"].Value;
                    comboBoxSerialPort.SelectedIndex = comboBoxSerialPort.Items.IndexOf(tmp);

                }
                
                if (config.AppSettings.Settings["BaudRate"] != null)
                {
                    var tmp = config.AppSettings.Settings["BaudRate"].Value;
                    comboBoxBaudRate.SelectedIndex = comboBoxBaudRate.Items.IndexOf(tmp);
                }
                
                if (config.AppSettings.Settings["tcpPort"] != null)
                {
                    var tmp = config.AppSettings.Settings["tcpPort"].Value;
                    textBoxTargetPort.Text = tmp;
                }
                
                if (config.AppSettings.Settings["tcpPortRO"] != null)
                {
                    var tmp = config.AppSettings.Settings["tcpPortRO"].Value;
                    textBoxReadOnlyPort.Text = tmp;
                }

                if (config.AppSettings.Settings["bTelnet"] != null)
                {
                    var tmp = config.AppSettings.Settings["bTelnet"].Value;
                    checkBoxTelnet.Checked = bool.Parse(tmp);
                }
                
            }
            catch (Exception ex)
            {
                Log("read config exception: " + ex.Message);
            }

            _bTelnet = checkBoxTelnet.Checked;
            radioButtonServer_CheckedChanged_do();
        }

        private void toStop()
        {
            try
            {
                buttonStartStop.Text = @"Start";
                _bIsRunning = false;

                if (_port.IsOpen)
                    _port.Close();

                if (_client != null)
                {
                    if (_client.Connected)
                        _client.Close();
                    _client = null;
                }
                foreach (var c in ro_clientList)
                {
                    c.Close();
                }
                ro_clientList.Clear();

                if (_server != null)
                {
                    _server.Stop();
                    _server = null;
                }
                if (ro_server != null)
                {
                    ro_server.Stop();
                    ro_server = null;
                }
            }
            catch (Exception e)
            {
                Log("stop server exception:" + e.Message);
                return;
            }
            
        }

        private StopBits strToStopBits(string s)
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

        private void saveAppConfig()
        {
            if (config.AppSettings.Settings["SerialPort"] == null)
            {
                config.AppSettings.Settings.Add("SerialPort", (string)comboBoxSerialPort.SelectedItem);
            } else
            {
                config.AppSettings.Settings["SerialPort"].Value = (string)comboBoxSerialPort.SelectedItem;
            }

            if (config.AppSettings.Settings["BaudRate"] == null) {
                config.AppSettings.Settings.Add("BaudRate", (string)comboBoxBaudRate.SelectedItem);
            } else
            {
                config.AppSettings.Settings["BaudRate"].Value = (string)comboBoxBaudRate.SelectedItem;
            }

            if (config.AppSettings.Settings["tcpPort"] == null)
            {
                config.AppSettings.Settings.Add("tcpPort", textBoxTargetPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpPort"].Value = textBoxTargetPort.Text;
            }

            if (config.AppSettings.Settings["tcpPortRO"] == null)
            {
                config.AppSettings.Settings.Add("tcpPortRO", textBoxReadOnlyPort.Text);
            }
            else
            {
                config.AppSettings.Settings["tcpPortRO"].Value = textBoxReadOnlyPort.Text;
            }

            if (config.AppSettings.Settings["login"] == null)
            {
                config.AppSettings.Settings.Add("login", textlogin.Text);
            }
            else
            {
                config.AppSettings.Settings["login"].Value = textlogin.Text;
            }

            if (config.AppSettings.Settings["bTelnet"] == null)
            {
                config.AppSettings.Settings.Add("bTelnet", _bTelnet.ToString());
            }
            else
            {
                config.AppSettings.Settings["bTelnet"].Value = _bTelnet.ToString();
            }
            
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void ButtonStartStopClick(object sender, EventArgs e)
        {
            if (!_bIsRunning)
            {                
                try
                {
                    _port = new SerialPort((string)comboBoxSerialPort.SelectedItem,
                        int.Parse((string)comboBoxBaudRate.SelectedItem),
                        Parity.None,
                        int.Parse((string)comboBoxDataBits.SelectedItem),
                        strToStopBits((string)comboBoxStopBits.SelectedItem));
                    _port.DataReceived += PortDataReceived;
                    _port.ReceivedBytesThreshold = 1;                    
                    _port.Open();
                } catch(Exception)
                {
                    MessageBox.Show(@"Couldn't open port " + (string) comboBoxSerialPort.SelectedItem);
                    return;
                }

                try
                {
                    if (_eConnectionMode == ConnectionMode.CLIENT)
                    {
                        Log("Connecting to " + textBoxIPAddress.Text + ":" + textBoxTargetPort.Text);

                        _client = new TcpClient();
                        _client.BeginConnect(textBoxIPAddress.Text, int.Parse(textBoxTargetPort.Text), TcpConnectedOut,
                                                 null);
                    }
                    else
                    {
                        _server = new TcpListener(IPAddress.Any, int.Parse(textBoxTargetPort.Text));
                        _server.Start();
                        _server.BeginAcceptTcpClient(TcpConnectedIn, null);

                        ro_server = new TcpListener(IPAddress.Any, int.Parse(textBoxReadOnlyPort.Text));
                        ro_server.Start();
                        ro_server.BeginAcceptTcpClient(TcpConnectedInRO, ro_server);

                        Log("Server start!");
                    }
                }
                catch (Exception ex)
                {
                    if(_eConnectionMode == ConnectionMode.CLIENT)   
                        Log("Couldn't connect: " + ex.Message);
                    else
                        Log("Couldn't listen: " + ex.Message);

                    _port.DataReceived -= PortDataReceived;
                    _port.Close();

                    return;
                }
                buttonStartStop.Text = @"Stop";
                _bIsRunning = true;

                saveAppConfig();
            }
            else
            {
                toStop();
            }              
        }

        void TcpReaderRO(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            NetworkStream stream = client.GetStream();
            try
            {
                var numRead = stream.EndRead(ar);
                if (numRead == 0)
                {
                    Log(client.Client.LocalEndPoint + " <==> " + client.Client.RemoteEndPoint + " disconnect");
                    client.Close();
                    ro_clientList.Remove(client);
                    return;
                }
                byte[] data = new byte[1024];
                stream.BeginRead(data, 0, data.Length, TcpReaderRO, client);
            }
            catch (Exception e)
            {
                Log(client.Client.LocalEndPoint + " <==> " + client.Client.RemoteEndPoint + " exception:" + e.Message);
                client.Close();
                ro_clientList.Remove(client);
                return;
            }
        }
        void TcpConnectedInRO(IAsyncResult result)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)result.AsyncState;
            try
            {
                if (!_bIsRunning)
                {
                    Log("TcpConnectedInRO: server shutdown");
                    goto end;
                }

                TcpClient tmp_client = listener.EndAcceptTcpClient(result);
                NetworkStream tmp_stream = tmp_client.GetStream();

                ro_clientList.Add(tmp_client);

                Log("Client Connected: " + tmp_client.Client.LocalEndPoint + " <==> " + tmp_client.Client.RemoteEndPoint);

                Log("dddd start!");
                if (_bTelnet) {
                    byte[] willEcho = new byte[] { (byte) TelnetCommand.IAC,
                                           (byte) TelnetCommand.WILL,
                                           (byte)TelnetOption.ECHO };
                    byte[] noGoAhead = new byte[] { (byte) TelnetCommand.IAC,
                                           (byte) TelnetCommand.WILL,
                                           (byte)TelnetOption.NO_GO_AHEAD };
                    byte[] wontLinemode = new byte[] { (byte) TelnetCommand.IAC,
                                           (byte) TelnetCommand.WONT,
                                           (byte)TelnetOption.LINEMODE };
                    tmp_stream.Write(willEcho, 0, willEcho.Length);
                    tmp_stream.Write(noGoAhead, 0, noGoAhead.Length);
                    tmp_stream.Write(wontLinemode, 0, wontLinemode.Length);
                }

                byte[] data = new byte[1024];
                tmp_stream.BeginRead(data, 0, data.Length, TcpReaderRO, tmp_client);

            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException)
                    Log("Connection shutdown");
                else
                    Log("Connection exception: " + e.Message);
            }

        end:
            try
            {
                if (_bIsRunning)
                {
                    /* accept other connection again */
                    listener.BeginAcceptTcpClient(TcpConnectedInRO, listener);
                }
            }
            catch (Exception e)
            {
                Log("Server exception: " + e.Message);
                toStop();
            }
        }

        void TcpConnectedIn(IAsyncResult result)
        {
            try
            {
                if (!_bIsRunning)
                {
                    Log("TcpConnectedIn: server shutdown");
                    goto end;
                }
                TcpClient tmp_client = _server.EndAcceptTcpClient(result);
                NetworkStream tmp_stream = tmp_client.GetStream();

                if (_client != null)
                {
                    Log("Already in use, close connected from: " + tmp_client.Client.RemoteEndPoint);
                    byte[] reject = System.Text.Encoding.ASCII.GetBytes( "Already in use!\r\n");
                    tmp_stream.Write(reject, 0, reject.Length);
                    tmp_stream.Close();
                    tmp_client.Close();
                    goto end;
                }

                _client = tmp_client;

                _stream = tmp_stream;

                Log("Client Connected: " + _client.Client.LocalEndPoint + "<==>" + _client.Client.RemoteEndPoint);

                if (_bTelnet) {
                    byte[] willEcho = new byte[] { (byte) TelnetCommand.IAC,
                                           (byte) TelnetCommand.WILL,
                                           (byte)TelnetOption.ECHO };
                    byte[] noGoAhead = new byte[] { (byte) TelnetCommand.IAC,
                                           (byte) TelnetCommand.WILL,
                                           (byte)TelnetOption.NO_GO_AHEAD };
                    byte[] wontLinemode = new byte[] { (byte) TelnetCommand.IAC,
                                           (byte) TelnetCommand.WONT,
                                           (byte)TelnetOption.LINEMODE };
                    _stream.Write(willEcho, 0, willEcho.Length);
                    _stream.Write(noGoAhead, 0, noGoAhead.Length);
                    _stream.Write(wontLinemode, 0, wontLinemode.Length);
                }
                
                _stream.BeginRead(_tcpdata, 0, _tcpdata.Length, TcpReader, null);
            } catch(Exception e)
            {
                if(e is ObjectDisposedException)
                    Log("Connection shutdown");
                else
                    Log("Connection exception: " + e.Message);
            }
end:
            try
            {
                if (_bIsRunning)
                {
                    /* accept other connection again */
                    _server.BeginAcceptTcpClient(TcpConnectedIn, null);
                }
            }
            catch (Exception e)
            {
                Log("Server exception: " + e.Message);
                toStop();
            }
        }

        void TcpConnectedOut(IAsyncResult result)
        {
            try
            {
                _client.EndConnect(result);

                _stream = _client.GetStream();

                Log("Connected");

                var tcpdata = new byte[1024];
                _stream.BeginRead(tcpdata, 0, tcpdata.Length, TcpReader, null);
               
                if (_stream.CanWrite)
                {
                    Thread.Sleep(1000 * 1);
                    byte[] login = System.Text.Encoding.UTF8.GetBytes(config.AppSettings.Settings["login"].Value);
                    Log("S->N: login[" + config.AppSettings.Settings["login"].Value+"]");
                    _stream.Write(login, 0, login.Length);
                }

            } catch(Exception e)
            {
                Log("Couldn't connect: " + e.Message);

                if (_eConnectionMode == ConnectionMode.CLIENT)
                {
                    if (_bAutoReconnect && _bIsRunning)
                    {
                        Log("Connecting to " + textBoxIPAddress.Text + ":" + textBoxTargetPort.Text);
                        _client.BeginConnect(textBoxIPAddress.Text, int.Parse(textBoxTargetPort.Text), TcpConnectedOut,
                                                 null);
                    }
                }
            }
        }

        void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var rxlen = _port.BytesToRead;
            var data = new byte[rxlen];
            _port.Read(data, 0, rxlen);

            var line = _bDisplayHex ? StringHelper.ToHexString(data, 0, rxlen) : System.Text.Encoding.ASCII.GetString(data, 0, rxlen);
            if (line.EndsWith("\r\n"))
                line = line.Substring(0, line.Length - 2);

            Log("S->N: " + line);

            foreach (var c in ro_clientList)
            {
                var stream = c.GetStream();
                if (stream.CanWrite)
                {
                    try
                    {
                        stream.Write(data, 0, rxlen);
                    }
                    catch (Exception ex)
                    {
                        Log("Write to " + c.Client.LocalEndPoint + " exception:" + ex.Message);
                        c.Close();
                        ro_clientList.Remove(c);
                    }
                }
                
            }
            if(_stream != null)
                if(_stream.CanWrite)
                {
                    try
                    {
                        _stream.Write(data, 0, rxlen);
                    }
                    catch(Exception ex)
                    {
                        Log("Can't write to TCP stream:" + ex.Message);

                        try
                        {
                            _stream.Close();
                        } catch{}

                        _stream = null;
                        if (_bAutoReconnect && _bIsRunning)
                        {
                            if (_eConnectionMode == ConnectionMode.CLIENT)
                            {
                                Log("Connecting to " + textBoxIPAddress.Text + ":" + textBoxTargetPort.Text);

                                _client.BeginConnect(textBoxIPAddress.Text, int.Parse(textBoxTargetPort.Text),
                                                         TcpConnectedOut, null);
                            }
                        }
                    }
                }
        }

        private void onConnectClosed()
        {
            try
            {
                _client.Close();
                _client = null;
            
            } catch { }

            if (_bAutoReconnect && _bIsRunning)
            {
                try
                {
                    if (_eConnectionMode == ConnectionMode.CLIENT)
                    {
                        Log("Connecting to " + textBoxIPAddress.Text + ":" + textBoxTargetPort.Text);

                        _client = new TcpClient();

                        _client.BeginConnect(textBoxIPAddress.Text, int.Parse(textBoxTargetPort.Text),
                                                 TcpConnectedOut,
                                                 null);
                    }
                }
                catch (Exception ex)
                {
                    Log("Problem reconnecting:" + ex.Message);
                }
            }
        }
 
        void TcpReader(IAsyncResult ar)
        {
            try
            {
               
                var rxbytes = _stream.EndRead(ar);

                if (rxbytes > 0)
                {
                    var offset = 0;

                    if (_bTelnet) {
                        while (_tcpdata[offset] == (byte)TelnetCommand.IAC && rxbytes >= 3) {
                            Log("Receive IAC: " + StringHelper.ToHexString(_tcpdata, offset, 3));
                            offset += 3;
                        }
                    }

                    _port.Write(_tcpdata, offset, rxbytes - offset);

                    var line = _bDisplayHex ? StringHelper.ToHexString(_tcpdata, offset, rxbytes - offset)
                        : System.Text.Encoding.ASCII.GetString(_tcpdata, offset, rxbytes - offset);
                    if (line.EndsWith("\r\n"))
                        line = line.Substring(0, line.Length - 2);

                    Log("N->S: " + line);
                    _stream.BeginRead(_tcpdata, 0, _tcpdata.Length, TcpReader, null);
                }

                if (rxbytes == 0)
                {
                    Log("Client closed");
                    onConnectClosed();
                }
            } catch(Exception e)
            {
                if (e is ObjectDisposedException)
                    Log("Connection closed");
                else if(e is IOException && e.Message.Contains("closed"))
                    Log("Connection closed");
                else
                    Log("Exception: " + e.Message);

                onConnectClosed();
            }
        }

        void Log(string text)
        {
            if(InvokeRequired)
            {
                Invoke(new LogHandler(Log), new object[] {text});
                return;
            }

            // Truncate
            if (textBoxLog.Text.Length > 4096)
                textBoxLog.Text = textBoxLog.Text.Substring(textBoxLog.Text.Length - 4096);

            textBoxLog.Text += text + "\r\n";
            textBoxLog.SelectionStart = textBoxLog.Text.Length - 1;
            textBoxLog.ScrollToCaret();
        }

        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.dynamicdevices.co.uk");
        }

        private void CheckBoxReconnectCheckedChanged(object sender, EventArgs e)
        {
            _bAutoReconnect = checkBoxReconnect.Checked;
        }

        private void CheckBoxDisplayHexCheckedChanged(object sender, EventArgs e)
        {
            _bDisplayHex = checkBoxDisplayHex.Checked;
        }

        private void ButtonClearClick(object sender, EventArgs e)
        {
            textBoxLog.Text = "";
        }

        private void radioButtonServer_CheckedChanged_do()
        {
            if (radioButtonServer.Checked)
            {
                _eConnectionMode = ConnectionMode.SERVER;
                textBoxIPAddress.Enabled = false;
                checkBoxReconnect.Enabled = false;
                textBoxReadOnlyPort.Enabled = true;
            }
            else
            {
                _eConnectionMode = ConnectionMode.CLIENT;
                textBoxIPAddress.Enabled = true;
                checkBoxReconnect.Enabled = true;
                textBoxReadOnlyPort.Enabled = false;
            }
        }

        private void radioButtonServer_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonServer_CheckedChanged_do();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/YeLincoln/Serial2Net");
        }

        private void checkBoxTelnet_CheckedChanged(object sender, EventArgs e)
        {
            _bTelnet = checkBoxTelnet.Checked;
        }
    }
}
