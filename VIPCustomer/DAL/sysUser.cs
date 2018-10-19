using Common;
using Common.Entity;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class sysUser
    {
        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="strloginName">用户名</param>
        /// <param name="strloginPassword">密码</param>
        /// <param name="isSystem">是否系统管理员</param>
        /// <returns></returns>
        public SysUserEntity getSysUserByLogin(string strloginName, string strloginPassword)
        {
            string sql = string.Format($@"SELECT TOP 1 * FROM SysUser WHERE LoginName='{strloginName}' AND LoginPwd='{strloginPassword}' and IsActivate=1");
            SysUserEntity user = null;
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql);
            if (dt.Rows.Count > 0)
            {
                user = new SysUserEntity();
                if (!DBNull.Value.Equals(dt.Rows[0]["PKID"]))
                    user.PKID = int.Parse(dt.Rows[0]["PKID"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["LoginName"]))
                    user.LoginName = dt.Rows[0]["LoginName"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["Name"]))
                    user.Name = dt.Rows[0]["Name"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["LoginPwd"]))
                    user.LoginPwd = dt.Rows[0]["LoginPwd"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["NickName"]))
                    user.NickName = dt.Rows[0]["NickName"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["DealerId"]))
                    user.DealerId = int.Parse(dt.Rows[0]["DealerId"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["UserTypeId"]))
                    user.UserTypeId = Convert.ToInt32(dt.Rows[0]["UserTypeId"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["Email"]))
                    user.Email = dt.Rows[0]["Email"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["Phone"]))
                    user.Phone = dt.Rows[0]["Phone"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["IsSystemUser"]))
                    user.IsSystemUser = Convert.ToInt32(dt.Rows[0]["IsSystemUser"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["IsActivate"]))
                    user.IsActivate = int.Parse(dt.Rows[0]["IsActivate"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["Remark"]))
                    user.Remark = dt.Rows[0]["Remark"].ToString();
                return user;
            }
            return user;
        }
        #endregion

        #region 获取所有经销店名称
        /// <summary>
        /// 获取所有经销店名称
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        public DataTable GetDealerAll(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select PKID,Name from Dealer");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by Name");
            return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
        }
        #endregion

        #region 根据预编码获取用户类别下拉列表
        /// <summary>
        /// 根据预编码获取用户类别下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserType()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select PKID as UserTypeId,Code,ListName as UserTypeName from Dictionary where Code='USERTYPE' order by Sort");
            return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"select * from SYsUser where LoginName='{loginName}'");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(@" select * from SysUser where LoginName=@LoginName and LoginPwd=@Password and IsActivate=1");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("LoginName", loginName);
                param1.Add("Password", password);
                SysUserEntity model = new SysUserEntity();
                model = conn.Query<SysUserEntity>(strSql.ToString(), param1).FirstOrDefault();
                return model;
            }
        }
        #endregion

        #region 新增一条数据
        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public int Add(SysUserEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into SysUser(LoginName,Name,NickName,DealerId,UserTypeId,LoginPwd,Email,Phone,IsSystemUser,IsActivate,Remark)");
            strSql.Append(" Values ");
            strSql.Append("(@LoginName,@Name,@NickName,@DealerId,@UserTypeId,@LoginPwd,@Email,@Phone,@IsSystemUser,@IsActivate,@Remark)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@LoginName",model.LoginName),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@NickName",model.NickName),
                new SqlParameter("@DealerId",model.DealerId),
                new SqlParameter("@UserTypeId",model.UserTypeId),
                new SqlParameter("@LoginPwd",model.LoginPwd),
                new SqlParameter("@Email",model.Email),
                new SqlParameter("@Phone",model.Phone),
                new SqlParameter("@IsSystemUser",model.IsSystemUser),
                new SqlParameter("@IsActivate",model.IsActivate),
                new SqlParameter("@Remark",model.Remark)
            };
            return SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"Update SysUser set LoginName=@LoginName,Name=@Name,NickName=@NickName,DealerId=@DealerId,
                                         UserTypeId=@UserTypeId,LoginPwd=@LoginPwd,Email=@Email,Phone=@Phone,IsActivate=@IsActivate,
                                         Remark=@Remark where PKID=@PKID");
                DynamicParameters param = new DynamicParameters();
                param.Add("LoginName", model.LoginName);
                param.Add("Name", model.Name);
                param.Add("NickName", model.NickName);
                param.Add("DealerId", model.DealerId);
                param.Add("UserTypeId", model.UserTypeId);
                param.Add("LoginPwd", model.LoginPwd);
                param.Add("Email", model.Email);
                param.Add("Phone", model.Phone);
                param.Add("IsActivate", model.IsActivate);
                param.Add("Remark", model.Remark);
                param.Add("PKID", model.PKID);
                int result = conn.Execute(sql, param);
                if (result > 0) return true;
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region 修改个人信息
        public bool SetBaseInfo(SysUserEntity model)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"Update SysUser set Name=@Name,Email=@Email,Phone=@Phone where PKID=@PKID");
                DynamicParameters param = new DynamicParameters();
                param.Add("Name", model.Name);
                param.Add("Email", model.Email);
                param.Add("Phone", model.Phone);
                param.Add("PKID", model.PKID);
                int result = conn.Execute(sql, param);
                if (result > 0) return true;
                else
                {
                    return false;
                }
            }
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"delete from SysUser where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            int rows = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1* from SysUser where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            SysUserEntity model = null;
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            if (dt.Rows.Count > 0)
            {
                model = new SysUserEntity();
                if (!DBNull.Value.Equals(dt.Rows[0]["PKID"]))
                    model.PKID = int.Parse(dt.Rows[0]["PKID"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["LoginName"]))
                    model.LoginName = dt.Rows[0]["LoginName"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["Name"]))
                    model.Name = dt.Rows[0]["Name"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["LoginPwd"]))
                    model.LoginPwd = dt.Rows[0]["LoginPwd"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["NickName"]))
                    model.NickName = dt.Rows[0]["NickName"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["DealerId"]))
                    model.DealerId = int.Parse(dt.Rows[0]["DealerId"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["DealerId"]))
                    model.DealerName = dt.Rows[0]["DealerId"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["UserTypeId"]))
                    model.UserTypeId = Convert.ToInt32(dt.Rows[0]["UserTypeId"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["UserTypeId"]))
                    model.UserTypeName = dt.Rows[0]["UserTypeId"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["Email"]))
                    model.Email = dt.Rows[0]["Email"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["Phone"]))
                    model.Phone = dt.Rows[0]["Phone"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["IsSystemUser"]))
                    model.IsSystemUser = Convert.ToInt32(dt.Rows[0]["IsSystemUser"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["IsActivate"]))
                    model.IsActivate = int.Parse(dt.Rows[0]["IsActivate"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["Remark"]))
                    model.Remark = dt.Rows[0]["Remark"].ToString();
                return model;
            }
            return model;
        }
        #endregion

        #region 设置用户角色
        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="AddRoleList">要增加的</param>
        /// <param name="DeleteRoleList">要删除的</param>
        /// <returns></returns>
        public bool SetRoleSingle(List<UserRoleEntity> AddRoleList, List<UserRoleEntity> DeleteRoleList)
        {
            Hashtable strStringList = new Hashtable();
            for (int i = 0; i < DeleteRoleList.Count; i++)// 删除的用户角色
            {
                StringBuilder strDeleteSql = new StringBuilder();
                strDeleteSql.Append(@"delete from UserRole where UserID=@UserID and RoleID=@RoleID");
                SqlParameter[] paramer1 = {
                    new SqlParameter("@UserID",SqlDbType.Int,1000),
                    new SqlParameter("@RoleID",SqlDbType.Int,1000)
                };
                paramer1[0].Value = DeleteRoleList[i].UserID;
                paramer1[1].Value = DeleteRoleList[i].RoleID;
                strStringList.Add(strDeleteSql, paramer1);
            }
            for (int i = 0; i < AddRoleList.Count; i++)// 新增的用户角色
            {
                StringBuilder strInsertSql = new StringBuilder();
                strInsertSql.Append(@"Insert into UserRole (RoleID,Remark,UserID) values (@RoleID,@Remark,@UserID)");
                strInsertSql.Append(";select @@IDENTITY");
                SqlParameter[] paramer2 = {
                    new SqlParameter("@RoleID",SqlDbType.Int,1000),
                    new SqlParameter("@Remark",SqlDbType.NVarChar),
                    new SqlParameter("@UserID",SqlDbType.Int,1000)
                };
                paramer2[0].Value = AddRoleList[i].RoleID;
                paramer2[1].Value = AddRoleList[i].Remark;
                paramer2[2].Value = AddRoleList[i].UserID;
                strStringList.Add(strInsertSql, paramer2);
            }
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.connStr, strStringList);
                return true;
            }
            catch
            {
                return false;
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"select DL.PKID as DealerId,DL.Name as DealerName,SU.* from SYsUser SU
                                        left join Dealer DL on SU.DealerId = DL.PKID  where SU.LoginName=@LoginName");
                DynamicParameters param = new DynamicParameters();
                param.Add("LoginName", username);
                SysUserEntity list = new SysUserEntity();
                list = conn.Query<SysUserEntity>(sql, param).SingleOrDefault();
                return list;
            }
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" select distinct CarName from RoleBrand RB
									   inner join [Role] R on R.PKID=RB.RoleID and R.IsActivate=1
									   inner join UserRole UR on R.PKID=UR.RoleID
									   and UR.UserID=@UserID");
                DynamicParameters param = new DynamicParameters();
                param.Add("UserID", userId);
                List<string> result = conn.Query<string>(sql, param).ToList();
                return result;
            }
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" select count(1) from RoleModule srm inner join Role sr on srm.RoleID=sr.PKID and sr.IsActivate=1
	                                          inner join UserRole sur on sr.PKID=sur.RoleID
	                                          where sur.UserID=@userID
	                                          and srm.ModuleID=@ModuleID");
                DynamicParameters param = new DynamicParameters();
                param.Add("userID", userID);
                param.Add("ModuleID", moduleID);
                int result = conn.Query<int>(sql, param).FirstOrDefault();
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

        /// <summary>
        /// 获取用户模块的操作权限
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public List<SysUserModuleRight> GetUserModuleControl(int userID, int moduleID)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" select distinct smc.Code ControlName,
	                                          case when  umc.ModuleCmdID>0 then 'true' else 'false' end IsRight from  ModuleCommand smc left join 
	                                          (select distinct srmc.ModuleCmdID  from RoleModuleControl srmc inner join Role sr on srmc.RoleID=sr.PKID and sr.IsActivate=1
	                                          inner join UserRole sur on sur.RoleID=sr.PKID and sur.UserID=@userID and srmc.ModuleID=@ModuleID)
	                                          umc on smc.PKID=umc.ModuleCmdID where smc.ModuleID=@ModuleID");
                DynamicParameters param = new DynamicParameters();
                param.Add("userID", userID);
                param.Add("ModuleID", moduleID);
                List<SysUserModuleRight> list = new List<SysUserModuleRight>();
                list = conn.Query<SysUserModuleRight>(sql, param).ToList();
                return list;
            }
        }
    }
}
