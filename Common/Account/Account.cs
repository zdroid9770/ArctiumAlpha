using System;
namespace Common.Account
{
    [Serializable()]
    public class Account
    {
        public string Name;
        public string Password;
        public byte[] SessionKey;
        public string IP;
        public byte GMLevel;
        public string Language;
    }
}
