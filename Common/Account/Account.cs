using System;
using Common.Database.ObjectDatabase;
using Common.Structs;
using Db4objects.Db4o.Linq;

namespace Common.Account
{
    public class Account
    {
        public uint Id;
        public string Name;
        public string Password;
        public string IP;
        public byte GMLevel;
        public string Language;

        public static Account GetAccountByName(string name)
        {
            Account acc = null;
            var conn = ODB.Realms.Connection;
            var account = from Account a in conn where a.Name == name select a;

            foreach (Account a in account)
                acc = a;

            return acc;
        }
    }
}
