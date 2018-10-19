using DAL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
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
            return new DAL.CarInfo().GetNameList();
        }
        #endregion

        #region 得到车辆类型列表
        /// <summary>
        /// 得到车辆类型列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTypeList()
        {
            return new DAL.CarInfo().GetTypeList();
        }
        #endregion

        #region 得到车辆品牌列表
        /// <summary>
        /// 得到车辆品牌列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetBrandList()
        {
            return new DAL.CarInfo().GetBrandList();
        }
        #endregion

        #region 得到车辆标识列表
        /// <summary>
        /// 得到车辆标识列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCarLogoList()
        {
            return new DAL.CarInfo().GetCarLogoList();
        }
        #endregion

        #region 从服务器端获取车型管理分页列表
        /// <summary>
        /// 从服务器端获取车型管理分页列表(当按排序查询时)
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.CarInfo> GetCarsManagePager(int pageIndex, int pageSize, string sortName, string IsDesc, Common.Entity.CarInfo filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from CarInfo where PKID>0");

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    sqlCount.Append(" and Name=@Name");
                }
                if (!string.IsNullOrEmpty(filter.SFX))
                {
                    sqlCount.Append(" and C.SFX like '%" + filter.SFX + "%'");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("Name", filter.Name);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from ( select row_number() over (order by C.PKID) as rowNumber, C.*,D.ListName as BrandName,D1.ListName as CarLogoName from CarInfo C left join Dictionary D on C.BrandID=D.PKID 
                                left join Dictionary D1 on C.CarLogoID=D1.PKID where C.PKID>0");
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    strSql.Append(" and C.Name like '%" + filter.Name + "%'");
                }
                if (!string.IsNullOrEmpty(filter.SFX))
                {
                    strSql.Append(" and C.SFX like '%" + filter.SFX + "%'");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize order by t.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("Name", filter.Name);
                List<Common.Entity.CarInfo> list = conn.Query<Common.Entity.CarInfo>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 从服务器端获取车型管理分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.CarInfo> GetCarsManagePager(int pageIndex, int pageSize, Common.Entity.CarInfo filter, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from CarInfo where PKID>0");

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    sqlCount.Append(" and Name=@Name");
                }
                if (!string.IsNullOrEmpty(filter.SFX))
                {
                    sqlCount.Append(" and SFX like '%" + filter.SFX + "%'");
                }
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("Name", filter.Name);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from ( select row_number() over (order by C.PKID) as rowNumber, C.*,D.ListName as BrandName,D1.ListName as CarLogoName from CarInfo C left join Dictionary D on C.BrandID=D.PKID 
                                left join Dictionary D1 on C.CarLogoID=D1.PKID where C.PKID>0");
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    strSql.Append(" and C.Name like '%" + filter.Name + "%'");
                }
                if (!string.IsNullOrEmpty(filter.SFX))
                {
                    strSql.Append(" and C.SFX like '%" + filter.SFX + "%'");
                }
                strSql.Append($@"  ) as t where t.rowNumber>(@pageIndex-1)*@pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("Name", filter.Name);
                List<Common.Entity.CarInfo> list = conn.Query<Common.Entity.CarInfo>(strSql.ToString(), param2).ToList();
                return list;
            };
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
            return new DAL.CarInfo().Exists(strCarName, strSFX);
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
            return new DAL.CarInfo().Add(model);
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
            return new DAL.CarInfo().Update(model);
        }
        #endregion

        #region 删除一条记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="PKID">PKID</param>
        /// <returns>返回值</returns>
        public int DeleteData(int PKID)
        {
            return new DAL.CarInfo().DeleteData(PKID);
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
            return new DAL.CarInfo().GetCarInfoByPKID(PKID);
        }
        #endregion

        #region 根据车型ID查询车辆颜色信息
        /// <summary>
        /// 根据车型ID查询车辆颜色信息
        /// </summary>
        /// <param name="carId">车型ID</param>
        /// <returns>Common.Entity.CarColor</returns>
        public List<Common.Entity.CarColor> GetCarColorInfo(int carId)
        {
            return new DAL.CarInfo().GetCarColorInfo(carId);
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
            return new DAL.CarInfo().EastExists(carId, code);
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
            return new DAL.CarInfo().EastAdd(model);
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
            return new DAL.CarInfo().EastUpdate(model);
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
            return new DAL.CarInfo().EastDeleteData(PKID);
        }
        #endregion

        #region 获取颜色实体对象
        /// <summary>
        /// 获取颜色实体对象
        /// </summary>
        /// <param name="pkid">pkid</param>
        /// <returns>返回实体</returns>
        public Common.Entity.CarColor GetCarColorByPKID(int pkid)
        {
            return new DAL.CarInfo().GetCarColorByPKID(pkid);
        }
        #endregion
    }
}
