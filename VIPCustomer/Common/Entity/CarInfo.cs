using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    [Serializable]
    /// <summary>
    /// 数据库表说明:车型信息表
    /// </summary>
    public class CarInfo
    {
        public int PKID { set; get; }

        /// <summary>
        /// 车名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 车型
        /// </summary>
        public string CarModel { set; get; }
        /// <summary>
        ///  变速箱型号
        /// </summary>
        public string GearboxVersion { set; get; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { set; get; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { set; get; }
        /// <summary>
        /// 车型缩写
        /// </summary>
        public string SFX { set; get; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        ///  类型ID
        /// </summary>
        public int TypeID { set; get; }
        /// <summary>
        /// 内装色
        /// </summary>
        public string BuiltInColor { set; get; }

        /// <summary>
        /// 驱动
        /// </summary>
        public string Drive { set; get; }
        /// <summary>
        /// 建议价格
        /// </summary>
        public double SuggestPrice { set; get; }

        /// <summary>
        /// 出厂价格
        /// </summary>
        public double FactoryPrice { set; get; }

        /// <summary>
        /// 网上价格
        /// </summary>
        public double InternetPrice { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 品牌ID
        /// </summary>
        public int BrandID { set; get; }
        /// <summary>
        /// 其他
        /// </summary>
        public string Other { set; get; }
        /// <summary>
        /// 车辆标识ID
        /// </summary>
        public int CarLogoID { set; get; }


        // 扩展字段
        public string CarLogoName { set; get; }
        public string BrandName { set; get; }
    }
}
