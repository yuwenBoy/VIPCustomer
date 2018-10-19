using BLL;
using DAL;
using Dapper;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VIPCustomer.IServer.Cars
{
    /// <summary>
    /// Cars 的摘要说明
    /// </summary>
    public class CarsManage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "GetMonthlyCarsNumberPagerList":
                    GetMonthlyCarsNumberPagerList(context);
                    break;

                case "cboCarName":
                    cboCarName(context);
                    break;

                case "SaveData":
                    SaveData(context);
                    break;

                case "DeleteData":
                    DeleteData(context);
                    break;

                case "GetExcelData":
                    GetExcelData(context);
                    break;

                case "GetCarColorByCarId":
                    GetCarColorByCarId(context); break;

            }
        }

        /// <summary>
        /// 加载数据分页列表
        /// </summary>
        public void GetMonthlyCarsNumberPagerList(HttpContext context)
        {
            int totalCount = 0; // 总记录数
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页
            int pageSize = Convert.ToInt32(context.Request["rows"]); // 页码大小
            Common.Entity.MonthlyCarsNumber search = new Common.Entity.MonthlyCarsNumber();
            if (!string.IsNullOrEmpty(context.Request.Form["filterContext"]))
            {
                search = JsonConvert.DeserializeObject<Common.Entity.MonthlyCarsNumber>(context.Request.Form["filterContext"]);
            }
            var list = new BLL.MonthlyCarsNumber().GetMonthlyCarsNumberPagerList(pageIndex, pageSize, search, out totalCount);
            int totalPages = CommonFunction.CalculateTotalPage(pageSize, totalCount);
            var ResultData = new CommonFunction.DataResult<Common.Entity.MonthlyCarsNumber>()
            {
                totalCount = totalCount,
                totalpages = totalPages,
                currPage = pageIndex,
                dataList = list,
            };
            context.Response.Write(JsonConvert.SerializeObject(ResultData));
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="context"></param>
        public void SaveData(HttpContext context)
        {
            string name = context.Request["Name"];
            string sfx = context.Request["SFX"];
            int count = Convert.ToInt32(context.Request["Count"]);
            int year = Convert.ToInt32(context.Request["cboYear"]);
            int month = Convert.ToInt32(context.Request["cboMonth"]);
            string opType = context.Request["opType"];
            int carId = Convert.ToInt32(context.Request["hidCarID"]);
            int carColorId = Convert.ToInt32(context.Request["cboColor"]);
            Common.Entity.MonthlyCarsNumber model = new Common.Entity.MonthlyCarsNumber();
            model.CarID = carId;
            model.CarColorID = carColorId;
            model.Year = year;
            model.Month = month;
            model.Count = count;
            BLL.MonthlyCarsNumber bll = new BLL.MonthlyCarsNumber();
            if ("update".Equals(opType))
            {
                int id = Convert.ToInt32(context.Request["HidID"]);
                Common.Entity.MonthlyCarsNumber entity = new Common.Entity.MonthlyCarsNumber();
                entity.CarID = carId;
                entity.CarColorID = carColorId;
                entity.Year = year;
                entity.Month = month;
                entity.Count = count;
                entity.PKID = id;
                if (bll.Update(entity))
                {
                    context.Response.Write("{\"msg\":\"修改成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"修改失败。\",\"success\":false}");
                }
            }
            else
            {
                if (bll.Exists(model))
                {
                    context.Response.Write("{\"msg\":\"已经存在该客户的月份库存，请修改。\",\"state\":1}");
                }
                else if (bll.Add(model))
                { context.Response.Write("{\"msg\":\"添加成功。\",\"success\":true}"); }
                else
                {
                    context.Response.Write("{\"msg\":\"添加失败。\",\"success\":false}");
                }

            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="context"></param>
        public void DeleteData(HttpContext context)
        {
            int Id = Convert.ToInt32(context.Request["Id"]);
            if (Id > 0)
            {
                if (new BLL.MonthlyCarsNumber().Delete(Id))
                {
                    context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
                }
            }
        }

        /// <summary>
        /// 填充车辆名称下拉列表
        /// </summary>
        /// <param name="context"></param>
        public void cboCarName(HttpContext context)
        {
            DataTable dt = new BLL.MonthlyCarsNumber().GetCarName();
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }

        public void GetExcelData(HttpContext context)
        {
            string carNames = "";
            string carName = context.Request["mcboCarName"];
            DateTime yearMonth = DateTime.Now;

            if (!string.IsNullOrEmpty(context.Request["dfSubmitBeginDate"]))
            {
                DateTime.TryParse(context.Request["dfSubmitBeginDate"], out yearMonth);
            }
            else
            {
                return;
            }

            if (!string.IsNullOrEmpty(carName))
            {
                carNames = carName;
            }
            else return;

            using (DataTable dt = new BLL.MonthlyCarsNumber().GetCarList(carNames, yearMonth.Year, yearMonth.Month))
            {
                Hashtable styles = new Hashtable();
                styles.Add(2, new Common.Utilities.Style("0"));
                styles.Add(3, new Common.Utilities.Style("0"));
                dt.TableName = string.Format("{0}车辆资源", yearMonth.ToString("yyyy年MM月"));
                string result = Common.Utilities.DataTableToExcel.CreateExcelXML(dt, styles);
                CommonFunction.ResponseToClient(context.Response, result, string.Format("Export_{0}_{1}.{2}", dt.TableName, DateTime.Now.ToString("yyyyMMddHHmmss"), "xls"));
            }
        }

        #region 查询车辆颜色下拉列表
        /// <summary>
        /// 查询车辆颜色下拉列表
        /// </summary>
        /// <param name="context"></param>
        public void GetCarColorByCarId(HttpContext context)
        {
            int carId = Convert.ToInt32(context.Request["carId"]);
            if (carId > 0)
            {
                var resultJson = "";
                using (IDbConnection conn = new SqlConnection(SqlHelper.connStr))
                {
                    string sql = string.Format($@"select PKID,Code,Name from CarColor where CarID={carId}");
                    var list = conn.Query<Common.Entity.CarColor>(sql).ToList();
                    resultJson = JsonConvert.SerializeObject(list);
                };
                context.Response.Write(resultJson);
            }
        }
        #endregion
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}