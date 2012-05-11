using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Account;
using Common.Authentication;
using Common.Cryptography;
using Common.Logging;
using Common.Network.Packets;

namespace WorldServer.Network
{
    public class RealmManager
    {
        Account Account { get; set; }
        byte[] RealmBuffer { get; set; }
        public static RealmSocket RealmSession;
        public SRP6 SecureRemotePassword { get; set; }
        public Socket realmSocket;

        void HandleRealmData(byte[] data)
        {
            PacketReader reader = new PacketReader(data, false);

            switch ((ClientLink)reader.ReadUInt8())
            {
                case ClientLink.CMD_AUTH_LOGON_CHALLENGE:
                case ClientLink.CMD_AUTH_RECONNECT_CHALLENGE:
                    HandleAuthLogonChallenge(this, reader);
                    break;
                case ClientLink.CMD_AUTH_LOGON_PROOF:
                case ClientLink.CMD_AUTH_RECONNECT_PROOF:
                    HandleAuthLogonProof(this, reader);
                    break;
                case ClientLink.CMD_REALM_LIST:
                    HandleRealmList(this, reader);
                    break;
            }
        }

        // ToDo: Fix SRP6 things...
        public void HandleAuthLogonChallenge(RealmManager Session, PacketReader ClientData)
        {
            Account = new Account();
            SecureRemotePassword = new SRP6();

            ClientData.SkipBytes(10);
            ushort ClientBuild = ClientData.ReadUInt16();
            ClientData.SkipBytes(8);
            Account.Language = ClientData.ReadStringFromBytes(4);
            ClientData.SkipBytes(4);

            Account.IP = ClientData.ReadIPAddress();
            Account.Name = ClientData.ReadAccountName();
            Account.Password = "admin";

            AuthResults? results = null;

            if (Account.Name != "ADMIN")
                results = AuthResults.WOW_FAIL_UNKNOWN_ACCOUNT;
            else
                results = AuthResults.WOW_SUCCESS;

            byte[] username = Encoding.ASCII.GetBytes(Account.Name.ToUpper());
            byte[] password = Encoding.ASCII.GetBytes(Account.Password.ToUpper());

            PacketWriter logonChallenge = new PacketWriter();
            logonChallenge.WriteUInt8((byte)ClientLink.CMD_AUTH_LOGON_CHALLENGE);
            logonChallenge.WriteUInt8(0);

            // Latest MoP Beta build
            if (ClientBuild == 15668)
            {
                switch (results)
                {
                    case AuthResults.WOW_SUCCESS:
                    {
                        Session.SecureRemotePassword.CalculateX(username, password);
                        byte[] buf = new byte[0x10];
                        SRP6.RAND_bytes(buf, 0x10);

                        logonChallenge.WriteUInt8((byte)AuthResults.WOW_SUCCESS);
                        logonChallenge.WriteBytes(Session.SecureRemotePassword.B);
                        logonChallenge.WriteUInt8(1);
                        logonChallenge.WriteUInt8(Session.SecureRemotePassword.g[0]);
                        logonChallenge.WriteUInt8(0x20);
                        logonChallenge.WriteBytes(Session.SecureRemotePassword.N);
                        logonChallenge.WriteBytes(Session.SecureRemotePassword.salt);
                        logonChallenge.WriteBytes(buf);
                        logonChallenge.WriteUInt8(Account.GMLevel);
                        break;
                    }
                    case AuthResults.WOW_FAIL_UNKNOWN_ACCOUNT:
                    {
                        logonChallenge.WriteUInt8((byte)AuthResults.WOW_FAIL_UNKNOWN_ACCOUNT);
                        break;
                    }
                }
            }

            Session.Send(logonChallenge);
        }

        public void HandleAuthLogonProof(RealmManager Session, PacketReader ClientData)
        {
            PacketWriter logonProof = new PacketWriter();

            byte[] a = new byte[32];
            byte[] m1 = new byte[20];

            Array.Copy(RealmBuffer, 1, a, 0, 32);
            Array.Copy(RealmBuffer, 33, m1, 0, 20);

            Session.SecureRemotePassword.CalculateU(a);
            Session.SecureRemotePassword.CalculateM2(m1);
            Session.SecureRemotePassword.CalculateK();

            Account.SessionKey = Session.SecureRemotePassword.K;

            logonProof.WriteUInt8((byte)ClientLink.CMD_AUTH_LOGON_PROOF);
            logonProof.WriteUInt8(0);
            logonProof.WriteBytes(Session.SecureRemotePassword.M2);
            logonProof.WriteUInt32(0x800000);
            logonProof.WriteUInt32(0);
            logonProof.WriteUInt16(0);

            Session.Send(logonProof);
        }

        public void HandleRealmList(RealmManager Session, PacketReader ClientData)
        {
            PacketWriter realmData = new PacketWriter();
            realmData.WriteUInt8(1);
            realmData.WriteUInt8(0);
            realmData.WriteUInt8(0);
            realmData.WriteString("Arctium MoP Beta Realm");
            realmData.WriteUInt8(0);
            realmData.WriteString("127.0.0.1:8100");
            realmData.WriteUInt8(0);
            realmData.WriteUInt32(0);
            realmData.WriteUInt8(0);
            realmData.WriteUInt8(1);
            realmData.WriteUInt8(0x2C);
            realmData.WriteUInt8(0x10);
            realmData.WriteUInt8(0);

            PacketWriter realmList = new PacketWriter();
            realmList.WriteUInt8((byte)ClientLink.CMD_REALM_LIST);
            realmList.WriteUInt16((ushort)(realmData.BaseStream.Length + 6));
            realmList.WriteUInt32(0);
            realmList.WriteUInt16(1);
            realmList.WriteBytes(realmData.ReadDataToSend());

            Session.Send(realmList);
        }

        public void RecieveRealm()
        {
            byte[] buffer = null;

            while (RealmSession.listenRealmSocket)
            {
                Thread.Sleep(200);
                if (realmSocket.Available > 0)
                {
                    buffer = new byte[realmSocket.Available];
                    realmSocket.Receive(buffer, buffer.Length, SocketFlags.None);

                    RealmBuffer = buffer;
                    HandleRealmData(RealmBuffer);
                }
            }

            realmSocket.Close();
        }

        public void Send(PacketWriter writer)
        {
            byte[] buffer = writer.ReadDataToSend(true);

            try
            {
                realmSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                Log.Message();
                realmSocket.Close();
            }
        }
    }
}
