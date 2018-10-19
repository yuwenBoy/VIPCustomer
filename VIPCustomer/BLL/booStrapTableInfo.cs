using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BoostrapTableInfo<T>
    {
        public List<T> rows { get; set; }
        public int total { get; set; }
    }

    public class sortInfo
    {
        public string SortName { get; set; }
        public string IsDesc { get; set; }
        /// <summary>
        /// 获取排序的信息
        /// </summary>
        /// <returns></returns>
        public sortInfo(string name, string order)
        {
            SortName = name;
            IsDesc = order;
        }
        #region 获取排序内容方法
        /// <summary>
        /// 获取排序内容方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static sortInfo GetSortOrder(string name, string order)
        {
            return new sortInfo(name, order);
        }
        #endregion
    }


}
