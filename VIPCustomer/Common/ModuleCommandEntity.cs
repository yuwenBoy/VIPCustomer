using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库说明:模块控制表
    /// </summary>
    public class ModuleCommandEntity
    {
        public int PKID { set; get; }

        public int ModuleID { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }

        public string Remark { set; get; }
    }
}
