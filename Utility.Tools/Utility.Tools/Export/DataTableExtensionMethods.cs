using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TechShare.Utility.Tools.Export
{
    public static class DataTableExtensionMethods
    {
        public static string ConvertToCSV(this DataRow row)
        {
            List<string> list = new List<string>();
            foreach (DataColumn col in row.Table.Columns)
            {
                if (col.DataType != typeof(string))
                {
                    list.Add((row[col] != null && row[col] != DBNull.Value) ? row[col].ToString() : string.Empty);
                }
                else
                {
                    string val = row[col].ToString();
                    val = val.Replace("\"", "\"\"");
                    list.Add(!string.IsNullOrEmpty(val) ? "\"" + val + "\"" : string.Empty);
                }
            }
            return list.Any() ? string.Join(",", list) : string.Empty;
        }

        public static string ConvertToCSV(this DataRowView row)
        {
            List<string> list = new List<string>();
            foreach (DataColumn col in row.DataView.Table.Columns)
            {
                if (col.DataType != typeof(string))
                {
                    list.Add((row[col.ColumnName] != null && row[col.ColumnName] != DBNull.Value) ? row[col.ColumnName].ToString() : string.Empty);
                }
                else
                {
                    string val = row[col.ColumnName].ToString();
                    val = val.Replace("\"", "\"\"");
                    list.Add(!string.IsNullOrEmpty(val) ? "\"" + val + "\"" : string.Empty);
                }
            }
            return list.Any() ? string.Join(",", list) : string.Empty;
        }

        public static string ConvertHeaderRowToCSV(this DataTable table)
        {
            if (table != null && table.Columns.Count > 0)
            {
                return string.Join(",", table.Columns.Cast<DataColumn>().Select(x => x.ColumnName));
            }
            return null;
        }
    }
}
