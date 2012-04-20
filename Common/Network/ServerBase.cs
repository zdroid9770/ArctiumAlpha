using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Common.Network.Packets;
using Common.Logging;

namespace Common.Network
{
    public abstract class ServerBase : IDisposable
    {
        bool IsConnected { get; set; }
        bool IsWorldServer { get; set; }
        byte[] IncomingBuffer { get; set; }
        byte[] OutgoingBuffer { get; set; }

        TcpListener listener;
        Socket socket;

        public bool Start(string address, int port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse(address), port);
                listener.Start();
                IsConnected = true;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void StartConnectionThread()
        {
            new Thread(AcceptConnection).Start();
        }

        private void AcceptConnection()
        {
            while (IsConnected)
            {
                if (listener.Pending())
                {
                    socket = listener.AcceptSocket();
                    new Thread(Recieve).Start();
                }
            }
        }

        public void Recieve()
        {
            Log.Message(LogType.NORMAL, "Incoming connection...");

            while (IsConnected)
            {
                if (socket.Available != 0)
                {
                    IncomingBuffer = new byte[socket.Available];
                    socket.Receive(IncomingBuffer, IncomingBuffer.Length, SocketFlags.None);

                    OnData(IncomingBuffer);
                }
            }

            socket.Close();
            Dispose();
        }

        public void Send(PacketWriter writer)
        {
            OutgoingBuffer = writer.ReadDataToSend();

            try
            {
                socket.Send(OutgoingBuffer, 0, OutgoingBuffer.Length, SocketFlags.None);
            }
            catch
            {
                socket.Close();
            }
        }

        public void OnData(byte[] data)
        {
            OnData();
        }

        public void OnData() { }

        public void Dispose()
        {
            IsConnected = false;
            listener.Stop();
        }
    }
}
