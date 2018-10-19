using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库说明:用户表
    /// </summary>
    [Serializable]
    public class SysUserEntity
    {
        private int _PKID;
        public int PKID
        {
            set { _PKID = value; }
            get { return _PKID; }
        }
        private string _LoginName;
        public string LoginName
        {
            set { _LoginName = value; }
            get { return _LoginName; }
        }
        private string _Name;
        public string Name
        {
            set { _Name = value; }
            get { return _Name; }
        }
        private string _NickName;
        public string NickName
        {
            set { _NickName = value; }
            get { return _NickName; }
        }

        private int _DealerId;
        public int DealerId
        {
            set { _DealerId = value; }
            get { return _DealerId; }
        }
        private int _UserTypeId;
        public int UserTypeId
        {
            set { _UserTypeId = value; }
            get { return _UserTypeId; }
        }
        private string _LoginPwd;
        public string LoginPwd
        {
            set { _LoginPwd = value; }
            get { return _LoginPwd; }
        }

        private string _Email;
        public string Email
        {
            set { _Email = value; }
            get { return _Email; }
        }
        private string _Phone;
        public string Phone
        {
            set { _Phone = value; }
            get { return _Phone; }
        }
        private int _IsSystemUser;
        public int IsSystemUser
        {
            set { _IsSystemUser = value; }
            get { return _IsSystemUser; }
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
        private string _DealerName;
        public string DealerName
        {
            get { return _DealerName; }
            set { _DealerName = value; }
        }
        private string _UserTypeName;
        public string UserTypeName
        {
            get { return _UserTypeName; }
            set { _UserTypeName = value; }
        }

        /// <summary>
        /// 车辆信息
        /// </summary>
        public List<string> CarName
        {
            get; set;
        }
    }
}
