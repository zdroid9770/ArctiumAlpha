using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Network;
using Common.Network.Packets;
using RealmServer.Authentication;
using Common.Cryptography;
using Common.Account;
using Common.Logging;
using System.Threading;
using System.Net.Sockets;

namespace RealmServer.Network
{
    public class RealmManager
    {
        public static RealmSocket RealmSession;
        public Socket realmSocket;
        public Socket proxySocket;

        public void HandleProxyConnection(RealmManager Session)
        {
            Log.Message();
            Log.Message(LogType.NORMAL, "Begin redirection to WorldServer.");

            PacketWriter proxyWriter = new PacketWriter();
            proxyWriter.WriteBytes(System.Text.Encoding.ASCII.GetBytes("127.0.0.1:8100"));
            proxyWriter.WriteUInt8(0);

            Session.Send(proxyWriter, proxySocket);
            proxySocket.Close();

            Log.Message(LogType.NORMAL, "Successfully redirected to WorldServer");
            Log.Message();
        }
        
        public void HandleRealmList(RealmManager Session)
        {
            PacketWriter realmWriter = new PacketWriter();
            realmWriter.WriteUInt8(1);
            realmWriter.WriteBytes(System.Text.Encoding.ASCII.GetBytes("|cFF00FFFFAlpha Test Realm"));
            realmWriter.WriteUInt8(0);
            realmWriter.WriteBytes(System.Text.Encoding.ASCII.GetBytes("127.0.0.1:9090"));
            realmWriter.WriteUInt8(0);
            realmWriter.WriteUInt32(0);

            Session.Send(realmWriter, realmSocket);
            realmSocket.Close();
        }

        public void RecieveRealm()
        {
            HandleRealmList(this);
        }

        public void RecieveProxy()
        {
            HandleProxyConnection(this);
        }

        public void Send(PacketWriter writer, Socket socket)
        {
            byte[] buffer = writer.ReadDataToSend();

            try
            {
                socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                Log.Message();
                socket.Close();
            }
        }
    }
}
