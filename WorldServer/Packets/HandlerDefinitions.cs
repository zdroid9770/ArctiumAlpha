using WorldServer.Packets.Handlers;

namespace WorldServer.Packets
{
    public class HandlerDefinitions
    {
        public static void InitializePacketHandler()
        {
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_AUTH_SESSION, AuthHandler.HandleAuthSession);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_ENUM, CharHandler.HandleCharEnum);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_CREATE, CharHandler.HandleCharCreate);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_CHAR_DELETE, CharHandler.HandleCharDelete);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_PING, NetHandler.HandlePing);
            PacketManager.DefineOpcodeHandler(Opcodes.CMSG_PLAYER_LOGIN, WorldHandler.HandleUpdateObject);

            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_FORWARD, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_BACKWARD, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_STRAFE_LEFT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_STRAFE_RIGHT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_STRAFE, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_JUMP, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_TURN_LEFT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_TURN_RIGHT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_TURN, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_PITCH_UP, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_PITCH_DOWN, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_PITCH, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_RUN_MODE, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_WALK_MODE, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_START_SWIM, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_STOP_SWIM, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_FACING, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_SET_PITCH, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_ROOT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_UNROOT, MovementHandler.HandleMovementStatus);
            PacketManager.DefineOpcodeHandler(Opcodes.MSG_MOVE_HEARTBEAT, MovementHandler.HandleMovementStatus);
        }
    }
}
