﻿using System;
using Common.Database.ObjectDatabase;
using Common.Network.Packets;
using Common.Structs;
using WorldServer.Network;

namespace WorldServer.Packets.Handlers
{
    public class MovementHandler
    {
        public static void HandleMovementStatus(ref PacketReader packet, ref WorldManager manager)
        {
            Opcodes MoveMessage = packet.Opcode;
            UInt64 TransportGuid = packet.ReadUInt64();
            Single TransportX = packet.ReadFloat();
            Single TransportY = packet.ReadFloat();
            Single TransportZ = packet.ReadFloat();
            Single TransportO = packet.ReadFloat();
            Single X = packet.ReadFloat();
            Single Y = packet.ReadFloat();
            Single Z = packet.ReadFloat();
            Single O = packet.ReadFloat();
            Single Pitch = packet.ReadFloat();
            UInt32 Flags = packet.ReadUInt32();

            PacketWriter movementStatus = new PacketWriter(MoveMessage);
            movementStatus.WriteUInt64(TransportGuid);
            movementStatus.WriteFloat(TransportX);
            movementStatus.WriteFloat(TransportY);
            movementStatus.WriteFloat(TransportZ);
            movementStatus.WriteFloat(TransportO);
            movementStatus.WriteFloat(X);
            movementStatus.WriteFloat(Y);
            movementStatus.WriteFloat(Z);
            movementStatus.WriteFloat(O);
            movementStatus.WriteFloat(Pitch);
            movementStatus.WriteUInt32(Flags);

            manager.Send(movementStatus);

            var result = ODB.Characters.Select<Character>();
            foreach (Character c in result)
            {
                if (c.Guid == 2)
                {
                    c.X = X;
                    c.Y = Y;
                    c.Z = Z;
                    c.O = O;
                    ODB.Characters.Save(c);
                }
            }
        }
    }
}
