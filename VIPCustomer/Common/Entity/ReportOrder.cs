using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 数据库表说明:封装一个订单的全部信息
    /// </summary>
    public class ReportOrder
    {
        public int PKID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 购买方式
        /// </summary>
        public string BuyWay { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 提交日期
        /// </summary>
        public DateTime? SubmitTime { get; set; }

        /// <summary>
        /// 经销店ID
        /// </summary>
        public int DealerID { get; set; }

        /// <summary>
        /// 经销店名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderState { get; set; }

        /// <summary>
        /// 提交类型
        /// </summary>
        public string SubmitType { get; set; }

        /// <summary>
        /// 采购类型
        /// </summary>
        public string PurchaseType { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户地址
        /// </summary>
        public string CustomerAddress { get; set; }

        /// <summary>
        /// 客户邮编
        /// </summary>
        public string CustomerZipCode { get; set; }

        /// <summary>
        /// 主管部门
        /// </summary>
        public string CustomerChargeDept { get; set; }

        /// <summary>
        /// 执行部门
        /// </summary>
        public string CustomerExecutiveDept { get; set; }

        /// <summary>
        /// 使用部门
        /// </summary>
        public string CustomerUseDept { get; set; }

        /// <summary>
        /// 主要业务
        /// </summary>
        public string CustomerMainBusiness { get; set; }

        /// <summary>
        /// 客户概况
        /// </summary>
        public string CustomerProfiles { get; set; }

        /// <summary>
        /// 记录名称
        /// </summary>
        public string RecordName { get; set; }

        /// <summary>
        /// 地担用户
        /// </summary>
        public string BearUser { get; set; }

        /// <summary>
        /// 制表人
        /// </summary>
        public string Watchmaker { get; set; }

        /// <summary>
        /// 车辆用途
        /// </summary>
        public string CarUse { get; set; }


        /// <summary>
        /// 是否需要异地交车
        /// </summary>
        public string DifferentPlace { get; set; }

        /// <summary>
        /// 发票与客户不一致
        /// </summary>
        public int InvoiceDiffer { get; set; }

        /// <summary>
        /// 客户建议
        /// </summary>
        public string CustomerSuggestion { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string BaseRemark { get; set; }

        /// <summary>
        /// 地担回复人
        /// </summary>
        public string Replyer1 { get; set; }

        /// <summary>
        /// 地担回复意见
        /// </summary>
        public string ReComment1 { get; set; }

        /// <summary>
        /// 地担回复日期
        /// </summary>
        public DateTime? ReDate1 { get; set; }


        /// <summary>
        /// 地担回复备注
        /// </summary>
        public string ReRemark1 { get; set; }


        /// <summary>
        /// 大区经理回复人
        /// </summary>
        public string Replyer2 { get; set; }


        /// <summary>
        /// 大区经理回复意见
        /// </summary>
        public string ReComment2 { get; set; }


        /// <summary>
        /// 大区经理回复日期
        /// </summary>
        public DateTime? ReDate2 { get; set; }

        /// <summary>
        /// 大区经理回复备注
        /// </summary>
        public string ReRemark2 { get; set; }

        /// <summary>
        ///FTMS回复人
        /// </summary>
        public string Replyer3 { get; set; }

        /// <summary>
        ///FTMS回复意见
        /// </summary>
        public string ReComment3 { get; set; }

        /// <summary>
        ///FTMS回复时间
        /// </summary>
        public string ReDate3 { get; set; }

        /// <summary>
        ///FTMS回复备注
        /// </summary>
        public string ReRemark3 { get; set; }

        /// <summary>
        ///FTMS返款审核人
        /// </summary>
        public string FTMSBackAuditor { get; set; }

        /// <summary>
        ///FTMS返款意见
        /// </summary>
        public string FTMSBackSuggestion { get; set; }

        /// <summary>
        ///FTMS返款审核时间
        /// </summary>
        public DateTime? FTMSBackToExamineDate { get; set; }

        /// <summary>
        ///FTMS返款备注
        /// </summary>
        public string FTMSBackRemark { get; set; }

        /// <summary>
        ///是否申请大客户资源
        /// </summary>
        public string IsApplyMaxCustomerResources { get; set; }

        /// <summary>
        ///审核状态
        /// </summary>
        public int ToExamineState { get; set; }
        /// <summary>
        ///是否可编辑
        /// </summary>
        public int IsEdit { get; set; }

        /// <summary>
        ///订单类别
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        ///原单ID
        /// </summary>
        public string YuanDanID { get; set; }

        /// <summary>
        ///非本地上牌报备
        /// </summary>
        public string OffAddressOnCardReport { get; set; }

        /// <summary>
        ///发票与客户不一致
        /// </summary>
        public string InvoiceAndCustomerAtypism { get; set; }

        /// <summary>
        ///发票客户信息
        /// </summary>
        public string InvoiceCustomerInfo { get; set; }

        /// <summary>
        /// 客户联系人
        /// </summary>
        public List<Common.Entity.OrderCustomerContact> CustomerContact { get; set; }

        /// <summary>
        /// 车辆订购情况
        /// </summary>
        public List<Common.Entity.CarPurchase> CarPurchase { get; set; }

        /// <summary>
        /// 经销店联系人
        /// </summary>
        public List<Common.Entity.DealerContact> DealerContact { get; set; }

        /// <summary>
        /// 客户性质1
        /// </summary>
        public string Customer_Type { get; set; }

        /// <summary>
        /// 客户性质2
        /// </summary>
        public string Customer_Type2 { get; set; }

        /// <summary>
        /// 客户备注
        /// </summary>
        public string Customer_Memo { get; set; }

        /// <summary>
        /// 客户机构代码
        /// </summary>
        public string CustomerNo { get; set; }

        public string FN_Email { get; set; }

        public string FN_FieldLicence { get; set; }
        public string FN_InvoiceDiffer { get; set; }
        public string FN_InvoiceName { get; set; }
    }
}
