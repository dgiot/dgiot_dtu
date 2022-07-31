//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SQLite;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace dgiot_dtu.component.sqlite
//{
//    class ManagerInfoDal
//    {
//        private int MId;
//        private string MName;
//        private string MPwd;
//        private int MType;

//        /// <summary>
//        /// select  查询 获取结果集
//        /// </summary>
//        /// <returns></returns>
//        public List<ManagerInfoDal> GetList()
//        {
//            //构造要查询的sql语句
//            string sql = "select * from ManagerInfo";
//            //使用helper进行查询，得到结果
//            DataTable dt = SqliteHelper.GetDataTable(sql);
//            //将dt中的数据转存到list中
//            List<ManagerInfoDal> list = new List<ManagerInfoDal>();
//            foreach (DataRow row in dt.Rows)
//            {
//                list.Add(new ManagerInfoDal()
//                {
//                    MId = Convert.ToInt32(row["mid"]),
//                    MName = row["mname"].ToString(),
//                    MPwd = row["mpwd"].ToString(),
//                    MType = Convert.ToInt32(row["mtype"])
//                });
//            }
//            //返回集合类型
//            return list;
//        }

//        /// <summary>
//        /// 插入数据
//        /// </summary>
//        /// <param name="mi">ManagerInfo类型对象</param>
//        /// <returns></returns>
//        public int Insert(ManagerInfoDal mi)
//        {
//            //构造insert语句
//            string sql = "insert into ManagerInfo(mname, mpwd,mtype) values(@name, @pwd, @type)";
//            //构造sql语句参数
//            SQLiteParameter[] ps = { //使用数组初始化器
//                                    new SQLiteParameter("@name", mi.MName),
//                                    //将密码进行md5加密
//                                    new SQLiteParameter("@pwd", mi.MPwd),
//                                    new SQLiteParameter("@type", mi.MType)
//                                   };
//            //执行插入操作
//            return SqliteHelper.ExecuteNonQuery(sql);
//        }

//        /// <summary>
//        /// 修改管理员，特别注意：密码
//        /// </summary>
//        /// <param name="mi"></param>
//        /// <returns></returns>
//        public int Update(ManagerInfoDal mi)
//        {
//            //为什么要进行密码的判断：
//            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

//            //定义参数集合，可以动态添加元素
//            List<SQLiteParameter> listPs = new List<SQLiteParameter>();
//            //构造update的sql语句
//            string sql = "update ManagerInfo set mname=@name";
//            listPs.Add(new SQLiteParameter("@name", mi.MName));
//            //判断是否修改密码
//            if (!mi.MPwd.Equals("这是原来的密码吗"))
//            {
//                sql += ",mpwd=@pwd";
//                listPs.Add(new SQLiteParameter("@pwd", mi.MPwd));
//            }
//            //继续拼接语句
//            sql += ",mtype=@type where mid=@id";
//            listPs.Add(new SQLiteParameter("@type", mi.MType));
//            listPs.Add(new SQLiteParameter("@id", mi.MId));

//            //执行语句并返回结果
//            return SqliteHelper.ExecuteNonQuery(sql);
//        }

//        /// <summary>
//        /// 根据编号删除管理员
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public int Delete(int id)
//        {
//            //构造删除的sql语句
//            string sql = "delete from ManagerInfo where mid=@id";
//            //根据语句构造参数
//            SQLiteParameter p = new SQLiteParameter("@id", id);
//            //执行操作
//            return SqliteHelper.ExecuteNonQuery(sql);
//        }
//    }
//}