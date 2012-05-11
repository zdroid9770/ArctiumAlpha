using System;
using System.IO;
using System.Text;
using Common.Constans;
using Common.Logging;
using Common.Opcodes;

namespace Common.Network.Packets
{
    public class PacketWriter : BinaryWriter
    {
        public uint Opcode { get; set; }
        public ushort Size { get; set; }
        public bool IsWorldOpcode { get { return Opcode == (uint)ClientMessage.AuthSession; } }

        public PacketWriter() : base(new MemoryStream()) { }
        public PacketWriter(JAMCCMessage mType, bool isWorldPacket = true) : base(new MemoryStream())
        {
            MessageClient message = new MessageClient(mType);
            SetMessageAndHeader((uint)message.Message, isWorldPacket);
        }

        public PacketWriter(JAMCMessage mType, bool isWorldPacket = true) : base(new MemoryStream())
        {
            MessageClient message = new MessageClient(mType);
            SetMessageAndHeader((uint)message.Message, isWorldPacket);
        }

        public PacketWriter(ServerMessage mType, bool isWorldPacket = true) : base(new MemoryStream())
        {
            MessageClient message = new MessageClient(mType);
            SetMessageAndHeader((uint)message.Message, isWorldPacket);
        }

        public PacketWriter(Message message, bool isWorldPacket = true) : base(new MemoryStream())
        {
            SetMessageAndHeader((uint)message, isWorldPacket);
        }

        void SetMessageAndHeader(uint mess, bool isWorldPacket)
        {
            Opcode = mess;
            WritePacketHeader(mess, isWorldPacket);
        }

        protected void WritePacketHeader(uint opcode, bool isWorldPacket = true)
        {
            WriteUInt8(0);
            WriteUInt8(0);
            WriteUInt8((byte)((uint)opcode % 0x100));
            WriteUInt8((byte)((uint)opcode / 0x100));

            if (isWorldPacket)
            {
                WriteUInt8(0);
                WriteUInt8(0);
            }
        }

        public byte[] ReadDataToSend(bool isAuthPacket = false)
        {
            byte[] data = new byte[BaseStream.Length];
            Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < BaseStream.Length; i++)
                data[i] = (byte)BaseStream.ReadByte();

            Size = (ushort)(data.Length - 2);
            if (!isAuthPacket)
            {
                // MoP Beta other Endian...
                data[0] = (byte)(Size % 0x100);
                data[1] = (byte)(Size / 0x100);
            }

            return data;
        }

        public void WriteInt8(sbyte data)
        {
            base.Write(data);
        }

        public void WriteInt16(short data)
        {
            base.Write(data);
        }

        public void WriteInt32(int data)
        {
            base.Write(data);
        }

        public void WriteInt64(long data)
        {
            base.Write(data);
        }

        public void WriteUInt8(byte data)
        {
            base.Write(data);
        }

        public void WriteUInt16(ushort data)
        {
            base.Write(data);
        }

        public void WriteUInt32(uint data)
        {
            base.Write(data);
        }

        public void WriteUInt64(ulong data)
        {
            base.Write(data);
        }

        public void WriteFloat(float data)
        {
            base.Write(data);
        }

        public void WriteDouble(double data)
        {
            base.Write(data);
        }

        public void WriteString(string data)
        {
            byte[] sBytes = Encoding.ASCII.GetBytes(data);
            this.WriteBytes(sBytes);
            base.Write((byte)0);    // String null terminated
        }

        public void WriteBytes(byte[] data)
        {
            base.Write(data);
        }
    }
}
