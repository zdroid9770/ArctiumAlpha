using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Network.Packets;
using WorldServer.Network;
using Common.Logging;

namespace WorldServer.Packets.Handlers
{
    public class CharHandler
    {
        public static void HandleCharEnum(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_ENUM, 1);
            writer.WriteUInt8(2);

            manager.Send(writer);
        }
    }
}
