using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using RealmServer.Network;

namespace RealmServer
{
    class RealmServer
    {
        static void Main()
        {
            Log.Message(LogType.INIT, "___________________________________________");
            Log.Message(LogType.INIT, "    __                                     ");
            Log.Message(LogType.INIT, "    / |                     ,              ");
            Log.Message(LogType.INIT, "---/__|---)__----__--_/_--------------_--_-");
            Log.Message(LogType.INIT, "  /   |  /   ) /   ' /    /   /   /  / /  )");
            Log.Message(LogType.INIT, "_/____|_/_____(___ _(_ __/___(___(__/_/__/_");
            Log.Message(LogType.INIT, "________________REALMPROXY_________________");
            Log.Message();

            Log.Message(LogType.NORMAL, "Starting Arctium Alpha RealmProxy...");

            RealmManager.RealmSession = new RealmSocket();

            if (RealmManager.RealmSession.Start())
            {
                RealmManager.RealmSession.StartRealmThread();
                RealmManager.RealmSession.StartProxyThread();
                Log.Message(LogType.NORMAL, "RealmProxy listening on {0} port {1}/{2}.", "127.0.0.1", 9090, 9100);
                Log.Message(LogType.NORMAL, "RealmProxy successfully started!");
            }
            else
            {
                Log.Message(LogType.ERROR, "RealmProxy couldn't be started: ");
            }
        }
    }
}
