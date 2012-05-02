using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Common.Logging;
using System.Data;

namespace Common.Database
{
    public class SQLiteBase
    {
        SQLiteConnection Connection;
        SQLiteDataReader SqlData;
        public int RowCount { get; set; }

        public void Init(string db)
        {
            Connection = new SQLiteConnection("Data Source=" + db + ".sqdb;Version=3");

            try
            {
                Connection.Open();
            }
            catch (SQLiteException ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
            }
        }

        public bool Execute(string sql, params object[] args)
        {
            StringBuilder sqlString = new StringBuilder();
            sqlString.AppendFormat(sql, args);

            SQLiteCommand sqlCommand = new SQLiteCommand(sqlString.ToString(), Connection);

            try
            {
                sqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
                return false;
            }
        }

        public DataTable Select(string sql)
        {
            DataTable retData = new DataTable();

            SQLiteCommand sqlCommand = new SQLiteCommand(sql, Connection);
            
            try
            {
                SqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
                retData.Load(SqlData);
                SqlData.Close();
            }
            catch (SQLiteException ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
            }

            return retData;
        }
    }
}
