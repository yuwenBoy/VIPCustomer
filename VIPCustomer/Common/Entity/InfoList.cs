using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 信息表
    /// </summary>
    [Serializable]
    public class InfoList
    {
        /// <summary>
        /// 信息ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? OutDate { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 正文内容
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// 紧急程度ID
        /// </summary>
        public int EmergencyDegreeID { get; set; }

        /// <summary>
        /// 紧急程度名称
        /// </summary>
        public string EmergencyDegreeName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }
    }
}
