using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldServer.Packets.Handlers;

namespace WorldServer.Packets
{
    public class HandlerDefinitions
    {
        public static void InitializePacketHandler()
        {
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AUTH_SESSION, AuthHandler.HandleAuthSession);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_ENUM, CharHandler.HandleCharEnum);
        }
    }
}
