using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace DAL
{
    public class DICDomain
    {
        #region 是否存在该域
        /// <summary>
        /// 是否存在该域
        /// </summary>
        /// <param name="name">域名称</param>
        /// <param name="code">域编码</param>
        /// <returns></returns>
        public bool ExistsDomain(string name, string code)
        {
            string sql = string.Format(" select count(1) from Dictionary where Name=@Name or Code=@Code");
            SqlParameter[] param = {
                new SqlParameter("@Name",name),
                new SqlParameter("@Code",code)
            };
            try
            {
                int count = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, System.Data.CommandType.Text, sql, param));
                if (count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 新增一条记录
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Add(Common.Entity.DICDomain model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Insert into Dictionary(Code,Name,ListName,Sort,Remark) Values");
            strSql.Append("(@Code,@Name,@ListName,@Sort,@Remark)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@ListName",model.ListName),
                new SqlParameter("@Sort",model.Sort),
                new SqlParameter("@Remark",model.Remark)
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, System.Data.CommandType.Text, strSql.ToString(), param);
                if (result > 0)
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

                throw ex;
            }
        }
        #endregion

        #region 修改一条数据
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Update(Common.Entity.DICDomain model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update Dictionary set Name=@Name where Code=@Code ");
            SqlParameter[] param = {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@Code",model.Code)
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 删除一条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="code">域编码</param>
        /// <returns></returns>
        public bool DeleteDicDomainByCode(string code)
        {
            string sql = string.Format(@"delete from Dictionary where Code = @Code");
            SqlParameter[] param = {
                new SqlParameter("@Code",code)
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, sql, param);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据域编码查询实体数据
        /// <summary>
        /// 根据域编码查询实体数据
        /// </summary>
        /// <param name="code">域编码</param>
        /// <returns></returns>
        public Common.Entity.DICDomain GetDicDomainByCode(string code)
        {
            string sql = string.Format(@"select * from Dictionary where Code=@Code");
            SqlParameter[] param = {
                new SqlParameter("@Code",code)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, System.Data.CommandType.Text, sql, param);
            Common.Entity.DICDomain model = new Common.Entity.DICDomain();
            if (dt.Rows.Count > 0)
            {
                model.PKID = Convert.ToInt32(dt.Rows[0]["PKID"]);
                model.Code = dt.Rows[0]["Code"].ToString();
                model.Name = dt.Rows[0]["Name"].ToString();
                model.ListName = dt.Rows[0]["ListName"].ToString();
                model.Sort = Convert.ToInt32(dt.Rows[0]["Sort"].ToString());
                model.Remark = dt.Rows[0]["Remark"].ToString();
            }
            return model;
        }
        #endregion

        #region 是否存在该列表名称
        /// <summary>
        /// 是否存在该列表名称
        /// </summary>
        /// <param name="pkid">主键ID</param>
        /// <param name="code">域编码</param>
        /// <param name="listName">列表名称</param>
        /// <returns>返回值</returns>
        public bool ExistsDICDomainItem(string code, string listName)
        {
            string sql = string.Format(@" select count(1) from Dictionary where Code=@Code and ListName=@ListName");
            SqlParameter[] param = {
                new SqlParameter("@Code",code),
                new SqlParameter("@ListName",listName)
            };
            int count = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, sql, param));
            if (count > 0)
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        public bool DeleteItemsByPKID(int PKID)
        {
            string sql = string.Format(@"delete from Dictionary where PKID = @PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, sql, param);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 根据PKID获取实体对象
        /// <summary>
        /// 根据PKID获取实体对象
        /// </summary>
        /// <param name="pkid">PKID</param>
        /// <returns>返回值</returns>
        public Common.Entity.DICDomain GetDicDomainByPKID(int pkid)
        {
            string sql = string.Format(@" select * from Dictionary where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",pkid)
            };
            try
            {
                Common.Entity.DICDomain model = new Common.Entity.DICDomain();
                DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
                if (dt.Rows.Count > 0)
                {
                    model.PKID = Convert.ToInt32(dt.Rows[0]["PKID"].ToString());
                    model.Code = dt.Rows[0]["Code"].ToString();
                    model.Name = dt.Rows[0]["Name"].ToString();
                    model.ListName = dt.Rows[0]["ListName"].ToString();
                    model.Sort = Convert.ToInt32(dt.Rows[0]["Sort"].ToString());
                    model.Remark = dt.Rows[0]["Remark"].ToString();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据PKID修改一条记录
        /// <summary>
        /// 根据PKID修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns>返回值</returns>
        public bool UpdateDomainItem(Common.Entity.DICDomain model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update Dictionary set ListName=@ListName,Sort=@Sort,Remark=@Remark where PKID=@PKID ");
            SqlParameter[] param = {
                new SqlParameter("@ListName",model.ListName),
                new SqlParameter("@Sort",model.Sort),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@PKID",model.PKID)
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取购买方式下拉列表
        /// <summary>
        /// 获取购买方式下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetBuyWay()
        {
            string sql = string.Format(@"select PKID,ListName from Dictionary where Code='BUYTYPE' order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取采购类型下拉列表
        /// <summary>
        /// 获取采购类型下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetPurchaseType()
        {
            string sql = string.Format(@"select PKID,ListName from Dictionary where Code='PURCHASETYPE' order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取记录状态下拉列表
        /// <summary>
        /// 获取采购类型下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetOrderState()
        {
            string sql = string.Format(@"select PKID,ListName from Dictionary where Code='ORDERSTATUS' order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取记录名称下拉列表
        /// <summary>
        /// 获取记录名称下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetRecordName()
        {
            string sql = string.Format(@"select PKID,ListName from Dictionary where Code='ORDERNAME' order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        /// <summary>
        /// 获取订单的订购车辆记录数
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public int GetDemandRecordsCount(int orderID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select count(1) from CarPurchase where OrderID=@OrderID");
            SqlParameter[] param = {
                new SqlParameter("@OrderID",orderID)
            };
            int result = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            if (result > 0)
                return result;
            else
                return result;
        }

        #region 获取信息列表类型
        /// <summary>
        /// 获取信息列表类型
        /// </summary>
        /// <returns></returns>
        public List<Common.Entity.DICDomain> GetTypeListJson()
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format("select PKID, ListName from Dictionary where Code='INFOTYPE'");
                List<Common.Entity.DICDomain> list = new List<Common.Entity.DICDomain>();
                list = conn.Query<Common.Entity.DICDomain>(sql).ToList();
                return list;
            }
        }
        #endregion

        #region 获取信息列表紧急程度
        /// <summary>
        /// 获取信息列表紧急程度
        /// </summary>
        /// <returns></returns>
        public List<Common.Entity.DICDomain> GetEmergencyDegreeListJson()
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format("select PKID,ListName from Dictionary where Code='INFOURGENT'");
                List<Common.Entity.DICDomain> list = new List<Common.Entity.DICDomain>();
                list = conn.Query<Common.Entity.DICDomain>(sql).ToList();
                return list;
            }
        }
        #endregion
    }
}
