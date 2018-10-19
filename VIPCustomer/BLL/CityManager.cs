using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dapper;
namespace BLL
{
    public class CityManager
    {
        #region 城市维护分页列表
        /// <summary>
        /// 城市维护分页列表
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="sortName">排序字段</param>
        /// <param name="IsDesc">排序规则</param>
        /// <param name="filter">模糊搜索</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.CityDictionary> GetCityManagePager(int pageIndex, int pageSize, string sortName, string IsDesc, string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@" select count(@pageSize) from 
                                            (select d.PKID,c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
                                            (select b.PKID,a.City as 大区,b.City as 省份 from
                                            (select PKID,City from CityDictionary WHERE ParentID='0') a,
                                            CityDictionary b where a.PKID=b.ParentID) c,
                                            CityDictionary d where c.PKID=d.ParentID) e");

                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(@" where ( e.大区 like '%" + filter + "%' or e.省份 like '%" + filter + "%' or e.城市 like '%" + filter + "%')");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数
                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from ( select row_number() over (order by e.Country asc,e.Province asc,e.City asc) as rowNumber,
                                 * from  (select c.CountryID,d.ParentID as ProvinceID,d.PKID, c.大区 as Country,c.省份 as Province,d.City from 
        		                (select b.PKID as ProvinceID,b.ParentID CountryID,a.City as 大区,b.City as 省份 from
        		                (select PKID,City from CityDictionary WHERE ParentID='0') a,
        		                CityDictionary b where a.PKID=b.ParentID) c,
        		                CityDictionary d where c.ProvinceID=d.ParentID ) e  ");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(@" where e.Country like '%" + filter + "%' or e.Province like '%" + filter + "%' or e.City like '%" + filter + "%'");
                }
                strSql.Append($@") as t where t.rowNumber>(@pageIndex-1)*@pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<Common.Entity.CityDictionary> list = conn.Query<Common.Entity.CityDictionary>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 城市维护分页列表
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="filter">模糊搜索</param>
        /// <param name="totalCount">总记录数</param>
        public List<Common.Entity.CityDictionary> GetCityManagePager(int pageIndex, int pageSize, string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@" select count(@pageSize) from 
                                            (select d.PKID,c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
                                            (select b.PKID,a.City as 大区,b.City as 省份 from
                                            (select PKID,City from CityDictionary WHERE ParentID='0') a,
                                            CityDictionary b where a.PKID=b.ParentID) c,
                                            CityDictionary d where c.PKID=d.ParentID) e");

                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(@" where ( e.大区 like '%" + filter + "%' or e.省份 like '%" + filter + "%' or e.城市 like '%" + filter + "%')");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数
                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from ( select row_number() over (order by e.Country asc,e.Province asc,e.City asc) as rowNumber,
                                 * from  (select c.CountryID,d.ParentID as ProvinceID,d.PKID, c.大区 as Country,c.省份 as Province,d.City from 
        		                (select b.PKID as ProvinceID,b.ParentID CountryID,a.City as 大区,b.City as 省份 from
        		                (select PKID,City from CityDictionary WHERE ParentID='0') a,
        		                CityDictionary b where a.PKID=b.ParentID) c,
        		                CityDictionary d where c.ProvinceID=d.ParentID ) e ");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(@" where e.Country like '%" + filter + "%' or e.Province like '%" + filter + "%' or e.City like '%" + filter + "%'");
                }
                strSql.Append($@") as t where t.rowNumber>(@pageIndex-1)*@pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<Common.Entity.CityDictionary> list = conn.Query<Common.Entity.CityDictionary>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        //#region 从服务器端获取城市分页列表
        ///// <summary>
        ///// 从服务器端获取城市分页列表
        ///// </summary>
        ///// <param name="pageIndex">当前页</param>
        ///// <param name="pageSize">页码大小</param>
        ///// <param name="totalCount">总记录数</param>
        ///// <returns></returns>
        //public DataTable GetCityManagerPager(int pageIndex, int pageSize, string searchContent, out int totalCount)
        //{
        //    StringBuilder sqlCount = new StringBuilder();
        //    sqlCount.Append(@" select count(1) from 
        //                                    (select d.PKID,c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
        //                                    (select b.PKID,a.City as 大区,b.City as 省份 from
        //                                    (select PKID,City from CityDictionary WHERE ParentID='0') a,
        //                                    CityDictionary b where a.PKID=b.ParentID) c,
        //                                    CityDictionary d where c.PKID=d.ParentID) e");
        //    if (!string.IsNullOrEmpty(searchContent))
        //    {
        //        sqlCount.Append(@" where ( e.大区 like '%" + searchContent + "%' or e.省份 like '%" + searchContent + "%' or e.城市 like '%" + searchContent + "%')");
        //    }
        //    totalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, sqlCount.ToString()));

        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append(@"select top (@pageSize) * from ( select row_number() over (order by e.大区 asc,e.省份 asc,e.城市 asc) as rowNumber,
        //                    * from  (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
        //		(select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from
        //		(select PKID,City from CityDictionary WHERE ParentID='0') a,
        //		CityDictionary b where a.PKID=b.ParentID) c,
        //		CityDictionary d where c.省份ID=d.ParentID) e ");
        //    if (!string.IsNullOrEmpty(searchContent))
        //    {
        //        strSql.Append(@" where e.大区 like '%" + searchContent + "%' or e.省份 like '%" + searchContent + "%' or e.城市 like '%" + searchContent + "%'");
        //    }
        //    strSql.Append(" ) as t where t.rowNumber>(@pageIndex-1)*@pageSize");

        //    SqlParameter[] param = {
        //        new SqlParameter("@pageSize",pageSize),
        //        new SqlParameter("@pageIndex",pageIndex)
        //    };
        //    DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
        //    Common.Entity.CityDictionary model = null;
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            model = new Common.Entity.CityDictionary();
        //            model.PKID = Convert.ToInt32(dt.Rows[i]["大区ID"]);
        //            model.City = dt.Rows[i]["大区"].ToString();
        //            model.City = dt.Rows[i]["省份"].ToString();
        //            model.City = dt.Rows[i]["城市"].ToString();
        //        }
        //        return dt;
        //    }
        //    return dt;
        //}
        //#endregion

        #region 获取省份或城市列表
        /// <summary>
        /// 获取省份或城市列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetList()
        {
            return new DAL.CityManager().GetList();
        }
        #endregion

        #region 获取大区获取对应的省
        /// <summary>
        /// 获取大区获取对应的省
        /// </summary>
        /// <returns></returns>
        public DataTable GetProvince(int cityId)
        {
            return new DAL.CityManager().GetProvince(cityId);
        }
        #endregion

        #region 获取省获取对应的城市
        /// <summary>
        /// 获取省获取对应的城市
        /// </summary>
        /// <returns></returns>
        public DataTable GetCity(int cityId)
        {
            return new DAL.CityManager().GetCity(cityId);
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(Common.Entity.CityDictionary model)
        {
            return new DAL.CityManager().Add(model);
        }
        #endregion

        #region 是否存在该记录
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <param name="city">城市</param>
        /// <returns></returns>
        public bool Exists(int parentId, string city)
        {
            return new DAL.CityManager().Exists(parentId, city);
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="cityId">PKID</param>
        /// <returns></returns>
        public Common.Entity.CityDictionary GetCityDictionaryByPKID(int cityId)
        {
            return new DAL.CityManager().GetCityDictionaryByPKID(cityId);
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public bool Update(Common.Entity.CityDictionary model)
        {
            return new DAL.CityManager().Update(model);
        }
        #endregion
    }
}
