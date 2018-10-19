using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 数据库表说明:客户信息表
    /// </summary>
    public class Customer
    {
        public int PKID { get; set; }

        /// <summary>
        /// 经销店ID
        /// </summary>
        public int DealerID { get; set; }

        /// <summary>
        /// 企业代码
        /// </summary>
        public string EnterpriseCode { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// 客户性质ID
        /// </summary>
        public int CustomerNatureID { get; set; }

        /// <summary>
        /// 客户性质2ID
        /// </summary>
        public int CustomerNature2ID { get; set; }

        /// <summary>
        /// 旧客户性质ID
        /// </summary>
        public int OldCustomerNatureID { get; set; }

        /// <summary>
        /// 旧客户性质ID
        /// </summary>
        public int OldCustomerNature2ID { get; set; }

        /// <summary>
        /// 客户分类ID
        /// </summary>
        public int CustomerTypeID { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Eamil { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// 主管部门
        /// </summary>
        public string CompetentDepartment { get; set; }

        /// <summary>
        /// 执行部门
        /// </summary>
        public string ExecutiveDepartment { get; set; }

        /// <summary>
        /// 使用部门
        /// </summary>
        public string UseDepartment { get; set; }

        /// <summary>
        /// 主要业务
        /// </summary>
        public string MainBusiness { get; set; }


        /// <summary>
        /// 行业地位
        /// </summary>
        public string IndustryStatus { get; set; }


        /// <summary>
        /// 客户概况
        /// </summary>
        public string CustomerProfiles { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 客户原ID
        /// </summary>
        public string CustomerPrimaryID { get; set; }

        /// <summary>
        /// 客户性质名称
        /// </summary>
        public string CustomerNatureName { get; set; }

        /// <summary>
        /// 客户性质名称2
        /// </summary>
        public string CustomerNatureName2 { get; set; }
        /// <summary>
        ///经销店名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 扩展字段 
        /// </summary>
        public List<Common.Entity.CustomerContact> contact_fields
        {
            get; set;
        }
    }
}
