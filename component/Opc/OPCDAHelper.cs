// <copyright file="OPCDAHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://github.com/titanium-as/TitaniumAS.Opc.Client
// https://github.com/chkr1011/MQTTnet
namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using Da;
    using Newtonsoft.Json;
    using TitaniumAS.Opc.Client.Common;
    using TitaniumAS.Opc.Client.Da;
    using TitaniumAS.Opc.Client.Da.Browsing;

    public class OPCDAHelper : IItemsValueChangedCallBack
    {
        private static string topic = "thing/opcda/";
        private static OPCDaImp opcDa = new OPCDaImp();

        private static string opchost = "127.0.0.1";
        private static string serviceProgId = "";
        private static string groupId = "";
        private static List<string> serviceList = new List<string> { };
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
            Scan();
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

            if (config["OpcHost"] != null)
            {
                opchost = config["OpcHost"].Value;
            }

            if (config["OpcServer"] != null)
            {
                serviceProgId = config["OpcServer"].Value;
            }

            if (config["OpcGroup"] != null)
            {
                groupId = config["OpcGroup"].Value;
            }

            if (config["OPCDACheck"] != null)
            {
                bChecked = DgiotHelper.StrTobool(config["OPCDACheck"].Value);
            }
        }

        public static List<string> GetItems()
        {
            LogHelper.Log("serviceProgId " + serviceProgId + " groupId " + groupId);
            return opcDa.GetItems(serviceProgId, groupId);
        }

        public static void View()
        {
            DgiotHelper.GetIps().ForEach(ip =>
            {
                serviceList.AddRange(opcDa.ScanOPCDa(ip, true));
            });
        }

        public static void Scan()
        {
            serviceList.ForEach(service =>
            {
                opcDa.GetTreeNodes(service);
            });
        }

        public static void Read()
        {
            JsonObject items = new JsonObject();
            List<string> arry = new List<string>();
            arry.Add("GCU331_YJ.SX_PZ96_U_55");
            arry.Add("GCU331_YJ.SX_PZ96_I_55");
            arry.Add("GCU331_YJ.SX_PZ96_P_55");
            arry.Add("GCU331_YJ.SX_PZ96_U_160");
            arry.Add("GCU331_YJ.SX_PZ96_I_160");
            arry.Add("GCU331_YJ.SX_PZ96_P_160");
            arry.Add("GCU331_YJ.p_L_1");
            arry.Add("GCU331_YJ.p_L_2");
            arry.Add("GCU331_YJ.p_Q_2");
            arry.Add("GCU331_YJ.Q_Q_DN65");
            arry.Add("GCU331_YJ.Q_Q_DN100");
            arry.Add("GCU331_YJ.Q_Q_DN125");

            // Read_group(serviceProgId, "GCU331_YJ", arry, items);
        }

        public void ValueChangedCallBack(string group, OpcDaItemValue[] values)
        {
            GroupEntity entity = new GroupEntity();
            entity.Name = group;
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

            entity.Items = collection;
            string json = JsonConvert.SerializeObject(entity);
            LogHelper.Log("Group: " + json.ToString());
        }
    }
 }