using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class WorldHandler
    {
        public static void HandleUpdateObject(ref PacketReader packet, ref WorldManager manager)
        {
            UInt64 guid = packet.ReadUInt64();
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_UPDATE_OBJECT);

            writer.WriteInt8(0);
            writer.WriteUInt64(guid);

            writer.WriteUInt64(0);
            writer.WriteFloat(0);
            writer.WriteFloat(0);
            writer.WriteFloat(0);
            writer.WriteFloat(0);

            writer.WriteFloat(-8913.23f);    // x
            writer.WriteFloat(554.633f);    // y
            writer.WriteFloat(93.7944f);    // z
            writer.WriteFloat(0);    // o

            writer.WriteFloat(0);

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            writer.WriteFloat(2.5f);
            writer.WriteFloat(7.0f);
            writer.WriteFloat(4.7222f);
            writer.WriteFloat(3.54f);

            manager.Send(writer);
        }
    }
}
