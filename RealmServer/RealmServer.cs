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
            Log.Message(LogType.INIT, "___________________REALM___________________");
            Log.Message();

            Log.Message(LogType.NORMAL, "Starting Arctium Alpha RealmServer...");

            RealmManager Realm = new RealmManager();
            ProxyManager Proxy = new ProxyManager();

            if (Proxy.Start("127.0.0.1", 9090) && Realm.Start("127.0.0.1", 9100))
            {
                Proxy.StartConnectionThread();
                Log.Message(LogType.NORMAL, "RealmProxy listening on {0} port {1}.", "127.0.0.1", 9090);
                Log.Message(LogType.NORMAL, "RealmProxy successfully started!");

                Realm.StartConnectionThread();
                Log.Message(LogType.NORMAL, "RealmServer listening on {0} port {1}.", "127.0.0.1", 9100);
                Log.Message(LogType.NORMAL, "RealmServer successfully started!");
            } 
            else
            {
                Log.Message(LogType.ERROR, "RealmServer couldn't be started: ");
            }
        }
    }
}
