// <copyright file="OPCDAHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
namespace Dgiot_dtu
{
    using Da;

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TitaniumAS.Opc.Client.Da;

    public class OPCDAHelper : IItemsValueChangedCallBack
    {
        private static readonly OPCDaImp OpcDa = new OPCDaImp();
        private static OPCDAHelper instance = null;
        private static string productId = string.Empty;
        private static string devAddr = string.Empty;
        private static string host = "127.0.0.1";
        private static int interval = 1000;

        public OPCDAHelper()
        {
            OpcDa.SetItemsValueChangedCallBack(this);
        }

        public static OPCDAHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new OPCDAHelper();
            }

            return instance;
        }

        public static void Start()
        {
            Config();
            OPCDAViewHelper.View();
            View();
            //SqliteHelper.Init("(id,name)");
        }

        public static void Stop()
        {
        }

        public static void Config()
        {
            host = ConfigHelper.GetConfig("OPCDAHost");
            interval = int.Parse(ConfigHelper.GetConfig("OPCDAInterval"));
            productId = ConfigHelper.GetConfig("MqttUserName");
            devAddr = ConfigHelper.GetConfig("DtuAddr");
        }

        public static void StartMonitor()
        {
            if (DgiotHelper.StrTobool(ConfigHelper.GetConfig("OPCDACheck")))
            {
                interval = int.Parse(ConfigHelper.GetConfig("OPCDAInterval"));
                int count = int.Parse(ConfigHelper.GetConfig("OPCDACount"));
                OpcDa.StartGroup(OPCDAViewHelper.GetRootNode(), interval, count);
            }
            else
            {
                OpcDa.StopGroup();
            }
        }

        public static void View()
        {
            OpcDa.ScanOPCDa(host, true).ForEach(service =>
            {
                OpcDaService server = OpcDa.GetOpcDaService(host, service);
                //OPCDAViewHelper.GetTreeNodes(server);
            });
        }

        public static void GetTreeNodes(string service)
        {
            OpcDaService server = OpcDa.GetOpcDaService(host, service);
            OPCDAViewHelper.GetTreeNodes(server);
        }

        public void ValueChangedCallBack(OpcDaGroup group, OpcDaItemValue[] values)
        {
            string groupKey = "";

            JsonObject properties = new JsonObject();
            values.ToList().ForEach(v =>
            {
                if (v.Item != null && v.Value != null)
                {
                    properties.Add(v.Item.ItemId, v.Value);
                    groupKey = v.Item.UserData as string;
                    OpcDa.setItems(groupKey, v.Item.ItemId, properties);
                }
            });
            int i = OpcDa.getItemsCount(groupKey);
            if (i <= 0)
            {
                properties = OpcDa.getItems(group, groupKey);
                int flag1 = OpcDa.GetGroupFlag(groupKey);
                if (flag1 > 0)
                {
                    properties.Add("dgiotcollectflag", 0);
                    // LogHelper.Log(" topic: " + topic + " payload: " + properties);
                }
                else
                {
                    properties.Add("dgiotcollectflag", 1);
                }
                properties.Add("groupid", groupKey);
                LogHelper.Log("send properties: " + properties.ToString());
                string topic = "$dg/thing/" + productId + "/" + devAddr + "/properties/report";
                MqttClientHelper.Publish(topic, Encoding.UTF8.GetBytes(properties.ToString()));
            }
        }

        public static void Additems(Dictionary<string, object> json)
        {
            string groupid = json["groupid"].ToString();
            string opcserver = json["opcserver"].ToString();
            object[] items = (object[])json["items"];
            List<string> itemlist = new List<string> { };
            foreach (object v in items)
            {
                itemlist.Add((string)v);
            }

            OpcDa.StopGroup();
            OpcDa.StartMonitor(groupid, itemlist.Distinct().ToList(), opcserver);
        }


        public static void Readitems(Dictionary<string, object> json)
        {
            string groupid = json["groupid"].ToString();
            string opcserver = json["opcserver"].ToString();
            object[] items = (object[])json["items"];

            List<string> itemlist = new List<string> { };
            foreach (object v in items)
            {
                itemlist.Add((string)v);
            }

            // OpcDa.StopGroup();
            OpcDa.StartMonitor(groupid, itemlist.Distinct().ToList(), opcserver);
            // OpcDa.ReadItemsValues1(opcserver, itemlist.Distinct().ToList(), groupid);
            // OpcDa.read_group(opcserver, groupid, itemlist.Distinct().ToList());
        }

        public static void Publishvalues(Dictionary<string, object> json)
        {
            string groupid = json["groupid"].ToString();
            int duration = (int)json["duration"];
            OpcDa.SetGroupFlag(groupid, duration);
        }
    }
}