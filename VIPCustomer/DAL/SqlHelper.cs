using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{ /// <summary>
  /// SQL Server数据库访问类
  /// </summary>
    public abstract class SqlHelper
    {
        // 读取配置文件里的数据库连接字符串
        public static readonly string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        // 空构造函数
        public SqlHelper() { }

        /// <summary>
        /// Dapper 连接数据库字符串
        /// </summary>
        /// <returns></returns>
        public static SqlConnection sqlconn()
        {
            var conntion = new SqlConnection(connStr);
            conntion.Open();
            return conntion;
        }

        // 用Hashtable存放缓存的参数
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        #region 执行增删改 【常用】
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, commandType, commandText, param);
                int result = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();  // 清空参数
                return result;
            }
        }
        #endregion

        public static int ExecuteReturnID(string connectionString, CommandType commandType, string commandText, params SqlParameter[] param)
        {
            int result = -1;
            try
            {
                SqlCommand cmd = new SqlCommand();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    PrepareCommand(cmd, conn, null, commandType, commandText, param);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = Convert.ToInt32(reader[0].ToString().Trim());
                        }
                    }
                    conn.Close();
                }
            }
            catch
            { }
            return result;

        }

        #region 执行增删改（对现有的数据库连接）【不常用】
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText, param);
            int result = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return result;
        }
        #endregion

        #region 执行多条sql语句
        /// <summary>
        /// 执行多条sql语句（List泛型集合）【事务】(无参数)
        /// </summary>
        /// <param name="connecionString"></param>
        /// <param name="listSql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connecionString, List<string> listSql)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connecionString);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            PrepareCommand(cmd, conn, trans, CommandType.Text, null, null);
            try
            {
                int count = 0;
                for (int i = 0; i < listSql.Count; i++)
                {
                    string strSql = listSql[i];
                    if (strSql.Trim().Length > 1)
                    {
                        cmd.CommandText = strSql;
                        count += cmd.ExecuteNonQuery();
                    }
                }
                trans.Commit();
                cmd.Parameters.Clear();
                return count;
            }
            catch (Exception)
            {
                trans.Rollback();
                cmd.Parameters.Clear();
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 执行多条sql语句（Hashtable）【事务】(带一组参数，一个参数也得封装成数组)
        /// <summary>
        /// 执行多条sql语句（Hashtable）【事务】(带一组参数，一个参数也得封装成数组)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="sqlStringList">Hashtable表，键值对形式</param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(string connectionString, Hashtable sqlStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        foreach (DictionaryEntry item in sqlStringList)
                        {
                            string cmdText = item.Key.ToString();// 要执行的sql语句
                            SqlParameter[] cmdParam = (SqlParameter[])item.Value;// sql语句对应的参数
                            PrepareCommand(cmd, conn, trans, CommandType.Text, cmdText, cmdParam);
                            int result = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        if (sqlStringList.Count > 0)
                            trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        return false;
                        throw;
                    }
                }
            }
        }
        #endregion

        #region 返回DataReader对象
        /// <summary>
        /// 返回DataReader对象
        /// </summary>
        /// <param name="connectionString">数据库连接对象</param>
        /// <param name="commandType">执行类型</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="pararm">参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string cmdText, params SqlParameter[] pararm)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, commandType, cmdText, pararm);
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch (Exception)
            {
                conn.Close();
                throw;
            }
        }
        #endregion

        #region 返回第一行第一列信息(可能是字符串 所以返回类型是object) 【常用】
        /// <summary>
        /// 返回第一行第一列信息(可能是字符串 所以返回类型是object) 【常用】
        /// </summary>
        /// <param name="conncetionString">数据库连接字符串</param>
        /// <param name="commandType">执行类型</param>
        /// <param name="commandText">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string conncetionString, CommandType commandType, string commandText, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(conncetionString))
            {
                PrepareCommand(cmd, connection, null, commandType, commandText, param);
                object result = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return result;
            }
        }
        #endregion

        #region 返回第一行第一列信息(针对现有的数据库连接)【不常用】
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText, param);
            object result = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return result;
        }
        #endregion

        // 返回DataTable
        public static DataTable GetDateTable(string connectionString, CommandType commandType, string commandText, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, commandType, commandText, param);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }


        // 返回DataSet
        public static DataSet GetDataSet(string conncetionString, CommandType commandType, string commandText, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(conncetionString))
            {
                PrepareCommand(cmd, conn, null, commandType, commandText, param);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }
        /// <summary> 
        /// 执行指定事务的命令,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary> 
        /// 执行指定事务的命令,指定参数,返回DataSet. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 预处理 
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建 DataAdapter & DataSet 
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }
        /// <summary> 
        /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数 
        /// </summary> 
        /// <param name="command">要处理的SqlCommand</param> 
        /// <param name="connection">数据库连接</param> 
        /// <param name="transaction">一个有效的事务或者是null值</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
        /// <param name="commandText">存储过程名或都T-SQL命令文本</param> 
        /// <param name="commandParameters">和命令相关联的SqlParameter参数数组,如果没有参数为'null'</param> 
        /// <param name="mustCloseConnection"><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param> 
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it 
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // 给命令分配一个数据库连接. 
            command.Connection = connection;

            // 设置命令文本(存储过程名或SQL语句) 
            command.CommandText = commandText;

            // 分配事务 
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // 设置命令类型. 
            command.CommandType = commandType;

            // 分配命令参数 
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        /// <summary> 
        /// 将SqlParameter参数数组(参数值)分配给SqlCommand命令. 
        /// 这个方法将给任何一个参数分配DBNull.Value; 
        /// 该操作将阻止默认值的使用. 
        /// </summary> 
        /// <param name="command">命令名</param> 
        /// <param name="commandParameters">SqlParameters数组</param> 
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // 检查未分配值的输出参数,将其分配以DBNull.Value. 
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// 获取泛型集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStr">要查询的T-SQL</param>
        /// <returns></returns>
        /// where T: new () 这里表示传入的类型必须有空的构造方法，不然会报错
        public static IList<T> GetList<T>(string connStr, string sqlStr, CommandType commandType, params SqlParameter[] param) where T : new()
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                PrepareCommand(cmd, conn, null, commandType, sqlStr, param);
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    return DataSetToList<T>(ds, 0);
                }
            }
        }
        /// <summary>
        /// DataSetToList
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dataSet">数据源</param>
        /// <param name="tableIndex">需要转换表的索引</param>
        /// <returns>泛型集合</returns>
        public static IList<T> DataSetToList<T>(DataSet dataSet, int tableIndex) where T : new()
        {
            if (dataSet == null || dataSet.Tables.Count <= 0 || tableIndex < 0)
            {
                return null;
            }
            DataTable dt = dataSet.Tables[tableIndex];
            IList<T> list = new List<T>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // 创建泛型对象
                T _t = Activator.CreateInstance<T>();

                // 获取对象所有属性
                PropertyInfo[] propertyInfo = _t.GetType().GetProperties();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    foreach (PropertyInfo info in propertyInfo)
                    {
                        // 属性名称和列名相同时赋值
                        if (dt.Columns[j].ColumnName.ToUpper().Equals(info.Name.ToUpper()))
                        {
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                info.SetValue(_t, dt.Rows[i][j], null);
                            }
                            else
                            {
                                info.SetValue(_t, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(_t);
            }
            return list;
        }
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParmeters)
        {
            paramCache[cacheKey] = commandParmeters;
        }

        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])paramCache[cacheKey];
            if (cachedParms == null)
                return null;
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 准备一个待执行的SqlCommand
        /// </summary>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType commandType, string commandText, params SqlParameter[] paras)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Close();
                    conn.Open();
                }
                cmd.Connection = conn;
                if (commandText != null)
                    cmd.CommandText = commandText;
                cmd.CommandType = commandType;     //这里设置执行的是T-Sql语句还是存储过程

                if (trans != null)
                    cmd.Transaction = trans;

                if (paras != null && paras.Length > 0)
                {
                    //cmd.Parameters.AddRange(paras);
                    for (int i = 0; i < paras.Length; i++)
                    {
                        if (paras[i].Value == null || paras[i].Value.ToString() == "")
                            paras[i].Value = DBNull.Value;   //插入或修改时，如果有参数是空字符串，那么以NULL的形式插入数据库
                        cmd.Parameters.Add(paras[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 2012-2-21新增重载，执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="connection">SqlConnection对象</param>
        /// <param name="trans">SqlTransaction事件</param>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(SqlConnection connection, SqlTransaction trans, string SQLString)
        {
            using (SqlCommand cmd = new SqlCommand(SQLString, connection))
            {
                try
                {
                    cmd.Connection = connection;
                    cmd.Transaction = trans;
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    //trans.Rollback();
                    throw e;
                }
            }
        }
    }
}
