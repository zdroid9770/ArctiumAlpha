using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Network.Packets;
using WorldServer.Network;
using Common.Logging;
using System.Threading;

namespace WorldServer.Packets.Handlers
{
    public class CharHandler
    {
        public static void HandleCharEnum(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_CHAR_ENUM, 14 + 45 + 100);
            writer.WriteUInt8(1);

            //for (int i = 0; i <= 1; i++)
            //{
            writer.WriteUInt64(1);

            writer.WriteString("Fabi");

                writer.WriteUInt8(1);
                writer.WriteUInt8(1);
                writer.WriteUInt8(0);
                writer.WriteUInt8(1);
                writer.WriteUInt8(1);
                writer.WriteUInt8(1);
                writer.WriteUInt8(1);
                writer.WriteUInt8(1);
                writer.WriteUInt8(1);

                writer.WriteUInt32(0);
                writer.WriteUInt32(0);

                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);

                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);

                for (int j = 0; j < 20; j++)
                {
                    writer.WriteUInt32(0);
                    writer.WriteUInt8(0);
                }
            //}

            manager.Send(writer);
        }
    }
}
