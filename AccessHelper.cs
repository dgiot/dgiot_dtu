using System;
using System.Data.OleDb;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Data.Odbc;
using MQTTnet.Core.Client;
using MQTTnet.Core;
using MQTTnet.Core.Protocol;
using System.Text;

namespace dgiot_dtu
{

    class AccessHelper
    {
        private static  OleDbCommand ole_command = null;
        private static OleDbDataReader ole_reader = null;
        private static DataTable dt = null;

        private static string MdbFile = "test.mdb";
        private static string Dbq = Path.Combine(Environment.CurrentDirectory, MdbFile);
        private static string Security = "TRUE";
        private static string Uid = "Admin";
        private static string Pwd = "123456";

        private static string scantopic = "dgiot_mdb_scan";

        //定义连接字符串

        private static string OdbcconnectionString = 
            "Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + Dbq +
            "; Uid=" + Uid + "; Pwd=" + Pwd + ";";

        public static string ConnectionString =
             @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + Dbq + ";" +
               "Persist Security Info = False; Jet OLEDB:Database Password = " + Pwd + ";";


        public static void do_mdb(MqttClient mqttClient, Dictionary<string, object> json, MainForm mainform)
        {
            string cmdType = "read";
            if (json.ContainsKey("cmdtype"))
            {
                try
                {
                    cmdType = (string)json["cmdtype"];
                    switch (cmdType)
                    {
                        case "scan":
                            scan_mdb(mqttClient, json);
                            break;
                        case "read":
                            read_mdb(mqttClient, json);
                            break;
                        case "write":
                            break;
                        default:
                            read_mdb(mqttClient, json);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }

        }

        public static void scan_mdb(MqttClient mqttClient, Dictionary<string, object> json)
        {
            if (json.ContainsKey("dbq"))
            {
                try
                {
                    Dbq = (string)json["dbq"];
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }

            if (json.ContainsKey("security"))
            {
                try
                {
                    Security = (string)json["security"];
                    Security = Security.ToUpper();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }

            if (json.ContainsKey("uid"))
            {
                try
                {
                    Uid = (string)json["uid"];
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }

            if (json.ContainsKey("pwd"))
            {
                try
                {
                    Pwd = (string)json["pwd"];
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }


            if (Security == "TRUE")
            {
                OdbcconnectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + Dbq + "; Uid=" + Uid + "; Pwd=" + Pwd + ";";
            }else
            {
                OdbcconnectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + Dbq;
            }

            Console.WriteLine(OdbcconnectionString);

            using (var db = new OdbcConnection(OdbcconnectionString))
            {
                db.Open();
                var schemaTable = db.GetSchema("Tables");
                var dataSet = new DataSet();
                for (var i = 0; i < schemaTable.Rows.Count; i++)
                {
                    //only source tables
                    if (schemaTable.Rows[i]["TABLE_TYPE"].ToString() == "TABLE")
                    {
                        var tableName = schemaTable.Rows[i]["TABLE_NAME"].ToString();
                        var sql = "SELECT TOP 1 * FROM [" + tableName + "]";

                        var dataTable = new DataTable(tableName);
                        using (var command = new OdbcCommand(sql, db))
                        {
                            using (var adapter = new OdbcDataAdapter(command))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                        //Console.WriteLine(tableName + "(" + dataTable.Rows.Count + " rows)");
                        dataSet.Tables.Add(dataTable);
                    }
                }
                dataSet.AcceptChanges();
                var jsonResults = DataSetToJson(dataSet);
                var appMsg = new MqttApplicationMessage(scantopic, Encoding.UTF8.GetBytes(jsonResults.ToString()), MqttQualityOfServiceLevel.AtLeastOnce, false);
                mqttClient.PublishAsync(appMsg);
                Console.WriteLine(jsonResults);
            }
        }

        public static void read_mdb(MqttClient mqttClient, Dictionary<string, object> json)
        {
        }

        private static string DataSetToJson(DataSet ds)
        {
            var results = new List<object>();
            foreach (var table in ds.Tables.Cast<DataTable>())
            {

                var parentRows = new List<Dictionary<string, object>>();
                foreach (var row in table.Rows.Cast<DataRow>())
                {

                    var childRow = new Dictionary<string, object>();
                    foreach (var column in table.Columns.Cast<DataColumn>())
                    {
                        childRow.Add(column.ColumnName, row[column]);
                    }

                    parentRows.Add(childRow);
                }

                results.Add(new
                {
                    name = table.TableName,
                    items = parentRows
                });
            }

            return JsonConvert.SerializeObject(results, Formatting.Indented);
        }

        /// <summary>
        /// 获取数据库中所有表名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllTableNames()
        {
            List<string> result = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    DataTable tbs = conn.GetSchema("tables");
                    foreach (DataRow dr in tbs.AsEnumerable().Where(x => x["TABLE_TYPE"].ToString() == "TABLE"))
                    {
                        result.Add(dr["TABLE_NAME"].ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 更新数据库中表，实际上可以说是替换,若表不存在将自动创建
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="tableName">表名</param>
        public static void UpdateDBTable(System.Data.DataTable dt, string tableName)
        {
            if (!IsTableExist(tableName))
            {
                CreatDBTable(dt, tableName);
            }
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    string sql = $"delete * from {tableName}";
                    OleDbCommand odc = new OleDbCommand(sql, conn);
                    odc.ExecuteNonQuery();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string add_sql = $"Insert Into {tableName} Values('{dt.Rows[i].ItemArray[0]}'";
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            add_sql += $",'{dt.Rows[i].ItemArray[j]}'";
                        }
                        add_sql += ")";
                        odc.CommandText = add_sql;
                        odc.Connection = conn;
                        odc.ExecuteNonQuery();
                    }
                    odc.Dispose();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("查询值的数目与目标字段中的数目不同"))
                    {
                        DeleteDBTable(tableName);
                        UpdateDBTable(dt, tableName);
                    }
                    else
                    {
                       Console.WriteLine(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 新建数据表
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="tableName">表名</param>
        public static bool CreatDBTable(System.Data.DataTable dt, string tableName)
        {
            if (IsTableExist(tableName))
            {
                Console.WriteLine("表已存在，请检查名称！");
                return false;
            }
            else
            {
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        //构建字段组合
                        string StableColumn = "";
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            Type t = dt.Columns[i].DataType;
                            if (t.Name == "String")
                            {
                                StableColumn += string.Format("{0} varchar", dt.Columns[i].ColumnName);
                            }
                            else if (t.Name == "Int32" || t.Name == "Double")
                            {
                                StableColumn += string.Format("{0} int", dt.Columns[i].ColumnName);
                            }
                            if (i != dt.Columns.Count - 1)
                            {
                                StableColumn += ",";
                            }
                        }
                        string sql = "";
                        if (StableColumn.Contains("ID int"))
                        {
                            StableColumn = StableColumn.Replace("ID int,", "");
                            sql = $"create table {tableName}(ID autoincrement primary key,{StableColumn}";
                        }
                        else
                        {
                            sql = $"create table {tableName}({StableColumn})";
                        }
                        OleDbCommand odc = new OleDbCommand(sql, conn);
                        odc.ExecuteNonQuery();
                        odc.Dispose();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除数据库中表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool DeleteDBTable(string tableName)
        {
            if (IsTableExist(tableName))
            {
                using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        string sql = $"drop table {tableName}";
                        OleDbCommand odc = new OleDbCommand(sql, conn);
                        odc.ExecuteNonQuery();
                        odc.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
            }
            else { return false; }
        }

        /// <summary>
        /// 查询表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsTableExist(string tableName)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    string sql = $"select * from {tableName}";
                    OleDbCommand odc = new OleDbCommand(sql, conn);
                    odc.ExecuteNonQuery();
                    odc.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取数据库中表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable GetDBTable(string tableName)
        {
            if (!IsTableExist(tableName))
            {
                return null;
            }
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    string sql = $"select * from {tableName}";
                    DataTable dt = new DataTable();
                    OleDbDataAdapter oda = new OleDbDataAdapter(sql, conn);
                    oda.Fill(dt);
                    oda.Dispose();
                    return dt;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 从数据库里面获取数据
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <returns>数据列表</returns>
        public static DataTable GetDataTableFromDB(string strSql)
        {
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    if (conn.State == ConnectionState.Closed)
                    {
                        return null;
                    }

                    ole_command.CommandText = strSql;
                    ole_command.Connection = conn;

                    ole_reader = ole_command.ExecuteReader(CommandBehavior.Default);

                    dt = ConvertOleDbReaderToDataTable(ref ole_reader);

                    ole_reader.Close();
                    ole_reader.Dispose();
                }
                catch (System.Exception e)
                {
                    //Console.WriteLine(e.ToString());
                    Console.WriteLine("{0}", e.Message);
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }

                return dt;
            }
        }

        /// <summary>
        /// 转换数据格式
        /// </summary>
        /// <param name="reader">数据源</param>
        /// <returns>数据列表</returns>
        private static DataTable ConvertOleDbReaderToDataTable(ref OleDbDataReader reader)
        {
            DataTable dt_tmp = null;
            DataRow dr = null;
            int data_column_count = 0;
            int i = 0;

            data_column_count = reader.FieldCount;
            dt_tmp = BuildAndInitDataTable(data_column_count);

            if (dt_tmp == null)
            {
                return null;
            }

            while (reader.Read())
            {
                dr = dt_tmp.NewRow();

                for (i = 0; i < data_column_count; ++i)
                {
                    dr[i] = reader[i];
                }

                dt_tmp.Rows.Add(dr);
            }

            return dt_tmp;
        }

        /// <summary>
        /// 创建并初始化数据列表
        /// </summary>
        /// <param name="Field_Count">列的个数</param>
        /// <returns>数据列表</returns>
        private static DataTable BuildAndInitDataTable(int Field_Count)
        {
            DataTable dt_tmp = null;
            DataColumn dc = null;
            int i = 0;

            if (Field_Count <= 0)
            {
                return null;
            }

            dt_tmp = new DataTable();

            for (i = 0; i < Field_Count; ++i)
            {
                dc = new DataColumn(i.ToString());
                dt_tmp.Columns.Add(dc);
            }

            return dt_tmp;
        }

    }
   
}