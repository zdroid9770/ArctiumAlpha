using System.Collections.Generic;
using Common.Constans;
using Common.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Packets
{
    public static class PacketManager
    {
        public static Dictionary<ClientMessage, HandlePacket> OpcodeHandlers = new Dictionary<ClientMessage, HandlePacket>();
        public delegate void HandlePacket(ref PacketReader packet, ref WorldManager manager);

        public static void DefineOpcodeHandler(ClientMessage opcode, HandlePacket handler)
        {
            OpcodeHandlers[opcode] = handler;
        }

        public static bool InvokeHandler(PacketReader reader, WorldManager manager, ClientMessage opcode)
        {
            if (OpcodeHandlers.ContainsKey(opcode))
            {
                OpcodeHandlers[opcode].Invoke(ref reader, ref manager);
                return true;
            }
            else
                return false;
        }
    }
}
