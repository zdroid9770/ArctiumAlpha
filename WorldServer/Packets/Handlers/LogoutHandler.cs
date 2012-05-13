using System;
using Common.Database.ObjectDatabase;
using Common.Network.Packets;
using Common.Structs;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class LogoutHandler
    {
        public static void HandleLogoutRequest(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter logoutComplete = new PacketWriter(Opcodes.SMSG_LOGOUT_COMPLETE);
            manager.Send(logoutComplete);
        }
    }
}
