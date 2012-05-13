using System;
using Common.Database.ObjectDatabase;
using Common.Network.Packets;
using Common.Structs;
using WorldServer.Network;
using WorldServer.Game.ObjectStore;

namespace WorldServer.Packets.Handlers
{
    public class CacheHandler
    {
        public static void HandleNameCache(ref PacketReader packet, ref WorldManager manager)
        {
            UInt64 guid = packet.ReadUInt64();
            Character character = CharacterObject.GetCharacterByGuid(guid);

            PacketWriter nameCache = new PacketWriter(Opcodes.SMSG_NAME_QUERY_RESPONSE);
            nameCache.WriteUInt64(guid);
            nameCache.WriteString(character.Name);
            nameCache.WriteUInt32(character.Race);
            nameCache.WriteUInt32(character.Gender);
            nameCache.WriteUInt32(character.Class);
            nameCache.WriteUInt8(0);
            manager.Send(nameCache);
        }
    }
}
