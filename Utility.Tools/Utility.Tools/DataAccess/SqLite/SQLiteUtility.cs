using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace TechShare.Utility.Tools.DataAccess.SqLite
{
    public static class SQLiteUtility
    {
        private static SQLiteBulkInsert sqlbulk = new SQLiteBulkInsert();
        public static string CreateDatabase(string databasePath, string caseIdentifier, string tableName, DataTable dt)
        {
            return CreateDatabase(databasePath + caseIdentifier + ".db", tableName, dt);
        }
        public static string CreateDatabase(string databaseFile, string tableName, DataTable dt)
        {
            string retVal = databaseFile;
            try
            {
                SQLiteConnection sqdb;//sqlite_conn;
                string connString = "Data Source=" + retVal + ";Version=3;";

                using (sqdb = new SQLiteConnection(connString))
                {
                    sqdb.Open();
                    sqdb.Close();
                    GC.Collect();
                    List<DataTable> lstDt = new List<DataTable>
                    {
                        dt
                    };
                    sqlbulk.SaveDataTable(lstDt, tableName, sqdb);
                }
            }
            catch (Exception e)
            {
                int i = 0;
            }
            return retVal;
        }
        public static void InsertData(string databasePath, string databaseName, DataTable dt)
        {
            if (dt != null)
            {
                if (!databasePath.EndsWith(@"\"))
                    databasePath += @"\";

                try
                {
                    IEnumerable<ColumnDefinition> columnDefs = ColumnDefinition.GetColumnDefinitions(dt);
                    if (columnDefs == null || !columnDefs.Any())
                        throw new DataException("No column definitions extrapolated from data.");

                    string connectionString = "Data Source=" + databasePath + databaseName + ".db;Version=3;";
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            CreateTable(conn, dt, columnDefs);
                            if (dt.Rows.Count > 0)
                            {
                                using (SQLiteTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            using (SQLiteCommand cmd = conn.CreateCommand())
                                            {
                                                cmd.CommandText = GenerateInsertStatement(dt, columnDefs);
                                                foreach (ColumnDefinition kvp in columnDefs)
                                                {
                                                    cmd.Parameters.Add(new SQLiteParameter(kvp.ParameterName, row[kvp.ColumnName] ?? DBNull.Value));
                                                }
                                                cmd.ExecuteNonQuery();
                                            }

                                        }
                                        trans.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        trans.Rollback();
                                        throw ex;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            conn.Close();
                            GC.Collect();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static void ExecuteNonQuery(string databasePath, string caseIdentifier, string statement)
        {
            try
            {
                if (!string.IsNullOrEmpty(statement))
                {
                    string connString = "Data Source=" + databasePath + caseIdentifier + ".db" + ";Version=3;";
                    using (SQLiteConnection conn = new SQLiteConnection(connString))
                    {
                        try
                        {
                            conn.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                cmd.CommandText = statement;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        finally
                        {
                            conn.Close();
                            GC.Collect();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CreateTable(SQLiteConnection conn, DataTable dt, IEnumerable<ColumnDefinition> columnDefs)
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM [sqlite_master] WHERE [type]='table' AND [name]=@p1;";
                cmd.Parameters.Add(new SQLiteParameter("@p1", dt.TableName));
                object result = cmd.ExecuteScalar();
                if (Int32.TryParse(result.ToString(), out int tableCount) && tableCount != 1)
                {
                    cmd.CommandText = string.Format("CREATE TABLE {0} ([RowId] INTEGER PRIMARY KEY AUTOINCREMENT, {1})", dt.TableName, string.Join(", ", columnDefs.Select(x => "[" + x.ColumnName + "] " + x.SQLiteDBType)));
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private static string GenerateInsertStatement(DataTable table, IEnumerable<ColumnDefinition> columnDefs)
        {
            if (string.IsNullOrEmpty(table.TableName))
                throw new DataException("Table name is required.");

            if (columnDefs == null || !columnDefs.Any())
                throw new DataException("Column definitions are required.");

            return string.Format("INSERT INTO [{0}] ({1}) VALUES ({2});", table.TableName, string.Join(", ", columnDefs.Select(x => "[" + x.ColumnName + "]")), string.Join(", ", columnDefs.Select(x => x.ParameterName)));
        }

        public static IEnumerable<string> GetUserDefinedTablesAndViews(string sourceFile)
        {
            IEnumerable<string> tables = GetUserDefinedTables(sourceFile);
            IEnumerable<string> views = GetUserDefinedViews(sourceFile);

            if (tables != null && views != null)
                return tables.Union(views);
            else if (tables == null && views != null)
                return views;
            else if (views == null && tables != null)
                return tables;
            else
                return null;
        }
        public static IEnumerable<string> GetUserDefinedTables(string sourceFile)
        {
            List<string> retVal = new List<string>();
            DataTable data = ReadData(sourceFile, "SELECT * FROM [sqlite_master] WHERE [type] = 'table' AND [name] NOT LIKE 'sqlite_%'");
            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    if (row.Table.Columns.Contains("name") && row["name"] != DBNull.Value && row["name"] != null)
                        retVal.Add(row["name"].ToString());
                }
            }
            return retVal;
        }
        public static IEnumerable<string> GetUserDefinedViews(string sourceFile)
        {
            List<string> retVal = new List<string>();
            DataTable data = ReadData(sourceFile, "SELECT * FROM [sqlite_master] WHERE [type] = 'view'");
            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    if (row.Table.Columns.Contains("name") && row["name"] != DBNull.Value && row["name"] != null)
                        retVal.Add(row["name"].ToString());
                }
            }
            return retVal;
        }
        public static IEnumerable<ExternalColumnDefinition> GetTableColumns(string sourceFile, string tableName)
        {
            List<ExternalColumnDefinition> retVal = new List<ExternalColumnDefinition>();

            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            parameters.Add(new SQLiteParameter("tableName", tableName));

            DataTable data = ReadData(sourceFile, "SELECT cid, name, type FROM PRAGMA_TABLE_INFO(@tableName);", parameters, null);
            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    if ((row.Table.Columns.Contains("cid") && row["cid"] != DBNull.Value && row["cid"] != null && Int32.TryParse(row["cid"].ToString(), out int tempCID)) &&
                        (row.Table.Columns.Contains("name") && row["name"] != DBNull.Value && row["name"] != null) &&
                        (row.Table.Columns.Contains("type") && row["type"] != DBNull.Value && row["type"] != null))
                    {
                        retVal.Add(new ExternalColumnDefinition(tempCID, row["name"].ToString(), row["type"].ToString()));
                    }
                }
            }
            return retVal;
        }
        public static DataTable ReadData(string sourceFile, string query, List<SQLiteParameter> parameters = null, DataTable emptyTable = null, string tableName = null)
        {
            if (!string.IsNullOrEmpty(sourceFile))
            {
                string connectionString = "Data Source=" + sourceFile + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        return ReadData(conn, query, parameters: parameters, emptyTable: emptyTable, tableName: tableName);
                    }
                    //catch (Exception ex)
                    //{
                    //    conn.Close();
                    //    throw ex;
                    //}
                    finally
                    {
                        conn.Close();
                        GC.Collect();
                    }
                }
            }
            return null;
        }

        public static DataTable ReadData(SQLiteConnection conn, string query, List<SQLiteParameter> parameters = null, DataTable emptyTable = null, string tableName = null)
        {
            DataTable retVal = emptyTable == null ? new DataTable(tableName) : emptyTable;
            if (conn != null && conn.State == ConnectionState.Closed)
                conn.Open();

            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                if (parameters != null && parameters.Any())
                    cmd.Parameters.AddRange(parameters.ToArray());

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(retVal);
                }
            }
            conn.Close();
            GC.Collect();
            return retVal;

        }
        private class ColumnDefinition
        {
            private ColumnDefinition(DataColumn col, int columnCount)
            {
                ColumnName = col.ColumnName;
                ParameterName = "@p" + columnCount;
                if (col.DataType == typeof(int) ||
                    col.DataType == typeof(bool))
                    SQLiteDBType = "INTEGER";
                else if (col.DataType == typeof(byte[]))
                    SQLiteDBType = "BLOB";
                else if (col.DataType == typeof(double) ||
                    col.DataType == typeof(long) ||
                    col.DataType == typeof(decimal) ||
                    col.DataType == typeof(float))
                    SQLiteDBType = "REAL";
                else
                    SQLiteDBType = "TEXT";
            }
            internal string ColumnName { get; private set; }
            internal string ParameterName { get; private set; }
            internal string SQLiteDBType { get; private set; }
            private bool IsValid
            {
                get
                {
                    return !string.IsNullOrEmpty(ColumnName) &&
                        !string.IsNullOrEmpty(ParameterName) &&
                        !string.IsNullOrEmpty(SQLiteDBType);
                }
            }

            internal static IEnumerable<ColumnDefinition> GetColumnDefinitions(DataTable table)
            {
                if (table == null)
                    throw new ApplicationException("Table undefined");

                List<ColumnDefinition> retVal = new List<ColumnDefinition>();

                int colCount = 0;
                foreach (DataColumn col in table.Columns)
                {
                    if (string.IsNullOrEmpty(col.ColumnName))
                        throw new ApplicationException("Column name undefined");

                    ColumnDefinition def = new ColumnDefinition(col, colCount);
                    if (def.IsValid)
                        retVal.Add(def);
                    else
                        throw new ApplicationException("Column definition is invalid.");
                    colCount++;
                }


                return retVal;
            }
        }
    }
}
