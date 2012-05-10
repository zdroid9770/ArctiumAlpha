using System;
using Common.Logging;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using System.Threading;

namespace Common.Database
{
    public class ObjectBase
    {
        IObjectContainer Connection;
        IObjectContainer Session;

        public string Database { get; set; }
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

        void ObjectDBThread()
        {
            Connection = Db4oEmbedded.OpenFile(Database + ".aodb");
        }

        public void Init(string db)
        {
            Database = db;
            new Thread(ObjectDBThread).Start();
        }

        public bool Save(object obj)
        {
            try
            {
                Session = Connection.Ext().OpenSession();
                Session.Store(obj);
                Commit(Session);

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
                Session = Connection.Ext().OpenSession();
                Session.Delete(obj);
                Commit(Session);

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
            Session = Connection.Ext().OpenSession();

            var sObject = from T o in Session select o;

            return sObject;
        }

        void Commit(IObjectContainer s)
        {
            s.Commit();
        }
    }
}
