using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Commands
{
    [AttributeUsageAttribute(AttributeTargets.Method)]
    public class ConsoleAttribute : Attribute
    {
        public string Command { get; set; }
        public string[] Args { get; set; }
        public int Count { get; set; }

        public ConsoleAttribute(string command, params string[] args)
        {
            Command = command;
            Args = args;
            Count = args.Length;
        }
    }
}
