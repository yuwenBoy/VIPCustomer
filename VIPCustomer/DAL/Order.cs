using Common.Entity;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DAL
{
    public class Order
    {
        #region 获取订单流水号
        /// <summary>
        /// 获取订单流水号
        /// </summary>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        public int GetNewOrdersNo(int dealerId)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                int result = 1;
                Hashtable paramHT = new Hashtable();
                StringBuilder sqlBuidler = new StringBuilder();
                sqlBuidler.Append("select DealerId,CONVERT(varchar(100),CreateTime,23) as CreateTime,SerialNumber from DealerOrderSerialNumberControl where DealerId=@DealerId");
                SqlParameter[] param1 = {
                    new SqlParameter("@DealerId",dealerId)
                };
                DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sqlBuidler.ToString(), param1);
                if (0 == dt.Rows.Count)
                {
                    StringBuilder strInsert = new StringBuilder();
                    strInsert.Append(@"Insert into DealerOrderSerialNumberControl(DealerId,CreateTime,SerialNumber) Values");
                    strInsert.Append("(@DealerId,@CreateTime,@SerialNumber)");
                    SqlParameter[] param2 = {
                        new SqlParameter("@DealerId",dealerId),
                        new SqlParameter("@CreateTime",DateTime.Now),
                        new SqlParameter("@SerialNumber",2)
                    };
                    paramHT.Add(strInsert, param2);
                }
                else
                {
                    if (dt.Rows[0]["CreateTime"].ToString() == DateTime.Now.ToString("yyyy-MM-dd"))
                        result = int.Parse(dt.Rows[0]["SerialNumber"].ToString());
                    StringBuilder strUpdate = new StringBuilder();
                    strUpdate.Append(@"UPDATE DealerOrderSerialNumberControl set CreateTime=@CreateTime,SerialNumber=@SerialNumber where DealerId=@DealerId");
                    SqlParameter[] param3 = {
                        new SqlParameter("@DealerId",dealerId),
                        new SqlParameter("@CreateTime",DateTime.Now),
                        new SqlParameter("@SerialNumber",result+1)
                    };
                    paramHT.Add(strUpdate, param3);
                }

                int count = Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, paramHT));
                tran.Complete();
                if (count > 0)
                {
                    return result;
                }
                else return 0;
            }
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

            string sql = string.Format($@"select B.Name CustomerName,
                                        B.EnterpriseCode,B.Eamil Email,
                                        B.[Address] CustomerAddress,
                                        B.Zip CustomerZipCode,
                                        B.CompetentDepartment,
                                        B.ExecutiveDepartment,
                                        B.CustomerProfiles,
                                        B.MainBusiness,
                                        B.UseDepartment,
                                        B.Remark,C.Name DealerName,A.*,
                                        Convert(varchar(100),A.CreateTime,23) as ToCreateTime,
                                        D.Name as LoginName,
						                Convert(varchar(100),A.SubmitTime,23) as ToSubmitTime from [Order] A 
                                       left join Customer B on A.CustomerID=B.PKID
                                       left join Dealer C on A.DealerID=C.PKID 
                                       left join SysUser D on C.PKID=D.DealerId 
                                       where A.PKID={orderId}");
            Common.Entity.Order order = new Common.Entity.Order();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                order = conn.Query<Common.Entity.Order>(sql).FirstOrDefault();
            }
            return order;
        }
        public Common.Entity.Order GetOrderByPKID(string orderId)
        {

            string sql = string.Format($@"select B.Name CustomerName,
                                        B.EnterpriseCode,B.Eamil Email,
                                        B.[Address] CustomerAddress,
                                        B.Zip CustomerZipCode,
                                        B.CompetentDepartment,
                                        B.ExecutiveDepartment,
                                        B.CustomerProfiles,
                                        B.MainBusiness,
                                        B.UseDepartment,
                                        B.Remark,C.Name DealerName,A.*,
                                        Convert(varchar(100),A.CreateTime,23) as ToCreateTime,
                                        D.Name as LoginName,
						                Convert(varchar(100),A.SubmitTime,23) as ToSubmitTime from [Order] A 
                                       left join Customer B on A.CustomerID=B.PKID
                                       left join Dealer C on A.DealerID=C.PKID 
                                       left join SysUser D on C.PKID=D.DealerId 
                                       where A.PKID={orderId}");
            Common.Entity.Order order = new Common.Entity.Order();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                order = conn.Query<Common.Entity.Order>(sql).FirstOrDefault();
            }
            return order;
        }
        #endregion

        /// <summary>
        /// 作废订单，返回成功的ordersID
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <returns></returns>
        public IList<int> Delete(int orderIDs)
        {
            IList<int> result = new List<int>();
            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"update [Order] set ToExamineState=-1000,IsEdit=0 where PKID=@PKID and IsEdit!=0");
                    SqlParameter[] param = {
                            new SqlParameter("@PKID",orderIDs)
                        };


                    int del = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
                    if (del > 0)
                    {
                        result.Add(del);
                    }
                    tran.Complete();
                    return result;
                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取订单中的订购车型
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IList<string> GetCarsOrder(int orderId)
        {
            StringBuilder strSql = new StringBuilder(@" select CI.Name,CP.OrderID from CarPurchase CP 
                                                       left join CarInfo CI on CP.CarID=CI.PKID where CP.OrderID=@OrderID");
            SqlParameter[] param = {
                new SqlParameter("@OrderID",orderId)
            };
            IList<CarsOrder> list = SqlHelper.GetList<CarsOrder>(SqlHelper.connStr, strSql.ToString(), CommandType.Text, param);
            return list.Select(m => m.Name).ToList();
        }

        #region 提交订单
        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="modelId">订单ID组</param>
        /// <param name="customerId">客户ID组</param>
        /// <param name="dealerId">经销店ID组</param>
        /// <param name="ToExamineState"></param>
        public void SubmitOrders(int[] modelId, int[] customerId, int dealerId, int ToExamineState)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    for (int i = 0; i < modelId.Length; i++)
                    {
                        // 更新状态值
                        string sqlUpdate = string.Format($@"update[order] set ToExamineState = { ToExamineState} ,IsEdit = 0,
                                                           SubmitTime = {DateTime.Now.ToString("yyyy - MM - dd")} where PKID = {modelId[i] } and IsEdit != 0");
                        // 删除旧联系人 
                        string sqlDelDC = string.Format($@" delete from OrderDealerContact where OrderID={modelId[i]}");
                        string sqlDelCC = string.Format($@" delete from OrderCustomerContact where OrderID={modelId[i]}");

                        // 复制联系人  
                        string sqlInsertDC = string.Format($@"INSERT INTO OrderCustomerContact (OrderID,Name,Job,RoleID,Phone,MobileTel,Fax,Email,OtherContactInfo,Birthday,Hobbies,Remark)
                                                           	SELECT {modelId[i]},Name,Job,RoleID,Phone,MobileTel,Fax,Email,OtherContactInfo,Birthday,Hobbies,Remark from CustomerContact 
                                                            where PKID={customerId[i]}");
                        string sqlInsertCC = string.Format($@"INSERT INTO OrderDealerContact (OrderID,Name,JobID,Phone,MobileTel,Fax,Email,OtherContactInfo,Remark)
                                                       	      SELECT {modelId[i]},Name,JobID,Phone,MobileTel,Fax,Email,OtherContactInfo,Remark from DealerContact where PKID={dealerId}");

                        conn.Execute(sqlUpdate, null, tran);
                        conn.Execute(sqlDelDC, null, tran);
                        conn.Execute(sqlDelCC, null, tran);
                        conn.Execute(sqlInsertDC, null, tran);
                        conn.Execute(sqlInsertCC, null, tran);
                        tran.Commit();
                    }
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
        }
        #endregion


        public void Update(Common.Entity.Order model, List<Common.Entity.CarPurchase> addList, List<Common.Entity.CarPurchase> deleteList, List<Common.Entity.CarPurchase> updateList)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                Hashtable strStringList = new Hashtable();
                try
                {
                    if (model.PKID > 0)
                    {
                        string sql = string.Format(@"update [Order] set
                                                BuyWay=@BuyWay,
                                                OrderState=@OrderState,
                                                SubmitType=@SubmitType,
                                                PurchaseType=@PurchaseType,
                                                CustomerID=@CustomerID,
                                                RecordName=@RecordName,
                                                CarUse=@CarUse,
                                                DifferentPlace=@DifferentPlace,
                                                CustomerSuggestion=@CustomerSuggestion,
                                                BaseRemark=@BaseRemark,
                                                IsApplyMaxCustomerResources=@IsApplyMaxCustomerResources,
                                                OrderType=@OrderType,
                                                OffAddressOnCardReport=@OffAddressOnCardReport,
                                                InvoiceAndCustomerAtypism=@InvoiceAndCustomerAtypism,
                                                InvoiceCustomerInfo=@InvoiceCustomerInfo
                                                where PKID=@PKID and (OrderState='协商' or IsEdit=1) and ToExamineState<300 and ToExamineState>-1000");
                        conn.Execute(sql, model, tran);
                        strStringList.Add(sql, model);
                    }
                    if (addList.Count > 0) // 新增
                    {
                        foreach (Common.Entity.CarPurchase item in addList)
                        {
                            string sql1 = string.Format(@"insert into CarPurchase (OrderID,CarID,CarColorID,WantSumbitCarDate,CarUsing,OtheRequirements,Users,Remake,RequirementNumber,
                                                      SumbitCarNumber,IsApplyMaxCustomerResources,FaxOrderNo,SubmitDealerID,OldRequirementID,OldSumbitCarID,OldNo,RuckSack,WithNoCurtains,
                                                      NameplateSeats,TableChang,Other,WantFTMSCarDateTime) values(@OrderID,@CarID,@CarColorID,@WantSumbitCarDate,@CarUsing,@OtheRequirements,@Users,@Remake,@RequirementNumber,
                                                      @SumbitCarNumber,@IsApplyMaxCustomerResources,@FaxOrderNo,@SubmitDealerID,@OldRequirementID,@OldSumbitCarID,@OldNo,@RuckSack,@WithNoCurtains,
                                                      @NameplateSeats,@TableChang,@Other,@WantFTMSCarDateTime)");
                            conn.Execute(sql1, item, tran);
                            strStringList.Add(sql1, item);
                        }
                    }
                    if (deleteList.Count > 0) // 删除
                    {
                        foreach (Common.Entity.CarPurchase item in deleteList)
                        {
                            string sql2 = string.Format(@"delete from CarPurchase where OrderID=@OrderID and PKID not in (" + item.PKID + ")", new { OrderID = model.PKID });
                            conn.Execute(sql2, item, tran);
                        }
                    }
                    if (updateList.Count > 0) // 编辑
                    {
                        foreach (Common.Entity.CarPurchase item in updateList)
                        {
                            string sql3 = string.Format(@"update  CarPurchase set
	                                                  CarID=@CarID,
	                                                  CarColorID=@CarColorID,
	                                                  WantSumbitCarDate=@WantSumbitCarDate,
	                                                  CarUsing=@CarUsing,
	                                                  OtheRequirements=@OtheRequirements,
	                                                  Users=@Users,
	                                                  Remake=@Remake,
	                                                  RequirementNumber=@RequirementNumber,
	                                                  IsApplyMaxCustomerResources=@IsApplyMaxCustomerResources,
	                                                  SubmitDealerID=@SubmitDealerID,
	                                                  FaxOrderNo=@FaxOrderNo,
	                                                  OldNo=@OldNo,
	                                                  RuckSack=@RuckSack,
	                                                  WithNoCurtains=@WithNoCurtains,
	                                                  NameplateSeats=@NameplateSeats,
	                                                  TableChang=@TableChang,
	                                                  Other=@Other,
	                                                  WantFTMSCarDateTime=@WantFTMSCarDateTime
	                                                  where PKID=@PKID");
                            conn.Execute(sql3, item, tran);
                        }
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }

        #region 修改订单
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="model">数据实体</param>
        /// <param name="carsDemand">车辆需求</param>
        /// <param name="carsPay">交车记录</param>
        /// <returns></returns>
        public void Update(Common.Entity.Order model, IDictionary<string, IList<Common.Entity.CarPurchase>> carsDemand, IDictionary<string, IList<Common.Entity.CarRecord>> carsPay)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    conn.Execute(@"update [Order] set
                                                BuyWay=@BuyWay,
                                                OrderState=@OrderState,
                                                SubmitType=@SubmitType,
                                                PurchaseType=@PurchaseType,
                                                CustomerID=@CustomerID,
                                                RecordName=@RecordName,
                                                CarUse=@CarUse,
                                                DifferentPlace=@DifferentPlace,
                                                CustomerSuggestion=@CustomerSuggestion,
                                                BaseRemark=@BaseRemark,
                                                IsApplyMaxCustomerResources=@IsApplyMaxCustomerResources,
                                                OrderType=@OrderType,
                                                OffAddressOnCardReport=@OffAddressOnCardReport,
                                                InvoiceAndCustomerAtypism=@InvoiceAndCustomerAtypism,
                                                InvoiceCustomerInfo=@InvoiceCustomerInfo
                                                where PKID=@PKID and (OrderState='协商' or IsEdit=1) and ToExamineState<300 and ToExamineState>-1000", model, tran);

                    if (carsDemand["exist"].Count > 0 || carsDemand["new"].Count > 0)
                    {
                        string ids = string.Format("{0}", 0);
                        foreach (Common.Entity.CarPurchase cd in carsDemand["exist"])
                        {
                            ids += string.Format(",{0}", cd.PKID);
                        }
                        string sql = string.Format(@"delete from CarRecord where OrderID=@OrderID and CarPurchaseID not in(" + ids + ")");
                        conn.Execute(sql, new { OrderID = model.PKID }, tran);
                        conn.Execute(@"delete from CarPurchase where OrderID=@OrderID and PKID not in (" + ids + ")", new { OrderID = model.PKID }, tran);
                    }
                    foreach (Common.Entity.CarPurchase cd in carsDemand["exist"])
                    {
                        conn.Execute(@"update  CarPurchase set
	                                                  CarID=@CarID,
	                                                  CarColorID=@CarColorID,
	                                                  WantSumbitCarDate=@WantSumbitCarDate,
	                                                  CarUsing=@CarUsing,
	                                                  OtheRequirements=@OtheRequirements,
	                                                  Users=@Users,
	                                                  Remake=@Remake,
	                                                  RequirementNumber=@RequirementNumber,
	                                                  IsApplyMaxCustomerResources=@IsApplyMaxCustomerResources,
	                                                  SubmitDealerID=@SubmitDealerID,
	                                                  FaxOrderNo=@FaxOrderNo,
	                                                  OldNo=@OldNo,
	                                                  RuckSack=@RuckSack,
	                                                  WithNoCurtains=@WithNoCurtains,
	                                                  NameplateSeats=@NameplateSeats,
	                                                  TableChang=@TableChang,
	                                                  Other=@Other,
	                                                  WantFTMSCarDateTime=@WantFTMSCarDateTime
	                                                  where PKID=@PKID", cd, tran);
                    }
                    foreach (Common.Entity.CarPurchase cd in carsDemand["new"])
                    {
                        conn.Execute(@"insert into CarPurchase (OrderID,CarID,CarColorID,WantSumbitCarDate,CarUsing,OtheRequirements,Users,Remake,RequirementNumber,
                                                      SumbitCarNumber,IsApplyMaxCustomerResources,FaxOrderNo,SubmitDealerID,OldRequirementID,OldSumbitCarID,OldNo,RuckSack,WithNoCurtains,
                                                      NameplateSeats,TableChang,Other,WantFTMSCarDateTime) values(@OrderID,@CarID,@CarColorID,@WantSumbitCarDate,@CarUsing,@OtheRequirements,@Users,@Remake,@RequirementNumber,
                                                      @SumbitCarNumber,@IsApplyMaxCustomerResources,@FaxOrderNo,@SubmitDealerID,@OldRequirementID,@OldSumbitCarID,@OldNo,@RuckSack,@WithNoCurtains,
                                                      @NameplateSeats,@TableChang,@Other,@WantFTMSCarDateTime)", cd, tran);
                    }
                    if (carsPay["exist"].Count > 0 || carsPay["new"].Count > 0)
                    {
                        string ids = string.Format("'{0}'", 0);
                        foreach (Common.Entity.CarRecord cr in carsPay["exist"])
                        {
                            ids += string.Format(",'{0}'", cr.PKID);
                        }
                        Hashtable paramHT = new Hashtable();
                        paramHT["OrderID"] = model.PKID;
                        paramHT["PKID"] = ids;
                        conn.Execute(@"delete from CarRecord where OrderID=@OrderID and PKID not in (@PKID)", paramHT, tran);
                    }

                    foreach (Common.Entity.CarRecord cr in carsPay["exist"])
                    {
                        conn.Execute(@"update CarRecord set
                                     OriginalNo=@OriginalNo,
                                     DateSale=@DateSale,
                                     EngineNumber=@EngineNumber,
                                     FrameNumber=@FrameNumber,
                                     CarSalePrice=@CarSalePrice,
                                     CarPreferentialMargin=@CarPreferentialMargin,
                                     CarBackMargin=@CarBackMargin,
                                     LeaveRemarks=@LeaveRemarks,
                                     DateTabulation=@DateTabulation
                                     where PKID=@PKID and AuditStatus is null", cr, tran);
                    }
                    foreach (Common.Entity.CarRecord cr in carsPay["new"])
                    {
                        conn.Execute(@"insert into CarRecord (OrderID,CarPurchaseID,OriginalNo,CarID,CarColorID,DateSale,EngineNumber,FrameNumber,CarSalePrice,CarPreferentialMargin,CarBackMargin,LeaveRemarks,DateTabulation)
                                       values (@OrderID,@CarPurchaseID,@OriginalNo,@CarID,@CarColorID,@DateSale,@EngineNumber,@FrameNumber,@CarSalePrice,@CarPreferentialMargin,@CarBackMargin,@LeaveRemarks,@DateTabulation)", cr, tran);
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }
        #endregion


        #region ================订单销售单模块=========================

        /// <summary>
        /// 修改订单类型
        /// </summary>
        /// <param name="ordersID"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public int ChangeOrderType(string ordersID, int orderType, int FN_BigCustomer)
        {
            int result = 0;
            DynamicParameters Parameters = new DynamicParameters();
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" update [Order] set ToExamineState=0,IsEdit=1,OrderType=@OrderType,IsApplyMaxCustomerResources=@FN_BagCustomer where PKID=@PKID
	                                          update CarPurchase set IsApplyMaxCustomerResources=@FN_BagCustomer where OrderID=@PKID");
                Parameters.Add("OrderType", orderType);
                Parameters.Add("FN_BagCustomer", FN_BigCustomer);
                Parameters.Add("PKID", ordersID);
                result = conn.Execute(sql, Parameters);
            }
            if (result > 0)
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 设置订单状态
        /// </summary>
        /// <param name="ordersID"></param>
        /// <param name="statusInt"></param>
        /// <returns></returns>
        public int InitialiseStatus(string ordersID, int statusInt, int editable)
        {
            int result = 0;
            DynamicParameters Parameters = new DynamicParameters();
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" update [Order] set ToExamineState=@ToExamineState, IsEdit=@IsEdit where PKID=@PKID");
                Parameters.Add("ToExamineState", statusInt);
                Parameters.Add("IsEdit", editable);
                Parameters.Add("PKID", ordersID);
                result = conn.Execute(sql, Parameters);
            }
            if (result > 0)
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="ordersID"></param>
        /// <returns></returns>
        public int Remove(string ordersID)
        {
            int result = 0;
            DynamicParameters Parameters = new DynamicParameters();
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@" delete from  [Order]  where PKID=@PKID");
                Parameters.Add("PKID", ordersID);
                result = conn.Execute(sql, Parameters);
            }
            if (result > 0)
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        #region 拆单
        /// <summary>
        /// 拆单
        /// </summary>
        /// <param name="ordersID"></param>
        /// <returns></returns>
        public void SplitOrders(string ordersID)
        {
            List<Common.Entity.CarPurchase> splitCars = SplitOrders_GetCarName(ordersID);
            if (splitCars.Count < 2)
            {
                return;
            }
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    string newNoTemplete = string.Format("{0}.{{0}}", GetModel(ordersID)[0].Code);
                    for (int i = 1; i <= splitCars.Count; i++)
                    {
                        SplitOrders_DupOrders(ordersID, string.Format(newNoTemplete, i.ToString()), splitCars[i - 1].IsApplyMaxCustomerResources);
                        SplitOrders_UpdateCarDemand(splitCars[i - 1].CarName, splitCars[i - 1].IsApplyMaxCustomerResources, ordersID);
                    }
                    SplitOrders_UpdatePayCar(ordersID);
                    SplitOrders_UpdateDistributeCar(ordersID);
                    SplitOrders_DeleteOld(ordersID);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }

        /// <summary>
        /// 获取一个订单中所定购的车型
        /// </summary>
        /// <param name="ordersID"></param>
        /// <returns></returns>
        public List<Common.Entity.CarPurchase> SplitOrders_GetCarName(string ordersID)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                DynamicParameters Parameters = new DynamicParameters();
                string sql = string.Format(@" select distinct C.Name,CP.IsApplyMaxCustomerResources from CarPurchase CP
                                              left join CarInfo C on CP.CarID=C.PKID where CP.OrderID=@OrderID");
                Parameters.Add("OrderID", ordersID);
                List<Common.Entity.CarPurchase> list = conn.Query<Common.Entity.CarPurchase>(sql, Parameters).ToList();
                return list;
            }
        }

        /// <summary>
        /// 复制订单
        /// </summary>
        /// <param name="ordersID"></param>
        /// <param name="newOrdersNo"></param>
        /// <param name="bigCustomer"></param>
        public void SplitOrders_DupOrders(string ordersID, string newOrdersNo, string bigCustomer)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                DynamicParameters Parameters = new DynamicParameters();
                string sql = string.Format(@"insert into [Order] (Code,BuyWay,CreateTime,SubmitTime,DealerID,OrderState, SubmitType,PurchaseType,CustomerID,RecordName,
                                           BearUser, Watchmaker, CarUse, DifferentPlace, CustomerSuggestion, BaseRemark, Replyer1, ReComment1, ReDate1, ReRemark1,
                                           Replyer2, ReComment2, ReDate2, ReRemark2, Replyer3, ReComment3, ReDate3, ReRemark3, FTMSBackAuditor, FTMSBackSuggestion, FTMSBackToExamineDate,
                                           FTMSBackRemark, IsApplyMaxCustomerResources, ToExamineState, IsEdit, OrderType, YuanDanID, OffAddressOnCardReport, InvoiceAndCustomerAtypism, InvoiceCustomerInfo) 
                                           select @Code,BuyWay,CreateTime,SubmitTime,DealerID,OrderState,SubmitType,PurchaseType,CustomerID,RecordName,
                                           BearUser, Watchmaker,CarUse, DifferentPlace, CustomerSuggestion, BaseRemark,Replyer1,ReComment1,ReDate1, ReRemark1,
                                           Replyer2, ReComment2, ReDate2,ReRemark2, Replyer3, ReComment3, ReDate3,ReRemark3,FTMSBackAuditor,FTMSBackSuggestion,FTMSBackToExamineDate,
                                           FTMSBackRemark, IsApplyMaxCustomerResources,ToExamineState,IsEdit, OrderType, YuanDanID, OffAddressOnCardReport, InvoiceAndCustomerAtypism, InvoiceCustomerInfo
                                           from [Order] where PKID=@PKID;

                                           insert into OrderCustomerContact(OrderID, Name, Job, RoleID, Phone, MobileTel, Fax, Email, OtherContactInfo, Birthday, Hobbies, Remark)
                                           select  @OrderID,Name, Job, RoleID, Phone, MobileTel, Fax, Email, OtherContactInfo, Birthday, Hobbies, Remark
                                           from OrderCustomerContact where OrderID=@PKID;

                                           insert into OrderDealerContact(OrderID, Name, JobID, Phone, MobileTel, Fax, Email, OtherContactInfo, Remark)
                                           select  @OrderID, Name, JobID, Phone, MobileTel, Fax, Email, OtherContactInfo, Remark
                                           from OrderDealerContact where OrderID=@PKID

                                           insert into OrderProcess( OrderId, OperationName, Operator, OperationDate, NextOperator)
                                           select  @OrderID, OperationName, Operator, OperationDate, NextOperator
                                           from OrderProcess where OrderId=@PKID;");
                sql += string.Format(@" SELECT CAST(SCOPE_IDENTITY() as int)"); // 返回自增长ID
                string newID = "SCOPE_IDENTITY()";
                Parameters.Add("OrderID", newID);
                Parameters.Add("PKID", ordersID);
                Parameters.Add("Code", newOrdersNo);
                conn.Execute(sql, Parameters);
            }
        }

        /// <summary>
        /// 更新车辆订购
        /// </summary>
        /// <param name="carName"></param>
        /// <param name="bigStatus"></param>
        /// <param name="ordersID"></param>
        public void SplitOrders_UpdateCarDemand(string carName, string bigStatus, string ordersID)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                DynamicParameters Parameters = new DynamicParameters();
                string sql = string.Format(@"update CarPurchase set CarPurchase.OrderID=@newID from CarPurchase
	                                        inner join CarInfo on CarPurchase.CarID=CarInfo.PKID
	                                        where CarPurchase.OrderID=@PKID and CarPurchase.IsApplyMaxCustomerResource=@bigStatus and CarInfo.Name=@Name");
                sql += string.Format(@" SELECT CAST(SCOPE_IDENTITY() as int)"); // 返回自增长ID
                string newID = "SCOPE_IDENTITY()";
                Parameters.Add("newID", newID);
                Parameters.Add("PKID", ordersID);
                Parameters.Add("bigStatus", bigStatus);
                Parameters.Add("Name", carName);
                conn.Execute(sql, Parameters);
            }
        }
        /// <summary>
        /// 得到一个实体
        /// </summary>
        /// <param name="ordersID"></param>
        /// <returns></returns>
        public List<Common.Entity.Order> GetModel(string ordersID)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                DynamicParameters Parameters = new DynamicParameters();
                string sql = string.Format(@" select * from [Order] where PKID=@OrderID");
                Parameters.Add("OrderID", ordersID);
                List<Common.Entity.Order> list = conn.Query<Common.Entity.Order>(sql).ToList();
                return list;
            }
        }

        /// <summary>
        /// 更新配车记录信息
        /// </summary>
        /// <param name="ordersID"></param>
        public void SplitOrders_UpdateDistributeCar(string ordersID)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                DynamicParameters Parameters = new DynamicParameters();
                string sql = string.Format(@" update WithCarDuiYing set WithCarDuiYing.OrderID=CarPurchase.OrderID
	                                          from WithCarDuiYing inner join CarPurchase on WithCarDuiYing.CarPurchaseID=CarPurchase.PKID
	                                          where WithCarDuiYing.OrderID=@PKID");
                Parameters.Add("PKID", ordersID);
                conn.Execute(sql, Parameters);
            }
        }

        /// <summary>
        /// 更新交车记录信息
        /// </summary>
        /// <param name="ordersID"></param>
        public void SplitOrders_UpdatePayCar(string ordersID)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                DynamicParameters Parameters = new DynamicParameters();
                string sql = string.Format(@" update CarRecord set CarRecord.OrderID=CarPurchase.OrderID
	                                          from CarRecord inner join CarPurchase on CarRecord.CarPurchaseID=CarPurchase.PKID
	                                          where CarRecord.OrderID=@PKID");
                Parameters.Add("PKID", ordersID);
                conn.Execute(sql, Parameters);
            }
        }

        /// <summary>
        /// 删除旧单
        /// </summary>
        /// <param name="ordersID"></param>
        public void SplitOrders_DeleteOld(string ordersID)
        {
            using (SqlConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                DynamicParameters Parameters = new DynamicParameters();
                string sql = string.Format(@" delete from [Order] where PKID=@PKID");
                Parameters.Add("PKID", ordersID);
                conn.Execute(sql, Parameters);
            }
        }

        #endregion

        /// <summary>
        /// 修改指定的订单的大客户配车需求为指定类型，订单号用“，”分割
        /// </summary>
        /// <param name="ordersListStr"></param>
        /// <param name="needPC"></param>
        public void ChangeOrdersNeeds(string ordersListStr, int needPC)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"update [Order] set IsApplyMaxCustomerResources=@needType where PKID in ({ordersListStr}) and (ToExamineState between 1 and 999) and OrderType=1");
                DynamicParameters Parameters = new DynamicParameters();
                Parameters.Add("needType", needPC);
                conn.Execute(sql, Parameters);
            }
        }

        #region 检查订单是否允许返款
        /// <summary>
        /// 检查订单是否允许返款 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CanRebatesOrder(string orderId)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                //string sql = string.Format(@"select count(1) from [Order] where PKID=@OrderId and 是否可返款=1"); 数据库没有是否可返款字段，不知道这儿这个条件什么意思？？？
                string sql = string.Format(@"select count(1) from [Order] where PKID=@OrderId");
                DynamicParameters Parameters = new DynamicParameters();
                Parameters.Add("@OrderId", orderId);
                bool result = Convert.ToBoolean(conn.Query<int>(sql, Parameters).FirstOrDefault());
                if (result)
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #region 检查返款条件是否符合
        /// <summary>
        /// 检查返款条件是否符合 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CheckRebatesCars(string orderId)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"select @CarCount=count(1) from CarRecord CR left join CarInfo CI on CR.CarID=CI.PKID 
                                            and OrderID=@OrderID and CI.Name in (select CarName from BrandDictionary where BackMark=1)");
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderID", orderId);
                param.Add("CarCount", 0);
                int result = conn.Query<int>(sql, param).FirstOrDefault(); // 查询出来的carCount个数最后赋值给@CarCount变量
                return int.Parse(param.Get<int>("@CarCount").ToString()) > 0;
            }
        }
        #endregion

        #region 提交返款审核
        /// <summary>
        /// 提交返款审核 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool SubmitRebates(string orderId, int status)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"update [Order] set ToExamineState=@ToExamineState,IsEdit=0 where PKID=@OrderID and ToExamineState<400");
                DynamicParameters param = new DynamicParameters();
                param.Add("ToExamineState", status);
                param.Add("OrderID", orderId);
                // 更新订单状态为410状态，上交车辆
                bool result = conn.Execute(sql, param) > 0;
                return result;
            }
        }
        #endregion    

        /// <summary>
        /// 结束指定的订单，订单号用“，”分割
        /// </summary>
        /// <param name="ordersListStr"></param>
        public void FinishOrders(string ordersListStr)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"update [Order]  set ToExamineState=1000,
                                              IsEdit=0 where PKID in ({ordersListStr}) and (ToExamineState between 1 and 999) ");
                conn.Execute(sql);
            }
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
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"select count(1) from CarPurchase CP
									  inner join CarInfo CI on CP.CarID=CI.PKID
									  inner join [Order] OD on CP.OrderID=OD.PKID where CP.OrderID=@OrderID and CI.Name in ({carName})");
                DynamicParameters param = new DynamicParameters();
                param.Add("OrderID", orderId);
                int result = conn.Query<int>(sql, param).FirstOrDefault();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        /// <summary>
        /// 批量审核配车订单
        /// </summary>
        /// <param name="ordersIDs"></param>
        /// <param name="user"></param>
        /// <param name="auditResult"></param>
        /// <param name="auditRemark"></param>
        /// <param name="auditState"></param>
        public void BatchUpdateAudit(string ordersIDs, string user, string auditResult, string auditRemark, int auditState)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format($@"update [Order] set Replyer3=@Replyer3,ReComment3=@ReComment3,
                                             ReDate3=@ReDate3,ReRemark3=@ReRemark3,
                                             ToExamineState=@ToExamineState,IsEdit=@IsEdit where PKID in ({ordersIDs}) and ToExamineState<=200");
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Replyer3", user);
                    param.Add("ReComment3", auditResult);
                    param.Add("ReDate3", DateTime.Now.Date);
                    param.Add("ReRemark3", auditRemark);
                    param.Add("ToExamineState", auditState);
                    param.Add("IsEdit", (auditState < 0) ? 1 : 0);
                    conn.Execute(sql, param);
                }
            }
            catch
            {
            }
        }
    }
}