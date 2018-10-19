using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库说明:角色车辆表
    /// </summary>
    [Serializable]
    public class RoleBrandEntity
    {

        private int _RoleID;
        public int RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }

        private string _CarName;
        public string CarName
        {
            get { return _CarName; }
            set { _CarName = value; }
        }
    }
}
