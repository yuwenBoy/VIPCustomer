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
    public class DICDomain
    {
        #region 获取数据信息分页列表
        /// <summary>
        /// 获取数据信息分页列表
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="sortName">排序字段</param>
        /// <param name="IsDesc">排序规则</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.DICDomain> PagerTypeList(int pageIndex, int pageSize, string sortName, string IsDesc, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from(SELECT distinct Code,Name from Dictionary) T");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from (select c.Code,c.Name ,row_number() over (order by c.Name) as rowNumber 
                                 from (select  distinct Code,Name from Dictionary) as c) T where T.rowNumber>(@pageIndex-1)*@pageSize order by T.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<Common.Entity.DICDomain> list = conn.Query<Common.Entity.DICDomain>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 获取数据信息分页列表
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.DICDomain> PagerTypeList(int pageIndex, int pageSize, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from(SELECT distinct Code,Name from Dictionary) T");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from (select c.Code,c.Name ,row_number() over (order by c.Name) as rowNumber 
                                 from (select  distinct Code,Name from Dictionary) as c) T where T.rowNumber>(@pageIndex-1)*@pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                List<Common.Entity.DICDomain> list = conn.Query<Common.Entity.DICDomain>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        #region 根据数据分类预编码获取数据分页
        /// <summary>
        /// 根据数据分类预编码获取数据分页
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="code">预编码</param>
        /// <param name="sortName">排序字段</param>
        /// <param name="IsDesc">排序规则</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public List<Common.Entity.DICDomain> PagerList(int pageIndex, int pageSize, string code, string sortName, string IsDesc, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from  Dictionary where Code=@Code");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("Code", code);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from  (select row_number() over (order by Sort) as rowNumber,
                                t.* from (select * from Dictionary where Code=@Code) as t) as T 
                                where T.rowNumber>(@pageIndex-1)*@pageSize order by T.{sortName} {IsDesc}");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("Code", code);
                List<Common.Entity.DICDomain> list = conn.Query<Common.Entity.DICDomain>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        /// <summary>
        /// 根据数据分类预编码获取数据分页
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">页码大小</param>
        /// <param name="code">预编码</param>
        /// <param name="totalCount">总记录数</param>
        public List<Common.Entity.DICDomain> PagerList(int pageIndex, int pageSize, string code, out int totalCount)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                StringBuilder sqlCount = new StringBuilder();
                sqlCount.Append(@"select count(@pageSize) from  Dictionary where Code=@Code");
                DynamicParameters param1 = new DynamicParameters();
                param1.Add("pageSize", pageSize);
                param1.Add("Code", code);
                totalCount = conn.Query<int>(sqlCount.ToString(), param1).FirstOrDefault(); // 输出总记录数

                StringBuilder strSql = new StringBuilder();
                strSql.Append($@"select top (@pageSize) * from  (select row_number() over (order by Sort) as rowNumber,
                                t.* from (select * from Dictionary where Code=@Code) as t) as T 
                                where T.rowNumber>(@pageIndex-1)*@pageSize");
                DynamicParameters param2 = new DynamicParameters();
                param2.Add("pageSize", pageSize);
                param2.Add("pageIndex", pageIndex);
                param2.Add("Code", code);
                List<Common.Entity.DICDomain> list = conn.Query<Common.Entity.DICDomain>(strSql.ToString(), param2).ToList();
                return list;
            };
        }
        #endregion

        #region 是否存在该域
        /// <summary>
        /// 是否存在该域
        /// </summary>
        /// <param name="name">域名称</param>
        /// <param name="code">域编码</param>
        /// <returns></returns>
        public bool ExistsDomain(string name, string code)
        {
            return new DAL.DICDomain().ExistsDomain(name, code);
        }
        #endregion

        #region 新增一条记录
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Add(Common.Entity.DICDomain model)
        {
            return new DAL.DICDomain().Add(model);
        }
        #endregion

        #region 修改一条数据
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public bool Update(Common.Entity.DICDomain model)
        {
            return new DAL.DICDomain().Update(model);
        }
        #endregion

        #region 删除一条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="code">域编码</param>
        /// <returns></returns>
        public bool DeleteDicDomainByCode(string code)
        {
            return new DAL.DICDomain().DeleteDicDomainByCode(code);
        }
        #endregion

        #region 根据域编码查询实体数据
        /// <summary>
        /// 根据域编码查询实体数据
        /// </summary>
        /// <param name="code">域编码</param>
        /// <returns></returns>
        public Common.Entity.DICDomain GetDicDomainByCode(string code)
        {
            return new DAL.DICDomain().GetDicDomainByCode(code);
        }
        #endregion


        #region 是否存在该列表名称
        /// <summary>
        /// 是否存在该列表名称
        /// </summary>
        /// <param name="pkid">主键ID</param>
        /// <param name="code">域编码</param>
        /// <param name="listName">列表名称</param>
        /// <returns>返回值</returns>
        public bool ExistsDICDomainItem(string code, string listName)
        {
            return new DAL.DICDomain().ExistsDICDomainItem(code, listName);
        }
        #endregion

        #region 根据PKID获取实体对象
        /// <summary>
        /// 根据PKID获取实体对象
        /// </summary>
        /// <param name="pkid">PKID</param>
        /// <returns>返回值</returns>
        public Common.Entity.DICDomain GetDicDomainByPKID(int pkid)
        {
            return new DAL.DICDomain().GetDicDomainByPKID(pkid);
        }
        #endregion

        public bool DeleteItemsByPKID(int PKID)
        {
            return new DAL.DICDomain().DeleteItemsByPKID(PKID);
        }

        #region 根据PKID修改一条记录
        /// <summary>
        /// 根据PKID修改一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns>返回值</returns>
        public bool UpdateDomainItem(Common.Entity.DICDomain model)
        {
            return new DAL.DICDomain().UpdateDomainItem(model);
        }
        #endregion

        #region 获取购买方式下拉列表
        /// <summary>
        /// 获取购买方式下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetBuyWay()
        {
            return new DAL.DICDomain().GetBuyWay();

        }
        #endregion

        #region 获取采购类型下拉列表
        /// <summary>
        /// 获取采购类型下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetPurchaseType()
        {
            return new DAL.DICDomain().GetPurchaseType();

        }
        #endregion

        #region 获取记录状态下拉列表
        /// <summary>
        /// 获取采购类型下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetOrderState()
        {
            return new DAL.DICDomain().GetOrderState();
        }
        #endregion

        #region 获取记录名称下拉列表
        /// <summary>
        /// 获取记录名称下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetRecordName()
        {
            return new DAL.DICDomain().GetRecordName();
        }
        #endregion

        /// <summary>
        /// 获取订单的订购车辆记录数
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public int GetDemandRecordsCount(int orderID)
        {
            return new DAL.DICDomain().GetDemandRecordsCount(orderID);
        }

        #region 获取信息列表类型
        /// <summary>
        /// 获取信息列表类型
        /// </summary>
        /// <returns></returns>
        public List<Common.Entity.DICDomain> GetTypeListJson()
        {
            return new DAL.DICDomain().GetTypeListJson();
        }
        #endregion

        #region 获取信息列表紧急程度
        /// <summary>
        /// 获取信息列表紧急程度
        /// </summary>
        /// <returns></returns>
        public List<Common.Entity.DICDomain> GetEmergencyDegreeListJson()
        {
            return new DAL.DICDomain().GetEmergencyDegreeListJson();
        }
        #endregion
        
    }
}
