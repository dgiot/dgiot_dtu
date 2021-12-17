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

    // 不在一开始读取节点结构时创建缓存的原因：用户不一定会全部都读取所有节点属性值 仅仅在需要关注的时候 才读取
    public class OPCDaImp : IOPCDa
    {
        private OpcServerEnumeratorAuto serverEnumerator = new OpcServerEnumeratorAuto();
        private Dictionary<string, OpcDaGroup> daGroupKeyPairs = new Dictionary<string, OpcDaGroup>();
        private List<HostCollection> hostCollection = new List<HostCollection>();
        private Dictionary<string, GroupEntity> groupCollection = new Dictionary<string, GroupEntity>();
        private IItemsValueChangedCallBack callBack;
        private List<OpcDaService> opcDaServices = new List<OpcDaService>();
        private List<string> groupKeys = new List<string>();

        public List<string> ScanOPCDa(string host, bool isClean = false)
        {
            if (isClean)
            {
                hostCollection.Clear();
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
                    if (hostCollection.Any(a => { return a.Host == host; }))
                    {
                        HostCollection item = hostCollection.Where(a => { return a.Host == host; })
                                                                   .FirstOrDefault();
                        var exceptList = item.ServiceIds.Except(serviceList);
                        if (exceptList.Any())
                        {
                            item.ServiceIds.AddRange(exceptList);
                        }
                    }
                    else
                    {
                        hostCollection.Add(new HostCollection() { Host = host, ServiceIds = serviceList.ToList() });
                    }

                    TreeNode opcdaNode = OPCDAViewHelper.GetRootNode();
                    TreeNode hostNode = OPCDAViewHelper.AddNode(opcdaNode, host, host);
                    foreach (string progId in serviceList)
                    {
                        OPCDAViewHelper.AddNode(hostNode, progId, progId);
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

        public OpcDaService GetOpcDaService(string host, string serviceProgId)
        {
           var service = hostCollection.Where(a => a.ServiceIds.Contains(serviceProgId) && a.Host == host)
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

        private bool CheckServiceExisted(HostCollection service, string serviceProgId)
        {
          return opcDaServices.Any(item => { return item.Host == service.Host && item.ServiceId == serviceProgId; });
        }

        public void StartGroup(TreeNode currNode, int interval)
        {
            if (currNode.Nodes == null || !currNode.Checked)
            {
                return; // 没有子节点或者没被选中
            }

            if (currNode.ForeColor == System.Drawing.Color.Blue)
            {
                TreeNode serviceNode = GetServerNode(currNode);
                if (null != serviceNode)
                {
                    LogHelper.Log("Level " + serviceNode.Level.ToString());
                    TreeNode hostNode = serviceNode.Parent;
                    StartMonitoringItems(hostNode.Text, serviceNode.Text, currNode, interval);
                }
            }

            foreach (TreeNode tmpNode in currNode.Nodes)
            {
                StartGroup(tmpNode, interval);
            }
        }

        public TreeNode GetServerNode(TreeNode node)
        {
            if (node == null)
            {
                return null;
            }

            if (node.Level == 2)
            {
                return node;
            }

            if (node.Parent != null)
            {
                if (node.Parent.Level == 2)
                {
                    return node.Parent;
                }

                if (node.Parent.Parent != null)
                {
                    if (node.Parent.Parent.Level == 2)
                    {
                        return node.Parent.Parent;
                    }

                    if (node.Parent.Parent.Parent != null)
                    {
                        if (node.Parent.Parent.Parent.Level == 2)
                        {
                            return node.Parent.Parent.Parent;
                        }

                        if (node.Parent.Parent.Parent.Parent != null)
                        {
                            if (node.Parent.Parent.Parent.Parent.Level == 2)
                            {
                                return node.Parent.Parent.Parent.Parent;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public void StopGroup()
        {
            groupKeys.ForEach(groupKey =>
            {
                StopMonitoringItems(groupKey);
            });
            groupKeys.Clear();
        }

        public string StartMonitoringItems(string host, string serviceProgId, TreeNode groupNode, int interval)
        {
            OpcDaService server = GetOpcDaService(host, serviceProgId);
            string groupKey = OPCDAViewHelper.Key(groupNode.Text);
            if (server == null)
            {
                LogHelper.Log("StartMonitoringItems  is null");
                return null;
            }

            if (server.OpcDaGroupS.Count == 0)
            {
                LogHelper.Log("StartMonitoringItems  is host opcda://" + host + "/" + serviceProgId);
                AddGroup(server, groupKey, groupNode, interval);
            }
            else
            {
                if (server.OpcDaGroupS.ContainsKey(groupKey))
                {
                    OpcDaGroup group = server.OpcDaGroupS[groupKey];
                }
                else
                {
                    AddGroup(server, groupKey, groupNode, interval);
                }
            }

            return groupKey;
        }

        public void AddGroup(OpcDaService server, string groupKey, TreeNode groupNode, int interval)
        {
            if (server.Service == null)
            {
                return;
            }

            OpcDaGroup group = server.Service.AddGroup(groupKey);  // maybe cost lot of time
            Thread.Sleep(100);
            group.IsActive = true;
            group.UserData = groupNode;
            server.OpcDaGroupS.Add(groupKey, group);

            List<OpcDaItemDefinition> itemDefList = new List<OpcDaItemDefinition>();
            foreach (TreeNode tmpNode in groupNode.Nodes)
            {
                if (tmpNode.Checked)
                {
                    var def = new OpcDaItemDefinition
                    {
                        ItemId = tmpNode.Text,
                        UserData = tmpNode,
                        IsActive = true
                    };
                    itemDefList.Add(def);

                    // LogHelper.Log("StartMonitor ItemId " + tmpNode.Text);
                }
            }

            OpcDaItemResult[] opcDaItemResults = group.AddItems(itemDefList);
            daGroupKeyPairs.Add(groupKey, group);
            groupKeys.Add(groupKey);
            LogHelper.Log("StartMonitoring  is groupId " + groupKey + " interval " + interval.ToString() + " ms", (int)LogHelper.Level.INFO);

            group.UpdateRate = TimeSpan.FromMilliseconds(interval); // 1000毫秒触发一次
            group.ValuesChanged += MonitorValuesChanged;
            GroupEntity groupEntity = new GroupEntity()
            {
                Host = server.Host,
                ProgId = server.ServiceId
            };
            groupCollection.Add(groupKey, groupEntity);
            JsonObject result = new JsonObject();
            result.Add("timestamp", DgiotHelper.Now());
            result.Add("deviceName", groupNode.Parent.Text);
            result.Add("deviceAddr", OPCDAViewHelper.Key(groupNode.Parent.FullPath));
            JsonArray properties = ScanItemsValues(server.Host, server.ServiceId, groupKey, groupNode);
            result.Add("properties", properties);
            LogHelper.Log("StartMonitoring  result " + result, (int)LogHelper.Level.INFO);
        }

        public void StopMonitoringItems(string groupKey)
        {
            Thread.Sleep(100);
            string host = groupCollection[groupKey].Host;
            string serviceProgId = groupCollection[groupKey].ProgId;
            OpcDaService server = GetOpcDaService(host, serviceProgId);
            if (server == null)
            {
                LogHelper.Log("StopMonitoringItems  is null");
                return;
            }

            OpcDaGroup group = daGroupKeyPairs[groupKey];
            group.ValuesChanged -= MonitorValuesChanged;
            server.OpcDaGroupS.Remove(groupKey);
            server.Service.RemoveGroup(group);
            daGroupKeyPairs.Remove(groupKey);
            groupCollection.Remove(groupKey);
        }

        public void SetItemsValueChangedCallBack(IItemsValueChangedCallBack callBack)
        {
            this.callBack = callBack;
        }

        public JsonArray ScanItemsValues(string host, string serverID, string groupKey, TreeNode groupNode)
        {
            JsonArray properties = new JsonArray();

            OpcDaService server = GetOpcDaService(host, serverID);
            if (server == null)
            {
                return properties;
            }

            if (server.OpcDaGroupS.ContainsKey(groupKey) == true)
            {
                OpcDaGroup group = server.OpcDaGroupS[groupKey];
                try
                {
                    OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);

                    if (values.Length != group.Items.Count)
                    {
                        LogHelper.Log($"values.Length(${values.Length}) != group.Items.Count(${group.Items.Count}) ");
                        return properties;
                    }

                    for (int i = 0; i < values.Length; ++i)
                    {
                        if (values[i].Value != null)
                        {
                            JsonObject json = new JsonObject();
                            TreeNode node = values[i].Item.UserData as TreeNode;
                            json.Add("name", node.Text);
                            json.Add("devicetype", groupNode.ToolTipText);
                            json.Add("identifier", node.ToolTipText);
                            JsonObject dataForm = new JsonObject();
                            dataForm.Add("slaveid", groupKey);
                            dataForm.Add("protocol", node.Tag);
                            dataForm.Add("address", values[i].Item.ItemId);
                            dataForm.Add("data", values[i].Value);
                            json.Add("dataForm", dataForm);
                            JsonObject dataType = new JsonObject();
                            dataType.Add("type", values[i].Value.GetType().ToString());
                            json.Add("dataType", dataType);
                            properties.Add(json);
                        }
                    }
                }
                 catch (Exception)
                {
                    return properties;
                }

                return properties;
            }

            return properties;
        }

        public List<Item> ReadItemsValues(string host, string serverID, string groupKey)
        {
            OpcDaService server = GetOpcDaService(host, serverID);
            if (server == null)
            {
                return null;
            }

            if (server.OpcDaGroupS.ContainsKey(groupKey) == true)
            {
                OpcDaGroup group = server.OpcDaGroupS[groupKey];
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
                    if (values[i].Value != null)
                    {
                        it.Data = values[i].Value;
                        it.Type = values[i].Value.GetType().ToString();
                        itemValues.Add(it);
                    }
                }

                return itemValues;
            }

            return null;
        }

        public void StartMonitor(string groupKey, List<string> items, string serverID, string host = "127.0.0.1")
        {
            JsonArray properties = new JsonArray();

            OpcDaService server = GetOpcDaService(host, serverID);
            if (server == null)
            {
                return;
            }

            if (server.OpcDaGroupS.ContainsKey(groupKey) == false)
            {
                OpcDaGroup group = server.Service.AddGroup(groupKey);  // maybe cost lot of time
                Thread.Sleep(100);
                group.IsActive = true;
                server.OpcDaGroupS.Add(groupKey, group);
                List<OpcDaItemDefinition> itemDefList = new List<OpcDaItemDefinition>();
                foreach (string item in items)
                {
                    var def = new OpcDaItemDefinition
                    {
                        ItemId = item,
                        IsActive = true
                    };
                    itemDefList.Add(def);
                }

                group.AddItems(itemDefList);
                daGroupKeyPairs.Add(groupKey, group);
                groupKeys.Add(groupKey);
                LogHelper.Log("StartMonitoring  is groupId " + groupKey + " interval " + "1000  ms", (int)LogHelper.Level.INFO);

                group.UpdateRate = TimeSpan.FromMilliseconds(1000); // 1000毫秒触发一次
                group.ValuesChanged += MonitorValuesChanged;
                GroupEntity groupEntity = new GroupEntity()
                {
                    Host = server.Host,
                    ProgId = server.ServiceId
                };
                groupCollection.Add(groupKey, groupEntity);
            }
        }

        public void WriteValues(string host, string serviceProgId, string groupKey, Dictionary<string, object> itemValuePairs)
        {
            OpcDaService server = GetOpcDaService(host, serviceProgId);
            if (server == null)
            {
                return;
            }

            if (server.OpcDaGroupS.ContainsKey(groupKey) == true)
            {
                OpcDaGroup group = server.OpcDaGroupS[groupKey];
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
                callBack.ValueChangedCallBack(opcGroup, e.Values);
            }
        }
    }
}
