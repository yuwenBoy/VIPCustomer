using Common;
using DAL;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace BLL
{
    /// <summary>
    /// 模块维护业务逻辑层
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
            return new DAL.Module().GetAllModulesJson();
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
            return new DAL.Module().GetModuleByPKID(nodeId);
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
            return new DAL.Module().GetModuleOrder(PKID);
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
            //获取排序，前台不保留排序
            model.Sort = GetModuleOrder(model.PKID);
            return new DAL.Module().Update(model);
        }
        #endregion

        #region 添加子级修改排序
        /// <summary>
        /// 添加子级修改排序
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        public int GetMaxOrder(int parentId)
        {
            return new DAL.Module().GetMaxOrder(parentId);
        }
        #endregion

        #region 新增一条数据
        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <returns></returns>
        public int Add(ModuleEntity model, int addType)
        {
            if (1 == addType)
            {
                // 添加同级
                ModuleEntity smSender = GetModuleByPKID(model.PKID);
                model.Sort = smSender.Sort + 1;
                model.ParentID = smSender.ParentID;
            }
            else
            {
                // 添加子级
                ModuleEntity smSender = GetModuleByPKID(model.PKID);
                model.ParentID = smSender.PKID;
                model.Sort = GetMaxOrder(model.PKID) + 1;
            }
            return new DAL.Module().Add(model);
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
            return new DAL.Module().Delete(nodeIds);
        }
        #endregion

        #region MC操作======================
        #region 根据模块ID查询模块控制数据列表
        /// <summary>
        /// 根据模块ID查询模块控制数据列表
        /// </summary>
        /// <param name="moduleID">模块ID</param>
        public List<ModuleCommandEntity> GetModuleContorlByModuleID(int moduleID)
        {
            return new DAL.Module().GetModuleContorlByModuleID(moduleID);
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
            return new DAL.Module().MCAdd(model);
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
            return new DAL.Module().MCUpdate(model);
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
            return new DAL.Module().ModuleCommandByPKID(PKID);
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
            return new DAL.Module().MCDelete(PKID);
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
        public void MoveTo(string moveType, int sID, int tID)
        {
            ModuleEntity source = GetModuleByPKID(sID);
            ModuleEntity target = GetModuleByPKID(tID);
            new DAL.Module().MoveTo(moveType, source, target);
        }
        #endregion

        #region 根据用户ID获取左侧导航菜单
        public class MenuClass
        {
            public int MenuId { get; set; }
            public string MenuName { get; set; }
            public string IsSystem { get; set; }
            public string ImageAddress { get; set; }
            public string MenuUrl { get; set; }
            public int ParentID { get; set; }
            public List<MenuClass> Children { get; set; }

        }
        public List<MenuClass> GetCTopMenu(int userId)
        {
            DataTable dt = new DAL.Module().GetCTopMenu(userId);
            List<MenuClass> list = new List<MenuClass>();
            DataRow[] rows = dt.Select("ParentID=0");
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    MenuClass firstMenu = new MenuClass
                    {
                        MenuId = Convert.ToInt32(rows[i]["PKID"].ToString()),
                        MenuName = rows[i]["Name"].ToString(),
                        ParentID = Convert.ToInt32(rows[i]["ParentID"].ToString()),
                        IsSystem = rows[i]["IsSysModule"].ToString(),
                        ImageAddress = rows[i]["ImagesAddress"].ToString(),
                    };
                    list.Add(firstMenu);
                    DataRow[] r_list = dt.Select(string.Format("ParentID={0}", rows[i]["PKID"]));
                    if (r_list.Length > 0)
                    {
                        firstMenu.Children = new List<MenuClass>();
                        for (int j = 0; j < r_list.Length; j++)
                        {
                            MenuClass secondMenu = new MenuClass
                            {
                                MenuId = Convert.ToInt32(r_list[j]["PKID"].ToString()),
                                MenuName = r_list[j]["Name"].ToString(),
                                ParentID = Convert.ToInt32(r_list[j]["ParentID"].ToString()),
                                IsSystem = r_list[j]["IsSysModule"].ToString(),
                                ImageAddress = r_list[j]["ImagesAddress"].ToString(),
                            };
                            firstMenu.Children.Add(secondMenu);
                            DataRow[] rows2 = dt.Select(string.Format("ParentID={0}", r_list[j]["PKID"]));
                            if (rows2.Length > 0)
                            {
                                secondMenu.Children = new List<MenuClass>();
                                for (int k = 0; k < rows2.Length; k++)
                                {
                                    MenuClass threeMenu = new MenuClass
                                    {
                                        MenuId = Convert.ToInt32(rows2[k]["PKID"].ToString()),
                                        MenuName = rows2[k]["Name"].ToString(),
                                        ParentID = Convert.ToInt32(rows2[k]["ParentID"].ToString()),
                                        IsSystem = rows2[k]["IsSysModule"].ToString(),
                                        ImageAddress = rows2[k]["ImagesAddress"].ToString(),
                                        MenuUrl = rows2[k]["PageAddress"].ToString(),
                                    };
                                    secondMenu.Children.Add(threeMenu);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
        #endregion

        /// <summary>
        /// 根据路径获取模块信息
        /// </summary>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public ModuleEntity GetModuleByPath(string pagePath)
        {
            return new DAL.Module().GetModuleByPath(pagePath);
        }
    }
}
