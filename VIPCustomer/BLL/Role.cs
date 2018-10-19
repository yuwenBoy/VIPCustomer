using Common;
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
    public class Role
    {
        #region 从服务器端获取角色维护分页列表
        /// <summary>
        /// 从服务器端获取角色维护分页列表(当按排序查询时)
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<RoleEntity> GetRoleManagePager(int pageIndex, int pageSize, string sortName, string IsDesc, string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from Role where PKID>0");

                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(" and Name like '%" + filter + "%'");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from ( select row_number() over (order by PKID desc) as rowNumber,* from Role where PKID>0");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(" and Name like '%" + filter + "%'");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<RoleEntity> list = conn.Query<RoleEntity>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 从服务器端获取角色维护分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<RoleEntity> GetRoleManagePager(int pageIndex, int pageSize, string filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from Role where PKID>0");

                if (!string.IsNullOrEmpty(filter))
                {
                    sqlCount.Append(" and Name like '%" + filter + "%'");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from ( select row_number() over (order by PKID desc) as rowNumber,* from Role where PKID>0");
                if (!string.IsNullOrEmpty(filter))
                {
                    strSql.Append(" and Name like '%" + filter + "%'");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<RoleEntity> list = conn.Query<RoleEntity>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        #region 查询用户角色列表
        /// <summary>
        /// 查询用户角色列表
        /// <returns></returns>
        public List<RoleEntity> GetRoleList()
        {
            var list = new List<RoleEntity>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"select * from Role");
            using (var reader = SqlHelper.ExecuteReader(SqlHelper.connStr, CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    var model = new RoleEntity
                    {
                        PKID = Convert.ToInt32(reader["PKID"]),
                        Name = Convert.ToString(reader["Name"].ToString()),
                        IsActivate = Convert.ToInt32(reader["IsActivate"]),
                        Remark = Convert.ToString(reader["Remark"]),
                        RoleGrade = int.Parse(Convert.ToString(reader["RoleGrade"]))
                    };
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region 查询车型厂牌字典数据列表
        /// <summary>
        /// 查询车型厂牌字典数据列表
        /// </summary>
        /// <returns></returns>
        public List<BrandDictionaryEntity> GetBrandDictionaryByPageList()
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select* from BrandDictionary");
                List<BrandDictionaryEntity> list = new List<BrandDictionaryEntity>();
                list = conn.Query<BrandDictionaryEntity>(strSql.ToString()).ToList();
                return list;
            }
        }
        #endregion

        #region 根据用户ID获取角色名称
        /// <summary>
        /// 根据用户ID获取角色名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetRoleByUserId(int userId)
        {
            return new DAL.Role().GetRoleByUserId(userId);
        }
        #endregion

        #region 搜索条件类
        /// <summary>
        /// 搜索条件类
        /// </summary>
        public class SearchDao
        {
            public string Name { get; set; }
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
            return new DAL.Role().GetRoleByPKID(PKID);
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
            return new DAL.Role().Exists(name);
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
            return new DAL.Role().Add(model);
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
            return new DAL.Role().Update(model);
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
            return new DAL.Role().DELETEDATE(PKID);
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
            return new DAL.Role().GetRoleBrandByRoleID(RoleID);
        }
        #endregion

        #region 根据角色ID设置角色车型
        /// <summary>
        /// 根据角色ID设置角色车型
        /// </summary>
        /// <param name="RoleId">角色ID</param>
        /// <param name="strCarNames">车型名称s</param>
        /// <returns></returns>
        public bool SaveCarRoleDate(int RoleId, string strCarNames)
        {
            DataTable dt_carRole_Old = new DAL.Role().GetRoleByCarId(RoleId);
            List<RoleBrandEntity> AddRoleList = new List<RoleBrandEntity>();
            List<RoleBrandEntity> DeleteRoleList = new List<RoleBrandEntity>();
            string[] str_strCarNames = strCarNames.Trim(',').Split(',');
            RoleBrandEntity CarRoleDelete = null;
            RoleBrandEntity CarRoleAdd = null;
            for (int i = 0; i < dt_carRole_Old.Rows.Count; i++)
            {
                if (Array.IndexOf(str_strCarNames, dt_carRole_Old.Rows[i]["CarName"].ToString()) == -1)
                {
                    CarRoleDelete = new RoleBrandEntity();
                    CarRoleDelete.RoleID = RoleId;
                    CarRoleDelete.CarName = dt_carRole_Old.Rows[i]["CarName"].ToString(); ;
                    DeleteRoleList.Add(CarRoleDelete);
                }
            }
            if (!string.IsNullOrEmpty(strCarNames))
            {
                for (int j = 0; j < str_strCarNames.Length; j++)
                {
                    if (dt_carRole_Old.Select("CarName = '" + str_strCarNames[j] + "'").Length == 0)
                    {
                        CarRoleAdd = new RoleBrandEntity();
                        CarRoleAdd.CarName = str_strCarNames[j];
                        CarRoleAdd.RoleID = RoleId;
                        AddRoleList.Add(CarRoleAdd);
                    }
                }
            }
            if (AddRoleList.Count == 0 && DeleteRoleList.Count == 0)
            {
                return true;
            }
            else
            {
                return new DAL.Role().SetRoleSingle(AddRoleList, DeleteRoleList);
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
            return new DAL.Role().GetRoleModuleByRoleIdTree(roleId);
        }
        #endregion

        #region 添加一批数据
        /// <summary>
        /// 添加一批数据
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="moduleIDs">模块ID</param>
        /// <returns></returns>
        public void Add(int roleId, List<int> moduleIDs)
        {
            new DAL.Role().Add(roleId, moduleIDs);
        }
        #endregion

        #region 获取系统模块页面按钮（根据不同角色对应不同的系统模块）
        /// <summary>
        /// 获取系统模块页面按钮（根据不同角色对应不同的系统模块）
        /// </summary>
        /// <returns></returns>
        public List<ModuleCommandEntity> GetRoleModuleContortList(int moduleId)
        {
            return new DAL.Role().GetRoleModuleContortList(moduleId);
        }
        public DataTable GeRoleModuleContorlByRoleIdAndModuleID2(int moduleId, int roleId)
        {
            return new DAL.Role().GeRoleModuleContorlByRoleIdAndModuleID2(moduleId, roleId);
        }
        #endregion

        #region 保存角色控制项数据
        /// <summary>
        /// 保存角色控制项数据
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <param name="ModuelsID">模块ID</param>
        /// <param name="ContortIDs">控制模块ID</param>
        public void Add(int RoleID, int ModuelsID, List<int> ContortIDs)
        {
            new DAL.Role().Add(RoleID, ModuelsID, ContortIDs);
        }
        #endregion
    }
}
