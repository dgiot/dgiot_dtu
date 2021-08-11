using System;
using System.Text;
using MQTTnet.Core.Client;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using System.IO.Ports;
using MQTTnet.Core;
using PortListener.Core.Utilities;
using System.Text.RegularExpressions;

//https://github.com/titanium-as/TitaniumAS.Opc.Client
//https://github.com/chkr1011/MQTTnet

namespace dgiot_dtu
{

    public class MqttHelper
    {
        private MqttHelper() { }

        private static MqttClient mqttClient = null;
        private static string _mqttserver = "prod.iotn2n.com";
        private static string _subopcda = "thing/opcda/";
        private static string _submdb =  "thing/mdb/";
        private static string _subtopic = "thing/com/";
        private static string _pubtopic = "thing/com/post/";
        private static string _clientid = Guid.NewGuid().ToString().Substring(0, 5);
        private static string _username = "dgiot";
        private static string _password = "dgiot";
        private static SerialPort _port = null;
        private static MqttHelper Instance;
        private static MainForm _mainform = null;
        private static bool _bIsRunning = false;
        public static MqttHelper GetInstance()
        {
            if (Instance == null)
                Instance = new MqttHelper();
            return Instance;
        }

        public void start(string server)
        {
            _mqttserver = server;
            _bIsRunning = true;
            Task.Run(async () => { await ConnectMqttServerAsync(); });
      
        }

        public void start(string server, string clientid, string username, string password, string subtopic, string pubtopic, 
            SerialPort comport, MainForm mainform)
        {
            _mqttserver = server;
            _clientid = clientid;
            _username = username;
            _password = password;
            _pubtopic = pubtopic;
            _subtopic = subtopic;
            _port = comport;
            _mainform = mainform;
            _bIsRunning = true;
            Task.Run(async () => { await ConnectMqttServerAsync(); }); 
        }

        public void stop()
        {
            _bIsRunning = false;
            Task.Run(async () => { await DisConnectMqttServerAsync(); });
        }

        public void publish(byte[] payload)
        {
            var appMsg = new MqttApplicationMessage(_pubtopic + _clientid, payload, MqttQualityOfServiceLevel.AtLeastOnce, false);
            mqttClient.PublishAsync(appMsg);
        }


        private static async Task ReConnectMqttServerAsync()
        {
            while (_bIsRunning)
            {
                Thread.Sleep(1000 * 10);
                if (!mqttClient.IsConnected)
                {
                    await ConnectMqttServerAsync();
                }
            }
        }

        private static async Task ConnectMqttServerAsync()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
                mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                mqttClient.Connected += MqttClient_Connected;
                mqttClient.Disconnected += MqttClient_Disconnected;
            }
            try
            {
                var options = new MqttClientTcpOptions
                {
                    Server = _mqttserver,
                    ClientId = _clientid,
                    UserName = _username,
                    Password = _password,
                    CleanSession = true
                };

                await DisConnectMqttServerAsync();
                await mqttClient.ConnectAsync(options);
                await ReConnectMqttServerAsync();

            }
            catch (Exception ex)
            {
                _mainform.Log(ex.ToString());
            }
        }

        private static async Task DisConnectMqttServerAsync()
        {
            try
            {
                await mqttClient.DisconnectAsync();

            }
            catch (Exception ex)
            {
                _mainform.Log(ex.ToString());
            }
        }


        /// <summary>
        /// 服务器连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Connected(object sender, EventArgs e)
        {
            _mainform.Log("mqtt:" + _clientid + " connected");
            mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(_subopcda + _clientid , MqttQualityOfServiceLevel.AtMostOnce),
                new TopicFilter(_submdb + _clientid , MqttQualityOfServiceLevel.AtMostOnce),
                new TopicFilter(_subtopic + _clientid, MqttQualityOfServiceLevel.AtMostOnce)
            });
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Disconnected(object sender, EventArgs e)
        {
            _mainform.Log("mqtt:" + _clientid + " disconnected");
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Regex r_subtopic = new Regex(MqttHelper._subtopic); // 定义一个Regex对象实例
            Match m_subtopic = r_subtopic.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
            if (m_subtopic.Success)
            {
                _port.Write(e.ApplicationMessage.Payload, 0, e.ApplicationMessage.Payload.Length);
                _mainform.Log("mqtt recv :topic: " + e.ApplicationMessage.Topic.ToString() + " payload: " + StringHelper.ToHexString(e.ApplicationMessage.Payload));
            }
            else
            {
                Regex r_subopcda = new Regex(MqttHelper._subopcda); // 定义一个Regex对象实例
                Match m_subopcda = r_subopcda.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
                if (m_subopcda.Success)
                {
                    String data = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    _mainform.Log("mqtt recv :topic: " + e.ApplicationMessage.Topic.ToString() + " payload: " + data);
                    Dictionary<string, object> json = get_payload(e.ApplicationMessage.Payload);

                    OPCDAHelper.Do_opc_da(mqttClient, json, _mainform);

                }else {
                    Regex r_submdb = new Regex(MqttHelper._submdb); // 定义一个Regex对象实例
                    Match m_submdb = r_submdb.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
                    if (m_submdb.Success)
                    {
                        Dictionary<string, object> json = get_payload(e.ApplicationMessage.Payload);
                        AccessHelper.do_mdb(mqttClient, json, _mainform);
                    }
                }
            }
        }

        private static Dictionary<string, object> get_payload(byte[] payload)
        {
            String data = Encoding.UTF8.GetString(payload);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(data);
            return json;
        }

        private static DateTime BaseTime = new DateTime(1970, 1, 1);

        /// <summary>   
        /// 将unixtime转换为.NET的DateTime   
        /// </summary>   
        /// <param name="timeStamp">秒数</param>   
        /// <returns>转换后的时间</returns>   
        public static DateTime FromUnixTime(long timeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(timeStamp * 10000000 + BaseTime.Ticks));
        }

        /// <summary>   
        /// 将.NET的DateTime转换为unix time   
        /// </summary>   
        /// <param name="dateTime">待转换的时间</param>   
        /// <returns>转换后的unix time</returns>   
        public static long FromDateTime(DateTime dateTime)
        {
            return (TimeZone.CurrentTimeZone.ToUniversalTime(dateTime).Ticks - BaseTime.Ticks) / 10000000;
        }

    }

}