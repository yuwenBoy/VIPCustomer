/*===================================================================
 * Write by Lyc
 * Version 1.0
=====================================================================*/

using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utilities
{
    /// <summary>
    /// 生成XML格式Excel类0000000000000000000
    /// </summary>
    public class ExcelXML
    {
        /// <summary>
        /// 表单列表
        /// </summary>
        public SheetCollection SheetList { get; set; }
        /// <summary>
        /// 样式列表
        /// </summary>
        public StyleCollection StyleList { get; set; }

        public new string ToString()
        {
            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("zh-CHS");

            StringBuilder excelSB = new StringBuilder();
            //添加固定信息
            excelSB.AppendLine("<?xml version=\"1.0\"?>");
            excelSB.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
            excelSB.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            excelSB.AppendLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            excelSB.AppendLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            excelSB.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            excelSB.AppendLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
            excelSB.AppendLine(" <DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
            excelSB.AppendLine("  <Author>Windows 用户</Author>");
            excelSB.AppendLine("  <LastAuthor>Windows 用户</LastAuthor>");
            excelSB.AppendFormat("  <Created>{0}</Created>\n", DateTime.Now.ToString("s"));
            excelSB.AppendFormat("  <LastSaved>{0}</LastSaved>\n", DateTime.Now.ToString("s"));
            excelSB.AppendLine("  <Version>11.00</Version>");
            excelSB.AppendLine(" </DocumentProperties>");
            ///标题样式
            Style headStyle = new Style("s1", "@");
            headStyle.Horizontal = HorizontalType.Center;
            headStyle.Vertical = VerticalType.Center;
            FontFormat ff = new FontFormat();
            ff.CharSet = "134";
            ff.FontName = "宋体";
            ff.Size = 16;
            headStyle.Font = ff;

            int index = StyleList.Count;
            StyleList.Add(headStyle);
            excelSB.Append(StyleList.ToString());
            //添加标题样式后删除，因为这个非用户控制
            StyleList.Delete(index);

            excelSB.Append(SheetList.ToString());
            excelSB.AppendLine("</Workbook>");

            return excelSB.ToString();
        }
    }

    /// <summary>
    /// 行对象
    /// </summary>
    public class Row : IEnumerable
    {
        private IList<Cell> _cellList = new List<Cell>();
        /// <summary>
        /// 列数
        /// </summary>
        public int Count { get { return _cellList.Count; } }
        /// <summary>
        /// 行高度
        /// </summary>
        public byte Height { get; set; }

        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public Cell Add(string value, DataType type)
        {
            Cell c = new Cell(value, type);
            _cellList.Add(c);

            return c;
        }
        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <param name="styleID">样式ID</param>
        /// <returns></returns>
        public Cell Add(string value, DataType type, string styleID)
        {
            Cell c = new Cell(value, type, styleID);
            _cellList.Add(c);

            return c;
        }
        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <param name="index">列索引(1开始)</param>
        /// <returns></returns>
        public Cell Add(string value, DataType type, int index)
        {
            Cell c = new Cell(value, type, index);
            _cellList.Add(c);

            return c;
        }
        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="c">Cell对象</param>
        /// <returns></returns>
        public Cell Add(Cell c)
        {
            _cellList.Add(c);
            return c;
        }
        /// <summary>
        /// 删除一个值
        /// </summary>
        /// <param name="index">索引</param>
        public void Delete(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new Exception("下标超出范围！");
            }
            _cellList.RemoveAt(index);
        }

        /// <summary>
        /// 遍历
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _cellList.GetEnumerator();
        }

        public new string ToString()
        {
            StringBuilder rowSB = new StringBuilder();
            rowSB.Append("<Row ss:AutoFitHeight=\"0\"");

            if (this.Height > 0)
                rowSB.AppendFormat(" ss:Height=\"{0}\"", this.Height.ToString());

            rowSB.AppendFormat(">\n");
            foreach (Cell c in _cellList)
                rowSB.Append(c.ToString());
            rowSB.AppendFormat("</Row>\n");

            return rowSB.ToString();
        }
    }

    /// <summary>
    /// 行集合
    /// </summary>
    public class RowCollection : IEnumerable
    {
        private IList<Row> _rowList = new List<Row>();

        /// <summary>
        /// 已使用行数
        /// </summary>
        public int Count { get { return _rowList.Count; } }

        public Row Add(IList<Cell> cells)
        {
            Row row = new Row();
            foreach (Cell c in cells)
            {
                row.Add(c);
            }
            return row;
        }

        public Row Add(Row row)
        {
            _rowList.Add(row);
            return row;
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="index">行号</param>
        public void Delete(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new Exception("下标超出范围！");
            }
            _rowList.RemoveAt(index);
        }

        /// <summary>
        /// 获取行
        /// </summary>
        /// <param name="index">行号</param>
        /// <returns></returns>
        public Row this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new Exception("下标超出范围！");
                }
                return _rowList[index];
            }
        }

        /// <summary>
        /// 遍历行
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _rowList.GetEnumerator();
        }

        public new string ToString()
        {
            StringBuilder rowsSB = new StringBuilder();
            foreach (Row r in _rowList)
                rowsSB.Append(r.ToString());
            return rowsSB.ToString();
        }
    }

    /// <summary>
    /// 表单对象
    /// </summary>
    public class Sheet
    {
        /// <summary>
        /// 列信息
        /// </summary>
        public ColumnCollection ColumnList { get; set; }
        /// <summary>
        /// 行值
        /// </summary>
        public RowCollection RowList { get; set; }
        /// <summary>
        /// 默认行宽
        /// </summary>
        public double DefaultColumnWidth { get; set; }

        /// <summary>
        /// 默认行高
        /// </summary>
        public double DefaultRowHeight { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否显示表单题头（同Name）
        /// </summary>
        public bool IsShowTitle { get; set; }

        /// <summary>
        /// 构造工作表
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        public Sheet(string sheetName)
        {
            this.Name = sheetName;
            ColumnList = new ColumnCollection();
            RowList = new RowCollection();
        }

        public new string ToString()
        {
            this.DefaultColumnWidth = (this.DefaultColumnWidth > 0) ? this.DefaultColumnWidth : 54;
            this.DefaultRowHeight = (this.DefaultRowHeight > 0) ? this.DefaultRowHeight : 13.5;
            StringBuilder sheetSB = new StringBuilder();
            int cCount = (this.RowList.Count > 0) ? this.RowList[0].Count : 0;
            sheetSB.AppendFormat("<Worksheet ss:Name=\"{0}\">\n", this.Name);
            sheetSB.AppendFormat("<Table ss:ExpandedColumnCount=\"{0}\" ss:ExpandedRowCount=\"{1}\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultColumnWidth=\"{2}\" ss:DefaultRowHeight=\"{3}\">\n",
                cCount.ToString(), (IsShowTitle) ? (this.RowList.Count + 1).ToString() : this.RowList.Count.ToString(),
                this.DefaultColumnWidth.ToString(), this.DefaultRowHeight.ToString());
            sheetSB.Append(this.ColumnList.ToString());

            if (IsShowTitle)
            {
                sheetSB.Append("<Row ss:AutoFitHeight=\"0\" ss:Height=\"20.25\">\n");
                sheetSB.AppendFormat("<Cell ss:MergeAcross=\"{0}\"  ss:StyleID=\"s1\"><Data ss:Type=\"String\">{1}</Data></Cell>\n", (cCount - 1).ToString(), this.Name);
                sheetSB.Append("</Row>\n");
            }

            foreach (Row r in this.RowList)
            {
                sheetSB.Append(r.ToString());
            }

            sheetSB.Append("</Table>\n");
            sheetSB.Append("</Worksheet>\n");
            return sheetSB.ToString();
        }
    }

    public class Column
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 样式ID
        /// </summary>
        public string StyleID { get; set; }
        /// <summary>
        /// 自适应大小
        /// </summary>
        public bool AutoWidth { get; set; }

        public Column(string styleID)
        {
            init(styleID, 0, false);
        }
        public Column(string styleID, double width)
        {
            init(styleID, width, false);
        }
        public Column(string styleID, double width, bool autoWidth)
        {
            init(styleID, width, autoWidth);
        }

        private void init(string styleID, double width, bool autoWidth)
        {
            this.Width = width;
            this.AutoWidth = autoWidth;
            this.StyleID = styleID;
        }

        public new string ToString()
        {
            StringBuilder colSB = new StringBuilder();

            colSB.AppendFormat("<Column ss:StyleID=\"{0}\" ss:AutoFitWidth=\"{1}\"",
                this.StyleID, (this.AutoWidth) ? "1" : "0");
            if (this.Width > 0)
                colSB.AppendFormat(" ss:Width=\"{0}\"", this.Width.ToString());

            colSB.Append("/>\n");
            return colSB.ToString();
        }
    }

    public class ColumnCollection : IEnumerable
    {
        private IList<Column> _columnList = new List<Column>();

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get { return _columnList.Count; } }

        /// <summary>
        /// 添加列格式
        /// </summary>
        /// <param name="styleID"></param>
        /// <returns></returns>
        public Column Add(string styleID)
        {
            return Add(styleID, 0, false);
        }
        /// <summary>
        /// 添加列格式
        /// </summary>
        /// <param name="styleID"></param>
        /// <returns></returns>
        public Column Add(string styleID, double width)
        {
            return Add(styleID, width, false);
        }
        /// <summary>
        /// 添加列格式
        /// </summary>
        /// <param name="styleID">样式ID</param>
        /// <param name="width">宽度</param>
        /// <param name="autoWidth">是否自动宽度</param>
        /// <returns></returns>
        public Column Add(string styleID, double width, bool autoWidth)
        {
            Column col = new Column(styleID, width, autoWidth);
            _columnList.Add(col);
            return col;
        }

        public void Delete(int index)
        {
            _columnList.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return _columnList.GetEnumerator();
        }

        public new string ToString()
        {
            StringBuilder colsSB = new StringBuilder();
            foreach (Column c in _columnList)
                colsSB.Append(c.ToString());
            return colsSB.ToString();
        }
    }

    /// <summary>
    /// 表单集合
    /// </summary>
    public class SheetCollection : IEnumerable
    {
        private IList<Sheet> _sheetList = new List<Sheet>();

        /// <summary>
        /// 表单数量
        /// </summary>
        public int Count { get { return _sheetList.Count; } }

        /// <summary>
        /// 添加工作表
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns>工作表对象</returns>
        public Sheet Add(string sheetName)
        {
            if (_sheetList.Any<Sheet>(p => p.Name == sheetName))
                throw new Exception("同一工作簿中工作表名不能相同！");

            Sheet sheet = new Sheet(sheetName);
            _sheetList.Add(sheet);
            return sheet;
        }

        /// <summary>
        /// 添加工作表
        /// </summary>
        /// <param name="sheet">工作表对象</param>
        /// <returns>工作表对象</returns>
        public Sheet Add(Sheet sheet)
        {
            if (_sheetList.Any<Sheet>(p => p.Name == sheet.Name))
                throw new Exception("同一工作簿中工作表名不能相同！");

            _sheetList.Add(sheet);
            return sheet;
        }

        /// <summary>
        /// 删除工作表
        /// </summary>
        /// <param name="index">工作表索引</param>
        public void Delete(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new Exception("下标超出范围！");
            }
            _sheetList.RemoveAt(index);
        }

        /// <summary>
        /// 获取工作表
        /// </summary>
        /// <param name="index">工作表索引</param>
        /// <returns></returns>
        public Sheet this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new Exception("下标超出范围！");
                }
                return _sheetList[index];
            }
        }

        /// <summary>
        /// 遍历工作表
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _sheetList.GetEnumerator();
        }

        public new string ToString()
        {
            StringBuilder sheetsSB = new StringBuilder();
            foreach (Sheet s in this._sheetList)
                sheetsSB.Append(s.ToString());
            return sheetsSB.ToString();
        }
    }

    /// <summary>
    /// 样式控制
    /// </summary>
    public class Style
    {
        /// <summary>
        /// 样式ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 横向对齐方式
        /// </summary>
        public HorizontalType Horizontal { get; set; }
        /// <summary>
        /// 竖向对齐方式
        /// </summary>
        public VerticalType Vertical { get; set; }
        /// <summary>
        /// 字体名称
        /// </summary>
        public FontFormat Font { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string BackgroundColor { get; set; }
        /// <summary>
        /// 格式化格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 宽度值，一个字符约为6.375
        /// </summary>
        public double Width { get; set; }

        public Style()
        {
            this.ID = "autoID";
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id">样式ID</param>
        /// <param name="format">格式化字符串</param>
        public Style(string id, string format)
        {
            this.ID = id;
            this.Format = format;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="width">一个字符约为6.375</param>
        public Style(double width)
        {
            this.ID = "autoID";
            this.Width = width;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id">样式ID</param>
        public Style(string format)
        {
            this.ID = "autoID";
            this.Format = format;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="width">一个字符约为6.375</param>
        public Style(string format, double width)
        {
            this.ID = "autoID";
            this.Format = format;
            this.Width = width;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id">样式ID</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="width">一个字符约为6.375</param>
        public Style(string id, string format, double width)
        {
            this.ID = id;
            this.Format = format;
            this.Width = width;
        }

        public new string ToString()
        {
            StringBuilder styleSB = new StringBuilder();
            styleSB.AppendFormat("<Style ss:ID=\"{0}\">\n", this.ID);

            if ((this.Horizontal > 0) || (this.Vertical > 0))
            {
                styleSB.Append("<Alignment");

                if (this.Horizontal > 0)
                    styleSB.AppendFormat(" ss:Horizontal=\"{0}\"", System.Enum.GetName(typeof(HorizontalType), this.Horizontal));

                if (this.Vertical > 0)
                    styleSB.AppendFormat(" ss:Vertical=\"{0}\"", System.Enum.GetName(typeof(VerticalType), this.Vertical));

                styleSB.Append(" />\n");
            }

            styleSB.Append(Font.ToString());

            if (!string.IsNullOrEmpty(BackgroundColor))
                styleSB.AppendFormat("<Interior ss:Color=\"{0}\" ss:Pattern=\"Solid\"/>\n", BackgroundColor);

            if (!string.IsNullOrEmpty(Format))
                styleSB.AppendFormat("<NumberFormat ss:Format=\"{0}\"/>\n", setFormat(Format));

            styleSB.AppendFormat("</Style>\n");

            return styleSB.ToString();
        }
        /// <summary>
        /// 格式化格式化字符串，不知道啥规矩，生成的文件是这样，不这样不好使
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string setFormat(string format)
        {
            format = format.Replace("-", "\\-");

            return Common.QuoteXMLString(format);
        }
    }

    /// <summary>
    /// 样式控制集合
    /// </summary>
    public class StyleCollection : IEnumerable
    {
        private uint _startIndex = 50;
        private uint _indexCount = 50;
        /// <summary>
        /// 获取当前样式索引ID值
        /// </summary>
        public uint StarIndex { get { return _indexCount; } }
        private IList<Style> _styleList = new List<Style>();
        /// <summary>
        /// 样式数量
        /// </summary>
        public int Count { get { return _styleList.Count; } }

        /// <summary>
        /// 添加样式
        /// </summary>
        /// <param name="styleID">系统将保留以s开头，后面为数字编号的ID，最小为s50，如果冲突，系统将自动重命名ID，自定义ID需要保证除首位外必须为数字的ID</param>
        /// <returns></returns>
        public Style Add(string styleID, string format)
        {
            Style newStyle = new Style(styleID, format);

            this.Add(newStyle);

            return newStyle;
        }

        /// <summary>
        /// 添加样式(系统将保留以s开头，后面为数字编号的ID，最小为s50，如果冲突，系统将自动重命名ID，自定义ID需要保证除首位外必须为数字的ID）
        /// </summary>
        /// <param name="style">style对象</param>
        /// <returns>style对象</returns>
        public Style Add(Style style)
        {
            uint index = 0;
            bool needRename = false;
            if (uint.TryParse(style.ID.Remove(0, 1), out index))
            {
                if (index < _startIndex)
                {
                    if (_styleList.Any<Style>(p => p.ID == style.ID))
                        needRename = true;
                }
                else
                    needRename = true;
            }
            else
                needRename = true;

            if (needRename)
            {
                _indexCount++;
                style.ID = string.Format("s{0}", _indexCount.ToString());
            }

            _styleList.Add(style);
            return style;
        }

        /// <summary>
        /// 删除工作表
        /// </summary>
        /// <param name="index">工作表索引</param>
        public void Delete(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new Exception("下标超出范围！");
            }
            _styleList.RemoveAt(index);
        }

        /// <summary>
        /// 获取工作表
        /// </summary>
        /// <param name="index">工作表索引</param>
        /// <returns></returns>
        public Style this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new Exception("下标超出范围！");
                }
                return _styleList[index];
            }
        }

        /// <summary>
        /// 遍历样式
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _styleList.GetEnumerator();
        }

        public new string ToString()
        {
            StringBuilder stylesSB = new StringBuilder();
            stylesSB.Append("<Styles>\n");

            foreach (Style s in _styleList)
            {
                stylesSB.Append(s.ToString());
            }

            stylesSB.Append("</Styles>\n");

            return stylesSB.ToString();
        }
    }

    /// <summary>
    /// 字体格式
    /// </summary>
    public struct FontFormat
    {
        /// <summary>字体名称</summary>
        public string FontName { get; set; }
        /// <summary>字符集名称</summary>
        public string CharSet { get; set; }
        /// <summary>字体大小</summary>
        public byte Size { get; set; }
        /// <summary>字体颜色</summary>
        public string Color { get; set; }
        /// <summary>是否粗体</summary>
        public bool Bold { get; set; }
        /// <summary>是否斜体</summary>
        public bool Italic { get; set; }
        /// <summary>下划线类型</summary>
        public UnderlineType Underline { get; set; }

        /// <summary>
        /// 转换为字符串格式
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            StringBuilder fontSB = new StringBuilder();
            fontSB.Append("<Font");

            if (!string.IsNullOrEmpty(FontName))
                fontSB.AppendFormat(" ss:FontName=\"{0}\"", FontName);

            if (!string.IsNullOrEmpty(CharSet))
                fontSB.AppendFormat(" x:CharSet=\"{0}\"", CharSet);

            if (Size > 0)
                fontSB.AppendFormat(" ss:Size=\"{0}\"", Size.ToString());

            if (!string.IsNullOrEmpty(Color))
                fontSB.AppendFormat(" ss:Color=\"{0}\"", Color);

            if (Bold)
                fontSB.Append(" ss:Bold=\"1\"");

            if (Italic)
                fontSB.Append(" ss:Italic=\"1\"");

            if (this.Underline > 0)
                fontSB.AppendFormat(" ss:Underline=\"{0}\"", System.Enum.GetName(typeof(UnderlineType), this.Underline));

            fontSB.Append(" />\n");

            if (fontSB.Length > 10)
                return fontSB.ToString();
            else return "";
        }
    }

    /// <summary>
    /// 单元格
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// 单元格值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// 样式ID
        /// </summary>
        public string StyleID { get; set; }

        /// <summary>
        /// 列位置（1为第1列）
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public Cell(string value, DataType type)
        {
            init(value, type, string.Empty, 0);
        }
        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <param name="styleID">样式ID</param>
        /// <returns></returns>
        public Cell(string value, DataType type, string styleID)
        {
            init(value, type, styleID, 0);
        }
        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <param name="index">列索引，1开始</param>
        /// <returns></returns>
        public Cell(string value, DataType type, int index)
        {
            init(value, type, string.Empty, index);
        }
        /// <summary>
        /// 添加一个列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">数据类型</param>
        /// <param name="styleID">样式ID</param>
        /// <param name="index">列索引，1开始</param>
        /// <returns></returns>
        private void init(string value, DataType type, string styleID, int index)
        {
            this.Value = value;
            this.Type = type;
            this.StyleID = styleID;
            this.Index = index;
        }

        public new string ToString()
        {
            StringBuilder cellSB = new StringBuilder();
            if (!string.IsNullOrEmpty(this.Value))
            {
                cellSB.Append("<Cell");
                if (!string.IsNullOrEmpty(this.StyleID))
                    cellSB.AppendFormat(" ss:StyleID=\"{0}\"", this.StyleID);
                if (this.Index > 0)
                    cellSB.AppendFormat(" ss:Index=\"{0}\"", this.Index.ToString());

                cellSB.Append(">");

                cellSB.AppendFormat("<Data ss:Type=\"{0}\">", System.Enum.GetName(typeof(DataType), this.Type));
                cellSB.Append(Common.QuoteXMLString(this.Value));
                cellSB.Append("</Data></Cell>\n");
            }
            return cellSB.ToString();
        }
    }

    public enum DataType
    {
        String,
        DateTime,
        Number
    }

    /// <summary>
    /// 横向对齐方式
    /// </summary>
    public enum HorizontalType
    {
        /// <summary>默认</summary>
        None,
        /// <summary>居中</summary>
        Center,
        /// <summary>左对齐</summary>
        Left,
        /// <summary>右对齐</summary>
        Right,
        /// <summary>填充</summary>
        Fill,
        /// <summary>两端对齐</summary>
        Justify,
        /// <summary>跨列居中</summary>
        CenterAcrossSelection,
        /// <summary>分散对齐</summary>
        Distributed
    }
    /// <summary>
    /// 竖向对齐方式
    /// </summary>
    public enum VerticalType
    {
        /// <summary>默认</summary>
        None,
        /// <summary>居中</summary>
        Center,
        /// <summary>居上</summary>
        Top,
        /// <summary>居下</summary>
        Bottom,
        /// <summary>两端对齐</summary>
        Justify,
        /// <summary>分散对齐</summary>
        Distributed
    }

    public enum UnderlineType
    {
        /// <summary>无</summary>
        None,
        /// <summary>单线</summary>
        Single,
        /// <summary>双线</summary>
        Double
    }

    public static class Common
    {
        /// <summary>
        /// 转换为格式化字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string QuoteXMLString(string value)
        {
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}