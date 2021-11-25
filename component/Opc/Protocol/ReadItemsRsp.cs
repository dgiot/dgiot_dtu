// <copyright file="ReadItemsRsp.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Da
{
    public class ReadItemsRsp
    {
        public string ServiceId { get; set; }

        public string GroupId { get; set; }

        public string StrMd5 { get; set; }

        public List<Item> ItemValues { get; set; }
    }
}
