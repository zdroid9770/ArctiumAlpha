using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Commands
{
    public class ConsoleManager
    {
        public Dictionary<string, HandleCommands> CommandHandlers = new Dictionary<string, HandleCommands>();
        public delegate void HandleCommands();
    }
}
