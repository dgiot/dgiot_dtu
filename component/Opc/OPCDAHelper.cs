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
    using System.Windows.Forms;
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
        private static List<string> opchostList = new List<string> { };

        private static string serviceProgId = "";
        private static List<string> serviceProgIdList = new List<string> { };

        private static string groupId = "";
        private static List<string> groupIdList = new List<string> { };
        private static OPCDAHelper instance = null;
        private static string clientid = string.Empty;

        private static bool bChecked = false;

        private static readonly List<string> Filter = new List<string>
        {
            "_AdvancedTags",
            "_ConnectionSharing",
            "_CustomAlarms",
            "_DataLogger",
            "_EFMExporter",
            "_IDF_for_Splunk",
            "_IoT_Gateway",
            "_LocalHistorian",
            "_Redundancy",
            "_Scheduler",
            "_SecurityPolicies",
            "_SNMP Agent",
            "_System",
            "_ThingWorx"
        };

        private static List<string> groupfilter = new List<string>
        {
            "._Statistics",
            "._System",
            ".SystemVariable"
        };

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

        public static List<string> GetServer()
        {
            opcDa.ScanOPCDa(opchost);
            return opcDa.GetServices(opchost);
        }

        public static List<string> GetGroup()
        {
            groupIdList.Clear();
            groupIdList = opcDa.GetGroups(opchost, serviceProgId);
            return groupIdList;
        }

        public static List<string> GetItems()
        {
            LogHelper.Log("serviceProgId " + serviceProgId + " groupId " + groupId);
            return opcDa.GetItems(serviceProgId, groupId);
        }

        public static void Scan()
        {
            if (bChecked)
            {
                DgiotHelper.GetIps().ForEach(ip =>
                {
                    opcDa.GetServices(ip).ForEach(service =>
                    {
                        LogHelper.Log("service " + service);
                        opcDa.GetGroups(opchost, service).ForEach(group =>
                        {
                            LogHelper.Log("group " + group);
                        });
                    });
                });
            }
        }

        public static void Write()
        {
            Uri url = UrlBuilder.Build(serviceProgId);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    // Connect to the server first.
                    server.Connect();
                    var browser = new OpcDaBrowserAuto(server);
                    JsonObject scan = new JsonObject();
                    BrowseChildren(serviceProgId, scan, browser);
                    MqttClientHelper.Publish(topic + "/metadata/derived", Encoding.UTF8.GetBytes(scan.ToString()));
                }
            }
            catch (Exception)
            {
            }
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

        private static void BrowseChildren(string serviceProgId, JsonObject json, IOpcDaBrowser browser, string group = null, int indent = 0)
        {
            // When itemId is null, root elements will be browsed.
            OpcDaBrowseElement[] elements = browser.GetElements(group);
            List<string> itemIds = new List<string> { };
            JsonObject items = new JsonObject();
            bool flag = false;
            foreach (OpcDaBrowseElement element in elements)
            {
                LogHelper.Log("ItemId  " + element.ItemId.ToString() + " " + indent.ToString());
                if (!(element.ItemId.IndexOf('$') == 0))
                {
                    if (IsPass(indent, element.ItemId.ToString()))
                    {
                        continue;
                    }

                    if (element.ItemId != null)
                    {
                        items.Add(element.ItemId.ToString(), element.Name.ToString());
                        itemIds.Add(element.ItemId.ToString());
                        flag = true;
                    }

                    // Skip elements without children.
                    if (element.HasChildren)
                    {
                        // Output children of the element.
                        BrowseChildren(serviceProgId, json, browser, element.ItemId, indent + 2);
                    }
                }
            }

            if (flag)
            {
                if (group != null)
                {
                    if (itemIds.Count == 0)
                    {
                        LogHelper.Log("group " + group);
                    }
                    else
                    {
                        opcDa.StartMonitoringItems(serviceProgId, itemIds, group);
                    }
                }
            }
        }

        private static bool IsGroupfilter(string item)
        {
            bool result = false;
            foreach (string filter in groupfilter)
            {
                 // LogHelper.Log("item  " + item + " filter " + filter + " " + item.LastIndexOf(filter));
                 if (-1 != item.LastIndexOf(filter))
                 {
                    return true;
                }
            }

            return result;
        }

        private static bool IsPass(int indent, string item)
        {
            if (indent == 0)
            {
                return Filter.IndexOf(item) != -1;
            }

            if (indent == 2)
            {
                return IsGroupfilter(item);
            }

            if (indent == 4)
            {
                return IsGroupfilter(item);
            }

            return true;
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