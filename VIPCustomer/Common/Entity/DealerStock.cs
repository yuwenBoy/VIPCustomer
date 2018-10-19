using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    [Serializable]
    public class DealerStock
    {
        public int PKID { get; set; }
        /// <summary>
        /// 经销店ID
        /// </summary>
        public int DealerID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 车辆名称
        /// </summary>
        public string CarName { get; set; }
        /// <summary>
        /// 库存月份
        /// </summary>
        public DateTime? StockMonth { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int StockCount { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public int IsEnabled { get; set; }

        public string EnterpriseCode { get; set; }

        public string Name { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
    }
}
