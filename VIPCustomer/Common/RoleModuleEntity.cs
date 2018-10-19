using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库说明:角色模块表
    /// </summary>
    [Serializable]
    public class RoleModuleEntity
    {
        public int RoleID { set; get; }

        public string Remark { set; get; }

        public int ModuleID { set; get; }
    }
}
