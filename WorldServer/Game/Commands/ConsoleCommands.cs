using System;
using System.Security.Cryptography;
using System.Text;
using Common.Account;
using Common.Commands;
using Common.Database.ObjectDatabase;

namespace WorldServer.Game.Commands
{
    public class ConsoleCommands : CommandParser
    {
        public static void CreateAccount(string[] args)
        {
            string name = Read<string>(args, 0);
            string password = Read<string>(args, 1);

            if (name == null || password == null)
                return;

            byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(password));
            string hashString = BitConverter.ToString(hash).Replace("-", "");

            Account acc = new Account();

            acc.Name = name;
            acc.Password = hashString;
            acc.Language = "enUS";
            acc.GMLevel = 3;

            var result = ODB.Realms.Select<Account>();
            if (ODB.Realms.RowCount == 0)
                ODB.Realms.Save(acc);

            foreach (Account a in result)
            {
                if (a.Name != name)
                {
                    ODB.Realms.Save(acc);
                    break;
                }
            }              
        }
    }
}
