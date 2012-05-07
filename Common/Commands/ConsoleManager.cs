using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Commands
{
    public class ConsoleManager
    {
        public static Dictionary<string, HandleCommand> CommandHandlers = new Dictionary<string, HandleCommand>();
        public delegate void HandleCommand(string command);

        public static void InitCommands()
        {
            //while (true)
            {
            //    Thread.Sleep(1);
            }
        }

        public static void DefineCommand(string command, HandleCommand handler)
        {
            CommandHandlers[command] = handler;
        }
    }
}
