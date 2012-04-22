using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Constans;
using Common.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class AuthHandler
    {
        public static void HandleAuthSession(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_AUTH_RESPONSE, 1);
            writer.WriteUInt8((byte)AuthCodes.AUTH_OK);

            manager.Send(writer);
            // WTF WHY!?!?!?!?!
            manager.Send(writer);
        }
    }
}
