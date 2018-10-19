using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 交车记录表
    /// </summary>
    public class CarRecord
    {
        public int PKID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int? OrderID { get; set; }

        /// <summary>
        /// 车辆订购ID
        /// </summary>
        public int? CarPurchaseID { get; set; }

        /// <summary>
        /// 原始单号
        /// </summary>
        public string OriginalNo { get; set; }

        /// <summary>
        /// 车辆ID
        /// </summary>
        public int? CarID { get; set; }

        /// <summary>
        /// 车辆颜色ID
        /// </summary>
        public int? CarColorID { get; set; }

        /// <summary>
        /// 销售日期
        /// </summary>
        public DateTime? DateSale { get; set; }

        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNumber { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        public string FrameNumber { get; set; }

        /// <summary>
        /// 单车销售价
        /// </summary>
        public decimal CarSalePrice { get; set; }
        /// <summary>
        /// 单车优惠幅度
        /// </summary>
        public decimal CarPreferentialMargin { get; set; }
        /// <summary>
        /// 单车返款额度
        /// </summary>
        public decimal CarBackMargin { get; set; }

        /// <summary>
        /// 交车备注
        /// </summary>
        public string LeaveRemarks { get; set; }
        /// <summary>
        /// 制表日期
        /// </summary>
        public DateTime? DateTabulation { get; set; }
        /// <summary>
        /// 返款金额1
        /// </summary>
        public decimal BackMoney1 { get; set; }
        /// <summary>
        /// 返款日期1
        /// </summary>
        public DateTime? BackMoneyDate1 { get; set; }

        /// <summary>
        /// 返款人1
        /// </summary>
        public string Regenerator1 { get; set; }

        /// <summary>
        /// 返款金额2
        /// </summary>
        public decimal BackMoney2 { get; set; }
        /// <summary>
        /// 返款日期2
        /// </summary>
        public DateTime? BackMoneyDate2 { get; set; }

        /// <summary>
        /// 返款人2
        /// </summary>
        public string Regenerator2 { get; set; }

        /// <summary>
        /// 计算方式
        /// </summary>
        public string CalculationMethod { get; set; }

        /// <summary>
        /// 返款标示
        /// </summary>
        public string BackMark { get; set; }

        /// <summary>
        /// 返款标示2
        /// </summary>
        public string BackMark2 { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public string AuditStatus { get; set; }
    }
}
