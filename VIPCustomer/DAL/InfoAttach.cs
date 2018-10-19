using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class InfoAttach
    {
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="attachID"></param>
        /// <returns></returns>
        public Common.Entity.InfoAttach GetModel(string attachID)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                Common.Entity.InfoAttach entity = new Common.Entity.InfoAttach();
                string sql = string.Format(@"select * from InfoAttach where PKID=@attachID");
                DynamicParameters param = new DynamicParameters();
                param.Add("attachID", attachID);
                entity = conn.Query<Common.Entity.InfoAttach>(sql, param).FirstOrDefault();
                return entity;
            }
        }

        public int DeleteByID(String id)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                int Result = 0;
                string sql = string.Format("delete   from InfoAttach where PKID in(@attachID)");
                DynamicParameters param = new DynamicParameters();
                param.Add("attachID", id);
                Result = conn.Execute(sql, param);
                if (Result > 0)
                    return Result;
                else
                    return 0;
            }
        }

        public int Add(Common.Entity.InfoAttach model)
        {
            using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
            {
                int Result = 0;
                string sql = string.Format(@"insert into InfoAttach(InfoID,AttachType,AttachName,AttachSize,AttachPath,CreateTime)
                                             values(@InfoID, @AttachType, @AttachName, @AttachSize, @AttachPath, @CreateTime)");
                DynamicParameters param = new DynamicParameters();
                param.Add("@InfoID",model.InfoID);
                param.Add("@AttachType", model.AttachType);
                param.Add("@AttachName", model.AttachName);
                param.Add("@AttachSize", model.AttachSize);
                param.Add("@AttachPath", model.AttachPath);
                param.Add("@CreateTime", model.CreateTime);
                Result = conn.Execute(sql, param);
                if (Result > 0)
                    return Result;
                else
                    return 0;
            }
        }
    }
}
