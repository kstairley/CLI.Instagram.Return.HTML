using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TechShare.Utility.Tools.DataAccess.SqLite
{
    public class SQLiteBulkInsert
    {

        private Dictionary<string, MySqlParameter> m_parameters = new Dictionary<string, MySqlParameter>();


        private static MySqlConnection m_dbCon;

        private string m_beginInsertText;
        public void SQLiteBulkInsert2(MySqlConnection dbConnection, string tableName)
        {
            m_dbCon = dbConnection;
            m_tableName = tableName;

            StringBuilder query = new StringBuilder(255);
            query.Append("INSERT INTO ["); query.Append(tableName); query.Append("] (");
            m_beginInsertText = query.ToString();
        }

        private bool m_allowBulkInsert = true;
        public bool AllowBulkInsert { get { return m_allowBulkInsert; } set { m_allowBulkInsert = value; } }

        public string CommandText
        {
            get
            {
                if (m_parameters.Count() < 1)
                {

                }

                StringBuilder sb = new StringBuilder(255);
                sb.Append(m_beginInsertText);

                foreach (string param in m_parameters.Keys)
                {
                    sb.Append('[');
                    sb.Append(param);
                    sb.Append(']');
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);

                sb.Append(") VALUES (");

                foreach (string param in m_parameters.Keys)
                {
                    sb.Append(m_paramDelim);
                    sb.Append(param);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);

                sb.Append(")");

                return sb.ToString();
            }
        }

        private uint m_commitMax = 10000;
        public uint CommitMax { get { return m_commitMax; } set { m_commitMax = value; } }

        private string m_tableName;
        public string TableName { get { return m_tableName; } }

        private string m_paramDelim = "@";
        public string ParamDelimiter { get { return m_paramDelim; } }

        public void AddParameter(string name, MySqlDbType dbType)
        {
            MySqlParameter param = new MySqlParameter(m_paramDelim + name, dbType);
            m_parameters.Add(name, param);
        }
        public double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        public Boolean Execute(MySqlCommand cmd)
        {
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public static Dictionary<string, string> GetColumnInfo(List<string> ColumnName, List<DbType> DbType)//used
        {
            Dictionary<string, string> dictColumns = new Dictionary<string, string>();

            for (int i = 0; i < ColumnName.Count; i++)
            {
                string x = DbType.ElementAt(i).ToString();
                if (DbType.ElementAt(i).ToString().Equals("Int32")) { x = "Int"; }
                try
                {
                    dictColumns.Add(ColumnName.ElementAt(i).ToString(), x);
                }
                catch (Exception e)
                {

                }
            }
            return dictColumns;
        }
        public static Dictionary<string, string> GetColumnInfo(List<string> ColumnName, List<MySqlDbType> DbType)//used
        {
            Dictionary<string, string> dictColumns = new Dictionary<string, string>();

            for (int i = 0; i < ColumnName.Count; i++)
            {
                string x = DbType.ElementAt(i).ToString();
                if (DbType.ElementAt(i).ToString().Equals("Int32")) { x = "Int"; }
                dictColumns.Add(ColumnName.ElementAt(i).ToString(), x);
            }
            return dictColumns;
        }
        public static string GetCreateTableCommand(string strTableName, Dictionary<string, string> dictColumns, Boolean Exist = false, Boolean UseRowId = false)
        {
            string strDropTable = "";
            if (Exist)
            {
                strDropTable = " IF NOT EXISTS ";
            }
            string strColumns = "`RowId` INTEGER PRIMARY KEY AUTOINCREMENT,";


            if (dictColumns.Count > 0)
            {
                // Loop through each column in the datagrid
                foreach (KeyValuePair<string, string> kvp in dictColumns)
                {
                    strColumns += string.Format("`{0}` {1} ,", kvp.Key, kvp.Value);
                }
            }
            else
            {
                strColumns = "";
            }


            strColumns = strColumns.TrimEnd(',');
            return string.Format("CREATE TABLE {0} `{1}` ({2});", strDropTable, strTableName, strColumns);
        }
        public static SQLiteConnection CreateTable(string tableName, string query, SQLiteConnection DBName, String FileName)//used
        {

            //DBName = SQLiteTest.MySqlTest_setup(FileName);

            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                cmd = DBName.CreateCommand();
                DBName.Open();
            }
            catch (Exception ex)
            {

            }
            try
            {
                cmd.CommandTimeout = 0;
                cmd.CommandText = String.Format("SELECT 1 FROM `{0}` LIMIT 1; ", tableName);
                cmd.ExecuteNonQuery();
                //cmd.CommandText = String.Format("Drop Table `{0}`", tableName);
                //cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }

            return DBName;
        }
        public DataTable SqlCycloComplextyQuery(String CommandText, SQLiteConnection conn, bool Social = false) //used
        {

            DataTable table = new DataTable();
            try
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {

                }
                using (SQLiteCommand fmd = conn.CreateCommand())
                {
                    fmd.CommandText = CommandText;
                    fmd.CommandType = CommandType.Text;
                    SQLiteDataAdapter da = new SQLiteDataAdapter(fmd);
                    da.Fill(table);
                }
                conn.Close();

            }
            catch (Exception e)
            {
                //err.log(e, "problem inserting data into database", CaseName);
                //throw;
                return null;
            }
            return table;
        }

        public String ChangeDates_LongHand4(String DateTimex)//used
        {
            String DateField = DateTimex;
            String godate = "";
            if (DateTimex != null)
            {

                DateTime date;
                try
                {
                    DateField = DateField.Replace("(AST)", "");
                    DateField = DateField.Replace("(EST)", "");
                    DateField = DateField.Replace("(EDT)", "");
                    DateField = DateField.Replace("(CST)", "");
                    DateField = DateField.Replace("(CDT)", "");
                    DateField = DateField.Replace("(MST)", "");
                    DateField = DateField.Replace("(UTC)", "");
                    DateField = DateField.Replace("(MDT)", "");
                    DateField = DateField.Replace("(PST)", "");
                    DateField = DateField.Replace("(PDT)", "");
                    DateField = DateField.Replace("(AKST)", "");
                    DateField = DateField.Replace("(AKDT)", "");
                    DateField = DateField.Replace("(HST)", "");
                    DateField = DateField.Replace("(HAST)", "");
                    DateField = DateField.Replace("(HADT)", "");
                    DateField = DateField.Replace("(SST)", "");
                    DateField = DateField.Replace("(CHST)", "");
                    DateField = DateField.Replace("(UTC)", "");
                    DateField = DateField.Replace("(EDT)", "");
                    //    DateField = DateField.Replace("-", " ");
                    DateField = DateField.Trim();


                    try
                    {

                        date = Convert.ToDateTime(DateField);
                    }
                    catch (Exception e)
                    {
                        date = tryThisNew(DateField);
                    }
                    //Tue Jul 11 02:14:12 2017

                    //   DateTime date = DateTime.Parse(DateField);

                    if (date != DateTime.MinValue)
                    {
                        double unixepoch = ConvertToUnixTimestamp(date);

                        godate = unixepoch.ToString();
                    }

                }
                catch (Exception e)
                {

                }
            }
            else
            {

            }
            return godate;
        }

        public DateTime tryThisNew(String Date)//used
        {
            DateTime dateParsed = DateTime.MinValue;
            //Sat, 31 Dec 2016 23:46:40  0800


            if (DateTime.TryParseExact(Date, "ddd, dd MMM yyyy HH:mm:ss zzzz", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out dateParsed))
            {
                return dateParsed;
            }
            return dateParsed;
        }
        public string CreateInsertText(List<string> Columns, String TableName)
        {
            string InsertQuote = String.Format("Insert into {0} (", TableName);

            foreach (string c in Columns)
            {
                InsertQuote += string.Format("`{0}`,", c);
            }
            InsertQuote = InsertQuote.TrimEnd(',');
            InsertQuote += ") VALUES(";
            foreach (string cx in Columns)
            {
                InsertQuote += string.Format("@{0}@,", cx);
            }
            InsertQuote = InsertQuote.TrimEnd(',');
            InsertQuote += ")";
            string DevCheck = InsertQuote;
            return InsertQuote;
        }
        public void bulkSqlInsert(String cmdString, SQLiteConnection con, List<string> Columns, List<DbType> dbtypes, object[] o, String TableName)//used
        {
            //con.Close();
            //  Parallel.ForEach<object[]>(Body, // source collection


            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                String CmdString = cmdString;
                cmd.CommandText = cmdString;
                cmd.CommandType = CommandType.Text;



                cmd.Connection = con;

                cmd.Parameters.Clear();
                for (int x = 0; x < Columns.Count; x++)
                {
                    cmd.Parameters.Add(String.Format("@{0}", Columns.ElementAt(x)), dbtypes.ElementAt(x));

                }
                try
                {
                    for (int a = 0; a < Columns.Count; a++)
                    {
                        var dbtype = dbtypes.ElementAt(a);
                        if (dbtype.ToString().Equals("LongBlob"))
                        {
                            if (string.IsNullOrWhiteSpace(o[a].ToString()))
                            {
                                cmd.Parameters["@" + Columns.ElementAt(a)].Value = null;
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", ""));
                            }
                            else
                            {
                                o[a] = System.Text.Encoding.UTF8.GetBytes(o[a].ToString());
                                cmd.Parameters["@" + Columns.ElementAt(a)].Value = o[a];
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", o[a].ToString()));
                            }

                        }
                        else if (dbtype.ToString().Equals("DateTime"))
                        {
                            if (string.IsNullOrWhiteSpace(o[a].ToString()))
                            {
                                cmd.Parameters["@" + Columns.ElementAt(a)].Value = null;
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", ""));
                            }
                            else
                            {
                                DateTime dt = Convert.ToDateTime(o[a]);
                                cmd.Parameters["@" + Columns.ElementAt(a)].Value = dt.ToString();
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", dt.ToString()));
                            }
                        }
                        else if (dbtype.ToString().Equals("Float") || dbtype.ToString().Equals("Int32"))
                        {
                            if (string.IsNullOrWhiteSpace(o[a].ToString()))
                            {
                                cmd.Parameters["@" + Columns.ElementAt(a)].Value = null;
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", ""));
                            }
                            else
                            {
                                cmd.Parameters["@" + Columns.ElementAt(a)].Value = o[a];
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", o[a].ToString()));
                            }

                        }
                        else
                        {
                            if (o[a] != null)
                            {
                                if (string.IsNullOrWhiteSpace(o[a].ToString()))
                                {
                                    try
                                    {
                                        cmd.Parameters["@" + Columns.ElementAt(a)].Value = "''";
                                        CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", ""));
                                    }
                                    catch (Exception esx)
                                    {

                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        cmd.Parameters["@" + Columns.ElementAt(a)].Value = MySqlHelper.EscapeString(o[a].ToString().Replace("'", "''"));
                                        CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", o[a].ToString()));


                                    }
                                    catch (Exception esx)
                                    {

                                    }

                                }
                            }
                            else
                            {
                                cmd.Parameters["@" + Columns.ElementAt(a)].Value = "''";
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", ""));
                            }
                        }

                    }

                    try
                    {

                        cmd.CommandText = CmdString;
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException esrrx)
                    {
                        if (esrrx.ErrorCode == -2147467259)
                        {
                            for (int x = 0; x < o.Length; x++)
                            {
                                Encoding wind1252 = Encoding.GetEncoding(1252);
                                Encoding utf8 = Encoding.UTF8;
                                byte[] wind1252Bytes = wind1252.GetBytes(o[x].ToString());
                                byte[] utf8Bytes = Encoding.Convert(wind1252, utf8, wind1252Bytes);
                                string utf8String = Encoding.UTF8.GetString(utf8Bytes);
                                o[x] = utf8String.Replace("'", "");
                            }
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Trying again ");
                            Console.ForegroundColor = ConsoleColor.White;
                            bulkSqlInsert(cmdString, con, Columns, dbtypes, o, TableName);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("error" + esrrx.ToString());
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("error" + e.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    //Console.WriteLine("issue");
                }
            }
            string end = "true";
        }
        public Dictionary<string, string> GetColumnInfo(DataTable dt)
        {
            Dictionary<string, string> dictColumns = new Dictionary<string, string>();
            if (dt != null)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    string dataType = "TEXT";
                    if (col.DataType == typeof(double) || col.DataType == typeof(long))
                        dataType = "DOUBLE";
                    else if (col.DataType == typeof(bool))
                        dataType = "BOOLEAN";
                    else if (col.DataType == typeof(DateTime))
                        dataType = "DATETIME";
                    else if (col.DataType == typeof(int))
                        dataType = "INTEGER";
                    dictColumns.Add(col.ColumnName, dataType);// "string");
                }
            }
            return dictColumns;
        }
        public bool SaveDataTable(List<DataTable> dtx, string tableName, SQLiteConnection conn, Boolean UseRowId = false)
        {
            return SaveDataTable(dtx, tableName, null, conn, UseRowId);
        }
        public bool SaveDataTable(List<DataTable> dtx, string tableName, string CaseName, SQLiteConnection conn, Boolean UseRowId = false)
        {


            bool Worksx = true;
            int RotationCount = 0;



            if (dtx != null && dtx.Count != 0)
            {
                int ix = 0;
                foreach (DataTable DT in dtx)
                {
                    ix++;

                    try
                    {
                        Dictionary<string, string> Columnsx = GetColumnInfo(DT);
                        if (Columnsx.ContainsKey("Row_id"))
                        {
                            Columnsx.Remove("Row_id");
                        }

                        if (Columnsx.ContainsKey("RowId"))
                        {
                            Columnsx.Remove("RowId");
                        }

                        String Query = GetCreateTableCommand(tableName, Columnsx);

                        if (DT.Rows.Count > 0 || RotationCount == 0)
                        {
                            SqlCycloComplextyQuery(Query, conn);
                        }
                    }
                    catch (Exception e)
                    {
                        Worksx = false;
                    }
                    if (DT != null)
                    {
                        if (DT.Rows.Count > 0)
                        {
                            DataTable table = new DataTable();
                            try
                            {
                                foreach (DataRow row in DT.Rows)
                                {
                                    if (row.RowState == DataRowState.Unchanged)
                                    {
                                        row.SetAdded();
                                    }
                                }

                                string strColumns = "";


                                using (
                                    SQLiteConnection connect =
                                        new SQLiteConnection(conn))
                                {
                                    try
                                    {
                                        connect.Open();
                                    }
                                    catch (Exception e)
                                    {

                                    }

                                    string selectCommand = string.Format("SELECT * FROM [{0}]", tableName);
                                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(selectCommand, connect);
                                    adapter.AcceptChangesDuringFill = false;
                                    SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
                                    SQLiteTransaction transaction = connect.BeginTransaction();
                                    builder.GetInsertCommand().Transaction = transaction;
                                    int rowsAffected = adapter.Update(DT.Select());
                                    transaction.Commit();

                                    connect.Close();

                                }
                            }
                            catch (Exception e)
                            {
                                String error = e.ToString();
                                Worksx = false;
                            }
                        }
                    }
                    RotationCount++;
                }
            }
            else
            {
                Worksx = false;
            }


            return Worksx;
        }

        public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

    }
}
