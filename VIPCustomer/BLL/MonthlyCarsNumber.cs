using DAL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
/// <summary>
/// =========================业务逻辑层===============================
/// 日期：2018年3月13日16:42:56
/// 作者：赵鉴
/// 描述：大客户每月车辆对应数表操作
/// </summary>
namespace BLL
{
    public class MonthlyCarsNumber
    {
        /// <summary>
        /// 获取大客户每月车辆对应数分页列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Common.Entity.MonthlyCarsNumber> GetMonthlyCarsNumberPagerList(int pageIndex, int pageSize, Common.Entity.MonthlyCarsNumber search, out int totalCount)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    StringBuilder sqlCount = new StringBuilder();
                    sqlCount.Append(@"select count(1) from (select row_number() over(order by MC.PKID) as rowNumbers,MC.*,CI.Name,CI.SFX,CC.Code from MonthlyCarsNumber MC
                              left join CarInfo CI on MC.CarID=CI.PKID
                              left join CarColor CC on MC.CarColorID=CC.PKID where 1=1");
                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(search.Name))
                    {
                        sqlCount.Append(" and CI.Name=@Name");
                    }
                    if (search.Year > 0)
                    {
                        sqlCount.Append(" and MC.[Year]=@Year");
                    }
                    if (search.Month > 0)
                    {
                        sqlCount.Append(" and MC.[Month]=@Month");
                    }
                    sqlCount.Append(" ) as Temp");
                    param.Add("Name", search.Name);
                    param.Add("Year", search.Year);
                    param.Add("Month", search.Month);
                    totalCount = conn.Query<int>(sqlCount.ToString(), param).FirstOrDefault();// 总数

                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"select top (@pageSize) * from (select row_number() over(order by MC.PKID) as rowNumbers,MC.*,CI.Name,CI.SFX,CC.Code from MonthlyCarsNumber MC
                            left join CarInfo CI on MC.CarID=CI.PKID
                            left join CarColor CC on MC.CarColorID=CC.PKID where 1=1");
                    DynamicParameters param2 = new DynamicParameters();
                    if (!string.IsNullOrEmpty(search.Name))
                    {
                        strSql.Append(" and CI.Name=@Name");
                    }
                    if (search.Year > 0)
                    {
                        strSql.Append(" and MC.[Year]=@Year");
                    }
                    if (search.Month > 0)
                    {
                        strSql.Append(" and MC.[Month]=@Month");
                    }
                    strSql.Append(@") as T where T.rowNumbers>(@pageIndex-1)*@pageSize ");
                    param2.Add("pageIndex", pageIndex);
                    param2.Add("pageSize", pageSize);
                    param2.Add("Name", search.Name);
                    param2.Add("Year", search.Year);
                    param2.Add("Month", search.Month);
                    List<Common.Entity.MonthlyCarsNumber> list = new List<Common.Entity.MonthlyCarsNumber>();
                    list = conn.Query<Common.Entity.MonthlyCarsNumber>(strSql.ToString(), param2).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("程序在执行过程中发生如下错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists(Common.Entity.MonthlyCarsNumber model)
        {
            return new DAL.MonthlyCarsNumber().Exists(model);
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(Common.Entity.MonthlyCarsNumber model)
        {
            return new DAL.MonthlyCarsNumber().Add(model);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(Common.Entity.MonthlyCarsNumber entity)
        {
            return new DAL.MonthlyCarsNumber().Update(entity);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool Delete(int Id)
        {
            return new DAL.MonthlyCarsNumber().Delete(Id);
        }

        /// <summary>
        /// 查询车辆名称
        /// </summary>
        /// <returns></returns>
        public DataTable GetCarName()
        {
            return new DAL.MonthlyCarsNumber().GetCarName();
        }

        /// <summary>
        /// 获取某月的车辆资源信息
        /// </summary>
        /// <param name="carNames"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable GetCarList(string carNames, int year, int month)
        {
            if (carNames.IndexOf("'") < 0)
                carNames = string.Format("'{0}'", carNames.Replace(",", "','"));
            return new DAL.MonthlyCarsNumber().GetCarList(carNames, year, month);
        }
    }
}
