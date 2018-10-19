using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DealerStock
    {
        #region 填充车辆名称列表
        /// <summary>
        /// 填充车辆名称列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCarName()
        {
            string sql = string.Format(@" select  CarName from BrandDictionary order by CarName");
            try
            {
                return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// 是否存在库存记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists(Common.Entity.DealerStock model)
        {
            string sql = string.Format("select count(1) from DealerStock where  DealerID=@DealerId and CustomerID=@CustomerID and CarName=@CarName and StockMonth=@StockMonth and IsEnabled=1");
            DynamicParameters Parameters = new DynamicParameters();
            Parameters.Add("DealerId", model.DealerID);
            Parameters.Add("CustomerID", model.CustomerID);
            Parameters.Add("CarName", model.CarName);
            Parameters.Add("StockMonth", model.StockMonth);
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                bool result = conn.Query<bool>(sql, Parameters).FirstOrDefault();
                if (result)
                {
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(Common.Entity.DealerStock model)
        {
            string sql = string.Format(@"insert into DealerStock(DealerID,CustomerID,CarName,StockMonth,StockCount,IsEnabled) 
                                       Values(@DealerID,@CustomerID,@CarName,@StockMonth,@StockCount,1)");
            DynamicParameters Parameters = new DynamicParameters();
            Parameters.Add("DealerID", model.DealerID);
            Parameters.Add("CustomerID", model.CustomerID);
            Parameters.Add("CarName", model.CarName);
            Parameters.Add("StockMonth", model.StockMonth);
            Parameters.Add("StockCount", model.StockCount);
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                bool result = Convert.ToBoolean(conn.Execute(sql, Parameters));
                if (result)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(Common.Entity.DealerStock model)
        {
            string sql = string.Format(@"Update DealerStock  set CustomerID=@CustomerID,CarName=@CarName,StockMonth=@StockMonth,StockCount=@StockCount 
                                         Where PKID=@PKID");
            DynamicParameters Parameters = new DynamicParameters();
            Parameters.Add("CustomerID", model.CustomerID);
            Parameters.Add("CarName", model.CarName);
            Parameters.Add("StockMonth", model.StockMonth);
            Parameters.Add("StockCount", model.StockCount);
            Parameters.Add("PKID", model.PKID);
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                int result = conn.Execute(sql, Parameters);
                if (result > 0)
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Delete(int stockId)
        {
            string sql = string.Format(@"delete from DealerStock where  PKID=@PKID");
            DynamicParameters Parameters = new DynamicParameters();
            Parameters.Add("PKID", stockId);
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                int result = conn.Execute(sql, Parameters);
                if (result > 0)
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
