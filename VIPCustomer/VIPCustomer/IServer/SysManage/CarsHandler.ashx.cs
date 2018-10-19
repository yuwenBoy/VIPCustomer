using BLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace VIPCustomer.IServer.SysManage
{
    /// <summary>
    /// CarsHandler 的摘要说明
    /// </summary>
    public class CarsHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                #region 车型管理================================
                case "GetCarsManagePager":
                    GetCarsManagePager(context);
                    break;
                case "GetNameList":
                    GetNameList(context);
                    break;

                case "GetTypeList":
                    GetTypeList(context);
                    break;

                case "GetBrandList":
                    GetBrandList(context);
                    break;


                case "GetCarLogoList":
                    GetCarLogoList(context);
                    break;


                case "SaveData":
                    SaveData(context);
                    break;

                case "DeleteCarInfoByPKID":
                    DeleteCarInfoByPKID(context);
                    break;

                case "GetCarInfoByPKID":
                    GetCarInfoByPKID(context);
                    break;

                case "GetCarColorInfo":
                    GetCarColorInfo(context);
                    break;

                case "EastSaveData":
                    EastSaveData(context);
                    break;

                case "EastDeleteData":
                    EastDeleteData(context);
                    break;
                    #endregion
            }
        }

        #region 车型管理==============================

        #region 获取车辆信息分页列表
        /// <summary>
        /// 获取车辆信息分页列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCarsManagePager(HttpContext context)
        {
            int pageIndex = Convert.ToInt32(context.Request["page"]); // 当前页码
            int pageSize = Convert.ToInt32(context.Request["rows"]);  // 页码大小
            int totalCount = 0;
            var orderName = context.Request["sort"]; // 排序字段
            var orderBy = context.Request["sortOrder"];// 排序规则
            var sort = sortInfo.GetSortOrder(orderName, orderBy);
            List<Common.Entity.CarInfo> list = new List<Common.Entity.CarInfo>();
            string sfx = context.Request.Form["SFX"];
            string name = context.Request.Form["Name"];
            Common.Entity.CarInfo filter = new Common.Entity.CarInfo
            {
                SFX = sfx,
                Name = name,
            };
            if (sort.SortName != null && !string.IsNullOrEmpty(sort.IsDesc))
            {
                list = new BLL.CarInfo().GetCarsManagePager(pageIndex, pageSize, sort.SortName, sort.IsDesc, filter, out totalCount);
            }
            else
            {
                list = new BLL.CarInfo().GetCarsManagePager(pageIndex, pageSize, filter, out totalCount);
            }
            var models = new BLL.BoostrapTableInfo<Common.Entity.CarInfo>
            {
                total = totalCount,
                rows = list
            };
            string resultJson = JsonConvert.SerializeObject(models);
            context.Response.Write(resultJson);
        }
        #endregion

        #region 得到车辆名称列表
        /// <summary>
        /// 得到车辆名称列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetNameList(HttpContext context)
        {
            DataTable dt = new BLL.CarInfo().GetNameList();
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 得到车辆类型列表
        /// <summary>
        /// 得到车辆类型列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetTypeList(HttpContext context)
        {
            DataTable dt = new BLL.CarInfo().GetTypeList();
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 得到车辆品牌列表
        /// <summary>
        /// 得到车辆品牌列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetBrandList(HttpContext context)
        {
            DataTable dt = new BLL.CarInfo().GetBrandList();
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 得到车辆标识列表
        /// <summary>
        /// 得到车辆标识列表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCarLogoList(HttpContext context)
        {
            DataTable dt = new BLL.CarInfo().GetCarLogoList();
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        #endregion

        #region 获取实体对象
        public void GetCarInfoByPKID(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["keyword"]);
            if (PKID > 0)
            {
                Common.Entity.CarInfo model = new BLL.CarInfo().GetCarInfoByPKID(PKID);
                if (model != null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(model));
                }
            }
        }
        #endregion

        #region 保存
        public void SaveData(HttpContext context)
        {
            int HidID = Convert.ToInt32(context.Request["HidID"]);
            string fr_Name = context.Request["fr_Name"];
            string CarModel = context.Request["CarModel"];
            string GearboxVersion = context.Request["GearboxVersion"];
            string Spec = context.Request["Spec"];
            string Model = context.Request["Model"];
            string fr_SFX = context.Request["fr_SFX"];
            string Code = context.Request["Code"];
            int TypeID = Convert.ToInt32(context.Request["TypeID"]);
            int BrandID = Convert.ToInt32(context.Request["BrandID"]);
            string BuiltInColor = context.Request["BuiltInColor"];
            string Drive = context.Request["Drive"];
            double SuggestPrice = 0;
            double.TryParse(context.Request["SuggestPrice"], out SuggestPrice);
            int CarLogoID = Convert.ToInt32(context.Request["CarLogoID"]);
            string Remark = context.Request["Remark"];
            Common.Entity.CarInfo model = new Common.Entity.CarInfo();
            BLL.CarInfo bll = new BLL.CarInfo();

            if (HidID > 0)
            {
                model.PKID = HidID;
                model.Name = fr_Name;
                model.CarModel = CarModel;
                model.GearboxVersion = GearboxVersion;
                model.Spec = Spec;
                model.Model = Model;
                model.SFX = fr_SFX;
                model.Code = Code;
                model.TypeID = TypeID;
                model.BrandID = BrandID;
                model.BuiltInColor = BuiltInColor;
                model.Drive = Drive;
                model.SuggestPrice = SuggestPrice;
                model.CarLogoID = CarLogoID;
                model.Remark = Remark;
                int result = bll.Update(model);
                if (result > 0)
                {
                    context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                }
            }
            else
            {
                model.Name = fr_Name;
                model.CarModel = CarModel;
                model.GearboxVersion = GearboxVersion;
                model.Spec = Spec;
                model.Model = Model;
                model.SFX = fr_SFX;
                model.Code = Code;
                model.TypeID = TypeID;
                model.BrandID = BrandID;
                model.BuiltInColor = BuiltInColor;
                model.Drive = Drive;
                model.SuggestPrice = SuggestPrice;
                model.CarLogoID = CarLogoID;
                model.Remark = Remark;
                if (bll.Exists(model.Name, model.SFX))
                {
                    context.Response.Write("{\"state\":1, \"msg\":\"" + fr_Name + "," + fr_SFX + "\"}");
                }
                else
                {
                    int result = bll.Add(model);
                    if (result > 0)
                    {
                        context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                    }
                }
            }
        }
        #endregion

        #region 删除数据
        public void DeleteCarInfoByPKID(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["PKID"]);

            BLL.CarInfo bll = new BLL.CarInfo();
            int result = bll.DeleteData(PKID);
            if (result > 0)
            {
                context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
            }
            else
            {
                context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
            }
        }
        #endregion

        #region 根据车型ID查询车辆颜色表
        /// <summary>
        /// 根据车型ID查询车辆颜色表
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void GetCarColorInfo(HttpContext context)
        {
            int carId = Convert.ToInt32(context.Request["PKID"]);
            if (carId > 0)
            {
                List<Common.Entity.CarColor> list = new BLL.CarInfo().GetCarColorInfo(carId);
                context.Response.Write(JsonConvert.SerializeObject(list));
            }
        }
        #endregion

        #region 保存设定颜色信息
        public void EastSaveData(HttpContext context)
        {
            int carId = Convert.ToInt32(context.Request["hidCarId"]);
            int HidCarColorId = Convert.ToInt32(context.Request["HidCarColorId"]);
            string name = context.Request["colorName"];
            string code = context.Request["colorCode"];
            BLL.CarInfo bll = new BLL.CarInfo();
            Common.Entity.CarColor model = new Common.Entity.CarColor();
            if (carId > 0)
            {
                if (HidCarColorId > 0)
                {
                    model.PKID = HidCarColorId;
                    model.Code = code;
                    model.Name = name;
                    model.CarID = carId;
                    int result = bll.EastUpdate(model);
                    if (result > 0)
                    {
                        context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                    }
                }
                else
                {
                    model.CarID = carId;
                    model.Code = code;
                    model.Name = name;
                    if (bll.EastExists(carId, code))
                    {
                        context.Response.Write("{\"state\":1, \"msg\":\"" + code + "\"}");
                    }
                    else if (bll.EastAdd(model))
                    {
                        context.Response.Write("{\"msg\":\"保存成功。\",\"success\":true}");
                    }
                    else
                    {
                        context.Response.Write("{\"msg\":\"保存失败。\",\"success\":false}");
                    }
                }
            }
        }
        #endregion

        #region 删除颜色信息
        /// <summary>
        /// 删除颜色信息
        /// </summary>
        /// <param name="context">删除颜色信息</param>
        public void EastDeleteData(HttpContext context)
        {
            int PKID = Convert.ToInt32(context.Request["PKID"]);
            if (PKID > 0)
            {
                int result = new BLL.CarInfo().EastDeleteData(PKID);
                if (result > 0)
                {
                    context.Response.Write("{\"msg\":\"删除成功。\",\"success\":true}");
                }
                else
                {
                    context.Response.Write("{\"msg\":\"删除失败。\",\"success\":false}");
                }
            }
        }
        #endregion
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