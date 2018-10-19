using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using Common.Utilities;

namespace Common.Utilities
{
    public class DataTableToExcel
    {
        private const double singleCharWidth = 6.38;
        /// <summary>
        /// 生成ExcelXml内容
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="styleHash">特定的样式表，key为列索引0起始，value为Style样式, 自定义样式大小输入字符个数即可，自动转换为宋体11号字对应的大小</param>
        /// <returns></returns>
        public static string CreateExcelXML(DataTable dt, System.Collections.Hashtable styleHash)
        {
            //特殊处理Guid列，直接导出这些列有问题，需要添加{}
            IList<int> guidColumns = new List<int>();
            for (int i = dt.Columns.Count - 1; i > -1; i--)
            {
                if (dt.Columns[i].DataType.FullName == typeof(Guid).FullName)
                    guidColumns.Add(i);
            }
            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("zh-CHS");

            ExcelXML eXml = new ExcelXML();

            StyleCollection styleC = new StyleCollection();
            SheetCollection sheetC = new SheetCollection();
            Sheet sheet = new Sheet(dt.TableName);
            ColumnCollection cc = new ColumnCollection();
            sheet.ColumnList = cc;

            //自动样式，同时添加题头
            DataType[] dts = new DataType[dt.Columns.Count];
            Row r = new Row();

            //先处理自定样式，因为ID要重新命名
            if (null != styleHash)
            {
                object[] keys = new object[styleHash.Keys.Count];
                styleHash.Keys.CopyTo(keys, 0);

                for (int i = 0; i < keys.Length; i++)
                {
                    Style newS = styleC.Add((Style)styleHash[keys[i]]);
                    newS.Width *= singleCharWidth;
                    if (newS.Width < 70)
                    {
                        double colW = System.Text.Encoding.Default.GetByteCount(dt.Columns[(int)keys[i]].ColumnName) * singleCharWidth;
                        colW = (colW > 70) ? colW : 70;
                        newS.Width = colW;
                    }
                    //如果没有定义数据类型格式，自动获取
                    if (string.IsNullOrEmpty(newS.Format))
                        newS.Format = getStyle(dt.Columns[int.Parse(keys[i].ToString())].DataType).Format; ;

                    styleHash[keys[i]] = newS;
                }
            }

            //定义题头格式
            Style headTitleStyle = styleC.Add("s2", "@");
            headTitleStyle.Horizontal = HorizontalType.Center;
            headTitleStyle.Vertical = VerticalType.Center;

            IList<int> datetimeCols = new List<int>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Style aStyle = getStyle(dt.Columns[i].DataType);
                aStyle.Width = System.Text.Encoding.Default.GetByteCount(dt.Columns[i].ColumnName) * singleCharWidth;
                aStyle.Width = (aStyle.Width < 70) ? 70 : aStyle.Width;
                styleC.Add(aStyle);
                dts[i] = getDatatype(dt.Columns[i].DataType);
                //记录下日期格式的列
                if (DataType.DateTime == dts[i])
                    datetimeCols.Add(i);

                Cell c = r.Add(dt.Columns[i].ColumnName, DataType.String);
                c.StyleID = "s2";
                //如果有设置样式，那么指定样式，否则采用自动样式
                if ((null != styleHash) && (styleHash.ContainsKey(i)))
                {
                    Style s = (Style)styleHash[i];
                    cc.Add(s.ID, s.Width);
                }
                else
                    cc.Add(aStyle.ID, aStyle.Width);
            }
            sheet.RowList.Add(r);

            eXml.StyleList = styleC;

            sheet.IsShowTitle = true;

            //写数据
            foreach (DataRow dr in dt.Rows)
            {
                Row newRow = new Row();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    //日期格式列特殊处理
                    if (datetimeCols.Contains(i))
                    {
                        if (dr.IsNull(i))
                            newRow.Add("", dts[i], i + 1);
                        else
                            newRow.Add(DateTime.Parse(dr[i].ToString()).ToString("s"), dts[i], i + 1);
                    }
                    else if (guidColumns.Contains(i))
                        newRow.Add(string.Format("{{{0}}}", dr[i].ToString()), dts[i], i + 1);
                    else
                        newRow.Add(dr[i].ToString(), dts[i], i + 1);
                }

                sheet.RowList.Add(newRow);
            }

            sheetC.Add(sheet);

            eXml.SheetList = sheetC;

            return eXml.ToString();
        }
        /// <summary>
        /// 自动获取数据类型的样式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Style getStyle(Type type)
        {
            Style ns = new Style();

            switch (type.ToString())
            {
                case "System.DateTime":
                    {
                        ns.Format = "yyyy-MM-dd";
                        ns.Vertical = VerticalType.Center;
                        ns.Horizontal = HorizontalType.Right;
                        return ns;
                    }
                case "System.Byte":
                case "System.UInt16":
                case "System.Int16":
                case "System.UInt32":
                case "System.Int32":
                case "System.UInt64":
                case "System.Int64":
                    {
                        ns.Format = "#,##0";
                        ns.Vertical = VerticalType.Center;
                        ns.Horizontal = HorizontalType.Right;
                        return ns;
                    }
                case "System.Double":
                case "System.Single":
                case "System.Decimal":
                    {
                        ns.Format = "#,##0.00";
                        ns.Vertical = VerticalType.Center;
                        ns.Horizontal = HorizontalType.Right;
                        return ns;
                    }
                default:
                    {
                        ns.Format = "@";
                        ns.Vertical = VerticalType.Center;
                        ns.Horizontal = HorizontalType.Left;
                        return ns;
                    }
            }
        }

        /// <summary>
        /// 获取数据Excel类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static DataType getDatatype(Type type)
        {
            switch (type.ToString())
            {
                case "System.DateTime":
                    return DataType.DateTime;
                case "System.Byte":
                case "System.UInt16":
                case "System.Int16":
                case "System.UInt32":
                case "System.Int32":
                case "System.UInt64":
                case "System.Int64":
                case "System.Double":
                case "System.Single":
                case "System.Decimal":
                    return DataType.Number;
                default:
                    return DataType.String;
            }
        }
    }
}