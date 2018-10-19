using BLL;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace VIPCustomer.IServer.Dealer
{
    /// <summary>
    /// Dealer 的摘要说明
    /// </summary>
    public class Dealer : IHttpHandler
    {
        private int dealerId = 0;
        private string userName = string.Empty;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "GetDealerListPager":
                    GetDealerListPager(context);
                    break;


                case "GetLastDate":
                    GetLastDate(context);
                    break;

                case "SetLastDate":
                    SetLastDate(context);
                    break;

                case "GetUserDealerByPKID":
                    GetUserDealerByPKID(context);
                    break;

                #region 经销店联系人==========================
                case "GetDealerContactManagerPager":
                    GetDealerContactManagerPager(context);
                    break;

                case "GetJobList":
                    GetJobList(context);
                    break;

                case "SaveDealerContact":
                    SaveDealerContact(context);
                    break;

                case "DeleteDealerContact":
                    DeleteDealerContact(context);
                    break;

                case "GetDealerContactByPKID":
                    GetDealerContactByPKID(context);
                    break;
                #endregion

                #region 经销店管理============================
                case "GetDealerManagePager":
                    GetDealerManagePager(context);
                    break;

                case "SaveDealer":
                    SaveDealer(context);
                    break;

                case "GetDealerByPKID":
                    GetDealerByPKID(context);
                    break;

                case "DeleteData":
                    DeleteData(context);
                    break;
                #endregion

                #region 经销店库存上报============================
                case "GetDealerStockPagerList":
                    GetDealerStockPagerList(context);
                    break;
                case "SaveData":
                    SaveData(context);
                    break;
                case "DeleteDealerStockByPKID":
                    DeleteDealerStockByPKID(context);
                    break;
                    #endregion
            }
        }

        #region 获取所有经销店列表
        /// <summary>
        /// 获取所有经销店列表
        /// </summary>
        /// <param name="context"></param>
        public void GetDealerListPager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.Dealer> list = new List<Common.Entity.Dealer>();
            string keyword = context.Request.Form["keyword"];
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.Dealer().GetDealerListPager(pageIndex, pageSize, sort.SortName, sort.IsDesc, keyword, out totalCount);
            }
            else
            {
                list = new BLL.Dealer().GetDealerListPager(pageIndex, pageSize, keyword, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.Dealer>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 查询权限
        public void GetLastDate(HttpContext context)
        {
            string code = context.Request.Form["code"];
            if (!string.IsNullOrEmpty(code))
            {
                Common.Entity.Dealer entity = new BLL.Dealer().GetDealerByCode(code);
                int dealerId = entity.PKID;
                var obj = new BLL.Dealer().GetLastReportDateSet(dealerId);
                context.Response.Write("{\"msg\":\"" + string.Format("{0},{1}", obj.LastRepDateTime.ToString("yyyy-MM-dd"), obj.InvoiceDateTime.ToString("yyyy-MM-dd")) + "\",\"success\":true}");
            }
        }
        #endregion

        #region 设置权限
        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void SetLastDate(HttpContext context)
        {
            string code = context.Request.Form["code"];
            if (!string.IsNullOrEmpty(code))
            {
                Common.Entity.Dealer entity = new BLL.Dealer().GetDealerByCode(code);
                int dealerId = entity.PKID;
                string LastRepDateTime = context.Request.Form["LastRepDateTime"];
                string InvoiceDateTime = context.Request.Form["InvoiceDateTime"];
                int res = new BLL.Dealer().SetLastReportDate(dealerId, code, LastRepDateTime, InvoiceDateTime);
                if (res > 0)
                {
                    context.Response.Write("{\"msg\":\"设置时间成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"设置时间失败，请重试。\",\"success\":false}");
                }
            }
        }
        #endregion

        #region 获取用户所属经销店
        /// <summary>
        /// 获取用户所属经销店
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetUserDealerByPKID(HttpContext context)
        {
            string username = context.User.Identity.Name;
            SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            context.Response.Write(JsonConvert.SerializeObject(model));
        }
        #endregion

        #region 经销店联系人==============================

        #region 经销店联系人分页列表
        /// <summary>
        /// 经销店联系人分页列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetDealerContactManagerPager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            userName = context.User.Identity.Name;
            SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(userName);
            List<Common.Entity.DealerContact> list = new List<Common.Entity.DealerContact>();
            string search_Name = context.Request.Form["Name"];
            int search_JobID = Convert.ToInt32(context.Request.Form["JobID"]);
            Common.Entity.DealerContact filter = new Common.Entity.DealerContact
            {
                Name = search_Name,
                JobID = search_JobID
            };
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.DealerContact().GetDealerContactManagerPager(pageIndex, pageSize, model.DealerId, sort.SortName, sort.IsDesc, filter, out totalCount);
            }
            else
            {
                list = new BLL.DealerContact().GetDealerContactManagerPager(pageIndex, pageSize, model.DealerId, filter, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.DealerContact>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 职务下拉列表
        /// <summary>
        /// 职务下拉列表
        /// </summary>
        /// <param name="context"></param>
        public void GetJobList(HttpContext context)
        {
            DataTable dt = new BLL.DealerContact().GetJobList();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="context"></param>
        public void GetDealerContactByPKID(HttpContext context)
        {
            int pkid = Convert.ToInt32(context.Request["keyword"]);
            if (pkid > 0)
            {
                Common.Entity.DealerContact model = new BLL.DealerContact().GetDealerContactByPKID(pkid);
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
        public void SaveDealerContact(HttpContext context)
        {
            int HidId = Convert.ToInt32(context.Request["HidId"]); // 获取前端隐藏域ID用来判断是新增还是修改操作
            int jobId = Convert.ToInt32(context.Request.Form["fr_JobID"]);
            int DealerId = Convert.ToInt32(context.Request.Form["DealerId"]);
            string name = context.Request.Form["fr_Name"];
            string Phone = context.Request.Form["Phone"];
            string MobileTel = context.Request.Form["MobileTel"];
            string Fax = context.Request.Form["Fax"];
            string Email = context.Request.Form["Email"];
            string OtherContactInfo = context.Request.Form["OtherContactInfo"];
            string Remark = context.Request.Form["Remark"];
            BLL.DealerContact bll = new BLL.DealerContact();
            if (HidId <= 0)
            {
                // 添加
                Common.Entity.DealerContact model = new Common.Entity.DealerContact();
                bool IsExistsNameAndMobileTel = bll.Exists(name, MobileTel);
                model.DealerID = DealerId;
                model.Name = name;
                model.JobID = jobId;
                model.Phone = Phone;
                model.MobileTel = MobileTel;
                model.Fax = Fax;
                model.Email = Email;
                model.OtherContactInfo = OtherContactInfo;
                model.Remark = Remark;
                int result = bll.Add(model);
                if (IsExistsNameAndMobileTel)
                {
                    context.Response.Write("{\"msg\":\"经销店联系人已存在(经销店，姓名，移动电话)，请修改。\",\"state\":\"T\"}");
                }
                else if (result > 0)
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
                Common.Entity.DealerContact entity = new Common.Entity.DealerContact();
                entity.PKID = HidId;
                entity.Name = name;
                entity.JobID = jobId;
                entity.Phone = Phone;
                entity.MobileTel = MobileTel;
                entity.Fax = Fax;
                entity.Email = Email;
                entity.OtherContactInfo = OtherContactInfo;
                entity.Remark = Remark;
                bool result = bll.Update(entity);
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
        #endregion

        #region 删除 
        /// <summary>
        /// 删除 
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DeleteDealerContact(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["PKID"]);
            if (PKID > 0)
            {
                BLL.DealerContact bll = new BLL.DealerContact();
                bool result = bll.DELETEContactDATE(PKID);
                if (result)
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

        #endregion

        #region 经销店管理================================
        #region 获取经销商数据分页
        /// <summary>
        /// 获取经销商数据分页
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetDealerManagePager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.Dealer> list = new List<Common.Entity.Dealer>();
            string code = context.Request.Form["Code"];
            string name = context.Request.Form["Name"];
            int countryId = Convert.ToInt32(context.Request["cboBigArea"]);
            int proviceId = Convert.ToInt32(context.Request["cboProvince"]);
            int cityId = Convert.ToInt32(context.Request["cboCity"]);
            Common.Entity.Dealer filter = new Common.Entity.Dealer
            {
                Code = code,
                Name = name,
                CountryID = countryId,
                ProvinceID = proviceId,
                CityId = cityId
            };
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.Dealer().GetDealerManagePager(pageIndex, pageSize, sort.SortName, sort.IsDesc, filter, out totalCount);
            }
            else
            {
                list = new BLL.Dealer().GetDealerManagePager(pageIndex, pageSize, filter, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.Dealer>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 获取经销店实体对象
        public void GetDealerByPKID(HttpContext context)
        {
            int dealerId = Convert.ToInt32(context.Request["dealerId"]);
            if (dealerId > 0)
            {
                Common.Entity.Dealer model = new BLL.Dealer().GetDealerByPKID(dealerId);
                if (model != null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
        }
        #endregion

        #region 保存经销店信息
        /// <summary>
        /// 保存经销店信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void SaveDealer(HttpContext context)
        {
            int HidId = Convert.ToInt32(context.Request.Form["HidId"]);
            BLL.Dealer bll = new BLL.Dealer();
            string code = context.Request.Form["fr_Code"];
            string name = context.Request.Form["fr_Name"];
            string oldName = context.Request.Form["OldName"];
            int BearUserId = Convert.ToInt32(context.Request.Form["BearUserId"]);
            int RegionManagerUserId = Convert.ToInt32(context.Request.Form["RegionManagerUserId"]);
            int cityId = Convert.ToInt32(context.Request.Form["cboCity"]);
            string Address = context.Request.Form["Address"];
            string ZipCode = context.Request.Form["ZipCode"];
            string Phone = context.Request.Form["Phone"];
            string fax = context.Request.Form["fax"];
            string Email = context.Request.Form["Email"];
            string SalesTel = context.Request.Form["SalesTel"];
            string ServerTel = context.Request.Form["ServerTel"];
            string SystemEmail = context.Request.Form["SystemEmail"];
            string SalesDepartment = context.Request.Form["SalesDepartment"];
            string SalesDepartmentTel = context.Request.Form["SalesDepartmentTel"];
            string MaxCommissioner = context.Request.Form["MaxCommissioner"];
            string MaxCommissionerTel = context.Request.Form["MaxCommissionerTel"];
            Common.Entity.Dealer model = new Common.Entity.Dealer();
            if (HidId > 0)
            {
                // 修改操作
                model.PKID = HidId;
                model.Code = code;
                model.Name = name;
                model.OldName = oldName;
                model.BearUserId = BearUserId;
                model.RegionManagerUserId = RegionManagerUserId;
                model.CityId = cityId;
                model.Address = Address;
                model.ZipCode = ZipCode;
                model.Phone = Phone;
                model.fax = fax;
                model.Email = Email;
                model.SalesTel = SalesTel;
                model.ServerTel = ServerTel;
                model.SystemEmail = SystemEmail;
                model.SalesDepartment = SalesDepartment;
                model.SalesDepartmentTel = SalesDepartmentTel;
                model.MaxCommissioner = MaxCommissioner;
                model.MaxCommissionerTel = MaxCommissionerTel;
                bool result = bll.Update(model);
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
                // 新增操作
                bool isExistDealerCodeAndName = bll.IsExistsDealerCodeAndName(code, name);
                model.Code = code;
                model.Name = name;
                model.OldName = oldName;
                model.BearUserId = BearUserId;
                model.RegionManagerUserId = RegionManagerUserId;
                model.CityId = cityId;
                model.Address = Address;
                model.ZipCode = ZipCode;
                model.Phone = Phone;
                model.fax = fax;
                model.Email = Email;
                model.SalesTel = SalesTel;
                model.ServerTel = ServerTel;
                model.SystemEmail = SystemEmail;
                model.SalesDepartment = SalesDepartment;
                model.SalesDepartmentTel = SalesDepartmentTel;
                model.MaxCommissioner = MaxCommissioner;
                model.MaxCommissionerTel = MaxCommissionerTel;

                if (isExistDealerCodeAndName)
                {
                    context.Response.Write("{\"msg\":\"经销店编号或经销店名称已存在，请修改。\",\"state\":\"T\"}");
                }
                else if (bll.Add(model) > 0)
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

        #region 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="context"></param>
        public void DeleteData(HttpContext context)
        {
            string dealerIds = context.Request["dealerIds"];
            bool result = new BLL.Dealer().DeleteData(dealerIds);
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
        #endregion

        #region 经销店库存上报============================
        #region 从服务器端获取经销店库存上报分页列表
        /// <summary>
        /// 从服务器端获取经销店库存上报分页列表
        /// </summary>
        /// <param name="context"></param>
        public void GetDealerStockPagerList(HttpContext context)
        {
            string username = context.User.Identity.Name;
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            int DealerId = model.DealerId;
            int totalCount = 0; // 总记录数
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页
            int pageSize = Convert.ToInt32(context.Request["rows"]); // 页码大小
            Common.Entity.DealerStock search = new Common.Entity.DealerStock();
            if (!string.IsNullOrEmpty(context.Request.Form["filterContext"]))
            {
                search = JsonConvert.DeserializeObject<Common.Entity.DealerStock>(context.Request.Form["filterContext"]);
            }
            var list = new BLL.DealerStock().GetDealerStockPagerList(pageIndex, pageSize, DealerId, search, out totalCount);
            int totalPages = CommonFunction.CalculateTotalPage(pageSize, totalCount);
            var ResultData = new CommonFunction.DataResult<Common.Entity.DealerStock>()
            {
                totalCount = totalCount,
                totalpages = totalPages,
                currPage = pageIndex,
                dataList = list,
            };
            context.Response.Write(JsonConvert.SerializeObject(ResultData));
        }
        #endregion
        #region 保存数据
        public void SaveData(HttpContext context)
        {
            string name = context.Request["CustomerName"];
            string cboCarname = context.Request["cboCarname"];
            DateTime StockDate = Convert.ToDateTime(context.Request["StockDate"]);
            int StockNumber = Convert.ToInt32(context.Request["StockNumber"]);
            int hidCustomerID = Convert.ToInt32(context.Request["hidCustomerID"]);
            string username = context.User.Identity.Name;
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            int DealerId = model.DealerId;
            string code = context.Request["hidCode"];

            Common.Entity.DealerStock stock = new Common.Entity.DealerStock();
            stock.DealerID = DealerId;
            stock.CustomerID = hidCustomerID;
            stock.CarName = cboCarname;
            stock.StockMonth = StockDate;
            stock.StockCount = StockNumber;
            string opType = context.Request["opType"];
            if (opType == "add")
            {
                if (new BLL.DealerStock().Exists(stock))
                {
                    context.Response.Write("{\"msg\":\"已经存在该客户的月份库存，请修改。\",\"state\":1}");
                }
                else if (new BLL.DealerStock().Add(stock))
                {
                    context.Response.Write("{\"msg\":\"添加成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"添加失败。\",\"success\":false}");
                }
            }
            else
            {
                int HidID = Convert.ToInt32(context.Request["HidID"]);
                Common.Entity.DealerStock entity = new Common.Entity.DealerStock();
                entity.CustomerID = hidCustomerID;
                entity.CarName = cboCarname;
                entity.StockMonth = StockDate;
                entity.StockCount = StockNumber;
                entity.PKID = HidID;
                int r = new BLL.DealerStock().Update(entity);
                if (r > 0)
                {
                    context.Response.Write("{\"msg\":\"修改成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"修改失败。\",\"success\":false}");
                }
            }
        }
        #endregion
        #region 删除
        public void DeleteDealerStockByPKID(HttpContext context)
        {
            int StockId = Convert.ToInt32(context.Request["StockId"]);
            if (StockId > 0)
            {
                int result = new BLL.DealerStock().Delete(StockId);
                if (result > 0)
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