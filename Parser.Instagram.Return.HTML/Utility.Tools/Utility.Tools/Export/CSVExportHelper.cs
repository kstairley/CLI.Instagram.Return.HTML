using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using TechShare.Utility.Tools.TableDefinitions;

namespace TechShare.Utility.Tools.Export
{
    public static class CSVExportHelper
    {
        private static readonly int DEFAULT_ROWLIMIT = 1048576;
        private static readonly string EXTENSION = ".csv";
        public static IEnumerable<string> SaveCSVFiles(string savePath, string fileName, DataTable data, bool includeHeaderRow = true, TableDefinition tableDefinition = null, IEnumerable<string> additionalHeaderRows = null)
        {
            return SaveCSVFiles(savePath, fileName, data.AsDataView(), includeHeaderRow, tableDefinition, additionalHeaderRows);
            //if (string.IsNullOrEmpty(savePath))
            //    throw new ArgumentException(typeof(CSVExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Path is required");
            //if (string.IsNullOrEmpty(fileName))
            //    throw new ArgumentException(typeof(CSVExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Filename is required");

            //List<string> savedFileNames = new List<string>();
            //if (data != null && data.Rows.Count > 0)
            //{
            //    if (!savePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            //        savePath += Path.DirectorySeparatorChar;

            //    List<string> rows = new List<string>();

            //    string headerRow = null;
            //    if (includeHeaderRow)
            //    {
            //        if (tableDefinition != null)
            //        {
            //            List<string> headerList = new List<string>();
            //            foreach (DataColumn col in data.Columns)
            //            {
            //                ColumnDefinition foundDef = tableDefinition.Columns.Where(x => x.Name.Equals(col.ColumnName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            //                headerList.Add((foundDef != null) ? foundDef.DisplayName : col.ColumnName);

            //            }
            //            headerRow = string.Join(",", headerList);
            //        }
            //        else
            //        {
            //            headerRow = data.ConvertHeaderRowToCSV();
            //        }
            //    }

            //    foreach (DataRow row in data.Rows)
            //    {
            //        rows.Add(row.ConvertToCSV());
            //    }

            //    if (rows != null && rows.Any(x => !string.IsNullOrEmpty(x)))
            //    {
            //        bool multipleFiles = false;
            //        if (rows.Count() > DEFAULT_ROWLIMIT)
            //            multipleFiles = true;

            //        string filenameNoExt = fileName.Replace(EXTENSION, "");

            //        StringBuilder sb = new StringBuilder();
            //        int rowCount = 0;
            //        int fileCount = 1;
            //        string generatedFileName = string.Empty;

            //        if (!Directory.Exists(savePath))
            //            Directory.CreateDirectory(savePath);

            //        if (includeHeaderRow && !string.IsNullOrEmpty(headerRow))
            //            sb.AppendLine(headerRow);

            //        foreach (string row in rows.Where(x => !string.IsNullOrEmpty(x)))
            //        {
            //            sb.AppendLine(row);
            //            rowCount++;

            //            if (rowCount == DEFAULT_ROWLIMIT)
            //            {
            //                generatedFileName = savePath + filenameNoExt + "_" + fileCount.ToString() + EXTENSION;
            //                File.WriteAllText(generatedFileName, sb.ToString());
            //                savedFileNames.Add(generatedFileName);
            //                sb = new StringBuilder();
            //                if (includeHeaderRow && !string.IsNullOrEmpty(headerRow))
            //                    sb.AppendLine(headerRow);

            //                fileCount++;
            //                rowCount = 0;
            //            }
            //        }
            //        if (rowCount > 0)
            //        {
            //            generatedFileName = savePath + filenameNoExt + (multipleFiles ? "_" + fileCount.ToString() : "") + EXTENSION;
            //            File.WriteAllText(generatedFileName, sb.ToString());
            //            savedFileNames.Add(generatedFileName);
            //        }
            //    }
            //}
            //return savedFileNames;
        }
        public static IEnumerable<string> SaveCSVFiles(string savePath, string fileName, DataView data, bool includeHeaderRow = true, TableDefinition tableDefinition = null, IEnumerable<string> additionalHeaderRows = null)
        {
            if (string.IsNullOrEmpty(savePath))
                throw new ArgumentException(typeof(CSVExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Path is required");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(typeof(CSVExportHelper).Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - Filename is required");

            List<string> savedFileNames = new List<string>();
            if (data != null && data.Count > 0)
            {
                if (!savePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    savePath += Path.DirectorySeparatorChar;

                List<string> rows = new List<string>();


                string headerRow = null;
                if (includeHeaderRow)
                {
                    if (tableDefinition != null)
                    {
                        List<string> headerList = new List<string>();
                        foreach (DataColumn col in data.Table.Columns)
                        {
                            ColumnDefinition foundDef = tableDefinition.Columns.Where(x => x.Name.Equals(col.ColumnName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            headerList.Add((foundDef != null) ? foundDef.DisplayName : col.ColumnName);

                        }
                        headerRow = string.Join(",", headerList);
                    }
                    else
                    {
                        headerRow = data.Table.ConvertHeaderRowToCSV();
                    }
                }

                foreach (DataRowView row in data.Cast<DataRowView>())
                {
                    rows.Add(row.ConvertToCSV());
                }

                if (rows != null && rows.Any(x => !string.IsNullOrEmpty(x)))
                {
                    bool multipleFiles = false;
                    if (rows.Count() > DEFAULT_ROWLIMIT)
                        multipleFiles = true;

                    string filenameNoExt = fileName.Replace(EXTENSION, "");

                    StringBuilder sb = new StringBuilder();
                    int rowCount = 0;
                    int fileCount = 1;
                    string generatedFileName = string.Empty;

                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);

                    if (additionalHeaderRows != null && additionalHeaderRows.Any())
                    {
                        foreach (string additionalHeaderRow in additionalHeaderRows)
                            sb.AppendLine(additionalHeaderRow);
                    }

                    if (includeHeaderRow && !string.IsNullOrEmpty(headerRow))
                        sb.AppendLine(headerRow);

                    foreach (string row in rows.Where(x => !string.IsNullOrEmpty(x)))
                    {
                        sb.AppendLine(row);
                        rowCount++;

                        if (rowCount == DEFAULT_ROWLIMIT)
                        {
                            generatedFileName = savePath + filenameNoExt + "_" + fileCount.ToString() + EXTENSION;
                            File.WriteAllText(generatedFileName, sb.ToString());
                            savedFileNames.Add(generatedFileName);
                            sb = new StringBuilder();
                            if (includeHeaderRow && !string.IsNullOrEmpty(headerRow))
                                sb.AppendLine(headerRow);

                            fileCount++;
                            rowCount = 0;
                        }
                    }
                    if (rowCount > 0)
                    {
                        generatedFileName = savePath + filenameNoExt + (multipleFiles ? "_" + fileCount.ToString() : "") + EXTENSION;
                        File.WriteAllText(generatedFileName, sb.ToString());
                        savedFileNames.Add(generatedFileName);
                    }
                }
            }
            return savedFileNames;
        }
    }
}
