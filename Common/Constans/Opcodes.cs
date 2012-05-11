public enum Opcodes : ushort
{
    #region UserRouterClient
    AuthSession          = 0x0449,
    Ping                 = 0,
    SuspendTokenResponse = 0,
    SuspendCommsAck      = 0,
    LogDisconnect        = 0x046D,
    AuthContinuedSession = 0x044D,
    EnableNagle          = 0x4449,
    #endregion
}
