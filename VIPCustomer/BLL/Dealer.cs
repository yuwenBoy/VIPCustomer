using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
/// <summary>
/// 业务逻辑层:BLL 创建日期:2017.09.05 创建人:zhao.jian
/// </summary>
namespace BLL
{
    public class Dealer
    {

        #region 获取经销商分页列表
        /// <summary>
        /// 获取经销商分页列表(当按排序查询时)
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.Dealer> GetDealerManagePager(int pageIndex, int pageSize, string sortName, string IsDesc, Common.Entity.Dealer filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from Dealer DL left join (select * from  (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
                                (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from (select PKID,City from CityDictionary WHERE ParentID='0') a,
                                 CityDictionary b where a.PKID=b.ParentID) c,
                                 CityDictionary d where c.省份ID=d.ParentID) e) CT on  DL.CityId=CT.城市ID where DL.PKID>0 ");
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    sqlCount.Append(" and DL.Code like'%" + filter.Code + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    sqlCount.Append(" and DL.Name like'%" + filter.Name + "%'");
                }
                if (filter.CountryID > 0)
                {
                    sqlCount.Append(" and CT.大区ID=@大区ID");
                }
                if (filter.ProvinceID > 0)
                {
                    sqlCount.Append(" and CT.省份ID=@省份ID");
                }
                if (filter.CityId > 0)
                {
                    sqlCount.Append("  and DL.CityId=@CityId");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("大区ID", filter.CountryID);
                param1.Add("省份ID", filter.ProvinceID);
                param1.Add("CityId", filter.CityId);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (select row_number() over (order by Name) as rowNumber,CT.城市ID,CT.City,CT.Province,CT.省份ID,CT.Country,CT.大区ID,DL.* from Dealer DL
        			         left join (select * from (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as Country,c.省份 as Province,d.City as City from 
        			         (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from
        			         (select PKID,City from CityDictionary WHERE ParentID='0') a,
        			         CityDictionary b where a.PKID=b.ParentID) c,
        			         CityDictionary d where c.省份ID=d.ParentID) e) CT
        			         on  DL.CityId=CT.城市ID where DL.PKID>0");
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    strSql.Append(" and DL.Code like'%" + filter.Code + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    strSql.Append(" and DL.Name like'%" + filter.Name + "%'");
                }
                if (filter.CountryID > 0)
                {
                    strSql.Append(" and CT.大区ID=@大区ID");
                }
                if (filter.ProvinceID > 0)
                {
                    strSql.Append(" and CT.省份ID=@省份ID");
                }
                if (filter.CityId > 0)
                {
                    strSql.Append(" and CT.城市ID=@城市ID");
                }
                strSql.Append($"  ) as t where t.rowNumber > (@pageIndex - 1) * @pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("大区ID", filter.CountryID);
                param2.Add("省份ID", filter.ProvinceID);
                param2.Add("城市ID", filter.CityId);
                List<Common.Entity.Dealer> list = conn.Query<Common.Entity.Dealer>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 获取经销商分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.Dealer> GetDealerManagePager(int pageIndex, int pageSize, Common.Entity.Dealer filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from Dealer DL left join (select * from  (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
                                (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from (select PKID,City from CityDictionary WHERE ParentID='0') a,
                                 CityDictionary b where a.PKID=b.ParentID) c,
                                 CityDictionary d where c.省份ID=d.ParentID) e) CT on  DL.CityId=CT.城市ID where DL.PKID>0 ");
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    sqlCount.Append(" and DL.Code like'%" + filter.Code + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    sqlCount.Append(" and DL.Name like'%" + filter.Name + "%'");
                }
                if (filter.CountryID > 0)
                {
                    sqlCount.Append(" and CT.大区ID=@大区ID");
                }
                if (filter.ProvinceID > 0)
                {
                    sqlCount.Append(" and CT.省份ID=@省份ID");
                }
                if (filter.CityId > 0)
                {
                    sqlCount.Append("  and DL.CityId=@CityId");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("大区ID", filter.CountryID);
                param1.Add("省份ID", filter.ProvinceID);
                param1.Add("CityId", filter.CityId);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (select row_number() over (order by Name) as rowNumber,CT.城市ID,CT.City,CT.Province,CT.省份ID,CT.Country,CT.大区ID,DL.* from Dealer DL
        			         left join (select * from (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as Country,c.省份 as Province,d.City as City from 
        			         (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from
        			         (select PKID,City from CityDictionary WHERE ParentID='0') a,
        			         CityDictionary b where a.PKID=b.ParentID) c,
        			         CityDictionary d where c.省份ID=d.ParentID) e) CT
        			         on  DL.CityId=CT.城市ID where DL.PKID>0 ");
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    strSql.Append(" and DL.Code like'%" + filter.Code + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    strSql.Append(" and DL.Name like'%" + filter.Name + "%'");
                }
                if (filter.CountryID > 0)
                {
                    strSql.Append(" and CT.大区ID=@大区ID");
                }
                if (filter.ProvinceID > 0)
                {
                    strSql.Append(" and CT.省份ID=@省份ID");
                }
                if (filter.CityId > 0)
                {
                    strSql.Append(" and CT.城市ID=@城市ID");
                }
                strSql.Append("  ) as t where t.rowNumber > (@pageIndex - 1) * @pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("大区ID", filter.CountryID);
                param2.Add("省份ID", filter.ProvinceID);
                param2.Add("城市ID", filter.CityId);
                List<Common.Entity.Dealer> list = conn.Query<Common.Entity.Dealer>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        #region 从服务器端获取经销店分页列表
        /// <summary>
        /// 从服务器端获取经销店分页列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>返回DataTable</returns>
        public List<Common.Entity.Dealer> GetDealerListPager(int pageIndex, int pageSize, string sortName, string IsDesc,string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from Dealer DL left join (select * from  (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
                                (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from (select PKID,City from CityDictionary WHERE ParentID='0') a,
                                 CityDictionary b where a.PKID=b.ParentID) c,
                                 CityDictionary d where c.省份ID=d.ParentID) e) CT on  DL.CityId=CT.城市ID where DL.PKID>0 ");
                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(" and DL.Code like '%" + filter + "%'or DL.Name like '%" + filter + "%'");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (select row_number() over (order by Name) as rowNumber,CT.城市ID,CT.City,CT.Province,CT.省份ID,CT.Country,CT.大区ID,DL.* from Dealer DL
        			         left join (select * from (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as Country,c.省份 as Province,d.City as City from 
        			         (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from
        			         (select PKID,City from CityDictionary WHERE ParentID='0') a,
        			         CityDictionary b where a.PKID=b.ParentID) c,
        			         CityDictionary d where c.省份ID=d.ParentID) e) CT
        			         on  DL.CityId=CT.城市ID where DL.PKID>0");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(" and DL.Code like '%" + filter + "%'or DL.Name like '%" + filter + "%'");
                }
                strSql.Append($"  ) as t where t.rowNumber > (@pageIndex - 1) * @pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<Common.Entity.Dealer> list = conn.Query<Common.Entity.Dealer>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 从服务器端获取经销店分页列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>返回DataTable</returns>
        public List<Common.Entity.Dealer> GetDealerListPager(int pageIndex, int pageSize,  string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from Dealer DL left join (select * from  (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
                                (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from (select PKID,City from CityDictionary WHERE ParentID='0') a,
                                 CityDictionary b where a.PKID=b.ParentID) c,
                                 CityDictionary d where c.省份ID=d.ParentID) e) CT on  DL.CityId=CT.城市ID where DL.PKID>0 ");
                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(" and DL.Code like '%" + filter + "%'or DL.Name like '%" + filter + "%'");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (select row_number() over (order by Name) as rowNumber,CT.城市ID,CT.City,CT.Province,CT.省份ID,CT.Country,CT.大区ID,DL.* from Dealer DL
        			         left join (select * from (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as Country,c.省份 as Province,d.City as City from 
        			         (select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from
        			         (select PKID,City from CityDictionary WHERE ParentID='0') a,
        			         CityDictionary b where a.PKID=b.ParentID) c,
        			         CityDictionary d where c.省份ID=d.ParentID) e) CT
        			         on  DL.CityId=CT.城市ID where DL.PKID>0");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(" and DL.Code like '%" + filter + "%'or DL.Name like '%" + filter + "%'");
                }
                strSql.Append($"  ) as t where t.rowNumber > (@pageIndex - 1) * @pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<Common.Entity.Dealer> list = conn.Query<Common.Entity.Dealer>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        #region 获取经销店实体对象
        public Common.Entity.Dealer GetDealerByPKID(int dealerId)
        {
            return new DAL.Dealer().GetDealerByPKID(dealerId);
        }
        #endregion

        #region 新增一条数据
        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(Common.Entity.Dealer model)
        {
            return new DAL.Dealer().Add(model);
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public bool Update(Common.Entity.Dealer model)
        {
            return new DAL.Dealer().Update(model);
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="PKID">经销店ID</param>
        /// <returns></returns>
        public bool DELETEDATE(int PKID)
        {
            return new DAL.Dealer().DELETEDATE(PKID);
        }
        #endregion

        #region 是否存在经销店编号或经销店名称
        /// <summary>
        /// 是否存在经销店编号或经销店名称
        /// </summary>
        /// <param name="code">经销店编号</param>
        /// <param name="name">经销店名称</param>
        /// <returns></returns>
        public bool IsExistsDealerCodeAndName(string code, string name)
        {
            return new DAL.Dealer().IsExistsDealerCodeAndName(code, name);
        }
        #endregion

        #region 根据经销店编号查询经销店信息
        /// <summary>
        /// 根据经销店编号查询经销店信息
        /// </summary>
        /// <param name="code">经销店编号</param>
        /// <returns>返回值</returns>
        public Common.Entity.Dealer GetDealerByCode(string code)
        {
            return new DAL.Dealer().GetDealerByCode(code);
        }
        #endregion

        #region 获取系统可以上报最晚数据的时间限制
        /// <summary>
        /// 获取系统可以上报最晚数据的时间限制
        /// </summary>
        /// <param name="dealerID">经销店ID</param>
        /// <returns></returns>
        public Common.Entity.DealerRepControl GetLastReportDateSet(int dealerID)
        {
            return new DAL.Dealer().GetLastReportDateSet(dealerID);
        }
        #endregion

        #region 设置权限
        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <param name="code">经销店编号</param>
        /// <param name="LastRepDateTime">最后上报日期</param>
        /// <param name="InvoiceDateTime">发票限制日期</param>
        /// <returns></returns>
        public int SetLastReportDate(int dealerId, string code, string LastRepDateTime, string InvoiceDateTime)
        {
            return new DAL.Dealer().SetLastReportDate(dealerId, code, LastRepDateTime, InvoiceDateTime);
        }
        #endregion

        #region 批量删除信息
        /// <summary>
        /// 批量删除信息
        /// </summary>
        /// <param name="dealerIds">dealerID</param>
        /// <returns>返回结果</returns>
        public bool DeleteData(string customerIds)
        {
            return new DAL.Dealer().DeleteData(customerIds);
        }
        #endregion
    }
}
