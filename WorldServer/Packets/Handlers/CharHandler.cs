using System;
using Common.Database.ObjectDatabase;
using Common.Network.Packets;
using Common.Structs;
using WorldServer.Game.ObjectStore;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class CharHandler
    {
        public static void HandleCharEnum(ref PacketReader packet, ref WorldManager manager)
        {
            var result = ODB.Characters.Select<Character>();

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_ENUM);
            writer.WriteUInt8((byte)ODB.Characters.RowCount);

            foreach (Character c in result)
            {
                writer.WriteUInt64(c.Guid);
                Console.WriteLine("GUID: {0}", c.Guid);

                writer.WriteString(c.Name);

                writer.WriteUInt8(c.Race);
                writer.WriteUInt8(c.Class);
                writer.WriteUInt8(c.Gender);
                writer.WriteUInt8(c.Skin);
                writer.WriteUInt8(c.Face);
                writer.WriteUInt8(c.HairStyle);
                writer.WriteUInt8(c.HairStyle);
                writer.WriteUInt8(c.FacialHair);
                writer.WriteUInt8(c.Level);

                writer.WriteUInt32(c.Zone);
                writer.WriteUInt32(c.Map);

                writer.WriteFloat(c.X);
                writer.WriteFloat(c.Y);
                writer.WriteFloat(c.Z);

                writer.WriteUInt32(c.GuildGuid);
                writer.WriteUInt32(c.PetDisplayInfo);
                writer.WriteUInt32(c.PetLevel);
                writer.WriteUInt32(c.PetFamily);

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
            Character cha = new Character();
            cha.Name = packet.ReadString();
            cha.Race = packet.ReadByte();
            cha.Class = packet.ReadByte();
            cha.Gender = packet.ReadByte();
            cha.Skin = packet.ReadByte();
            cha.Face = packet.ReadByte();
            cha.HairStyle = packet.ReadByte();
            cha.HairColor = packet.ReadByte();
            cha.FacialHair = packet.ReadByte();
            packet.ReadByte();

            var result = ODB.Characters.Select<Character>();
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_CREATE);

            foreach (Character c in result)
            {
                if (c.Name == cha.Name)
                {
                    // Name already in use
                    writer.WriteUInt8(0x2B);
                    manager.Send(writer);
                    return;
                }
            }

            cha.Guid = (ulong)(ODB.Characters.RowCount + 1);
            ODB.Characters.Save(cha);

            // Success
            writer.WriteUInt8(0x28);
            manager.Send(writer);
        }

        public static void HandleCharDelete(ref PacketReader packet, ref WorldManager manager)
        {
            UInt64 guid = packet.ReadUInt64();
            Character character = CharacterObject.GetCharacterByGuid(guid);

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_DELETE);
            writer.WriteUInt8(0x2C);
            manager.Send(writer);

            ODB.Characters.Delete(character);
        }
    }
}
