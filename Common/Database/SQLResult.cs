using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

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
