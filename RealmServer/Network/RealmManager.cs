using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Network;
using Common.Network.Packets;
using RealmServer.Authentication;
using Common.Cryptography;
using Common.Account;

namespace RealmServer.Network
{
    public class RealmManager : ServerBase
    {
        Account account;
        Srp6 SRP6;

        public RealmManager()
        {
            account = new Account();
            SRP6 = new Srp6();
        }

        public new void OnData(byte[] data)
        {
            PacketReader loginPackets = new PacketReader(data, false);

            switch ((ClientLink)loginPackets.ReadUInt8())
            {
                case ClientLink.CMD_AUTH_LOGON_CHALLENGE:
                case ClientLink.CMD_AUTH_RECONNECT_CHALLENGE:
                    HandleAuthLogonChallenge(data);
                    break;
                case ClientLink.CMD_AUTH_LOGON_PROOF:
                case ClientLink.CMD_AUTH_RECONNECT_PROOF:
                    HandleAuthLogonProof(data);
                    break;
                case ClientLink.CMD_REALM_LIST:
                    HandleRealmList(data);
                    break;
            }
        }

        public void HandleAuthLogonChallenge(byte[] data)
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteUInt8((byte)ClientLink.CMD_AUTH_LOGON_CHALLENGE);
            writer.WriteUInt8(0);
            writer.WriteUInt8((byte)AuthResults.WOW_SUCCESS);
            writer.WriteBytes(SRP6.B);
            writer.WriteUInt8(1);
            writer.WriteUInt8(SRP6.g);
            writer.WriteUInt8(0x20);
            writer.WriteBytes(SRP6.N);
            writer.WriteBytes(SRP6.Salt);
            writer.WriteBytes(SRP6.RandBytes);
            writer.WriteUInt8(account.GMLevel);

            Send(writer);
        }

        public void HandleAuthLogonProof(byte[] data)
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteUInt8((byte)ClientLink.CMD_AUTH_LOGON_PROOF);
            writer.WriteUInt8(0);

            Send(writer);
        }

        public void HandleRealmList(byte[] data)
        {
            PacketWriter writer = new PacketWriter();

            writer.WriteUInt8(1);
            writer.WriteBytes(System.Text.Encoding.ASCII.GetBytes("|cFF00FFFFAlpha Test Realm"));
            writer.WriteUInt8(0);
            writer.WriteBytes(System.Text.Encoding.ASCII.GetBytes("127.0.0.1:9090"));
            writer.WriteUInt8(0);
            writer.WriteUInt32(0);

            Send(writer);
        }
    }
}
