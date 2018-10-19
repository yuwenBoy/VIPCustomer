using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 城市字典表
    /// </summary>
    public class CityDictionary
    {
        /// <summary>
        /// 城市ID
        /// </summary>
        public int PKID { set; get; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentID { set; get; }
        
        /// <summary>
        /// 城市
        /// </summary>
        public string City { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }

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
        /// 大区ID
        /// </summary>
        public int CountryID { set; get; }
        /// <summary>
        /// 省ID
        /// </summary>
        public int ProvinceID { set; get; }

    }
}
