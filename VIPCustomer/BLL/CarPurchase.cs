using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace BLL
{
    public class CarPurchase
    {
        #region 根据订单编号获取车辆需求信息
        /// <summary>
        /// 根据订单编号获取车辆需求信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>返回list集合</returns>
        public List<Common.Entity.CarPurchase> GetList(string orderId)
        {
            string sql = string.Format($@"select  CI.Name CarName,CI.CarModel CarModel,CI.Model,CI.SFX,CC.Code as CarColorCode,
                                    CP.* from CarPurchase CP 
                                    left join CarInfo CI on CP.CarID=CI.PKID
                                    left join CarColor CC on CP.CarColorID=CC.PKID 
                                    where CP.OrderID={orderId}");

            List<Common.Entity.CarPurchase> list = new List<Common.Entity.CarPurchase>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.CarPurchase>(sql).ToList();
            };
            return list;
        }

        /// <summary>
        /// 根据订单编号获取车辆需求信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>返回list集合</returns>
        public List<Common.Entity.CarPurchase> GetList(int orderId)
        {
            string sql = string.Format($@"select  CI.Name CarName,CI.CarModel CarModel,CI.Model,CI.SFX,CC.Code as CarColorCode,
                                    CP.* from CarPurchase CP 
                                    left join CarInfo CI on CP.CarID=CI.PKID
                                    left join CarColor CC on CP.CarColorID=CC.PKID 
                                    where CP.OrderID={orderId}");

            List<Common.Entity.CarPurchase> list = new List<Common.Entity.CarPurchase>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.CarPurchase>(sql).ToList();
            };
            return list;
        }
        #endregion

        #region 获取车辆需求实体对象
        /// <summary>
        /// 获取车辆需求实体对象
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns>返回list集合</returns>
        public List<Common.Entity.CarPurchase> GetCarDemandByPKID(int pkid)
        {
            return new DAL.CarPurchase().GetCarDemandByPKID(pkid);
        }
        #endregion

        public List<Common.Entity.CarPurchase> GetOriginalList(string code)
        {
            return new DAL.CarPurchase().GetOriginalList(code);
        }
    }
}
