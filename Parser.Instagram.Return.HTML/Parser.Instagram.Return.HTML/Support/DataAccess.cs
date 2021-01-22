using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using TechShare.Utility.Tools.DataAccess.SqLite;
using TechShare.Utility.Tools.Zip;

namespace TechShare.Parser.Instagram.Return.HTML.Support
{
    public class DataAccess
    {
        static SQLiteBulkInsert sqlbulk = new SQLiteBulkInsert();

        public static void CreateDatabase(string defaultDirectory, DataTable dt, string Name, string CaseName)
        {
            string databasePath = AppDomain.CurrentDomain.BaseDirectory + defaultDirectory;
            try
            {
                SQLiteConnection sqdb;//sqlite_conn;
                string connString = "Data Source=" + databasePath + CaseName + ".db;Version=3;";

                using (sqdb = new SQLiteConnection(connString))
                {
                    sqdb.Open();
                    sqdb.Close();
                    List<DataTable> lstDt = new List<DataTable>();
                    lstDt.Add(dt);
                    sqlbulk.SaveDataTable(lstDt, Name, CaseName, sqdb);
                }
            }
            catch (Exception e)
            {
                int i = 0;
            }
        }

        public static void DiffPreservationAndSave(string defaultDirectory, string CaseName, string preservationTableName, string queryString)
        {
            string databasePath = AppDomain.CurrentDomain.BaseDirectory + defaultDirectory;
            SQLiteConnection sqdb;
            string connString = "Data Source=" + databasePath + CaseName + ".db;Version=3;";

            DataTable dt = new DataTable();
            try
            {
                using (sqdb = new SQLiteConnection(connString))
                {
                    //don't include a rowid.  will fail in sqlitebulkinsert.cs
                    if (queryString != string.Empty)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(queryString, sqdb))
                        {
                            sqdb.Open();
                            // create data adapter
                            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                            // this will query your database and return the result to your datatable
                            da.Fill(dt);
                            sqdb.Close();
                            da.Dispose();
                        }
                    }

                }
                if (dt.Rows.Count > 0)
                    CreateDatabase(defaultDirectory, dt, "Diff_" + preservationTableName, CaseName);
            }
            catch (Exception)
            {
            }
        }

        public static void AddSourceFiles(string defaultDirectory, List<ExtractFileInfo> filePaths, string caseName)
        {
            string databasePath = AppDomain.CurrentDomain.BaseDirectory + defaultDirectory;
            DataTable dt = new DataTable();
            dt.Columns.Add("File");

            IEnumerable<string> paths = filePaths.Where(x => !string.IsNullOrEmpty(x.OriginalFile_Path) && !x.IsTemporary).Select(x => x.OriginalFile_Path).Distinct();
            foreach (string path in paths)
            {
                dt.Rows.Add(path);
            }
            CreateDatabase(defaultDirectory, dt, "SourceFiles", caseName);
        }
    }
}
