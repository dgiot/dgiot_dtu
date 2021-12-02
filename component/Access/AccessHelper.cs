// <copyright file="AccessHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Odbc;
    using System.Data.OleDb;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    internal class AccessHelper
    {
        private AccessHelper()
        {
        }

        private static OleDbCommand oleCommand = null;
        private static OleDbDataReader oleReader = null;
        private static DataTable dt = null;

        private static AccessHelper instance = null;
        private static string mdbFile = "test.mdb";
        private static string dbq = Path.Combine(Environment.CurrentDirectory, mdbFile);
        private static string security = "TRUE";
        private static string uid = "Admin";
        private static string pwd = "123456";

        private static string scantopic = "thing/mdb/";

        // 定义连接字符串
        private static string odbcconnectionString =
            "Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + dbq +
            "; Uid=" + uid + "; Pwd=" + pwd + ";";

        private static string connectionString =
             @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + dbq + ";" +
               "Persist Security Info = False; Jet OLEDB:Database Password = " + pwd + ";";

        private static bool bIsRunning = false;
        private static bool bIsCheck = false;

        public static AccessHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new AccessHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, bool bIsRunning)
        {
            Config(config);
            AccessHelper.bIsRunning = bIsRunning;
        }

        public static void Stop()
        {
            AccessHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
            if (config["AccessIsCheck"] != null)
            {
               bIsCheck = DgiotHelper.StrTobool(config["AccessIsCheck"].Value);
            }
        }

        public static void Check(bool isCheck)
        {
            bIsCheck = isCheck;
        }

        public static void Do_mdb(string topic, Dictionary<string, object> json, string clientid)
        {
            Regex r_submdb = new Regex(topic); // 定义一个Regex对象实例
            Match m_submdb = r_submdb.Match(topic); // 在字符串中匹配
            if (!m_submdb.Success)
            {
                return;
            }

            string cmdType = "read";
            scantopic = "thing/mdb/" + clientid + "post";
            if (json.ContainsKey("cmdtype"))
            {
                try
                {
                    cmdType = (string)json["cmdtype"];
                    switch (cmdType)
                    {
                        case "scan":
                            Scan_mdb(json);
                            break;
                        case "read":
                            Read_mdb(json);
                            break;
                        case "write":
                            break;
                        default:
                            Read_mdb(json);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }
        }

        public static void Scan_mdb(Dictionary<string, object> json)
        {
            if (json.ContainsKey("dbq"))
            {
                try
                {
                    dbq = (string)json["dbq"];
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
                    security = (string)json["security"];
                    security = security.ToUpper();
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
                    uid = (string)json["uid"];
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
                    pwd = (string)json["pwd"];
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }

            if (security == "TRUE")
            {
                odbcconnectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + dbq + "; Uid=" + uid + "; Pwd=" + pwd + ";";
            }
            else
            {
                odbcconnectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + dbq;
            }

            Console.WriteLine(odbcconnectionString);

            using (var db = new OdbcConnection(odbcconnectionString))
            {
                db.Open();
                var schemaTable = db.GetSchema("Tables");
                var dataSet = new DataSet();
                for (var i = 0; i < schemaTable.Rows.Count; i++)
                {
                    // only source tables
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

                        // Console.WriteLine(tableName + "(" + dataTable.Rows.Count + " rows)");
                        dataSet.Tables.Add(dataTable);
                    }
                }

                dataSet.AcceptChanges();
                var jsonResults = DataSetToJson(dataSet);
                MqttClientHelper.Publish(scantopic, Encoding.UTF8.GetBytes(jsonResults.ToString()));
                LogHelper.Log(jsonResults);
            }
        }

        public static void Read_mdb(Dictionary<string, object> json)
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
        /// <returns> List </returns>
        public static List<string> GetAllTableNames()
        {
            List<string> result = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
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

            using (OleDbConnection conn = new OleDbConnection(connectionString))
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
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();

                        // 构建字段组合
                        string stableColumn = string.Empty;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            Type t = dt.Columns[i].DataType;
                            if (t.Name == "String")
                            {
                                stableColumn += string.Format("{0} varchar", dt.Columns[i].ColumnName);
                            }
                            else if (t.Name == "Int32" || t.Name == "Double")
                            {
                                stableColumn += string.Format("{0} int", dt.Columns[i].ColumnName);
                            }

                            if (i != dt.Columns.Count - 1)
                            {
                                stableColumn += ",";
                            }
                        }

                        string sql = string.Empty;
                        if (stableColumn.Contains("ID int"))
                        {
                            stableColumn = stableColumn.Replace("ID int,", string.Empty);
                            sql = $"create table {tableName}(ID autoincrement primary key,{stableColumn}";
                        }
                        else
                        {
                            sql = $"create table {tableName}({stableColumn})";
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
                using (OleDbConnection conn = new OleDbConnection(connectionString))
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
            else
            {
                return false;
            }
        }

        public static bool IsTableExist(string tableName)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
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

        public static DataTable GetDBTable(string tableName)
        {
            if (!IsTableExist(tableName))
            {
                return null;
            }

            using (OleDbConnection conn = new OleDbConnection(connectionString))
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

        public static DataTable GetDataTableFromDB(string strSql)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    if (conn.State == ConnectionState.Closed)
                    {
                        return null;
                    }

                    oleCommand.CommandText = strSql;
                    oleCommand.Connection = conn;

                    oleReader = oleCommand.ExecuteReader(CommandBehavior.Default);

                    dt = ConvertOleDbReaderToDataTable(ref oleReader);

                    oleReader.Close();
                    oleReader.Dispose();
                }
                catch (System.Exception e)
                {
                    // Console.WriteLine(e.ToString());
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
        /// <param name="field_Count">列的个数</param>
        /// <returns>数据列表</returns>
        private static DataTable BuildAndInitDataTable(int field_Count)
        {
            DataTable dt_tmp = null;
            DataColumn dc = null;
            int i = 0;

            if (field_Count <= 0)
            {
                return null;
            }

            dt_tmp = new DataTable();

            for (i = 0; i < field_Count; ++i)
            {
                dc = new DataColumn(i.ToString());
                dt_tmp.Columns.Add(dc);
            }

            return dt_tmp;
        }
    }
}