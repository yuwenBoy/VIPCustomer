using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class InfoList
    {
        #region 从服务器端获取维护通知分页列表
        /// <summary>
        /// 从服务器端获取维护通知分页列表(当按排序查询时)
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.InfoList> GetInfoManagePager(int pageIndex, int pageSize, string sortName, string IsDesc, Common.Entity.InfoList filter, out int totalCount)
        {
            return new DAL.InfoList().GetInfoManagePager(pageIndex, pageSize, sortName, IsDesc, filter, out totalCount);
        }
        /// <summary>
        /// 从服务器端获取维护通知分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.InfoList> GetInfoManagePager(int pageIndex, int pageSize, Common.Entity.InfoList filter, out int totalCount)
        {
            return new DAL.InfoList().GetInfoManagePager(pageIndex, pageSize, filter, out totalCount);
        }
        #endregion

        #region 新建信息
        /// <summary>
        /// 新建信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(Common.Entity.InfoList entity)
        {
            return new DAL.InfoList().Add(entity);
        }
        #endregion

        #region 获取实体对象
        public Common.Entity.InfoList GetModel(int InfoID)
        {
            return new DAL.InfoList().GetModel(InfoID);
        }
        #endregion

        #region 删除信息
        public int DELETEDATE(int pkid)
        {
            return new DAL.InfoList().DELETEDATE(pkid);
        }
        #endregion

        #region 更新数据
        public int Update(Common.Entity.InfoList entity)
        {
            return new DAL.InfoList().Update(entity);
        }
        #endregion

        public void InsertDealerInfo(int infoID, string dealerIDs)
        {
           // IList<int> subs = 
        }
    }
}
