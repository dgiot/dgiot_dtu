// <copyright file="OPCDaImp.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
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

        public List<string> ScanOPCDa(string host, bool isClean = false)
        {
            if (isClean)
            {
                serviceCollection.Clear();
            }

            List<string> retValue = new List<string>();
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

                    OPCDAViewHelper.AddHost(host);
                    foreach (string progId in serviceList)
                    {
                        OPCDAViewHelper.AddService(host, progId);
                    }
                }

                return opcServers.Select(a => a.ProgId).ToList();
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

        private static readonly List<string> DeviceFilter = new List<string>
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

        private static bool IsDevicefilter(string group)
        {
            bool result = false;
            foreach (string filter in DeviceFilter)
            {
                if (-1 != group.LastIndexOf(filter))
                {
                    return true;
                }
            }

            return result;
        }

        private static readonly List<string> Groupfilter = new List<string>
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
            foreach (string filter in Groupfilter)
            {
                if (-1 != group.LastIndexOf(filter))
                {
                    return true;
                }
            }

            return result;
        }

        private static readonly List<string> Itemfilter = new List<string>
        {
            "._Statistics",
            "._System",
            ".SystemVariable"
        };

        private static bool IsItemsfilter(string item)
        {
            bool result = false;
            foreach (string filter in Itemfilter)
            {
                if (-1 != item.LastIndexOf(filter))
                {
                    return true;
                }
            }

            return result;
        }

        public List<string> GetDevices(string host, string serviceProgId)
        {
            Uri url = UrlBuilder.Build(serviceProgId);
            List<string> groups = new List<string>();
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    server.Connect();
                    var browser = new OpcDaBrowserAuto(server);
                    OpcDaBrowseElement[] deviceElements = browser.GetElements(null);
                    foreach (OpcDaBrowseElement deviceElement in deviceElements)
                    {
                        if (!(deviceElement.Name.IndexOf('$') == 0) && !IsDevicefilter(deviceElement.Name))
                        {
                             return GetGroups(host, serviceProgId, deviceElement.Name, browser);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return groups;
        }

    public List<string> GetGroups(string host, string serviceProgId, string device, IOpcDaBrowser browser)
    {
        List<string> groups = new List<string>();
        try
        {
            OpcDaBrowseElement[] groupElements = browser.GetElements(device);
            foreach (OpcDaBrowseElement groupElement in groupElements)
            {
                    if (!(groupElement.Name.IndexOf('$') == 0) && !IsGroupfilter(groupElement.Name))
                    {
                        List<Item> items = new List<Item>();
                        LogHelper.Log("groupElement  " + groupElement.Name);
                        TreeNode node = OPCDAViewHelper.AddGroup(host, serviceProgId, device, groupElement.Name);
                        string deviceAddr = GetDevAddr(serviceProgId, groupElement.Name);
                        GroupEntity group = new GroupEntity
                        {
                            Path = node.FullPath,
                            Name = groupElement.Name
                        };
                        OpcDaBrowseElement[] itemElements = browser.GetElements(groupElement.Name);
                        foreach (OpcDaBrowseElement itemElement in itemElements)
                        {
                            if (!IsItemsfilter(itemElement.ItemId))
                            {
                                LogHelper.Log("itemElement  " + itemElement.ItemId);
                                OPCDAViewHelper.AddItem(host, serviceProgId, device, groupElement.Name, itemElement.ItemId);
                                Item item = new Item
                                {
                                    ItemId = itemElement.ItemId
                                };
                                items.Add(item);
                            }
                        }

                        group.Items = items;
                        groups.Add(groupElement.Name);
                        groupCollection.Add(deviceAddr, group);
                        StartMonitoringItems(serviceProgId, group.Items.Select(a => a.ItemId).ToList(), groupElement.Name);
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

        public TreeNode GetTreeNodes(string serviceProgId)
        {
            var server = GetOpcDaService(serviceProgId);
            TreeNode node = OPCDAViewHelper.AddService(server.Host, serviceProgId);
            try
            {
                OpcDaBrowserAuto browserAuto = new OpcDaBrowserAuto(server.Service);
                BrowseChildren(browserAuto, node);
            }
            catch (Exception)
            {
                return node;
            }

            return node;
        }

        public void BrowseChildren(IOpcDaBrowser browser, TreeNode node,  string itemId = null, int indent = 0)
        {
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            foreach (OpcDaBrowseElement element in elements)
            {
                if (!(element.ItemId.IndexOf('$') == 0))
                {
                    TreeNode curNode = node.Nodes.Add(node.FullPath + "/" + element.Name, element.ItemId);
                    if (element.HasChildren)
                    {
                        BrowseChildren(browser, curNode, element.ItemId, indent + 2);
                    }
                }
            }
        }

        private bool CheckServiceExisted(ServiceCollection service, string serviceProgId)
        {
          return opcDaServices.Any(item => { return item.Host == service.Host && item.ServiceId == serviceProgId; });
        }

        public string StartMonitoringItems(string serviceProgId, List<string> items, string groupId)
        {
            OpcDaService server = GetOpcDaService(serviceProgId);
            if (server == null)
            {
                LogHelper.Log("StartMonitoringItems  is null");
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

                items.ForEach(itemId =>
                {
                    var def = new OpcDaItemDefinition();
                    def.ItemId = itemId;
                    def.IsActive = true;

                    // def.RequestedDataType = ;
                    itemDefList.Add(def);
                    LogHelper.Log("StartMonitoringItems  is itemId " + itemId);
                });
                OpcDaItemResult[] opcDaItemResults = group.AddItems(itemDefList);
                daGroupKeyPairs.Add(groupId, group);
                group.UpdateRate = TimeSpan.FromMilliseconds(1000); // 1000毫秒触发一次
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
