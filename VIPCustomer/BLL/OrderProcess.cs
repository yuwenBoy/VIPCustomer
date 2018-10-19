using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class OrderProcess
    {
        /// <summary>
        /// 添加一条订单流程(默认直接提交大客户室)
        /// </summary>
        /// <param name="operUser">操作人</param>
        /// <param name="operType">操作类型（利用enum流程名称枚举获取）</param>
        /// <param name="dealerID">经销店ID，只有在提交订单时需要，其他填写Guid.Empty</param>
        /// <param name="ordersID">订单ID</param>
        public static void Add(string operUser, string operType, int dealerID, int ordersID)
        {
            new DAL.OrderProcess().Add(operUser, operType, dealerID, ordersID, true);
        }

        /// <summary>
        /// 添加一条订单流程
        /// </summary>
        /// <param name="operUser">操作人</param>
        /// <param name="operType">操作类型（利用enum流程名称枚举获取）</param>
        /// <param name="dealerID">经销店ID，只有在提交订单时需要，其他填写Guid.Empty</param>
        /// <param name="ordersID">订单ID</param>
        public static void Add(string operUser, string operType, int dealerID, int ordersID, bool directSubmit)
        {
            new DAL.OrderProcess().Add(operUser, operType, dealerID, ordersID, directSubmit);
        }

        #region 查看订单进度流程
        /// <summary>
        /// 查看订单进度流程
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public List<Common.Entity.OrderProcess> GetOrderProcessByOrderID(int orderId)
        {
            return new DAL.OrderProcess().GetOrderProcessByOrderID(orderId);
        }
        #endregion
    }
}
