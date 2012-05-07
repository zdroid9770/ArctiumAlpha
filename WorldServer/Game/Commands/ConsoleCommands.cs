using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Commands;

namespace WorldServer.Game.Commands
{
    public class ConsoleCommands
    {
        [ConsoleAttribute("create", "name", "password")]
        public void Create()
        {
            Console.WriteLine("CreateCommand!!!");
        }
    }
}
