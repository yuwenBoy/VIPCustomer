using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;

namespace BLL
{
    public class DealerContact
    {
        #region 从服务器端获取经销店分页列表
        /// <summary>
        /// 从服务器端获取经销店分页列表(当按排序查询时)
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.DealerContact> GetDealerContactManagerPager(int pageIndex, int pageSize, int dealerId, string sortName, string IsDesc, Common.Entity.DealerContact keywork, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from (select DL.Name as DealerName,DT.ListName as JobName,DC.* from DealerContact DC
                                  left join Dealer DL on DC.DealerID=DL.PKID left join Dictionary DT on DC.JobID=DT.PKID where DC.PKID>0 
                                  and DC.DealerID=@DealerID");
                if (!string.IsNullOrEmpty(keywork.Name))
                {
                    sqlCount.Append(@" and DC.Name like '%"+ keywork.Name + "%'");
                }
                if (keywork.JobID > 0)
                {
                    sqlCount.Append(@" and DC.JobID=@JobID");
                }
                sqlCount.Append(@"  ) as T");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("DealerID", dealerId);
                param1.Add("JobID", keywork.JobID);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from (select row_number() over (order by DC.Name) as rowNumber,
                                            DL.Name as DealerName,DT.ListName as JobName,
                                            DC.* from DealerContact DC
                                            left join Dealer DL on DC.DealerID=DL.PKID
                                            left join Dictionary DT on DC.JobID=DT.PKID where DC.PKID>0 and DC.DealerID=@DealerID");
                if (!string.IsNullOrEmpty(keywork.Name))
                {
                    strSql.Append(@" and DC.Name like '%" + keywork.Name + "%'");
                }
                if (keywork.JobID > 0)
                {
                    strSql.Append(@" and DC.JobID=@JobID");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("DealerID", dealerId);
                param2.Add("JobID", keywork.JobID);
                List<Common.Entity.DealerContact> list = conn.Query<Common.Entity.DealerContact>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 从服务器端获取经销店分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.DealerContact> GetDealerContactManagerPager(int pageIndex, int pageSize, int dealerId, Common.Entity.DealerContact keywork, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from (select DL.Name as DealerName,DT.ListName as JobName,DC.* from DealerContact DC
                                                 left join Dealer DL on DC.DealerID=DL.PKID
                                                 left join Dictionary DT on DC.JobID=DT.PKID where DC.PKID>0 and DC.DealerID=@DealerID");
                if (!string.IsNullOrEmpty(keywork.Name))
                {
                    sqlCount.Append(@" and DC.Name like '%" + keywork.Name + "%'");
                }
                if (keywork.JobID > 0)
                {
                    sqlCount.Append(@" and DC.JobID=@JobID");
                }
                sqlCount.Append(@" ) as T");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("DealerID", dealerId);
                param1.Add("JobID", keywork.JobID);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault();// 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (select row_number() over (order by DC.Name) as rowNumber,
                            DL.Name as DealerName,DT.ListName as JobName,
                            DC.* from DealerContact DC
                            left join Dealer DL on DC.DealerID=DL.PKID
                            left join Dictionary DT on DC.JobID=DT.PKID where DC.PKID>0 and DC.DealerID=@DealerID");
                if (!string.IsNullOrEmpty(keywork.Name))
                {
                    strSql.Append(@" and DC.Name like '%" + keywork.Name + "%'");
                }
                if (keywork.JobID > 0)
                {
                    strSql.Append(@" and DC.JobID=@JobID");
                }
                strSql.Append(@"   ) as t where t.rowNumber>(@pageIndex-1)*@pageSize ");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("DealerID", dealerId);
                param2.Add("JobID", keywork.JobID);
                List<Common.Entity.DealerContact> list = conn.Query<Common.Entity.DealerContact>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        #region 获取职务列表
        /// <summary>
        /// 获取职务列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetJobList()
        {
            return new DAL.DealerContact().GetJobList();
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(Common.Entity.DealerContact model)
        {
            return new DAL.DealerContact().Add(model);
        }
        #endregion

        #region 是否存在该记录
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="mobileTel">移动电话</param>
        /// <returns></returns>
        public bool Exists(string name, string mobileTel)
        {
            return new DAL.DealerContact().Exists(name, mobileTel);
        }
        #endregion

        #region 获取体对象
        public Common.Entity.DealerContact GetDealerContactByPKID(int dealerId)
        {
            return new DAL.DealerContact().GetDealerContactByPKID(dealerId);
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public bool Update(Common.Entity.DealerContact model)
        {
            return new DAL.DealerContact().Update(model);
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="PKID">经销店联系人ID</param>
        /// <returns></returns>
        public bool DELETEContactDATE(int PKID)
        {
            return new DAL.DealerContact().DELETEContactDATE(PKID);
        }
        #endregion

        #region 根据经销店ID获取经销店联系人列表
        /// <summary>
        /// 根据经销店ID获取经销店联系人列表
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <returns>返回数据</returns>
        public List<Common.Entity.DealerContact> GetListByDealerID(string dealerId)
        {
            string sql = $@"select DT.ListName JobName, DC.*FROM DealerContact DC
                          LEFT JOIN Dictionary DT ON DC.JobID = DT.PKID
                          where DC.DealerID = {dealerId}";
            List<Common.Entity.DealerContact> list = new List<Common.Entity.DealerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.DealerContact>(sql).ToList();
            }
            return list;
        }

        /// <summary>
        /// 根据经销店ID获取经销店联系人列表
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <returns>返回数据</returns>
        public List<Common.Entity.DealerContact> GetListByDealerID(int dealerId)
        {
            string sql = $@"select DT.ListName JobName, DC.*FROM DealerContact DC
                          LEFT JOIN Dictionary DT ON DC.JobID = DT.PKID
                          where DC.DealerID = {dealerId}";
            List<Common.Entity.DealerContact> list = new List<Common.Entity.DealerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.DealerContact>(sql).ToList();
            }
            return list;
        }
        #endregion

        #region 根据订单ID获取订单经销店联系人列表
        /// <summary>
        /// 根据订单ID获取订单经销店联系人列表
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>返回数据</returns>
        public List<Common.Entity.OrderDealerContact> GetListByOrdersID(string orderId)
        {
            string sql = string.Format($@"SELECT ODC.Name,DT.ListName JobName,ODC.Phone,ODC.MobileTel,ODC.Fax,ODC.Email,
                                             ODC.OtherContactInfo FROM OrderDealerContact ODC LEFT JOIN Dictionary DT ON ODC.JobID= DT.PKID
                                             where ODC.OrderID={orderId}");
            List<Common.Entity.OrderDealerContact> list = new List<Common.Entity.OrderDealerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.OrderDealerContact>(sql).ToList();
            }
            return list;
        }

        /// <summary>
        /// 根据订单ID获取订单经销店联系人列表
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>返回数据</returns>
        public List<Common.Entity.DealerContact> GetListByOrdersID(int orderId)
        {
            string sql = string.Format($@"SELECT ODC.Name,DT.ListName JobName,ODC.Phone,ODC.MobileTel,ODC.Fax,ODC.Email,
                                             ODC.OtherContactInfo FROM OrderDealerContact ODC LEFT JOIN Dictionary DT ON ODC.JobID= DT.PKID
                                             where ODC.OrderID={orderId}");
            List<Common.Entity.DealerContact> list = new List<Common.Entity.DealerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.DealerContact>(sql).ToList();
            }
            return list;
        }
        #endregion

        /// <summary>
        /// 根据经销店ID获取经销店联系人的个数
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <returns></returns>
        public int GetDealerContactCount(int dealerId)
        {
            return new DAL.DealerContact().GetDealerContactCount(dealerId);
        }
    }
}
