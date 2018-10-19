using Common;
using Common.Entity;
using Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.SessionState;
using static BLL.Module;

namespace VIPCustomer.IServer
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler, IRequiresSessionState
    {
        protected List<SysUserModuleRight> rigthList;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string action = context.Request["action"];
            switch (action)
            {
                case "UserLogin":
                    UserLogin(context);
                    break;

                case "getUser":
                    if (context.Request.IsAuthenticated)// 判断是否通过了forms身份验证请求
                    {
                        getLogin(context);
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"nocookie\",\"success\":false}");
                    }
                    break;

                case "GetAuthorizeMenuButton":
                    if (context.Request.IsAuthenticated)
                    {
                        GetAuthorizeMenuButton(context);
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"" + context.Request.ApplicationPath + "Login.html\",\"success\":false}");
                    }
                    break;

                case "LoginOut":
                    LoginOut(context);
                    break;

                case "loadInfo":
                    loadInfo(context);
                    break;
            }
        }

        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void UserLogin(HttpContext context)
        {
            string username = context.Request.Form["username"];
            string password = context.Request.Form["password"];
            #region 非空验证
            if (string.IsNullOrEmpty(username.Trim()))
            {
                context.Response.Write("{\"msg\":\"用户名不能为空，请仔细检查输入的用户名。\",\"state\":0}");
                return;
            }
            if (string.IsNullOrEmpty(password.Trim()))
            {
                context.Response.Write("{\"msg\":\"密码不能为空，请仔细检查输入的用户名。\",\"state\":0}");
                return;
            }
            #endregion
            #region 数据库验证
            // 用户是否存在
            var userInfo = new BLL.sysUser().GetSysUserByLoginName(username);
            if (userInfo == null)
            {
                context.Response.Write("{\"msg\":\"用户名不存在，请仔细检查输入的用户名。\",\"state\":0}");
                return;
            }
            // 密码不匹配
            if (!userInfo.LoginPwd.Equals(Utils.MD5(Utils.MD5(password))))
            {
                context.Response.Write("{\"msg\":\"登录失败，用户名或密码错误。\",\"state\":0}");
                return;
            }
            // 判断当前账号是否被启用
            if (userInfo.IsActivate == 0)
            {
                context.Response.Write("{\"msg\":\"当前账号未被启用，请联系管理人员激活。\",\"state\":0}");
                return;
            }
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, userInfo.LoginName, DateTime.Now, DateTime.Now.AddDays(1), false, new JavaScriptSerializer().Serialize(userInfo));
            string encTicke = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicke);
            cookie.Expires = DateTime.MaxValue; // 设置cookie过期时间
            cookie.Path = FormsAuthentication.FormsCookiePath;
            context.Response.Cookies.Add(cookie);

            context.Response.Write("{\"msg\":\"登录成功。\",\"state\":1}");
            #endregion
        }
        #endregion

        protected void getLogin(HttpContext context)
        {
            string userID = context.User.Identity.Name;
            if (context.Session["LoginName"] == null
                || context.Session["PKID"] == null
                || context.Session["Name"] == null
                || context.Session["DealerId"] == null
                || context.Session["DealerName"] == null
                || context.Session["LoginName"].ToString() != userID
                )
            {
                SysUserEntity user = new BLL.sysUser().GetSysUserByLoginName(userID);
                context.Session["LoginName"] = user.LoginName;
                context.Session["PKID"] = user.PKID;
                context.Session["Name"] = user.Name;
                context.Session["DealerId"] = user.DealerId;
                context.Session["DealerName"] = user.DealerName;
                context.Session["UserTypeId"] = user.UserTypeId;
                List<string> list = new BLL.sysUser().GetUserCarNames(user.PKID);
                context.Session["UserCar"] = string.Join(",", list.ToArray<string>());
                var data = new
                {
                    authorizeMenu = this.GetMenuList(user.PKID),
                    userLoginInfo = user,
                };
                if (data == null)
                {
                    FormsAuthentication.SignOut();
                    context.Response.Write("{\"msg\":\"nocookie\",\"success\":false}");
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(data));
                }
            }
        }

        /// <summary>
        /// 获取用户权限操作按钮
        /// </summary>
        /// <param name="context"></param>
        public void GetAuthorizeMenuButton(HttpContext context)
        {
            string pagePath = context.Request["moduleURL"];
            int userID = Convert.ToInt32(context.Session["PKID"]);
            if (userID > 0 && pagePath != null)
            {
                try
                {
                    string noRigthUrl = string.Format("/Page/CommonPage/NoRight.html");
                    BLL.Module moduleBiz = new BLL.Module();
                    int moduleID = moduleBiz.GetModuleByPath(pagePath).PKID;
                    BLL.sysUser sysUserBiz = new BLL.sysUser();
                    if (sysUserBiz.GetUserModuleRigth(userID, moduleID))
                    {
                        rigthList = sysUserBiz.GetUserModuleControl(userID, moduleID);
                        string s = "{\"msg\":\"" + JsonConvert.SerializeObject(rigthList) + "\",\"success\":true}";
                        context.Response.Write(JsonConvert.SerializeObject(rigthList));
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"" + noRigthUrl + "\",\"success\":false}");
                    }
                }
                catch
                {

                    throw;
                }
            }
            else
            {
                context.Response.Write("{\"msg\":\"" + context.Request.ApplicationPath + "Login.html\",\"success\":false}");
            }
        }

        /// <summary>
        /// 功能是否可用
        /// </summary>
        /// <param name="controName"></param>
        /// <returns></returns>
        protected bool IsValidFunction(string controName)
        {
            if (null == rigthList)
            {

                return false;
            }
            IEnumerable<Common.Entity.SysUserModuleRight> cRgith =
                rigthList.Where(r => r.ControlName.ToLower() == controName.ToLower());
            foreach (Common.Entity.SysUserModuleRight ur in cRgith)
            {
                if (ur.IsRight)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取权限菜单
        /// </summary>
        /// <param name="userId">用户登录ID</param>
        /// <returns></returns>
        public object GetMenuList(int userId)
        {
            List<MenuClass> menuJson = new BLL.Module().GetCTopMenu(userId);
            return menuJson;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void loadInfo(HttpContext context)
        {
            int userType = (context.Session["DealerId"] == null || int.Parse(context.Session["DealerId"].ToString()) == 0) ? 2 : 1;
            if (userType == 2)
            {
                context.Response.Write("{\"msg\":\"大客户室用户\",\"success\":false}");
            }
            else
            {

            }
        }

        #region 退出登录
        public void LoginOut(HttpContext context)
        {
            context.Session["UserCar"] = null;
            context.Session["Name"] = null;
            context.Session["DealerID"] = null;
            context.Session["UserID"] = null;
            context.Session["DealerName"] = null;
            context.Session.Clear();
            FormsAuthentication.SignOut();
            context.Response.Write("{\"msg\":\"退出成功！\",\"success\":true}");
        }
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