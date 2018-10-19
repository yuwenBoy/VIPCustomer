using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    [Serializable]
    public class SysUserModuleRight
    {
        /// <summary>
        /// 菜单名称标识
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsRight { get; set; }
    }
}
