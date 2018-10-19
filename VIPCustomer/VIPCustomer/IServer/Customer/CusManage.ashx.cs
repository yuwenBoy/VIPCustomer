using BLL;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace VIPCustomer.IServer.Customer
{
    /// <summary>
    /// Customer 的摘要说明
    /// </summary>
    public class CusManage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "GetCustomerManagerPager":
                    GetCustomerManagerPager(context);
                    break;

                case "GetList":
                    GetList(context);
                    break;

                case "GetCustomerContact2":
                    GetCustomerContact2(context);
                    break;

                case "GetCustomerContactsPager":
                    GetCustomerContactsPager(context);
                    break;

                case "GetCustomerPagerByDealerID":
                    GetCustomerPagerByDealerID(context);
                    break;

                case "GetJobList":
                    GetJobList(context);
                    break;

                case "SaveCustomer":
                    SaveCustomer(context);
                    break;

                case "GetCustomerByPKID":
                    GetCustomerByPKID(context);
                    break;

                case "DeleteData":
                    DeleteData(context);
                    break;
            }
        }

        #region 从服务器端获取分页数据
        /// <summary>
        /// 从服务器端获取分页数据
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCustomerManagerPager(HttpContext context)
        {
            string username = context.User.Identity.Name;
            int DealerId = 0;
            SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            DealerId = model.DealerId;
            int totalCount = 0; // 总记录数
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页
            int pageSize = Convert.ToInt32(context.Request["rows"]); // 页码大小
            Common.Entity.Customer search = new Common.Entity.Customer();

            if (!string.IsNullOrEmpty(context.Request.Form["CustomersearchContext"]))
            {
                search = JsonConvert.DeserializeObject<Common.Entity.Customer>(context.Request.Form["CustomersearchContext"]);
            }

            var list = new BLL.Customer().GetDealerManagerPager(pageIndex, pageSize, DealerId, search, out totalCount);
            int totalPages = CommonFunction.CalculateTotalPage(pageSize, totalCount);
            var ResultData = new CommonFunction.DataResult<Common.Entity.Customer>()
            {
                totalCount = totalCount,
                totalpages = totalPages,
                currPage = pageIndex,
                dataList = list,
            };
            context.Response.Write(JsonConvert.SerializeObject(ResultData));
        }
        #endregion

        #region 获取客户性质一级列表
        /// <summary>
        /// 获取客户性质一级列表
        /// </summary>
        /// <param name="context"></param>
        public void GetList(HttpContext context)
        {
            DataTable dt = new BLL.Customer().GetList();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 获取客户性质二级列表
        /// <summary>
        /// 获取客户性质二级列表
        /// </summary>
        /// <param name="context"></param>
        public void GetCustomerContact2(HttpContext context)
        {
            int contactId = Convert.ToInt32(context.Request.Form["contactId"]);
            if (contactId > 0)
            {
                DataTable dt = new BLL.Customer().GetCustomerContact2(contactId);
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 根据客户ID查询客户联系人信息
        /// <summary>
        /// 根据客户ID查询客户联系人信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCustomerContactsPager(HttpContext context)
        {
            try
            {
                List<Common.Entity.CustomerContact> resultBuy = new List<Common.Entity.CustomerContact>();
                string customerId = context.Request["customerId"];
                if (string.IsNullOrEmpty(customerId))
                {
                    var model = new CommonFunction.DataResult<Common.Entity.CustomerContact>()
                    {
                        dataList = resultBuy,
                    };
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
                else
                {
                    resultBuy = new BLL.Customer().GetCustomerContactsPager(customerId);
                    var model = new CommonFunction.DataResult<Common.Entity.CustomerContact>()
                    {
                        dataList = resultBuy,
                    };
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取客户联系人角色列表
        /// <summary>
        /// 获取客户联系人角色列表
        /// </summary>
        /// <param name="context"></param>
        public void GetJobList(HttpContext context)
        {
            DataTable dt = new BLL.Customer().GetJobList();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 获取客户信息实体对象
        /// <summary>
        /// 获取客户信息实体对象
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCustomerByPKID(HttpContext context)
        {
            int customerId = Convert.ToInt32(context.Request["customerId"]);
            if (customerId > 0)
            {
                Common.Entity.Customer model = new Common.Entity.Customer();
                model = new BLL.Customer().GetCustomerByPKID(customerId);
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        #region 保存客户信息
        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void SaveCustomer(HttpContext context)
        {
            try
            {
                string opType = context.Request.Form["opType"];
                int DealerId = Convert.ToInt32(context.Request.Form["DealerId"]);
                int ContactName = Convert.ToInt32(context.Request.Form["ContactName"]);
                int ContactName2 = Convert.ToInt32(context.Request.Form["ContactName2"]);
                string EnterpriseCode = context.Request.Form["EnterpriseCode"];
                string Name = context.Request.Form["Name"];
                string Zip = context.Request.Form["Zip"];
                string Address = context.Request.Form["Address"];
                string Eamil = context.Request.Form["Eamil"];
                string Phone = context.Request.Form["Phone"];
                DateTime CreateTime = Convert.ToDateTime(context.Request.Form["CreateTime"]);
                string CompetentDepartment = context.Request.Form["CompetentDepartment"];
                string ExecutiveDepartment = context.Request.Form["ExecutiveDepartment"];
                string MainBusiness = context.Request.Form["MainBusiness"];
                string Remark = context.Request.Form["Remark"];
                string contactData = context.Request["contactData"];
                BLL.Customer bll = new BLL.Customer();
                List<Common.Entity.CustomerContact> Contactlist = JsonConvert.DeserializeObject<List<Common.Entity.CustomerContact>>(contactData); // 获取客户联系人信息
                if ("update".Equals(opType))
                {
                    // 编辑
                    int cusId = Convert.ToInt32(context.Request["cusId"]); // 客户ID

                    if (cusId > 0)
                    {
                        Common.Entity.Customer entity = new Common.Entity.Customer();
                        entity.PKID = cusId;
                        entity.DealerID = DealerId;
                        entity.CustomerNatureID = ContactName;
                        entity.CustomerNature2ID = ContactName2;
                        entity.EnterpriseCode = EnterpriseCode;
                        entity.Name = Name;
                        entity.Zip = Zip;
                        entity.Address = Address;
                        entity.Eamil = Eamil;
                        entity.Phone = Phone;
                        entity.CreateTime = CreateTime;
                        entity.CompetentDepartment = CompetentDepartment;
                        entity.ExecutiveDepartment = ExecutiveDepartment;
                        entity.MainBusiness = MainBusiness;
                        entity.Remark = Remark;
                        bool result = bll.Update(entity, Contactlist);

                        if (result)
                        {
                            context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                        }
                        else
                        {
                            context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                        }
                    }
                }
                else
                {
                    // 添加
                    Common.Entity.Customer model = new Common.Entity.Customer();
                    model.DealerID = DealerId;
                    model.CustomerNatureID = ContactName;
                    model.CustomerNature2ID = ContactName2;
                    model.EnterpriseCode = EnterpriseCode;
                    model.Name = Name;
                    model.Zip = Zip;
                    model.Address = Address;
                    model.Eamil = Eamil;
                    model.Phone = Phone;
                    model.CreateTime = CreateTime;
                    model.CompetentDepartment = CompetentDepartment;
                    model.ExecutiveDepartment = ExecutiveDepartment;
                    model.MainBusiness = MainBusiness;
                    model.Remark = Remark;

                    if (bll.Exists(model.EnterpriseCode))
                    {
                        context.Response.Write("{\"msg\":\"客户信息已存在，请修改。\",\"state\":\"T\"}");
                    }
                    else if (bll.AddCustomer(model, Contactlist) > 0)
                    {
                        context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 批量删除客户信息
        public void DeleteData(HttpContext context)
        {
            string customerIds = context.Request["customerIds"];
            bool result = new BLL.Customer().DeleteData(customerIds);
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

        #region 根据登录用户经销店获取客户信息
        /// <summary>
        /// 根据登录用户经销店获取客户信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCustomerPagerByDealerID(HttpContext context)
        {
            string username = context.User.Identity.Name;
            SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            int dealerId = model.DealerId;
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.Customer> list = new List<Common.Entity.Customer>();
            string keyword = context.Request.Form["keyword"];
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.Customer().GetCustomerPagerByDealerID(pageIndex, pageSize, dealerId, sort.SortName, sort.IsDesc, keyword, out totalCount);
            }
            else
            {
                list = new BLL.Customer().GetCustomerPagerByDealerID(pageIndex, pageSize, dealerId, keyword, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.Customer>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
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