// <copyright file="TreeViewHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;

    public class TreeViewHelper
    {
        private TreeViewHelper()
        {
        }

        private static TreeViewHelper instance = null;
        private static TreeView treeView = null;
        private static TreeNode treenode = new TreeNode();

        public static TreeViewHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new TreeViewHelper();
            }

            return instance;
        }

        public static void Init(TreeView treeView)
        {
            TreeViewHelper.treeView = treeView;
        }

        public static void UpdateTreeView()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Add(treenode);
            treeView.EndUpdate();
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
        }

        public static void AddNode(TreeNode node)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.Add(node);
            treeView.EndUpdate();
        }

        public static void AppendNode(TreeNode node)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Add(node);
            treeView.EndUpdate();
        }

        public static void DeleteNode(TreeNode node)
        {
            treeView.Nodes.Remove(node);
        }

        public static void ClearNodes()
        {
            treeView.Nodes.Clear();
        }

        /// <summary>
        /// 通过fullpath 查找treeview节点 节点name属性需要赋过值
        /// </summary>
        /// <param name="nodes">TreeNodeCollection node集合</param>
        /// <param name="fullPath">要查找的节点的fullPath</param>
        /// <returns> TreeNode  </returns>
        public static TreeNode GetNode(TreeNodeCollection nodes, string fullPath)
        {
            string[] paths = fullPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            TreeNode tn = null;
            if (paths.Length > 0)
            {
                string lastPath = paths[paths.Length - 1];
                var finds = nodes.Find(lastPath, true);
                if (finds.Length > 0)
                {
                    foreach (var item in finds)
                    {
                        if (item.FullPath == fullPath)
                        {
                            tn = item;
                            break;
                        }
                    }
                }
            }

            return tn;
        }

        // 设置子节点状态
        public static void SetChildNodeCheckedState(TreeNode currNode, bool isCheckedOrNot)
        {
            if (currNode.Nodes == null)
            {
                return; // 没有子节点返回
            }

            foreach (TreeNode tmpNode in currNode.Nodes)
            {
                LogHelper.Log("node  " + tmpNode.Text);
                tmpNode.Checked = isCheckedOrNot;
                SetChildNodeCheckedState(tmpNode, isCheckedOrNot);
            }
        }

        // 设置父节点状态
        public static void SetParentNodeCheckedState(TreeNode currNode, bool isCheckedOrNot)
        {
            if (currNode.Parent == null)
            {
                return; // 没有父节点返回
            }

            if (isCheckedOrNot) // 如果当前节点被选中，则设置所有父节点都被选中
            {
                currNode.Parent.Checked = isCheckedOrNot;
                SetParentNodeCheckedState(currNode.Parent, isCheckedOrNot);
            }
            else // 如果当前节点没有被选中，则当其父节点的子节点有一个被选中时，父节点被选中，否则父节点不被选中
            {
                bool checkedFlag = false;
                foreach (TreeNode tmpNode in currNode.Parent.Nodes)
                {
                    if (tmpNode.Checked)
                    {
                        checkedFlag = true;
                        break;
                    }
                }

                currNode.Parent.Checked = checkedFlag;
                SetParentNodeCheckedState(currNode.Parent, checkedFlag);
            }
        }
    }
}