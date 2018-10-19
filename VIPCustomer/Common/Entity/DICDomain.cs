using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    [Serializable]
    /// <summary>
    /// 数据库表说明:域字典表
    /// </summary>
    public class DICDomain
    {
        public int PKID { set; get; }

        /// <summary>
        /// 预编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 域名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 列表名称
        /// </summary>
        public string ListName { set; get; }

        /// <summary>
        ///排序
        /// </summary>
        public int? Sort { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
    }
}
