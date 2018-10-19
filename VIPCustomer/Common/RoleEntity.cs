using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库表说明:角色表
    /// </summary>
    [Serializable]
    public class RoleEntity
    {
        private int _PKID;
        public int PKID
        {
            set { _PKID = value; }
            get { return _PKID; }
        }
        private string _Name;
        public string Name
        {
            set { _Name = value; }
            get { return _Name; }
        }

        private int _IsActivate;
        public int IsActivate
        {
            set { _IsActivate = value; }
            get { return _IsActivate; }
        }

        private string _Remark;
        public string Remark
        {
            set { _Remark = value; }
            get { return _Remark; }
        }

        private int _RoleGrade;
        public int RoleGrade
        {
            get { return _RoleGrade; }
            set { _RoleGrade = value; }
        }


    }
}
