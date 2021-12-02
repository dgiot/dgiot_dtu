// <copyright file="IOPCDa.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using TitaniumAS.Opc.Client.Da;

namespace Da
{
    public interface IOPCDa
    {
        string[] ScanOPCDa(string host, Boolean isClean = true);

        IList<TreeNode> GetTreeNodes(string service);

        string StartMonitoringItems(string serviceProgId, List<string> itemIds, string strMd5);

        void SetItemsValueChangedCallBack(IItemsValueChangedCallBack callBack);

        void StopMonitoringItems(string serviceProgId, string groupId);

        List<Item> ReadItemsValues(string serverID, string groupId, List<string> items,  string strMd5);

        void WriteValues(string serviceProgId, string groupId, Dictionary<string, object> itemValuePairs);
    }

    public interface IItemsValueChangedCallBack
    {
        void ValueChangedCallBack(string group, OpcDaItemValue[] values);
    }
}
