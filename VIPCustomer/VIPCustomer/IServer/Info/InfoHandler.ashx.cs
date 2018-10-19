using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utilities;
using System.Web.SessionState;
using BLL;
using System.Collections;

namespace VIPCustomer.IServer.Info
{
    /// <summary>
    /// InfoHandler 的摘要说明
    /// </summary>
    public class InfoHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "GetInfoListPager":
                    GetInfoListPager(context);
                    break;

                case "GetTypeListJson":
                    GetTypeListJson(context);
                    break;

                case "GetEmergencyDegreeListJson":
                    GetEmergencyDegreeListJson(context);
                    break;

                case "addRecord":
                    addRecord(context);
                    break;

                case "UpdateRecord":
                    UpdateRecord(context);
                    break;

                case "DELETEDATE":
                    DELETEDATE(context);
                    break;

                case "GetModel":
                    GetModel(context);
                    break;
            }
        }

        #region 维护通知列表
        public void GetInfoListPager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.InfoList> list = new List<Common.Entity.InfoList>();
            string loginName = context.Request.Form["LoginName"];
            string name = context.Request.Form["Name"];
            int userTypeId = Convert.ToInt32(context.Request.Form["UserTypeId"]);
            int isActivate = Convert.ToInt32(context.Request.Form["IsActivate"]);
            Common.Entity.InfoList filter = new Common.Entity.InfoList
            {
                //LoginName = loginName,
                //Name = name,
                //UserTypeId = userTypeId,
                //IsActivate = isActivate,
            };
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.InfoList().GetInfoManagePager(pageIndex, pageSize, sort.SortName, sort.IsDesc, filter, out totalCount);
            }
            else
            {
                list = new BLL.InfoList().GetInfoManagePager(pageIndex, pageSize, filter, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.InfoList>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 获取信息列表类型
        public void GetTypeListJson(HttpContext context)
        {
            BLL.DICDomain domainBLL = new BLL.DICDomain();
            List<Common.Entity.DICDomain> list = new List<Common.Entity.DICDomain>();
            list = domainBLL.GetTypeListJson();
            context.Response.Write(JsonConvert.SerializeObject(list));
        }
        #endregion

        #region 获取信息列表紧急程度
        public void GetEmergencyDegreeListJson(HttpContext context)
        {
            BLL.DICDomain domainBLL = new BLL.DICDomain();
            List<Common.Entity.DICDomain> list = new List<Common.Entity.DICDomain>();
            list = domainBLL.GetEmergencyDegreeListJson();
            context.Response.Write(JsonConvert.SerializeObject(list));
        }
        #endregion

        #region 新建信息
        /// <summary>
        /// 新建信息
        /// </summary>
        /// <param name="context"></param>
        public void addRecord(HttpContext context)
        {
            BLL.InfoList infoList = new BLL.InfoList();
            string json = context.Request["param"];
            Common.Entity.InfoList info = JsonConvert.DeserializeObject<Common.Entity.InfoList>(json);
            string a = context.Session["Name"].ToString();
            info.Author = context.Session["Name"].ToString(); // Session ,必须实现接口
            info.CreateTime = DateTime.Now;
            info.ModifyDate = DateTime.Now;
            if (context.Session["infoID"] != null && context.Session["addAttachmentList"] != null)
            {
                try
                {

                }
                catch (Exception ex)
                {

                    throw;
                }
                finally
                {
                    context.Session.Remove("infoID");
                    context.Session.Remove("addAttachmentList");
                }
            }

            else
            {
                int result = infoList.Add(info);
                if (result > 0)
                {
                    context.Response.Write("{\"msg\":\"添加成功\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"添加失败。\",\"success\":false}");
                }
            }

        }
        #endregion

        #region 更新信息
        public void UpdateRecord(HttpContext context)
        {
            string json = context.Request["param"];
            string dealerIDs = context.Request["dealerIds"];
            Common.Entity.InfoList info = JsonConvert.DeserializeObject<Common.Entity.InfoList>(json);
            info.ModifyDate = DateTime.Now;
            int result = 0;
            try
            {
                BLL.InfoList infoList = new BLL.InfoList();
                BLL.InfoAttach attachList = new InfoAttach();
                // 删除附件
                if (context.Session["deleteAttachmentList"] != null)
                {
                    ArrayList deleteAttachmentList = context.Session["deleteAttachmentList"] as ArrayList;
                    attachList.DeleteByList(deleteAttachmentList);
                }
                if (context.Session["infoID"] != null && context.Session["deleteAttachmentList"] != null)
                {
                    // 保存信息
                    result = infoList.Update(info);
                    if (result > 0)
                    {
                        ArrayList addAttachmentList = (ArrayList)context.Session["addAttachmentList"];
                        attachList.AddAttachment(info.PKID, addAttachmentList);
                        context.Response.Write("{\"msg\":\"更新成功\",\"success\":true}");
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"更新失败\",\"success\":false}");
                    }
                }
                else
                {
                    result = infoList.Update(info);
                    if (result > 0)
                    {
                        context.Response.Write("{\"msg\":\"更新成功\",\"success\":true}");
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"更新失败\",\"success\":false}");
                    }
                }
                if (!string.IsNullOrEmpty(dealerIDs))
                {
                    infoList.InsertDealerInfo(info.PKID, dealerIDs.Remove(dealerIDs.Length - 2, 2).Remove(0, 2).Replace("\",\"", ","));
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion


        #region 获取实体对象
        public void GetModel(HttpContext context)
        {
            int InfoID = Convert.ToInt32(context.Request["InfoID"]);
            if (InfoID > 0)
            {
                Common.Entity.InfoList entity = new BLL.InfoList().GetModel(InfoID);
                context.Response.Write(JsonConvert.SerializeObject(entity));
            }
            return;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="context"></param>
        public void DELETEDATE(HttpContext context)
        {
            int pkid = Convert.ToInt32(context.Request["PKID"]);
            if (pkid > 0)
            {
                int result = new BLL.InfoList().DELETEDATE(pkid);
                if (result > 0)
                {
                    context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
                }
            }
            else
            {
                context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
            }
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