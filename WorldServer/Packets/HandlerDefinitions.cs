using Common.Constans;
using WorldServer.Packets.Handlers;

namespace WorldServer.Packets
{
    public class HandlerDefinitions
    {
        public static void InitializePacketHandler()
        {
            PacketManager.DefineOpcodeHandler(ClientMessage.TransferInitiate, AuthenticationHandler.HandleTransferInitiate);
            PacketManager.DefineOpcodeHandler(ClientMessage.AuthSession, AuthenticationHandler.HandleAuthSession);
        }
    }
}
