using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common.Cryptography
{
    public class PacketCrypt
    {
        public const int DropN = 1024;

        private static readonly byte[] ServerDecryptionKey = { 0x40, 0xAA, 0xD3, 0x92, 0x26, 0x71, 0x43, 0x47, 0x3A, 0x31, 0x08, 0xA6, 0xE7, 0xDC, 0x98, 0x2A };
        private static readonly byte[] ServerEncryptionKey = { 0x08, 0xF1, 0x95, 0x9F, 0x47, 0xE5, 0xD2, 0xDB, 0xA1, 0x3D, 0x77, 0x8F, 0x3F, 0x3E, 0xE7, 0x00 };

        static readonly HMACSHA1 s_decryptClientDataHMAC = new HMACSHA1(ServerDecryptionKey);
        static readonly HMACSHA1 s_encryptServerDataHMAC = new HMACSHA1(ServerEncryptionKey);

        private readonly ARC4 encryptServerData;
        private readonly ARC4 decryptClientData;

        public PacketCrypt(byte[] sessionKey)
        {
            var encryptHash = s_encryptServerDataHMAC.ComputeHash(sessionKey);
            var decryptHash = s_decryptClientDataHMAC.ComputeHash(sessionKey);

            decryptClientData = new ARC4(decryptHash);
            encryptServerData = new ARC4(encryptHash);

            var syncBuffer = new byte[DropN];
            encryptServerData.Process(syncBuffer, 0, syncBuffer.Length);

            syncBuffer = new byte[DropN];
            decryptClientData.Process(syncBuffer, 0, syncBuffer.Length);
        }

        public void Decrypt(byte[] data, int start, int count)
        {
            decryptClientData.Process(data, start, count);
        }

        public void Encrypt(byte[] data, int start, int count)
        {
            encryptServerData.Process(data, start, count);
        }
    }
}
