using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 数据库表说明:车辆颜色表
    /// </summary>
    [Serializable]
    public class CarColor
    {
        public int PKID { set; get; }
        /// <summary>
        /// 车辆ID
        /// </summary>
        public int CarID { set; get; }
        /// <summary>
        /// 颜色编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        ///  颜色名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
    }
}
