// <copyright file="MqttServerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
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

        private static string clientid = Guid.NewGuid().ToString().Substring(0, 5);
        private static MqttServerHelper instance;
        private static bool bIsCheck = false;
        private static bool bIsRuning = false;

        public static MqttServerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new MqttServerHelper();
            }

            bIsRuning = false;
            return instance;
        }

        public static void Start()
        {
            Config();
            bIsRuning = true;
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
                            LogHelper.Log("mqtt server:" + clientid + " connected");
                        });
                    }
                }
            }
        }

        public static void Config()
        {
            clientid = ConfigHelper.GetConfig("MqttClientId");
            pubtopic = ConfigHelper.GetConfig("MqttPubTopic");
            port = int.Parse(ConfigHelper.GetConfig("DgiotPort"));
            if (DgiotHelper.StrTobool(ConfigHelper.GetConfig("MqttClient_Checked")) && DgiotHelper.StrTobool(ConfigHelper.GetConfig("Bridge_Checked")))
            {
                bIsCheck = true;
            }
            else
            {
                bIsCheck = false;
            }
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
                LogHelper.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 服务器连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttServer_Connected(object sender, EventArgs e)
        {
            LogHelper.Log("mqtt server:" + clientid + " connected");
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttServer_Disconnected(object sender, EventArgs e)
        {
            LogHelper.Log("mqtt server:" + clientid + " disconnected");
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttServer_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string data = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            LogHelper.Log("mqtt server recv :topic: " + e.ApplicationMessage.Topic.ToString() + " payload: " + data + " ClientId " + e.ClientId);

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
                var appMsg = new MqttApplicationMessage(pubtopic + clientid, Encoding.UTF8.GetBytes(LogHelper.Logdata(data, offset, len)), MqttQualityOfServiceLevel.AtLeastOnce, false);
                LogHelper.Log("mqtt Server publish:" + LogHelper.Logdata(data, offset, len));
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
    }
}