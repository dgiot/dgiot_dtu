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
        private static OPCDaImp opcDa = new OPCDaImp();

        private static string opchost = "127.0.0.1";
        private static List<string> opchostList = new List<string> { };

        private static string serviceProgId = "";
        private static List<string> serviceProgIdList = new List<string> { };

        private static string groupId = "";
        private static List<string> groupIdList = new List<string> { };

        private static List<TreeNode> opcDaServerList = new List<TreeNode>();
        private static List<TreeNode> treeNodeCaches = new List<TreeNode>();
        private static OPCDAHelper instance = null;
        private static string clientid = string.Empty;

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
        }

        public static List<string> GetServer()
        {
            serviceProgIdList.Clear();
            string[] progIds = opcDa.ScanOPCDa(opchost);
            foreach (string progId in progIds)
            {
                serviceProgIdList.Add(progId);
                LogHelper.Log("ProgId: " + progId);
            }

            return serviceProgIdList;
        }

        public static List<string> GetGroup()
        {
            groupIdList.Clear();
            groupIdList = opcDa.GetGroups(serviceProgId);
            return groupIdList;
        }

        public static List<TreeNode> ScanOPCClassicServer()
        {
            opcDaServerList.Clear();
            string[] opcDaList = opcDa.ScanOPCDa(opchost);
            if (opcDaList.Length > 0)
            {
                TreeNode node = new TreeNode();
                node.Name = opchost.ToString();
                node.NodeType = TreeNodeType.Local;
                List<TreeNode> childNodes = new List<TreeNode>();
                foreach (var opcItem in opcDaList)
                {
                    childNodes.Add(new TreeNode() { Name = opcItem });
                }

                node.Children.AddRange(childNodes);
                opcDaServerList.Add(node);
            }

            return opcDaServerList;
        }

        public static List<TreeNode> ScanOPCServerData(List<TreeNode> opcServerNodes)
        {
            opcServerNodes.ForEach((service) =>
            {
                service.Children.ForEach((opc) =>
                {
                    IList<TreeNode> dataNodes = opcDa.GetTreeNodes(opc.Name);
                    opc.Children.AddRange(dataNodes);
                });
            });

            return opcServerNodes;
        }

        public static void Scan()
        {
            List<TreeNode> servers = ScanOPCClassicServer();
            foreach (TreeNode server in servers)
            {
                Recursion(server);
            }

            List<TreeNode> tempDataList = ScanOPCServerData(opcDaServerList);
            treeNodeCaches.Clear();
            treeNodeCaches.AddRange(tempDataList);

            // OpcDaService server1 = OpcDa.GetOpcDaService(serviceProgId);
            // if (server1 != null)
            // {
            //    LogHelper.Log("Host " + server1.Host + " ServiceId " + server1.ServiceId + " serviceProgId " + serviceProgId);
            //     treeNodeCaches = (List<TreeNode>)OpcDa.GetTreeNodes(serviceProgId);
            //    treeNodeCaches.ForEach((node) =>
            //    {
            //        Recursion(node);
            //     });
            // }
            Write();
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

        private static void Recursion(TreeNode childNode)
        {
            LogHelper.Log("name " + childNode.Name + " " + childNode.NodeType.ToString());
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

        private static Boolean IsGroupfilter(string item)
        {
            Boolean result = false;
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