﻿public enum Opcodes : ushort
{
    SMSG_AUTH_CHALLENGE = 0x1DD,
    SMSG_AUTH_RESPONSE  = 0x1DF,
    CMSG_AUTH_SESSION   = 0x1DE,
    CMSG_CHAR_CREATE    = 0x036,
    SMSG_CHAR_CREATE    = 0x03A,
    CMSG_CHAR_DELETE    = 0x038,
    SMSG_CHAR_DELETE    = 0x03C,
    CMSG_CHAR_ENUM      = 0x037,
    SMSG_CHAR_ENUM      = 0x03B,
    CMSG_PING           = 0x1CD,
    SMSG_PONG           = 0x1CE,
    CMSG_PLAYER_LOGIN   = 0x03D,
    SMSG_UPDATE_OBJECT  = 0x0A9,
}