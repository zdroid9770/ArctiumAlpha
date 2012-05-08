using System;
using Common.Logging;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;

namespace Common.Database
{
    public class ObjectBase
    {
        IObjectContainer Connection;
        public int RowCount
        {
            get
            {
                int count = 0;
                var obj = Select<Object>();
                foreach (object o in obj)
                    ++count;

                return count;
            }
        }

        public void Init(string db)
        {
            Connection = Db4oEmbedded.OpenFile(db + ".aodb");
        }

        public bool Save(object obj)
        {
            try
            {
                Connection.Store(obj);
                Connection.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
                return false;
            }
        }

        public bool Delete(object obj)
        {
            try
            {
                Connection.Delete(obj);
                Connection.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
                return false;
            }
        }

        public IDb4oLinqQuery Select<T>()
        {
            var sObject = from T o in Connection select o;
            return sObject;
        }
    }
}
