using System;
using Common.Commands;
using Common.Database.ObjectDatabase;
using Common.Logging;
using WorldServer.Game.Commands;
using WorldServer.Network;
using WorldServer.Packets;
using Common.Network.Packets;
using Common.Constans;
using Common.Opcodes;

namespace WorldServer
{
    class WorldServer
    {
        static void Main()
        {
            Log.Message(LogType.INIT, "___________________________________________");
            Log.Message(LogType.INIT, "    __                                     ");
            Log.Message(LogType.INIT, "    / |                     ,              ");
            Log.Message(LogType.INIT, "---/__|---)__----__--_/_--------------_--_-");
            Log.Message(LogType.INIT, "  /   |  /   ) /   ' /    /   /   /  / /  )");
            Log.Message(LogType.INIT, "_/____|_/_____(___ _(_ __/___(___(__/_/__/_");
            Log.Message(LogType.INIT, "___________________________________________");
            Log.Message();

            Log.Message(LogType.NORMAL, "Starting Arctium MoP Beta Server...");

            ODB.Characters.Init("Characters");
            ODB.Realms.Init("Realms");

            RealmManager.RealmSession = new RealmSocket();
            WorldManager.WorldSession = new WorldSocket();

            if (WorldManager.WorldSession.Start() && RealmManager.RealmSession.Start())
            {
                RealmManager.RealmSession.StartRealmThread();
                Log.Message(LogType.NORMAL, "RealmServer listening on {0} port {1}/{2}.", "127.0.0.1", 9090, 9100);
                Log.Message(LogType.NORMAL, "RealmServer successfully started!");

                WorldManager.WorldSession.StartConnectionThread();
                Log.Message(LogType.NORMAL, "WorldServer listening on {0} port {1}.", "127.0.0.1", 8100);
                Log.Message(LogType.NORMAL, "WorldServer successfully started!");
                Log.Message();

                HandlerDefinitions.InitializePacketHandler();
            }
            else
            {
                Log.Message(LogType.ERROR, "WorldServer couldn't be started: ");
                Log.Message();
            }

            // Free memory...
            GC.Collect();
            Log.Message(LogType.NORMAL, "Total Memory: {0}", GC.GetTotalMemory(false));

            // Init Command handlers...
            CommandDefinitions.LoadCommandDefinitions();
            ConsoleManager.InitCommands();
        }
    }
}
