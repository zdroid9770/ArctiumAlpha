public enum Opcodes : ushort
{
    SMSG_AUTH_CHALLENGE = 0x1DD,
    SMSG_AUTH_RESPONSE  = 0x1DF,
    CMSG_AUTH_SESSION   = 0x1DE,
    CMSG_CHAR_CREATE    = 0x036,
    SMSG_CHAR_CREATE    = 0x03A,
    CMSG_CHAR_ENUM      = 0x037,
    SMSG_CHAR_ENUM      = 0x03B,
    CMSG_PING           = 0x1CD,
    SMSG_PONG           = 0x1CE,
}
