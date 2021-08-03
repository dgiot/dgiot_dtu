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

//https://github.com/titanium-as/TitaniumAS.Opc.Client
//https://github.com/chkr1011/MQTTnet

namespace dgiot_dtu
{

    public class MqttHelper
    {
        private MqttHelper() { }

        private static MqttClient mqttClient = null;
        private static string mqttserver = "prod.iotn2n.com";
        private static string subopcda = "dgiot_opc_da";

        private static MqttHelper Instance;
        public static MqttHelper GetInstance()
        {
            if (Instance == null)
                Instance = new MqttHelper();
            return Instance;
        }

        public void start(string server)
        {
            mqttserver = server;
            Task.Run(async () => { await ConnectMqttServerAsync(); });
            Task.Run(async () => { await ReConnectMqttServerAsync(); });
        }


        private static async Task ReConnectMqttServerAsync()
        {
            while (true)
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
                    ClientId = Guid.NewGuid().ToString().Substring(0, 5),
                    UserName = "dgiot_opc",
                    Password = "dgiot_opc",
                    CleanSession = true
                };

                await mqttClient.ConnectAsync(options);

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.ToString());
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
                Console.WriteLine("{0}", ex.ToString());
            }
        }


        /// <summary>
        /// 服务器连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Connected(object sender, EventArgs e)
        {
            mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(subopcda, MqttQualityOfServiceLevel.AtMostOnce)
            });
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Disconnected(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Dictionary<string, object> json = get_payload(e.ApplicationMessage.Payload);

            if ("dgiot_opc_da" == e.ApplicationMessage.Topic)
            {
                OPCDAHelper.do_opc_da(mqttClient, json);
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