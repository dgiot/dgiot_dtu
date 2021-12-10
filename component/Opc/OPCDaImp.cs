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

        public void StopGroup()
        {
            groupKeys.ForEach(groupKey =>
            {
                StopMonitoringItems(groupKey);
            });
            groupKeys.Clear();
        }

        public void StartGroup(TreeNode currNode)
        {
            if (currNode.Nodes == null && currNode.Checked)
            {
                return; // 没有子节点返回
            }

            if (currNode.ForeColor == System.Drawing.Color.Blue)
            {
                List<string> items = new List<string>();
                if (currNode.Checked && currNode.Nodes != null)
                {
                    foreach (TreeNode tmpNode in currNode.Nodes)
                    {
                        items.Add(tmpNode.Text);
                    }

                    TreeNode deviceNode = currNode.Parent;
                    if (currNode.Level == 5)
                    {
                        deviceNode = deviceNode.Parent;
                    }

                    TreeNode serviceNode = deviceNode.Parent;
                    TreeNode hostNode = serviceNode.Parent;
                    StartMonitoringItems(hostNode.Text, serviceNode.Text, currNode);
                }
            }
            else
            {
                foreach (TreeNode tmpNode in currNode.Nodes)
                {
                    StartGroup(tmpNode);
                }
            }
        }

        public string StartMonitoringItems(string host, string serviceProgId, TreeNode groupNode)
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
                AddGroup(server, groupKey, groupNode);
            }
            else
            {
                if (server.OpcDaGroupS.ContainsKey(groupKey))
                {
                    OpcDaGroup group = server.OpcDaGroupS[groupKey];
                }
                else
                {
                    AddGroup(server, groupKey, groupNode);
                }
            }

            return groupKey;
        }

        public void AddGroup(OpcDaService server, string groupKey, TreeNode groupNode)
        {
            if (server.Service == null)
            {
                return;
            }

            OpcDaGroup group = server.Service.AddGroup(groupKey);  // maybe cost lot of time
            Thread.Sleep(100);
            group.IsActive = true;
            server.OpcDaGroupS.Add(groupKey, group);

            List<OpcDaItemDefinition> itemDefList = new List<OpcDaItemDefinition>();
            foreach (TreeNode tmpNode in groupNode.Nodes)
            {
                if (tmpNode.Checked)
                {
                    var def = new OpcDaItemDefinition
                    {
                        ItemId = tmpNode.Text,
                        IsActive = true
                    };
                    itemDefList.Add(def);
                }
            }

            OpcDaItemResult[] opcDaItemResults = group.AddItems(itemDefList);
            daGroupKeyPairs.Add(groupKey, group);
            groupKeys.Add(groupKey);
            LogHelper.Log("StartMonitoringItems  is groupId " + groupKey);
            group.UpdateRate = TimeSpan.FromMilliseconds(1000); // 1000毫秒触发一次
            group.ValuesChanged += MonitorValuesChanged;
            GroupEntity groupEntity = new GroupEntity()
            {
                Host = server.Host,
                ProgId = server.ServiceId
            };
            groupCollection.Add(groupKey, groupEntity);
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

        public void WriteValues(string host, string serverID, string groupKey, Dictionary<string, object> itemValuePairs)
        {
            OpcDaService server = GetOpcDaService(host, serverID);
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
               callBack.ValueChangedCallBack(opcGroup.Name, e.Values);
            }
        }
    }
}
