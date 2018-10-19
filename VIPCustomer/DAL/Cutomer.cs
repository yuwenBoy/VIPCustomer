using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
namespace DAL
{
    public class Customer
    {
        #region 获取客户性质一级列表 
        /// <summary>
        /// 获取客户性质一级列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetList()
        {
            string sql = string.Format(@"select  distinct PKID,Name,ParentID,Sort  from CustomerNature where ParentID=0 and IsActivate=1 order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取客户性质二级列表
        /// <summary>
        /// 获取客户性质二级列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomerContact2(int contactId)
        {
            string sql = string.Format(@"select  distinct PKID,Name,ParentID,Sort  
                                       from CustomerNature where ParentID=@contactId and IsActivate=1 order by Sort");
            SqlParameter[] param = {
                new SqlParameter("@contactId",contactId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取客户联系人角色列表
        /// <summary>
        /// 获取客户联系人角色列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetJobList()
        {
            string sql = string.Format(@"select PKID,ListName from Dictionary where Code='CUSTOMERCONTACTROLE' order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else return dt;
        }
        #endregion

        #region 获取客户信息实体对象
        /// <summary>
        /// 获取客户信息实体对象
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <returns>返回实体类</returns>
        public Common.Entity.Customer GetCustomerByPKID(int customerId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"select B.Name CustomerNatureName2,D.Name DealerName, A.* from 
					        (select CN.Name as CustomerNatureName,C.* from Customer C left join CustomerNature CN on C.CustomerNatureID=CN.PKID) A 
                            left join (select * from CustomerNature CN) B on A.CustomerNature2ID=B.PKID 
                            left join Dealer D on A.DealerID=D.PKID where A.PKID={customerId}");
            Common.Entity.Customer customer = new Common.Entity.Customer();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                customer = conn.Query<Common.Entity.Customer>(strSql.ToString()).FirstOrDefault();
            }
            return customer;
        }
        #endregion

        #region 添加客户
        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="model">客户实体</param>
        /// <param name="listContact">客户联系人实体</param>
        /// <returns>返回值</returns>
        public int AddCustomer(Common.Entity.Customer model, List<Common.Entity.CustomerContact> listContact)
        {
            Hashtable sqlStringListAdd = new Hashtable();
            int result = 0;
            if (model != null)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"Insert into Customer(DealerID,EnterpriseCode,CreateTime,Name,Address,Zip,CustomerNatureID,CustomerNature2ID,OldCustomerNatureID,CustomerTypeID,
                                Phone,Fax,Eamil,WebSite,CompetentDepartment,ExecutiveDepartment,UseDepartment,MainBusiness,IndustryStatus,CustomerProfiles,Remark,CustomerPrimaryID) Values");
                strSql.Append(@"(@DealerID,@EnterpriseCode,@CreateTime,@Name,@Address,@Zip,@CustomerNatureID,@CustomerNature2ID,@OldCustomerNatureID,@CustomerTypeID,
                                @Phone,@Fax,@Eamil,@WebSite,@CompetentDepartment,@ExecutiveDepartment,@UseDepartment,@MainBusiness,@IndustryStatus,@CustomerProfiles,@Remark,@CustomerPrimaryID)");
                strSql.Append("  SELECT @@IDENTITY");// 返回插入用户的主键
                SqlParameter[] param = {
                    new SqlParameter("@DealerID",model.DealerID),
                    new SqlParameter("@EnterpriseCode",model.EnterpriseCode),
                    new SqlParameter("@CreateTime",model.CreateTime),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@Address",model.Address),
                    new SqlParameter("@Zip",model.Zip),
                    new SqlParameter("@CustomerNatureID",model.CustomerNatureID),
                    new SqlParameter("@CustomerNature2ID",model.CustomerNature2ID),
                    new SqlParameter("@OldCustomerNatureID",model.OldCustomerNatureID),
                    new SqlParameter("@CustomerTypeID",model.CustomerTypeID),
                    new SqlParameter("@Phone",model.Phone),
                    new SqlParameter("@Fax",model.Fax),
                    new SqlParameter("@Eamil",model.Eamil),
                    new SqlParameter("@WebSite",model.WebSite),
                    new SqlParameter("@CompetentDepartment",model.CompetentDepartment),
                    new SqlParameter("@ExecutiveDepartment",model.ExecutiveDepartment),
                    new SqlParameter("@UseDepartment",model.UseDepartment),
                    new SqlParameter("@MainBusiness",model.MainBusiness),
                    new SqlParameter("@IndustryStatus",model.IndustryStatus),
                    new SqlParameter("@CustomerProfiles",model.CustomerProfiles),
                    new SqlParameter("@Remark",model.Remark),
                    new SqlParameter("@CustomerPrimaryID",model.CustomerPrimaryID),
                };
                model.PKID = SqlHelper.ExecuteReturnID(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            }
            if (listContact != null && listContact.Count > 0)
            {
                foreach (Common.Entity.CustomerContact contatcEntity in listContact)
                {
                    contatcEntity.CustomerID = model.PKID;
                    StringBuilder strSqlContact = new StringBuilder();
                    strSqlContact.Append(@"Insert into CustomerContact(CustomerID,Name,Job,RoleID,Phone,
                                 MobileTel,Fax,Email,OtherContactInfo,Birthday,Hobbies,Remark) Values");
                    strSqlContact.Append(@"(@CustomerID,@Name,@Job,@RoleID,@Phone,@MobileTel,@Fax,@Email,
                                   @OtherContactInfo,@Birthday,@Hobbies,@Remark)");
                    strSqlContact.Append(" SELECT @@IDENTITY");// 返回插入用户的主键
                    SqlParameter[] paramContact = {
                    new SqlParameter("@CustomerID",contatcEntity.CustomerID),
                    new SqlParameter("@Name",contatcEntity.Name),
                    new SqlParameter("@Job",contatcEntity.Job),
                    new SqlParameter("@RoleID",contatcEntity.RoleID),
                    new SqlParameter("@Phone",contatcEntity.Phone),
                    new SqlParameter("@MobileTel",contatcEntity.MobileTel),
                    new SqlParameter("@Fax",contatcEntity.Fax),
                    new SqlParameter("@Email",contatcEntity.Email),
                    new SqlParameter("@OtherContactInfo",contatcEntity.OtherContactInfo),
                    new SqlParameter("@Birthday",contatcEntity.Birthday),
                    new SqlParameter("@Hobbies",contatcEntity.Hobbies),
                    new SqlParameter("@Remark",contatcEntity.Remark),
                    };
                    sqlStringListAdd.Add(strSqlContact, paramContact);
                }
            }
            try
            {
                result = Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, sqlStringListAdd));
                if (result > 0)
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {

                throw;
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
            StringBuilder strSql = new StringBuilder();
            for (int i = 0; i < strCustomerIDs.Length; i++)
            {
                strSql.Append(@"delete from Customer where PKID=" + strCustomerIDs[i]);
            }
            result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
            if (result > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region 是否存在该记录
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="EnterpriseCode">企业代码</param>
        /// <returns>返回值</returns>
        public bool Exists(string EnterpriseCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select count(1) from Customer where EnterpriseCode=@EnterpriseCode");
            SqlParameter[] param = {
                new SqlParameter("@EnterpriseCode",EnterpriseCode)
            };
            int result = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 根据客户ID查询客户联系人
        /// <summary>
        /// 根据客户ID查询客户联系人
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <returns>返回DataTable</returns>
        public DataTable GetCustomerContactByCustomerId(int customerId)
        {
            string sql = string.Format(@"select * from CustomerContact where CustomerID=@CustomerID");
            SqlParameter[] param = {
                new SqlParameter("@CustomerID",customerId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            return dt;
        }
        #endregion

        #region 编辑客户
        /// <summary>
        /// 编辑客户
        /// </summary>
        /// <param name="customer">客户实体</param>
        /// <param name="DeleteContactList">客户联系人</param>
        /// <returns></returns>
        public bool Update(Common.Entity.Customer customer, List<Common.Entity.CustomerContact> list, List<Common.Entity.CustomerContact> Dellist, List<Common.Entity.CustomerContact> UpdateList)
        {
            try
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    Hashtable strStringList = new Hashtable();
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"Update Customer set DealerID=@DealerID,EnterpriseCode=@EnterpriseCode,CreateTime=@CreateTime,Name=@Name,Address=@Address,Zip=@Zip,CustomerNatureID=@CustomerNatureID,
                                  CustomerNature2ID=@CustomerNature2ID,OldCustomerNatureID=@OldCustomerNatureID,CustomerTypeID=@CustomerTypeID,Phone=@Phone,Fax=@Fax,Eamil=@Eamil,WebSite=@WebSite,CompetentDepartment=@CompetentDepartment,
                                  ExecutiveDepartment=@ExecutiveDepartment,UseDepartment=@UseDepartment,MainBusiness=@MainBusiness,IndustryStatus=@IndustryStatus,CustomerProfiles=@CustomerProfiles,
                                  Remark=@Remark,CustomerPrimaryID=@CustomerPrimaryID where PKID=@PKID");
                    SqlParameter[] param = {
                            new SqlParameter("@PKID",customer.PKID),
                            new SqlParameter("@DealerID",customer.DealerID),
                            new SqlParameter("@EnterpriseCode",customer.EnterpriseCode),
                            new SqlParameter("@CreateTime",customer.CreateTime),
                            new SqlParameter("@Name",customer.Name),
                            new SqlParameter("@Address",customer.Address),
                            new SqlParameter("@Zip",customer.Zip),
                            new SqlParameter("@CustomerNatureID",customer.CustomerNatureID),
                            new SqlParameter("@CustomerNature2ID",customer.CustomerNature2ID),
                            new SqlParameter("@OldCustomerNatureID",customer.OldCustomerNatureID),
                            new SqlParameter("@CustomerTypeID",customer.CustomerTypeID),
                            new SqlParameter("@Phone",customer.Phone),
                            new SqlParameter("@Fax",customer.Fax),
                            new SqlParameter("@Eamil",customer.Eamil),
                            new SqlParameter("@WebSite",customer.WebSite),
                            new SqlParameter("@CompetentDepartment",customer.CompetentDepartment),
                            new SqlParameter("@ExecutiveDepartment",customer.ExecutiveDepartment),
                            new SqlParameter("@UseDepartment",customer.UseDepartment),
                            new SqlParameter("@MainBusiness",customer.MainBusiness),
                            new SqlParameter("@IndustryStatus",customer.IndustryStatus),
                            new SqlParameter("@CustomerProfiles",customer.CustomerProfiles),
                            new SqlParameter("@Remark",customer.Remark),
                            new SqlParameter("@CustomerPrimaryID",customer.CustomerPrimaryID),
                        };
                    strStringList.Add(strSql, param);
                    //删除
                    if (Dellist.Count > 0)
                    {
                        for (int i = 0; i < Dellist.Count; i++)// 删除的用户角色
                        {
                            StringBuilder strDeleteSql = new StringBuilder();
                            strDeleteSql.Append(@"delete from CustomerContact where PKID=@PKID");
                            SqlParameter[] paramer1 = {
                    new SqlParameter("@PKID",SqlDbType.Int,1000)
                };
                            paramer1[0].Value = Dellist[i].PKID;
                            strStringList.Add(strDeleteSql, paramer1);
                        }
                    }
                    //新增
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            StringBuilder strInsertSql = new StringBuilder();
                            strInsertSql.Append(@"Insert into CustomerContact(CustomerID,Name,Job,RoleID,Phone,
                                             MobileTel,Fax,Email,OtherContactInfo,Birthday,Hobbies,Remark) Values");
                            strInsertSql.Append(@"(@CustomerID,@Name,@Job,@RoleID,@Phone,@MobileTel,@Fax,@Email,
                                               @OtherContactInfo,@Birthday,@Hobbies,@Remark)");
                            strInsertSql.Append(" SELECT @@IDENTITY");// 返回插入用户的主键
                            SqlParameter[] paramer2 = {
                                new SqlParameter("@CustomerID",item.CustomerID),
                                new SqlParameter("@Name",item.Name),
                                new SqlParameter("@Job",item.Job),
                                new SqlParameter("@RoleID",item.RoleID),
                                new SqlParameter("@Phone",item.Phone),
                                new SqlParameter("@MobileTel",item.MobileTel),
                                new SqlParameter("@Fax",item.Fax),
                                new SqlParameter("@Email",item.Email),
                                new SqlParameter("@OtherContactInfo",item.OtherContactInfo),
                                new SqlParameter("@Birthday",item.Birthday),
                                new SqlParameter("@Hobbies",item.Hobbies),
                                new SqlParameter("@Remark",item.Remark),
                                };
                            strStringList.Add(strInsertSql, paramer2);
                        }
                    }
                    //编辑
                    if (UpdateList.Count > 0)
                    {
                        foreach (var item in UpdateList)
                        {
                            StringBuilder strUpdateSql = new StringBuilder();
                            strUpdateSql.Append(@"update  CustomerContact set Name=@Name,Job=@Job,RoleID=@RoleID,Phone=@Phone,
                                                 MobileTel=@MobileTel where PKID=@PKID");
                            SqlParameter[] paramer3 = {
                                new SqlParameter("@PKID",item.PKID),
                                new SqlParameter("@Name",item.Name),
                                new SqlParameter("@Job",item.Job),
                                new SqlParameter("@RoleID",item.RoleID),
                                new SqlParameter("@Phone",item.Phone),
                                new SqlParameter("@MobileTel",item.MobileTel),
                                };
                            strStringList.Add(strUpdateSql, paramer3);
                        }
                    }
                    int result = Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, strStringList));
                    tran.Complete();
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch 
            {
                throw;
            }
        }
        #endregion


        /// <summary>
        /// 获取客户联系人记录数
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public int GetCustomerContactCount(int customerID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select count(1) from CustomerContact where CustomerID=@CustomerID");
            SqlParameter[] param = {
                new SqlParameter("@CustomerID",customerID)
            };
            int result = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            if (result > 0)
            {
                return result;
            }
            else
                return result;
        }
    }
}
