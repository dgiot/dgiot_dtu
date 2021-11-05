// <copyright file="BacProperty.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System.IO.BACnet;

    public class BacProperty
    {
        #region ObjectId

        public BacnetObjectId ObjectId { get; set; }

        #endregion

        #region PROP_DESCRIPTION 描述

        public string PROP_DESCRIPTION { get; set; }

        #endregion

        #region PROP_OBJECT_NAME 点名

        public string PROP_OBJECT_NAME { get; set; }

        #endregion

        public object PROP_PRESENT_VALUE { get; set; }
    }
}