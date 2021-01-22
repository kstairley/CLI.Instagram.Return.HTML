using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TechShare.Utility.Tools.TableDefinitions;
using Excel = Microsoft.Office.Interop.Excel;

namespace TechShare.Utility.Tools.Export
{
    public static class ExcelExportHelper
    {
        private static readonly string EXTENSION = ".xlsx";
        public static string SaveExcelFile(string savePath, string fileName, IEnumerable<DataTable> tables, bool includeHeaderRow = true, IEnumerable<TableDefinition> tableDefinitions = null, TableDefinition globalTableDefinition = null, List<KeyValuePair<string, List<string>>> additionalHeaderRowsByTable = null)
        {
            if (string.IsNullOrEmpty(savePath))
                throw new ArgumentException(typeof(CSVExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Path is required");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(typeof(CSVExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Filename is required");

            string savedFileName = string.Empty;
            if (tables != null && tables.Any(x => x.Rows.Count > 0))
            {
                Excel.Application excelApp = new Excel.Application();
                if (excelApp != null)
                {

                    if (!savePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        savePath += Path.DirectorySeparatorChar;

                    string filenameNoExt = fileName.Replace(EXTENSION, "");

                    Excel.Workbook workbook = excelApp.Workbooks.Add();
                    int unknownTableCount = 1;
                    foreach (DataTable table in tables.Where(x => x.Rows.Count > 0))
                    {
                        TableDefinition foundDef = globalTableDefinition != null ? globalTableDefinition :
                            (tableDefinitions != null ? tableDefinitions.Where(x => x.Name.Equals(table.TableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() : null);

                        Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                        if (!string.IsNullOrEmpty(table.TableName))
                        {
                            if (table.TableName.Length <= 31)
                                worksheet.Name = table.TableName;
                            else if (foundDef != null)
                                worksheet.Name = foundDef.ShortName;

                            if (string.IsNullOrEmpty(worksheet.Name))
                            {
                                worksheet.Name = "UNK" + unknownTableCount.ToString();
                                unknownTableCount++;
                            }
                        }

                        List<string> additionalHeaderRows = null;
                        if (additionalHeaderRowsByTable != null && additionalHeaderRowsByTable.Any(x => x.Key.Equals(table.TableName)))
                            additionalHeaderRows = additionalHeaderRowsByTable.Where(x => x.Key.Equals(table.TableName)).First().Value;

                        int additionalRowCount = additionalHeaderRows != null ? additionalHeaderRows.Count() : 0;
                        if (includeHeaderRow)
                            additionalRowCount++;

                        string[,] indexMatrix = new string[table.Rows.Count + additionalRowCount, table.Columns.Count];
                        if (indexMatrix.GetLength(0) > 0 && indexMatrix.GetLength(1) > 0)
                        {
                            int headerIndex = 0;
                            if (additionalHeaderRows != null && additionalHeaderRows.Any())
                            {
                                foreach (string additionalHeaderRow in additionalHeaderRows)
                                {
                                    indexMatrix[headerIndex, 0] = additionalHeaderRow;
                                    headerIndex++;
                                }
                            }
                            if (includeHeaderRow)
                            {
                                for (int cIdx = 0; cIdx < table.Columns.Count; cIdx++)
                                {
                                    string columnName = table.Columns[cIdx].ColumnName;
                                    if (foundDef != null)
                                    {
                                        ColumnDefinition foundCol = foundDef.Columns.Where(x => x.Name.Equals(table.Columns[cIdx].ColumnName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                        if (foundCol != null && !string.IsNullOrEmpty(foundCol.DisplayName))
                                            columnName = foundCol.DisplayName;
                                    }
                                    indexMatrix[headerIndex, cIdx] = columnName;
                                }
                                headerIndex++;
                            }
                            for (int rIdx = 0; rIdx < table.Rows.Count; rIdx++)
                            {
                                for (int cIdx = 0; cIdx < table.Columns.Count; cIdx++)
                                {

                                    indexMatrix[includeHeaderRow ? rIdx + headerIndex : rIdx, cIdx] = table.Rows[rIdx][cIdx].ToString();
                                }
                            }

                            int rowCount = indexMatrix.GetLength(0);
                            int columnCount = indexMatrix.GetLength(1);
                            // Get an Excel Range of the same dimensions
                            Excel.Range range = (Excel.Range)worksheet.Cells[1, 1];
                            range = range.get_Resize(rowCount, columnCount);
                            // Assign the 2-d array to the Excel Range
                            range.set_Value(Excel.XlRangeValueDataType.xlRangeValueDefault, indexMatrix);
                        }
                        Marshal.FinalReleaseComObject(worksheet);
                    }

                    if (workbook.Sheets.Count > 1)
                        ((Excel.Worksheet)workbook.Sheets[1]).Delete();
                    ((Excel.Worksheet)workbook.Sheets[1]).Activate();

                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);

                    savedFileName = savePath + filenameNoExt + EXTENSION;

                    excelApp.ActiveWorkbook.SaveAs(savedFileName, Excel.XlFileFormat.xlOpenXMLWorkbook);

                    workbook.Close();
                    excelApp.Quit();

                    Marshal.FinalReleaseComObject(workbook);
                    Marshal.FinalReleaseComObject(excelApp);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            return savedFileName;
        }
    }
}
