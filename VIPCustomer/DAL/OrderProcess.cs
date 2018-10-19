using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace DAL
{
    public class OrderProcess
    {
        /// <summary>
        /// 把用户列表转换为字符串形式
        /// </summary>
        /// <param name="uList"></param>
        /// <returns></returns>
        private static string userList(List<Common.SysUserEntity> uList)
        {
            StringBuilder sbu = new StringBuilder();
            string userTemplete = "{0}, {1}; ";
            foreach (Common.SysUserEntity u in uList)
                sbu.AppendFormat(userTemplete, u.Name, u.Phone);

            return sbu.ToString();
        }

        /// <summary>
        /// 获取大客户室审批人员
        /// </summary>
        /// <returns></returns>
        private static List<Common.SysUserEntity> GetFTMSUser()
        {
            List<Common.SysUserEntity> list = new List<Common.SysUserEntity>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format(@"select distinct U.PKID,U.Name,
                            U.NickName,U.Email,U.Phone from SysUser U
                            inner join UserRole UR on U.PKID=UR.UserID and U.UserTypeId=6
                            inner join RoleModule RM on RM.RoleID=UR.RoleID and RM.ModuleID=249");
                list = conn.Query<Common.SysUserEntity>(sql).ToList();
                return list;
            }
        }

        /// <summary>
        /// 获取大客户室返款人员
        /// </summary>
        /// <returns></returns>
        private static List<Common.SysUserEntity> GetRebatesUser(int orderId)
        {
            List<Common.SysUserEntity> list = new List<Common.SysUserEntity>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"select distinct U.PKID,U.Name,
                            U.NickName,U.Email,U.Phone from SysUser U
                            inner join UserRole UR on UR.UserID=U.PKID 
                            and U.UserTypeId=6 and UR.RoleID in(select distinct R.RoleID from CarPurchase O
                            inner join CarInfo C on O.CarID=C.PKID
                            inner join RoleBrand R on C.Name=R.CarName
                            and O.OrderID={orderId})
                            inner join RoleModule RM on RM.RoleID=UR.RoleID
                            and RM.ModuleID=251");
                list = conn.Query<Common.SysUserEntity>(sql).ToList();
                return list;
            }
        }

        /// <summary>
        /// 获取经销店的地担和大区经理
        /// </summary>
        /// <param name="dealerID"></param>
        /// <returns></returns>
        private static List<Common.SysUserEntity> GetDDDQUser(int dealerId)
        {
            List<Common.SysUserEntity> list = new List<Common.SysUserEntity>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"select distinct  U.PKID,U.Name,
                                            U.NickName,U.Email,U.Phone from SysUser U
                                            inner join Dealer D on (D.BearUserId=U.PKID or D.RegionManagerUserId=U.PKID)
                                            and D.PKID={dealerId}");
                list = conn.Query<Common.SysUserEntity>(sql).ToList();
                return list;
            }
        }
        /// <summary>
        /// 添加订单流程表
        /// </summary>
        /// <param name="model"></param>
        private static void Add(Common.Entity.OrderProcess model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Insert into OrderProcess(OrderId,OperationName,Operator,OperationDate,NextOperator) Values");
            strSql.Append("(@OrderId,@OperationName,@Operator,@OperationDate,@NextOperator)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键

            SqlParameter[] param = {
                new SqlParameter("@OrderId",model.OrderId),
                new SqlParameter("@OperationName",model.OperationName),
                new SqlParameter("@Operator",model.Operator),
                new SqlParameter("@OperationDate",model.OperationDate),
                new SqlParameter("@NextOperator",SqlNull(model.NextOperator)),
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
        }

        public static string SqlNull(string str)
        {
            if (str == null || str == "")
            {
                return " ";
            }
            return str;
        }
        /// <summary>
        /// 获取大客户室配车人员
        /// </summary>
        /// <param name="orderId"></param>
        private static List<Common.SysUserEntity> GetDistributeUser(int orderId)
        {
            List<Common.SysUserEntity> list = new List<Common.SysUserEntity>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@"select distinct U.PKID,U.Name,
                                            U.NickName,U.Email,U.Phone from SysUser U
                                            inner join UserRole UR on UR.UserID=U.PKID 
                                            and U.UserTypeId=6 and UR.RoleID in(select distinct R.RoleID from CarPurchase O
                                            inner join CarInfo C on O.CarID=C.PKID
                                            inner join RoleBrand R on C.Name=R.CarName
                                            and O.OrderID=1)
                                            inner join RoleModule RM on RM.RoleID=UR.RoleID
                                            and RM.ModuleID={orderId}");
                list = conn.Query<Common.SysUserEntity>(sql).ToList();
                return list;
            }
        }

        /// <summary>
        /// 添加一条订单流程 这个add是重载函数，不是调用的这个方法
        /// </summary>
        /// <param name="operUser">操作人</param>
        /// <param name="operType">操作类型（利用enum流程名称枚举获取）</param>
        /// <param name="dealerID">经销店ID，只有在提交订单时需要，其他填写Guid.Empty</param>
        /// <param name="ordersID">订单ID</param>
        /// <param name="ordersID">是否直接提交给大客户室</param>
        public void Add(string operUser, string operType, int dealerID, int ordersID, bool directSubmit)
        {
            Common.Entity.OrderProcess e = new Common.Entity.OrderProcess();
            e.OperationName = operType;
            e.Operator = operUser;
            e.OperationDate = DateTime.Now;
            e.OrderId = ordersID;

            switch ((int)Enum.Parse(typeof(Common.Utilities.enum流程名称枚举), operType))
            {
                case (int)Common.Utilities.enum流程名称枚举.订单生成:
                    break;
                case (int)Common.Utilities.enum流程名称枚举.订单提交:
                    if (directSubmit)
                        e.NextOperator = userList(GetFTMSUser());
                    else
                        e.NextOperator = userList(GetDDDQUser(dealerID));
                    break;
                case (int)Common.Utilities.enum流程名称枚举.地担审批通过:
                    e.NextOperator = userList(GetFTMSUser());
                    break;
                case (int)Common.Utilities.enum流程名称枚举.大区经理审批通过:
                    e.NextOperator = userList(GetFTMSUser());
                    break;
                case (int)Common.Utilities.enum流程名称枚举.大客户室审批通过:
                    e.NextOperator = string.Format("配车：{0}； 返款：{1}", userList(GetDistributeUser(ordersID)), userList(GetRebatesUser(ordersID)));
                    break;
                case (int)Common.Utilities.enum流程名称枚举.订单交车:
                    e.NextOperator = string.Format("返款：{0}", userList(GetRebatesUser(ordersID)));
                    break;
                case (int)Common.Utilities.enum流程名称枚举.订单配车:
                    e.NextOperator = string.Format("返款：{0}", userList(GetRebatesUser(ordersID)));
                    break;
                case (int)Common.Utilities.enum流程名称枚举.订单返款:
                    e.NextOperator = string.Format("返款：{0}", userList(GetRebatesUser(ordersID)));
                    break;
                case (int)Common.Utilities.enum流程名称枚举.订单年终补款:
                    e.NextOperator = string.Format("返款：{0}", userList(GetRebatesUser(ordersID)));
                    break;
                default:
                    e.NextOperator = "";
                    break;
            }

            Add(e);
        }

        #region 查看订单进度流程
        /// <summary>
        /// 查看订单进度流程
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public List<Common.Entity.OrderProcess> GetOrderProcessByOrderID(int orderId)
        {
            List<Common.Entity.OrderProcess> list = new List<Common.Entity.OrderProcess>();
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                string sql = string.Format($@" select * from OrderProcess where OrderId={orderId} order by OperationDate");
                list = conn.Query<Common.Entity.OrderProcess>(sql).ToList();
                return list;
            }
        }
        #endregion
    }
}
