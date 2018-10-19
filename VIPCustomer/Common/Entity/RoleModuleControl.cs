using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    /// <summary>
    /// 数据库表说明:角色模块控制表
    /// </summary>
    [Serializable]
    public class RoleModuleControl
    {
        public int RoleID { set; get; }

        /// <summary>
        /// 模块控制ID
        /// </summary>
        public int ModuleCmdID { set; get; }

        public int ModuleID { set; get; }

        public string Remark { set; get; }
    }
}
