using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CityManager
    {
        #region 获取省份或城市列表
        /// <summary>
        /// 获取省份或城市列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetList()
        {
            string sql = string.Format(@"select PKID as cityId, City from CityDictionary where ParentID=0 order by City");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取大区获取对应的省
        /// <summary>
        /// 获取大区获取对应的省
        /// </summary>
        /// <returns></returns>
        public DataTable GetProvince(int cityId)
        {
            string sql = string.Format(@"select PKID,City from CityDictionary where ParentID = @ParentID order by City");
            SqlParameter[] paramer = {
                new SqlParameter("@ParentID",cityId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, paramer);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取省获取对应的城市
        /// <summary>
        /// 获取省获取对应的城市
        /// </summary>
        /// <returns></returns>
        public DataTable GetCity(int cityId)
        {
            string sql = string.Format(@"select PKID as cityId,ParentID,City as CityName from CityDictionary where ParentID=@ParentID order by City");
            SqlParameter[] paramer = {
                new SqlParameter("@ParentID",cityId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, paramer);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into CityDictionary(ParentID,City,Sort,Code) Values");
            strSql.Append("(@ParentID,@City,@Sort,@Code)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@ParentID",model.ParentID),
                new SqlParameter("@City",model.City),
                new SqlParameter("@Sort",model.Sort),
                new SqlParameter("@Code",model.Code),
            };
            try
            {
                return SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            string sql = string.Format(@"select count(1) from CityDictionary where ParentID=@ParentID and City=@City");
            SqlParameter[] param = {
                new SqlParameter("@ParentID",parentId),
                new SqlParameter("@City",city)
            };
            int count = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, sql, param));
            if (count > 0)
                return true;
            else
                return false;
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from  (select c.大区ID,d.ParentID as 省份ID,d.PKID as 城市ID, c.大区 as 大区,c.省份 as 省份,d.City as 城市 from 
										(select b.PKID as 省份ID,b.ParentID as 大区ID,a.City as 大区,b.City as 省份 from
										(select PKID,City from CityDictionary WHERE ParentID='0') a,
										CityDictionary b where a.PKID=b.ParentID) c,
										CityDictionary d where c.省份ID=d.ParentID) e where e.城市ID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",cityId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            Common.Entity.CityDictionary model = null;
            if (dt.Rows.Count > 0)
            {
                model = new Common.Entity.CityDictionary();
                model.PKID = Convert.ToInt32(dt.Rows[0]["城市ID"]);
                model.ProvinceID = Convert.ToInt32(dt.Rows[0]["省份ID"]);
                model.CountryID = Convert.ToInt32(dt.Rows[0]["大区ID"]);
                model.Country = dt.Rows[0]["大区"].ToString();
                model.Province = dt.Rows[0]["省份"].ToString();
                model.City = dt.Rows[0]["城市"].ToString();
                return model;
            }
            return model;
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update CityDictionary set ParentID=@ParentID,City=@City where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",model.PKID),
                new SqlParameter("@ParentID",model.ParentID),
                new SqlParameter("@City",model.City),
            };
            object obj = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            try
            {
                if (Convert.ToInt32(obj) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
