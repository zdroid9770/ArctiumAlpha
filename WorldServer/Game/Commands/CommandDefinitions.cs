using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Commands;

namespace WorldServer.Game.Commands
{
    public class CommandDefinitions
    {
        public static void LoadCommandDefinitions()
        {
            ConsoleManager.DefineCommand("Create", ConsoleCommands.CreateAccount);
        }
    }
}
