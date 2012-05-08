using System;
using Common.Database;

namespace Common.Structs
{
    public class Character : ObjectBase
    {
        public UInt64 Guid;
        public String Name;
        public Byte Race;
        public Byte Class;
        public Byte Gender;
        public Byte Skin;
        public Byte Face;
        public Byte HairStyle;
        public Byte HairColor;
        public Byte FacialHair;
        public Byte Level;
        public UInt32 Zone;
        public UInt32 Map;
        public Single X;
        public Single Y;
        public Single Z;
        public UInt32 GuildGuid;
        public UInt32 PetDisplayInfo;
        public UInt32 PetLevel;
        public UInt32 PetFamily;
        public Byte OutFitId;
    }
}
