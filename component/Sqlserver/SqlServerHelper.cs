// <copyright file="SqlServerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Configuration;

namespace Dgiot_dtu
{
    public class SqlServerHelper
    {
        private SqlServerHelper()
        {
        }

        private static SqlServerHelper instance;

        public static SqlServerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new SqlServerHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config)
        {
            Config(config);
        }

        public static void Stop()
        {
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
        }
    }
}