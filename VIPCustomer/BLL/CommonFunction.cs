using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    public class CommonFunction
    {
        /// <summary>
        /// 获取当前日期时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDateTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 获取当前时间的年首日和年最后一天
        /// </summary>
        /// <param name="yearBegin"></param>
        /// <param name="yearEnd"></param>
        public static void GetYearDateTime(out DateTime yearBegin, out DateTime yearEnd)
        {
            yearBegin = new DateTime(DateTime.Now.Year, 1, 1);
            yearEnd = new DateTime(yearBegin.Year, 12, 31);
        }

        public static void GetYearDateTime(int year, out DateTime yearBegin, out DateTime yearEnd)
        {
            yearBegin = new DateTime(year, 1, 1);
            yearEnd = new DateTime(year, 12, 31);
        }
        /// <summary>
        /// 获取档期日期字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDateString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static bool CheckOrderAttach(int OrderID, string configName)
        {
            BLL.Order order = new BLL.Order();
            Common.Entity.Order oInfo = order.GetOrderByPKID(OrderID);
            IList<string> orderCars = order.GetCarsOrder(OrderID);
            JArray requireJObj = JArray.Parse(ConfigurationManager.AppSettings[configName]);
            IList<string> filesRequire = _getMatchingAttach(oInfo, orderCars, requireJObj);
            String uploadTempPath = ConfigurationManager.AppSettings["uploadPath"];
            string tempPath = oInfo.Code.Split(new char[] { '-' })[1];
            uploadTempPath = uploadTempPath + "\\OrdersPic\\" + tempPath.Substring(0, 4) + "\\" + tempPath + "\\";
            foreach (string s in filesRequire)
            {
                if ("9" == s)
                {
                    //9,B,C,D,E都为合同
                    if (!(File.Exists(string.Format("{0}{1}fufAttach9.jpg", uploadTempPath, oInfo.Code)) || File.Exists(string.Format("{0}{1}fufAttachB.jpg", uploadTempPath, oInfo.Code)) || File.Exists(string.Format("{0}{1}fufAttachC.jpg", uploadTempPath, oInfo.Code)) || File.Exists(string.Format("{0}{1}fufAttachD.jpg", uploadTempPath, oInfo.Code)) || File.Exists(string.Format("{0}{1}fufAttachE.jpg", uploadTempPath, oInfo.Code))))
                        return false;
                }
                else
                {
                    if (!File.Exists(string.Format("{0}{1}fufAttach{2}.jpg", uploadTempPath, oInfo.Code, s)))
                        return false;
                }
            }

            return true;
        }

        private static IList<string> _getMatchingAttach(Common.Entity.Order order, IList<string> orderCars, JArray jaCondation)
        {
            IList<string> filesRequire = new List<string>();
            foreach (JObject jo in jaCondation)
            {
                bool matchCondation = true;
                foreach (JProperty jp in jo.Properties())
                {
                    string jpValue = jp.Value.Value<string>();
                    switch (jp.Name)
                    {
                        case "Cartype":
                            if (jpValue.IndexOf('!') == 0)
                                matchCondation &= !orderCars.Contains(jpValue.Substring(1));
                            else
                                matchCondation &= orderCars.Contains(jpValue);
                            break;
                        case "Require":
                            break;
                        default:
                            var orderValue = GetPropValue(order, jp.Name);
                            if (null != orderValue)
                            {
                                if (jpValue.IndexOf('!') == 0)
                                    matchCondation &= (orderValue.ToString() != jpValue.Substring(1));
                                else
                                    matchCondation &= (orderValue.ToString() == jpValue);
                            }
                            else
                            {
                                matchCondation = false;
                            }
                            break;
                    }
                    if (!matchCondation)
                        break;
                }

                if (matchCondation)
                    filesRequire = filesRequire.Union(jo["Require"].Value<string>().Split(new char[] { ',' })).ToList<string>();
            }
            return filesRequire;
        }
    
        /// <summary>
        /// 获取实体类里面名称值
        /// </summary>
        public static object GetPropValue<T>(T t, string propName)
        {
            System.Reflection.PropertyInfo propertie = t.GetType().GetProperty(propName);
            if (null == propertie)
                return null;
            else
                return propertie.GetValue(t, null);
        }
        /// <summary>
        /// 分页返回数据结果
        /// </summary>
        public class DataResult<T>
        {
            /// <summary>
            /// 总记录数 
            /// </summary>
            public int totalCount { get; set; }

            /// <summary>
            /// 总页数
            /// </summary>
            public int totalpages { get; set; }

            /// <summary>
            /// 页码
            /// </summary>
            public int currPage { get; set; }

            /// <summary>
            /// 数据
            /// </summary>
            public List<T> dataList { get; set; }
        }

        /// <summary>
        /// 根据每页显示数与总记录数计算出总页数
        /// </summary>
        /// <param name="rows">每页显示数</param>
        /// <param name="totalRecord">结果总记录数</param>
        /// <returns></returns>
        public static int CalculateTotalPage(int rows, int totalRecord)
        {
            return Convert.ToInt32(Math.Ceiling((double)totalRecord / (double)rows));
        }

        /// <summary>
        /// 输出到客户端
        /// </summary>
        /// <param name="hr"></param>
        /// <param name="fileContent"></param>
        /// <param name="clientFilename"></param>
        public static void ResponseToClient(HttpResponse hr, string fileContent, string clientFilename)
        {
            clientFilename = HttpUtility.UrlEncode(clientFilename);
            hr.Clear();
            hr.Charset = "gb2312";
            hr.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", clientFilename));

            if (hr.IsClientConnected)
            {
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(fileContent);
                    // 文件总大小
                    hr.AddHeader("Content-Length", buffer.Length.ToString());
                    hr.ContentType = "application/vnd.ms-excel";
                    hr.OutputStream.Write(buffer, 0, buffer.Length);
                    hr.Flush();
                }
                catch (Exception ex)
                {
                    hr.Write("Error : " + ex.Message);
                }
            }
            hr.End();
        }
    }
}
