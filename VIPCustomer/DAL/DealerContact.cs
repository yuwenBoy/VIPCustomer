using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DealerContact
    {
        #region 获取职务列表
        /// <summary>
        /// 获取职务列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetJobList()
        {
            string sql = string.Format(@"select PKID as JobID,ListName as JobName from Dictionary where Code='DEALERCONTACTPOST' order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
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
        public int Add(Common.Entity.DealerContact model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into DealerContact(DealerID,Name,JobID,Phone,MobileTel,Fax,Email,OtherContactInfo,Remark) Values");
            strSql.Append("(@DealerID,@Name,@JobID,@Phone,@MobileTel,@Fax,@Email,@OtherContactInfo,@Remark)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@DealerID",model.DealerID),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@JobID",model.JobID),
                new SqlParameter("@Phone",model.Phone),
                new SqlParameter("@MobileTel",model.MobileTel),
                new SqlParameter("@Fax",model.Fax),
                new SqlParameter("@Email",model.Email),
                new SqlParameter("@OtherContactInfo",model.OtherContactInfo),
                new SqlParameter("@Remark",model.Remark),
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
        /// <param name="name">姓名</param>
        /// <param name="mobileTel">移动电话</param>
        /// <returns></returns>
        public bool Exists(string name, string mobileTel)
        {
            string sql = string.Format(@"select count(1) from DealerContact where Name=@name and MobileTel=@mobileTel");
            SqlParameter[] param = {
                new SqlParameter("@name",name),
                new SqlParameter("@mobileTel",mobileTel)
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
        /// <param name="PKID">PKID</param>
        /// <returns></returns>
        public Common.Entity.DealerContact GetDealerContactByPKID(int PKID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select DL.Name as 所属经销店,DT.ListName as 职务,
                    DC.* from DealerContact DC
                    left join Dealer DL on DC.DealerID=DL.PKID
                    left join Dictionary DT on DC.JobID=DT.PKID where DC.PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            Common.Entity.DealerContact model = null;
            if (dt.Rows.Count > 0)
            {
                model = new Common.Entity.DealerContact();
                model.PKID = Convert.ToInt32(dt.Rows[0]["PKID"]);
                model.DealerID = Convert.ToInt32(dt.Rows[0]["DealerID"]);
                model.Name = dt.Rows[0]["Name"].ToString();
                model.JobID = Convert.ToInt32(dt.Rows[0]["JobID"]);
                model.DealerName = dt.Rows[0]["所属经销店"].ToString();
                model.JobName = dt.Rows[0]["职务"].ToString();
                model.Phone = dt.Rows[0]["Phone"].ToString();
                model.Email = dt.Rows[0]["Email"].ToString();
                model.OtherContactInfo = dt.Rows[0]["OtherContactInfo"].ToString();
                model.MobileTel = dt.Rows[0]["MobileTel"].ToString();
                model.Fax = dt.Rows[0]["Fax"].ToString();
                model.Remark = dt.Rows[0]["Remark"].ToString();
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
        public bool Update(Common.Entity.DealerContact model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update DealerContact set Name=@Name,JobID=@JobID,Phone=@Phone,
                          MobileTel=@MobileTel,Fax=@Fax,Email=@Email,OtherContactInfo=@OtherContactInfo,
                          Remark=@Remark where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",model.PKID),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@JobID",model.JobID),
                new SqlParameter("@Phone",model.Phone),
                new SqlParameter("@MobileTel",model.MobileTel),
                new SqlParameter("@Fax",model.Fax),
                new SqlParameter("@Email",model.Email),
                new SqlParameter("@OtherContactInfo",model.OtherContactInfo),
                new SqlParameter("@Remark",model.Remark),
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
        /// <param name="PKID">经销店联系人ID</param>
        /// <returns></returns>
        public bool DELETEContactDATE(int PKID)
        {
            string sql = string.Format(@"delete from DealerContact where PKID=@PKID");
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

        #region 根据订单ID获取订单经销店联系人列表
        /// <summary>
        /// 根据订单ID获取订单经销店联系人列表
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>返回数据</returns>
        public List<Common.Entity.OrderDealerContact> GetListByOrdersID(string orderId)
        {
            string sql = string.Format(@"SELECT ODC.Name,DT.ListName 职务,ODC.Phone,ODC.MobileTel,ODC.Fax,ODC.Email,
                                        ODC.OtherContactInfo FROM OrderDealerContact ODC LEFT JOIN Dictionary DT ON ODC.JobID= DT.PKID
                                        where ODC.OrderID=@DealerID");
            SqlParameter[] param = {
                new SqlParameter("@DealerID",orderId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
            List<Common.Entity.OrderDealerContact> list = new List<Common.Entity.OrderDealerContact>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Common.Entity.OrderDealerContact model = new Common.Entity.OrderDealerContact();
                    model.PKID = Convert.ToInt32(dt.Rows[i]["PKID"]);
                    model.Name = dt.Rows[i]["Name"].ToString();
                    model.Phone = dt.Rows[i]["Phone"].ToString();
                    model.MobileTel = dt.Rows[i]["MobileTel"].ToString();
                    model.Email = dt.Rows[i]["Email"].ToString();
                    model.JobName = dt.Rows[i]["职务"].ToString();
                    model.OtherContactInfo = dt.Rows[i]["OtherContactInfo"].ToString();
                    list.Add(model);
                }
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select count(1) from DealerContact where DealerID=@DealerID");
            SqlParameter[] param = {
                new SqlParameter("@DealerID",dealerId)
            };

            int result = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            if (result > 0)
                return result;
            else
                return 0;
        }
    }
}
