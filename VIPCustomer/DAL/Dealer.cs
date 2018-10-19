using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 数据访问层:DAL 创建日期:2017.09.05 创建人:zhao.jian
/// </summary>
namespace DAL
{
    public class Dealer
    {
        #region 获取经销店实体对象
        /// <summary>
        /// 获取经销店实体对象
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <returns>返回DataTable</returns>
        public Common.Entity.Dealer GetDealerByPKID(int dealerId)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"select * from Dealer DL
							                left join (select * from  (select c.CountryID,d.ParentID,d.PKID as CityId, c.大区 as Country,c.省份 as Province,d.City as City from 
							                (select b.PKID as ProvinceID,b.ParentID as CountryID,a.City as 大区,b.City as 省份 from
							                (select PKID,City from CityDictionary WHERE ParentID='0') a,
							                CityDictionary b where a.PKID=b.ParentID) c,
							                CityDictionary d where c.ProvinceID=d.ParentID) e) CT on  DL.CityId=CT.CityId where DL.PKID=@PKID");
                Common.Entity.Dealer model = new Common.Entity.Dealer();
                DynamicParameters param = new DynamicParameters();
                param.Add("PKID", dealerId);
                model = conn.Query<Common.Entity.Dealer>(sql, param).FirstOrDefault();
                return model;
            }
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into Dealer(Code,Name,OldName,BearUserId,RegionManagerUserId,CityId,Address,ZipCode,Phone,fax,Email,SalesTel,ServerTel,SystemEmail,SalesDepartment,SalesDepartmentTel,MaxCommissioner,MaxCommissionerTel) Values");
            strSql.Append("(@Code,@Name,@OldName,@BearUserId,@RegionManagerUserId,@CityId,@Address,@ZipCode,@Phone,@fax,@Email,@SalesTel,@ServerTel,@SystemEmail,@SalesDepartment,@SalesDepartmentTel,@MaxCommissioner,@MaxCommissionerTel)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@OldName",model.OldName),
                new SqlParameter("@BearUserId",model.BearUserId),
                new SqlParameter("@RegionManagerUserId",model.RegionManagerUserId),
                new SqlParameter("@CityId",model.CityId),
                new SqlParameter("@Address",model.Address),
                new SqlParameter("@ZipCode",model.ZipCode),
                new SqlParameter("@Phone",model.Phone),
                new SqlParameter("@fax",model.fax),
                new SqlParameter("@Email",model.Email),
                new SqlParameter("@SalesTel",model.SalesTel),
                new SqlParameter("@ServerTel",model.ServerTel),
                new SqlParameter("@SystemEmail",model.SystemEmail),
                new SqlParameter("@SalesDepartment",model.SalesDepartment),
                new SqlParameter("@SalesDepartmentTel",model.SalesDepartmentTel),
                new SqlParameter("@MaxCommissioner",model.MaxCommissioner),
                new SqlParameter("@MaxCommissionerTel",model.MaxCommissionerTel),
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

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public bool Update(Common.Entity.Dealer model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update Dealer set Code=@Code,Name=@Name,OldName=@OldName,BearUserId=@BearUserId,RegionManagerUserId=@RegionManagerUserId,CityId=@CityId,Address=@Address,
                          ZipCode=@ZipCode,Phone=@Phone,fax=@fax,Email=@Email,SalesTel=@SalesTel,ServerTel=@ServerTel,SystemEmail=@SystemEmail,SalesDepartment=@SalesDepartment,
                          SalesDepartmentTel=@SalesDepartmentTel,MaxCommissioner=@MaxCommissioner,MaxCommissionerTel=@MaxCommissionerTel where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@OldName",model.OldName),
                new SqlParameter("@BearUserId",model.BearUserId),
                new SqlParameter("@RegionManagerUserId",model.RegionManagerUserId),
                new SqlParameter("@CityId",model.CityId),
                new SqlParameter("@Address",model.Address),
                new SqlParameter("@ZipCode",model.ZipCode),
                new SqlParameter("@Phone",model.Phone),
                new SqlParameter("@fax",model.fax),
                new SqlParameter("@Email",model.Email),
                new SqlParameter("@SalesTel",model.SalesTel),
                new SqlParameter("@ServerTel",model.ServerTel),
                new SqlParameter("@SystemEmail",model.SystemEmail),
                new SqlParameter("@SalesDepartment",model.SalesDepartment),
                new SqlParameter("@SalesDepartmentTel",model.SalesDepartmentTel),
                new SqlParameter("@MaxCommissioner",model.MaxCommissioner),
                new SqlParameter("@MaxCommissionerTel",model.MaxCommissionerTel),
                 new SqlParameter("@PKID",model.PKID),
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

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="PKID">经销店ID</param>
        /// <returns></returns>
        public bool DELETEDATE(int PKID)
        {
            string sql = string.Format(@"delete from Dealer where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID),
            };
            int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, sql, param);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" select count(1) from Dealer where Code=@Code or Name=@Name");
            SqlParameter[] paramer = {
                new SqlParameter("@Code",code),
                new SqlParameter("@Name",name)
            };
            int count = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paramer));
            try
            {
                if (count > 0)
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

        #region 根据经销店编号查询经销店信息
        /// <summary>
        /// 根据经销店编号查询经销店信息
        /// </summary>
        /// <param name="code">经销店编号</param>
        /// <returns>返回值</returns>
        public Common.Entity.Dealer GetDealerByCode(string code)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"select* from Dealer DL
                                            left join (select * from(select c.大区ID, d.ParentID as 省份ID, d.PKID as 城市ID, c.大区 as 大区, c.省份 as 省份, d.City as 城市 from
                                            (select b.PKID as 省份ID, b.ParentID as 大区ID, a.City as 大区, b.City as 省份 from
                                            (select PKID, City from CityDictionary WHERE ParentID = '0') a,
                                            CityDictionary b where a.PKID = b.ParentID) c,
                                            CityDictionary d where c.省份ID = d.ParentID) e) CT on  DL.CityId = CT.城市ID where DL.Code = @Code");
                Common.Entity.Dealer model = new Common.Entity.Dealer();
                DynamicParameters param = new DynamicParameters();
                param.Add("Code", code);
                model = conn.Query<Common.Entity.Dealer>(sql, param).FirstOrDefault();
                return model;
            }
        }
        #endregion

        public Common.Entity.DealerRepControl GetLastReportDate(int dealerID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"  select * from DealerRepControl where DealerId=@DealerId and Convert(char(6),LastRepDateTime,112)=@LastRepDateTime");
            SqlParameter[] param = {
                new SqlParameter("@DealerId",dealerID),
                new SqlParameter("@LastRepDateTime",DateTime.Now.ToString("yyyyMM"))
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            Common.Entity.DealerRepControl model = null;
            if (dt.Rows.Count > 0)
            {
                model = new Common.Entity.DealerRepControl();
                model.DealerId = Convert.ToInt32(dt.Rows[0]["DealerId"]);
                model.DealerCode = dt.Rows[0]["DealerCode"].ToString();
                model.LastRepDateTime = Convert.ToDateTime(dt.Rows[0]["LastRepDateTime"].ToString());
                model.InvoiceDateTime = Convert.ToDateTime(dt.Rows[0]["InvoiceDateTime"].ToString());
                return model;
            }
            return model;
        }

        #region 获取系统可以上报最晚数据的时间限制
        /// <summary>
        /// 获取系统可以上报最晚数据的时间限制
        /// </summary>
        /// <param name="dealerID">经销店ID</param>
        /// <returns></returns>
        public Common.Entity.DealerRepControl GetLastReportDateSet(int dealerID)
        {
            var result = new DAL.Dealer().GetLastReportDate(dealerID);
            if (null != result)
            {
                if (result.LastRepDateTime >= DateTime.Now.Date)
                    return result;
            }
            Common.Entity.DealerRepControl cResult = new Common.Entity.DealerRepControl();
            int setDay = int.Parse(System.Configuration.ConfigurationManager.AppSettings["defaultReportCarDay"]);
            cResult.LastRepDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, setDay).AddDays(-1);
            cResult.InvoiceDateTime = cResult.LastRepDateTime.AddDays(1 - cResult.LastRepDateTime.Day);
            cResult.InvoiceDateTime = cResult.LastRepDateTime < DateTime.Now.Date ? cResult.InvoiceDateTime : cResult.InvoiceDateTime.AddMonths(-1);
            return cResult;
        }
        #endregion

        #region 判断是否已经存在经销店最后上报日期记录
        /// <summary>
        /// 判断是否已经存在经销店最后上报日期记录
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <returns></returns>
        public bool ExistsLastReportDate(int dealerId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"  select count(1) from DealerRepControl where DealerId=@DealerId");
            SqlParameter[] paramer = {
                new SqlParameter("@DealerId",dealerId)
            };
            int count = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paramer));
            try
            {
                if (count > 0)
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

        #region 更新经销店最后上报日期
        /// <summary>
        /// 更新经销店最后上报日期
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <param name="LastRepDateTime">最后上报日期</param>
        /// <param name="InvoiceDateTime">发票限制日期</param>
        /// <returns></returns>
        public bool UpdateLastReportDate(int dealerId, DateTime LastRepDateTime, DateTime InvoiceDateTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update DealerRepControl set LastRepDateTime=@LastRepDateTime,InvoiceDateTime=@InvoiceDateTime,DealerId=@DealerId");
            SqlParameter[] param = {
                new SqlParameter("@LastRepDateTime",LastRepDateTime),
                new SqlParameter("@InvoiceDateTime",InvoiceDateTime),
                new SqlParameter("@DealerId",dealerId)
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

        #region 添加经销店最后上报日期
        /// <summary>
        /// 添加经销店最后上报日期
        /// </summary>
        /// <param name="dealerId">经销店ID</param>
        /// <param name="code">经销店编号</param>
        /// <param name="LastRepDateTime">最后上报日期</param>
        /// <param name="InvoiceDateTime">发票限制日期</param>
        /// <returns></returns>
        public bool AddLastReportDate(int dealerId, string code, DateTime LastRepDateTime, DateTime InvoiceDateTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into DealerRepControl(DealerId,DealerCode,LastRepDateTime,InvoiceDateTime) Values");
            strSql.Append("(@DealerId,@DealerCode,@LastRepDateTime,@InvoiceDateTime)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@DealerId",dealerId),
                new SqlParameter("@DealerCode",code),
                new SqlParameter("@LastRepDateTime",LastRepDateTime),
                new SqlParameter("@InvoiceDateTime",InvoiceDateTime),
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
                if (result > 0)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            if (ExistsLastReportDate(dealerId))
            {
                int result = Convert.ToInt32(new DAL.Dealer().UpdateLastReportDate(dealerId, DateTime.Parse(LastRepDateTime), DateTime.Parse(InvoiceDateTime)));
                if (result > 0)
                {
                    return result;
                }
                return result;
            }
            else
            {
                int result = Convert.ToInt32(new DAL.Dealer().AddLastReportDate(dealerId, code, DateTime.Parse(LastRepDateTime), DateTime.Parse(InvoiceDateTime)));
                if (result > 0)
                {
                    return result;
                }
                return result;
            }
        }
        #endregion

        #region 批量删除客户信息
        /// <summary>
        /// 批量删除客户信息
        /// </summary>
        /// <param name="customerIds">客户ID</param>
        /// <returns>返回结果</returns>
        public bool DeleteData(string customerIds)
        {
            string[] strCustomerIDs = customerIds.Split(',');
            int result = 0;
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                for (int i = 0; i < strCustomerIDs.Length; i++)
                {
                    string sql = $"delete from Dealer where PKID={strCustomerIDs[i]}";
                    result = conn.Execute(sql);
                }
            }
            if (result > 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
