using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;

namespace WorldServer.Network
{
    public class RealmSocket
    {
        public bool listenRealmSocket = true;
        private TcpListener realmListener;

        public bool Start()
        {
            try
            {
                realmListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3724);
                realmListener.Start();

                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                Log.Message();

                return false;
            }
        }

        public void StartRealmThread()
        {
            new Thread(AcceptRealmConnection).Start();
        }

        protected void AcceptRealmConnection()
        {
            while (listenRealmSocket)
            {
                Thread.Sleep(1);
                if (realmListener.Pending())
                {
                    RealmManager Realm = new RealmManager();
                    Realm.realmSocket = realmListener.AcceptSocket();

                    Thread NewThread = new Thread(Realm.RecieveRealm);
                    NewThread.Start();
                }
            }
        }

        public void Dispose()
        {
            listenRealmSocket = false;
            realmListener.Stop();
        }
    }
}
