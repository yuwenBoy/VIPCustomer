using Common;
using Common.Entity;
using DAL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BLL
{
    public class sysUser
    {

        #region 从服务器端获取用户维护分页列表
        /// <summary>
        /// 从服务器端获取用户维护分页列表(当按排序查询时)
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<SysUserEntity> GetUserManagePager(int pageIndex, int pageSize, string sortName, string IsDesc, SysUserEntity filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from SysUser SU left join Dealer DL on SU.DealerId=DL.PKID 
                                  left join Dictionary DC on SU.UserTypeId=DC.PKID where SU.PKID>0");

                if (!string.IsNullOrEmpty(filter.LoginName))
                {
                    sqlCount.Append(" and SU.LoginName like '%" + filter.LoginName + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    sqlCount.Append(" and SU.Name like '%" + filter.Name + "%'");
                }
                if (filter.UserTypeId > 0)
                {
                    sqlCount.Append(" and SU.UserTypeId=@UserTypeId");
                }
                if (filter.IsActivate != -1)
                {
                    sqlCount.Append(" and SU.IsActivate=@IsActivate");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("UserTypeId", filter.UserTypeId);
                param1.Add("pageSize", pageSize);
                param1.Add("IsActivate", filter.IsActivate);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from ( select row_number() over (order by SU.PKID) as rowNumber");
                strSql.Append(" ,SU.*,DL.Name as DealerName,DC.ListName as UserTypeName from SYsUser SU");
                strSql.Append($@" left join Dealer DL  on SU.DealerId=DL.PKID left join Dictionary DC on SU.UserTypeId=DC.PKID
                                 where SU.PKID>0 ");

                if (!string.IsNullOrEmpty(filter.LoginName))
                {
                    strSql.Append(" and SU.LoginName like '%" + filter.LoginName + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    strSql.Append(" and SU.Name like '%" + filter.Name + "%'");
                }
                if (filter.UserTypeId > 0)
                {
                    strSql.Append("  and SU.UserTypeId=@UserTypeId");
                }
                if (filter.IsActivate != -1)
                {
                    strSql.Append(" and SU.IsActivate=@IsActivate");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("UserTypeId", filter.UserTypeId);
                param2.Add("IsActivate", filter.IsActivate);
                List<SysUserEntity> list = conn.Query<SysUserEntity>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 从服务器端获取用户维护分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<SysUserEntity> GetUserManagePager(int pageIndex, int pageSize, SysUserEntity filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from SysUser SU left join Dealer DL on SU.DealerId=DL.PKID left join Dictionary DC on SU.UserTypeId=DC.PKID where SU.PKID>0");
                if (!string.IsNullOrEmpty(filter.LoginName))
                {
                    sqlCount.Append(" and SU.LoginName like '%" + filter.LoginName + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    sqlCount.Append(" and SU.Name like '%" + filter.Name + "%'");
                }
                if (filter.UserTypeId > 0)
                {
                    sqlCount.Append(" and SU.UserTypeId=@UserTypeId");
                }
                if (filter.IsActivate != -1)
                {
                    sqlCount.Append(" and SU.IsActivate=@IsActivate");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("UserTypeId", filter.UserTypeId);
                param1.Add("pageSize", pageSize);
                param1.Add("IsActivate", filter.IsActivate);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"select top (@pageSize) * from ( select row_number() over (order by SU.PKID) as rowNumber");
                strSql.Append(" ,SU.*,DL.Name as DealerName,DC.ListName as UserTypeName from SYsUser SU");
                strSql.Append(@" left join Dealer DL  on SU.DealerId=DL.PKID left join Dictionary DC on SU.UserTypeId=DC.PKID 
                                   where SU.PKID > 0 ");
                if (!string.IsNullOrEmpty(filter.LoginName))
                {
                    strSql.Append(" and SU.LoginName like '%" + filter.LoginName + "%'");
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    strSql.Append(" and SU.Name like '%" + filter.Name + "%'");
                }
                if (filter.UserTypeId > 0)
                {
                    strSql.Append(" and SU.UserTypeId=@UserTypeId");
                }
                if (filter.IsActivate != -1)
                {
                    strSql.Append(" and SU.IsActivate=@IsActivate");
                }
                strSql.Append("  ) as t where t.rowNumber > (@pageIndex - 1) * @pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("UserTypeId", filter.UserTypeId);
                param2.Add("IsActivate", filter.IsActivate);
                List<SysUserEntity> list = conn.Query<SysUserEntity>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        #region  获取所有经销店名称
        /// <summary>
        /// 获取所有经销店名称
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        public DataTable GetDealerAll(string strWhere)
        {
            return new DAL.sysUser().GetDealerAll(strWhere);
        }
        #endregion

        #region 根据预编码获取用户类别下拉列表
        /// <summary>
        /// 根据预编码获取用户类别下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserType()
        {
            return new DAL.sysUser().GetUserType();
        }
        #endregion

        #region 检索是否存在该用户名称
        /// <summary>
        /// 检索是否存在该用户名称
        /// </summary>
        /// <param name="loginName">用户名称</param>
        /// <returns></returns>
        public bool Exists(string loginName)
        {
            return new DAL.sysUser().Exists(loginName);
        }
        #endregion

        #region 检索是否存在该用户名称
        /// <summary>
        /// 检索是否存在该用户名称
        /// </summary>
        /// <param name="loginName">用户名称</param>
        /// <returns></returns>
        public SysUserEntity UserLogin(string loginName, string password)
        {
            return new DAL.sysUser().UserLogin(loginName, password);
        }
        #endregion

        #region 新增一条记录
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public int Add(SysUserEntity model)
        {
            return new DAL.sysUser().Add(model);
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public bool Update(SysUserEntity model)
        {
            return new DAL.sysUser().Update(model);
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="PKID">用户ID</param>
        /// <returns></returns>
        public bool Delete(int PKID)
        {
            return new DAL.sysUser().Delete(PKID);
        }
        #endregion

        #region 根据用户ID查询数据
        /// <summary>
        /// 根据用户ID查询数据
        /// </summary>
        /// <param name="PKID">用户ID</param>
        /// <returns></returns>
        public SysUserEntity GetSysUserByPKID(int PKID)
        {
            return new DAL.sysUser().GetSysUserByPKID(PKID);
        }
        #endregion

        #region 设置用户角色信息
        /// <summary>
        /// 设置用户角色信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="RoleIds">角色ID</param>
        /// <returns></returns>
        public bool SaveUserRoleDate(int userId, string RoleIds)
        {
            DataTable dt_userRole_Old = new DAL.Role().GetRoleByUserId(userId);
            List<UserRoleEntity> AddRoleList = new List<UserRoleEntity>();
            List<UserRoleEntity> DeleteRoleList = new List<UserRoleEntity>();
            string[] str_RoleIds = RoleIds.Trim(',').Split(',');
            UserRoleEntity userRoleDelete = null;
            UserRoleEntity userRoleAdd = null;
            for (int i = 0; i < dt_userRole_Old.Rows.Count; i++)
            {
                if (Array.IndexOf(str_RoleIds, dt_userRole_Old.Rows[i]["RoleID"].ToString()) == -1)
                {
                    userRoleDelete = new UserRoleEntity();
                    userRoleDelete.RoleID = Convert.ToInt32(dt_userRole_Old.Rows[i]["RoleID"].ToString());
                    userRoleDelete.UserID = userId;
                    DeleteRoleList.Add(userRoleDelete);
                }
            }
            if (!string.IsNullOrEmpty(RoleIds))
            {
                for (int j = 0; j < str_RoleIds.Length; j++)
                {
                    if (dt_userRole_Old.Select("RoleID = '" + str_RoleIds[j] + "'").Length == 0)
                    {
                        userRoleAdd = new UserRoleEntity();
                        userRoleAdd.UserID = userId;
                        userRoleAdd.RoleID = Convert.ToInt32(str_RoleIds[j]);
                        AddRoleList.Add(userRoleAdd);
                    }
                }
            }
            if (AddRoleList.Count == 0 && DeleteRoleList.Count == 0)
            {
                return true;
            }
            else
            {
                return new DAL.sysUser().SetRoleSingle(AddRoleList, DeleteRoleList);
            }
        }
        #endregion

        #region 根据登录名获取用户信息
        /// <summary>
        /// 根据登录名获取用户信息
        /// </summary>
        /// <param name="username">登录名</param>
        /// <returns></returns>
        public SysUserEntity GetSysUserByLoginName(string username)
        {
            return new DAL.sysUser().GetSysUserByLoginName(username);
        }
        #endregion

        #region 修改个人信息
        public bool SetBaseInfo(SysUserEntity model)
        {
            return new DAL.sysUser().SetBaseInfo(model);
        }
        #endregion

        #region 查询用户拥有权限的车辆名称
        /// <summary>
        /// 查询用户拥有权限的车辆名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetUserCarNames(int userId)
        {
            return new DAL.sysUser().GetUserCarNames(userId);
        }
        #endregion

        /// <summary>
        /// 获取用户是否具有模块权限
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public bool GetUserModuleRigth(int userID, int moduleID)
        {
            return new DAL.sysUser().GetUserModuleRigth(userID, moduleID);
        }

        /// <summary>
        /// 获取用户模块的操作权限
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public List<SysUserModuleRight> GetUserModuleControl(int userID, int moduleID)
        {
            return new DAL.sysUser().GetUserModuleControl(userID, moduleID);
        }
    }
}
