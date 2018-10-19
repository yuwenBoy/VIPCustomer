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
    public class Role
    {
        #region 根据用户ID获取角色
        /// <summary>
        /// 根据用户ID获取角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public DataTable GetRoleByUserId(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from Role RL inner join UserRole UR on RL.PKID=UR.RoleID and UR.UserID=@userId");
            SqlParameter[] paramer = {
                new SqlParameter("@UserID",userId)
            };
            return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paramer);
        }
        #endregion

        #region 根据PKID获取实体对象
        /// <summary>
        /// 根据PKID获取实体对象
        /// </summary>
        /// <param name="PKID">角色ID</param>
        /// <returns></returns>
        public RoleEntity GetRoleByPKID(int PKID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select top 1 * from Role where PKID=@PKID");
            SqlParameter[] paramer = {
                new SqlParameter("@PKID",PKID),
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paramer);
            RoleEntity model = null;
            if (dt.Rows.Count > 0)
            {
                model = new RoleEntity();
                if (!DBNull.Value.Equals(dt.Rows[0]["PKID"]))
                    model.PKID = int.Parse(dt.Rows[0]["PKID"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["Name"]))
                    model.Name = dt.Rows[0]["Name"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["Remark"]))
                    model.Remark = dt.Rows[0]["Remark"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["IsActivate"]))
                    model.IsActivate = int.Parse(dt.Rows[0]["IsActivate"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["RoleGrade"]))
                    model.RoleGrade = int.Parse(dt.Rows[0]["RoleGrade"].ToString());
                return model;
            }
            return model;
        }
        #endregion

        #region 检索是否存在该记录
        /// <summary>
        /// 检索是否存在该记录
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            string sql = string.Format(@" select * from Role where Name=@Name");
            SqlParameter[] param = {
                new SqlParameter("@Name",name),
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
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

        #region 新增一条记录
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(RoleEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into Role(Name,Remark,IsActivate,RoleGrade) Values");
            strSql.Append("(@Name,@Remark,@IsActivate,@RoleGrade)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@IsActivate",model.IsActivate),
                new SqlParameter("@RoleGrade",model.RoleGrade),
            };
            return SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Update(RoleEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update Role set Name=@Name,RoleGrade=@RoleGrade,IsActivate=@IsActivate,Remark=@Remark where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",model.PKID),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@IsActivate",model.IsActivate),
                new SqlParameter("@RoleGrade",model.RoleGrade),
            };
            object obj = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            if (Convert.ToInt32(obj) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="PKID">角色ID</param>
        /// <returns></returns>
        public bool DELETEDATE(int PKID)
        {
            string sql = string.Format(@"delete from Role where PKID=@PKID");
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

        #region 根据角色ID查询角色车型信息
        /// <summary>
        /// 根据角色ID查询角色车型信息
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <returns></returns>
        public DataTable GetRoleBrandByRoleID(int RoleID)
        {
            string sql = string.Format(@"select distinct RoleID,CarName from RoleBrand where RoleID=@RoleID");
            SqlParameter[] param = {
                new SqlParameter("@RoleID",RoleID)
            };
            return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
        }
        #endregion

        #region 根据角色ID获取车型
        /// <summary>
        /// 根据角色ID获取车型
        /// </summary>
        /// <param name="RoleId">角色ID</param>
        /// <returns></returns>
        public DataTable GetRoleByCarId(int RoleId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select distinct RoleID,CarName from RoleBrand where RoleID=@RoleID");
            SqlParameter[] paramer = {
                new SqlParameter("@RoleID",RoleId)
            };
            return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paramer);
        }
        #endregion

        #region 设置车型角色
        /// <summary>
        /// 设置车型角色
        /// </summary>
        /// <param name="AddRoleList">要增加的</param>
        /// <param name="DeleteRoleList">要删除的</param>
        /// <returns></returns>
        public bool SetRoleSingle(List<RoleBrandEntity> AddRoleList, List<RoleBrandEntity> DeleteRoleList)
        {
            Hashtable strStringList = new Hashtable();
            for (int i = 0; i < DeleteRoleList.Count; i++)// 删除的车型角色
            {
                StringBuilder strDeleteSql = new StringBuilder();
                strDeleteSql.Append(@" delete from  RoleBrand where RoleID=@RoleID and CarName=@CarName");
                SqlParameter[] paramer1 = {
                    new SqlParameter("@RoleID",SqlDbType.Int,1000),
                    new SqlParameter("@CarName",SqlDbType.NVarChar)
                };
                paramer1[0].Value = DeleteRoleList[i].RoleID;
                paramer1[1].Value = DeleteRoleList[i].CarName;
                strStringList.Add(strDeleteSql, paramer1);
            }
            for (int i = 0; i < AddRoleList.Count; i++)// 新增的车型角色
            {
                StringBuilder strInsertSql = new StringBuilder();
                strInsertSql.Append(@" insert into RoleBrand(RoleID,CarName) values (@RoleID,@CarName)");
                strInsertSql.Append(";select @@IDENTITY");
                SqlParameter[] paramer2 = {
                    new SqlParameter("@RoleID",SqlDbType.Int,1000),
                    new SqlParameter("@CarName",SqlDbType.NVarChar)
                };
                paramer2[0].Value = AddRoleList[i].RoleID;
                paramer2[1].Value = AddRoleList[i].CarName;
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

        #region 获取角色模块树节点信息
        /// <summary>
        /// 获取角色模块树节点信息
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public string GetRoleModuleByRoleIdTree(int roleId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string sql = string.Format(@"select MD.PKID as PKID,MD.Name as Name,MD.Sort,MD.ParentID as ParentID,
                                       case when RM.RoleID is null then 'false' 
                                       else 'true' end IsChecked from Module MD
                                       left join RoleModule RM on MD.PKID=RM.ModuleID
                                       and MD.IsActivate=1 and RM.RoleID=@RoleID order by Sort");
            SqlParameter[] param = {
                new SqlParameter("@RoleID",roleId)
            };
            try
            {
                stringBuilder.Append("[");
                DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var str = $"{{id:{dt.Rows[i]["PKID"]},pId:{dt.Rows[i]["ParentID"]},name:'{dt.Rows[i]["Name"]}',checked:{dt.Rows[i]["IsChecked"]}}},";
                    stringBuilder.Append(str);
                }
                stringBuilder.Append("]");
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 新增一批数据
        /// <summary>
        /// 新增一批数据
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(RoleModuleEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Insert into RoleModule(RoleID,Remark,ModuleID) Values");
            strSql.Append("(@RoleID,@Remark,@ModuleID)");
            SqlParameter[] param = {
                new SqlParameter("@RoleID",model.RoleID),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@ModuleID",model.ModuleID)
            };

            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
                if (result > 0)
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion


        /// <summary>
        /// 删除角色模块表
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        public void DeleteRoleModule(int RoleID)
        {
            string sql = string.Format("delete from RoleModule where RoleID=@RoleID");
            SqlParameter[] param = {
                new SqlParameter("@RoleID",RoleID)
            };
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, sql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 清空无用数据(在RoleModuleCommand表中有，但在RoleModule中没有的数据)
        /// </summary>
        public void cleanDeathData()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"delete from RoleModuleControl from ");
            strSql.Append("RoleModuleControl RMC left join RoleModule RM on RMC.RoleID=RM.RoleID");
            strSql.Append(" and RMC.ModuleID=RM.ModuleID where RM.ModuleID is null");
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 添加一批数据
        /// <summary>
        /// 添加一批数据
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="moduleIDs">模块ID</param>
        /// <returns></returns>
        public void Add(int roleId, List<int> moduleIDs)
        {
            try
            {
                DeleteRoleModule(roleId);
                List<RoleModuleEntity> list = new List<RoleModuleEntity>();
                foreach (int item in moduleIDs)
                {
                    RoleModuleEntity model = new RoleModuleEntity();
                    model.RoleID = roleId;
                    model.ModuleID = item;
                    list.Add(model);
                }
                foreach (RoleModuleEntity entity in list)
                {
                    Add(entity);
                }
                cleanDeathData();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        // 根据角色ID和模块ID查询角色控制项列表
        public DataTable GeRoleModuleContorlByRoleIdAndModuleID2(int moduleID, int roleId)
        {
            string sql = string.Format(@"select MC.* from ModuleCommand MC
			                            inner join RoleModuleControl RMC
			                            on MC.ModuleID=RMC.ModuleID
			                            and MC.PKID=RMC.ModuleCmdID
			                            and RMC.RoleID=@RoleID
			                            and RMC.ModuleID=@ModuleID
			                            order by MC.PKID");
            SqlParameter[] param = {
                new SqlParameter("@ModuleID",moduleID),
                new SqlParameter("@RoleID",roleId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
            return dt;
        }

        #region 保存角色控制项数据
        /// <summary>
        /// 保存角色控制项数据
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <param name="ModuelsID">模块ID</param>
        /// <param name="ContortIDs">控制模块ID</param>
        public void Add(int RoleID, int ModuelsID, List<int> ContortIDs)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction()) // 开始数据库事务，即创建一个事务对象tran
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = tran;  // 获取或设置将要执行的事务
                        try
                        {
                            cmd.CommandText = $@"delete from RoleModuleControl where RoleID={RoleID} and ModuleID={ModuelsID}";
                            cmd.ExecuteNonQuery();
                            List<RoleModuleControlEntity> list = new List<RoleModuleControlEntity>();
                            foreach (var item in ContortIDs)
                            {
                                RoleModuleControlEntity model = new RoleModuleControlEntity();
                                model.RoleID = RoleID;
                                model.ModuleID = ModuelsID;
                                model.ModuleCmdID = item;
                                list.Add(model);
                            }
                            foreach (RoleModuleControlEntity entity in list)
                            {
                                cmd.CommandText = string.Format($@"Insert into RoleModuleControl(RoleID,ModuleCmdID,ModuleID,Remark) values({entity.RoleID},{entity.ModuleCmdID},{entity.ModuleID},'{entity.Remark}')");
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();  // 如果两条sql命令都执行成功，则执行commit这个方法来执行这些操作。
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback(); // 如果执行不成功，发送异常，则执行rollback方法，回滚到事务操作开始之前。
                            throw ex;
                        }
                    }
                }
            }
        }
        #endregion


        #region 获取系统模块页面按钮（根据不同角色对应不同的系统模块）
        /// <summary>
        /// 获取系统模块页面按钮（根据不同角色对应不同的系统模块）
        /// </summary>
        /// <returns></returns>
        public List<ModuleCommandEntity> GetRoleModuleContortList(int moduleId)
        {
            List<ModuleCommandEntity> list = new List<ModuleCommandEntity>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" select* from ModuleCommand where ModuleID=@ModuleID");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("ModuleID", moduleId);
                list = conn.Query<ModuleCommandEntity>(sql, param1).ToList();
            }
            return list;
        }
        #endregion
    }
}
