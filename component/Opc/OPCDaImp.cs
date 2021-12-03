// <copyright file="OPCDaImp.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Dgiot_dtu;
    using TitaniumAS.Opc.Client.Common;
    using TitaniumAS.Opc.Client.Da;
    using TitaniumAS.Opc.Client.Da.Browsing;

    // 不在一开始读取节点结构时创建缓存的原因：用户不一定会全部都读取所有节点属性值 仅仅在需要关注的时候 才读取
    public class OPCDaImp : IOPCDa
    {
        private OpcServerEnumeratorAuto serverEnumerator = new OpcServerEnumeratorAuto();
        private Dictionary<string, OpcDaGroup> daGroupKeyPairs = new Dictionary<string, OpcDaGroup>();
        private List<ServiceCollection> serviceCollection = new List<ServiceCollection>();
        private Dictionary<string, GroupEntity> groupCollection = new Dictionary<string, GroupEntity>();
        private IItemsValueChangedCallBack callBack;
        private List<OpcDaService> opcDaServices = new List<OpcDaService>();

        public string[] ScanOPCDa(string host, Boolean isClean = true)
        {
            if (isClean)
            {
                serviceCollection.Clear();
            }

            string[] retValue = new string[] { };
            try
            {
                OpcServerDescription[] opcServers = serverEnumerator.Enumerate(host, OpcServerCategory.OpcDaServer10,
                                                                            OpcServerCategory.OpcDaServer20,
                                                                            OpcServerCategory.OpcDaServer30);

                string[] serviceList = opcServers.Select(a => a.ProgId).ToArray();
                if (serviceList.Any())
                {
                    if (serviceCollection.Any(a => { return a.Host == host; }))
                    {
                        ServiceCollection item = serviceCollection.Where(a => { return a.Host == host; })
                                                                   .FirstOrDefault();
                        var exceptList = item.ServiceIds.Except(serviceList);
                        if (exceptList.Any())
                        {
                            item.ServiceIds.AddRange(exceptList);
                        }
                    }
                    else
                    {
                        serviceCollection.Add(new ServiceCollection() { Host = host, ServiceIds = serviceList.ToList() });
                    }
                }

                return opcServers.Select(a => a.ProgId).ToArray();
            }
            catch (Exception)
            {
                return retValue;
            }
        }

        public void ConnectionStateChanged(object sender, OpcDaServerConnectionStateChangedEventArgs e)
        {
            LogHelper.Log($"---ConnectionStateChanged---IsConnected ${e.IsConnected}, ${sender}");
            if (e.IsConnected == false)
            {
                Thread.Sleep(1000);
                Environment.Exit(0);
            }
        }

        public OpcDaService GetOpcDaService(string serviceProgId)
        {
            var service = serviceCollection.Where(a => a.ServiceIds.Contains(serviceProgId))
                      .FirstOrDefault();

            if (service == null)
            {
                return null;
            }

            OpcDaService service1 = null;
            if (CheckServiceExisted(service, serviceProgId))
            {
                service1 = opcDaServices.Find(item => { return item.Host == service.Host && item.ServiceId == serviceProgId; });
            }
            else
            {
                OpcDaServer daService = new OpcDaServer(serviceProgId, service.Host);

                service1 = new OpcDaService()
                {
                    Host = service.Host,

                    ServiceId = serviceProgId,

                    Service = daService,

                    OpcDaGroupS = new Dictionary<string, OpcDaGroup>()
                };
                opcDaServices.Add(service1);
            }

            if (service1.Service.IsConnected == false)
            {
                try
                {
                    service1.Service.ConnectionStateChanged += new EventHandler<OpcDaServerConnectionStateChangedEventArgs>(ConnectionStateChanged);

                    service1.Service.Connect();
                }
                catch (Exception e)
                {
                    LogHelper.Log("Connect " + service1.Host + ", ServiceId " + service1.ServiceId + "error!!" + e.Message);
                }
            }

            return service1;
        }

        public List<string> GetServices(string host)
        {
            ScanOPCDa(host);
            ServiceCollection s1 = serviceCollection.Find(item => { return item.Host == host; });

            return s1.ServiceIds;
        }

        private static List<string> groupfilter = new List<string>
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

        private static bool IsGroupfilter(string group)
        {
            bool result = false;
            foreach (string filter in groupfilter)
            {
                if (-1 != group.LastIndexOf(filter))
                {
                    return true;
                }
            }

            return result;
        }

        private static List<string> itemfilter = new List<string>
        {
            "._Statistics",
            "._System",
            ".SystemVariable"
        };

        private static bool IsItemsfilter(string item)
        {
            bool result = false;
            foreach (string filter in itemfilter)
            {
                if (-1 != item.LastIndexOf(filter))
                {
                    return true;
                }
            }

            return result;
        }

        public List<string> GetGroups(string serviceProgId)
        {
            List<string> groups = new List<string>();
            Uri url = UrlBuilder.Build(serviceProgId);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    server.Connect();
                    var browser = new OpcDaBrowserAuto(server);
                    JsonObject scan = new JsonObject();
                    OpcDaBrowseElement[] elements = browser.GetElements(null);
                    foreach (OpcDaBrowseElement element in elements)
                    {
                        if (!(element.Name.IndexOf('$') == 0))
                        {
                            if (!IsGroupfilter(element.Name))
                            {
                                LogHelper.Log("group  " + element.ItemId);
                                string deviceAddr = GetDevAddr(serviceProgId, element.Name);
                                GroupEntity group = new GroupEntity();
                                group.Name = element.Name;
                                OpcDaBrowseElement[] childElements = browser.GetElements(element.Name);
                                List<Item> items = new List<Item>();
                                foreach (OpcDaBrowseElement childElement in childElements)
                                {
                                    if (!IsItemsfilter(childElement.ItemId))
                                    {
                                        LogHelper.Log("Item  " + childElement.ItemId);
                                        Item item = new Item();
                                        item.ItemId = childElement.ItemId;
                                        items.Add(item);
                                    }
                                }

                                group.Items = items;
                                groupCollection.Add(deviceAddr, group);
                                groups.Add(element.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return groups;
        }

        public List<string> GetItems(string serviceProgId, string groupId)
        {
            List<string> items = new List<string>();
            string deviceAddr = GetDevAddr(serviceProgId, groupId);
            if (groupCollection.ContainsKey(deviceAddr) == true)
            {
                GroupEntity group = groupCollection[deviceAddr];
                if (group.Items != null)
                {
                    foreach (Item item in group.Items)
                    {
                        LogHelper.Log("ItemId " + item.ItemId);
                        items.Add(item.ItemId);
                    }
                }
            }

            return items;
        }

        public void AddItems(string serviceProgId, string groupId, List<Item> items)
        {
            OpcDaService server = GetOpcDaService(serviceProgId);
            if (server == null)
            {
                return;
            }

            string deviceAddr = GetDevAddr(serviceProgId, groupId);
            if (groupCollection.ContainsKey(deviceAddr) == true)
            {
                GroupEntity group = groupCollection[deviceAddr];
                group.Items = items;
                groupCollection.Add(deviceAddr, group);
            }
        }

        public string GetDevAddr(string serviceProgId, string group)
        {
            return DgiotHelper.Md5(serviceProgId + "_" + group);
        }

        public IList<TreeNode> GetTreeNodes(string serviceProgId)
        {
            // var service =  _serviceCollection.Where(a => a.ServiceIds.Contains(serviceProgId))
            // .FirstOrDefault();
            var server = GetOpcDaService(serviceProgId);

            List<TreeNode> nodes = new List<TreeNode>();
            try
            {
                OpcDaBrowserAuto browserAuto = new OpcDaBrowserAuto(server.Service);
                BrowseChildren(browserAuto, nodes);
            }
            catch (Exception)
            {
                return new List<TreeNode>();
            }

            return nodes;
        }

        public void BrowseChildren(IOpcDaBrowser browser, IList<TreeNode> items, string itemId = null, int indent = 0)
        {
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);

            foreach (OpcDaBrowseElement element in elements)
            {
                if (!(element.ItemId.IndexOf('$') == 0))
                {
                    TreeNode treeNode = new TreeNode() { Name = element.Name, NodeType = TreeNodeType.Property };
                    items.Add(treeNode);
                    if (element.HasChildren)
                    {
                        BrowseChildren(browser, treeNode.Children, element.ItemId, indent + 2);
                    }
                }
            }
        }

        private bool CheckServiceExisted(ServiceCollection service, string serviceProgId)
        {
          return opcDaServices.Any(item => { return item.Host == service.Host && item.ServiceId == serviceProgId; });
        }

        public string StartMonitoringItems(string serviceProgId, List<string> itemIds, string groupId)
        {
            OpcDaService server = GetOpcDaService(serviceProgId);
            if (server == null)
            {
                return null;
            }

            string strMd5 = Guid.NewGuid().ToString();
            OpcDaGroup group;
            if (server.OpcDaGroupS.ContainsKey(strMd5) == false)
            {
                // OpcDaGroup group = _server.Service.AddGroup(groupId);  // maybe cost lot of time
                group = server.Service.AddGroup(strMd5);  // maybe cost lot of time
                group.IsActive = true;

                server.OpcDaGroupS.Add(strMd5, group);

                List<OpcDaItemDefinition> itemDefList = new List<OpcDaItemDefinition>();

                itemIds.ForEach(itemId =>
                {
                    var def = new OpcDaItemDefinition();
                    def.ItemId = itemId;
                    def.IsActive = true;

                    // def.RequestedDataType = ;
                    itemDefList.Add(def);
                });
                OpcDaItemResult[] opcDaItemResults = group.AddItems(itemDefList);
                daGroupKeyPairs.Add(groupId, group);
                group.UpdateRate = TimeSpan.FromMilliseconds(1000); // 100毫秒触发一次
                group.ValuesChanged += MonitorValuesChanged;
            }
            else
            {
                group = server.OpcDaGroupS[strMd5];
            }

            return groupId;
        }

        public void StopMonitoringItems(string serviceProgId, string groupId)
        {
            var service = serviceCollection.Where(a => a.ServiceIds.Contains(serviceProgId))
                               .FirstOrDefault();
            OpcDaServer daService = null;
            if (CheckServiceExisted(service, serviceProgId))
            {
                daService = opcDaServices.Find(item => { return item.Host == service.Host && item.ServiceId == serviceProgId; })
                                      .Service;
                OpcDaGroup group = daGroupKeyPairs[groupId];
                group.ValuesChanged -= MonitorValuesChanged;
                daService.RemoveGroup(group);
                daGroupKeyPairs.Remove(groupId);
            }
        }

        public void SetItemsValueChangedCallBack(IItemsValueChangedCallBack callBack)
        {
            this.callBack = callBack;
        }

        public List<Item> ReadItemsValues(string serverID, string groupId,  List<string> items,  string strMd5)
        {
            OpcDaService server = GetOpcDaService(serverID);
            if (server == null)
            {
                return null;
            }

            if (server.OpcDaGroupS.ContainsKey(strMd5) == true)
            {
                OpcDaGroup group = server.OpcDaGroupS[strMd5];
                OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Cache);

                if (values.Length != group.Items.Count)
                {
                    LogHelper.Log($"values.Length(${values.Length}) != group.Items.Count(${group.Items.Count}) ");
                    return null;
                }

                List<Item> itemValues = new List<Item>();
                for (int i = 0; i < values.Length; ++i)
                {
                    Item it = new Item();
                    it.ItemId = group.Items[i].ItemId;
                    it.Data = values[i].Value;
                    it.Type = values[i].Value.GetType().ToString();
                    LogHelper.Log($"values[{i}].Value.GetType() ${values[i].Value.GetType()} ");

                    itemValues.Add(it);
                }

                return itemValues;
            }

            return null;
        }

        public void WriteValues(string serverID, string groupId, Dictionary<string, object> itemValuePairs)
        {
            OpcDaService server = GetOpcDaService(serverID);
            if (server == null)
            {
                return;
            }

            if (server.OpcDaGroupS.ContainsKey(groupId) == true)
            {
                OpcDaGroup group = server.OpcDaGroupS[groupId];
                var keyList = itemValuePairs.Keys.ToList();
                List<OpcDaItem> itemList = new List<OpcDaItem>();
                keyList.ForEach(ids =>
                {
                    var daItem = group.Items
                                      .Where(a => a.ItemId == ids)
                                      .FirstOrDefault();
                    itemList.Add(daItem);
                });

                object[] dd = itemValuePairs.Values.ToArray();
                HRESULT[] res = group.Write(itemList, dd);

                LogHelper.Log("Write HRESULT " + res.ToString());
            }
        }

        private void MonitorValuesChanged(object sender, OpcDaItemValuesChangedEventArgs e)
        {
            if (callBack != null)
            {
               var opcGroup = sender as OpcDaGroup;
               callBack.ValueChangedCallBack(opcGroup.Name, e.Values);
            }
        }
    }
}
