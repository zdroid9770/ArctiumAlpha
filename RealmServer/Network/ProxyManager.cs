using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Network;
using Common.Network.Packets;

namespace RealmServer.Network
{
    public class ProxyManager : ServerBase
    {
        new void OnData()
        {
            HandleRealmList();
        }

        void HandleRealmList()
        {
            PacketWriter writer = new PacketWriter();

            writer.WriteBytes(System.Text.Encoding.ASCII.GetBytes("127.0.0.1:8100"));
            writer.WriteUInt8(0);

            Send(writer);
        }
    }
}
