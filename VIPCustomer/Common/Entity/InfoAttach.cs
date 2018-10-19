using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 信息附件表
    /// </summary>
    [Serializable]
    public class InfoAttach
    {
        /// <summary>
        /// 附件ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 信息ID
        /// </summary>
        public int InfoID { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public string AttachType { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string AttachName { get; set; }

        /// <summary>
        /// 附件大小
        /// </summary>
        public int? AttachSize { get; set; }

        /// <summary>
        /// 附件路径
        /// </summary>
        public string AttachPath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
