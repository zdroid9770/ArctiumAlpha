using Common.Constans;
using Common.Logging;
using Common.Network.Packets;
using WorldServer.Network;
using Common.Account;

namespace WorldServer.Packets.Handlers
{
    public class AuthHandler
    {
        public static void HandleAuthSession(ref PacketReader packet, ref WorldManager manager)
        {
            packet.ReadUInt32();
            packet.ReadUInt32();

            string name = packet.ReadAccountName();
            Account account = Account.GetAccountByName(name);

            PacketWriter writer = new PacketWriter(Opcodes.SMSG_AUTH_RESPONSE);

            if (account == null)
                writer.WriteUInt8((byte)AuthCodes.AUTH_UNKNOWN_ACCOUNT);
            else
            {
                manager.account = account;
                writer.WriteUInt8((byte)AuthCodes.AUTH_OK);
            }

            manager.Send(writer);
        }
    }
}
