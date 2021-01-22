using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TechShare.Utility.Tools.DataAccess.SqLite
{
    public class SQLServerBulkInsert
    {
        private Dictionary<string, SqlParameter> m_parameters = new Dictionary<string, SqlParameter>();


        private static SqlConnection m_dbCon;

        private string m_beginInsertText;
        public void SQLServerBulkInsert2(SqlConnection dbConnection, string tableName)
        {
            m_dbCon = dbConnection;
            m_tableName = tableName;

            StringBuilder query = new StringBuilder(255);
            query.Append("INSERT INTO ["); query.Append(tableName); query.Append("] (");
            m_beginInsertText = query.ToString();
        }
        private bool m_allowBulkInsert = true;
        public bool AllowBulkInsert { get { return m_allowBulkInsert; } set { m_allowBulkInsert = value; } }
        private uint m_commitMax = 10000;
        public uint CommitMax { get { return m_commitMax; } set { m_commitMax = value; } }

        private string m_tableName;
        public string TableName { get { return m_tableName; } }

        private string m_paramDelim = "@";
        public string ParamDelimiter { get { return m_paramDelim; } }
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
        public void AddParameter(string name, SqlDbType dbType)
        {
            SqlParameter param = new SqlParameter(m_paramDelim + name, dbType);
            m_parameters.Add(name, param);
        }
        public double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        public Boolean Execute(SqlCommand cmd)
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
        public static Dictionary<string, string> GetColumnInfo(List<string> ColumnName, List<SqlDbType> DbType)//used
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
        public static string GetCreateTableCommand(string strTableName, Dictionary<string, string> dictColumns,Boolean Identity=true)
        {
            string strDropTable = "";
            string strColumns = "";

            if (dictColumns.Count > 0)
            {
                if (Identity)
                {
                    strColumns += "[RowId] int not null identity(1, 1) primary key,";
                }
                // Loop through each column in the datagrid
                foreach (KeyValuePair<string, string> kvp in dictColumns)
                {
                    strColumns += string.Format("[{0}] {1}(max) ,", kvp.Key, kvp.Value);
                }
            }
            else
            {
                strColumns = "";
            }

            strColumns = strColumns.TrimEnd(',');
            return string.Format("IF NOT EXISTS (SELECT[name] FROM sys.tables WHERE[name] = '{1}') CREATE TABLE [{1}] ({2});", strDropTable, strTableName, strColumns);
        }
        public static SqlConnection CreateTable(string tableName, string query, SqlConnection DBName, String FileName)//used
        {

            //DBName = SQLiteTest.MySqlTest_setup(FileName);

            SqlCommand cmd = new SqlCommand();
            try
            {
               
                cmd = DBName.CreateCommand(); //Do not switch this around call the command before trying to open connection...most likely will already be open
               
                DBName.Open();

            }
            catch (Exception ex)
            {

            }
            try
            {
                cmd.CommandTimeout = 0;
                cmd.CommandText = String.Format("SELECT 1 FROM [{0}]; ", tableName);
                cmd.ExecuteNonQuery();
                //cmd.CommandText = String.Format("Drop Table `{0}`", tableName);
                //cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                try
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {

                }
            }

            return DBName;
        }
        public DataTable SqlCycloComplextyQuery(String CommandText, SqlConnection conn, bool Social = false) //used
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
                SqlCommand fmd = conn.CreateCommand();
                        fmd.CommandText = CommandText;
                        fmd.CommandType = CommandType.Text;
                        SqlDataAdapter da = new SqlDataAdapter(fmd);
                        da.Fill(table);
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
        public void bulkSqlInsert(String cmdString, SqlConnection con, List<string> Columns, List<SqlDbType> dbtypes, object[] o, String TableName)//used
        {
            //con.Close();
            //  Parallel.ForEach<object[]>(Body, // source collection


            using (SqlCommand cmd = new SqlCommand())
            {
                String CmdString = cmdString;
                cmd.CommandText = cmdString;
                cmd.CommandType = CommandType.Text;



                cmd.Connection = con;

                cmd.Parameters.Clear();
                for (int x = 0; x < Columns.Count; x++)
                {
                    cmd.Parameters.Add(String.Format(@"{0}", Columns.ElementAt(x)), dbtypes.ElementAt(x));

                }
                try
                {
                    for (int a = 0; a < Columns.Count; a++)
                    {
                        var dbtype = dbtypes.ElementAt(a);
                        if (dbtype.ToString().Equals("VarChar"))
                        {
                            if (o[a] != null)
                            {
                                if (string.IsNullOrWhiteSpace(o[a].ToString()))
                                {
                                    cmd.Parameters[Columns.ElementAt(a)].Value = "";
                                    CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", ""));
                                }
                                else
                                {
                                    // o[a] = System.Text.Encoding.UTF8.GetBytes(o[a].ToString());
                                    cmd.Parameters[Columns.ElementAt(a)].Value = o[a];
                                    CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", o[a].ToString()));
                                }
                            }
                            else
                            {
                                cmd.Parameters[Columns.ElementAt(a)].Value = "";
                                CmdString = CmdString.Replace("@" + Columns.ElementAt(a) + "@", String.Format("'{0}'", ""));
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

        public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }
    }
    }
