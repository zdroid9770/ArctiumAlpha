using System;
using System.Collections.Generic;
using System.Threading;
using Common.Logging;

namespace Common.Commands
{
    public class ConsoleManager
    {
        protected static Dictionary<string, HandleCommand> CommandHandlers = new Dictionary<string, HandleCommand>();
        public delegate void HandleCommand(string[] args);

        public static void InitCommands()
        {
            while (true)
            {
                Thread.Sleep(1);
                Log.Message(LogType.CMD, "Arctium >> ");

                string[] line = Console.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);
                string[] args = new string[line.Length - 1];
                Array.Copy(line, 1, args, 0, line.Length - 1);

                InvokeHandler(line[0].ToLower(), args);
            }
        }

        public static void DefineCommand(string command, HandleCommand handler)
        {
            CommandHandlers[command] = handler;
        }

        protected static bool InvokeHandler(string command, params string[] args)
        {
            if (CommandHandlers.ContainsKey(command))
            {
                CommandHandlers[command].Invoke(args);
                return true;
            }
            else
                return false;
        }
    }
}
