// <copyright file="IOPCDa.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TitaniumAS.Opc.Client.Da;

namespace Da
{
    public interface IOPCDa
    {
        List<string> ScanOPCDa(string host, bool isClean = true);

        string StartMonitoringItems(string host, string serviceProgId, TreeNode groupNode, int interval, int count);

        void SetItemsValueChangedCallBack(IItemsValueChangedCallBack callBack);

        void StopMonitoringItems(string groupKey);

        List<Item> ReadItemsValues(string host, string serviceProgId, string groupKey);

        void WriteValues(string host, string serviceProgId, string groupKey, Dictionary<string, object> itemValuePairs);
    }

    public interface IItemsValueChangedCallBack
    {
        void ValueChangedCallBack(OpcDaGroup opcGroup, OpcDaItemValue[] values);
    }
}
