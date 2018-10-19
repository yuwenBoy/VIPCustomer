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
    public class CarInfo
    {
        #region 得到车辆名称列表
        /// <summary>
        /// 得到车辆名称列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetNameList()
        {
            string sql = string.Format(@"select CarName from BrandDictionary order by CarName");
            try
            {
                return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 得到车辆类型列表
        /// <summary>
        /// 得到车辆类型列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTypeList()
        {
            string sql = string.Format(@"select PKID as TypeID,ListName as TypeName from Dictionary where Code='CARSTYPE' order by PKID");
            try
            {
                return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 得到车辆类型列表
        /// <summary>
        /// 得到车辆类型列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetBrandList()
        {
            string sql = string.Format(@"select PKID as BrandID,ListName as BrandName from Dictionary where Code='CARBRAND' order by PKID");
            try
            {
                return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 得到车辆标识列表
        /// <summary>
        /// 得到车辆标识列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCarLogoList()
        {
            string sql = string.Format(@"select PKID as CarLogoID,ListName as CarLogoName from Dictionary where Code='CARSTATUS' order by PKID ");
            try
            {
                return SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否存在该记录
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="strCarName">车名</param>
        /// <param name="strSFX">车名缩写</param>
        /// <returns>返回值</returns>
        public bool Exists(string strCarName, string strSFX)
        {
            string sql = string.Format(@" select count(1) from CarInfo where Name=@Name and SFX=@SFX");
            SqlParameter[] param = {
                new SqlParameter("@Name",strCarName),
                new SqlParameter("@SFX",strSFX)
            };
            int count = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, sql, param));
            if (count > 0)
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        #region 新增一条记录
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns>返回值</returns>
        public int Add(Common.Entity.CarInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Insert into CarInfo(Name,CarModel,GearboxVersion,
                            Spec,Model,SFX,Code,TypeID,BuiltInColor,Drive,
                           SuggestPrice,FactoryPrice,InternetPrice,Remark,BrandID,Other,CarLogoID) 
                           Values (@Name,@CarModel,@GearboxVersion,@Spec,@Model,@SFX,@Code,
                           @TypeID,@BuiltInColor,@Drive,@SuggestPrice,@FactoryPrice,@InternetPrice,
                           @Remark,@BrandID,@Other,@CarLogoID)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@CarModel",model.CarModel),
                new SqlParameter("@GearboxVersion",model.GearboxVersion),
                new SqlParameter("@Spec",model.Spec),
                new SqlParameter("@Model",model.Model),
                new SqlParameter("@SFX",model.SFX),
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@TypeID",model.TypeID),
                new SqlParameter("@BuiltInColor",model.BuiltInColor),
                new SqlParameter("@Drive",model.Drive),
                new SqlParameter("@SuggestPrice",model.SuggestPrice),
                new SqlParameter("@FactoryPrice",model.FactoryPrice),
                new SqlParameter("@InternetPrice",model.InternetPrice),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@BrandID",model.BrandID),
                new SqlParameter("@Other",model.Other),
                new SqlParameter("@CarLogoID",model.CarLogoID),
            };
            try
            {
                return SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns>返回值</returns>
        public int Update(Common.Entity.CarInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update CarInfo set Name=@Name,CarModel=@CarModel,GearboxVersion=@GearboxVersion,Spec=@Spec,Model=@Model,
                          SFX=@SFX,Code=@Code,TypeID=@TypeID,BuiltInColor=@BuiltInColor,
                          Drive=@Drive,SuggestPrice=@SuggestPrice,FactoryPrice=@FactoryPrice,InternetPrice=@InternetPrice, 
                          Remark=@Remark,BrandID=@BrandID,Other=@Other,CarLogoID=@CarLogoID where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@CarModel",model.CarModel),
                new SqlParameter("@GearboxVersion",model.GearboxVersion),
                new SqlParameter("@Spec",model.Spec),
                new SqlParameter("@Model",model.Model),
                new SqlParameter("@SFX",model.SFX),
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@TypeID",model.TypeID),
                new SqlParameter("@BuiltInColor",model.BuiltInColor),
                new SqlParameter("@Drive",model.Drive),
                new SqlParameter("@SuggestPrice",model.SuggestPrice),
                new SqlParameter("@FactoryPrice",model.FactoryPrice),
                new SqlParameter("@InternetPrice",model.InternetPrice),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@BrandID",model.BrandID),
                new SqlParameter("@Other",model.Other),
                new SqlParameter("@CarLogoID",model.CarLogoID),
                new SqlParameter("@PKID",model.PKID)
            };
            try
            {
                return Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="int">PKID</param>
        /// <returns>返回值</returns>
        public int DeleteData(int PKID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" delete from CarInfo where PKID=@PKID");
            SqlParameter[] param = {
               new SqlParameter("@PKID",PKID)
            };
            try
            {
                return Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="PKID">PKID</param>
        /// <returns>返回值</returns>
        public Common.Entity.CarInfo GetCarInfoByPKID(int PKID)
        {
            string sql = string.Format(@"select * from CarInfo where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            try
            {
                Common.Entity.CarInfo model = new Common.Entity.CarInfo();
                DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
                if (dt.Rows.Count > 0)
                {
                    model.PKID = Convert.ToInt32(dt.Rows[0]["PKID"]);
                    model.Name = dt.Rows[0]["Name"].ToString();
                    model.CarModel = dt.Rows[0]["CarModel"].ToString();
                    model.GearboxVersion = dt.Rows[0]["GearboxVersion"].ToString();
                    model.Spec = dt.Rows[0]["Spec"].ToString();
                    model.Model = dt.Rows[0]["Model"].ToString();
                    model.Other = dt.Rows[0]["Other"].ToString();
                    model.SFX = dt.Rows[0]["SFX"].ToString();
                    model.SuggestPrice = double.Parse(dt.Rows[0]["SuggestPrice"].ToString());
                    model.CarLogoID = Convert.ToInt32(dt.Rows[0]["CarLogoID"].ToString());
                    model.BrandID = Convert.ToInt32(dt.Rows[0]["BrandID"].ToString());
                    model.TypeID = Convert.ToInt32(dt.Rows[0]["TypeID"]);
                    model.Remark = dt.Rows[0]["Remark"].ToString();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据车型ID查询车辆颜色信息
        /// <summary>
        /// 根据车型ID查询车辆颜色信息
        /// </summary>
        /// <param name="carId">车型ID</param>
        /// <returns>返回Datatable</returns>
        public List<Common.Entity.CarColor> GetCarColorInfo(int carId)
        {
            try
            {
                List<Common.Entity.CarColor> list = new List<Common.Entity.CarColor>();
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format(@" select * from CarColor where CarID=@CarID order by PKID");
                    DynamicParameters param = new DynamicParameters();
                    param.Add("CarID", carId);
                    list = conn.Query<Common.Entity.CarColor>(sql, param).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否存在车辆颜色编码
        /// <summary>
        /// 是否存在车辆颜色编码
        /// </summary>
        /// <param name="code">颜色编码</param>
        /// <returns>返回值</returns>
        public bool EastExists(int carId, string code)
        {
            string sql = string.Format(@"select count(1) from CarColor where CarID=@CarID and Code=@Code");
            SqlParameter[] param = {
                new SqlParameter("@CarID",carId),
                new SqlParameter("@Code",code)
            };
            int count = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, sql, param));
            if (count > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region 新增一条记录
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns>返回值</returns>
        public bool EastAdd(Common.Entity.CarColor model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Insert into CarColor(CarID,Code,Name,Remark) 
                           Values (@CarID,@Code,@Name,@Remark)");
            strSql.Append(";SELECT @@IDENTITY");// 返回插入用户的主键
            SqlParameter[] param = {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@CarID",model.CarID),
                new SqlParameter("@Remark",model.Remark),
            };
            try
            {
                int result = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 修改一条记录
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns>返回值</returns>
        public int EastUpdate(Common.Entity.CarColor model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"Update CarColor set Name=@Name,Code=@Code,Remark=@Remark where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@Code",model.Code),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@PKID",model.PKID)
            };
            try
            {
                return Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="int">PKID</param>
        /// <returns>返回值</returns>
        public int EastDeleteData(int PKID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" delete from CarColor where PKID=@PKID");
            SqlParameter[] param = {
               new SqlParameter("@PKID",PKID)
            };
            try
            {
                return Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), param));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取实体对象
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="PKID">PKID</param>
        /// <returns>返回值</returns>
        public Common.Entity.CarColor GetCarColorByPKID(int PKID)
        {
            string sql = string.Format(@"select * from CarColor where PKID=@PKID");
            SqlParameter[] param = {
                new SqlParameter("@PKID",PKID)
            };
            try
            {
                Common.Entity.CarColor model = new Common.Entity.CarColor();
                DataTable dt = SqlHelper.GetDateTable(SqlHelper.connStr, CommandType.Text, sql, param);
                if (dt.Rows.Count > 0)
                {
                    model.PKID = Convert.ToInt32(dt.Rows[0]["PKID"]);
                    model.Name = dt.Rows[0]["Name"].ToString();
                    model.Code = dt.Rows[0]["Code"].ToString();
                    model.CarID = Convert.ToInt32(dt.Rows[0]["CarID"].ToString());
                    model.Remark = dt.Rows[0]["Remark"].ToString();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
