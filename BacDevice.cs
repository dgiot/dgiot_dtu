// <copyright file="BacDevice.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System.Collections.Generic;
    using System.IO.BACnet;

    public class BacDevice
    {
        #region Address

        public BacnetAddress Address { get; set; }

        #endregion

        #region DeviceId

        public uint DeviceId { get; set; }

        public List<BacProperty> Properties { get; set; }

        #endregion

        public BacDevice(BacnetAddress adr, uint deviceId)
        {
            this.Address = adr;
            this.DeviceId = deviceId;
        }

        public BacnetAddress GetAddr(uint deviceId)
        {
            if (this.DeviceId == deviceId)
            {
                return Address;
            }
            else
            {
                return null;
            }
        }
    }
}