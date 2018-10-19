using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库说明:厂牌字典表
    /// </summary>
    [Serializable]
    public class BrandDictionaryEntity
    {
        private string _CarName;
        public string CarName
        {
            get { return _CarName; }
            set { _CarName = value; }
        }

        private int _BackMark;
        public int BackMark
        {
            get { return _BackMark; }
            set { _BackMark = value; }
        }

        private int _CarsBackMark;
        public int CarsBackMark
        {
            get { return _CarsBackMark; }
            set
            {
                _CarsBackMark = value;
            }
        }
    }
}
