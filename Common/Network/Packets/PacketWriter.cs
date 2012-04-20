using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Network.Packets
{
    public class PacketWriter : BinaryWriter
    {
        public Opcodes Opcode { get; set; }
        public ushort Size { get; set; }

        public PacketWriter() : base(new MemoryStream()) { }

        public PacketWriter(Opcodes opcode, byte length) : base(new MemoryStream())
        {
            WritePacketHeader(opcode, length);
        }

        protected void WritePacketHeader(Opcodes opcode, byte length)
        {
            // Packet header (0.5.3.3368): Size: 2 bytes + Cmd: 2 bytes
            WriteUInt8(0);
            WriteUInt8((byte)(length));
            WriteUInt8((byte)((uint)opcode % 0x100));
            WriteUInt8((byte)((uint)opcode / 0x100));
        }

        public byte[] ReadDataToSend()
        {
            byte[] data = new byte[BaseStream.Length];
            Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < BaseStream.Length; i++)
                data[i] = (byte)BaseStream.ReadByte();

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
            base.Write(data);
            base.Write((byte)0);    // String null terminated
        }

        public void WriteBytes(byte[] data)
        {
            base.Write(data);
        }
    }
}
