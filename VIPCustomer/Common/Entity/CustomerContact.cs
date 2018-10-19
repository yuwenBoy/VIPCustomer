using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 数据库表说明:客户联系人表
    /// </summary>
    public class CustomerContact
    {
        /// <summary>
        /// 客户联系人ID
        /// </summary>
        public string PKID { set; get; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerID { set; get; }

        /// <summary>
        /// 客户联系人姓名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Job { set; get; }

        /// <summary>
        /// 客户联系人角色ID
        /// </summary>
        public int RoleID { set; get; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// 移动电话
        /// </summary>
        public string MobileTel { set; get; }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { set; get; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { set; get; }

        /// <summary>
        /// 其它联系方式
        /// </summary>
        public string OtherContactInfo { set; get; }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { set; get; }

        /// <summary>
        /// 爱好
        /// </summary>
        public string Hobbies { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }

        //=======数据表外加字段=============//
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { set; get; }
    }
}
