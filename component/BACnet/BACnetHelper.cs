// <copyright file="BACnetHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.IO.BACnet;
    using System.Linq;
    using Newtonsoft.Json;

    public class BACnetHelper
    {
        private BACnetHelper()
        {
        }

        private static BACnetHelper instance = null;
        private static BacnetClient bacnetClient = null;

        // All the present Bacnet Device List
        private static List<BacDevice> devicesList = new List<BacDevice>();
        private static int scanBatchStep = 5;
        private static byte invokeId = 0x00;
        private static bool bIsCheck = false;

        public static BACnetHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new BACnetHelper();
            }

            return instance;
        }

        public static void Start()
        {
            Config();
            Stop();
            if (bacnetClient == null)
            {
                // Bacnet on UDP/IP/Ethernet
                bacnetClient = new BacnetClient(new BacnetIpUdpProtocolTransport(0xBAC0, false));

                // or Bacnet Mstp on COM4 à 38400 bps, own master id 8
                // m_bacnet_client = new BacnetClient(new BacnetMstpProtocolTransport("COM4", 38400, 8);
                // Or Bacnet Ethernet
                // bacnetClient = new BacnetClient(new BacnetEthernetProtocolTransport("Connexion au réseau local"));
                // Or Bacnet on IPV6
                // bacnetClient = new BacnetClient(new BacnetIpV6UdpProtocolTransport(0xBAC0));
            }

            bacnetClient.OnIam += Handler_OnIam;
            bacnetClient.Start();
            bacnetClient.WhoIs();
            LogHelper.Log("bacnetClient start IpUdpProtocol 0xBAC0: " + bacnetClient.ToString());
        }

        public static void Stop()
        {
            if (bacnetClient != null)
            {
                devicesList.Clear();
                bacnetClient.Dispose();
                bacnetClient = null;
            }
        }

        public static void Config()
        {
        }

        public static void Write(byte[] data, int offset, int len)
        {
                LogHelper.Log("bacnet write: " + bIsCheck.ToString());

                foreach (var device in devicesList)
                {
                    var count = GetDeviceArrayIndexCount(device);
                    LogHelper.Log("bacnet write: " + device.ToString() + " count: " + count.ToString());
                    ScanPointsBatch(device, count);
                }

                foreach (var device in devicesList)
                {
                    System.IO.File.WriteAllText($"{device.DeviceId}.json", JsonConvert.SerializeObject(device));
                }

                foreach (var device in devicesList)
                {
                    ScanSubProperties(device);
                }

                foreach (var device in devicesList)
                {
                    System.IO.File.WriteAllText($"{device.DeviceId}pppp.json", JsonConvert.SerializeObject(device));
                    LogHelper.Log("bacnet device: " + JsonConvert.SerializeObject(device).ToString());
                }
        }

        // 批量扫点,注意不要太多,超过maxAPDU失败
        public static void ScanPointsBatch(BacDevice device, uint count)
        {
            try
            {
                if (device == null)
                {
                    return;
                }

                var pid = BacnetPropertyIds.PROP_OBJECT_LIST;
                var device_id = device.DeviceId;
                var bobj = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device_id);
                var adr = device.Address;
                if (adr == null)
                {
                    return;
                }

                device.Properties = new List<BacProperty>();

                List<BacnetPropertyReference> rList = new List<BacnetPropertyReference>();

                for (uint i = 1; i < count; i++)
                {
                    rList.Add(new BacnetPropertyReference((uint)pid, i));
                    if (i % scanBatchStep == 0 || i == count) // 不要超了 MaxAPDU
                    {
                        IList<BacnetReadAccessResult> lstAccessRst;
                        var bRst = bacnetClient.ReadPropertyMultipleRequest(adr, bobj, rList, out lstAccessRst, GetCurrentInvokeId());
                        if (bRst)
                        {
                            foreach (var aRst in lstAccessRst)
                            {
                                if (aRst.values == null)
                                {
                                    continue;
                                }

                                foreach (var bPValue in aRst.values)
                                {
                                    if (bPValue.value == null)
                                    {
                                        continue;
                                    }

                                    foreach (var bValue in bPValue.value)
                                    {
                                        var strBValue = "" + bValue.Value;

                                        // mainform.Log(pid + " , " + strBValue + " , " + bValue.Tag);
                                        var strs = strBValue.Split(':');
                                        if (strs.Length < 2)
                                        {
                                            continue;
                                        }

                                        var strType = strs[0];
                                        var strObjId = strs[1];
                                        var subNode = new BacProperty();
                                        BacnetObjectTypes otype;
                                        Enum.TryParse(strType, out otype);
                                        if (otype == BacnetObjectTypes.OBJECT_NOTIFICATION_CLASS || otype == BacnetObjectTypes.OBJECT_DEVICE)
                                        {
                                            continue;
                                        }

                                        subNode.ObjectId = new BacnetObjectId(otype, Convert.ToUInt32(strObjId));
                                        device.Properties.Add(subNode);
                                    }
                                }
                            }
                        }

                        rList.Clear();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static byte GetCurrentInvokeId()
        {
            invokeId = (byte)((invokeId + 1) % 256);
            return invokeId;
        }

        // 获取子节点个数
        public static uint GetDeviceArrayIndexCount(BacDevice device)
        {
            try
            {
                var adr = device.Address;
                if (adr == null)
                {
                    return 0;
                }

                var list = ReadScalarValue(adr,
                    new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device.DeviceId),
                    BacnetPropertyIds.PROP_OBJECT_LIST, 0, 0);
                var rst = Convert.ToUInt32(list.FirstOrDefault().Value);
                return rst;
            }
            catch
            {
            }

            return 0;
        }

        // 逐个扫点,速度较慢
        public static void ScanPointSingle(BacDevice device, uint count)
        {
            if (device == null)
            {
                return;
            }

            var pid = BacnetPropertyIds.PROP_OBJECT_LIST;
            var device_id = device.DeviceId;
            var bobj = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device_id);
            var adr = device.Address;
            if (adr == null)
            {
                return;
            }

            device.Properties = new List<BacProperty>();

            for (uint index = 1; index <= count; index++)
            {
                try
                {
                    var list = ReadScalarValue(adr, bobj, pid, GetCurrentInvokeId(), index);
                    if (list == null)
                    {
                        continue;
                    }

                    foreach (var bValue in list)
                    {
                        var strBValue = "" + bValue.Value;
                        LogHelper.Log(pid + " , " + strBValue + " , " + bValue.Tag);
                        var strs = strBValue.Split(':');
                        if (strs.Length < 2)
                        {
                            continue;
                        }

                        var strType = strs[0];
                        var strObjId = strs[1];
                        var subNode = new BacProperty();
                        BacnetObjectTypes otype;
                        Enum.TryParse(strType, out otype);
                        subNode.ObjectId = new BacnetObjectId(otype, Convert.ToUInt32(strObjId));
                        device.Properties.Add(subNode);
                    }
                }
                catch (Exception exp)
                {
                    LogHelper.Log("Error: " + index + " , " + exp.Message);
                }
            }
        }

        public static void ScanSubProperties(BacDevice device)
        {
            var adr = device.Address;
            if (adr == null)
            {
                return;
            }

            if (device.Properties == null)
            {
                return;
            }

            foreach (BacProperty subNode in device.Properties)
            {
                try
                {
                    List<BacnetPropertyReference> rList = new List<BacnetPropertyReference>();
                    rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_DESCRIPTION, uint.MaxValue));
                    rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_REQUIRED, uint.MaxValue));
                    IList<BacnetReadAccessResult> lstAccessRst;
                    var bRst = bacnetClient.ReadPropertyMultipleRequest(adr, subNode.ObjectId, rList, out lstAccessRst, GetCurrentInvokeId());
                    if (bRst)
                    {
                        foreach (var aRst in lstAccessRst)
                        {
                            if (aRst.values == null)
                            {
                                continue;
                            }

                            foreach (var bPValue in aRst.values)
                            {
                                if (bPValue.value == null || bPValue.value.Count == 0)
                                {
                                    continue;
                                }

                                var pid = (BacnetPropertyIds)bPValue.property.propertyIdentifier;
                                var bValue = bPValue.value.First();
                                var strBValue = "" + bValue.Value;
                                LogHelper.Log(pid + " , " + strBValue + " , " + bValue.Tag);
                                switch (pid)
                                {
                                    case BacnetPropertyIds.PROP_DESCRIPTION: // 描述
                                        {
                                            subNode.PROP_DESCRIPTION = bValue + "";
                                        }

                                        break;
                                    case BacnetPropertyIds.PROP_OBJECT_NAME: // 点名
                                        {
                                            subNode.PROP_OBJECT_NAME = bValue + "";
                                        }

                                        break;
                                    case BacnetPropertyIds.PROP_PRESENT_VALUE: // 值
                                        {
                                            subNode.PROP_PRESENT_VALUE = bValue.Value;
                                        }

                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    LogHelper.Log("Error: " + exp.Message.ToString(), (int)LogHelper.Level.ERROR);
                }
            }
        }

        private static void Handler_OnIam(BacnetClient sender, BacnetAddress adr, uint deviceId, uint maxAPDU,
                                  BacnetSegmentations segmentation, ushort vendorId)
        {
            if (devicesList == null)
            {
                devicesList = new List<BacDevice>();
            }

            lock (devicesList)
            {
                if (devicesList.Any(x => x.DeviceId == deviceId))
                {
                    return;
                }

                int index = 0;
                for (; index < devicesList.Count; index++)
                {
                    if (devicesList[index].DeviceId > deviceId)
                    {
                        break;
                    }
                }

                devicesList.Insert(index, new BacDevice(adr, deviceId));
                LogHelper.Log(@"Detect Device: " + deviceId);
            }
        }

        /*****************************************************************************************************/
        private static bool ReadScalarValue(int device_id, BacnetObjectId bacnetObjet, BacnetPropertyIds propriete, out BacnetValue value)
        {
            BacnetAddress adr;
            IList<BacnetValue> noScalarValue;

            value = new BacnetValue(null);

            // Looking for the device
            adr = DeviceAddr((uint)device_id);
            if (adr == null)
            {
                return false;  // not found
            }

            // Property Read
            if (bacnetClient.ReadPropertyRequest(adr, bacnetObjet, propriete, out noScalarValue) == false)
            {
                return false;
            }

            value = noScalarValue[0];
            return true;
        }

        private static IList<BacnetValue> ReadScalarValue(BacnetAddress adr, BacnetObjectId oid,
            BacnetPropertyIds pid, byte invokeId = 0, uint arrayIndex = uint.MaxValue)
        {
            try
            {
                IList<BacnetValue> noScalarValue;
                var rst = bacnetClient.ReadPropertyRequest(adr, oid, pid, out noScalarValue, invokeId, arrayIndex);
                if (!rst)
                {
                    return null;
                }

                return noScalarValue;
            }
            catch
            {
            }

            return null;
        }

        /*****************************************************************************************************/
        private static bool WriteScalarValue(int device_id, BacnetObjectId bacnetObjet, BacnetPropertyIds propriete, BacnetValue value)
        {
            BacnetAddress adr;

            // Looking for the device
            adr = DeviceAddr((uint)device_id);
            if (adr == null)
            {
                return false;  // not found
            }

            // Property Write
            BacnetValue[] noScalarValue = { value };
            if (bacnetClient.WritePropertyRequest(adr, bacnetObjet, propriete, noScalarValue) == false)
            {
                return false;
            }

            return true;
        }

        /*****************************************************************************************************/
        private static BacnetAddress DeviceAddr(uint deviceId)
        {
            BacnetAddress ret;

            lock (devicesList)
            {
                foreach (BacDevice bn in devicesList)
                {
                    ret = bn.GetAddr(deviceId);
                    if (ret != null)
                    {
                        return ret;
                    }
                }

                // not in the list
                return null;
            }
        }
    }
}