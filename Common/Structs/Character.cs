using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structs
{
    public struct Character
    {
        UInt64 Guid;
        String Name;
        Byte Race;
        Byte Class;
        Byte Gender;
        Byte Skin;
        Byte Face;
        Byte HairStyle;
        Byte HairColor;
        Byte FacialHair;
        Byte Level;
        UInt32 Zone;
        UInt32 Map;
        Single X;
        Single Y;
        Single Z;
        UInt32 GuildGuid;
        UInt32 PetDisplayInfo;
        UInt32 PetLevel;
        UInt32 PetFamily;
        Byte OutFitId;
    }
}
