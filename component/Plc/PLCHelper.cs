// <copyright file="PLCHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// https://www.cnblogs.com/dathlin/p/8685855.html
// https://github.com/dathlin/HslCommunication
namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using PLCComHelperProj;

    public class PLCHelper
    {
        private PLCHelper()
        {
        }

        private static PLCHelper instance;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;
        private static ClientComHelper m_PLCComHelper = new ClientComHelper();

        public static PLCHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new PLCHelper();
            }

            m_PLCComHelper.TagConfigFile = Path.Combine(Application.StartupPath, "syscfg.xml");

            m_PLCComHelper.Init();
            return instance;
        }

        public static void Start(String setValue)
        {
            PLCHelper.bIsRunning = true;
            

            LogHelper.Log("value " + m_PLCComHelper.IP);
            // 0：未连接，1：TCP连接成功，2：PLC握手成功，3：读取过程中
            bool res = true;
            switch (m_PLCComHelper.CommStatus)
            {
                case 0:
                    //label1.Text = "未连接";
                    res = m_PLCComHelper.Start();
                    break;

                case 1:
                    //label1.Text = "正在进行TCP连接";
                    break;

                case 2:
                    //label1.Text = "TCP连接成功";
                    break;

                case 3:
                    //label1.Text = "PLC握手成功";
                    break;

                case 4:
                    //label1.Text = "正常采集过程中...";
                    break;
                case 5:
                    //label1.Text = "PLC握手错误";
                    res = m_PLCComHelper.Start();
                    break;

                case 6:
                    //label1.Text = "通讯错误";
                    res = m_PLCComHelper.Start();
                    break;

                default:
                    res = m_PLCComHelper.Start();
                    break;
            }

            
            if (!res)
            {
                LogHelper.Log("连接失败");
            }
            else
            {
                LogHelper.Log("连接成功");

                Device s7 = m_PLCComHelper.GetDevice();

                foreach (KeyValuePair<string, TagGroup> ch in s7.TagGroups)
                {
                    TagGroup c = ch.Value;
                    foreach (KeyValuePair<string, Tag> tag in c.Tags)
                    {
                        Tag ta = tag.Value;
                        string name = string.Format("{0}.{1}", c.Name, ta.Name);
                        switch (ta.CheckDataType())
                        {
                            case e_PLC_DATA_TYPE.TYPE_INT:
                               // m_PLCComHelper.WriteData(name, Convert.ToInt32(setValue));
                                break;
                            case e_PLC_DATA_TYPE.TYPE_BYTE:
                               // m_PLCComHelper.WriteData(name, Convert.ToInt32(setValue)); 
                                break;

                            case e_PLC_DATA_TYPE.TYPE_FLOAT:
                                //m_PLCComHelper.WriteData(name, Convert.ToDouble(setValue));               
                                break;

                            case e_PLC_DATA_TYPE.TYPE_SHORT:
                               // m_PLCComHelper.WriteData(name, Convert.ToInt32(setValue));
                                break;

                            case e_PLC_DATA_TYPE.TYPE_BOOL:
                               // m_PLCComHelper.WriteData(name, Convert.ToInt32(setValue));
                                break;

                            default:
                                break;
                        }

                    }
                }
                Thread.Sleep(1000);
                s7 = m_PLCComHelper.GetDevice();
                foreach (KeyValuePair<string, TagGroup> ch in s7.TagGroups)
                {
                    TagGroup c = ch.Value;
                    foreach (KeyValuePair<string, Tag> tag in c.Tags)
                    {
                        Tag ta = tag.Value;

                        string name = string.Format("{0}.{1}", c.Name, ta.Name);
                        String value = "";
                        switch (ta.CheckDataType())
                        {
                            case e_PLC_DATA_TYPE.TYPE_INT:                          
                                value = Convert.ToInt32(m_PLCComHelper.GetValue(name)).ToString();
                                break;
                            case e_PLC_DATA_TYPE.TYPE_BYTE:
                                value = Convert.ToInt32(m_PLCComHelper.GetValue(name)).ToString();
                                break;

                            case e_PLC_DATA_TYPE.TYPE_FLOAT:
                                value = m_PLCComHelper.GetValue(name).ToString();
                                break;

                            case e_PLC_DATA_TYPE.TYPE_SHORT:
                                value = Convert.ToInt32(m_PLCComHelper.GetValue(name)).ToString();
                                break;

                            case e_PLC_DATA_TYPE.TYPE_BOOL:
                                value = Convert.ToInt32(m_PLCComHelper.GetValue(name)).ToString();
                                break;

                            default:
                                break;
                        }

                        // LogHelper.Log("plc name : " + name + " value " + value + " DataType: " + ta.DataType + " Address : " + ta.GetAddressName());
                        LogHelper.Log("name : " + name + " value " + value + " Type: " + ta.DataType);
                    }
                }
            }
        }

        public static void Stop()
        {
            PLCHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["PLCIsCheck"] != null)
            {
                bIsCheck = DgiotHelper.StrTobool(config["PLCIsCheck"].Value);
            }
        }

        public static void Write(byte[] data, int offset, int len)
        {
        }
    }
}