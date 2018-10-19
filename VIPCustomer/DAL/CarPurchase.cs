using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace DAL
{
    public class CarPurchase
    {
        #region 获取车辆需求实体对象
        /// <summary>
        /// 获取车辆需求实体对象
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns>返回list集合</returns>
        public List<Common.Entity.CarPurchase> GetCarDemandByPKID(int pkid)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"select  CI.Name CarName,CI.CarModel CarModel,CI.Model,CI.SFX,CC.Code as CarColorCode,
                                    CP.* from CarPurchase CP 
                                    left join CarInfo CI on CP.CarID=CI.PKID
                                    left join CarColor CC on CP.CarColorID=CC.PKID
								    where CP.PKID={pkid}");
                var list = conn.Query<Common.Entity.CarPurchase>(sql).ToList();
                return list;
            };
        }
        #endregion

        #region 获取车辆需求实体对象
        /// <summary>
        /// 获取车辆需求实体对象
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns>返回list集合</returns>
        public List<Common.Entity.CarPurchase> GetModel(int pkid)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"select * from CarPurchase  where OrderID={pkid}");
                var list = conn.Query<Common.Entity.CarPurchase>(sql).ToList();
                return list;
            };
        }
        #endregion

        public List<Common.Entity.CarPurchase> GetOriginalList(string code)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"select  CI.Name CarName,CI.CarModel CarModel,CI.Model,CI.SFX,CC.Code as CarColorCode,
                                    CP.* from CarPurchase CP 
                                    left join CarInfo CI on CP.CarID=CI.PKID
                                    left join CarColor CC on CP.CarColorID=CC.PKID
									 and Exists(Select 1 from [Order] where PKID=CP.OrderID and Code like '%"+code+"%')");
                var list = conn.Query<Common.Entity.CarPurchase>(sql).ToList();
                return list;
            };
        }
    }
}
