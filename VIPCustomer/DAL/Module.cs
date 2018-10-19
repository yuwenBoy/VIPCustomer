using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DAL
{
    /// <summary>
    /// 模块维护:数据访问层
    /// </summary>
    public class Module
    {
        #region 获取所有节点信息
        /// <summary>
        /// 获取所有节点信息
        /// </summary>
        /// <returns></returns>
        public string GetAllModulesJson()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string sql = string.Format(@"select * from Module order by Sort");
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            stringBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var str = $"{{id:{dt.Rows[i]["PKID"]},pId:{dt.Rows[i]["ParentID"]},name:'{dt.Rows[i]["Name"]}'}},";
                stringBuilder.Append(str);
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="nodeId">PKID</param>
        /// <returns></returns>
        public ModuleEntity GetModuleByPKID(int nodeId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" select  * from Module where PKID=@PKID ");
            SqlParameter[] paramer = {
                new SqlParameter("@PKID",nodeId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paramer);
            ModuleEntity model = null;
            if (dt.Rows.Count > 0)
            {
                model = new ModuleEntity();
                if (!DBNull.Value.Equals(dt.Rows[0]["PKID"]))
                    model.PKID = int.Parse(dt.Rows[0]["PKID"].ToString());

                if (!DBNull.Value.Equals(dt.Rows[0]["Name"]))
                    model.Name = dt.Rows[0]["Name"].ToString();

                if (!DBNull.Value.Equals(dt.Rows[0]["ParentID"]))
                    model.ParentID = Convert.ToInt32(dt.Rows[0]["ParentID"].ToString());

                if (!DBNull.Value.Equals(dt.Rows[0]["IsActivate"]))
                    model.IsActivate = int.Parse(dt.Rows[0]["IsActivate"].ToString());

                if (!DBNull.Value.Equals(dt.Rows[0]["IsSysModule"]))
                    model.IsSysModule = int.Parse(dt.Rows[0]["IsSysModule"].ToString());

                if (!DBNull.Value.Equals(dt.Rows[0]["PageAddress"]))
                    model.PageAddress = dt.Rows[0]["PageAddress"].ToString();

                if (!DBNull.Value.Equals(dt.Rows[0]["ImagesAddress"]))
                    model.ImagesAddress = dt.Rows[0]["ImagesAddress"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["ChangeImgAddress"]))
                    model.ChangeImgAddress = dt.Rows[0]["ChangeImgAddress"].ToString();

                if (!DBNull.Value.Equals(dt.Rows[0]["FrameName"]))
                    model.FrameName = dt.Rows[0]["FrameName"].ToString();

                if (!DBNull.Value.Equals(dt.Rows[0]["Remark"]))
                    model.Remark = dt.Rows[0]["Remark"].ToString();

                if (!DBNull.Value.Equals(dt.Rows[0]["Sort"]))
                    model.Sort = Convert.ToInt32(dt.Rows[0]["Sort"].ToString());
                return model;
            }
            return model;
        }
        #endregion

        #region 获取模块的排序号
        /// <summary>
        /// 获取模块的排序号
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public int GetModuleOrder(int PKID)
        {
            string sql = string.Format(@"select Sort  from Module where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID),
            };
            int result = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, sql, param));
            if (result < 0)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(result);
            }
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Update(ModuleEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update Module set Name=@Name,IsActivate=@IsActivate,IsSysModule=@IsSysModule,PageAddress=@PageAddress,Remark=@Remark where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@IsActivate",model.IsActivate),
                new SqlParameter("@IsSysModule",model.IsSysModule),
                new SqlParameter("@PageAddress",model.PageAddress),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@PKID",model.PKID)
            };
            int result = Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
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

        #region 添加子级节点修改排序
        /// <summary>
        /// 添加子级节点修改排序
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        public int GetMaxOrder(int parentId)
        {
            string sql = string.Format(@" select isNull(Max(Sort),0) from Module where ParentID=@ParentID"); // 不明白
            SqlParameter[] param = {
                new SqlParameter("@ParentID",parentId)
            };
            int res = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, sql, param));
            if (res > 0)
            {
                return Convert.ToInt32(res);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 新增模块修改父级以及排序
        /// <summary>
        /// 新增模块修改父级以及排序
        /// </summary>
        /// <param name="parentID">父级</param>
        /// <param name="sort">排序</param>
        public void IncOrder(int parentID, int sort)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" update Module set Sort =Sort+1 where ParentID=@ParentID and Sort>=@Sort");
            SqlParameter[] param = {
                new SqlParameter("@ParentID",parentID),
                new SqlParameter("@Sort",sort)
            };
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除模块节点同时修改ParentID和Sort
        /// <summary>
        /// 删除模块节点同时修改ParentID和Sort
        /// </summary>
        /// <param name="PId">父级ID</param>
        /// <param name="Sort">排序</param>
        private void DecOrder(int PId, int Sort)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" update Module set Sort =Sort-1 where ParentID=@ParentID and Sort>=@Sort");
            SqlParameter[] param = {
                new SqlParameter("@ParentID",PId),
                new SqlParameter("@Sort",Sort)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
        }
        #endregion

        #region 新增一条记录
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public int Add(ModuleEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into Module(Name,ParentID,IsActivate,IsSysModule,PageAddress,ImagesAddress,ChangeImgAddress,FrameName,Sort,Remark) Values");
            strSql.Append("(@Name,@ParentID,@IsActivate,@IsSysModule,@PageAddress,@ImagesAddress,@ChangeImgAddress,@FrameName,@Sort,@Remark)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                  new SqlParameter("@Name",model.Name),
                  new SqlParameter("@ParentID",model.ParentID),
                  new SqlParameter("@IsActivate",model.IsActivate),
                  new SqlParameter("@IsSysModule",model.IsSysModule),
                  new SqlParameter("@PageAddress",model.PageAddress),
                  new SqlParameter("@ImagesAddress",model.ImagesAddress),
                  new SqlParameter("@ChangeImgAddress",model.ChangeImgAddress),
                  new SqlParameter("@FrameName",model.FrameName),
                  new SqlParameter("@Sort",model.Sort),
                  new SqlParameter("@Remark",model.Remark)
            };
            try
            {
                IncOrder(model.ParentID, model.Sort);
                return SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 删除模块信息
        /// <summary>
        /// 删除模块信息
        /// </summary>
        /// <param name="nodeIds">模块ID</param>
        /// <returns></returns>
        public bool Delete(string nodeIds)
        {
            try
            {
                ModuleEntity root = GetModuleByPKID(Convert.ToInt32(nodeIds.Split(',')[0].Replace("'", "")));
                List<string> list = new List<string>();
                list.Add($@"Delete from Module where PKID in({nodeIds})");
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, list);
                if (result > 0)
                {
                    DecOrder(root.ParentID, root.Sort);
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

        #region 更新模块父级ID和排序
        /// <summary>
        /// 更新模块父级ID和排序
        /// </summary>
        /// <param name="model">实体对象</param>
        public void updateOrder(ModuleEntity model)
        {
            string sql = string.Format(@"update Module set ParentID=@ParentID,Sort=@Sort where PKID=@PKID");
            SqlParameter[] param ={
                new SqlParameter("@ParentID",model.ParentID),
                new SqlParameter("@Sort",model.Sort),
                new SqlParameter("@PKID",model.PKID)
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
        #endregion

        #region MC操作========================
        #region 根据模块ID查询模块控制数据列表
        /// <summary>
        /// 根据模块ID查询模块控制数据列表
        /// </summary>
        /// <param name="moduleID">模块ID</param>
        public List<ModuleCommandEntity> GetModuleContorlByModuleID(int moduleID)
        {
            try
            {
                string sql = string.Format(@"select * from ModuleCommand where ModuleID=@ModuleID order by Name");
                SqlParameter[] param = {
                new SqlParameter("@ModuleID",moduleID)
            };
                List<ModuleCommandEntity> list = new List<ModuleCommandEntity>();
                DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ModuleCommandEntity model = new ModuleCommandEntity();
                        model.PKID = Convert.ToInt32(dt.Rows[i]["PKID"]);
                        model.Code = dt.Rows[i]["Code"].ToString();
                        model.Name = dt.Rows[i]["Name"].ToString();
                        model.Remark = dt.Rows[i]["Remark"].ToString();
                        list.Add(model);
                    }
                }
                return list;
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
        public int MCAdd(ModuleCommandEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into ModuleCommand(ModuleID,Code,Name,Remark) Values");
            strSql.Append("(@ModuleID,@Code,@Name,@Remark)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                  new SqlParameter("@ModuleID",model.ModuleID),
                  new SqlParameter("@Code",model.Code),
                  new SqlParameter("@Name",model.Name),
                  new SqlParameter("@Remark",model.Remark)
            };
            try
            {
                return SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// /*修改一条记录*/
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool MCUpdate(ModuleCommandEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"update ModuleCommand set Code=@Code,Name=@Name, Remark=@Remark where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@PKID",model.PKID)
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
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

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="PKID">模块控制ID</param>
        /// <returns></returns>
        public ModuleCommandEntity ModuleCommandByPKID(int PKID)
        {
            string sql = string.Format(@"select * from ModuleCommand where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            try
            {
                DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
                ModuleCommandEntity model = new ModuleCommandEntity();
                if (dt.Rows.Count > 0)
                {
                    model.PKID = Convert.ToInt32(dt.Rows[0]["PKID"]);
                    model.ModuleID = Convert.ToInt32(dt.Rows[0]["ModuleID"]);
                    model.Code = dt.Rows[0]["Code"].ToString();
                    model.Name = dt.Rows[0]["Name"].ToString();
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

        #region 删除一条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="PKID">模块控制ID</param>
        /// <returns></returns>
        public bool MCDelete(int PKID)
        {
            string sql = string.Format(@"delete from ModuleCommand where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            try
            {
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
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #endregion

        #region 移动模块
        /// <summary>
        /// 移动模块
        /// </summary>
        /// <param name="moveType">移动类型1为同级、2为下级</param>
        /// <param name="sID">源数据</param>
        /// <param name="tID">移动目标数据</param>
        /// <returns></returns>
        public void MoveTo(string moveType, ModuleEntity source, ModuleEntity target)
        {
            try
            {
                int newOrder = (source.Sort > target.Sort) ? target.Sort + 1 : target.Sort;
                //修改自己的位置，把自己的原排序去除
                DecOrder(source.ParentID, source.Sort);
                if ("1" == moveType)
                {
                    source.ParentID = target.ParentID;
                    source.Sort = newOrder;
                    IncOrder(source.ParentID, source.Sort);
                    updateOrder(source);
                }
                else
                {
                    source.ParentID = target.PKID;
                    source.Sort = GetMaxOrder(source.ParentID) + 1;
                    updateOrder(source);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 获取左侧菜单
        /// <summary>
        /// 获取左侧菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>返回实体类</returns>
        public DataTable GetCTopMenu(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" select distinct M.* from SysUser SU
                             inner join UserRole UR on SU.PKID=UR.UserID and SU.IsActivate=1
                             inner join Role R on UR.RoleID=R.PKID and R.IsActivate=1
                             inner join RoleModule RM on R.PKID=RM.RoleID
                             inner join Module M on RM.ModuleID=M.PKID and M.IsActivate=1
                             where UR.UserID=@UserID order by M.ParentID,M.Sort");
            SqlParameter[] param = {
                new SqlParameter("@UserID",userId)
            };
            DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            ModuleEntity model = null;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    model = new ModuleEntity();
                    model.PKID = Convert.ToInt32(dt.Rows[i]["PKID"]);
                    model.Name = dt.Rows[i]["Name"].ToString();
                    model.ParentID = Convert.ToInt32(dt.Rows[i]["ParentID"]);
                    model.IsActivate = Convert.ToInt32(dt.Rows[i]["IsActivate"]);
                    model.IsSysModule = Convert.ToInt32(dt.Rows[i]["IsSysModule"]);
                    model.PageAddress = dt.Rows[i]["PageAddress"].ToString();
                    model.ImagesAddress = dt.Rows[i]["ImagesAddress"].ToString();
                    model.ChangeImgAddress = dt.Rows[i]["ChangeImgAddress"].ToString();
                    model.FrameName = dt.Rows[i]["FrameName"].ToString();
                    model.Remark = dt.Rows[i]["Remark"].ToString();
                    model.Sort = Convert.ToInt32(dt.Rows[i]["Sort"].ToString());
                }
                return dt;
            }
            return dt;
        }
        #endregion


        /// <summary>
        /// 根据路径获取模块信息
        /// </summary>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public ModuleEntity GetModuleByPath(string pagePath)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format("select *from Module where upper(PageAddress)=upper(@PageAddress)");
                DynamicParameters param = new DynamicParameters();
                param.Add("PageAddress", pagePath);
                ModuleEntity moduleEntity = new ModuleEntity();
                moduleEntity = conn.Query<ModuleEntity>(sql, param).FirstOrDefault();
                return moduleEntity;
            }
        }
    }
}
