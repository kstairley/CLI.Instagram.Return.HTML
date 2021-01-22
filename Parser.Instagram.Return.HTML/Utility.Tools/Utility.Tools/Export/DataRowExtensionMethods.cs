using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TechShare.Utility.Tools.Export
{
    public static class DataRowExtensionMethods
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
    }
}
