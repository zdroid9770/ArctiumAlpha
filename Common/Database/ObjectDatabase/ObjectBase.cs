using System;
using Common.Logging;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using System.Threading;

namespace Common.Database
{
    public class ObjectBase
    {
        public IObjectContainer Connection;

        public string Database { get; set; }
        public int RowCount { get; set; }

        void ObjectDBThread()
        {
            Connection = Db4oEmbedded.OpenFile(Database + ".aodb");
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

            int count = 0;
            foreach (object o in sObject)
                ++count;

            RowCount = count;

            return sObject;
        }
    }
}
