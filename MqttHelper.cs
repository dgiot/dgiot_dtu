//https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using MQTTnet;
    using MQTTnet.Core;
    using MQTTnet.Core.Client;
    using MQTTnet.Core.Packets;
    using MQTTnet.Core.Protocol;
    using PortListener.Core.Utilities;

    public class MqttHelper
    {
        private MqttHelper()
        {
        }

        private static MqttClient mqttClient = null;
        private static string mqttserver = "prod.iotn2n.com";
        private static string subopcda = "thing/opcda/";
        private static string submdb =  "thing/mdb/";
        private static string subtopic = "thing/com/";
        private static string pubtopic = "thing/com/post/";
        private static string clientid = Guid.NewGuid().ToString().Substring(0, 5);
        private static string username = "dgiot";
        private static string password = "dgiot";
        private static SerialPort port = null;
        private static MqttHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRunning = false;

        public static MqttHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new MqttHelper();
            }

            return instance;
        }

        public void Start1(string server)
        {
            mqttserver = server;
            bIsRunning = true;
            Task.Run(async () => { await ConnectMqttServerAsync(); });
        }

        public void Start(string server, string clientid, string username, string password, string subtopic, string pubtopic,
            SerialPort comport, MainForm mainform)
        {
            mqttserver = server;
            MqttHelper.clientid = clientid;
            MqttHelper.username = username;
            MqttHelper.password = password;
            MqttHelper.pubtopic = pubtopic;
            MqttHelper.subtopic = subtopic;
            port = comport;
            MqttHelper.mainform = mainform;
            bIsRunning = true;
            Task.Run(async () => { await ConnectMqttServerAsync(); }); 
        }

        public void Stop()
        {
            bIsRunning = false;
            Task.Run(async () => { await DisConnectMqttServerAsync(); });
        }

        public void Publish(byte[] payload)
        {
            var appMsg = new MqttApplicationMessage(pubtopic + clientid, payload, MqttQualityOfServiceLevel.AtLeastOnce, false);
            mqttClient.PublishAsync(appMsg);
        }

        private static async Task ReConnectMqttServerAsync()
        {
            while (bIsRunning)
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
                    Server = mqttserver,
                    ClientId = clientid,
                    UserName = username,
                    Password = password,
                    CleanSession = true
                };

                await DisConnectMqttServerAsync();
                await mqttClient.ConnectAsync(options);
                await ReConnectMqttServerAsync();

            }
            catch (Exception ex)
            {
                mainform.Log(ex.ToString());
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
                mainform.Log(ex.ToString());
            }
        }


        /// <summary>
        /// 服务器连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Connected(object sender, EventArgs e)
        {
            mainform.Log("mqtt client:" + clientid + " connected");
            mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(subopcda + clientid , MqttQualityOfServiceLevel.AtMostOnce),
                new TopicFilter(submdb + clientid , MqttQualityOfServiceLevel.AtMostOnce),
                new TopicFilter(subtopic + clientid, MqttQualityOfServiceLevel.AtMostOnce)
            });
            mainform.Log("mqtt client subscribe topic: " + subopcda + clientid);
            mainform.Log("mqtt client subscribe topic: " + submdb + clientid);
            mainform.Log("mqtt client subscribe topic: " + subtopic + clientid);
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Disconnected(object sender, EventArgs e)
        {
            mainform.Log("mqtt:" + clientid + " disconnected");
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Regex r_subtopic = new Regex(MqttHelper.subtopic); // 定义一个Regex对象实例
            Match m_subtopic = r_subtopic.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
            if (m_subtopic.Success)
            {
                port.Write(e.ApplicationMessage.Payload, 0, e.ApplicationMessage.Payload.Length);
                mainform.Log("mqtt recv :topic: " + e.ApplicationMessage.Topic.ToString() + " payload: " + StringHelper.ToHexString(e.ApplicationMessage.Payload));
            }
            else
            {
                Regex r_subopcda = new Regex(MqttHelper.subopcda); // 定义一个Regex对象实例
                Match m_subopcda = r_subopcda.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
                if (m_subopcda.Success)
                {
                    string data = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    mainform.Log("mqtt recv :topic: " + e.ApplicationMessage.Topic.ToString() + " payload: " + data);
                    Dictionary<string, object> json = get_payload(e.ApplicationMessage.Payload);
                    OPCDAHelper.Do_opc_da(mqttClient, json, MqttHelper.clientid, mainform);

                }else {
                    Regex r_submdb = new Regex(MqttHelper.submdb); // 定义一个Regex对象实例
                    Match m_submdb = r_submdb.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
                    if (m_submdb.Success)
                    {
                        string data = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        mainform.Log("mqtt recv :topic: " + e.ApplicationMessage.Topic.ToString() + " payload: " + data);
                        Dictionary<string, object> json = get_payload(e.ApplicationMessage.Payload);
                        AccessHelper.Do_mdb(mqttClient, json, clientid, mainform);
                    }
                }
            }
        }

        private static Dictionary<string, object> get_payload(byte[] payload)
        {
            string data = Encoding.UTF8.GetString(payload);
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