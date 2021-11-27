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
    using System.Text.RegularExpressions;
    using Da;
    using MQTTnet.Core.Client;
    using TitaniumAS.Opc.Client.Common;
    using TitaniumAS.Opc.Client.Da;
    using TitaniumAS.Opc.Client.Da.Browsing;

    public class OPCDAHelper
    {
        private const bool V = false;
        private static string topic = "thing/opcda/";
        private static string opcip = "127.0.0.1";
        private static List<string> serverlist = new List<string> { };
        private static OPCDAHelper instance = null;
        private static string clientid = string.Empty;
        private static bool bIsRun = V;
        private static bool bIsCheck = false;
        private static SocketService socketserver = new SocketService();
        private static List<TreeNode> dataList = null;
        private static List<string> filter = new List<string>
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
            "._System"
        };

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
            socketserver.Start();
            bIsRun = true;
        }

        public static void Stop()
        {
            socketserver.Stop();
            bIsRun = false;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["OPCDAIsCheck"] != null)
            {
                bIsCheck = DgiotHelper.StrTobool(config["OPCDAIsCheck"].Value);
            }

            if (config["OPCDATopic"] != null)
            {
                topic = config["OPCDATopic"].Value;
            }

            if (config["OpcServer"] != null)
            {
                opcip = config["OpcServer"].Value;
            }
        }

        public static void Do_opc_da(MqttClient mqttClient, string topic, Dictionary<string, object> json, string clientid)
        {
            Regex r_subopcda = new Regex(OPCDAHelper.topic); // 定义一个Regex对象实例
            Match m_subopcda = r_subopcda.Match(topic); // 在字符串中匹配

            if (!m_subopcda.Success)
            {
                return;
            }

            LogHelper.Log(topic);
            OPCDAHelper.clientid = clientid;
            string cmdType = "read";
            if (json.ContainsKey("cmdtype"))
            {
                try
                {
                    cmdType = (string)json["cmdtype"];
                    switch (cmdType)
                    {
                        case "scan":
                            break;
                        case "read":
                            Read_opc_da(json);
                            break;
                        case "write":
                            break;
                        default:
                            Read_opc_da(json);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex.ToString());
                }
            }
        }

        public static List<string> GetServer()
        {
            List<string> addresses = new List<string> { opcip };
            dataList = socketserver.ScanOPCClassicServer(addresses);
            serverlist.Clear();
            foreach (TreeNode node in dataList)
            {
                if (node.Children.Any())
                {
                    foreach (TreeNode childnode in node.Children)
                    {
                        serverlist.Add(childnode.Name);
                    }
                }
            }

            return serverlist;
        }

        public static void Scan()
        {
            foreach (string opcserver in serverlist)
            {
                Uri url = UrlBuilder.Build(opcserver);
                try
                {
                    using (var server = new OpcDaServer(url))
                    {
                        // Connect to the server first.
                        server.Connect();
                        var browser = new OpcDaBrowserAuto(server);
                        JsonObject scan = new JsonObject();
                        BrowseChildren(opcserver, scan, browser);
                        MqttClientHelper.Publish(topic + "/metadata/derived", Encoding.UTF8.GetBytes(scan.ToString()));
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static void Write()
        {
        }

        private static void Recursion(TreeNode childNode)
        {
            if (childNode.Children.Any())
            {
                var children = childNode.Children.ToList();
                children.ForEach((child) =>
                {
                    Recursion(child);
                });
            }
        }

        private static void BrowseChildren(string opcserver, JsonObject json, IOpcDaBrowser browser, string itemId = null, int indent = 0)
        {
            // When itemId is null, root elements will be browsed.
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            List<string> array = new List<string> { };
            JsonObject items = new JsonObject();
            bool flag = false;
            foreach (OpcDaBrowseElement element in elements)
            {
                if (IsPass(indent, element.ItemId.ToString()))
                {
                    continue;
                }

                // Skip elements without children.
                if (!element.HasChildren)
                {
                    items.Add(element.ItemId.ToString(), element.Name.ToString());
                    array.Add(element.ItemId.ToString());
                    flag = true;
                    continue;
                }

                // Output children of the element.
                BrowseChildren(opcserver, json, browser, element.ItemId, indent + 2);
            }

            if (flag)
            {
                if (itemId != null)
                {
                    JsonObject thing = GetItems(opcserver, itemId, array, items);
                    JsonObject payload = new JsonObject();
                    payload.Add("thing", thing);
                    payload.Add("opcserver", opcserver);
                    payload.Add("group", itemId);
                    MqttClientHelper.Publish(topic + "/metadata/derived", Encoding.UTF8.GetBytes(payload.ToString()));
                    LogHelper.Log("Scan OPCDA payload: " + payload.ToString());
                    json.Add(itemId, thing);
                }
            }
        }

        private static JsonObject GetItems(string opcserver, string group_name, List<string> array, JsonObject jsonItems)
        {
            JsonObject json = new JsonObject();
            Uri url = UrlBuilder.Build(opcserver);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    server.Connect();
                    OpcDaGroup group = server.AddGroup(group_name);
                    IList<OpcDaItemDefinition> definitions = new List<OpcDaItemDefinition>();
                    int i = 0;
                    foreach (string id in array)
                    {
                        var definition = new OpcDaItemDefinition
                        {
                            ItemId = id,
                            IsActive = true
                        };
                        definitions.Insert(i++, definition);
                    }

                    group.IsActive = true;
                    OpcDaItemResult[] results = group.AddItems(definitions);
                    OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);
                    foreach (OpcDaItemValue item in values)
                    {
                        JsonObject tmp = new JsonObject();
                        tmp.Add("Type", item.Value.GetType().ToString());
                        tmp.Add("ItemId", item.Item.ItemId.ToString());
                        if (jsonItems.ContainsKey(item.Item.ItemId.ToString()))
                        {
                            json.Add(jsonItems[item.Item.ItemId.ToString()].ToString(), tmp);
                        }
                    }

                    server.Disconnect();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.GetBaseException().ToString());
            }

            return json;
        }

        private static Boolean IsGroupfilter(string item)
        {
            Boolean result = false;
            foreach (string filter in groupfilter)
            {
                // mainform.Log("item  " + item + " filter " + filter + " " + item.LastIndexOf(filter));
                 if (-1 != item.LastIndexOf(filter))
                 {
                    return true;
                }
            }

            return result;
        }

        private static Boolean IsPass(int indent, string item)
        {
            if (indent == 0)
            {
                return filter.IndexOf(item) != -1;
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

        private static void Read_opc_da(Dictionary<string, object> json)
        {
            string opcserver = "Matrikon.OPC.Simulation.1";
            string group = "addr";
            IList<OpcDaItemDefinition> itemlist = new List<OpcDaItemDefinition>();
            if (json.ContainsKey("opcserver"))
            {
                try
                {
                    opcserver = (string)json["opcserver"];
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex.ToString());
                }
            }

            if (json.ContainsKey("group"))
            {
                try
                {
                    group = (string)json["group"];
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex.ToString());
                }
            }

            if (json.ContainsKey("items"))
            {
                try
                {
                    string items = (string)json["items"];
                    string[] arry = items.Split(',');
                    JsonObject data = new JsonObject();
                    try
                    {
                        JsonObject result = new JsonObject();
                        Read_group(opcserver, group, arry, data);
                        result.Add("status", 0);
                        result.Add(group, data);
                        LogHelper.Log("result " + result.ToString());
                        MqttClientHelper.Publish(topic + "/properties/read/reply", Encoding.UTF8.GetBytes(result.ToString()));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Log(ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex.ToString());
                }
            }
        }

        private static void Read_group(string opcserver, string group_name, string[] arry, JsonObject items)
        {
            Uri url = UrlBuilder.Build(opcserver);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    // Connect to the server first.
                    server.Connect();

                    // Create a group with items.
                    OpcDaGroup group = server.AddGroup(group_name);
                    IList<OpcDaItemDefinition> definitions = new List<OpcDaItemDefinition>();
                    int i = 0;
                    foreach (string id in arry)
                    {
                        var definition = new OpcDaItemDefinition
                        {
                            ItemId = id,
                            IsActive = true
                        };
                        definitions.Insert(i++, definition);
                    }

                    group.IsActive = true;
                    OpcDaItemResult[] results = group.AddItems(definitions);
                    OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);

                    // Handle adding results.
                    JsonObject data = new JsonObject();
                    foreach (OpcDaItemValue item in values)
                    {
                        LogHelper.Log(topic + "/properties/read/reply" + " " + item.GetHashCode().ToString() + " " + item.Value.ToString() + string.Empty + item.Timestamp.ToString());
                        data.Add(item.Item.ItemId, item.Value);
                    }

                    items.Add("status", 0);
                    items.Add(group_name, data);
                    LogHelper.Log(items.ToString());
                    MqttClientHelper.Publish(topic + "/properties/read/reply", Encoding.UTF8.GetBytes(items.ToString()));
                    server.Disconnect();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.ToString());
            }
        }

        private static void Subscription_opc_da(string opcserver, string name)
        {
            Uri url = UrlBuilder.Build(opcserver);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    // Connect to the server first.
                    server.Connect();

                    // Create a group with items.
                    OpcDaGroup group = server.AddGroup("Group1");
                    group.IsActive = true;

                    var definition = new OpcDaItemDefinition
                    {
                        ItemId = name,
                        IsActive = true
                    };

                    OpcDaItemDefinition[] definitions = { definition };

                    OpcDaItemResult[] results = group.AddItems(definitions);

                    group.ValuesChanged += OnGroupValuesChanged;
                    group.UpdateRate = TimeSpan.FromMilliseconds(100);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.GetBaseException().ToString());
                JsonObject result = new JsonObject();
                result.Add("opcserver", opcserver);
                result.Add("name", name);
                result.Add("status", ex.GetHashCode());
                result.Add("err", ex.ToString());
                MqttClientHelper.Publish(topic + "/properties/read/reply", Encoding.UTF8.GetBytes(result.ToString()));
            }
        }

        private static void OnGroupValuesChanged(object sender, OpcDaItemValuesChangedEventArgs args)
        {
            // Output values.
            foreach (OpcDaItemValue value in args.Values)
            {
                LogHelper.Log("ItemId: " + value.Item.ItemId.ToString() + "; Value: {1}" + value.Value.ToString() +
                    ";Quality: " + value.Quality.ToString() + ";Timestamp: {3}" + value.Timestamp.ToString());
            }
        }

        private static DateTime baseTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将unixtime转换为.NET的DateTime
        /// </summary>
        /// <param name="timeStamp">秒数</param>
        /// <returns>转换后的时间</returns>
        public static DateTime FromUnixTime(long timeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime((timeStamp * 10000000) + baseTime.Ticks));
        }

        /// <summary>
        /// 将.NET的DateTime转换为unix time
        /// </summary>
        /// <param name="dateTime">待转换的时间</param>
        /// <returns>转换后的unix time</returns>
        public static long FromDateTime(DateTime dateTime)
        {
            return (TimeZone.CurrentTimeZone.ToUniversalTime(dateTime).Ticks - baseTime.Ticks) / 10000000;
        }
    }
    }