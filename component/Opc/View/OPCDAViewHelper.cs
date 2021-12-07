// <copyright file="OPCDAViewHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System.Windows.Forms;

    public class OPCDAViewHelper
    {
        private OPCDAViewHelper()
        {
        }

        private static OPCDAViewHelper instance = null;
        private static TreeNode treeNode = new TreeNode(Type(), 0, 0);

        public static OPCDAViewHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new OPCDAViewHelper();
                treeNode.Tag = Type();
            }

            return instance;
        }

        public static string Type()
        {
            return TreeViewHelper.Type(TreeViewHelper.NodeType.OPCDA);
        }

        public static void View()
        {
            TreeViewHelper.AddNode(treeNode);
            OPCDAHelper.View();
        }

        public static string HostKey(string key)
        {
            return DgiotHelper.Md5("opcda_host_" + key);
        }

        public static string ServiceKey(string key)
        {
            return DgiotHelper.Md5("opcda_service_" + key);
        }

        public static string DeviceKey(string key)
        {
            return DgiotHelper.Md5("opcda_device_" + key);
        }

        public static string GroupKey(string key)
        {
            return DgiotHelper.Md5("opcda_group_" + key);
        }

        public static string ItemKey(string key)
        {
            return DgiotHelper.Md5("opcda_item_" + key);
        }

        public static bool CheckHost(string host)
        {
            return treeNode.Nodes.ContainsKey(HostKey(host));
        }

        public static TreeNode AddHost(string host)
        {
            if (!CheckHost(host))
            {
                treeNode.Nodes.Add(HostKey(host), host, "opcda");
                treeNode.Nodes[HostKey(host)].Tag = Type();
            }

            return treeNode.Nodes[HostKey(host)];
        }

        public static bool CheckService(string host, string service)
        {
            bool result = false;
            if (CheckHost(host))
            {
                return treeNode.Nodes[HostKey(host)].Nodes.ContainsKey(ServiceKey(service));
            }

            return result;
        }

        public static TreeNode AddService(string host, string service)
        {
            TreeNode hostNode = AddHost(host);
            if (!CheckService(host, service))
            {
                hostNode.Nodes.Add(ServiceKey(service), service, "opcda");
                hostNode.Nodes[ServiceKey(service)].Tag = Type();
            }

            return hostNode.Nodes[ServiceKey(service)];
        }

        public static bool CheckDevice(string host, string service, string device)
        {
            bool result = false;
            if (CheckService(host, service))
            {
                return treeNode.Nodes[HostKey(host)].Nodes[ServiceKey(service)].Nodes.ContainsKey(DeviceKey(device));
            }

            return result;
        }

        public static TreeNode AddDevice(string host, string service, string device)
        {
            AddHost(host);
            TreeNode servicepNode = AddService(host, service);
            if (!CheckDevice(host, service, device))
            {
                servicepNode.Nodes.Add(DeviceKey(device), device, "opcda");
                servicepNode.Nodes[DeviceKey(device)].Tag = Type();
            }

            return servicepNode.Nodes[DeviceKey(device)];
        }

        public static bool CheckGroup(string host, string service, string device, string group)
        {
            bool result = false;
            if (CheckDevice(host, device, service))
            {
                return treeNode.Nodes[HostKey(host)].Nodes[ServiceKey(service)].Nodes[DeviceKey(device)].Nodes.ContainsKey(GroupKey(group));
            }

            return result;
        }

        public static TreeNode AddGroup(string host, string service, string device,  string group)
        {
            AddHost(host);
            AddService(host, service);
            TreeNode deviceNode = AddDevice(host, service, device);
            if (!CheckGroup(host, service, device, group))
            {
                deviceNode.Nodes.Add(GroupKey(group), group, "opcda");
                deviceNode.Nodes[GroupKey(group)].Tag = Type();
            }

            return deviceNode.Nodes[GroupKey(group)];
        }

        public static bool CheckItem(string host, string service, string device, string group, string item)
        {
            bool result = false;
            if (CheckGroup(host, service, device, group))
            {
                return treeNode.Nodes[HostKey(host)].Nodes[ServiceKey(service)].Nodes[DeviceKey(device)].Nodes[GroupKey(group)].Nodes.ContainsKey(ItemKey(item));
            }

            return result;
        }

        public static TreeNode AddItem(string host, string service, string device, string group, string item)
        {
            AddHost(host);
            AddService(host, service);
            AddDevice(host, service, device);
            TreeNode groupNode = AddGroup(host, device, service, group);
            if (!CheckItem(host, service, device, group, item))
            {
                groupNode.Nodes.Add(ItemKey(item), item, "opcda");
                groupNode.Nodes[ItemKey(item)].Tag = Type();
            }

            return groupNode.Nodes[ItemKey(item)];
        }
    }
}