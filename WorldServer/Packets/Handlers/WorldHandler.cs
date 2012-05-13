using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.ObjectDatabase;
using Common.Network.Packets;
using Common.Structs;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class WorldHandler
    {
        public static void HandleUpdateObject(ref PacketReader packet, ref WorldManager manager)
        {
            Character character = new Character();
            UInt64 guid = packet.ReadUInt64();
            var result = ODB.Characters.Select<Character>();

            foreach (Character c in result)
            {
                if (c.Guid == guid)
                {
                    character = c;
                    break;
                }
            }

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_UPDATE_OBJECT);
            writer.WriteUInt32(1);           // ObjectCount
            writer.WriteUInt8(2);            // UpdateType, UPDATE_FULL/CreateObject) + PartialUpdateFromFullUpdate
            writer.WriteUInt64(guid);        // ObjectGuid
            writer.WriteUInt8(4);            // ObjectType, 4 = Player

            writer.WriteUInt64(0);           // TransportGuid
            writer.WriteFloat(0);            // TransportX
            writer.WriteFloat(0);            // TransportY
            writer.WriteFloat(0);            // TransportZ
            writer.WriteFloat(0);            // TransportW (TransportO)

            writer.WriteFloat(character.X);  // x
            writer.WriteFloat(character.Y);  // y
            writer.WriteFloat(character.Z);  // z
            writer.WriteFloat(character.O);  // w (o)

            writer.WriteFloat(0);            // Pitch

            writer.WriteUInt32(0x08000000);  // MovementFlagMask
            writer.WriteUInt32(0);           // FallTime

            writer.WriteFloat(2.5f);         // WalkSpeed
            writer.WriteFloat(7.0f);         // RunSpeed
            writer.WriteFloat(4.7222f);      // SwimSpeed
            writer.WriteFloat(3.14f);        // TurnSpeed

            writer.WriteUInt32(1);           // Flags, 1 - Player
            writer.WriteUInt32(1);           // AttackCycle
            writer.WriteUInt32(0);           // TimerId
            writer.WriteUInt64(0);           // VictimGuid

            // FillInPartialObjectData
            writer.WriteUInt8(0x14);         // UpdateMaskBlocks, 20

            for (int i = 0; i < 0x14; i++)
                writer.WriteUInt32(0xFFFFFFFF);

            // ObjectFields
            writer.WriteUInt64(guid);
            writer.WriteUInt32(0x19);         // UpdateType, 0x19 - Player (Player + Unit + Object)
            writer.WriteUInt32(0);
            writer.WriteFloat(1);
            writer.WriteUInt32(0);

            // UnitFields
            for (int i = 0; i < 16; i++)
                writer.WriteUInt32(0);

            writer.WriteUInt32(character.Health);
            writer.WriteUInt32(character.Mana);
            writer.WriteUInt32(character.Rage);
            writer.WriteUInt32(character.Focus);
            writer.WriteUInt32(character.Energy);
            // Max values...
            writer.WriteUInt32(character.Health);
            writer.WriteUInt32(character.Mana);
            writer.WriteUInt32(character.Rage);
            writer.WriteUInt32(character.Focus);
            writer.WriteUInt32(character.Energy);
            writer.WriteUInt32(character.Level);
            writer.WriteUInt32(5);
            writer.WriteUInt8(character.Race);
            writer.WriteUInt8(character.Class);
            writer.WriteUInt8(character.Gender);
            writer.WriteUInt8(1);

            writer.WriteUInt32(character.Strength);
            writer.WriteUInt32(character.Agility);
            writer.WriteUInt32(character.Stamina);
            writer.WriteUInt32(character.Intellect);
            writer.WriteUInt32(character.Spirit);
            // BastStats, copy ...
            writer.WriteUInt32(character.Strength);
            writer.WriteUInt32(character.Agility);
            writer.WriteUInt32(character.Stamina);
            writer.WriteUInt32(character.Intellect);
            writer.WriteUInt32(character.Spirit);

            for (int i = 0; i < 10; i++)
                writer.WriteUInt32(0);

            // Money
            writer.WriteUInt32(character.Money);

            for (int i = 0; i < 56; i++)
                writer.WriteUInt32(0);

            for (int i = 0; i < 39; i++)
                writer.WriteUInt32(0);

            // DisplayId
            switch (character.Race)
            {
                case 1:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x31 : 0x32));
                    break;
                case 2:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x33 : 0x34));
                    break;
                case 3:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x35 : 0x36));
                    break;
                case 4:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x37 : 0x38));
                    break;
                case 5:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x39 : 0x40));
                    break;
                case 6:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x3B : 0x3C));
                    break;
                case 7:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x61B : 0x61C));
                    break;
                case 8:
                    writer.WriteUInt32((uint)(character.Gender == 0 ? 0x5C6 : 0x5C7));
                    break;
            }

            for (int i = 0; i < 32; i++)
                writer.WriteUInt32(0);

            // PlayerFields
            for (int i = 0; i < 46; i++)
                writer.WriteUInt32(0);

            for (int i = 0; i < 32; i++)
                writer.WriteUInt32(0);

            for (int i = 0; i < 48; i++)
                writer.WriteUInt32(0);

            for (int i = 0; i < 12; i++)
                writer.WriteUInt32(0);

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            // InventarSlots
            writer.WriteUInt32(14);

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            // PLAYER_BYTES (Skin, Face, HairStyle, HairColor)
            writer.WriteUInt8(character.Skin);
            writer.WriteUInt8(character.Face);
            writer.WriteUInt8(character.HairStyle);
            writer.WriteUInt8(character.HairColor);

            // XP
            writer.WriteUInt32(0);
            // NextLevel
            writer.WriteUInt32(200);

            // SkillInfo
            for (int i = 0; i < 192; i++)
                writer.WriteUInt32(0);

            // PLAYER_BYTES_2 (FacialHair, Unknown, BankBagSlotCount, RestState)
            writer.WriteUInt8(character.FacialHair);
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            writer.WriteUInt8(1);

            // QuestInfo
            for (int i = 0; i < 96; i++)
                writer.WriteUInt32(0);

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            // BaseMana
            writer.WriteUInt32(20);
            writer.WriteUInt32(0);

            // Hmm xD?
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            writer.WriteUInt8(0);
            manager.Send(writer);
        }
    }
}
