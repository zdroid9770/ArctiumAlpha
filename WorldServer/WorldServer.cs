using System;
using Common.Database;
using Common.Logging;
using WorldServer.Network;
using WorldServer.Packets;

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
            Log.Message(LogType.INIT, "______________REALMPROXY/WORLD_____________");
            Log.Message();

            Log.Message(LogType.NORMAL, "Starting Arctium Alpha WorldServer...");

            DB.Characters.Init("Characters");

            RealmManager.RealmSession = new RealmSocket();
            WorldManager.WorldSession = new WorldSocket();

            if (WorldManager.WorldSession.Start() && RealmManager.RealmSession.Start())
            {
                RealmManager.RealmSession.StartRealmThread();
                RealmManager.RealmSession.StartProxyThread();
                Log.Message(LogType.NORMAL, "RealmProxy listening on {0} port {1}/{2}.", "127.0.0.1", 9090, 9100);
                Log.Message(LogType.NORMAL, "RealmProxy successfully started!");

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
        }
    }
}
