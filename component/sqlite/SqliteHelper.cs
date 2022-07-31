namespace dgiot_dtu.component.sqlite
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dgiot_dtu;

    class SqliteHelper
    {
        //从配置文件中读取连接字符串
        private static string connStr = @"data source=D:\dgiot\data.db;Journal Mode=WAL";
        //执行命令的方法，insert、update、delete
        //public static int ExecuteNonQuery(string sql, params SQLiteParameter[] ps)
        public static string values = "";
        private static string head = "";
        public static long i = 0;
        public static void ExecuteNonQuery(string sql)
        {
            //创建连接对象
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                //创建命令对象
                string v = sql + ";";
                SQLiteCommand comd = new SQLiteCommand(v, conn);
                //添加参数
                //comd.Parameters.AddRange(ps);
                //打开连接
                conn.Open();
                //执行命令，并返回受影响的行数
                comd.ExecuteNonQuery();
                conn.Close();
                //return A; 
            }
        }

        //获取首行首列的方法
        public static object ExecuteScalar(string sql, params SQLiteParameter[] ps)
        {
            //创建连接对象
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                //创建命令对象
                string v = sql + ";";
                SQLiteCommand comd = new SQLiteCommand(v, conn);
                //添加参数
                comd.Parameters.AddRange(ps);
                //打开连接
                conn.Open();
                //执行命令，获取查询结果中的首行首列的值
                return comd.ExecuteScalar();
            }
        }

        //获取结果集select
        //public static DataTable GetDataTable(string sql, params SQLiteParameter[] ps)
        public static DataTable GetDataTable(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                //构造适配器对象
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                //构造数据表，用于接收查询结果
                DataTable dt = new DataTable();
                //添加参数
                //adapter.SelectCommand.Parameters.AddRange(ps);
                //执行结果
                adapter.Fill(dt);
                //返回结果集
                return dt;
            }
        }

        public static void Init(string head)
        {
            string create = "create table if not exists cache(id BIGINT NOT NULL , name TEXT  NOT NULL)";
            SqliteHelper.ExecuteNonQuery(create);
            // SqliteHelper.head = "INSERT INTO cache (id,name)"
            SqliteHelper.head = "INSERT INTO cache " + head;
        } 

        public static void Insert(string value)
        {
            long timestamp = DgiotHelper.Ms();
            SqliteHelper.values = SqliteHelper.values + ", (" + timestamp.ToString() +",\'" + value.ToString() + "\')"; 
           
             //list.ToArray().Length;
            SqliteHelper.ExecuteNonQuery(SqliteHelper.values);
            SqliteHelper.values = "";

        }
        public static void Test(string sqls)
        {
            using (SQLiteConnection con = new SQLiteConnection(connStr))
            {

                SQLiteCommand cmd = new SQLiteCommand(con);
                //添加参数
                //comd.Parameters.AddRange(ps);
                //打开连接
                
                con.Open();

                //cmd.CommandText = "PRAGMA synchronous = OFF";
                //cmd.ExecuteNonQuery();

                cmd.CommandText = "PRAGMA synchronous = OFF";
                cmd.ExecuteNonQuery();
                // 使用事务测试
                IDbTransaction trans = con.BeginTransaction();
                foreach (var sql in sqls)
                {
                    long timestamp = DgiotHelper.Ms();
                    string Insert = "INSERT INTO cache (id,name) VALUES (" + timestamp.ToString() + ",\'" + sql.ToString() + "\');";
                    LogHelper.Log("Insert: " + Insert);
                    cmd.CommandText = Insert ;
                    cmd.ExecuteNonQueryAsync();
                }
                trans.Commit();
                //执行命令，并返回受影响的行数
                con.Close();
                //return A; 
                //返回结果集

            }
        }




    }
}