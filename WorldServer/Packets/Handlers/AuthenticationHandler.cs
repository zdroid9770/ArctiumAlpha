using System;
using Common.Constans;
using Common.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class AuthenticationHandler
    {
        public static void HandleTransferInitiate(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter authChallenge = new PacketWriter(JAMCCMessage.AuthChallenge);
            for (int i = 0; i < 8; i++)
                authChallenge.WriteUInt32(0);

            authChallenge.WriteUInt8(1);
            authChallenge.WriteUInt32((uint)new Random(Environment.TickCount).Next());

            manager.Send(authChallenge);
        }

        public static void HandleAuthSession(ref PacketReader packet, ref WorldManager manager)
        {
            // ToDo: Parse AuthSession
            manager.IsEncrypted = true;

            // ToDo: AuthResponse struct
            PacketWriter authResponse = new PacketWriter(JAMCMessage.AuthResponse);
            BitPack BitPack = new BitPack(authResponse);

            manager.Send(authResponse);
        }
    }
}
