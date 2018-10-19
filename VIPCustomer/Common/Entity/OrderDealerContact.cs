using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    public class OrderDealerContact
    {
        public int PKID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 职务ID
        /// </summary>
        public int JobID { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 移动电话
        /// </summary>
        public string MobileTel { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// 电子信箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 其他联系人信息
        /// </summary>
        public string OtherContactInfo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 经销店名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string JobName { get; set; }
    }
}
