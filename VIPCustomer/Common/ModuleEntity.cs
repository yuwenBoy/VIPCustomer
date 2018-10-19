using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 数据库说明:模块表
    /// </summary>
    [Serializable]
    public class ModuleEntity
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
        private int _ParentID;
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentID
        {
            set { _ParentID = value; }
            get { return _ParentID; }
        }

        public int IsActivate { set; get; }

        public int IsSysModule { set; get; }
        public string PageAddress { set; get; }


        private string _ImagesAddress;
        public string ImagesAddress 
        {
            get { return _ImagesAddress; }
            set { _ImagesAddress = value; }
        }

        private string _ChangeImgAddress;
        public string ChangeImgAddress
        {
            get { return _ChangeImgAddress; }
            set { _ChangeImgAddress = value; }
        }

        private string _FrameName;
        public string FrameName
        {
            get { return _FrameName; }
            set { _FrameName = value; }
        }

        private string _Remark;
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }

        private int _Sort;

        public int Sort
        {
            get { return _Sort; }
            set { _Sort = value; }
        }
    }
}
