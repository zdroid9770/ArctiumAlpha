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
        public Byte Level = 1;
        public UInt32 Zone;
        public UInt32 Map;
        public Single X;
        public Single Y;
        public Single Z;
        public UInt32 GuildGuid;
        public UInt32 PetDisplayInfo;
        public UInt32 PetLevel;
        public UInt32 PetFamily;
        public UInt32 Health = 52;
        public UInt32 Mana = 10;
        public UInt32 Rage = 20;
        public UInt32 Focus = 30;
        public UInt32 Energy = 40;
        public UInt32 Strength = 1;
        public UInt32 Agility = 2;
        public UInt32 Stamina = 3;
        public UInt32 Intellect = 4;
        public UInt32 Spirit = 5;
        public UInt32 Money = 50;
    }
}
