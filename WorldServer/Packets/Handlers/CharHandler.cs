using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Network.Packets;
using WorldServer.Network;
using Common.Logging;
using System.Threading;
using Common.Database;
using System.Data;
using System.Data.SQLite;

namespace WorldServer.Packets.Handlers
{
    public class CharHandler
    {
        public static void HandleCharEnum(ref PacketReader packet, ref WorldManager manager)
        {
            DataTable result = DB.Characters.Select("SELECT guid, name, race, class, gender, skin, face, hairstyle, " +
                                                           "haircolor, facialhair, level, zone, map, x, y, z, guildid, petdisplayid, " + 
                                                           "petlevel, petfamily FROM characters WHERE accountid = 1");

            byte Count = (byte)result.Rows.Count;

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_ENUM);
            writer.WriteUInt8(Count);

            for (int i = 0; i < Count; i++)
            {
                writer.WriteUInt64(Convert.ToUInt64(result.Rows[i].ItemArray[0]));

                writer.WriteString((string)result.Rows[i].ItemArray[1]);

                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[2]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[3]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[4]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[5]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[6]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[7]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[8]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[9]));
                writer.WriteUInt8(Convert.ToByte(result.Rows[i].ItemArray[10]));

                writer.WriteUInt32(Convert.ToUInt32(result.Rows[i].ItemArray[11]));
                writer.WriteUInt32(Convert.ToUInt32(result.Rows[i].ItemArray[12]));

                writer.WriteFloat(Convert.ToSingle(result.Rows[i].ItemArray[13]));
                writer.WriteFloat(Convert.ToSingle(result.Rows[i].ItemArray[14]));
                writer.WriteFloat(Convert.ToSingle(result.Rows[i].ItemArray[15]));

                writer.WriteUInt32(Convert.ToUInt32(result.Rows[i].ItemArray[16]));    // GuildID
                writer.WriteUInt32(Convert.ToUInt32(result.Rows[i].ItemArray[17]));    // PetDisplayId
                writer.WriteUInt32(Convert.ToUInt32(result.Rows[i].ItemArray[18]));    // PetLevel
                writer.WriteUInt32(Convert.ToUInt32(result.Rows[i].ItemArray[19]));    // PetFamily

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

            DataTable result = DB.Characters.Select("SELECT name from characters");

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_CREATE);

            int Count = result.Rows.Count;
            for (int i = 0; i < Count; i++)
            {
                if (result.Rows[i].ItemArray[0].ToString() == name)
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
    }
}
