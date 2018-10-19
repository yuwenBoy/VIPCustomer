using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 创建日期:2017.09.07
/// 开发作者:zhao.jian
/// </summary>
namespace Common.Entity
{
    /// <summary>
    /// 数据库表说明:经销店上报控制表
    /// </summary>
    public class DealerRepControl
    {
        /// <summary>
        /// 经销店Id
        /// </summary>
        public int DealerId { set; get; }

        /// <summary>
        /// 经销店编号
        /// </summary>
        public string DealerCode { set; get; }

        /// <summary>
        /// 最后上报日期
        /// </summary>
        public DateTime LastRepDateTime { set; get; }

        /// <summary>
        /// 发票限制日期
        /// </summary>
        public DateTime InvoiceDateTime { set; get; }
    }
}
