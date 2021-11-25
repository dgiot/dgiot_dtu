// <copyright file="MqttServerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using MQTTnet;
    using MQTTnet.Core;
    using MQTTnet.Core.Protocol;
    using MQTTnet.Core.Server;

    public class MqttServerHelper
    {
        private MqttServerHelper()
        {
        }

        private static MqttServer mqttServer = null;
        private static int port = 1883;
        private static string pubtopic = "thing/com/post/";

        private static string plctopic = "thing/plc/clientid/";
        private static string opcdatopic = "thing/opcda/clientid/";
        private static string opcuatopic = "thing/opcua/clientid/";
        private static string bacnettopic = "thing/bacnet/clientid/";
        private static string controltopic = "thing/control/clientid/";
        private static string accesstopic = "thing/access/clientid/";
        private static string sqlservertopic = "thing/sqlserver/clientid/";

        private static string clientid = Guid.NewGuid().ToString().Substring(0, 5);
        private static MqttServerHelper instance;
        private static MainForm mainform = null;
        private static bool bIsCheck = false;
        private static bool bIsRuning = false;

        public static MqttServerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new MqttServerHelper();
            }

            MqttServerHelper.bIsRuning = false;
            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, MainForm mainform)
        {
            Config(config, mainform);
            MqttServerHelper.bIsRuning = true;
            if (bIsCheck)
            {
                Task.Run(async () => { await ConnectMqttServerAsync(); });
            }
        }

        public static void Stop()
        {
            if (bIsCheck)
            {
                if (mqttServer != null)
                {
                    if (bIsRuning)
                    {
                        Task.Run(async () =>
                        {
                            await mqttServer.StopAsync();
                            bIsRuning = false;
                            mainform.Log("mqtt server:" + clientid + " connected");
                        });
                    }
                }
            }
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["mqttPubTopic"] != null)
            {
               pubtopic = (string)config["mqttPubTopic"].Value;
            }

            if (config["mqttClientId"] != null)
            {
                clientid = (string)config["mqttClientId"].Value;
            }

            if (config["mqttServerPort"] != null)
            {
                port = int.Parse(config["mqttServerPort"].Value);
            }

            if (config["mqttbridgeIsCheck"] != null)
            {
                bIsCheck = StringHelper.StrTobool(config["mqttbridgeIsCheck"].Value);
            }

            if (config["PLCTopic"] != null)
            {
                plctopic = config["PLCTopic"].Value;
            }

            if (config["OPCDATopic"] != null)
            {
                opcdatopic = config["OPCDATopic"].Value;
            }

            if (config["OPCUATopic"] != null)
            {
                opcuatopic = config["OPCUATopic"].Value;
            }

            if (config["BACnetTopic"] != null)
            {
                bacnettopic = config["BACnetTopic"].Value;
            }

            if (config["ControlTopic"] != null)
            {
                controltopic = config["ControlTopic"].Value;
            }

            if (config["AccessTopic"] != null)
            {
                accesstopic = config["AccessTopic"].Value;
            }

            if (config["SqlServerTopic"] != null)
            {
                sqlservertopic = config["SqlServerTopic"].Value;
            }

            MqttServerHelper.mainform = mainform;
        }

        private static async Task ConnectMqttServerAsync()
        {
            if (mqttServer == null)
            {
                var serveroptions = new MqttServerOptions
                {
                    DefaultEndpointOptions = { Port = port }
                };
                mqttServer = new MqttServerFactory().CreateMqttServer(serveroptions) as MqttServer;
                mqttServer.ApplicationMessageReceived += MqttServer_ApplicationMessageReceived;
                mqttServer.ClientConnected += MqttServer_Connected;
                mqttServer.ClientDisconnected += MqttServer_Disconnected;
            }

            try
            {
                await mqttServer.StartAsync();

                Thread.Sleep(1000);
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
        private static void MqttServer_Connected(object sender, EventArgs e)
        {
            mainform.Log("mqtt server:" + clientid + " connected");
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttServer_Disconnected(object sender, EventArgs e)
        {
              mainform.Log("mqtt server:" + clientid + " disconnected");
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttServer_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string data = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            mainform.Log("mqtt server recv :topic: " + e.ApplicationMessage.Topic.ToString() + " payload: " + data + " ClientId " + e.ClientId);

            Regex r_pubtopic = new Regex(pubtopic + clientid); // 定义一个Regex对象实例
            Match m_pubtopic = r_pubtopic.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
            if (m_pubtopic.Success)
            {
                MqttClientHelper.Write(e.ApplicationMessage);
            }
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
                var appMsg = new MqttApplicationMessage(pubtopic + clientid, Encoding.UTF8.GetBytes(mainform.Logdata(data, offset, len)), MqttQualityOfServiceLevel.AtLeastOnce, false);
                mainform.Log("mqtt Server publish:" + mainform.Logdata(data, offset, len));
                mqttServer.Publish(appMsg);
            }
        }

        public static void Write(MqttApplicationMessage appMsg)
        {
            if (bIsCheck)
            {
                mqttServer.Publish(appMsg);
            }
        }

        private static Dictionary<string, object> Get_payload(byte[] payload)
        {
            string data = Encoding.UTF8.GetString(payload);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(data);
            return json;
        }

        private static Dictionary<string, object> Get_payload(byte[] payload, int offset, int len)
        {
            string data = Encoding.UTF8.GetString(payload, offset, len);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(data);
            return json;
        }

        private static readonly DateTime BaseTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将unixtime转换为.NET的DateTime
        /// </summary>
        /// <param name="timeStamp">秒数</param>
        /// <returns>转换后的时间</returns>
        public static DateTime FromUnixTime(long timeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime((timeStamp * 10000000) + BaseTime.Ticks));
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