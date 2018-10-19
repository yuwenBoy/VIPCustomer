using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 时间：2018年4月11日11:03:28
/// 作者：赵鉴
/// 描述：交车记录业务逻辑层
/// </summary>
namespace BLL
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
            return new DAL.CarRecord().GetList(orderId);
        }
    }
}
