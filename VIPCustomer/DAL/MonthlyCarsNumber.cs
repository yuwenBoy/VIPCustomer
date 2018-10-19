using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
/// <summary>
///  ========================数据访问层==================
///  日期：2018年3月14日10:49:43
///  作者：赵鉴
///  描述：大客户每月车辆对应数表操作
/// </summary>
namespace DAL
{
    public class MonthlyCarsNumber
    {
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists(Common.Entity.MonthlyCarsNumber model)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format(@" select count(1) from MonthlyCarsNumber where CarID=@CarID and CarColorID=@CarColorID and [Year]=@Year and [Month]=@Month");
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("CarID", model.CarID);
                    Parameters.Add("CarColorID", model.CarColorID);
                    Parameters.Add("Year", model.Year);
                    Parameters.Add("Month", model.Month);
                    bool result = conn.Query<bool>(sql, Parameters).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("程序在执行过程中发生如下错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(Common.Entity.MonthlyCarsNumber model)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format(@"Insert into MonthlyCarsNumber(CarID,CarColorID,Year,Month,Count,ResidualQuantity) 
                                                 values(@CarID,@CarColorID,@Year,@Month,@Count,@ResidualQuantity)");
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("CarID", model.CarID);
                    Parameters.Add("CarColorID", model.CarColorID);
                    Parameters.Add("Year", model.Year);
                    Parameters.Add("Month", model.Month);
                    Parameters.Add("Count", model.Count);
                    Parameters.Add("ResidualQuantity", 1);
                    bool result = Convert.ToBoolean(conn.Execute(sql, Parameters));
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("程序在执行过程中发生如下错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(Common.Entity.MonthlyCarsNumber model)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format(@"Update MonthlyCarsNumber set CarID=@CarID,CarColorID=@CarColorID,Year=@Year,Month=@Month,Count=@Count,
                                                 ResidualQuantity=@ResidualQuantity where PKID=@PKID");
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("CarID", model.CarID);
                    Parameters.Add("CarColorID", model.CarColorID);
                    Parameters.Add("Year", model.Year);
                    Parameters.Add("Month", model.Month);
                    Parameters.Add("Count", model.Count);
                    Parameters.Add("ResidualQuantity", 1);
                    Parameters.Add("PKID", model.PKID);
                    bool result = Convert.ToBoolean(conn.Execute(sql, Parameters));
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("程序在执行过程中发生如下错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool Delete(int Id)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format(@"delete  from MonthlyCarsNumber where PKID=@PKID");
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("PKID", Id);
                    bool result = Convert.ToBoolean(conn.Execute(sql, Parameters));
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("程序在执行过程中发生如下错误：" + ex.Message);
            }
        }

        #region 填充车辆名称列表
        /// <summary>
        /// 填充车辆名称列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCarName()
        {
            string sql = string.Format(@" select  CarName from BrandDictionary order by CarName");
            try
            {
                return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception("程序在执行过程中发生如下错误：" + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 获取某月的车辆资源信息
        /// </summary>
        /// <param name="carNames"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable GetCarList(string carNames, int year, int month)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"select CI.PKID 车辆ID,CI.PKID 车辆颜色ID,@Year 年,@Month 月,
                                        CI.Name 车辆名称,CI.SFX 车型缩写,CC.Code 颜色编码,0 数量 from CarInfo CI
                                        inner join Dictionary DT on CI.CarLogoID=DT.PKID
                                        inner join  CarColor CC on CI.PKID=CC.CarID
                                        where CI.Name in ({carNames}) and DT.ListName like '%配车%' order by CI.Name, CI.SFX, CC.Code");
            strSql.Append("");
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Year",year),
                new SqlParameter("@Month",month),
            };
            try
            {
                return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            }
            catch (Exception ex)
            {
                throw new Exception("程序在执行过程中发生如下错误：" + ex.Message);
            }
        }
    }
}
