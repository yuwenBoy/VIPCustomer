using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 数据库表说明:车辆订购表
    /// </summary>
    public class CarPurchase
    {
        /// <summary>
        /// 未解决添加订购又同时添加交车记录时，无法确定订购GUID而临时由客户端生成
        /// </summary>
        public int AddSortNo
        { set; get; }
        public int PKID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int? OrderID { get; set; }

        /// <summary>
        /// 车辆ID
        /// </summary>
        public string CarID { get; set; }


        /// <summary>
        /// 车辆颜色ID
        /// </summary>
        public string CarColorID { get; set; }

        /// <summary>
        /// 希望交车日期
        /// </summary>
        public DateTime? WantSumbitCarDate { get; set; }

        /// <summary>
        /// 车辆用途
        /// </summary>
        public string CarUsing { get; set; }

        /// <summary>
        /// 其它需求
        /// </summary>
        public string OtheRequirements { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public string Users { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remake { get; set; }

        /// <summary>
        /// 需求数量
        /// </summary>
        public int? RequirementNumber { get; set; }

        /// <summary>
        /// 交车数量
        /// </summary>
        public string SumbitCarNumber { get; set; }

        /// <summary>
        /// 是否申请大客户资源
        /// </summary>
        public string IsApplyMaxCustomerResources { get; set; }

        /// <summary>
        /// 传真订单编号
        /// </summary>
        public string FaxOrderNo { get; set; }

        /// <summary>
        /// 交车经销店ID
        /// </summary>
        public string SubmitDealerID { get; set; }

        /// <summary>
        /// 原需求ID
        /// </summary>
        public string OldRequirementID { get; set; }

        /// <summary>
        /// 原交车ID
        /// </summary>
        public string OldSumbitCarID { get; set; }

        /// <summary>
        /// 原始单号
        /// </summary>
        public string OldNo { get; set; }

        /// <summary>
        /// TACT订单协议号
        /// </summary>
        public string RuckSack { get; set; }

        /// <summary>
        /// 窗帘加否
        /// </summary>
        public string WithNoCurtains { get; set; }

        /// <summary>
        /// 铭牌座位数
        /// </summary>
        public string NameplateSeats { get; set; }

        /// <summary>
        /// 桌椅变更
        /// </summary>
        public string TableChang { get; set; }

        /// <summary>
        /// 其他要求
        /// </summary>
        public string Other { get; set; }

        /// <summary>
        /// 期望FTMS发车时间
        /// </summary>
        public DateTime? WantFTMSCarDateTime { get; set; }


        //=========外加字段=================//
        public string CarName { get; set; }

        public string CarModel { get; set; }
        public string Model { get; set; }

        public string SFX { get; set; }
        public string CarColorCode { get; set; }
    }
}
