using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace BLL
{
    public class Customer
    {
        /// <summary>
        /// 根据客户id查询客户联系人信息
        /// </summary>
        /// <returns>返回list</returns>
        public List<Common.Entity.CustomerContact> GetListByCustomerID(string customerId)
        {
            string sql = string.Format($@"select CC.PKID,CC.CustomerID,CC.Name,CC.Job,DT.ListName as RoleName,CC.Phone,CC.MobileTel,CC.Fax
                                            ,CC.Email,CC.OtherContactInfo,CC.Birthday,CC.Hobbies
                                            from CustomerContact CC 
                                            left join Dictionary DT on CC.RoleID=DT.PKID where CC.CustomerID={customerId}");
            List<Common.Entity.CustomerContact> list = new List<Common.Entity.CustomerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.CustomerContact>(sql).ToList();
            };
            return list;
        }

        /// <summary>
        /// 根据客户id查询客户联系人信息
        /// </summary>
        /// <returns>返回list</returns>
        public List<Common.Entity.OrderCustomerContact> GetListByCustomerID(int customerId)
        {
            string sql = string.Format($@"select CC.PKID,CC.CustomerID,CC.Name,CC.Job,DT.ListName as RoleName,CC.Phone,CC.MobileTel,CC.Fax
                                            ,CC.Email,CC.OtherContactInfo,CC.Birthday,CC.Hobbies
                                            from CustomerContact CC 
                                            left join Dictionary DT on CC.RoleID=DT.PKID where CC.CustomerID=@PKID");
            List<Common.Entity.OrderCustomerContact> list = new List<Common.Entity.OrderCustomerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.OrderCustomerContact>(sql, new { PKID = customerId }).ToList();
            };
            return list;
        }

        /// <summary>
        /// 根据订单id查询订单客户联系人信息
        /// </summary>
        /// <returns>返回list</returns>
        public List<Common.Entity.OrderCustomerContact> GetListByOrdersID(string orderId)
        {
            string sql = string.Format($@"select CC.PKID,CC.OrderID,CC.Name,CC.Job,DT.ListName as RoleName,CC.Phone,CC.MobileTel,CC.Fax
                                            ,CC.Email,CC.OtherContactInfo,CC.Birthday,CC.Hobbies
                                            from OrderCustomerContact CC 
                                            left join Dictionary DT on CC.RoleID=DT.PKID where CC.OrderID={orderId}");
            List<Common.Entity.OrderCustomerContact> list = new List<Common.Entity.OrderCustomerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.OrderCustomerContact>(sql).ToList();
            }
            return list;
        }

        /// <summary>
        /// 根据订单id查询订单客户联系人信息
        /// </summary>
        /// <returns>返回list</returns>
        public List<Common.Entity.OrderCustomerContact> GetListByOrdersID(int orderId)
        {
            string sql = string.Format($@"select CC.PKID,CC.OrderID,CC.Name,CC.Job,DT.ListName as RoleName,CC.Phone,CC.MobileTel,CC.Fax
                                            ,CC.Email,CC.OtherContactInfo,CC.Birthday,CC.Hobbies
                                            from OrderCustomerContact CC 
                                            left join Dictionary DT on CC.RoleID=DT.PKID where CC.OrderID=@OrderID");
            List<Common.Entity.OrderCustomerContact> list = new List<Common.Entity.OrderCustomerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.OrderCustomerContact>(sql, new { OrderID = orderId }).ToList();
            }
            return list;
        }

        #region 从服务器端获取分页列表
        /// <summary>
        /// 从服务器端获取分页列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>返回DataTable</returns>
        public List<Common.Entity.Customer> GetDealerManagerPager(int pageIndex, int pageSize, int dealerId, Common.Entity.Customer search, out int totalCount)
        {
            StringBuilder sqlCount = new StringBuilder();
            sqlCount.Append($@"select count(1) from (select distinct b.PKID 父级ID,b.Name 父级名称,c.* from CustomerNature a INNER JOIN 
                            (select * from CustomerNature where ParentID='0') b
                            ON a.ParentID=b.PKID
                            INNER JOIN Customer c on b.PKID=c.CustomerNatureID and c.DealerID={dealerId}
                            INNER JOIN Dealer d on c.DealerID=d.PKID where c.PKID>0");
            if (!string.IsNullOrEmpty(search.EnterpriseCode))
            {
                sqlCount.Append(" and c.EnterpriseCode like '%" + search.EnterpriseCode + "%'");
            }
            if (!string.IsNullOrEmpty(search.Name))
            {
                sqlCount.Append(" and c.Name like '%" + search.Name + "%'");
            }
            if (search.CustomerNatureID > 0)
            {
                sqlCount.Append(" and c.CustomerNatureID=" + search.CustomerNatureID);
            }
            if (search.CustomerNature2ID > 0)
            {
                sqlCount.Append(" and c.CustomerNature2ID=" + search.CustomerNature2ID);
            }
            sqlCount.Append(" ) A");
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"select top ({pageSize}) * from (
                            select  row_number() over (order by S.Name) as rowNumber,S.*  from
                           (select distinct  b.PKID 父级ID,d.Name as DealerName,b.Name CustomerNatureName,
                            c.* from CustomerNature a INNER JOIN 
                            (select * from CustomerNature where ParentID='0') b
                            ON a.ParentID=b.PKID INNER JOIN Customer c 
                            on b.PKID=c.CustomerNatureID and c.DealerID={dealerId} INNER JOIN Dealer d 
                            on c.DealerID=d.PKID where c.PKID>0");
            if (!string.IsNullOrEmpty(search.EnterpriseCode))
            {
                strSql.Append(" and c.EnterpriseCode like '%" + search.EnterpriseCode + "%'");
            }
            if (!string.IsNullOrEmpty(search.Name))
            {
                strSql.Append(" and c.Name like '%" + search.Name + "%'");
            }
            if (search.CustomerNatureID > 0)
            {
                strSql.Append(" and c.CustomerNatureID=" + search.CustomerNatureID);
            }
            if (search.CustomerNature2ID > 0)
            {
                strSql.Append(" and c.CustomerNature2ID=" + search.CustomerNature2ID);
            }
            strSql.Append($" ) as S) as T where T.rowNumber>({pageIndex}-1)*{pageSize}");
            List<Common.Entity.Customer> list = new List<Common.Entity.Customer>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                totalCount = conn.Query<int>(sqlCount.ToString()).FirstOrDefault();
                list = conn.Query<Common.Entity.Customer>(strSql.ToString()).ToList();
            }
            return list;
        }
        #endregion

        #region 获取客户性质一级列表
        /// <summary>
        /// 获取省份或城市列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetList()
        {
            return new DAL.Customer().GetList();
        }
        #endregion

        #region 获取客户性质二级列表
        /// <summary>
        /// 获取客户性质二级列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomerContact2(int contactId)
        {
            return new DAL.Customer().GetCustomerContact2(contactId);
        }
        #endregion

        #region 根据客户ID获取客户联系人信息
        /// <summary>
        /// 根据客户ID获取客户联系人信息
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<Common.Entity.CustomerContact> GetCustomerContactsPager(string customerId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"select DC.ListName as RoleName,CC.* from CustomerContact CC left join Dictionary DC on CC.RoleID=DC.PKID where CC.CustomerID={customerId}");
            List<Common.Entity.CustomerContact> list = new List<Common.Entity.CustomerContact>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                list = conn.Query<Common.Entity.CustomerContact>(strSql.ToString()).ToList();
            };
            return list;
        }
        #endregion

        #region 获取客户联系人角色列表
        /// <summary>
        /// 获取客户联系人角色列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetJobList()
        {
            return new DAL.Customer().GetJobList();
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
            return new DAL.Customer().GetCustomerByPKID(customerId);
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
            return new DAL.Customer().AddCustomer(model, listContact);
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
            return new DAL.Customer().Exists(EnterpriseCode);
        }
        #endregion

        public DataTable GetCustomerContactByCustomerId(int cusId)
        {
            return new DAL.Customer().GetCustomerContactByCustomerId(cusId);
        }

        #region 编辑客户
        /// <summary>
        /// 编辑客户
        /// </summary>
        /// <param name="customer">客户实体</param>
        /// <param name="listContact">客户联系人</param>
        /// <returns>返回值</returns>
        public bool Update(Common.Entity.Customer customer, List<Common.Entity.CustomerContact> listContact)
        {
            DataTable dt = new BLL.Customer().GetCustomerContactByCustomerId(customer.PKID);
            List<Common.Entity.CustomerContact> AddList = new List<Common.Entity.CustomerContact>();
            List<Common.Entity.CustomerContact> DeleteList = new List<Common.Entity.CustomerContact>();
            List<Common.Entity.CustomerContact> UpdateList = new List<Common.Entity.CustomerContact>();
            Common.Entity.CustomerContact AddContact = null;
            Common.Entity.CustomerContact DeleteContact = null;
            Common.Entity.CustomerContact UpdateContact = null;
            if (dt.Rows.Count > 0)   //查询已删除添加至List
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    List<Common.Entity.CustomerContact> newlist = listContact.Where(x => x.PKID == dt.Rows[i]["PKID"].ToString()).ToList();
                    if (newlist.Count == 0)
                    {
                        DeleteContact = new Common.Entity.CustomerContact();
                        DeleteContact.PKID = dt.Rows[i]["PKID"].ToString();
                        DeleteList.Add(DeleteContact);
                    }
                }
            }
            for (int j = 0; j < listContact.Count; j++)
            {
                if (string.IsNullOrWhiteSpace(listContact[j].PKID))//原数据不存在新数据（新增）
                {
                    AddContact = new Common.Entity.CustomerContact();
                    AddContact.CustomerID = customer.PKID;
                    AddContact.Name = listContact[j].Name;
                    AddContact.Job = listContact[j].Job;
                    AddContact.RoleID = listContact[j].RoleID;
                    AddContact.Phone = listContact[j].Phone;
                    AddContact.MobileTel = listContact[j].MobileTel;
                    AddList.Add(AddContact);
                }
                else//源数据存在新数据（修改）
                {
                    UpdateContact = listContact[j];
                    UpdateList.Add(UpdateContact);
                }
            }
            return new DAL.Customer().Update(customer, AddList, DeleteList, UpdateList);
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
            return new DAL.Customer().DeleteData(customerIds);
        }
        #endregion

        #region 获取客户联系人记录数
        /// <summary>
        /// 获取客户联系人记录数
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public int GetCustomerContactCount(int customerID)
        {
            return new DAL.Customer().GetCustomerContactCount(customerID);
        }
        #endregion

        #region 从服务器端获取客户分页列表
        /// <summary>
        /// 从服务器端获取客户分页列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>返回DataTable</returns>
        public List<Common.Entity.Customer> GetCustomerPagerByDealerID(int pageIndex, int pageSize, int dealerId, string sortName, string IsDesc, string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from (select distinct b.PKID 父级ID,b.Name 父级名称,c.* from CustomerNature a INNER JOIN 
                                (select * from CustomerNature where ParentID='0') b
                                ON a.ParentID=b.PKID
                                INNER JOIN Customer c on b.PKID=c.CustomerNatureID and c.DealerID=@dealerId
                                INNER JOIN Dealer d on c.DealerID=d.PKID where c.PKID>0 ");
                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(" and c.EnterpriseCode like '%" + filter + "%' or c.Name like '%" + filter + "%'");
                }
                sqlCount.Append(" ) A");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("dealerId", dealerId);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (
                                select  row_number() over (order by S.Name) as rowNumber,S.*  from
                               (select distinct  b.PKID 父级ID,d.Name as DealerName,b.Name as CustomerNatureName,
                                c.* from CustomerNature a INNER JOIN 
                                (select * from CustomerNature where ParentID='0') b
                                ON a.ParentID=b.PKID INNER JOIN Customer c 
                                on b.PKID=c.CustomerNatureID and c.DealerID=@dealerId INNER JOIN Dealer d 
                                on c.DealerID=d.PKID where c.PKID>0");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(" and c.EnterpriseCode like '%" + filter + "%' or c.Name like '%" + filter + "%'");
                }
                strSql.Append($@" ) as S) as T where T.rowNumber>(@pageIndex - 1) * @pageSize order by T.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("dealerId", dealerId);
                List<Common.Entity.Customer> list = conn.Query<Common.Entity.Customer>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 从服务器端获取客户分页列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>返回DataTable</returns>
        public List<Common.Entity.Customer> GetCustomerPagerByDealerID(int pageIndex, int pageSize, int dealerId, string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from (select distinct b.PKID 父级ID,b.Name 父级名称,c.* from CustomerNature a INNER JOIN 
                                (select * from CustomerNature where ParentID='0') b
                                ON a.ParentID=b.PKID
                                INNER JOIN Customer c on b.PKID=c.CustomerNatureID and c.DealerID=@dealerId
                                INNER JOIN Dealer d on c.DealerID=d.PKID where c.PKID>0 ");
                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(" and c.EnterpriseCode like '%" + filter + "%' or c.Name like '%" + filter + "%'");
                }
                sqlCount.Append(" ) A");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("dealerId", dealerId);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from (
                                select  row_number() over (order by S.Name) as rowNumber,S.*  from
                               (select distinct  b.PKID 父级ID,d.Name as DealerName,b.Name as CustomerNatureName,
                                c.* from CustomerNature a INNER JOIN 
                                (select * from CustomerNature where ParentID='0') b
                                ON a.ParentID=b.PKID INNER JOIN Customer c 
                                on b.PKID=c.CustomerNatureID and c.DealerID=@dealerId INNER JOIN Dealer d 
                                on c.DealerID=d.PKID where c.PKID>0");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(" and c.EnterpriseCode like '%" + filter + "%' or c.Name like '%" + filter + "%'");
                }
                strSql.Append($@" ) as S) as T where T.rowNumber>(@pageIndex - 1) * @pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("dealerId", dealerId);
                List<Common.Entity.Customer> list = conn.Query<Common.Entity.Customer>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion
    }
}

