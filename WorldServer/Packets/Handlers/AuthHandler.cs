using Common.Constans;
using Common.Logging;
using Common.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class AuthHandler
    {
        public static void HandleAuthSession(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_AUTH_RESPONSE);
            writer.WriteUInt8((byte)AuthCodes.AUTH_OK);

            manager.Send(writer);
        }
    }
}
