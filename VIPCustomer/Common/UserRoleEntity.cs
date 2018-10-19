using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库表说明:用户角色表
    /// </summary>
    [Serializable]
    public class UserRoleEntity
    {
        private int _RoleID;
        public int RoleID
        {
            set { _RoleID = value; }
            get { return _RoleID; }
        }

        private string _Remark;
        public string Remark
        {
            set { _Remark = value; }
            get { return _Remark; }
        }

        private int _UserID;
        public int UserID
        {
            set { _UserID = value; }
            get { return _UserID; }
        }
    }
}
