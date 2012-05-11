using System;

namespace Common.Network.Packets
{
    public class BitPack
    {
        PacketWriter writer;

        byte BitPosition { get; set; }
        byte BitValue { get; set; }

        public BitPack(PacketWriter writer)
        {
            this.writer = writer;
            BitPosition = 8;
        }

        public void Write<T>(T bit)
        {
            --BitPosition;

            if (Convert.ToBoolean(bit))
                BitValue |= (byte)(1 << (BitPosition));

            if (BitPosition == 0)
            {
                BitPosition = 8;
                writer.WriteUInt8(BitValue);
                BitValue = 0;
            }
        }

        public void Flush()
        {
            if (BitPosition == 8)
                return;

            writer.WriteUInt8(BitValue);
            BitValue = 0;
            BitPosition = 8;
        }
    }
}
