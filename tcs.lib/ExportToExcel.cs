using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.IO;
using System.Reflection;
using NPOI.HSSF.UserModel;
using Array = System.Array;
using NPOI.SS.UserModel;

namespace tcs.lib
{
    public class ExportToExcel
    {
        /// <summary>
        /// Export data to excel from List object T
        /// </summary>
        /// <param name="list">Source data to export</param>
        /// <param name="templatePath">Path template to export</param>
        /// <param name="outFileName">File name excel export</param>
        public static void Export<T>(List<T> list, string templatePath, string outFileName)
        {
            Export(list, templatePath, outFileName, "", "");
        }

        /// <summary>
        /// Export data to excel from List object T
        /// </summary>
        /// <param name="list">Source data to export</param>
        /// <param name="templatePath">Path template to export</param>
        /// <param name="outFileName">File name excel export</param>
        /// <param name="subtitle">Title of data</param>
        public static void Export<T>(List<T> list, string templatePath, string outFileName, string subtitle)
        {
            Export(list, templatePath, outFileName, "", subtitle);
        }

        /// <summary>
        /// Export data to excel from List object T
        /// </summary>
        /// <param name="list">Source data to export</param>
        /// <param name="templatePath">Path template to export</param>
        /// <param name="outFileName">File name excel export</param>
        /// <param name="title">Title of excel</param>
        /// <param name="subtitle">Title of data</param>
        public static void Export<T>(List<T> list, string templatePath, string outFileName, string title, string subtitle)
        {
            try
            {
                if (list == null || list.Count <= 0) return;
                FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read); // Doc file
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true); // Lay thong tin workbook
                ISheet infoSheet = templateWorkbook.GetSheetAt(1); // Lay sheet thu 2 de lay dong bat dau va cot bat dau
                if (infoSheet == null)
                {
                    return;
                }
                try
                {
                    int startRowIndex = Convert.ToInt32(infoSheet.GetRow(2).GetCell(1).NumericCellValue); // Lay dong bat dau
                    int startColIndex = Convert.ToInt32(infoSheet.GetRow(2).GetCell(2).NumericCellValue); // Lay cot bat dau
                    infoSheet.GetRow(2).GetCell(0).SetCellType(CellType.BLANK); // xoa thong tin trong sheet2
                    infoSheet.GetRow(2).GetCell(1).SetCellType(CellType.BLANK); // xoa thong tin trong sheet2
                    infoSheet.GetRow(2).GetCell(2).SetCellType(CellType.BLANK); // xoa thong tin trong sheet2
                    ISheet sheet = templateWorkbook.GetSheetAt(0);
                    if (!string.IsNullOrEmpty(title))
                    {
                        int titleRowIndex = Convert.ToInt32(infoSheet.GetRow(0).GetCell(1).NumericCellValue);
                        int titleColIndex = Convert.ToInt32(infoSheet.GetRow(0).GetCell(2).NumericCellValue);
                        sheet.GetRow(titleRowIndex).GetCell(titleColIndex).SetCellValue(title);
                        infoSheet.GetRow(0).GetCell(0).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(0).GetCell(1).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(0).GetCell(2).SetCellType(CellType.BLANK);
                    }
                    if (!string.IsNullOrEmpty(subtitle))
                    {
                        int subtitleRowIndex = Convert.ToInt32(infoSheet.GetRow(1).GetCell(1).NumericCellValue);
                        int subtitleColIndex = Convert.ToInt32(infoSheet.GetRow(1).GetCell(2).NumericCellValue);
                        sheet.GetRow(subtitleRowIndex).GetCell(subtitleColIndex).SetCellValue(subtitle);
                        infoSheet.GetRow(1).GetCell(0).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(1).GetCell(1).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(1).GetCell(2).SetCellType(CellType.BLANK);
                    }
                    int n;
                    double d;
                    DateTime dt;
                    ICell cell;
                    PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (T item in list)
                    {
                        if (list.IndexOf(item) > 0)
                        {
                            CopyRow(templateWorkbook, sheet, startRowIndex + list.IndexOf(item) - 1, startRowIndex + list.IndexOf(item), startColIndex, startColIndex + props.Length - 1);
                        }
                        foreach (PropertyInfo prop in props)
                        {
                            cell = sheet.GetRow(list.IndexOf(item) + startRowIndex).GetCell(Array.IndexOf(props, prop) + startColIndex);
                            if (prop.GetValue(item, null) != null && cell != null)
                                if (prop.GetValue(item, null).GetType() == typeof(string))
                                {
                                    cell.SetCellValue(prop.GetValue(item, null).ToString());
                                }
                                else
                                {
                                    if (int.TryParse(prop.GetValue(item, null).ToString(), out n))
                                    {
                                        cell.SetCellValue(int.Parse(prop.GetValue(item, null).ToString()));
                                    }
                                    else
                                    {
                                        if (double.TryParse(prop.GetValue(item, null).ToString(), out d))
                                        {
                                            cell.SetCellValue(double.Parse(prop.GetValue(item, null).ToString()));
                                        }
                                        else
                                        {
                                            if (DateTime.TryParse(prop.GetValue(item, null).ToString(), out dt))
                                            {
                                                cell.SetCellValue(DateTime.Parse(prop.GetValue(item, null).ToString()));
                                            }
                                            else
                                            {
                                                cell.SetCellValue(prop.GetValue(item, null).ToString());
                                            }
                                        }
                                    }
                                }
                        }
                    }

                    // Tim dong tinh tong
                    int intStartRow = startRowIndex + list.Count;
                    int intEndRow = sheet.LastRowNum;

                    for (int k = intStartRow; k <= intEndRow; k++)
                    {
                        IRow lastRow = sheet.GetRow(k);
                        if (lastRow == null) continue;

                        // tinh cot tong cong cuoi cung
                        for (int i = 0; i < lastRow.LastCellNum; i++)
                        {
                            ICell lastRowCell = lastRow.GetCell(i);
                            if (lastRowCell == null) continue;

                            if (lastRowCell.CellType == CellType.FORMULA)
                            {
                                string oldFormular = lastRowCell.CellFormula;

                                // Find column index
                                if (oldFormular.Contains(":"))
                                {
                                    string strColumnId = oldFormular.Substring(oldFormular.LastIndexOf(":") + 1);
                                    // Loop until find a digit char
                                    for (int j = 0; j < strColumnId.Length; j++)
                                    {
                                        if (Char.IsNumber(strColumnId[j]))
                                        {
                                            strColumnId = strColumnId.Substring(0, j);
                                            break;
                                        }
                                    }

                                    string newFormular = string.Format("{0}:{1}{2})", oldFormular.Substring(0, oldFormular.LastIndexOf(':')), strColumnId, startRowIndex + list.Count);
                                    lastRowCell.CellFormula = newFormular;
                                }
                            }
                        }
                    }

                    sheet.ForceFormulaRecalculation = true;
                    templateWorkbook.SetActiveSheet(0);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        templateWorkbook.Write(ms);
                        HttpResponse response = HttpContext.Current.Response;
                        response.ContentType = "application/vnd.ms-excel";
                        response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", outFileName));
                        response.Clear();
                        response.BinaryWrite(ms.GetBuffer());
                        //response.End();
                        ms.Close();
                    }
                }
                catch (NullReferenceException ex)
                {
                    return;
                }
            }
            catch (FileNotFoundException ex)
            {
                return;
            }
        }

        public static void Export(DataTable data, string templatePath, string outFileName, string title, string subtitle)
        {
            try
            {
                FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                ISheet infoSheet = templateWorkbook.GetSheetAt(1);
                if (infoSheet == null)
                {
                    return;
                }
                try
                {
                    int startRowIndex = Convert.ToInt32(infoSheet.GetRow(2).GetCell(0).NumericCellValue);
                    int startColIndex = Convert.ToInt32(infoSheet.GetRow(2).GetCell(1).NumericCellValue);
                    infoSheet.GetRow(2).GetCell(0).SetCellType(CellType.BLANK);
                    infoSheet.GetRow(2).GetCell(1).SetCellType(CellType.BLANK);
                    infoSheet.GetRow(2).GetCell(2).SetCellType(CellType.BLANK);
                    ISheet sheet = templateWorkbook.GetSheetAt(0);
                    if (title != null)
                    {
                        int titleRowIndex = Convert.ToInt32(infoSheet.GetRow(0).GetCell(0).NumericCellValue);
                        int titleColIndex = Convert.ToInt32(infoSheet.GetRow(0).GetCell(1).NumericCellValue);
                        sheet.GetRow(titleRowIndex).GetCell(titleColIndex).SetCellValue(title);
                        infoSheet.GetRow(0).GetCell(0).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(0).GetCell(1).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(0).GetCell(2).SetCellType(CellType.BLANK);
                    }
                    if (subtitle != null)
                    {
                        int subtitleRowIndex = Convert.ToInt32(infoSheet.GetRow(1).GetCell(0).NumericCellValue);
                        int subtitleColIndex = Convert.ToInt32(infoSheet.GetRow(1).GetCell(1).NumericCellValue);
                        sheet.GetRow(subtitleRowIndex).GetCell(subtitleColIndex).SetCellValue(subtitle);
                        infoSheet.GetRow(1).GetCell(0).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(1).GetCell(1).SetCellType(CellType.BLANK);
                        infoSheet.GetRow(1).GetCell(2).SetCellType(CellType.BLANK);
                    }
                    int n;
                    double d;
                    DateTime dt;
                    ICell cell;
                    foreach (DataRow row in data.Rows)
                    {
                        if (data.Rows.IndexOf(row) > 0)
                        {
                            CopyRow(templateWorkbook, sheet, startRowIndex + data.Rows.IndexOf(row) - 1, startRowIndex + data.Rows.IndexOf(row), startColIndex, startColIndex + data.Rows.Count - 1);
                        }
                        for (int i = 0; i < row.ItemArray.Length; i++)
                        {
                            cell = sheet.GetRow(data.Rows.IndexOf(row) + startRowIndex).GetCell(i + startColIndex);
                            if (int.TryParse(row[i].ToString(), out n))
                            {
                                cell.SetCellValue(int.Parse(row[i].ToString()));
                            }
                            else
                            {
                                if (double.TryParse(row[i].ToString(), out d))
                                {
                                    cell.SetCellValue(double.Parse(row[i].ToString()));
                                }
                                else
                                {
                                    if (DateTime.TryParse(row[i].ToString(), out dt))
                                    {
                                        cell.SetCellValue(DateTime.Parse(row[i].ToString()));
                                    }
                                    else
                                    {
                                        cell.SetCellValue(row[i].ToString());
                                    }
                                }
                            }
                        }
                    }
                    IRow lastRow = sheet.GetRow(sheet.LastRowNum);
                    ICell lastRowCell;
                    for (int i = 0; i < lastRow.LastCellNum; i++)
                    {
                        lastRowCell = lastRow.GetCell(i);
                        if (lastRowCell != null)
                        {
                            if (lastRowCell.CellType == CellType.FORMULA)
                            {
                                string oldFormular = lastRowCell.CellFormula;
                                string newFormular = string.Format("{0}{1})", oldFormular.Substring(0, oldFormular.LastIndexOf(')') - (startRowIndex / 10) - 1), startRowIndex + data.Rows.Count);
                                lastRowCell.CellFormula = newFormular;
                            }
                        }
                    }
                    sheet.ForceFormulaRecalculation = true;
                    templateWorkbook.SetActiveSheet(0);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        templateWorkbook.Write(ms);
                        HttpResponse response = HttpContext.Current.Response;
                        response.ContentType = "application/vnd.ms-excel";
                        response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", outFileName));
                        response.Clear();
                        response.BinaryWrite(ms.GetBuffer());
                        response.End();
                        ms.Close();
                    }
                }
                catch (NullReferenceException ex)
                {
                    return;
                }
            }
            catch (FileNotFoundException ex)
            {
                return;
            }
        }

        public static bool isfirst = true;
        private static void CopyRow(HSSFWorkbook workbook, ISheet worksheet, int sourceRowNum, int destinationRowNum, int startCellNum, int endCellNum)
        {
            IRow newRow = worksheet.GetRow(destinationRowNum);
            IRow sourceRow = worksheet.GetRow(sourceRowNum);
            if (newRow != null)
            {
                worksheet.ShiftRows(destinationRowNum, worksheet.LastRowNum, 1);
            }
            else
            {
                newRow = worksheet.CreateRow(destinationRowNum);
            }

            ICellStyle newCellStyle;
            if (isfirst == true)
            {
                newCellStyle = workbook.CreateCellStyle();
                isfirst = false;
            }

            for (int i = startCellNum; i <= endCellNum; i++)
            {
                ICell oldCell = sourceRow.GetCell(i);
                ICell newCell = newRow.CreateCell(i);
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }
                newCell.CellStyle = oldCell.CellStyle;
            }
        }
    }
}