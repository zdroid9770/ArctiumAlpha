using System;
using Common.Database;
using Common.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class CharHandler
    {
        public static void HandleCharEnum(ref PacketReader packet, ref WorldManager manager)
        {
            SQLResult result = DB.Characters.Select("SELECT guid, name, race, class, gender, skin, face, hairstyle, " +
                                                           "haircolor, facialhair, level, zone, map, x, y, z, guildid, petdisplayid, " + 
                                                           "petlevel, petfamily FROM characters WHERE accountid = 1");

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_ENUM);
            writer.WriteUInt8((byte)result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                writer.WriteUInt64(result.Read<UInt64>(i, 0));

                writer.WriteString(result.Read<String>(i, 1));

                writer.WriteUInt8(result.Read<Byte>(i, 2));
                writer.WriteUInt8(result.Read<Byte>(i, 3));
                writer.WriteUInt8(result.Read<Byte>(i, 4));
                writer.WriteUInt8(result.Read<Byte>(i, 5));
                writer.WriteUInt8(result.Read<Byte>(i, 6));
                writer.WriteUInt8(result.Read<Byte>(i, 7));
                writer.WriteUInt8(result.Read<Byte>(i, 8));
                writer.WriteUInt8(result.Read<Byte>(i, 9));
                writer.WriteUInt8(result.Read<Byte>(i, 10));

                writer.WriteUInt32(result.Read<UInt32>(i, 11));
                writer.WriteUInt32(result.Read<UInt32>(i, 12));

                writer.WriteFloat(result.Read<Single>(i, 13));
                writer.WriteFloat(result.Read<Single>(i, 14));
                writer.WriteFloat(result.Read<Single>(i, 15));

                writer.WriteUInt32(result.Read<UInt32>(i, 16));    // GuildID
                writer.WriteUInt32(result.Read<UInt32>(i, 17));    // PetDisplayId
                writer.WriteUInt32(result.Read<UInt32>(i, 18));    // PetLevel
                writer.WriteUInt32(result.Read<UInt32>(i, 19));    // PetFamily

                // Not implented
                for (int j = 0; j < 20; j++)
                {
                    writer.WriteUInt32(0);    // DisplayId
                    writer.WriteUInt8(0);     // InventoryType
                }
            }

            manager.Send(writer);
        }

        public static void HandleCharCreate(ref PacketReader packet, ref WorldManager manager)
        {
            string name = packet.ReadString();
            byte race = packet.ReadByte();
            byte pClass = packet.ReadByte();
            byte gender = packet.ReadByte();
            byte skin = packet.ReadByte();
            byte face = packet.ReadByte();
            byte hairStyle = packet.ReadByte();
            byte hairColor = packet.ReadByte();
            byte facialHair = packet.ReadByte();
            byte outFitId = packet.ReadByte();

            SQLResult result = DB.Characters.Select("SELECT name from characters");
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_CREATE);

            for (int i = 0; i < result.Count; i++)
            {
                if (result.Read<String>(i, 0) == name)
                {
                    // Name already in use
                    writer.WriteUInt8(0x2B);
                    manager.Send(writer);
                    return;
                }
            }

            DB.Characters.Execute("INSERT INTO characters (name, accountid, race, class, gender, skin, face, hairstyle, haircolor, facialhair) VALUES (" +
                                  "'{0}', 1, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", name, race, pClass, gender, skin, face, hairStyle, hairColor, facialHair);

            // Success
            writer.WriteUInt8(0x28);
            manager.Send(writer);
        }

        public static void HandleCharDelete(ref PacketReader packet, ref WorldManager manager)
        {
            UInt64 guid = packet.ReadUInt64();

            SQLResult result = DB.Characters.Select("SELECT name FROM characters WHERE guid = {0}", guid);

            if (result.Count != 0)
            {
                PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_DELETE);
                writer.WriteUInt8(0x2C);
                manager.Send(writer);

                DB.Characters.Execute("DELETE FROM characters WHERE guid = {0}", guid);
            }

        }

        public static void HandlePlayerLogin(ref PacketReader packet, ref WorldManager manager)
        {

        }
    }
}
