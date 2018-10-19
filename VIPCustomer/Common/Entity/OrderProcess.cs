using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 订单流程表
    /// </summary>
    public class OrderProcess
    {
        /// <summary>
        /// 订单流程ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        ///  操作名称
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        ///  操作人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        ///  操作时间
        /// </summary>
        public DateTime OperationDate { get; set; }

        /// <summary>
        ///  下步操作候选人
        /// </summary>
        public string NextOperator { get; set; }
    }
}
