// <copyright file="OPCDAHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
namespace Dgiot_dtu
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Da;
    using TitaniumAS.Opc.Client.Da;

    public class OPCDAHelper : IItemsValueChangedCallBack
    {
        private static readonly OPCDaImp OpcDa = new OPCDaImp();
        private static OPCDAHelper instance = null;
        private static string productId = string.Empty;
        private static string devAddr = string.Empty;
        private static bool bChecked = false;
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

        public static void Start(KeyValueConfigurationCollection config)
        {
            Config(config);
        }

        public static void Stop()
        {
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["OPCDACheck"] != null)
            {
                bChecked = DgiotHelper.StrTobool(config["OPCDACheck"].Value);
            }

            if (config["OPCDAInterval"] != null)
            {
                interval = int.Parse(config["OPCDAInterval"].Value) * 1000;
            }

            if (config["mqttUserName"] != null)
            {
                productId = config["mqttUserName"].Value;
            }

            if (config["devAddr"] != null)
            {
                devAddr = config["devAddr"].Value;
            }
        }

        public static void StartMonitor()
        {
            if (bChecked)
            {
                OpcDa.StartGroup(OPCDAViewHelper.GetRootNode(), interval);
            }
            else
            {
                OpcDa.StopGroup();
            }
        }

        public static void View()
        {
            DgiotHelper.GetIps().ForEach(host =>
            {
                OpcDa.ScanOPCDa(host, true).ForEach(service =>
                {
                    OpcDaService server = OpcDa.GetOpcDaService(host, service);
                    OPCDAViewHelper.GetTreeNodes(server);
                });
            });
        }

        public void ValueChangedCallBack(OpcDaGroup group, OpcDaItemValue[] values)
        {
            JsonObject result = new JsonObject();
            result.Add("timestamp", DgiotHelper.Now());

            if (group.UserData != null)
            {
                TreeNode node = group.UserData as TreeNode;
                result.Add("deviceAddr", DgiotHelper.Now());
            }

            JsonObject properties = new JsonObject();
            List<Item> collection = new List<Item>();
            values.ToList().ForEach(v =>
            {
                Item i = new Item();
                if (v.Item != null)
                {
                    properties.Add(v.Item.ItemId, v.Value);
                }
            });
            result.Add("properties", properties);
            string topic = "/" + productId + "/" + devAddr + "/topo/" + group.Name + "/post";
            LogHelper.Log("topic " + topic + " payload: " + result);
            MqttClientHelper.Publish(topic, Encoding.UTF8.GetBytes(result.ToString()));
        }
    }
 }