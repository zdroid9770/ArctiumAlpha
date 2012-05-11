using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Constans;

namespace Common.Opcodes
{
    public class MessageClient
    {
        public int Message { get; set; }

        public MessageClient(JAMCCMessage mType)
        {
            Calculate(mType);
        }

        public MessageClient(JAMCMessage mType)
        {
            Calculate(mType);
        }

        public MessageClient(ServerMessage mType)
        {
            Calculate(mType);
        }

        public void Calculate<T>(T mType)
        {
            if (mType.GetType() == typeof(JAMCCMessage))
            {
                for (int i = 0; i < 0xFFF; i++)
                    if ((i & 0x774) == 1828 && JAMCCMessageCalc(i) == (int)Convert.ChangeType(mType, typeof(JAMCCMessage)))
                        Message = i;
            }
            else if (mType.GetType() == typeof(JAMCMessage))
            {
                int ret = 0;
                for (int i = 0; i < 0xFFF; i++)
                    if ((i & 0x440) == 64 && (ret = JAMCMessageCalc(i)) == (int)Convert.ChangeType(mType, typeof(JAMCMessage)))
                        if ((ret - 4) <= 0x3E0)
                            Message = i;
            }
            else
            {
                for (int i = 0; i < 0xFFF; i++)
                    if ((i & 0x400) != 64 && ServerMessageCalc(i) == (int)Convert.ChangeType(mType, typeof(ServerMessage)))
                        Message = i;
            }

        }

        public int JAMCCMessageCalc(int value)
        {
            return ((value & 0xF800) >> 7) | value & 3 | ((value & 0x80) >> 4) | ((value & 8) >> 1);
        }

        public int JAMCMessageCalc(int value)
        {
            return value & 0x3F | ((value & 0xF800) >> 2) | ((value & 0x380) >> 1);
        }

        public int ServerMessageCalc(int value)
        {
            return (value & 0x3F | ((value & 0xF800) >> 2) | ((value & 0x380) >> 1));
        }
    }
}
