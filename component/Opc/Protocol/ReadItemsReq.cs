// <copyright file="ReadItemsReq.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    using System.Collections.Generic;

    public class ReadItemsReq
    {
        public string ServiceId { get; set; }

        public string GroupId { get; set; }

        public string StrMd5 { get; set; }

        public List<string> Items { get; set; }

        public ReadItemsReq(string serviceProgId, List<string> items, string groupId, string strmd5)
        {
            ServiceId = serviceProgId;
            GroupId = groupId;
            StrMd5 = strmd5;
            Items = items;
        }
    }
}
