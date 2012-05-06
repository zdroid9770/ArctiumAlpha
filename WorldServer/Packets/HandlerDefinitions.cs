using WorldServer.Packets.Handlers;

namespace WorldServer.Packets
{
    public class HandlerDefinitions
    {
        public static void InitializePacketHandler()
        {
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AUTH_SESSION, AuthHandler.HandleAuthSession);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_ENUM, CharHandler.HandleCharEnum);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_CREATE, CharHandler.HandleCharCreate);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_DELETE, CharHandler.HandleCharDelete);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_PING, NetHandler.HandlePing);
        }
    }
}
