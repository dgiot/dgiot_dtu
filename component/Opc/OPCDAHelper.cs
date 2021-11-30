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
    using Newtonsoft.Json;
    using TitaniumAS.Opc.Client.Common;
    using TitaniumAS.Opc.Client.Da;
    using TitaniumAS.Opc.Client.Da.Browsing;

    public class OPCDAHelper : IItemsValueChangedCallBack
    {
        private static string topic = "thing/opcda/";
        private static string opcip = "127.0.0.1";
        private static List<string> opcipList = new List<string> { };
        private static string serviceProgId = "";
        private static List<TreeNode> opcDaServerList = new List<TreeNode>();
        private static List<TreeNode> nodeList = new List<TreeNode>();
        private static OPCDAHelper instance = null;
        private static string clientid = string.Empty;
        private static bool bIsCheck = false;
        private static OPCDaImp OpcDa = new OPCDaImp();
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
            OpcDa.SetItemsValueChangedCallBack(this);;
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
            if (config["OPCDAIsCheck"] != null)
            {
                bIsCheck = DgiotHelper.StrTobool(config["OPCDAIsCheck"].Value);
            }

            if (config["OPCDATopic"] != null)
            {
                topic = config["OPCDATopic"].Value;
            }

            if (config["OpcIp"] != null)
            {
                opcip = config["OpcIp"].Value;
            }

            if (config["OpcServer"] != null)
            {
                serviceProgId = config["OpcServer"].Value;
            }
        }

        public static void Do_opc_da(string topic, Dictionary<string, object> json, string clientid)
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
            string[] groups = OpcDa.ScanOPCDa(opcip);
            foreach(string group in groups)
            {
                opcipList.Add(group);
                LogHelper.Log("group " + group);
            };  
            return opcipList;
        }

        public static void Scan()
        {
            OpcDaService server1 = OpcDa.GetOpcDaService(serviceProgId);
            LogHelper.Log("server1 " + server1.ToString());
            nodeList = (List <TreeNode>)OpcDa.GetTreeNodes(serviceProgId);
            nodeList.ForEach((node) =>
            {
                Recursion(node);
            });
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

        private static void Recursion(TreeNode childNode)
        {
            LogHelper.Log("node name " + childNode.Name.ToString() + " " + childNode.NodeType.ToString());
            if (childNode.Children.Any())
            {
                var children = childNode.Children.ToList();
                children.ForEach((child) =>
                {
                    Recursion(child);
                });
            }
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
                if (!(element.ItemId.IndexOf('$') == 0))
                {
                    LogHelper.Log("element " + element.ItemId.ToString() + " " + element.Name.ToString() + " " + indent.ToString());
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
                    JsonObject thing = GetItems(serviceProgId, group, itemIds, items);
                    JsonObject payload = new JsonObject();
                    payload.Add("thing", thing);
                    payload.Add("opcserver", serviceProgId);
                    payload.Add("group", group);
                    string strMd5 = DgiotHelper.Md5(serviceProgId);
                    //OpcDa.StartMonitoringItems(serviceProgId, itemIds, strMd5);
                    MqttClientHelper.Publish(topic + "/metadata/derived", Encoding.UTF8.GetBytes(payload.ToString()));
                    //LogHelper.Log("Scan OPCDA payload: " + payload.ToString());
                    json.Add(group, thing);
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
                    foreach (string id in array)
                    {
                        JsonObject tmp = new JsonObject();
                        tmp.Add("ItemId", id);
                        tmp.Add("Name", jsonItems[id].ToString());

                        foreach (OpcDaItemValue item in values)
                        {
                            if (item.Item != null && item.Item.ItemId != null)
                            {
                                if (item.Value != null)
                                {
                                    if (item.Item.ItemId.ToString() == id)
                                    {
                                        tmp.Add("Type", item.Value.GetType().ToString());
                                        break;
                                    }
                                }
                            }
                        }

                      json.Add(jsonItems[id].ToString(), tmp);
                    }

                    server.Disconnect();
                }
            }
            catch (Exception ex)
            {
                foreach (string id in array)
                {
                    JsonObject tmp = new JsonObject();
                    tmp.Add("ItemId", id);
                    tmp.Add("Name", jsonItems[id].ToString());
                    json.Add(jsonItems[id].ToString(), tmp);
                }

                // LogHelper.Log(ex.GetBaseException().ToString());
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

        public void ValueChangedCallBack(string group, OpcDaItemValue[] values)
        {
            GroupEntity entity = new GroupEntity();
            entity.Id = group;
            List<Item> collection = new List<Item>();
            values.ToList().ForEach(v =>
            {
                Item i = new Item();
                i.ItemId = v.Item.ItemId;
                i.Data = v.Value;
                i.Type = v.Value.GetType().ToString();
                collection.Add(i);
            });

            entity.Items = collection;
            string json = JsonConvert.SerializeObject(entity);
            byte[] bufferList = StructUtility.Package(new Header() { Id = 0, Cmd = (int)Command.Notify_Nodes_Values_Ex, ContentSize = json.Length }, json);
            LogHelper.Log("ValueChangedCallBack: " + json.ToString());
            //sessionDic[group].Send(bufferList, 0, bufferList.Length);
        }

        private static DateTime baseTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将unixtime转换为.NET的DateTimea
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