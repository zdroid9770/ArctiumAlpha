using System;
using System.Net.Sockets;
using System.Threading;
using Common.Constans;
using Common.Logging;
using Common.Network.Packets;
using WorldServer.Packets;
using Common.Cryptography;
using System.Collections.Generic;

namespace WorldServer.Network
{
    public class WorldManager
    {
        public Socket socket;
        public static WorldSocket WorldSession;
        public bool IsEncrypted { get; set; }
        public PacketCrypt Crypt;
        byte[] buffer = null;

        public void OnData()
        {
            PacketReader pkt = new PacketReader(buffer);

            if (Enum.IsDefined(typeof(uint), pkt.Opcode))
                Log.Message(LogType.DUMP, "Recieved OPCODE: {0}, LENGTH: {1}", pkt.Opcode, pkt.Size);
            else
                Log.Message(LogType.DUMP, "UNKNOWN OPCODE: {0}, LENGTH: {1}", pkt.Opcode, pkt.Size);

            Log.Message();
            PacketManager.InvokeHandler(pkt, this, (ClientMessage)pkt.Opcode);
            Log.Message();
        }

        public void Recieve()
        {
            PacketWriter TransferInitiate = new PacketWriter((ServerMessage)ClientMessage.TransferInitiate);
            TransferInitiate.WriteString("RLD OF WARCRAFT CONNECTION - SERVER TO CLIENT");

            Send(TransferInitiate);

            while (WorldSession.listenWorldSocket)
            {
                Thread.Sleep(1);
                if (socket.Available > 0)
                {
                    buffer = new byte[socket.Available];
                    socket.Receive(buffer, buffer.Length, SocketFlags.None);

                    //if (IsEncrypted)
                    //    Crypt.Decrypt(buffer, 0, buffer.Length);

                    OnData();
                }
            }

            socket.Close();
        }

        public void Send(PacketWriter packet)
        {
            List<byte> dataList = new List<byte>();

            foreach (byte b in packet.ReadDataToSend())
                dataList.Add(b);

            if (dataList[2] == 0x57 && dataList[3] == 0x4F)
                dataList.RemoveAt(4);

            byte[] buffer = new byte[dataList.Count];
            dataList.CopyTo(buffer);

            try
            {
                //if (IsEncrypted)
                //    Crypt.Encrypt(buffer, 0, buffer.Length);

                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(FinishSend), socket);
                packet.Flush();

                Log.Message(LogType.DUMP, "Send {0}.", packet.Opcode);
                Log.Message();
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
                Log.Message();
                socket.Close();
            }
        }

        public void FinishSend(IAsyncResult result)
        {
            socket.EndSend(result);
        }
    }
}
