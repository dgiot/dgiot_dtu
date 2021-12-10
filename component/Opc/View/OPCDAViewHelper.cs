// <copyright file="OPCDAViewHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Da;
    using TitaniumAS.Opc.Client.Da.Browsing;

    public class OPCDAViewHelper
    {
        public enum NodeType
        {
            OPCDA = 0,
            Host = 1,
            Service = 2,
            Device = 3,
            Group = 4,
            Item = 5,
            Property = 6
        }

        private OPCDAViewHelper()
        {
        }

        private static OPCDAViewHelper instance = null;
        private static TreeNode opcdaNode = new TreeNode(Type(), 0, 0);

        public static OPCDAViewHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new OPCDAViewHelper();
                opcdaNode.Tag = Type();
                opcdaNode.ForeColor = System.Drawing.Color.Red;
            }

            return instance;
        }

        public static string Type()
        {
            return TreeViewHelper.Type(TreeViewHelper.NodeType.OPCDA);
        }

        public static void View()
        {
            TreeViewHelper.AddNode(opcdaNode);
            OPCDAHelper.View();
        }

        public static string Key(TreeNode parentNode, string itemid)
        {
            return DgiotHelper.Md5(parentNode.FullPath + "/" + itemid).Substring(0, 10);
        }

        public static string Key(string path)
        {
            return DgiotHelper.Md5(path).Substring(0, 10);
        }

        public static TreeNode GetRootNode()
        {
            return opcdaNode;
        }

        public static TreeNode GetNode(string path)
        {
            TreeNode[] nodes = opcdaNode.Nodes.Find(Key(path), true);
            if (nodes.Length > 0)
            {
                return nodes[0];
            }

            return null;
        }

        public static TreeNode GetTreeNodes(OpcDaService server)
        {
            TreeNode hostNode = GetNode(opcdaNode.FullPath + "/" + server.Host);
            TreeNode serviceNode = GetNode(hostNode.FullPath + "/" + server.ServiceId);
            try
            {
                OpcDaBrowserAuto browserAuto = new OpcDaBrowserAuto(server.Service);
                BrowseChildren(browserAuto, serviceNode);
            }
            catch (Exception)
            {
                return serviceNode;
            }

            return serviceNode;
        }

        public static void BrowseChildren(IOpcDaBrowser browser, TreeNode node, string itemId = null, int indent = 0)
        {
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            foreach (OpcDaBrowseElement element in elements)
            {
                if (!(element.ItemId.IndexOf('$') == 0))
                {
                    TreeNode curNode = AddNode(node, element.Name, element.ItemId);
                    if (element.HasChildren && curNode != null)
                    {
                        BrowseChildren(browser, curNode, element.ItemId, indent + 2);
                    }
                }
            }
        }

        public static TreeNode AddNode(TreeNode parentNode, string name, string itemid)
        {
            if (parentNode == null)
            {
                return null;
            }

            if (!opcdaNode.Nodes.ContainsKey(Key(parentNode, itemid)))
            {
                parentNode.Nodes.Add(Key(parentNode, itemid), itemid);
            }

            switch (parentNode.Level + 1)
            {
                case (int)NodeType.OPCDA: // OPCDA
                    parentNode.Nodes[Key(parentNode, itemid)].ForeColor = System.Drawing.Color.Black;
                    break;
                case (int)NodeType.Host: // host
                    parentNode.Nodes[Key(parentNode, itemid)].ForeColor = System.Drawing.Color.Black;
                    break;
                case (int)NodeType.Service: // service
                    parentNode.Nodes[Key(parentNode, itemid)].ForeColor = System.Drawing.Color.Black;
                    break;
                case (int)NodeType.Device: // device
                    if (IsDevicefilter(name) || IsDevicefilter(itemid))
                    {
                        parentNode.Nodes.RemoveByKey(Key(parentNode, itemid));
                        return null;
                    }

                    parentNode.Nodes[Key(parentNode, itemid)].ForeColor = System.Drawing.Color.Black;
                    break;
                case (int)NodeType.Group: // group
                    if (IsGroupfilter(name) || IsGroupfilter(itemid))
                    {
                        parentNode.Nodes.RemoveByKey(Key(parentNode, itemid));
                        return null;
                    }

                    parentNode.Nodes[Key(parentNode, itemid)].ForeColor = System.Drawing.Color.Blue;
                    break;
                case (int)NodeType.Item: // item
                    if (IsItemsfilter(name) || IsItemsfilter(itemid))
                    {
                        parentNode.Nodes.RemoveByKey(Key(parentNode, itemid));
                        return null;
                    }

                    parentNode.Nodes[Key(parentNode, itemid)].ForeColor = System.Drawing.Color.Green;
                    break;
                case (int)NodeType.Property: // property
                    parentNode.Nodes[Key(parentNode, itemid)].ForeColor = System.Drawing.Color.Green;
                    parentNode.Nodes[Key(parentNode, itemid)].Parent.ForeColor = System.Drawing.Color.Red;
                    parentNode.Nodes[Key(parentNode, itemid)].Parent.Parent.ForeColor = System.Drawing.Color.Gray;
                    break;
                default:
                    break;
            }

            parentNode.Nodes[Key(parentNode, itemid)].Tag = Type();
            parentNode.Nodes[Key(parentNode, itemid)].ToolTipText = name;
            return parentNode.Nodes[Key(parentNode, itemid)];
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
            "_ThingWorx",
            "._Statistics",
            "._System",
            ".SystemVariable"
        };

        private static bool IsDevicefilter(string device)
        {
            bool result = false;
            foreach (string filter in DeviceFilter)
            {
                if (-1 != device.LastIndexOf(filter))
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
            "_ThingWorx",
            "._Statistics",
            "._System",
            ".SystemVariable"
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
    }
}