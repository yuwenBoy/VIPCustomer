using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 时间：2018年4月11日11:08:17
/// 作者：zhao.jian
/// 描述：交车记录数据访问层
/// </summary>
namespace DAL
{
    public class CarRecord
    {
        /// <summary>
        /// 获取审核信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public List<Common.Entity.CarRecord> GetList(string orderId)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"select * from CarRecord where OrderID=@OrderID");
                DynamicParameters Parameters = new DynamicParameters();
                Parameters.Add("OrderID", orderId);
                List<Common.Entity.CarRecord> list = conn.Query<Common.Entity.CarRecord>(sql, Parameters).ToList();
                return list;
            }
        }
    }
}
