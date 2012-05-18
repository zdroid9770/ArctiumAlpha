using System;
using Common.Database.ObjectDatabase;
using Common.Network.Packets;
using Common.Structs;
using WorldServer.Game.ObjectStore;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class LogoutHandler
    {
        public static void HandleLogoutRequest(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter logoutComplete = new PacketWriter(Opcodes.SMSG_LOGOUT_COMPLETE);
            manager.Send(logoutComplete);

            var result = CharacterObject.GetOnlineCharacter(manager.account);
            result.IsOnline = false;
            ODB.Characters.Save(result);
        }
    }
}
