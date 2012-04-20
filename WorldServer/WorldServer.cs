using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using WorldServer.Network;

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
            Log.Message(LogType.INIT, "___________________WORLD___________________");
            Log.Message();

            Log.Message(LogType.NORMAL, "Starting Arctium Alpha WorldServer...");

            WorldManager World = new WorldManager();

            if (World.Start("127.0.0.1", 8888))
            {
                World.StartConnectionThread();

                Log.Message(LogType.NORMAL, "WorldServer listening on {0} port {1}.", "127.0.0.1", 8888);
                Log.Message(LogType.NORMAL, "WorldServer successfully started!");
            }
            else
            {
                Log.Message(LogType.ERROR, "WorldServer couldn't be started: ");
            }
        }
    }
}
