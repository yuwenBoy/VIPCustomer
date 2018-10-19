using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace DAL
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from (select  DT2.ListName EmergencyDegreeName,IL.PKID,IL.Title,DT.ListName TypeName,
                                IL.Author,IL.InDate,IL.OutDate,IL.ModifyDate from InfoList IL
                                left join Dictionary DT2 on IL.EmergencyDegreeID=DT2.PKID
                                left join Dictionary DT on IL.TypeID=DT.PKID and IL.PKID>0) as T");

                if (!string.IsNullOrEmpty(filter.Title))
                {
                    sqlCount.Append(" and SU.LoginName like '%" + filter.Title + "%'");
                }
                if (filter.EmergencyDegreeID > 0)
                {
                    sqlCount.Append(" and SU.UserTypeId=@UserTypeId");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("EmergencyDegreeID", filter.EmergencyDegreeID);
                param1.Add("pageSize", pageSize);
                param1.Add("Title", filter.Title);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (select row_number() over(order by IL.CreateTime desc) as rowNumber,
                                 DT2.ListName EmergencyDegreeName,IL.PKID,IL.Title,DT.ListName TypeName,IL.Author,IL.InDate,IL.OutDate,IL.ModifyDate from InfoList IL
                                left join Dictionary DT2 on IL.EmergencyDegreeID=DT2.PKID
                                left join Dictionary DT on IL.TypeID=DT.PKID and IL.PKID>0");
                if (!string.IsNullOrEmpty(filter.Title))
                {
                    strSql.Append(" and SU.LoginName like '%" + filter.Title + "%'");
                }
                if (filter.EmergencyDegreeID > 0)
                {
                    strSql.Append("  and SU.UserTypeId=@UserTypeId");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("EmergencyDegreeID", filter.EmergencyDegreeID);
                param2.Add("Title", filter.Title);
                List<Common.Entity.InfoList> list = conn.Query<Common.Entity.InfoList>(strSql.ToString(), param2).ToList();
                return list;
            };
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from (select  DT2.ListName EmergencyDegreeName,IL.PKID,IL.Title,DT.ListName TypeName,
                                IL.Author,IL.InDate,IL.OutDate,IL.ModifyDate from InfoList IL
                                left join Dictionary DT2 on IL.EmergencyDegreeID=DT2.PKID
                                left join Dictionary DT on IL.TypeID=DT.PKID and IL.PKID>0) as T");

                if (!string.IsNullOrEmpty(filter.Title))
                {
                    sqlCount.Append(" and SU.LoginName like '%" + filter.Title + "%'");
                }
                if (filter.EmergencyDegreeID > 0)
                {
                    sqlCount.Append(" and SU.UserTypeId=@UserTypeId");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("EmergencyDegreeID", filter.EmergencyDegreeID);
                param1.Add("pageSize", pageSize);
                param1.Add("Title", filter.Title);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (select row_number() over(order by IL.CreateTime desc) as rowNumber,
                                 DT2.ListName EmergencyDegreeName,IL.PKID,IL.Title,DT.ListName TypeName,IL.Author,IL.InDate,IL.OutDate,IL.ModifyDate from InfoList IL
                                left join Dictionary DT2 on IL.EmergencyDegreeID=DT2.PKID
                                left join Dictionary DT on IL.TypeID=DT.PKID and IL.PKID>0");
                if (!string.IsNullOrEmpty(filter.Title))
                {
                    strSql.Append(" and SU.LoginName like '%" + filter.Title + "%'");
                }
                if (filter.EmergencyDegreeID > 0)
                {
                    strSql.Append("  and SU.UserTypeId=@UserTypeId");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("EmergencyDegreeID", filter.EmergencyDegreeID);
                param2.Add("Title", filter.Title);
                List<Common.Entity.InfoList> list = conn.Query<Common.Entity.InfoList>(strSql.ToString(), param2).ToList();
                return list;
            };
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"Insert into InfoList (TypeID,Title,CreateTime,InDate,OutDate,Author,Contents,EmergencyDegreeID,ModifyDate) values(@TypeID,
                                              @Title,@CreateTime,@InDate,@OutDate,@Author,@Contents,@EmergencyDegreeID,@ModifyDate)");
                DynamicParameters Parameters = new DynamicParameters();
                Parameters.Add("TypeID", entity.TypeID);
                Parameters.Add("Title", entity.Title);
                Parameters.Add("CreateTime", entity.CreateTime);
                Parameters.Add("InDate", entity.InDate);
                Parameters.Add("OutDate", entity.OutDate);
                Parameters.Add("Author", entity.Author);
                Parameters.Add("Contents", entity.Contents);
                Parameters.Add("EmergencyDegreeID", entity.EmergencyDegreeID);
                Parameters.Add("ModifyDate", entity.ModifyDate);
                int result = conn.Execute(sql, Parameters);
                if (result > 0)
                    return result;
                else
                    return 0;
            }
        }
        #endregion

        #region 删除信息
        public int DELETEDATE(int pkid)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format("delete from InfoList where PKID=@pkid");
                int result = 0;
                DynamicParameters param = new DynamicParameters();
                param.Add("pkid", pkid);
                result = conn.Execute(sql, param);
                if (result > 0)
                    return result;
                else
                    return 0;
            }
        }
        #endregion

        #region 获取实体对象
        public Common.Entity.InfoList GetModel(int InfoID)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                Common.Entity.InfoList entity = new Common.Entity.InfoList();
                string sql = string.Format(@"select * from InfoList where PKID=@InfoID");
                DynamicParameters param = new DynamicParameters();
                param.Add("InfoID", InfoID);
                entity = conn.Query<Common.Entity.InfoList>(sql, param).FirstOrDefault();
                return entity;
            }
        }
        #endregion

        #region 更新数据
        public int Update(Common.Entity.InfoList entity)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" Update InfoList set TypeID=@TypeID,Title=@Title,InDate=@InDate,OutDate=@OutDate,
                                              Contents=@Contents,EmergencyDegreeID=@EmergencyDegreeID,ModifyDate=@ModifyDate where PKID=@PKID");
                DynamicParameters param = new DynamicParameters();
                param.Add("TypeID", entity.TypeID);
                param.Add("Title", entity.Title);
                param.Add("InDate", entity.InDate);
                param.Add("OutDate", entity.OutDate);
                param.Add("Contents", entity.Contents);
                param.Add("EmergencyDegreeID", entity.EmergencyDegreeID);
                param.Add("ModifyDate", entity.ModifyDate);
                param.Add("PKID", entity.PKID);
                int result = conn.Execute(sql, param);
                if (result > 0)
                    return result;
                else
                    return 0;
            }
        }
        #endregion
    }
}
