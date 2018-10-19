using BLL;
using Common;
using Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
namespace VIPCustomer.IServer.SysManage
{
    /// <summary>
    /// SysManage 的摘要说明
    /// </summary>
    public class SysManage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                #region 用户维护=============================
                case "SaveParsonInfo":
                    SaveParsonInfo(context);
                    break;
                case "SelfPassword":
                    SelfPassword(context);
                    break;
                case "GetUserManagePager":
                    GetUserManagePager(context);
                    break;
                case "GetUserType":
                    GetUserType(context);
                    break;
                case "GetDealerAll":
                    GetDealerAll(context);
                    break;
                case "SaveData":
                    SaveData(context);
                    break;
                case "DELETEDATE":
                    DELETEDATE(context);
                    break;
                case "GetSysUserByPKID":
                    GetSysUserByPKID(context);
                    break;
                case "GetRoleList":
                    GetRoleList(context);
                    break;
                case "GetRoleByUserId":
                    GetRoleByUserId(context);
                    break;
                case "SubmitUserRoleData":
                    SubmitUserRoleData(context);
                    break;

               
                #endregion

                #region 角色维护=============================
                case "GetRoleManagePager":
                    GetRoleManagePager(context);
                    break;
                case "GetRoleByPKID":
                    GetRoleByPKID(context);
                    break;
                case "SaveRoleData":
                    SaveRoleData(context);
                    break;

                case "DeleteRole":
                    DeleteRole(context);
                    break;

                case "GetRoleModuleByRoleIdTree":
                    GetRoleModuleByRoleIdTree(context);
                    break;

                case "SaveRoleModule":
                    SaveRoleModule(context);
                    break;

                case "GetBrandDictionaryByPageList":
                    GetBrandDictionaryByPageList(context);
                    break;

                case "GetRoleBrandByRoleID":
                    GetRoleBrandByRoleID(context);
                    break;

                case "SaveCars":
                    SaveCars(context);
                    break;

                case "GetControlByModule":
                    GetControlByModule(context);
                    break;

                case "GetControlInfo":
                    GetControlInfo(context);
                    break;

                case "SaveControl":
                    SaveControl(context);
                    break;
                #endregion

                #region 模块管理=============================
                case "LoadTree":
                    LoadTree(context);
                    break;

                case "GetModuleByPKID":
                    GetModuleByPKID(context);
                    break;

                case "SaveModule":
                    SaveModule(context);
                    break;
                case "DeleteTree":
                    DeleteTree(context);
                    break;

                case "GetModuleContorl":
                    GetModuleContorl(context);
                    break;

                case "MCSaveData":
                    MCSaveData(context);
                    break;

                case "ModuleCommandByPKID":
                    ModuleCommandByPKID(context);
                    break;

                case "DeleteModuleControl":
                    DeleteModuleControl(context);
                    break;

                case "MoveModules":
                    MoveModules(context);
                    break;
                #endregion

                #region 数据信息管理=============================
                case "PagerTypeList":
                    PagerTypeList(context);
                    break;

                case "PagerList":
                    PagerList(context);
                    break;

                case "submitTypeData":
                    submitTypeData(context);
                    break;

                case "submitListData":
                    submitListData(context);
                    break;

                case "GetDicDomainByCode":
                    GetDicDomainByCode(context);
                    break;

                case "GetDicDomainByPKID":
                    GetDicDomainByPKID(context);
                    break;

                case "DeleteDicDomainByCode":
                    DeleteDicDomainByCode(context);
                    break;

                case "DeleteItemsByPKID":
                    DeleteItemsByPKID(context);
                    break;


                #endregion

                #region 城市维护=============================
                case "GetCityManagePager":
                    GetCityManagePager(context);
                    break;

                case "GetList":
                    GetList(context);
                    break;

                case "GetProvince":
                    GetProvince(context);
                    break;

                case "GetCity":
                    GetCity(context);
                    break;

                case "SaveCityData":
                    SaveCityData(context);
                    break;

                case "SaveProvinceData":
                    SaveProvinceData(context);
                    break;

                case "GetCityDictionaryByPKID":
                    GetCityDictionaryByPKID(context);
                    break;
                    #endregion
            }
        }

        #region 用户维护=============================
        #region 保存个人信息
        public void SaveParsonInfo(HttpContext context)
        {
            int pkid = Convert.ToInt32(context.Request["HidID"]);
            string name = context.Request.Form["Name"];
            string emial = context.Request.Form["Email"];
            string phone = context.Request.Form["Phone"];
            SysUserEntity model = new SysUserEntity();
            model.PKID = pkid;
            model.Name = name;
            model.Email = emial;
            model.Phone = phone;
            if (new BLL.sysUser().SetBaseInfo(model))
            {
                context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
            }
        }
        #endregion

        #region 修改密码
        public void SelfPassword(HttpContext context)
        {
            int userId = Convert.ToInt32(context.Request["Pwd_PKID"]);
            if (userId > 0)
            {
                string LoginPwd = context.Request["LoginPwd"];
                string newPwd = context.Request["newPassword"];
                string newPwd2 = context.Request["newPassword2"];
                var modal = new BLL.sysUser().GetSysUserByPKID(userId);
                if (!modal.LoginPwd.Equals(Utils.MD5(Utils.MD5(LoginPwd))))
                {
                    context.Response.Write("{\"msg\":\"修改失败,旧密码输入有误！\",\"success\":false}");
                }
                else if (!Utils.MD5(Utils.MD5(newPwd)).Equals(Utils.MD5(Utils.MD5(newPwd2))))
                {
                    context.Response.Write("{\"msg\":\"修改失败,两次输入密码不一致!\",\"success\":false}");
                }
                else
                {
                    modal.PKID = userId;
                    modal.LoginPwd = Utils.MD5(Utils.MD5(newPwd2));
                    if (new BLL.sysUser().Update(modal))
                    {
                        context.Response.Write("{\"msg\":\"修改成功。\",\"success\":true}");
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"修改失败。\",\"success\":false}");
                    }
                }
            }
            else
            {
                return;
            }
        }
        #endregion

        #region 用户维护分页数据列表
        /// <summary>
        /// 用户维护分页数据列表
        /// </summary>
        /// <param name="context"></param>
        public void GetUserManagePager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<SysUserEntity> list = new List<SysUserEntity>();
            string loginName = context.Request.Form["LoginName"];
            string name = context.Request.Form["Name"];
            int userTypeId = Convert.ToInt32(context.Request.Form["UserTypeId"]);
            int isActivate = Convert.ToInt32(context.Request.Form["IsActivate"]);
            SysUserEntity filter = new SysUserEntity
            {
                LoginName = loginName,
                Name = name,
                UserTypeId = userTypeId,
                IsActivate = isActivate,
            };
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.sysUser().GetUserManagePager(pageIndex, pageSize, sort.SortName, sort.IsDesc, filter, out totalCount);
            }
            else
            {
                list = new BLL.sysUser().GetUserManagePager(pageIndex, pageSize, filter, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<SysUserEntity>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 用户类别下拉列表
        /// <summary>
        /// 用户类别下拉列表
        /// </summary>
        /// <param name="context"></param>
        public void GetUserType(HttpContext context)
        {
            DataTable dt = new BLL.sysUser().GetUserType();
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 获取所有经销店名称
        /// <summary>
        /// 获取所有经销店名称
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        public void GetDealerAll(HttpContext context)
        {
            DataTable dt = new BLL.sysUser().GetDealerAll("1=1");
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetSysUserByPKID(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["keyword"]);
            if (PKID > 0)
            {
                SysUserEntity model = new BLL.sysUser().GetSysUserByPKID(PKID);
                if (model != null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
            else
            {
                context.Response.Write("{\"msg\":\"新增操作。\",\"success\":true}");
            }
        }
        #endregion

        #region 保存数据
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="context">上下文对象</param>
        protected void SaveData(HttpContext context)
        {
            string LoginName = context.Request.Form["fr_LoginName"];
            int HidId = Convert.ToInt32(context.Request["HidId"]);// 判断是否大于0，大于0为修改
            string Name = context.Request.Form["fr_Name"];
            string LoginPwd = string.Empty;
            if (string.IsNullOrEmpty(context.Request.Form["LoginPwd"]))
            {
                LoginPwd = "123";
            }
            else
            {
                LoginPwd = context.Request.Form["LoginPwd"];
            }
            string Email = context.Request.Form["Email"];
            string Phone = context.Request.Form["Phone"];
            int DealerId = Convert.ToInt32(context.Request.Form["DealerId"]);
            int UserTypeId = Convert.ToInt32(context.Request.Form["fr_UserTypeId"]);
            int IsActivate = Convert.ToInt32(context.Request.Form["IsActivate"]);
            SysUserEntity model = new SysUserEntity();
            string Remark = context.Request.Form["Remark"];
            sysUser bll = new sysUser();
            if (HidId > 0)// 编辑
            {
                model.PKID = HidId;
                model.LoginName = LoginName;
                model.Name = Name;
                model.LoginPwd = Utils.MD5(Utils.MD5(LoginPwd));
                model.Email = Email;
                model.Phone = Phone;
                model.DealerId = DealerId;
                model.UserTypeId = UserTypeId;
                model.IsActivate = IsActivate;
                model.Remark = Remark;
                if (bll.Update(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
            else // 添加
            {
                model.LoginName = LoginName;
                model.Name = Name;
                model.LoginPwd = Utils.MD5(Utils.MD5(LoginPwd));
                model.Email = Email;
                model.Phone = Phone;
                model.DealerId = DealerId;
                model.UserTypeId = UserTypeId;
                model.IsActivate = IsActivate;
                model.Remark = Remark;
                if (bll.Exists(LoginName))
                {
                    context.Response.Write("{\"status\":1, \"msg\":\"" + LoginName + "\"}");
                }
                else if (bll.Add(model) > 0)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DELETEDATE(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["PKID"]);
            sysUser bll = new sysUser();
            bool result = bll.Delete(PKID);
            if (result)
            {
                context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
            }
        }
        #endregion

        #region 从服务器端获取角色分页列表
        /// <summary>
        /// 从服务器端获取角色分页列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetRoleList(HttpContext context)
        {
            List<RoleEntity> list = new List<RoleEntity>();
            list = new BLL.Role().GetRoleList();
            string resultJson = JsonConvert.SerializeObject(list);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 根据用户ID获取角色名称
        /// <summary>
        /// 根据用户ID获取角色名称
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetRoleByUserId(HttpContext context)
        {
            int userId = Convert.ToInt32(context.Request["keyword"]);
            if (userId > 0)
            {
                DataTable dt = new BLL.Role().GetRoleByUserId(userId);
                if (dt.Rows.Count > 0)
                {
                    context.Response.Write(JsonConvert.SerializeObject(dt));
                }
            }
        }
        #endregion

        #region 保存用户角色数据
        /// <summary>
        /// 保存用户角色数据
        /// </summary>
        /// <param name="context"></param>
        public void SubmitUserRoleData(HttpContext context)
        {
            int userId = Convert.ToInt32(context.Request["userId"]); // 单用户Id
            string RoleIds = context.Request["roleIds"] ?? "";// 角色Id可能是多个
            if (userId > 0)
            {
                if (new BLL.sysUser().SaveUserRoleDate(userId, RoleIds))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
        }
        #endregion

        #endregion

        #region 角色维护=============================
        #region 从服务器端获取分页列表
        /// <summary>
        /// 从服务器端获取分页列表
        /// </summary>
        /// <param name="context"></param>
        public void GetRoleManagePager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<RoleEntity> list = new List<RoleEntity>();
            string name = context.Request.Form["Name"];
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.Role().GetRoleManagePager(pageIndex, pageSize, sort.SortName, sort.IsDesc, name, out totalCount);
            }
            else
            {
                list = new BLL.Role().GetRoleManagePager(pageIndex, pageSize, name, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<RoleEntity>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetRoleByPKID(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["keyword"]);
            if (PKID > 0)
            {
                RoleEntity model = new BLL.Role().GetRoleByPKID(PKID);
                if (model != null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
        }
        #endregion

        #region 保存数据
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void SaveRoleData(HttpContext context)
        {
            int HidId = Convert.ToInt32(context.Request["HidId"]);
            string Name = context.Request.Form["fr_Name"];
            string Remark = context.Request.Form["Remark"];
            int IsActivate = Convert.ToInt32(context.Request.Form["IsActivate"]);
            int RoleGrade = Convert.ToInt32(context.Request.Form["RoleGrade"]);
            Role bll = new Role();
            RoleEntity model = new RoleEntity();
            if (HidId > 0)
            {
                model.PKID = HidId;
                model.Name = Name;
                model.Remark = Remark;
                model.RoleGrade = RoleGrade;
                model.IsActivate = IsActivate;
                if (bll.Update(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
            else
            {
                model.Name = Name;
                model.Remark = Remark;
                model.RoleGrade = RoleGrade;
                model.IsActivate = IsActivate;
                if (bll.Exists(Name))
                {
                    context.Response.Write("{\"status\":1, \"msg\":\"" + Name + "\"}");
                }
                else if (bll.Add(model) > 0)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DeleteRole(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["PKID"]);
            if (PKID > 0)
            {
                bool result = new BLL.Role().DELETEDATE(PKID);
                if (result)
                {
                    context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"删除失败,请重试。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 获取角色模块树节点信息
        /// <summary>
        /// 获取角色模块树节点信息
        /// </summary>
        /// <param name="context"></param>
        public void GetRoleModuleByRoleIdTree(HttpContext context)
        {
            int RoleID = Convert.ToInt32(context.Request["RoleId"]);
            string result = new BLL.Role().GetRoleModuleByRoleIdTree(RoleID);
            if (result != null)
            {
                context.Response.Write(result);
            }
        }
        #endregion

        #region 保存权限
        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void SaveRoleModule(HttpContext context)
        {
            string[] modules = context.Request["moules"].Split(',');
            int roleId = Convert.ToInt32(context.Request["roleId"]);
            List<int> moduleList = new List<int>();
            foreach (string item in modules)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    moduleList.Add(Convert.ToInt32(item));
                }
            }
            new BLL.Role().Add(roleId, moduleList);
        }
        #endregion

        #region 分页查询车型厂牌字典数据列表
        /// <summary>
        /// 分页查询车型厂牌字典数据列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetBrandDictionaryByPageList(HttpContext context)
        {
            List<BrandDictionaryEntity> list = new List<BrandDictionaryEntity>();
            list = new BLL.Role().GetBrandDictionaryByPageList();
            string resultJson = JsonConvert.SerializeObject(list);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 根据角色ID查询角色车型信息
        /// <summary>
        /// 根据角色ID查询角色车型信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetRoleBrandByRoleID(HttpContext context)
        {
            try
            {
                int RoleID = Convert.ToInt32(context.Request["RoleId"]);
                if (RoleID > 0)
                {
                    DataTable dt = new BLL.Role().GetRoleBrandByRoleID(RoleID);
                    if (dt.Rows.Count > 0)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(dt));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 保存设置车型数据
        /// <summary>
        /// 保存设置车型数据
        /// </summary>
        /// <param name="context"></param>
        public void SaveCars(HttpContext context)
        {
            int roleId = Convert.ToInt32(context.Request["roleId"]); // 单角色
            string carNames = context.Request["carNames"] ?? "";
            if (roleId > 0)
            {
                if (new BLL.Role().SaveCarRoleDate(roleId, carNames))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 根据不同角色查询不同页面对应的不同按钮控制
        /// <summary>
        /// 根据不同角色查询不同页面对应的不同按钮控制
        /// </summary>
        /// <param name="context"></param>
        public void GetControlByModule(HttpContext context)
        {
            int moduleId = Convert.ToInt32(context.Request["ModuleId"]);
            if (moduleId > 0)
            {
                List<ModuleCommandEntity> list = new BLL.Role().GetRoleModuleContortList(moduleId);
                string resultJson = JsonConvert.SerializeObject(list);
                context.Response.Write(resultJson);
            }
        }
        public void GetControlInfo(HttpContext context)
        {
            int moduleId = Convert.ToInt32(context.Request["ModuleId"]);
            int RoldId = Convert.ToInt32(context.Request["RoleId"]);
            DataTable dt = new Role().GeRoleModuleContorlByRoleIdAndModuleID2(moduleId, RoldId);
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 保存角色控制按钮设定项数据
        /// <summary>
        /// 保存角色控制按钮设定项数据
        /// </summary>
        /// <param name="context"></param>
        public void SaveControl(HttpContext context)
        {
            int roleId = Convert.ToInt32(context.Request["roleId"]);
            int moduleId = Convert.ToInt32(context.Request["moduleId"]);
            string[] ContortIds = context.Request["ContortIds"].Split(',');
            List<int> contortList = new List<int>();
            foreach (string item in ContortIds)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    contortList.Add(Convert.ToInt32(item));
                }
            }
            new BLL.Role().Add(roleId, moduleId, contortList);
        }
        #endregion
        #endregion

        #region 模块管理=============================

        #region 获取所有节点信息
        /// <summary>
        /// 获取所有节点信息
        /// </summary>
        /// <param name="context"></param>
        public void LoadTree(HttpContext context)
        {
            BLL.Module bll = new BLL.Module();
            string result = bll.GetAllModulesJson();
            context.Response.Write(result);
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetModuleByPKID(HttpContext context)
        {
            int nodeId = Convert.ToInt32(context.Request["PKID"]);
            if (nodeId > 0)
            {
                ModuleEntity model = new BLL.Module().GetModuleByPKID(nodeId);
                if (model != null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
        }
        #endregion

        #region 保存数据
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void SaveModule(HttpContext context)
        {
            string Name = context.Request["Name"];
            int IsActivate = int.Parse(context.Request["IsActivate"]);
            int IsSysModule = int.Parse(context.Request["IsSysModule"]);
            string PageAddress = context.Request["PageAddress"];
            string Remark = context.Request["Remark"];
            int opType = Convert.ToInt32(context.Request["opType"]);
            if (3 == opType)
            {
                // 编辑操作
                int pkid = Convert.ToInt32(context.Request["PKID"]);
                ModuleEntity editEntity = new ModuleEntity();
                editEntity.PKID = pkid;
                editEntity.Name = Name;
                editEntity.IsActivate = IsActivate;
                editEntity.IsSysModule = IsSysModule;
                editEntity.PageAddress = PageAddress;
                editEntity.Remark = Remark;
                bool result = new BLL.Module().Update(editEntity);
                if (result)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
            else
            {
                int ParentID = 0;
                // 添加:1、添加同级模块;2、添加子级模块;
                int.TryParse(context.Request["ParentID"], out ParentID);
                int id = Convert.ToInt32(context.Request["id"]);
                ModuleEntity addEntity = new ModuleEntity();
                addEntity.PKID = id;
                addEntity.ParentID = ParentID;
                addEntity.Name = Name;
                addEntity.IsActivate = IsActivate;
                addEntity.IsSysModule = IsSysModule;
                addEntity.PageAddress = PageAddress;
                addEntity.Remark = Remark;
                int result = new BLL.Module().Add(addEntity, opType);
                if (result > 0)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 删除模块信息
        /// <summary>
        /// 删除模块信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DeleteTree(HttpContext context)
        {
            string nodeIds = context.Request["PKID"];
            string delString = string.Format("'{0}'", nodeIds.Replace(",", "','"));
            bool result = new BLL.Module().Delete(delString);
            if (result)
            {
                context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
            }
        }
        #endregion

        #region MC 操作
        #region 根据模块ID获取模块控制数据列表
        /// <summary>
        /// 根据模块ID获取模块控制数据列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetModuleContorl(HttpContext context)
        {
            int moduleID = Convert.ToInt32(context.Request["ModuleID"]);
            List<ModuleCommandEntity> list = new BLL.Module().GetModuleContorlByModuleID(moduleID);
            if (list != null)
            {
                context.Response.Write(JsonConvert.SerializeObject(list));
            }
        }
        #endregion

        #region  保存数据
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void MCSaveData(HttpContext context)
        {
            int opTypeMC = Convert.ToInt32(context.Request["opTypeMC"]);
            string code = context.Request["Code"];
            string name = context.Request["Name"];
            string remark = context.Request["Remark"];
            if (3 == opTypeMC)
            {
                int PKID = Convert.ToInt32(context.Request["PKID"]);
                ModuleCommandEntity editEntity = new ModuleCommandEntity();
                editEntity.PKID = PKID;
                editEntity.Name = name;
                editEntity.Code = code;
                editEntity.Remark = remark;
                if (new BLL.Module().MCUpdate(editEntity))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
            else
            {
                int moduleID = Convert.ToInt32(context.Request["moduleID"]);
                ModuleCommandEntity model = new ModuleCommandEntity();
                model.ModuleID = moduleID;
                model.Code = code;
                model.Name = name;
                model.Remark = remark;
                int result = new BLL.Module().MCAdd(model);
                if (result > 0)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }

            }

        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void ModuleCommandByPKID(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["pkid"]);
            if (PKID > 0)
            {
                ModuleCommandEntity model = new BLL.Module().ModuleCommandByPKID(PKID);
                if (model != null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
        }
        #endregion

        #region 删除模块控制信息
        /// <summary>
        /// 删除键模块控制信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DeleteModuleControl(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["PKID"]);
            bool result = new BLL.Module().MCDelete(PKID);
            if (result)
            {
                context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"删除失败。\",\"success\":true}");
            }
        }
        #endregion

        #endregion

        #region 移动模块
        /// <summary>
        /// 移动模块
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void MoveModules(HttpContext context)
        {
            string moveType = context.Request["moveType"];
            int tID = Convert.ToInt32(context.Request["targetObj"]);
            int sID = Convert.ToInt32(context.Request["sourceObj"]);
            new BLL.Module().MoveTo(moveType, sID, tID);
        }
        #endregion

        #endregion

        #region 数据信息管理=========================
        #region 获取数据分类分页列表
        /// <summary>
        /// 获取数据分类分页列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void PagerTypeList(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.DICDomain> list = new List<Common.Entity.DICDomain>();
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.DICDomain().PagerTypeList(pageIndex, pageSize, sort.SortName, sort.IsDesc, out totalCount);
            }
            else
            {
                list = new BLL.DICDomain().PagerTypeList(pageIndex, pageSize, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.DICDomain>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 根据预编码查询数据信息分页
        /// <summary>
        /// 根据预编码查询数据信息分页
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void PagerList(HttpContext context)
        {
            string code = context.Request["keyword"];
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.DICDomain> list = new List<Common.Entity.DICDomain>();
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.DICDomain().PagerList(pageIndex, pageSize, code, sort.SortName, sort.IsDesc, out totalCount);
            }
            else
            {
                list = new BLL.DICDomain().PagerList(pageIndex, pageSize, code, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.DICDomain>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 保存分类数据
        public void submitTypeData(HttpContext context)
        {
            string name = context.Request["Name"];
            string HidCode = context.Request["HidCode"];
            Common.Entity.DICDomain model = new Common.Entity.DICDomain();
            BLL.DICDomain bll = new BLL.DICDomain();
            if (!string.IsNullOrEmpty(HidCode))
            {
                model.Code = HidCode;
                model.Name = name;
                if (bll.Update(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
            else
            {
                string code = context.Request["Code"];
                string listName = context.Request["ListName"];
                int sort = Convert.ToInt32(context.Request["Sort"]);
                string remark = context.Request["Remark"];
                model.Code = code;
                model.Name = name;
                model.ListName = listName;
                model.Sort = sort;
                model.Remark = remark;
                if (bll.ExistsDomain(name, code))
                {
                    context.Response.Write("{\"state\":1, \"msg\":\"" + code + "," + name + "\"}");
                }
                else if (bll.Add(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 保存列表数据
        /// <summary>
        /// 保存列表数据
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void submitListData(HttpContext context)
        {
            int HidID = Convert.ToInt32(context.Request["HidID"]);
            string HidCode2 = context.Request["HidCode2"];
            string hidName = context.Request["HidName"];
            string listName = context.Request["ListName"];
            int sort = Convert.ToInt32(context.Request["Sort"]);
            string remark = context.Request["Remark"];
            Common.Entity.DICDomain model = new Common.Entity.DICDomain();
            BLL.DICDomain bll = new BLL.DICDomain();
            if (HidID > 0)
            {
                model.PKID = HidID;
                model.ListName = listName;
                model.Sort = sort;
                model.Remark = remark;
                if (bll.UpdateDomainItem(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
            else
            {
                model.Code = HidCode2;
                model.Name = hidName;
                model.ListName = listName;
                model.Sort = Convert.ToInt32(context.Request["Sort"]);
                model.Remark = context.Request["Remark"];
                if (bll.ExistsDICDomainItem(HidCode2, listName))
                {
                    context.Response.Write("{\"state\":11, \"msg\":\"" + listName + "\"}");
                }
                else if (bll.Add(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 获取实体对象（根据域编码）
        /// <summary>
        /// 获取实体对象（根据域编码）
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetDicDomainByCode(HttpContext context)
        {
            string code = context.Request["keyword"];
            if (!string.IsNullOrEmpty(code))
            {
                Common.Entity.DICDomain model = new BLL.DICDomain().GetDicDomainByCode(code);
                if (model == null)
                    return;
                else
                    context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        #region 获取实体对象（根据主键ID）
        /// <summary>
        /// 获取实体对象（根据主键ID）
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetDicDomainByPKID(HttpContext context)
        {
            int pkid = Convert.ToInt32(context.Request["keyword"]);
            if (pkid > 0)
            {
                Common.Entity.DICDomain model = new BLL.DICDomain().GetDicDomainByPKID(pkid);
                if (model == null)
                    return;
                else
                    context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        #region 删除数据信息(根据域编码)
        /// <summary>
        ///  删除数据信息(根据域编码)
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DeleteDicDomainByCode(HttpContext context)
        {
            string code = context.Request["keyword"];
            if (!string.IsNullOrEmpty(code))
            {
                if (new BLL.DICDomain().DeleteDicDomainByCode(code))
                {
                    context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 删除数据信息(根据主键ID)
        /// <summary>
        /// 删除数据信息(根据主键ID)
        /// </summary>
        /// <param name="context"></param>
        public void DeleteItemsByPKID(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["keyword"]);
            if (new BLL.DICDomain().DeleteItemsByPKID(PKID))
            {
                context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
            }
        }
        #endregion
        #endregion

        #region 城市维护=============================
        #region 从服务器端获取城市维护分页列表
        /// <summary>
        /// 从服务器端获取城市维护分页列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCityManagePager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.CityDictionary> list = new List<Common.Entity.CityDictionary>();
            string name = context.Request.Form["Name"];
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.CityManager().GetCityManagePager(pageIndex, pageSize, sort.SortName, sort.IsDesc, name, out totalCount);
            }
            else
            {
                list = new BLL.CityManager().GetCityManagePager(pageIndex, pageSize, name, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.CityDictionary>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 获得大区或省份城市列表
        /// <summary>
        /// 获得大区或省份城市列表
        /// </summary>
        /// <param name="context"></param>
        public void GetList(HttpContext context)
        {
            DataTable dt = new BLL.CityManager().GetList();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 根据大区获取对应的省
        /// <summary>
        /// 根据大区获取对应的省
        /// </summary>
        /// <param name="context"></param>
        public void GetProvince(HttpContext context)
        {
            int cityId = Convert.ToInt32(context.Request["cityId"]);
            if (cityId > 0)
            {
                DataTable dt = new BLL.CityManager().GetProvince(cityId);
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 根据省获取对应的城市
        /// <summary>
        /// 根据省获取对应的城市
        /// </summary>
        /// <param name="context"></param>
        public void GetCity(HttpContext context)
        {
            int cityId = Convert.ToInt32(context.Request["cityId"]);
            if (cityId > 0)
            {
                DataTable dt = new BLL.CityManager().GetCity(cityId);
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCityDictionaryByPKID(HttpContext context)
        {
            int cityId = Convert.ToInt32(context.Request["cityId"]);
            if (cityId > 0)
            {
                Common.Entity.CityDictionary model = new Common.Entity.CityDictionary();
                model = new BLL.CityManager().GetCityDictionaryByPKID(cityId);
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        #region 保存城市数据
        /// <summary>
        /// 保存城市数据
        /// </summary>
        /// <param name="context"></param>
        public void SaveCityData(HttpContext context)
        {
            int HidId = Convert.ToInt32(context.Request["HidId"]);
            string city = context.Request["City"];
            int provice = Convert.ToInt32(context.Request["fr_Province"]);
            Common.Entity.CityDictionary model = new Common.Entity.CityDictionary();
            BLL.CityManager bll = new CityManager();
            if (HidId > 0)
            {
                model.ParentID = provice;
                model.City = city;
                model.PKID = HidId;
                if (bll.Update(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
            else
            {
                model.ParentID = provice;
                model.City = city;
                if (bll.Exists(provice, city))
                {
                    context.Response.Write("{\"msg\":\"已经存在该城市信息。\",\"state\":\"T\"}");
                }
                else if (bll.Add(model) > 0)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 保存省数据
        /// <summary>
        /// 保存省数据
        /// </summary>
        /// <param name="context"></param>
        public void SaveProvinceData(HttpContext context)
        {
            int hidPKID = Convert.ToInt32(context.Request["hidPKID"]);
            string province = context.Request["Province"];
            int countryId = Convert.ToInt32(context.Request["cboBigArea"]);// 添加省，需要
            int hidProvince = Convert.ToInt32(context.Request["hidProvince"]);// 编辑省，需要
            Common.Entity.CityDictionary model = new Common.Entity.CityDictionary();
            BLL.CityManager bll = new CityManager();
            if (hidPKID > 0)
            {
                model.ParentID = countryId;
                model.City = province;
                model.PKID = hidProvince;
                if (bll.Update(model))
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
            else
            {

                model.ParentID = countryId;
                model.City = province;
                if (bll.Exists(countryId, province))
                {
                    context.Response.Write("{\"msg\":\"已经存在该城市信息。\",\"state\":\"T\"}");
                }
                else if (bll.Add(model) > 0)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败,请重试。\",\"success\":false}");
                }
            }
        }
        #endregion
        #endregion
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}