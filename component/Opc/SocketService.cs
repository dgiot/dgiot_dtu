// <copyright file="SocketService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;
    using IniParser;
    using IniParser.Model;
    using Newtonsoft.Json;
    using SuperSocket.SocketBase;
    using SuperSocket.SocketBase.Protocol;
    using TitaniumAS.Opc.Client.Da;

    // 启动定时广播自己地址 实现网络发现功能
    // 开启TCP服务
    // 请求本机还是网络
    // 请求ProgId对象节点
    // 请求监听节点值
    // 节点定时刷新 与此同时 节点本身可能会处于监听消息状态 因此考虑隔离
    public class SocketService : IItemsValueChangedCallBack
    {
        private AppServer server = new AppServer(new ReceiveFilterFactory());
        private IOPCDa iOpcDa = new OPCDaImp();
        private string iniFilePath = string.Empty;
        private System.Timers.Timer scanTimer = new System.Timers.Timer();
        private List<TreeNode> treeNodeCaches = new List<TreeNode>();
        private long exchanging = 0;
        private Dictionary<string, AppSession> sessionDic = new Dictionary<string, AppSession>();

        // private Dictionary<string, AppSession> _sessionDic = new Dictionary<string, AppSession>();
        private Dictionary<string, string> whiteList = null;

        public SocketService()
        {
            iOpcDa.SetItemsValueChangedCallBack(this);
        }

        private bool CheckFileExist()
        {
            iniFilePath = Path.Combine(System.Environment.CurrentDirectory, "cfg.ini");
            if (!File.Exists(iniFilePath))
            {
                return false;
            }

            return true;
        }

        private bool CheckINISectionExist()
        {
            FileIniDataParser fileIniDataParser = new FileIniDataParser();
            IniData data = fileIniDataParser.ReadFile(iniFilePath);
            SectionDataCollection dataCollection = data.Sections;
            if (!dataCollection.ContainsSection("scan"))
            {
                return false;
            }

            SectionData sectionData = dataCollection.GetSectionData("scan");
            if (sectionData == null)
            {
            }
            else
            {
                if (sectionData.Keys.ContainsKey("whitelist") == true)
                {
                    string whitelist = sectionData.Keys["whitelist"];

                    string[] recvArr = whitelist.Split('|');
                    whiteList = recvArr.ToDictionary(t => t);
                }
            }

            return true;
        }

        private Dictionary<string, object> CheckSectionParam()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            FileIniDataParser fileIniDataParser = new FileIniDataParser();
            IniData data = fileIniDataParser.ReadFile(iniFilePath);
            SectionDataCollection dataCollection = data.Sections;
            SectionData sectionData = dataCollection.GetSectionData("scan");
            if (sectionData != null)
            {
                KeyDataCollection keys = sectionData.Keys;
                foreach (var keyItem in keys)
                {
                    if (keyItem.KeyName.Equals("networkSegments"))
                    {
                        string networkSegments = keyItem.Value;
                        List<string> ipAddrList = networkSegments.Split('|').ToList();
                        List<string> invalidList = new List<string>();
                        for (int index = 0; index < ipAddrList.Count(); index++)
                        {
                            IPAddress point;
                            if (!System.Net.IPAddress.TryParse(ipAddrList[index], out point))
                            {
                                invalidList.Add(ipAddrList[index]);
                            }
                        }

                        keyValuePairs.Add("ipAddrList", ipAddrList.Except(invalidList).ToList());
                    }
                    else if (keyItem.KeyName.Equals("interval"))
                    {
                        int interval;
                        if (!int.TryParse(keyItem.Value, out interval))
                        {
                        }
                        else
                        {
                            keyValuePairs.Add("interval", interval);
                        }
                    }
                }
            }

            return keyValuePairs;
        }

        private List<string> CheckValidateIPAddress(List<string> addressList)
        {
            List<string> unusedDataList = new List<string>();
            foreach (var item in addressList)
            {
                using (Ping p = new Ping())
                {
                    PingReply pingReply = p.Send(item, 100);
                    if (pingReply.Status != IPStatus.Success)
                    {
                        unusedDataList.Add(item);
                    }
                }
            }

            return addressList.Except(unusedDataList).ToList();
        }

        public List<TreeNode> ScanOPCClassicServer(List<string> addresses)
        {
            List<TreeNode> opcDaServerList = new List<TreeNode>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string localIp = string.Empty;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                    break;
                }
            }

            foreach (var usefulItem in addresses)
            {
                string[] opcDaList = iOpcDa.ScanOPCDa(usefulItem);
                if (opcDaList.Length > 0)
                {
                    TreeNode node = new TreeNode();
                    node.Name = usefulItem.ToString();
                    if (usefulItem.ToString() != localIp)
                    {
                        node.NodeType = TreeNodeType.Remote;
                    }

                    List<TreeNode> childNodes = new List<TreeNode>();
                    foreach (var opcItem in opcDaList)
                    {
                        if (whiteList != null && whiteList.Count != 0)
                        {
                            if (whiteList.ContainsKey(opcItem) == true)
                            {
                                // 白名单里有该条目，添加
                                childNodes.Add(new TreeNode() { Name = opcItem });
                            }
                            else
                            {
                                // 白名单里没有该条目，不添加
                            }
                        }
                        else
                        {
                            // 没有白名单，全添加
                            childNodes.Add(new TreeNode() { Name = opcItem });
                        }

                        // if ("NETxKNX.OPC.Server.3.5" == opcItem)
                        // {
                        //    _debugDataCallBack.DoEventLogCallBack(debugInfo(string.Format("扫描成功 地址：{0} OPCClassic {1} 添加", usefulItem.ToString(), opcItem)));
                        //    childNodes.Add(new TreeNode() { Name = opcItem });
                        // }
                        // else
                        // {
                        //    _debugDataCallBack.DoEventLogCallBack(debugInfo(string.Format("扫描成功 地址：{0} OPCClassic {1} 不添加", usefulItem.ToString(), opcItem)));
                        // }
                    }

                    node.Children.AddRange(childNodes);
                    opcDaServerList.Add(node);
                }
                else
                {
                }
            }

            return opcDaServerList;
        }

        public List<TreeNode> ScanOPCServerData(List<TreeNode> opcServerNodes)
        {
            opcServerNodes.ForEach((service) =>
            {
                service.Children.ForEach((opc) =>
                {
                    IList<TreeNode> dataNodes = iOpcDa.GetTreeNodes(opc.Name);
                    opc.Children.AddRange(dataNodes);
                });
            });

            return opcServerNodes;
        }

        private int CompareIPs(byte[] ip1, byte[] ip2)
        {
            if (ip1 == null || ip1.Length != 4)
            {
                return -1;
            }

            if (ip2 == null || ip2.Length != 4)
            {
                return 1;
            }

            int comp = ip1[0].CompareTo(ip2[0]);
            if (comp == 0)
            {
                comp = ip1[1].CompareTo(ip2[1]);
            }

            if (comp == 0)
            {
                comp = ip1[2].CompareTo(ip2[2]);
            }

            if (comp == 0)
            {
                comp = ip1[3].CompareTo(ip2[3]);
            }

            return comp;
        }

        private void IncrementIP(byte[] ip, int idx = 3)
        {
            if (ip == null || ip.Length != 4 || idx < 0)
            {
                return;
            }

            if (ip[idx] == 254)
            {
                ip[idx] = 1;
                IncrementIP(ip, idx - 1);
            }
            else
            {
                ip[idx] = (byte)(ip[idx] + 1);
            }
        }

        private string DebugInfo(string info)
        {
            return string.Format("[{0}]:{1}", System.DateTime.Now.ToLocalTime(), info);
        }

        public void Start()
        {
            server.NewRequestReceived += NewRequestReceived;
            server.NewSessionConnected += NewSessionConnected;
            server.Setup(10010);
            server.Start();
        }

        private void NewSessionConnected(AppSession session)
        {
        }

        private void NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            ProcessRequest(session, requestInfo);
        }

        private void ProcessRequest(AppSession session, StringRequestInfo requestInfo)
        {
            int cmd = int.Parse(requestInfo.Parameters[0]);
            if (Interlocked.CompareExchange(ref exchanging, 1, 0) == 0)
            {
                try
                {
                    switch (cmd)
                    {
                        case (int)Command.Get_Nodes_Req:
                            {
                                string json = JsonConvert.SerializeObject(treeNodeCaches);
                                byte[] bufferList = StructUtility.Package(new Header()
                                {
                                    Id = int.Parse(requestInfo.Key) + 1,
                                    Cmd = (int)Command.Get_Nodes_Rsp,
                                    ErrorCode = 0,
                                    ContentSize = json.Length
                                }, json);
                                session.Send(bufferList, 0, bufferList.Length);
                            }

                            break;
                        case (int)Command.Start_Monitor_Nodes_Req:
                            {
                                StartMonitoringItemsReq req = JsonConvert.DeserializeObject<StartMonitoringItemsReq>(requestInfo.Body);

                                string groupId = iOpcDa.StartMonitoringItems(req.ServiceId, req.Items, req.StrMd5);

                                if (groupId == null)
                                {
                                    StartMonitoringItemsRsp rsp = new StartMonitoringItemsRsp() { ServiceId = req.ServiceId, GroupId = groupId };
                                    string json = JsonConvert.SerializeObject(rsp);
                                    byte[] bufferList = StructUtility.Package(new Header()
                                    {
                                        Id = int.Parse(requestInfo.Key) + 1,
                                        Cmd = (int)Command.Start_Monitor_Nodes_Rsp,
                                        ErrorCode = -1,
                                        ContentSize = json.Length
                                    }, json);
                                    session.Send(bufferList, 0, bufferList.Length);
                                }
                                else
                                {
                                    StartMonitoringItemsRsp rsp = new StartMonitoringItemsRsp() { ServiceId = req.ServiceId, GroupId = groupId };
                                    string json = JsonConvert.SerializeObject(rsp);
                                    byte[] bufferList = StructUtility.Package(new Header()
                                    {
                                        Id = int.Parse(requestInfo.Key) + 1,
                                        Cmd = (int)Command.Start_Monitor_Nodes_Rsp,
                                        ErrorCode = 0,
                                        ContentSize = json.Length
                                    }, json);
                                    session.Send(bufferList, 0, bufferList.Length);
                                }

                                // _sessionDic[groupId] = session;
                            }

                            break;
                        case (int)Command.Stop_Monitor_Nodes_Req:
                            {
                                StopMonitoringItemsReq req = JsonConvert.DeserializeObject<StopMonitoringItemsReq>(requestInfo.Body);
                                iOpcDa.StopMonitoringItems(req.ServiceId, req.Id);

                                byte[] bufferList = StructUtility.Package(new Header()
                                {
                                    Id = int.Parse(requestInfo.Key) + 1,
                                    Cmd = (int)Command.Stop_Monitor_Nodes_Rsp,
                                    ErrorCode = 0,
                                    ContentSize = 0
                                }, string.Empty);
                                session.Send(bufferList, 0, bufferList.Length);

                                // if (_sessionDic.ContainsKey(req.Id))
                                // {
                                //    _sessionDic.Remove(req.Id);
                                // }
                            }

                            break;
                        case (int)Command.Read_Nodes_Values_Req:
                            {
                                ReadItemsReq req = JsonConvert.DeserializeObject<ReadItemsReq>(requestInfo.Body);

                                List<Item> values = iOpcDa.ReadItemsValues(req.ServiceId, req.Items, req.GroupId, req.StrMd5);
                                if (values != null)
                                {
                                    ReadItemsRsp rsp = new ReadItemsRsp() { ServiceId = req.ServiceId, GroupId = req.GroupId, ItemValues = values, StrMd5 = req.StrMd5 };
                                    string json = JsonConvert.SerializeObject(rsp);
                                    byte[] bufferList = StructUtility.Package(new Header()
                                    {
                                        Id = int.Parse(requestInfo.Key) + 1,
                                        Cmd = (int)Command.Read_Nodes_Values_Rsp,
                                        ErrorCode = 0,
                                        ContentSize = json.Length
                                    }, json);
                                    session.Send(bufferList, 0, bufferList.Length);
                                }
                                else
                                {
                                    ReadItemsRsp rsp = new ReadItemsRsp() { ServiceId = req.ServiceId, GroupId = req.GroupId, ItemValues = values, StrMd5 = req.StrMd5 };
                                    string json = JsonConvert.SerializeObject(rsp);
                                    byte[] bufferList = StructUtility.Package(new Header()
                                    {
                                        Id = int.Parse(requestInfo.Key) + 1,
                                        Cmd = (int)Command.Read_Nodes_Values_Rsp,
                                        ErrorCode = -1,
                                        ContentSize = json.Length
                                    }, json);
                                    session.Send(bufferList, 0, bufferList.Length);
                                }
                            }

                            break;
                        case (int)Command.Write_Nodes_Values_Req:
                            {
                                WriteNodesValuesReq req = JsonConvert.DeserializeObject<WriteNodesValuesReq>(requestInfo.Body);
                                iOpcDa.WriteValues(req.ServiceId, req.StrMd5, req.ItemValuePairs);
                                byte[] bufferList = StructUtility.Package(new Header()
                                {
                                    Id = int.Parse(requestInfo.Key) + 1,
                                    Cmd = (int)Command.Write_Nodes_Values_Rsp,
                                    ErrorCode = 0,
                                    ContentSize = 0
                                }, string.Empty);

                                session.Send(bufferList, 0, bufferList.Length);
                            }

                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{session.RemoteEndPoint.Address.ToString()}-------------" + e);
                    session.Close();
                }

                Interlocked.Decrement(ref exchanging);
            }
            else
            {
                byte[] bufferList = StructUtility.Package(new Header()
                {
                    Id = int.Parse(requestInfo.Key) + 1,
                    Cmd = cmd + 1,
                    ErrorCode = -99,
                    ContentSize = 0
                }, string.Empty);
                session.Send(bufferList, 0, bufferList.Length);
            }
        }

        public void Stop()
        {
            server.NewRequestReceived -= NewRequestReceived;
            server.NewSessionConnected -= NewSessionConnected;
            server.Stop();
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
                collection.Add(i);
            });

            entity.Items = collection;
            string json = JsonConvert.SerializeObject(entity);
            byte[] bufferList = StructUtility.Package(new Header() { Id = 0, Cmd = (int)Command.Notify_Nodes_Values_Ex, ContentSize = json.Length }, json);

            // _sessionDic[group].Send(bufferList, 0, bufferList.Length);
        }
    }
}
