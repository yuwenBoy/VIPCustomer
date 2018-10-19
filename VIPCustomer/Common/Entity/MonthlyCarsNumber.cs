using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    [Serializable]
    public class MonthlyCarsNumber
    {
        public int PKID { get; set; }
        /// <summary>
        /// 车辆ID
        /// </summary>
        public int CarID { get; set; }

        /// <summary>
        /// 车辆颜色ID
        /// </summary>
        public int CarColorID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int ResidualQuantity { get; set; }


        public string Code { get; set; }
        public string Name { get; set; }
        public string SFX { get; set; }
    }
}
