using BLL;
using Common.Utilities;
using DAL;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace VIPCustomer.IServer.OrderManage
{
    /// <summary>
    /// Order 的摘要说明
    /// </summary>
    public class OrderManage : IHttpHandler, IRequiresSessionState
    {
        private string userName = string.Empty;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            userName = context.User.Identity.Name;
            string action = context.Request["action"];
            switch (action)
            {
                case "GetOrderManagerPager":
                    GetOrderManagerPager(context);
                    break;

                case "GetOrderNoandDate":
                    GetOrderNoandDate(context);
                    break;

                case "GetBuyWay":
                    GetBuyWay(context);
                    break;

                case "GetPurchaseType":
                    GetPurchaseType(context);
                    break;

                case "GetOrderState":
                    GetOrderState(context);
                    break;

                case "GetRecordName":
                    GetRecordName(context);
                    break;

                case "showCarDemand":
                    showCarDemand(context);
                    break;

                case "getCC":
                    getCC(context);
                    break;

                case "getDC":
                    getDC(context);
                    break;

                case "GetOrderByPKID":
                    GetOrderByPKID(context);
                    break;

                case "GetWatchmaker":
                    GetWatchmaker(context);
                    break;

                case "DeleteOrders":
                    DeleteOrders(context);
                    break;

                case "SubmitOrders":
                    SubmitOrders(context);
                    break;

                case "GetCarColorByCarId":
                    GetCarColorByCarId(context);
                    break;

                case "GetCarDemandByPKID":
                    GetCarDemandByPKID(context);
                    break;

                case "SaveData":
                    SaveData(context);
                    break;

                case "GetOrderProcessByOrderID":
                    GetOrderProcessByOrderID(context);
                    break;

                case "GetOrdersAllPager":
                    GetOrdersAllPager(context);
                    break;

                case "ChangeType":
                    ChangeType(context);
                    break;

                case "SetOrdersStatus":
                    SetOrdersStatus(context);
                    break;

                case "Delete":
                    Delete(context);
                    break;

                case "SplitOrders":
                    SplitOrders(context);
                    break;

                case "DistributeNeed":
                    DistributeNeed(context);
                    break;

                case "FinishOrders":
                    FinishOrders(context);
                    break;

                case "GetOrderSaleManagerPager":
                    GetOrderSaleManagerPager(context);
                    break;

                case "SubmitRebates":
                    SubmitRebates(context);
                    break;

                case "GetPayCarDetail":
                    GetPayCarDetail(context);
                    break;

                case "GetData":
                    GetData(context);
                    break;

                #region 订单初审（即大客户室审核管理）====================
                case "GetOrdersByToExamineState":
                    GetOrdersByToExamineState(context);
                    break;

                case "BatchUpdateAudit":
                    BatchUpdateAudit(context);
                    break;
                case "CancelOrders":
                    CancelOrders(context);
                    break;
                case "getExcelData":
                    getExcelData(context);
                    break;
                    #endregion
            }
        }


        #region 订单管理===========================================
        #region 根据经销店编号获取订单管理数据分页列表
        /// <summary>
        /// 根据经销店编号获取订单管理数据分页列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetOrderManagerPager(HttpContext context)
        {
            int dealerID = GetLoginByDealerId();
            int totalCount = 0; // 总记录数
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页
            int pageSize = Convert.ToInt32(context.Request["rows"]); // 页码大小
            Common.Entity.Order search = new Common.Entity.Order();
            if (!string.IsNullOrEmpty(context.Request.Form["OrdersearchContext"]))
            {
                search = JsonConvert.DeserializeObject<Common.Entity.Order>(context.Request.Form["OrdersearchContext"]);
            }
            var list = new BLL.Order().GetDealerManagerPager(1, dealerID, search, pageIndex, pageSize, out totalCount);
            int totalPages = CommonFunction.CalculateTotalPage(pageSize, totalCount);
            var ResultData = new CommonFunction.DataResult<Common.Entity.Order>()
            {
                totalCount = totalCount,
                totalpages = totalPages,
                currPage = pageIndex,
                dataList = list,
            };
            context.Response.Write(JsonConvert.SerializeObject(ResultData));
        }
        #endregion

        #region 获取一个新订单编号和日期的JSON数据
        /// <summary>
        /// 获取一个新订单编号和日期的JSON数据
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetOrderNoandDate(HttpContext context)
        {
            BLL.Order bll = new BLL.Order();
            StringBuilder sb = new StringBuilder();
            string username = context.User.Identity.Name;
            int DealerId = 0;
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            DealerId = model.DealerId;
            sb.Append("{\"OrderNo\":");
            sb.Append(string.Format("\"{0}\"", bll.GetNewOrdersNo(DealerId)));
            sb.Append(",\"DateNow\":");
            sb.Append(string.Format("\"{0}\"}}", CommonFunction.GetDateString()));
            context.Response.Write(sb.ToString());
        }
        #endregion

        #region 获取订单制表人
        public void GetWatchmaker(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            string username = context.User.Identity.Name;
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            string strWatchmacker = model.Name;
            int dealerId = model.DealerId;
            sb.Append("{\"strWatchmacker\":");
            sb.Append(string.Format("\"{0}\"", strWatchmacker));
            sb.Append(",\"dealerId\":");
            sb.Append(string.Format("\"{0}\"}}", dealerId));
            context.Response.Write(sb.ToString());
        }
        #endregion

        #region 获取购买方式
        /// <summary>
        /// 获取购买方式
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetBuyWay(HttpContext context)
        {
            BLL.DICDomain bll = new BLL.DICDomain();
            DataTable dt = bll.GetBuyWay();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 获取购买方式
        /// <summary>
        /// 获取购买方式
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetPurchaseType(HttpContext context)
        {
            BLL.DICDomain bll = new BLL.DICDomain();
            DataTable dt = bll.GetPurchaseType();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 获取记录状态
        /// <summary>
        /// 获取购买方式
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetOrderState(HttpContext context)
        {
            BLL.DICDomain bll = new BLL.DICDomain();
            DataTable dt = bll.GetOrderState();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 获取记录名称
        /// <summary>
        /// 获取购买方式
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetRecordName(HttpContext context)
        {
            BLL.DICDomain bll = new BLL.DICDomain();
            DataTable dt = bll.GetRecordName();
            context.Response.Write(JsonConvert.SerializeObject(dt));
        }
        #endregion

        #region 获取订单实体对象
        /// <summary>
        /// 获取订单实体对象
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetOrderByPKID(HttpContext context)
        {
            int orderId = Convert.ToInt32(context.Request["orderId"]);
            if (orderId > 0)
            {
                Common.Entity.Order model = new Common.Entity.Order();
                model = new BLL.Order().GetOrderByPKID(orderId);
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        #region 获取车辆需求列表
        public void showCarDemand(HttpContext context)
        {
            string orderId = context.Request["orderId"];
            List<Common.Entity.CarPurchase> resultBuy = new List<Common.Entity.CarPurchase>();
            if (string.IsNullOrEmpty(orderId))
            {
                var model = new CommonFunction.DataResult<Common.Entity.CarPurchase>()
                {
                    dataList = resultBuy,
                };
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
            else
            {
                resultBuy = new BLL.CarPurchase().GetList(orderId);
                var model = new CommonFunction.DataResult<Common.Entity.CarPurchase>()
                {
                    dataList = resultBuy,
                };
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        #region 获取经销店联系人列表
        public void getDC(HttpContext context)
        {
            List<Common.Entity.DealerContact> dealerContactResult = new List<Common.Entity.DealerContact>();
            List<Common.Entity.OrderDealerContact> orderContactResult = new List<Common.Entity.OrderDealerContact>();
            BLL.DealerContact dealerContact = new BLL.DealerContact();
            string DealerId = context.Request["DealerId"];
            string orderId = context.Request["orderId"];
            string IsEdit = context.Request["IsEdit"];
            if (!string.IsNullOrEmpty(DealerId))
            {
                if ("1" == IsEdit)
                {
                    dealerContactResult = dealerContact.GetListByDealerID(DealerId);
                    var model = new CommonFunction.DataResult<Common.Entity.DealerContact>
                    {
                        dataList = dealerContactResult
                    };
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
            else
            {
                orderContactResult = dealerContact.GetListByOrdersID(orderId);
                var model = new CommonFunction.DataResult<Common.Entity.OrderDealerContact>
                {
                    dataList = orderContactResult
                };
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        #region 获取客户联系人列表
        public void getCC(HttpContext context)
        {
            List<Common.Entity.CustomerContact> customerContactResult = new List<Common.Entity.CustomerContact>();
            List<Common.Entity.OrderCustomerContact> orderCustomerContactResult = new List<Common.Entity.OrderCustomerContact>();
            BLL.Customer bll = new BLL.Customer();
            string customerId = context.Request["customerId"];
            string orderId = context.Request["orderId"];
            string IsEdit = context.Request["IsEdit"];
            if (string.IsNullOrEmpty(customerId))
            {
                var model = new CommonFunction.DataResult<Common.Entity.CustomerContact>
                {
                    dataList = customerContactResult
                };
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
            else
            {
                if ("1" == IsEdit)
                {
                    customerContactResult = bll.GetListByCustomerID(customerId);
                    var model = new CommonFunction.DataResult<Common.Entity.CustomerContact>
                    {
                        dataList = customerContactResult
                    };
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
                else
                {
                    orderCustomerContactResult = bll.GetListByOrdersID(orderId);
                    var model = new CommonFunction.DataResult<Common.Entity.OrderCustomerContact>
                    {
                        dataList = orderCustomerContactResult
                    };
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
        }
        #endregion

        #region 作废订单操作
        /// <summary>
        /// 作废订单操作
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DeleteOrders(HttpContext context)
        {
            BLL.Order bll = new BLL.Order();
            int orderId = ZJRequest.GetFormInt("orderId");
            string username = context.User.Identity.Name;
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            bll.Delete(model.Name, orderId);
        }
        #endregion

        #region 查询车辆颜色下拉列表
        /// <summary>
        /// 查询车辆颜色下拉列表
        /// </summary>
        /// <param name="context"></param>
        public void GetCarColorByCarId(HttpContext context)
        {
            int carId = Convert.ToInt32(context.Request["carId"]);
            if (carId > 0)
            {
                var resultJson = "";
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format($@"select PKID,Code,Name from CarColor where CarID={carId}");
                    var list = conn.Query<Common.Entity.CarColor>(sql).ToList();
                    resultJson = JsonConvert.SerializeObject(list);
                };
                context.Response.Write(resultJson);
            }
        }
        #endregion

        #region 提交订单
        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="context"></param>
        public void SubmitOrders(HttpContext context)
        {
            string submitIDs = context.Request["submitData"];
            string customerIDs = context.Request["customerData"];
            if (string.IsNullOrWhiteSpace(submitIDs))
            {
                context.Response.Write("{\"msg\":\"没有可以提交的订单。\",\"success\":false}");
                return;
            }
            BLL.Order order = new BLL.Order();
            string username = context.User.Identity.Name;
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            if (new BLL.DealerContact().GetDealerContactCount(model.DealerId) < 1)
            {
                context.Response.Write("{\"msg\":\"至少添加一个经销店联系人（需要返款的话，请填写返款联系人）。\",\"success\":\"false\"}");
                return;
            }

            bool directSubmit = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["directSubmitOrders"]);

            string[] subIDs = submitIDs.Split(',');
            string[] subCustomerIDs = customerIDs.Split(',');

            // 判断是否提交的订单数据中已有购车等信息
            string result = "";
            string tempResult = "";
            for (int i = 0; i < subIDs.Length; i++)
            {
                if (!CommonFunction.CheckOrderAttach(int.Parse(subIDs[i]), "attachRequireOrder"))
                {
                    context.Response.Write("{\"msg\":\"电子资料不全。具体要求查看在线帮助。\",\"success\":\"false\"}");
                    return;
                }
                else
                {
                    tempResult = order.SubmitOrders(model.Name, subIDs[i], subCustomerIDs[i], model.DealerId, directSubmit);
                    if (!string.IsNullOrWhiteSpace(tempResult))
                        result = tempResult;
                    context.Response.Write("{\"msg\":\"" + result + "\",\"success\":true}");
                    return;
                }
            }
        }
        #endregion

        #region 获取车辆需求实体对象
        /// <summary>
        /// 获取车辆需求实体对象
        /// </summary>
        /// <param name="context"></param>
        public void GetCarDemandByPKID(HttpContext context)
        {
            int carId = Convert.ToInt32(context.Request["carId"]);
            if (carId > 0)
            {
                var list = new BLL.CarPurchase().GetCarDemandByPKID(carId);
                context.Response.Write(JsonConvert.SerializeObject(list));
            }
        }
        #endregion

        #region 保存数据
        public void SaveData(HttpContext context)
        {
            bool isConfirmData = bool.Parse(ZJRequest.GetFormString("isConfirmData"));
            string opType = ZJRequest.GetFormString("info[opType]");
            string BuyWay = ZJRequest.GetFormString("info[BuyWay]");
            string orderNo = ZJRequest.GetFormString("info[OrderNo]");
            DateTime CreateTime = Convert.ToDateTime(context.Request.Form["info[CreateTime]"]);
            string PurchaseType = ZJRequest.GetFormString("info[PurchaseType]");
            string OrderState = ZJRequest.GetFormString("info[OrderState]");
            string CarUse = ZJRequest.GetFormString("info[CarUse]");
            int OffAddressOnCardReport = (ZJRequest.GetFormInt("info[OffAddressOnCardReport]"));
            int InvoiceDiffer = ZJRequest.GetFormInt("info[InvoiceDiffer]");
            int IsApplyMaxCustomerResources = ZJRequest.GetFormInt("info[IsApplyMaxCustomerResources]");
            int hiddenDealerId = ZJRequest.GetFormInt("info[hiddenDealerId]");
            int hiddenCustomerId = ZJRequest.GetFormInt("info[hiddenCustomerId]");
            string RecordName = ZJRequest.GetFormString("info[RecordName]");
            string Watchmaker = ZJRequest.GetFormString("info[Watchmaker]");
            int DifferentPlace = ZJRequest.GetFormInt("info[DifferentPlace]");
            string InvoiceCustomerInfo = ZJRequest.GetFormString("info[FN_InvoiceName]");
            Common.Entity.Order order = new Common.Entity.Order();
            order.BuyWay = BuyWay;
            order.Code = orderNo;
            order.CreateTime = CreateTime;
            order.PurchaseType = PurchaseType;
            order.OrderState = OrderState;
            order.CarUse = CarUse;
            order.OffAddressOnCardReport = OffAddressOnCardReport;
            order.InvoiceAndCustomerAtypism = InvoiceDiffer;
            order.IsApplyMaxCustomerResources = IsApplyMaxCustomerResources;
            order.CustomerID = hiddenCustomerId;
            order.DealerID = hiddenDealerId;
            order.RecordName = RecordName;
            order.Watchmaker = Watchmaker;
            order.DifferentPlace = DifferentPlace;
            order.OrderType = 1; // 订单类别：订单为1，销售单为2
            order.InvoiceCustomerInfo = InvoiceCustomerInfo;
            string result = "";
            List<Common.Entity.Order> list = new List<Common.Entity.Order>();
            string username = context.User.Identity.Name;
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(username);
            if (opType == "update")
            {
                order.PKID = Convert.ToInt32(context.Request["info[HidOrderID]"]);

                if (new BLL.Order().GetOrderByPKID(order.PKID).ToExamineState >= 300)
                {
                    result = "已经进行配车的订单无法进行更改。";
                }
            }

            Hashtable CarDemandSort2Guid = new Hashtable();
            string arrCar = context.Request.Form["arrCar"];
            List<Common.Entity.CarPurchase> carsDemand = JsonConvert.DeserializeObject<List<Common.Entity.CarPurchase>>(arrCar);
            bool hasInvalidData = false;
            IDictionary<string, IList<Common.Entity.CarPurchase>> demandCars = new Dictionary<string, IList<Common.Entity.CarPurchase>>();
            demandCars.Add("exist", new List<Common.Entity.CarPurchase>());
            demandCars.Add("new", new List<Common.Entity.CarPurchase>());
            foreach (Common.Entity.CarPurchase cd in carsDemand)
            {
                cd.IsApplyMaxCustomerResources = "1";
                cd.OrderID = order.PKID;
                if ((0 == cd.PKID))
                {
                    demandCars["new"].Add(cd);
                    CarDemandSort2Guid.Add(cd.AddSortNo, cd.PKID);
                }
                else
                {
                    demandCars["exist"].Add(cd);
                }
                if ("COASTER" == cd.CarName.Trim().ToUpper())
                {
                    if (string.IsNullOrEmpty(cd.OldNo) || string.IsNullOrEmpty(cd.RuckSack))
                    {
                        hasInvalidData = true;
                    }
                }
            }
            IDictionary<string, IList<Common.Entity.CarRecord>> payCars = new Dictionary<string, IList<Common.Entity.CarRecord>>();
            List<Common.Entity.CarRecord> parList = new List<Common.Entity.CarRecord>();
            payCars.Add("exist", new List<Common.Entity.CarRecord>());
            payCars.Add("new", new List<Common.Entity.CarRecord>());
            if (hasInvalidData && ("协商" != order.OrderState))
            {
                order.OrderState = "协商";
                result = "订单已经被强制设置为协商状态，有不符合规范的订购信息（请检查COASTER车型信息是否填写完全）。";
            }
            if (opType == "update")
            {
                new BLL.Order().Update(order, demandCars, payCars);
                //new BLL.Order().Update(order, carsDemand, parList);
            }
            else
            {
                new BLL.Order().Add(model.Name, order, demandCars, payCars);

            }
            if ((isConfirmData) && string.IsNullOrEmpty(result))
            {
                int? demandCount = demandCars["new"].Sum(p => p.RequirementNumber) + demandCars["exist"].Sum(p => p.RequirementNumber);
                if (!CommonFunction.CheckOrderAttach(order.PKID, "attachRequireOrder"))
                {
                    result = "电子资料不全。具体要求查看在线帮助。";
                }
                else if ((demandCount > 0) && new BLL.Order().CheckSubmitOrders(order.CustomerID, order.DealerID))
                {
                    bool directSubmit = bool.Parse(ConfigurationManager.AppSettings["directSubmitOrders"]);
                    new BLL.Order().SubmitOrders(model.Name, order.PKID.ToString("D"), order.CustomerID.ToString("D"), order.DealerID, directSubmit);
                }
                else
                {
                    result = "订单已保存，但订单不符合提交需求，请修改后提交。请检查<BR>1.车辆需求是否填写<BR>2.经销店以及客户联系人是否填写<BR>";
                }
            }
            context.Response.Write("{\"msg\":\" " + result + "\",\"state\":\"true\"}");
        }
        #endregion

        #region 获取订单进度流程
        public void GetOrderProcessByOrderID(HttpContext context)
        {
            int orderId = Convert.ToInt32(context.Request["orderId"]);
            if (orderId > 0)
            {
                List<Common.Entity.OrderProcess> list = new BLL.OrderProcess().GetOrderProcessByOrderID(orderId);
                var json = JsonConvert.SerializeObject(list);
                context.Response.Write(json);
            }
        }
        #endregion
        #endregion

        #region 销售单模块==========================================

        #region 获取订单销售单列表
        public void GetOrderSaleManagerPager(HttpContext context)
        {
            int totalCount = 0; // 总记录数
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页
            int pageSize = Convert.ToInt32(context.Request["rows"]); // 页码大小
            Common.Entity.Order filter = new Common.Entity.Order();
            if (!string.IsNullOrEmpty(context.Request.Form["OrdersearchContext"]))
            {
                filter = JsonConvert.DeserializeObject<Common.Entity.Order>(context.Request.Form["OrdersearchContext"]);
            }
            int dealerID = GetLoginByDealerId();
            var listData = new BLL.Order().GetOrderSaleManagerPager(2, dealerID, filter, pageIndex, pageSize, out totalCount);
            int totalPages = CommonFunction.CalculateTotalPage(pageSize, totalCount);
            var ResultData = new CommonFunction.DataResult<Common.Entity.Order>()
            {
                totalCount = totalCount,
                totalpages = totalPages,
                currPage = pageIndex,
                dataList = listData,
            };
            context.Response.Write(JsonConvert.SerializeObject(ResultData));
        }
        #endregion

        #region 根据用户登录身份获取经销店ID
        /// <summary>
        /// 根据用户登录身份获取经销店ID
        /// </summary>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        public int GetLoginByDealerId()
        {
            Common.SysUserEntity model = new BLL.sysUser().GetSysUserByLoginName(userName);
            int dealerId = model.DealerId;
            return dealerId;
        }
        #endregion

        #region 申请返款
        /// <summary>
        /// 申请返款
        /// </summary>
        /// <param name="context"></param>
        public void SubmitRebates(HttpContext context)
        {
            string orderId = context.Request["orderId"];
            if (string.IsNullOrEmpty(orderId))
            {
                context.Response.Write("{\"msg\":\"没有可以提交的销售单。\",\"success\":\"false\"}");
                return;
            }
            var order = new BLL.Order();
            string result = "";
            foreach (string s in orderId.Split(new char[] { ',' }))
            {
                if (order.SubmitOrders(s, order.GetOrderByPKID(orderId).CustomerID.ToString()).Length > 0)
                {
                    result += "不可提交";
                }
                else if (!CommonFunction.CheckOrderAttach(int.Parse(orderId), "attachRequireSale"))
                {
                    result += "电子资料不全。具体要求查看在线帮助。";
                }
                else
                    result += order.SubmitRebates(orderId);
            }
            result = (result.Length > 0) ? "部分销售单不能申请返款。可能以下情况：<BR>1.被禁止申请返款;2.已提交过<BR>3.未填写交车记录;4.车型不符合返款规定;5.电子资料不全" : "";
            context.Response.Write("{\"msg\":\" " + result + "\",\"success\":\"false\"}");
        }
        #endregion

        #endregion

        #region 订单销售单管理业务逻辑=============================

        #region 获取全部订单信息
        /// <summary>
        /// 获取全部订单信息
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetOrdersAllPager(HttpContext context)
        {
            int totalCount = 0; // 总记录数
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页
            int pageSize = Convert.ToInt32(context.Request["rows"]); // 页码大小
            Common.Entity.Order filter = new Common.Entity.Order();
            if (!string.IsNullOrEmpty(context.Request.Form["OrdersearchContext"]))
            {
                filter = JsonConvert.DeserializeObject<Common.Entity.Order>(context.Request.Form["OrdersearchContext"]);
            }
            var list = new BLL.Order().GetOrdersAllPager(pageIndex, pageSize, filter, out totalCount);
            int totalPages = CommonFunction.CalculateTotalPage(pageSize, totalCount);
            var ResultData = new CommonFunction.DataResult<Common.Entity.Order>()
            {
                totalCount = totalCount,
                totalpages = totalPages,
                currPage = pageIndex,
                dataList = list,
            };
            context.Response.Write(JsonConvert.SerializeObject(ResultData));
        }
        #endregion

        #region 保存单据类别
        public void ChangeType(HttpContext context)
        {
            BLL.Order order = new BLL.Order();
            string ordersID = context.Request["rowIDs"];
            string ordersType = context.Request["cmboxOrderType"];
            string[] ordersList = getOrdersList(ordersID);
            int nType = int.Parse(ordersType);
            int result = 0;
            foreach (string ids in ordersList)
            {
                result = order.ChangeOrderType(ids, nType);
            }
            if (result > 0)
            {
                context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
            }
        }
        #endregion

        #region 设置订单状态
        public void SetOrdersStatus(HttpContext context)
        {
            BLL.Order order = new BLL.Order();
            string ordersID = context.Request["rowIDs"];
            string status = context.Request["status"];
            string[] ordersList = getOrdersList(ordersID);
            int statusInt = int.Parse(status);
            int result = 0;
            foreach (string ids in ordersList)
            {
                result = order.InitialiseStatus(ids, statusInt);
            }
            if (result > 0)
            {
                context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
            }
        }
        #endregion

        #region 删除订单
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="context"></param>
        public void Delete(HttpContext context)
        {
            string id = context.Request["orderId"];
            BLL.Order order = new BLL.Order();
            string[] ordersList = getOrdersList(id);
            int result = 0;
            foreach (string ids in ordersList)
            {
                result = order.Remove(ids);
            }
            if (result > 0)
            {
                context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
            }
        }
        #endregion

        #region 拆分订单
        public void SplitOrders(HttpContext context)
        {
            string id = context.Request["orderId"];
            BLL.Order order = new BLL.Order();
            string[] ordersList = getOrdersList(id);
            foreach (string ids in ordersList)
            {
                order.SplitOrders(ids);
                context.Response.Write("{\"msg\":\"拆单完成。\",\"success\":true}");
            }
        }
        #endregion

        private string[] getOrdersList(string ordersStr)
        {
            return ordersStr.Replace(" ", "").Replace("\"", "").Replace("[", "").Replace("]", "").Split(',');
        }

        /// <summary>
        /// 配车状态：恢复配车1，取消配车0
        /// </summary>
        /// <param name="context"></param>
        public void DistributeNeed(HttpContext context)
        {
            string orderId = context.Request["orderId"];
            if (string.IsNullOrEmpty(orderId))
                return;
            int needType = Convert.ToInt32(context.Request["needType"]);
            BLL.Order bll = new BLL.Order();
            bll.ChangeOrdersNeeds(orderId.Replace(" ", "").Replace("\"", "").Replace("[", "").Replace("]", ""), needType);
        }

        public void FinishOrders(HttpContext context)
        {
            string orderId = context.Request["orderId"];
            if (string.IsNullOrEmpty(orderId))
                return;
            BLL.Order bll = new BLL.Order();
            bll.FinishOrders(orderId.Replace(" ", "").Replace("\"", "").Replace("[", "").Replace("]", ""));
        }

        #region 获取审核信息
        /// <summary>
        /// 获取审核信息
        /// </summary>
        /// <param name="context"></param>
        public void GetPayCarDetail(HttpContext context)
        {
            string orderId = context.Request["orderId"];
            List<Common.Entity.CarRecord> resultBuy = new List<Common.Entity.CarRecord>();
            if (string.IsNullOrEmpty(orderId))
            {
                var model = new CommonFunction.DataResult<Common.Entity.CarRecord>()
                {
                    dataList = resultBuy,
                };
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
            else
            {
                resultBuy = new BLL.CarRecord().GetList(orderId);
                var model = new CommonFunction.DataResult<Common.Entity.CarRecord>()
                {
                    dataList = resultBuy,
                };
                context.Response.Write(JsonConvert.SerializeObject(model));
            }
        }
        #endregion

        public void GetData(HttpContext context)
        {
            int orderId = int.Parse(context.Request["orderId"]);
            Common.Entity.ReportOrder order = new BLL.Order().GetOrderFullInfo(orderId);
            context.Response.Write(JsonConvert.SerializeObject(order));
        }

        #endregion

        #region ======大客户室审核==========================
        #region 获取大客户室审核分页列表
        /// <summary>
        /// 获取大客户室审核分页列表
        /// </summary>
        /// <param name="context"></param>
        public void GetOrdersByToExamineState(HttpContext context)
        {
            int totalCount = 0; // 总记录数
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页
            int pageSize = Convert.ToInt32(context.Request["rows"]); // 页码大小
            Common.Entity.Order filter = new Common.Entity.Order();
            if (!string.IsNullOrEmpty(context.Request.Form["OrdersearchContext"]))
            {
                filter = JsonConvert.DeserializeObject<Common.Entity.Order>(context.Request.Form["OrdersearchContext"]);
            }
            int dealerID = GetLoginByDealerId();
            var listData = new BLL.Order().GetOrdersByToExamineState(1, dealerID, filter, pageIndex, pageSize, out totalCount);
            int totalPages = CommonFunction.CalculateTotalPage(pageSize, totalCount);
            var ResultData = new CommonFunction.DataResult<Common.Entity.Order>()
            {
                totalCount = totalCount,
                totalpages = totalPages,
                currPage = pageIndex,
                dataList = listData,
            };
            context.Response.Write(JsonConvert.SerializeObject(ResultData));
        }
        #endregion

        #region 保存审核信息
        public void BatchUpdateAudit(HttpContext context)
        {
            string orderId = context.Request["orderId"];
            string Batch_cboAudit = context.Request["Batch_cboAudit"];
            string Batch_FN_RebatesRemark = context.Request["Batch_FN_RebatesRemark"];
            string result = "";
            if (string.IsNullOrEmpty(orderId))
            {
                result = "没有可以审核的订单。";
                context.Response.Write("{\"msg\":\"" + result + "。\",\"success\":false}");
                return;
            }
            BLL.Order bll = new BLL.Order();
            string[] orders = orderId.Split(new char[] { ',' });
            List<string> newOrders = new List<string>();
            var rightCar = string.Format("'{0}'", context.Session["UserCar"].ToString().Replace(",", "','"));
            foreach (string s in orders)
            {
                if (bll.ExistsCarOrder(s, rightCar.Trim()))
                {
                    newOrders.Add(s);
                }
            }
            int orderlen = orderId.Length;
            if (newOrders.Count < 1)
            {
                result = "没有有权限审核的订单";
                context.Response.Write("{\"msg\":\"" + result + "。\",\"success\":false}");
                return;
            }
            orderId = string.Join(",", newOrders.ToArray<string>());
            if (orderId.Length != orderlen)
            {
                result = "部分订单没有权限审核";
                context.Response.Write("{\"msg\":\"" + result + "。\",\"success\":false}");
                return;
            }
            bll.BatchUpdateAudit(orderId, context.Session["Name"].ToString(), Batch_cboAudit, Batch_FN_RebatesRemark);
            if (Batch_cboAudit.Equals("同意"))
            {
                foreach (string s in orderId.Split(new char[] { ',' }))
                {
                    try
                    {
                        // 更新审核状态
                        bll.SplitOrders(orderId);
                        result = "批审核成功。";
                    }
                    catch
                    {
                    }
                }
            }
            context.Response.Write("{\"msg\":\"" + result + "。\",\"success\":true}");
        }
        #endregion

        #region 作废订单
        public void CancelOrders(HttpContext context)
        {
            string orderIds = context.Request["orderIds"];
            string[] ordersList = getOrdersList(orderIds);
            int result = 0;
            foreach (string ids in ordersList)
            {
                result = new BLL.Order().InitialiseStatus(ids, -1000);
            }
            if (result > 0)
            {
                context.Response.Write("{\"msg\":\"操作完成。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"操作失败。\",\"success\":false}");
            }
        }
        #endregion

        #region 导出Excel
        public void getExcelData(HttpContext context)
        {
            string sendData = context.Request["sendData"];
            var list = JsonConvert.DeserializeObject<List<Common.Entity.Order>>(sendData);
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