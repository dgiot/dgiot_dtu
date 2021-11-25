// <copyright file="StartMonitoringItemsReq.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Da
{
    public class StartMonitoringItemsReq
    {
        public string ServiceId { get; set; }

        public string StrMd5 { get; set; }

        public List<string> Items { get; set; }

        public StartMonitoringItemsReq(string serviceProgId, List<string> items, string strmd5)
        {
            ServiceId = serviceProgId;
            Items = items;
            StrMd5 = strmd5;
        }
    }
}
