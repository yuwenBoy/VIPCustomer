using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    public class Dealer
    {
        public int PKID { get; set; }

        /// <summary>
        /// 经销店编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 经销店名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 曾用名
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// 地担用户ID
        /// </summary>
        public int? BearUserId { get; set; }

        /// <summary>
        /// 大区经理用户ID
        /// </summary>
        public int? RegionManagerUserId { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        ///  电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 传值
        /// </summary>
        public string fax { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 销售电话
        /// </summary>
        public string SalesTel { get; set; }
        /// <summary>
        /// 服务电话
        /// </summary>
        public string ServerTel { get; set; }
        /// <summary>
        /// 系统邮箱
        /// </summary>
        public string SystemEmail { get; set; }

        /// <summary>
        /// 销售部长
        /// </summary>
        public string SalesDepartment { get; set; }
        /// <summary>
        /// 销售部长电话
        /// </summary>
        public string SalesDepartmentTel { get; set; }
        /// <summary>
        /// 大客专员
        /// </summary>
        public string MaxCommissioner { get; set; }
        /// <summary>
        /// 大客专员电话
        /// </summary>
        public string MaxCommissionerTel { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsEnable { get; set; }

        /* 附加字段说明*/

        /// <summary>
        /// 大区
        /// </summary>
        public string Country { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { set; get; }

        /// <summary>
        /// 大区ID
        /// </summary>
        public int CountryID { set; get; }
        /// <summary>
        /// 省ID
        /// </summary>
        public int ProvinceID { set; get; }
    }
}
