using Common.Entity;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Web;

namespace BLL
{
    public class Order
    {
        private DAL.Order order = new DAL.Order();

        public string SubmitOrders(string submitIDs, string customerIDs)
        {
            if (string.IsNullOrEmpty(submitIDs))
            {
                return "没有可以提交的订单";
            }
            BLL.CarPurchase pay = new BLL.CarPurchase();
            if (new BLL.DealerContact().GetDealerContactCount(Convert.ToInt32(HttpContext.Current.Session["DealerID"])) < 0)
            {
                return "至少添加一个经销店联系人（需要返款的话，请填写返款联系人）。";
            }
            bool directSubmit = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["directSubmitOrders"]);
            string[] subIDs = submitIDs.Split(',');
            string[] subCustomerIDs = customerIDs.Split(',');
            // 判断是否提交的订单数据中已有购车等信息
            string result = "";
            for (int i = 0; i < subIDs.Length; i++)
            {
                string tempResult = "";
                tempResult = SubmitOrders(HttpContext.Current.Session["Name"].ToString(), subIDs[i], subCustomerIDs[i], Convert.ToInt32(HttpContext.Current.Session["DealerID"]), directSubmit);
                if (!string.IsNullOrEmpty(tempResult))
                {
                    result = tempResult;
                }
            }
            return result;
        }

        /// <summary>
        /// 提交返款申请
        /// </summary>
        /// <param name="ordersID"></param>
        /// <returns></returns>
        public string SubmitRebates(string ordersID)
        {
            if (!order.CanRebatesOrder(ordersID))
                return "该单不允许申请返款。";

            if (!order.CheckRebatesCars(ordersID))
                return "交车信息中不包含可返款的车型，不能进行返款申请（请确认是否已提交交车信息）。";

            int status = 400;

            if (order.SubmitRebates(ordersID, status))
                return "";
            else
                return "已申请过返款。";
        }
        #region 订单模块===========================================
        #region 从服务器端获取分页列表
        /// <summary>
        /// 从服务器端获取分页列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>返回DataTable</returns>
        public List<Common.Entity.Order> GetDealerManagerPager(int type, int dealerID, Common.Entity.Order search, int pageIndex, int pageSize, out int totalCount)
        {
            StringBuilder sqlCount = new StringBuilder();
            sqlCount.Append($@"select count(*) from (select A.* from [Order] A left join Customer B on A.CustomerID=B.PKID
                           left join Dealer C on A.DealerID=C.PKID 
                           left join SysUser D on C.PKID=D.DealerId 
                           where A.PKID>0 and A.DealerID={dealerID} and A.OrderType={type}");
            if (!string.IsNullOrEmpty(search.Code))
            {
                sqlCount.Append(" and A.Code like '%" + search.Code + "%' or B.Name like'%" + search.Code + "%'");
            }
            switch (search.ToExamineState)
            {
                case 0:
                    break;

                case 200:
                    sqlCount.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case -200:
                    sqlCount.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case 300:
                    sqlCount.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case 1000:
                    sqlCount.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case -1000:
                    sqlCount.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;
                case -500:
                    break;
            }
            sqlCount.Append(" ) A");
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"select top {pageSize} *  from (select  distinct row_number() over (order by A.PKID) as rowNumber,
                         D.Name as LoginName,B.Name,C.Name as DealerName,Convert(varchar(100),A.CreateTime,23) as ToCreateTime,
						 Convert(varchar(100),A.SubmitTime,23) as ToSubmitTime,B.Name as CustomerName,A.* from [Order] A left join Customer B on 
                         A.CustomerID=B.PKID left join Dealer C on A.DealerID=C.PKID left join SysUser D on C.PKID=D.DealerId 
                         where A.PKID>0 and A.DealerID={dealerID} and A.OrderType={type}");
            if (!string.IsNullOrEmpty(search.Code))
            {
                strSql.Append(" and A.Code like '%" + search.Code + "%' or B.Name like'%" + search.Code + "%'");
            }
            switch (search.ToExamineState)
            {
                case 0:
                    break;

                case 200:
                    strSql.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case -200:
                    strSql.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case 300:
                    strSql.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case 1000:
                    strSql.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case -1000:
                    strSql.Append(" and A.ToExamineState=" + search.ToExamineState);
                    break;

                case -500:
                    break;
            }
            strSql.Append($" ) as T where T.rowNumber > ({pageIndex} - 1) * {pageSize} ");
            List<Common.Entity.Order> list = new List<Common.Entity.Order>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                totalCount = conn.Query<int>(sqlCount.ToString()).FirstOrDefault();
                list = conn.Query<Common.Entity.Order>(strSql.ToString()).ToList();
            }
            return list;
        }
        #endregion

        #region 获取经销店新流水号
        /// <summary>
        /// 获取经销店新流水号
        /// </summary>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        public string GetNewOrdersNo(int dealerId)
        {
            BLL.Dealer dal = new Dealer();
            Common.Entity.Dealer dealer = dal.GetDealerByPKID(dealerId);
            return string.Format("{0}-{1}-{2}", dealer.Code, DateTime.Now.ToString("yyyyMMdd"), order.GetNewOrdersNo(dealerId).ToString());
        }
        #endregion

        #region 获取订单实体对象
        /// <summary>
        /// 获取订单实体对象
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Common.Entity.Order GetOrderByPKID(int orderId)
        {
            return new DAL.Order().GetOrderByPKID(orderId);
        }
        public Common.Entity.Order GetOrderByPKID(string orderId)
        {
            return new DAL.Order().GetOrderByPKID(orderId);
        }
        #endregion

        #region 作废一个订单
        /// <summary>
        /// 作废一个订单
        /// </summary>
        /// <param name="OpratorName"></param>
        /// <param name="orderId"></param>
        public void Delete(string OpratorName, int orderId)
        {
            IList<int> delS = order.Delete(orderId);

            //foreach (int g in delS)
            //{

            //}
            BLL.OrderProcess.Add(OpratorName, Common.Utilities.enum流程名称枚举.订单作废.ToString(), 0, orderId);
        }
        #endregion

        #region 获取订单中的订购车型
        /// <summary>
        /// 获取订单中的订购车型
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IList<string> GetCarsOrder(int orderId)
        {
            return new DAL.Order().GetCarsOrder(orderId);
        }
        #endregion

        #region 提交订单
        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="OpratorName"></param>
        /// <param name="modelId"></param>
        /// <param name="customID"></param>
        /// <param name="dealerId"></param>
        /// <param name="directSubmit"></param>
        /// <returns></returns>
        public string SubmitOrders(string OpratorName, string modelId, string customID, int dealerId, bool directSubmit)
        {
            string[] subIDs = modelId.Split(',');
            string[] subCustomIDs = customID.Split(',');
            if (subIDs.Length < 1)
                return "没有提交任何有效数据。";

            string result = "";
            List<int> subInt = new List<int>();
            List<int> subCustomInt = new List<int>();
            for (int i = 0; i < subIDs.Length; i++)
            {
                int ordersID = Convert.ToInt32(subIDs[i]);
                int customerID = Convert.ToInt32(subCustomIDs[i]);
                if (CheckSubmitOrders(ordersID, customerID, dealerId))
                {
                    subInt.Add(ordersID);
                    subCustomInt.Add(customerID);
                    BLL.OrderProcess.Add(OpratorName, Common.Utilities.enum流程名称枚举.订单提交.ToString(), dealerId, ordersID, directSubmit);
                }
                else
                {
                    result = "部分数据不符合规范，未能提交。请检查<BR>1.车辆需求是否填写<BR>2.经销店以及客户联系人是否填写。<BR>";
                }
            }

            int auditStat = directSubmit ? (int)Common.Utilities.订单状态.大客户审核 : (int)Common.Utilities.订单状态.大区审核;
            order.SubmitOrders(subInt.ToArray(), subCustomInt.ToArray(), dealerId, auditStat);
            return result;
        }
        #endregion

        #region 检测订单是否符合提交要求
        /// <summary>
        /// 检测订单是否符合提交要求
        /// </summary>
        /// <param name="ordersID"></param>
        /// <param name="customerID"></param>
        /// <param name="dealerID"></param>
        /// <returns></returns>
        public bool CheckSubmitOrders(int ordersID, int customerID, int dealerID)
        {
            bool result = true;
            if (ordersID > 0)
            {
                // 是否添加订购车辆
                result &= new BLL.DICDomain().GetDemandRecordsCount(ordersID) > 0;
                if (!result)
                    return false;
            }

            // 是否添加客户联系人
            result &= new BLL.Customer().GetCustomerContactCount(customerID) > 0;
            if (!result)
                return false;

            // 是否添加经销店联系人
            if (dealerID > 0)
                result &= new BLL.DealerContact().GetDealerContactCount(dealerID) > 0;
            return result;
        }
        #endregion

        #region 新增订单
        /// <summary>
        /// 新增一个订单信息
        /// </summary>
        /// <param name="OpratorName"></param>
        /// <param name="model"></param>
        /// <param name="carsDemand"></param>
        /// <param name="carsPay"></param>
        /// <returns></returns>
        public void Add(string OpratorName, Common.Entity.Order model, IDictionary<string, IList<Common.Entity.CarPurchase>> carsDemand, IDictionary<string, IList<Common.Entity.CarRecord>> carsPay)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    conn.Execute($@"insert into [Order] (Code,BuyWay,CreateTime,SubmitTime,DealerID,OrderState, SubmitType,PurchaseType,CustomerID,RecordName,
                                 BearUser, Watchmaker, CarUse, DifferentPlace, CustomerSuggestion, BaseRemark, Replyer1, ReComment1, ReDate1, ReRemark1,
                                 Replyer2, ReComment2, ReDate2, ReRemark2, Replyer3, ReComment3, ReDate3, ReRemark3, FTMSBackAuditor, FTMSBackSuggestion, FTMSBackToExamineDate,
                                 FTMSBackRemark, IsApplyMaxCustomerResources, ToExamineState, IsEdit, OrderType, YuanDanID, OffAddressOnCardReport, InvoiceAndCustomerAtypism, InvoiceCustomerInfo) 
                                 values(@Code,@BuyWay,@CreateTime,@SubmitTime,@DealerID,@OrderState,@SubmitType,@PurchaseType,@CustomerID,@RecordName,
                                 @BearUser, @Watchmaker,@CarUse, @DifferentPlace, @CustomerSuggestion, @BaseRemark, @Replyer1, @ReComment1, @ReDate1, @ReRemark1,
                                 @Replyer2, @ReComment2, @ReDate2,@ReRemark2, @Replyer3, @ReComment3, @ReDate3, @ReRemark3, @FTMSBackAuditor, @FTMSBackSuggestion, @FTMSBackToExamineDate,
                                 @FTMSBackRemark, @IsApplyMaxCustomerResources,{0},{1}, @OrderType, @YuanDanID, @OffAddressOnCardReport, @InvoiceAndCustomerAtypism, @InvoiceCustomerInfo)", model, tran);
                    int orderId = conn.Query<int>("select ident_current('[Order]');", null, tran).FirstOrDefault();
                    foreach (Common.Entity.CarPurchase cd in carsDemand["new"])
                    {
                        conn.Execute($@"insert into [CarPurchase] (OrderID,CarID,CarColorID,WantSumbitCarDate,CarUsing,OtheRequirements,
                                                Users,Remake,RequirementNumber,IsApplyMaxCustomerResources,FaxOrderNo,SubmitDealerID,OldRequirementID,
                                                OldSumbitCarID,OldNo,RuckSack,WithNoCurtains,NameplateSeats,TableChang,Other,WantFTMSCarDateTime) values ({orderId},@CarID,@CarColorID,@WantSumbitCarDate,@CarUsing,@OtheRequirements,
                                                @Users,@Remake,@RequirementNumber,@IsApplyMaxCustomerResources,@FaxOrderNo,@SubmitDealerID,@OldRequirementID,
                                                @OldSumbitCarID,@OldNo,@RuckSack,@WithNoCurtains,@NameplateSeats,@TableChang,@Other,@WantFTMSCarDateTime)", cd, tran);
                    }
                    foreach (Common.Entity.CarRecord cr in carsPay["new"])
                    {
                        conn.Execute(@"insert into CarRecord (OrderID,CarPurchaseID,OriginalNo,CarID,CarColorID,DateSale,
                                                EngineNumber,FrameNumber,CarSalePrice,CarPreferentialMargin,CarBackMargin,LeaveRemarks,DateTabulation,
                                                BackMoney1,BackMoneyDate1,Regenerator1,BackMoney2,BackMoneyDate2,Regenerator2,CalculationMethod,BackMark,BackMark2,AuditStatus) values 
                                                (@OrderID,@CarPurchaseID,@OriginalNo,@CarID,@CarColorID,@DateSale,
                                                @EngineNumber,@FrameNumber,@CarSalePrice,@CarPreferentialMargin,@CarBackMargin,@LeaveRemarks,@DateTabulation,
                                                @BackMoney1,@BackMoneyDate1,@Regenerator1,@BackMoney2,@BackMoneyDate2,@Regenerator2,@CalculationMethod,@BackMark,@BackMark2,@AuditStatus)", cr, tran);
                    }
                    tran.Commit();
                    new DAL.OrderProcess().Add(OpratorName, Common.Utilities.enum流程名称枚举.订单生成.ToString(), model.DealerID, orderId, true);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    new Exception(ex.Message);
                }
            }
        }
        #endregion

        #region 修改订单
        public void Update(Common.Entity.Order model, IDictionary<string, IList<Common.Entity.CarPurchase>> carsDemand, IDictionary<string, IList<Common.Entity.CarRecord>> carsPay)
        {
            new DAL.Order().Update(model, carsDemand, carsPay);
        }
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="model">数据实体</param>
        /// <param name="carsDemand">车辆需求</param>
        /// <param name="carsPay">交车记录</param>
        /// <returns></returns>
        public void Update(Common.Entity.Order model, List<Common.Entity.CarPurchase> newList, List<Common.Entity.CarRecord> carsPay)
        {
            List<Common.Entity.CarPurchase> list = new DAL.CarPurchase().GetModel(model.PKID);
            List<Common.Entity.CarPurchase> AddList = new List<Common.Entity.CarPurchase>();
            List<Common.Entity.CarPurchase> DeleteList = new List<Common.Entity.CarPurchase>();
            List<Common.Entity.CarPurchase> UpdateList = new List<Common.Entity.CarPurchase>();
            Common.Entity.CarPurchase AddPurchase = null;
            Common.Entity.CarPurchase DeletePurchase = null;
            Common.Entity.CarPurchase UpdatePurchase = null;

            List<Common.Entity.CarRecord> AddRList = new List<Common.Entity.CarRecord>();
            List<Common.Entity.CarRecord> DeleteRList = new List<Common.Entity.CarRecord>();
            List<Common.Entity.CarRecord> UpdateRList = new List<Common.Entity.CarRecord>();
            //Common.Entity.CarRecord AddRContact = null;
            //Common.Entity.CarRecord DelReteContact = null;
            //Common.Entity.CarRecord UpdateRContact = null;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    List<Common.Entity.CarPurchase> newlist = newList.Where(x => x.PKID == list[i].PKID).ToList();
                    if (newlist.Count == 0)//查询已删除添加至List
                    {
                        DeletePurchase = new Common.Entity.CarPurchase();
                        DeletePurchase.PKID = list[i].PKID;
                        DeleteList.Add(DeletePurchase);
                    }
                }
            }
            for (int j = 0; j < newList.Count; j++)
            {
                if (newList[j].PKID == 0)//原数据不存在新数据（新增）
                {
                    AddPurchase = new Common.Entity.CarPurchase();
                    AddPurchase.OrderID = model.PKID;
                    AddPurchase.CarID = newList[j].CarID;
                    AddPurchase.CarColorID = newList[j].CarColorID;
                    AddPurchase.WantFTMSCarDateTime = newList[j].WantFTMSCarDateTime;
                    AddPurchase.CarUsing = newList[j].CarUsing;
                    AddPurchase.Remake = newList[j].Remake;
                    AddPurchase.RequirementNumber = newList[j].RequirementNumber;
                    AddPurchase.SumbitCarNumber = newList[j].SumbitCarNumber;
                    AddPurchase.IsApplyMaxCustomerResources = newList[j].IsApplyMaxCustomerResources;
                    AddPurchase.FaxOrderNo = newList[j].FaxOrderNo;
                    AddPurchase.SubmitDealerID = newList[j].SubmitDealerID;
                    AddPurchase.OldRequirementID = newList[j].OldRequirementID;
                    AddPurchase.OldSumbitCarID = newList[j].OldSumbitCarID;
                    AddPurchase.OldNo = newList[j].OldNo;
                    AddPurchase.RuckSack = newList[j].RuckSack;
                    AddPurchase.WithNoCurtains = newList[j].WithNoCurtains;
                    AddPurchase.NameplateSeats = newList[j].NameplateSeats;
                    AddPurchase.TableChang = newList[j].TableChang;
                    AddPurchase.Other = newList[j].Other;
                    AddPurchase.WantFTMSCarDateTime = newList[j].WantFTMSCarDateTime;
                    AddList.Add(AddPurchase);
                }
                else//源数据存在新数据（修改）
                {
                    UpdatePurchase = newList[j];
                    UpdateList.Add(UpdatePurchase);
                }
            }
            new DAL.Order().Update(model, AddList, DeleteList, UpdateList);
        }
        #endregion

        #region 检测订单是否符合提交要求，不检查订购车辆情况
        /// <summary>
        /// 检测订单是否符合提交要求，不检查订购车辆情况
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="dealerID"></param>
        /// <returns></returns>
        public bool CheckSubmitOrders(int customerID, int dealerID)
        {
            return CheckSubmitOrders(0, customerID, dealerID);
        }
        #endregion

        #region 订单销售单管理逻辑BLL层代码

        #region 获取全部订单信息
        public List<Common.Entity.Order> GetOrdersAllPager(int pageIndex, int pageSize, Common.Entity.Order filter, out int totalCount)
        {
            StringBuilder sqlCount = new StringBuilder();
            sqlCount.Append($@"select count(*) from (select A.* from [Order] A left join Customer B on A.CustomerID=B.PKID
                           left join Dealer C on A.DealerID=C.PKID 
                           left join SysUser D on C.PKID=D.DealerId 
                           where A.PKID>0 ");
            if (!string.IsNullOrEmpty(filter.Code))
            {
                sqlCount.Append(" and  A.Code like '%" + filter.Code + "%' or B.Name like '%" + filter.Code + "%' ");
            }
            if (filter.CarName != null && filter.CarName != "-1")
            {
                sqlCount.Append(" and Exists (select* from CarPurchase CP left join CarInfo CI on CP.CarID=CI.PKID  where CP.OrderID=A.PKID and CI.Name='" + filter.CarName + "')");
            }
            if (filter.OrderType > 0)
            {
                sqlCount.Append(" and A.OrderType=" + filter.OrderType + "");
            }
            if (!string.IsNullOrEmpty(filter.stateBegin))
            {
                sqlCount.Append("  and A.ToExamineState>=" + filter.stateBegin + "");
            }
            else if (!string.IsNullOrEmpty(filter.stateEnd))
            {
                sqlCount.Append("  and A.ToExamineState<=" + filter.stateEnd + "");
            }
            if (!string.IsNullOrEmpty(filter.carNo))
            {
                sqlCount.Append("   and Exists (select * from CarRecord CR where CR.OrderID = A.PKID and CR.FrameNumber like '%" + filter.carNo + "%')");
            }
            sqlCount.Append(" ) as TB");

            StringBuilder strSql = new StringBuilder();
            strSql.Append($@" select top {pageSize} *  from (select  DISTINCT row_number() over (order by A.PKID) as rowNumber,
                         D.Name as LoginName,B.Name 客户名称,C.Name as DealerName,Convert(varchar(100),A.CreateTime,23) as ToCreateTime,
						 Convert(varchar(100),A.SubmitTime,23) as ToSubmitTime,B.Name as CustomerName,cn.Name 客户性质1名称,x.性质2 客户性质2名称,A.* from [Order] A left join Customer B on 
                         A.CustomerID=B.PKID left join CustomerNature cn on B.CustomerNatureID=cn.PKID left join 
                        (select  DISTINCT CustomerNature2ID,cn.Name 性质2 from Customer c inner join CustomerNature cn on c.CustomerNature2ID=cn.PKID) x on B.CustomerNature2ID=x.CustomerNature2ID 
						left join Dealer C on A.DealerID=C.PKID left join SysUser D on C.PKID=D.DealerId where A.PKID>0");
            if (!string.IsNullOrEmpty(filter.Code))
            {
                strSql.Append(" and  A.Code like '%" + filter.Code + "%' or B.Name like '%" + filter.Code + "%' ");
            }
            if (filter.CarName != null && filter.CarName != "-1")
            {
                strSql.Append(" and Exists (select* from CarPurchase CP left join CarInfo CI on CP.CarID=CI.PKID  where CP.OrderID=A.PKID and CI.Name='" + filter.CarName + "')");
            }
            if (filter.OrderType > 0)
            {
                strSql.Append(" and A.OrderType=" + filter.OrderType + "");
            }
            if (!string.IsNullOrEmpty(filter.stateBegin))
            {
                strSql.Append("  and A.ToExamineState>=" + filter.stateBegin + "");
            }
            else if (!string.IsNullOrEmpty(filter.stateEnd))
            {
                strSql.Append("  and A.ToExamineState<=" + filter.stateEnd + "");
            }
            if (!string.IsNullOrEmpty(filter.carNo))
            {
                strSql.Append("   and Exists (select * from CarRecord CR where CR.OrderID = A.PKID and CR.FrameNumber like '%" + filter.carNo + "%')");
            }

            strSql.Append($" ) as T where T.rowNumber > ({pageIndex} - 1) * {pageSize} ");
            List<Common.Entity.Order> list = new List<Common.Entity.Order>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                totalCount = conn.Query<int>(sqlCount.ToString()).FirstOrDefault();
                list = conn.Query<Common.Entity.Order>(strSql.ToString()).ToList();
            }
            return list;
        }
        #endregion

        /// <summary>
        /// 修改订单类型
        /// </summary>
        /// <param name="ordersID"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public int ChangeOrderType(string ordersID, int orderType)
        {
            int FN_BifCustomer = (1 == orderType) ? 1 : 0;
            return new DAL.Order().ChangeOrderType(ordersID, orderType, FN_BifCustomer);
        }

        /// <summary>
        /// 设置订单状态
        /// </summary>
        /// <param name="ordersID"></param>
        /// <param name="statusInt"></param>
        /// <returns></returns>
        public int InitialiseStatus(string ordersID, int statusInt)
        {
            int editable = (statusInt > 0) ? 0 : 1;
            return new DAL.Order().InitialiseStatus(ordersID, statusInt, editable);
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="ordersID"></param>
        /// <returns></returns>
        public int Remove(string ordersID)
        {
            return new DAL.Order().Remove(ordersID);
        }

        /// <summary>
        /// 拆单
        /// </summary>
        /// <param name="ordersID"></param>
        /// <returns></returns>
        public void SplitOrders(string ordersID)
        {
            new DAL.Order().SplitOrders(ordersID);
        }

        /// <summary>
        /// 修改指定的订单的大客户配车需求为指定类型，订单号用“，”分割
        /// </summary>
        /// <param name="ordersListStr"></param>
        /// <param name="needPC"></param>
        public void ChangeOrdersNeeds(string ordersListStr, int needPC)
        {
            ordersListStr = string.Format("'{0}'", ordersListStr.Trim().Replace(" ", "").Replace(",", "','"));
            new DAL.Order().ChangeOrdersNeeds(ordersListStr, needPC);
        }

        /// <summary>
        /// 结束指定的订单，订单号用“，”分割
        /// </summary>
        /// <param name="ordersListStr"></param>
        public void FinishOrders(string ordersListStr)
        {
            ordersListStr = string.Format("'{0}'", ordersListStr.Trim().Replace(" ", "").Replace(",", "','"));
            new DAL.Order().FinishOrders(ordersListStr);
        }

        /// <summary>
        /// 获取一个订单的完整信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Common.Entity.ReportOrder GetOrderFullInfo(int orderId)
        {
            return GetOrderFullInfo(orderId, false);
        }

        #region 获取一个订单的完全信息 
        /// <summary>
        /// 获取一个订单的完全信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="viewOriginal">查看原始订购信息</param>
        /// <returns></returns>
        public Common.Entity.ReportOrder GetOrderFullInfo(int orderId, bool viewOriginal)
        {
            Common.Entity.ReportOrder ordersInfo = new ReportOrder();
            // 订单基本信息
            BLL.Order bll = new Order();
            Common.Entity.Order oInfo = bll.GetOrderByPKID(orderId);
            if (oInfo != null)
            {
                ordersInfo.BaseRemark = oInfo.BaseRemark;
                ordersInfo.IsApplyMaxCustomerResources = oInfo.IsApplyMaxCustomerResources == 1 ? "是" : "否";
                ordersInfo.BuyWay = oInfo.BuyWay;
                ordersInfo.CarUse = oInfo.CarUse;
                ordersInfo.CreateTime = oInfo.CreateTime == null ? new DateTime(1990, 1, 1) : (DateTime)oInfo.CreateTime;
                ordersInfo.CustomerName = oInfo.CustomerName;
                ordersInfo.CustomerSuggestion = oInfo.CustomerSuggestion;

                ordersInfo.DealerName = oInfo.DealerName;
                ordersInfo.DifferentPlace = oInfo.DifferentPlace == 1 ? "是" : "否";
                ordersInfo.CustomerID = oInfo.CustomerID;
                ordersInfo.DealerID = oInfo.DealerID;
                ordersInfo.PKID = oInfo.PKID;
                ordersInfo.Watchmaker = oInfo.Watchmaker;
                ordersInfo.Code = oInfo.Code;
                ordersInfo.ReComment1 = oInfo.ReComment1;
                ordersInfo.ReComment2 = oInfo.ReComment2;
                ordersInfo.ReComment3 = oInfo.ReComment3;
                ordersInfo.RecordName = oInfo.RecordName;
                ordersInfo.ReDate1 = oInfo.ReDate1;
                ordersInfo.ReDate2 = oInfo.ReDate2;
                ordersInfo.ReDate3 = oInfo.ReDate3;
                ordersInfo.ReRemark1 = oInfo.ReRemark1;
                ordersInfo.ReRemark2 = oInfo.ReRemark2;
                ordersInfo.ReRemark3 = oInfo.ReRemark3;
                ordersInfo.Replyer1 = oInfo.Replyer1;
                ordersInfo.Replyer2 = oInfo.Replyer2;
                ordersInfo.Replyer3 = oInfo.Replyer3;
                ordersInfo.OrderState = oInfo.OrderState;
                ordersInfo.SubmitType = oInfo.SubmitType;
                ordersInfo.BearUser = oInfo.BearUser;
                ordersInfo.FN_Email = oInfo.Email;
                ordersInfo.OffAddressOnCardReport = oInfo.OffAddressOnCardReport == 1 ? "是" : "否";
                ordersInfo.FN_InvoiceDiffer = oInfo.InvoiceAndCustomerAtypism == 1 ? "是" : "否";
                ordersInfo.FN_InvoiceName = oInfo.InvoiceCustomerInfo;
            }

            // 客户基本信息
            BLL.Customer customer = new Customer();
            Common.Entity.Customer cInfo = customer.GetCustomerByPKID(ordersInfo.CustomerID);
            if (cInfo != null)
            {
                ordersInfo.CustomerAddress = cInfo.Address;
                ordersInfo.CustomerChargeDept = cInfo.CompetentDepartment;
                ordersInfo.CustomerExecutiveDept = cInfo.ExecutiveDepartment;
                ordersInfo.CustomerMainBusiness = cInfo.MainBusiness;
                ordersInfo.CustomerProfiles = cInfo.CustomerProfiles;
                ordersInfo.CustomerUseDept = cInfo.UseDepartment;
                ordersInfo.CustomerZipCode = cInfo.Zip;
                ordersInfo.Customer_Type = cInfo.CustomerNatureName;
                ordersInfo.Customer_Type2 = cInfo.CustomerNatureName2;
                ordersInfo.Customer_Memo = cInfo.Remark;
                ordersInfo.CustomerNo = cInfo.EnterpriseCode;
            }

            if (oInfo.ToExamineState > 0)
            {
                ordersInfo.CustomerContact = customer.GetListByOrdersID(ordersInfo.PKID);
            }
            else
            {
                ordersInfo.CustomerContact = customer.GetListByCustomerID(ordersInfo.CustomerID);
            }

            if (viewOriginal)
            {
                BLL.CarPurchase cdBll = new CarPurchase();
                ordersInfo.CarPurchase = cdBll.GetOriginalList(ordersInfo.Code.Substring(0, ordersInfo.Code.IndexOf('.')));
            }
            else
            {
                BLL.CarPurchase cdBll = new CarPurchase();
                ordersInfo.CarPurchase = cdBll.GetList(ordersInfo.PKID);
            }

            // 经销店联系人
            BLL.DealerContact dcBLL = new DealerContact();
            if (oInfo.ToExamineState > 0)
            {
                ordersInfo.DealerContact = dcBLL.GetListByOrdersID(ordersInfo.PKID);
            }
            else
            {
                ordersInfo.DealerContact = dcBLL.GetListByDealerID(ordersInfo.DealerID);
            }

            return ordersInfo;
        }
        #endregion
        #endregion
        #endregion

        #region 销售单模块==================================================
        #region 获取销售单列表
        /// <summary>
        /// 获取销售单列表
        /// </summary>
        /// <param name="type">订单类别，1：订单，2：销售单</param>
        /// <param name="dealerId">经销店 编号</param>
        /// <param name="filter">过滤条件</param>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.Order> GetOrderSaleManagerPager(int type, int dealerId, Common.Entity.Order filter, int pageIndex, int pageSize, out int totalCount)
        {
            StringBuilder strCount = new StringBuilder();
            DynamicParameters param1 = new DynamicParameters();
            strCount.Append(@" select count(*) from (select O.* from [Order] O left join Customer B on O.CustomerID=B.PKID
						      left join Dealer D on O.DealerID=D.PKID left join SysUser SU on D.PKID=SU.DealerId");
            strCount.Append(" where O.PKID>0 and O.DealerID=@DealerID and O.OrderType=@type");
            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                strCount.Append(" and O.Code like '%" + filter.Code + "%' or B.Name like '%" + filter.Code + "%'");
            }
            strCount.Append(" ) temp");
            param1.Add("DealerID", dealerId);
            param1.Add("type", type);

            StringBuilder strSql = new StringBuilder();
            DynamicParameters param2 = new DynamicParameters();
            strSql.Append(@" select top (@pageSize) * from (select distinct row_number() over (order by O.CreateTime desc,O.Code desc) 
						     as rowNumber, O.*,D.Name  DealerName, B.Name CustomerName from [Order] O left join Customer B on O.CustomerID=B.PKID
						     left join Dealer D on O.DealerID=D.PKID left join SysUser SU on D.PKID=SU.DealerId where O.PKID>0
						     and O.DealerID=@dealerId and O.OrderType=@type");
            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                strSql.Append(" and O.Code like '%" + filter.Code + "%' or B.Name like '%" + filter.Code + "%'");
            }
            strSql.Append(@" ) as T where T.rowNumber>(@pageIndex-1)*@pageSize");
            param2.Add("dealerId", dealerId);
            param2.Add("type", type);
            param2.Add("pageIndex", pageIndex);
            param2.Add("pageSize", pageSize);
            List<Common.Entity.Order> model = new List<Common.Entity.Order>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                totalCount = conn.Query<int>(strCount.ToString(), param1).FirstOrDefault();
                model = conn.Query<Common.Entity.Order>(strSql.ToString(), param2).ToList();
            }
            return model;
        }
        #endregion
        #endregion

        #region 大客户审核业务逻辑层================================
        #region 获取订单初审分页（即大客户室审核列表）
        /// <summary>
        /// 获取订单初审分页（即大客户室审核列表）
        /// </summary>
        /// <param name="type">订单类别</param>
        /// <param name="dealerId">所属经销店</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.Order> GetOrdersByToExamineState(int type, int dealerId, Common.Entity.Order filter, int pageIndex, int pageSize, out int totalCount)
        {
            StringBuilder strCount = new StringBuilder();
            DynamicParameters param1 = new DynamicParameters();
            strCount.Append(@" select count(@pageSize) from (select A.* from [Order] A left join Customer B on A.CustomerID=B.PKID
                           left join Dealer C on A.DealerID=C.PKID 
                           left join SysUser D on C.PKID=D.DealerId 
                           where A.PKID>0  and A.DealerID=@DealerID and A.OrderType=@type and A.ToExamineState=200");
            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                strCount.Append(" and A.Code like '%" + filter.Code + "%' or B.Name like '%" + filter.Code + "%'");
            }
            strCount.Append(" ) temp");
            param1.Add("pageSize", pageSize);
            param1.Add("DealerID", dealerId);
            param1.Add("type", type);

            StringBuilder strSql = new StringBuilder();
            DynamicParameters param2 = new DynamicParameters();
            strSql.Append(@" select top (@pageSize) *  from (select  DISTINCT row_number() over (order by A.PKID) as rowNumber,
                         D.Name as LoginName,B.Name 客户名称,C.Name as DealerName,Convert(varchar(100),A.CreateTime,23) as ToCreateTime,
						 Convert(varchar(100),A.SubmitTime,23) as ToSubmitTime,B.Name as CustomerName,cn.Name 客户性质1名称,x.性质2 客户性质2名称,A.* from [Order] A left join Customer B on 
                         A.CustomerID=B.PKID left join CustomerNature cn on B.CustomerNatureID=cn.PKID left join 
                        (select  DISTINCT CustomerNature2ID,cn.Name 性质2 from Customer c inner join CustomerNature cn on c.CustomerNature2ID=cn.PKID) x on B.CustomerNature2ID=x.CustomerNature2ID 
						left join Dealer C on A.DealerID=C.PKID left join SysUser D on C.PKID=D.DealerId where A.PKID>0
                        and A.DealerID=@dealerId and A.OrderType=@type and A.ToExamineState=200");
            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                strSql.Append(" and A.Code like '%" + filter.Code + "%' or B.Name like '%" + filter.Code + "%'");
            }
            strSql.Append(@" ) as T where T.rowNumber>(@pageIndex-1)*@pageSize");
            param2.Add("dealerId", dealerId);
            param2.Add("type", type);
            param2.Add("pageIndex", pageIndex);
            param2.Add("pageSize", pageSize);
            List<Common.Entity.Order> model = new List<Common.Entity.Order>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                totalCount = conn.Query<int>(strCount.ToString(), param1).FirstOrDefault();
                model = conn.Query<Common.Entity.Order>(strSql.ToString(), param2).ToList();
            }
            return model;
        }
        #endregion

        #region 是否存在指定的车辆类型
        /// <summary>
        /// 是否存在指定的车辆类型
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="carName"></param>
        /// <returns></returns>
        public bool ExistsCarOrder(string orderId, string carName)
        {
            return new DAL.Order().ExistsCarOrder(orderId, carName);
        }
        #endregion

        /// <summary>
        /// 批量更新订单记录的返款审核信息
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <param name="user"></param>
        /// <param name="auditResult"></param>
        /// <param name="auditRemark"></param>
        public void BatchUpdateAudit(string orderIDs, string user, string auditResult, string auditRemark)
        {
            if (orderIDs.IndexOf("'") < 0)
                orderIDs = string.Format("'{0}'", orderIDs.Replace(",", "','"));
            int auditState = (auditResult.Equals("同意")) ? 310 : (auditResult.Equals("不同意")) ? -200 : -1000;
            new DAL.Order().BatchUpdateAudit(orderIDs, user, auditResult, auditRemark, auditState);
        }
        #endregion
    }
}
