using System;
using Common.Database.ObjectDatabase;
using Common.Network.Packets;
using Common.Structs;
using WorldServer.Network;
using WorldServer.Game.ObjectStore;

namespace WorldServer.Packets.Handlers
{
    public class ChatHandler
    {
        public static void HandleMessageChat(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter welcomeMessage = new PacketWriter(Opcodes.SMSG_MESSAGECHAT);
            welcomeMessage.WriteUInt8((byte)packet.ReadInt32());     // slashCmd, 9: SystemMessage
            welcomeMessage.WriteUInt32(packet.ReadUInt32());         // Language: General
            welcomeMessage.WriteUInt64(2);                           // Guid: 0 - ToAll???
            welcomeMessage.WriteString(packet.ReadString());
            welcomeMessage.WriteUInt8(0);                            // afkDND, 0: nothing

            manager.Send(welcomeMessage);
        }
    }
}
