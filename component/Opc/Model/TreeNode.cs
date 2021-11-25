// <copyright file="TreeNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public enum TreeNodeType
    {
        Root,
        Local,
        Remote,
        Property
    }

    public class TreeNode
    {
        public string Name { get; set; }

        [JsonIgnore]
        public TreeNodeType NodeType { get; set; }

        public List<TreeNode> Children { get; set; }

        public TreeNode()
        {
            Children = new List<TreeNode>();
            NodeType = TreeNodeType.Local;
        }
    }
}
