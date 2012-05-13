using System;
using Common.Database.ObjectDatabase;
using Common.Structs;
using Db4objects.Db4o.Linq;

namespace WorldServer.Game.ObjectStore
{
    public class CharacterObject : Character
    {
        public UInt64 GetGuid()
        {
            var conn = ODB.Characters.Connection;
            var character = from Character c in conn where c.Guid == this.Guid select c;
            UInt64 guid = 0;

            foreach (Character cc in character)
                guid = cc.Guid;

            return guid;
        }
    }
}
