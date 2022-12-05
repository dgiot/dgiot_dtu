// <copyright file="MqttClientHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using LitJson;
    using MQTTnet;
    using MQTTnet.Core;
    using MQTTnet.Core.Client;
    using MQTTnet.Core.Packets;
    using MQTTnet.Core.Protocol;

    public class MqttClientHelper
    {
        private MqttClientHelper()
        {
        }

        private static MqttClient mqttClient = null;
        private static string server = "prod.cloud.com";
        private static int port = 1883;
        private static string subtopic = "$dg/device/";
        private static string pubtopic = "$dg/thing/";
        private static string clientid = "";
        private static string username = "dgiot";
        private static string password = "dgiot";
        private static MqttClientHelper instance = null;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
        private static bool bAutoReconnect = false;
        private static string dtuAddr = "";

        public static MqttClientHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new MqttClientHelper();
            }

            return instance;
        }

        public static void Start()
        {
            Config();
            bIsRunning = true;
            if (bIsCheck)
            {
                Task.Run(async () => { await ConnectMqttServerAsync(); });
            }
        }

        public static void Stop()
        {
            if (mqttClient != null)
            {
                bAutoReconnect = false;
                bIsRunning = false;
                Task.Run(async () => { await DisConnectMqttServerAsync(); });
            }
        }

        public static void Config()
        {
            bAutoReconnect = DgiotHelper.StrTobool(ConfigHelper.GetConfig("ReconnectChecked"));
            server = ConfigHelper.GetConfig("DgiotSever");
            LogHelper.Log("DgiotPort " + ConfigHelper.GetConfig("DgiotPort"));
            port = int.Parse(ConfigHelper.GetConfig("DgiotPort"));
            clientid = ConfigHelper.GetConfig("MqttClientId");
            username = ConfigHelper.GetConfig("MqttUserName");
            password = ConfigHelper.GetConfig("MqttPassword");
            pubtopic = ConfigHelper.GetConfig("MqttPubTopic");
            subtopic = ConfigHelper.GetConfig("MqttSubTopic");
            dtuAddr = ConfigHelper.GetConfig("DtuAddr");
            bIsCheck = DgiotHelper.StrTobool(ConfigHelper.GetConfig("MqttClient_Checked"));
        }

        public void Publish(byte[] payload)
        {
            var appMsg = new MqttApplicationMessage(pubtopic, payload, MqttQualityOfServiceLevel.AtLeastOnce, false);
            mqttClient.PublishAsync(appMsg);
        }

        public static void Publish(string pubtopic, byte[] payload)
        {
            if (mqttClient != null && mqttClient.IsConnected)
            {
                var appMsg = new MqttApplicationMessage(pubtopic, payload, MqttQualityOfServiceLevel.AtLeastOnce, false);
                mqttClient.PublishAsync(appMsg);
            }
        }

        private static async Task ReConnectMqttServerAsync()
        {
            while (bIsRunning)
            {
                if (!bAutoReconnect)
                {
                    break;
                }

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
                    Server = server,
                    ClientId = clientid,
                    UserName = username,
                    Password = password,
                    Port = port,
                    CleanSession = true
                };

               await DisConnectMqttServerAsync();
               await mqttClient.ConnectAsync(options);
               await ReConnectMqttServerAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.ToString());
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
                LogHelper.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 服务器连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Connected(object sender, EventArgs e)
        {
            LogHelper.Log("mqtt client:" + clientid + " connected");

            mqttClient.SubscribeAsync(new TopicFilter(subtopic, MqttQualityOfServiceLevel.AtLeastOnce));

            LogHelper.Log("mqtt client subscribe topic: " + subtopic);
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_Disconnected(object sender, EventArgs e)
        {
            if (bAutoReconnect)
            {
                _ = ReConnectMqttServerAsync();
            }
            else
            {
                LogHelper.Log("mqtt:" + clientid + " disconnected");
            }
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Dictionary<string, object> json = Get_payload(e.ApplicationMessage.Payload);
            string topic = e.ApplicationMessage.Topic;
            LogHelper.Log("mqtt recv:topic: " + topic);
            Regex r_subtopic = new Regex(subtopic); // 定义一个Regex对象实例
            Match m_subtopic = r_subtopic.Match(e.ApplicationMessage.Topic); // 在字符串中匹配
            if (m_subtopic.Success)
            {
                SerialPortHelper.Write(e.ApplicationMessage.Payload, 0, e.ApplicationMessage.Payload.Length);
            }
            if (topic.IndexOf("$dg/device/" + username + "/" + dtuAddr) == 0)
            {
                if (json.ContainsKey("cmd"))
                {
                    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    JsonData jsonPayload = JsonMapper.ToObject<JsonData>(payload);//obj是json格式的string
                    LogHelper.Log("cmd: " + jsonPayload["cmd"].ToJson());
                    JsonData jsonData = JsonMapper.ToObject<JsonData>(jsonPayload["data"].ToJson());
                    if (json["cmd"].ToString() == "opc_items")
                    {
                        OPCDAHelper.Readitems(json);
                    }
                    else if (json["cmd"].ToString() == "opc_report")
                    {
                        OPCDAHelper.Publishvalues(json);
                    }
                    else if (json["cmd"].ToString() == "scan_printer")
                    {
                        PrinterHelper.GetPrinter();
                    }
                    else if (json["cmd"].ToString() == "printer_barcode")
                    {
                        LogHelper.Log("data: " + jsonPayload["data"].ToJson());
                        PrinterHelper.PrintBarCode(jsonData);
                    }
                    else if (json["cmd"].ToString() == "printer_pdf")
                    {
                        LogHelper.Log("data: " + jsonPayload["data"].ToJson());
                        PrinterHelper.PrintPdf(jsonData);
                    }
                }
            }

            AccessHelper.Do_mdb(topic,  json, clientid);

            MqttServerHelper.Write(e.ApplicationMessage);
        }

        public static void Write(byte[] data, int offset, int len)
        {
            if (bIsCheck)
            {
                var appMsg = new MqttApplicationMessage(pubtopic + clientid, Encoding.UTF8.GetBytes(LogHelper.Logdata(data, offset, len)), MqttQualityOfServiceLevel.AtLeastOnce, false);
                LogHelper.Log("mqtt client publish:" + LogHelper.Logdata(data, offset, len));
                mqttClient.PublishAsync(appMsg);
           }
        }

        public static void Write(MqttApplicationMessage appMsg)
        {
            if (bIsCheck)
            {
                mqttClient.PublishAsync(appMsg);
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