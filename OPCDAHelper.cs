using System;
using System.Text;
using MQTTnet.Core.Client;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Common;

using MQTTnet.Core;
using System.Collections.Generic;

using MQTTnet.Core.Protocol;
using TitaniumAS.Opc.Client.Da.Browsing;

//https://github.com/titanium-as/TitaniumAS.Opc.Client
//https://github.com/chkr1011/MQTTnet

namespace dgiot_dtu
{

    public class OPCDAHelper
    {
        private static string pubtopic = "dgiot_opc_da_ack";
        private static string scantopic = "dgiot_opc_da_scan";
        private static MainForm _mainform = null;

        public static void do_opc_da(MqttClient mqttClient, Dictionary<string, object> json, MainForm mainform)
        {
            _mainform = mainform;
            string cmdType = "read";
            if (json.ContainsKey("cmdtype"))
            {
                try
                {
                    cmdType = (string)json["cmdtype"];
                    switch (cmdType)
                    {
                        case "scan":
                            scan_opc_da(mqttClient,json);
                            break;
                        case "read":
                            read_opc_da(mqttClient, json);
                            break;
                        case "write":
                            break;
                        default:
                            read_opc_da(mqttClient, json);
                            break;
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }

        }

        private static void scan_opc_da(MqttClient mqttClient, Dictionary<string, object> json)
        {
            string opcserver = "Matrikon.OPC.Simulation.1";
      
            IList<OpcDaItemDefinition> itemlist = new List<OpcDaItemDefinition>();
            if (json.ContainsKey("opcserver"))
            {
                try
                {
                    opcserver = (string)json["opcserver"];
                }
                catch (Exception ex)
                {
                    _mainform.Log(ex.ToString());
                }
            }

            Uri url = UrlBuilder.Build(opcserver);
        
            _mainform.Log("opcserver " + opcserver.ToString());
            try
            {    
                using (var server = new OpcDaServer(url))
                {
                    // Connect to the server first.
                    server.Connect();
                    var browser = new OpcDaBrowserAuto(server);
                    JsonObject scan = new JsonObject();
                    BrowseChildren(scan, browser);
                    var appMsg = new MqttApplicationMessage(scantopic, Encoding.UTF8.GetBytes(scan.ToString()), MqttQualityOfServiceLevel.AtLeastOnce, false);
                    _mainform.Log("appMsg " + scan.ToString());
                    mqttClient.PublishAsync(appMsg);
                }
            }
            catch (Exception ex)
            {
                _mainform.Log(" error  " +  ex.GetBaseException().ToString());
                JsonObject result = new JsonObject();
                result.Add("TimeStamp", FromDateTime(DateTime.UtcNow));
                result.Add("opcserver", opcserver);
                result.Add("status", ex.GetHashCode());
                result.Add("err", ex.ToString());
                var appMsg = new MqttApplicationMessage(pubtopic, Encoding.UTF8.GetBytes(result.ToString()), MqttQualityOfServiceLevel.AtLeastOnce, false);
                _mainform.Log("appMsg  " +  appMsg.ToString());
                mqttClient.PublishAsync(appMsg);
            }
        }

        private static void BrowseChildren(JsonObject json,IOpcDaBrowser browser, string itemId = null, int indent = 0)
        {
            // When itemId is null, root elements will be browsed.
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            JsonArray array = new JsonArray();
            Boolean flag = false;
            foreach (OpcDaBrowseElement element in elements)
            {
                // Skip elements without children.
               if (!element.HasChildren){       
                    array.Add(element);
                    flag = true;
                    continue;
                }     
                // Output children of the element.
                BrowseChildren(json, browser, element.ItemId, indent + 2);
            }

            if (flag){
                if (null != itemId)
                {
                    json.Add(itemId, array);
                } 
            }     
        }

        private static void read_opc_da(MqttClient mqttClient, Dictionary<string, object>  json)
        {
            string opcserver = "Matrikon.OPC.Simulation.1";
            string group = "addr";
            IList<OpcDaItemDefinition> itemlist = new List<OpcDaItemDefinition>();
            if (json.ContainsKey("opcserver"))
            {
                try
                {
                    opcserver = (string)json["opcserver"];
                }
                catch (Exception ex)
                {
                    _mainform.Log( ex.ToString());
                }
            }

            if (json.ContainsKey("group"))
            {
                try
                {
                    group = (string)json["group"];
                }
                catch (Exception ex)
                {
                    _mainform.Log( ex.ToString());
                }
            }

            if (json.ContainsKey("items"))
            {
                try
                {
                    string items = (string)json["items"];
                    Console.WriteLine(" from task {0} {1} {2} ", opcserver, group, items);
                    string[] arry = items.Split(',');
                    JsonObject data = new JsonObject();
                    try
                    { 
                        JsonObject result = new JsonObject();
                        read_group(mqttClient, opcserver, group, arry, data);
                        result.Add("status", 0);
                        result.Add(group, data);
                        _mainform.Log("result " + result.ToString());
                        var appMsg = new MqttApplicationMessage(pubtopic, Encoding.UTF8.GetBytes(result.ToString()), MqttQualityOfServiceLevel.AtLeastOnce, false);
                        mqttClient.PublishAsync(appMsg);
                    }
                    catch (Exception ex)
                    {
                        _mainform.Log(ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _mainform.Log(ex.ToString());
                }
            }

        }

        private static void read(MqttClient mqttClient, string opcserver, string group_name, string[] arry, JsonObject items)
        {
            Uri url = UrlBuilder.Build(opcserver);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    // Connect to the server first.
                    
                    foreach (string id in arry)
                    {
                        server.Connect();
                        OpcDaGroup group = server.AddGroup(group_name);
                        var definition = new OpcDaItemDefinition
                        {
                            ItemId = id,
                            IsActive = true
                        };
                        group.IsActive = true;
                        OpcDaItemDefinition[] definitions = { definition };
                        OpcDaItemResult[] results = group.AddItems(definitions);
                        OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);
                        foreach (OpcDaItemValue item in values)
                        {
                            _mainform.Log( pubtopic + " " + id.ToString() + " " + item.GetHashCode().ToString() + " " + item.Value.ToString() + "" + item.Timestamp.ToString());
                            items.Add(id, item.Value);
                        }
                        server.Disconnect();
                    }   
                }
            }
            catch (Exception ex)
            {
                _mainform.Log(ex.GetBaseException().ToString());
                JsonObject result = new JsonObject();
                result.Add("opcserver", opcserver);
                result.Add("status", ex.GetHashCode());
                result.Add("err", ex.ToString());
                var appMsg = new MqttApplicationMessage(pubtopic, Encoding.UTF8.GetBytes(result.ToString()), MqttQualityOfServiceLevel.AtLeastOnce, false);
                mqttClient.PublishAsync(appMsg);
            }
        }
        private static void read_group(MqttClient mqttClient, string opcserver, string group_name, string[] arry, JsonObject items)
        {
            Uri url = UrlBuilder.Build(opcserver);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    // Connect to the server first.
                    server.Connect();
                    // Create a group with items.
                    OpcDaGroup group = server.AddGroup(group_name);
                    IList<OpcDaItemDefinition> definitions = new List<OpcDaItemDefinition>();
                    int i = 0;
                    foreach (string id in arry)
                    {
                        var definition = new OpcDaItemDefinition
                        {
                            ItemId = id,
                            IsActive = true
                        };
                        definitions.Insert(i++, definition);
                    }
                    group.IsActive = true;
                    OpcDaItemResult[] results = group.AddItems(definitions);
                    OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);

                  
                    // Handle adding results.
                    JsonObject data = new JsonObject();
                    foreach (OpcDaItemValue item in values)
                    {
                        _mainform.Log(pubtopic + " "  + item.GetHashCode().ToString() + " " + item.Value.ToString() + "" + item.Timestamp.ToString());
                        data.Add(item.Item.ItemId, item.Value);
                    }
                    items.Add("status", 0);
                    items.Add(group_name, data);
                    _mainform.Log(items.ToString());
                    var appMsg = new MqttApplicationMessage(pubtopic, Encoding.UTF8.GetBytes(items.ToString()), MqttQualityOfServiceLevel.AtLeastOnce, false);
                    mqttClient.PublishAsync(appMsg);
                    server.Disconnect();
                }
            }
            catch (Exception ex)
            {
                _mainform.Log(ex.ToString());
                read(mqttClient,opcserver, group_name, arry, items);
            }
        }

        private static void subscription_opc_da(MqttClient mqttClient, string opcserver, string name)
        {
            Uri url = UrlBuilder.Build(opcserver);
            try
            {
                using (var server = new OpcDaServer(url))
                {
                    // Connect to the server first.
                    server.Connect();
                    // Create a group with items.
                    OpcDaGroup group = server.AddGroup("Group1");
                    group.IsActive = true;

                    var definition = new OpcDaItemDefinition
                    {
                        ItemId = name,
                        IsActive = true
                    };
                    
                    OpcDaItemDefinition[] definitions = { definition };
                   
                    OpcDaItemResult[] results = group.AddItems(definitions);
                   
                    group.ValuesChanged += OnGroupValuesChanged;
                    group.UpdateRate = TimeSpan.FromMilliseconds(100);
                }
            }
            catch (Exception ex)
            {
                _mainform.Log(ex.GetBaseException().ToString());
                JsonObject result = new JsonObject();
                result.Add("opcserver", opcserver);
                result.Add("name", name);
                result.Add("status", ex.GetHashCode());
                result.Add("err", ex.ToString());
                var appMsg = new MqttApplicationMessage(pubtopic, Encoding.UTF8.GetBytes(result.ToString()), MqttQualityOfServiceLevel.AtLeastOnce, false);
                mqttClient.PublishAsync(appMsg);
            }
        }

        static void OnGroupValuesChanged(object sender, OpcDaItemValuesChangedEventArgs args)
        {
            // Output values.
            foreach (OpcDaItemValue value in args.Values)
            {
                _mainform.Log("ItemId: " + value.Item.ItemId.ToString() + "; Value: {1}" + value.Value.ToString() +  
                    ";Quality: " + value.Quality.ToString() + ";Timestamp: {3}" +  value.Timestamp.ToString());
            }
        }

        private static DateTime BaseTime = new DateTime(1970, 1, 1);

        /// <summary>   
        /// 将unixtime转换为.NET的DateTime   
        /// </summary>   
        /// <param name="timeStamp">秒数</param>   
        /// <returns>转换后的时间</returns>   
        public static DateTime FromUnixTime(long timeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(timeStamp * 10000000 + BaseTime.Ticks));
        }

        /// <summary>   
        /// 将.NET的DateTime转换为unix time   
        /// </summary>   
        /// <param name="dateTime">待转换的时间</param>   
        /// <returns>转换后的unix time</returns>   
        public static long FromDateTime(DateTime dateTime)
        {
            return (TimeZone.CurrentTimeZone.ToUniversalTime(dateTime).Ticks - BaseTime.Ticks) / 10000000;
        }

    }

    }