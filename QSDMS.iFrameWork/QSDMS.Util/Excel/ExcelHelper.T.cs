using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Drawing;
using NPOI.HSSF.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace QSDMS.Util.Excel
{
    /// <summary>
    /// NPOI Excel泛型操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelHelper<T>
    {
        #region Excel导出方法 ExcelDownload
        /// <summary>
        /// Excel导出下载
        /// </summary>
        /// <param name="lists">List<T>数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static void ExcelDownload(List<T> lists, ExcelConfig excelConfig)
        {
            HttpContext curContext = HttpContext.Current;
            // 设置编码和附件格式
            curContext.Response.ContentType = "application/ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(excelConfig.FileName, Encoding.UTF8));
            //调用导出具体方法Export()
            curContext.Response.BinaryWrite(ExportMemoryStream(lists, excelConfig).GetBuffer());
            curContext.Response.End();
        }
        /// <summary>
        /// Excel导出下载
        /// </summary>
        /// <param name="list">数据源</param>
        /// <param name="templdateName">模板文件名</param>
        /// <param name="newFileName">文件名</param>
        public static void ExcelDownload(List<TemplateMode> list, string templdateName, string newFileName)
        {
            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.Charset = "UTF-8";
            response.ContentType = "application/vnd-excel";//"application/vnd.ms-excel";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + newFileName));
            System.Web.HttpContext.Current.Response.BinaryWrite(ExportListByTempale(list, templdateName).ToArray());
        }
        #endregion

        #region list导出到Excel文件excelConfig中FileName设置为全路径
        /// <summary>
        /// List<T>导出到Excel文件 ExcelImport
        /// </summary>
        /// <param name="lists">List<T>数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static void ExcelImport(List<T> lists, ExcelConfig excelConfig)
        {
            using (MemoryStream ms = ExportMemoryStream(lists, excelConfig))
            {
                using (FileStream fs = new FileStream(excelConfig.FileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region list导出到Excel的MemoryStream
        /// <summary>
        /// DataTable导出到Excel的MemoryStream Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static MemoryStream ExportMemoryStream(List<T> lists, ExcelConfig excelConfig)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            Type type = typeof(T);
            //PropertyInfo[] properties = type.GetProperties();
            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = ""; //填加xls文件作者信息
                si.ApplicationName = ""; //填加xls文件创建程序信息
                si.LastAuthor = ""; //填加xls文件最后保存者信息
                si.Comments = ""; //填加xls文件作者信息
                si.Title = ""; //填加xls文件标题信息
                si.Subject = "";//填加文件主题信息
                si.CreateDateTime = System.DateTime.Now;
                workbook.SummaryInformation = si;
               
            }
            //网格线
            sheet.DisplayGridlines = true;
            #endregion

            #region 设置标题样式
            //导出的列数
            int excelcolumnsLen = excelConfig.ColumnEntity.Count;
            ICellStyle headStyle = workbook.CreateCellStyle();
            int[] arrColWidth = new int[excelcolumnsLen];
            string[] arrColName = new string[excelcolumnsLen];//列名
            ICellStyle[] arryColumStyle = new ICellStyle[excelcolumnsLen];//样式表
            headStyle.Alignment = HorizontalAlignment.Center; // ------------------
            if (excelConfig.Background != new Color())
            {
                if (excelConfig.Background != new Color())
                {
                    headStyle.FillPattern = FillPattern.SolidForeground;
                    headStyle.FillForegroundColor = GetXLColour(workbook, excelConfig.Background);
                }
            }
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = excelConfig.TitlePoint;
            if (excelConfig.ForeColor != new Color())
            {
                font.Color = GetXLColour(workbook, excelConfig.ForeColor);
            }
            font.Boldweight = 700;
            headStyle.SetFont(font);
            #endregion

            #region 列头及样式
            ICellStyle cHeadStyle = workbook.CreateCellStyle();
            cHeadStyle.Alignment = HorizontalAlignment.Center; // ------------------
            IFont cfont = workbook.CreateFont();
            cfont.FontHeightInPoints = excelConfig.HeadPoint;
            cHeadStyle.SetFont(cfont);

            //有边框
            cHeadStyle.BorderBottom = BorderStyle.Thin;
            cHeadStyle.BorderLeft = BorderStyle.Thin;
            cHeadStyle.BorderRight = BorderStyle.Thin;
            cHeadStyle.BorderTop = BorderStyle.Thin;
            #endregion

            #region 设置内容单元格样式
            int i = 0;
            foreach (ColumnEntity expEntity in excelConfig.ColumnEntity)
            {
                string columnName = expEntity.Column;
                ICellStyle columnStyle = workbook.CreateCellStyle();
                columnStyle.Alignment = HorizontalAlignment.Center;
                // arrColWidth[i] = Encoding.GetEncoding(936).GetBytes(column.Name).Length;
                //arrColName[i] = columnName;
                var columnentity = expEntity;

                //ColumnEntity columnentity = excelConfig.ColumnEntity.Find(t => t.Column == column.Name);
                //if (columnentity != null)
                //{
                arrColName[i] = columnentity.ExcelColumn;
                if (columnentity.Width != 0)
                {
                    arrColWidth[i] = columnentity.Width;
                }
                if (columnentity.Background != new Color())
                {
                    if (columnentity.Background != new Color())
                    {
                        columnStyle.FillPattern = FillPattern.SolidForeground;
                        columnStyle.FillForegroundColor = GetXLColour(workbook, columnentity.Background);
                    }
                }
                if (columnentity.Font != null || columnentity.Point != 0 || columnentity.ForeColor != new Color())
                {
                    IFont columnFont = workbook.CreateFont();
                    columnFont.FontHeightInPoints = 10;
                    if (columnentity.Font != null)
                    {
                        columnFont.FontName = columnentity.Font;
                    }
                    if (columnentity.Point != 0)
                    {
                        columnFont.FontHeightInPoints = columnentity.Point;
                    }
                    if (columnentity.ForeColor != new Color())
                    {
                        columnFont.Color = GetXLColour(workbook, columnentity.ForeColor);
                    }
                    columnStyle.SetFont(font);
                }
                //}
                arryColumStyle[i] = columnStyle;
                i++;
            }


            #endregion

            #region 填充数据

            #endregion
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            int rowIndex = 0;
            foreach (T item in lists)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        if (excelConfig.Title != null)
                        {
                            IRow headerRow = sheet.CreateRow(0);
                            if (excelConfig.TitleHeight != 0)
                            {
                                headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                            }
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, excelConfig.ColumnEntity.Count)); // ------------------
                        }

                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(1);
                        #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                        int headIndex = 0;

                        foreach (var aaa in excelConfig.ColumnEntity)
                        {
                            headerRow.CreateCell(headIndex).SetCellValue(arrColName[headIndex]);
                            headerRow.GetCell(headIndex).CellStyle = cHeadStyle;
                            //设置列宽
                            sheet.SetColumnWidth(headIndex, (arrColWidth[headIndex] + 1) * 256);
                            headIndex++;
                        }
                        #endregion
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                int ordinal = 0;
                foreach (ColumnEntity expEntity in excelConfig.ColumnEntity)
                {
                    ICell newCell = dataRow.CreateCell(ordinal);
                    newCell.CellStyle = arryColumStyle[ordinal];
                    //find the property type
                    PropertyInfo p = type.GetProperty(expEntity.Column);
                    var columnType = p.PropertyType;
                    if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                        columnType = p.PropertyType.GetGenericArguments()[0];
                    }
                    string drValue = p.GetValue(item, null) == null ? "" : p.GetValue(item, null).ToString();
                    SetCell(newCell, dateStyle, columnType, drValue);
                    ordinal++;
                }

                #endregion
                rowIndex++;
            }
            //合并行
            if (excelConfig.MergeRangeIndexArr != null)
            {
                SetCellMerge(sheet, excelConfig.MergeRangeIndexArr);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        #endregion

        #region 设置表格内容
        private static void SetCell(ICell newCell, ICellStyle dateStyle, Type dataType, string drValue)
        {
            switch (dataType.ToString())
            {
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型
                    System.DateTime dateV;
                    if (System.DateTime.TryParse(drValue, out dateV))
                    {
                        newCell.SetCellValue(dateV);
                    }
                    else
                    {
                        newCell.SetCellValue("");
                    }
                    newCell.CellStyle = dateStyle;//格式化显示
                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }
        #endregion

        #region 读取excel ,默认第一行为标头
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="head">中文列名对照</param>
        /// <param name="workbookFile">Excel所在路径</param>
        /// <returns></returns>
        public List<T> ExcelImport(Hashtable head, string workbookFile)
        {
            try
            {
                HSSFWorkbook hssfworkbook;
                List<T> lists = new List<T>();
                using (FileStream file = new FileStream(workbookFile, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                HSSFSheet sheet = hssfworkbook.GetSheetAt(0) as HSSFSheet;
                IEnumerator rows = sheet.GetRowEnumerator();
                HSSFRow headerRow = sheet.GetRow(0) as HSSFRow;
                int cellCount = headerRow.LastCellNum;
                //Type type = typeof(T);
                PropertyInfo[] properties;
                T t = default(T);
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    HSSFRow row = sheet.GetRow(i) as HSSFRow;
                    t = Activator.CreateInstance<T>();
                    properties = t.GetType().GetProperties();
                    foreach (PropertyInfo column in properties)
                    {
                        int j = headerRow.Cells.FindIndex(delegate(ICell c)
                        {
                            return c.StringCellValue == (head[column.Name] == null ? column.Name : head[column.Name].ToString());
                        });
                        if (j >= 0 && row.GetCell(j) != null)
                        {
                            object value = valueType(column.PropertyType, row.GetCell(j).ToString());
                            column.SetValue(t, value, null);
                        }
                    }
                    lists.Add(t);
                }
                return lists;
            }
            catch (Exception ee)
            {
                string see = ee.Message;
                return null;
            }
        }
        #endregion

        #region RGB颜色转NPOI颜色
        private static short GetXLColour(HSSFWorkbook workbook, Color SystemColour)
        {
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    s = XlColour.Indexed;
                }

            }
            else
                s = XlColour.Indexed;
            return s;
        }
        #endregion

        #region ListExcel导出(加载模板)
        /// <summary>
        /// List根据模板导出ExcelMemoryStream
        /// </summary>
        /// <param name="list"></param>
        /// <param name="templdateName"></param>
        public static MemoryStream ExportListByTempale(List<TemplateMode> list, string templdateName)
        {
            try
            {

                string templatePath = HttpContext.Current.Server.MapPath("/") + "/Resources/ExcelTemplate/";
                string templdateName1 = string.Format("{0}{1}", templatePath, templdateName);

                FileStream fileStream = new FileStream(templdateName1, FileMode.Open, FileAccess.Read);
                ISheet sheet = null;
                if (templdateName.IndexOf(".xlsx") == -1)//2003
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(fileStream);
                    sheet = hssfworkbook.GetSheetAt(0);
                    SetPurchaseOrder(sheet, list);
                    sheet.ForceFormulaRecalculation = true;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        hssfworkbook.Write(ms);
                        ms.Flush();
                        return ms;
                    }
                }
                else//2007
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
                    sheet = xssfworkbook.GetSheetAt(0);
                    SetPurchaseOrder(sheet, list);
                    sheet.ForceFormulaRecalculation = true;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        xssfworkbook.Write(ms);
                        ms.Flush();
                        return ms;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 赋值单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="list"></param>
        private static void SetPurchaseOrder(ISheet sheet, List<TemplateMode> list)
        {
            try
            {
                foreach (var item in list)
                {
                    IRow row = null;
                    ICell cell = null;
                    row = sheet.GetRow(item.row);
                    if (row == null)
                    {
                        row = sheet.CreateRow(item.row);
                    }
                    cell = row.GetCell(item.cell);
                    if (cell == null)
                    {
                        cell = row.CreateCell(item.cell);
                    }
                    cell.SetCellValue(item.value);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region 从Excel导入
        /// <summary>
        /// 读取excel ,默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ExcelImport(string strFileName)
        {
            DataTable dt = new DataTable();

            ISheet sheet = null;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (strFileName.IndexOf(".xlsx") == -1)//2003
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                    sheet = hssfworkbook.GetSheetAt(0);
                }
                else//2007
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(file);
                    sheet = xssfworkbook.GetSheetAt(0);
                }
            }

            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 文件流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataTable ExcelImport(Stream stream)
        {
            DataTable dt = new DataTable();

            ISheet sheet = null;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(stream);
            sheet = hssfworkbook.GetSheetAt(0);

            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        #endregion
        public static void SetCellMerge(ISheet workSheet, int[] mergeRangeIndexArr)
        {
            if (mergeRangeIndexArr != null)
            {
                List<CellRangeAddress> rangList = new List<CellRangeAddress>();
                List<MergeRangeEntity> titleList = new List<MergeRangeEntity>();
                for (int i = workSheet.FirstRowNum + 1; i <= workSheet.LastRowNum; i++)
                {
                    if (i == 1)
                    {
                        int index = mergeRangeIndexArr[0];
                        MergeRangeEntity entity = new MergeRangeEntity();
                        entity.ColSpan = 1;
                        entity.ColumnName = workSheet.GetRow(i).GetCell(index).ToString();
                        titleList.Add(entity);
                    }

                    else
                    {
                        int index = mergeRangeIndexArr[0];
                        if (titleList[0].ColumnName != workSheet.GetRow(i).GetCell(index).ToString())
                        {
                            titleList[0].ColumnName = workSheet.GetRow(i).GetCell(index).ToString();

                            rangList.Add(new CellRangeAddress(titleList[0].ColSpan, i - 1, index, index));

                            titleList[0].ColSpan = i;
                        }
                        else
                        {
                            if (i == workSheet.LastRowNum)
                            {
                                rangList.Add(new CellRangeAddress(titleList[0].ColSpan, i, index, index));
                            }
                        }
                    }

                }
                //取第一列的数据为合并的标识
                List<CellRangeAddress> newrangList = new List<CellRangeAddress>();
                foreach (CellRangeAddress cellRangin in rangList)
                {
                    for (int i = 0; i < mergeRangeIndexArr.Length; i++)
                    {
                        int index = mergeRangeIndexArr[i];
                        newrangList.Add(new CellRangeAddress(cellRangin.FirstRow, cellRangin.LastRow, index, index));
                    }
                }

                foreach (CellRangeAddress cellRang in newrangList)
                {
                    workSheet.AddMergedRegion(cellRang);
                }
            }
        }
        object valueType(Type t, string value)
        {
            object o = null;
            string strt = "String";
            if (t.Name == "Nullable`1")
            {
                strt = t.GetGenericArguments()[0].Name;
            }
            switch (strt)
            {
                case "Decimal":
                    o = decimal.Parse(value);
                    break;
                case "Int":
                    o = int.Parse(value);
                    break;
                case "Float":
                    o = float.Parse(value);
                    break;
                case "DateTime":
                    o = DateTime.Parse(value);
                    break;
                default:
                    o = value;
                    break;
            }
            return o;
        }
    }

}
