using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace BLL
{
    public class DealerStock
    {
        #region 查询经销店库存上报分页列表
        /// <summary>
        /// 查询经销店库存上报分页列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userID"></param>
        /// <param name="search"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Common.Entity.DealerStock> GetDealerStockPagerList(int pageIndex, int pageSize, int DealerID, Common.Entity.DealerStock search, out int totalCount)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    StringBuilder sqlCount = new StringBuilder();
                    DynamicParameters param1 = new DynamicParameters();
                    sqlCount.Append(@"select count(1) from (select DS.* from DealerStock DS left join Dealer D on DS.DealerID=D.PKID
                                      left join Customer C on DS.CustomerID=C.PKID where DS.DealerID=@DealerID");
                    if (!string.IsNullOrWhiteSpace(search.EnterpriseCode))
                    {
                        sqlCount.Append(" and (C.Name like '%" + search.EnterpriseCode + "%' or C.EnterpriseCode like '%" + search.EnterpriseCode + "%' or DS.CarName like '%" + search.EnterpriseCode + "%')");
                    }
                    if (search.Year != "-1"&&search.Year!=null)
                    {
                        sqlCount.Append("  and year(StockMonth)=@Year");
                        param1.Add("Year", search.Year);
                    }
                    if (search.Month != "-1" && search.Month != null)
                    {
                        sqlCount.Append("  and month(StockMonth)=@Month");
                        param1.Add("Month", search.Month);
                    }
                    sqlCount.Append(@") as Temp");
                    param1.Add("DealerID", DealerID);
                    totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault();

                    StringBuilder strSql = new StringBuilder();
                    DynamicParameters param2 = new DynamicParameters();
                    strSql.Append(@"select top (@pageSize) * from (select row_number() over(order by DS.StockMonth) as rowNumbers,C.EnterpriseCode,C.Name,
                                            DS.* from DealerStock DS left join Dealer D on DS.DealerID=D.PKID
                                            left join Customer C on DS.CustomerID=C.PKID where DS.DealerID=@DealerID");
                    if (!string.IsNullOrWhiteSpace(search.EnterpriseCode))
                    {
                        strSql.Append(" and (C.Name like '%" + search.EnterpriseCode + "%' or C.EnterpriseCode like '%" + search.EnterpriseCode + "%' or DS.CarName like '%" + search.EnterpriseCode + "%')");
                    }
                    if (search.Year != "-1" && search.Year != null)
                    {
                        strSql.Append("  and year(StockMonth)=@Year");
                        param2.Add("Year", search.Year);
                    }
                    if (search.Month != "-1" && search.Month != null)
                    {
                        strSql.Append("  and month(StockMonth)=@Month");
                        param2.Add("Month", search.Month);
                    }
                    strSql.Append(" ) as T where T.rowNumbers>(@pageIndex-1)*@pageSize");
                    param2.Add("pageSize", pageSize);
                    param2.Add("pageIndex", pageIndex);
                    param2.Add("DealerID", DealerID);
                    List<Common.Entity.DealerStock> list = new List<Common.Entity.DealerStock>();
                    list = conn.Query<Common.Entity.DealerStock>(strSql.ToString(), param2).ToList();
                    return list;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 填充车辆名称列表
        /// <summary>
        /// 填充车辆名称列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCarName()
        {
            return new DAL.DealerStock().GetCarName();
        }
        #endregion

        public bool Exists(Common.Entity.DealerStock model)
        {
            return new DAL.DealerStock().Exists(model);
        }

        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public bool Add(Common.Entity.DealerStock stock)
        {
            return new DAL.DealerStock().Add(stock);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public int Update(Common.Entity.DealerStock stock)
        {
            return new DAL.DealerStock().Update(stock);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public int Delete(int stockId)
        {
            return new DAL.DealerStock().Delete(stockId);
        }
    }
}
