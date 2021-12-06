// <copyright file="WriteNodesValuesReq.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Da
{
    internal class WriteNodesValuesReq
    {
        public string ServiceId { get; set; }

        public string GroupId { get; set; }

        public string StrMd5 { get; set; }

        public Dictionary<string, object> ItemValuePairs { get; set; }

        public WriteNodesValuesReq(string serviceProgId, Dictionary<string, object> items, string groupId, string strmd5)
        {
            ServiceId = serviceProgId;
            GroupId = groupId;
            StrMd5 = strmd5;
            ItemValuePairs = items;
        }
    }
}
