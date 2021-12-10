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
    using System.Windows.Forms;
    using Da;
    using Newtonsoft.Json;
    using TitaniumAS.Opc.Client.Da;

    public class OPCDAHelper : IItemsValueChangedCallBack
    {
        private static string topic = "thing/opcda/";
        private static OPCDaImp opcDa = new OPCDaImp();
        private static OPCDAHelper instance = null;
        private static string clientid = string.Empty;
        private static bool bChecked = false;

        public OPCDAHelper()
        {
            opcDa.SetItemsValueChangedCallBack(this);
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
            if (config["OPCDATopic"] != null)
            {
                topic = config["OPCDATopic"].Value;
            }

            if (config["OPCDACheck"] != null)
            {
                bChecked = DgiotHelper.StrTobool(config["OPCDACheck"].Value);
            }
        }

        public static void StartMonitor(bool isCheck)
        {
            if (isCheck)
            {
                opcDa.StartGroup(OPCDAViewHelper.GetRootNode());
            }
            else
            {
                opcDa.StopGroup();
            }
        }

        public static void View()
        {
            DgiotHelper.GetIps().ForEach(host =>
            {
                opcDa.ScanOPCDa(host, true).ForEach(service =>
                {
                    OpcDaService server = opcDa.GetOpcDaService(host, service);
                    OPCDAViewHelper.GetTreeNodes(server);
                });
            });
        }

        public void ValueChangedCallBack(string groupKey, OpcDaItemValue[] values)
        {
            Thing thing = new Thing();
            thing.Proctol = "OPCDA";
            thing.Device = groupKey;
            LogHelper.Log("groupKey: " + groupKey);
            List<Item> collection = new List<Item>();
            values.ToList().ForEach(v =>
            {
                Item i = new Item();
                if (v.Item != null)
                {
                    i.ItemId = v.Item.ItemId;
                    i.Data = v.Value;
                    i.Type = v.Value.GetType().ToString();
                    collection.Add(i);
                }
            });

            thing.Items = collection;
            string json = JsonConvert.SerializeObject(thing);

            LogHelper.Log("thing: " + json);
        }
    }
 }