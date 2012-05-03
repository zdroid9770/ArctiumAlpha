using System;
using System.Data;

namespace Common.Database
{
    public class SQLResult : DataTable
    {
        public int Count { get; set; }

        public T Read<T>(int row, byte column)
        {
            return (T)Convert.ChangeType(Rows[row].ItemArray[column], typeof(T));
        }
    }
}
