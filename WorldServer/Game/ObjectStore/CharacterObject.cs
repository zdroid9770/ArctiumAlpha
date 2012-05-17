using System;
using Common.Account;
using Common.Database.ObjectDatabase;
using Common.Structs;
using Db4objects.Db4o.Linq;
using System.Collections.Generic;

namespace WorldServer.Game.ObjectStore
{
    public class CharacterObject : Character
    {
        public static Character GetCharacterByGuid(UInt64 charGuid)
        {
            Character chara = null;
            var conn = ODB.Characters.Connection;
            var character = from Character c in conn where c.Guid == charGuid select c;

            foreach (Character cc in character)
                chara = cc;

            return chara;
        }

        public static Character[] GetCharactersByAccount(Account acc)
        {
            var conn = ODB.Characters.Connection;
            var character = from Character c in conn where c.AccountId == acc.Id select c;
            List<Character> chars = new List<Character>();

            foreach (Character cc in character)
                chars.Add(cc);

            return chars.ToArray();
        }
    }
}
