using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Common.Commands
{
    public class CommandParser
    {
        public static T Read<T>(string[] args, int index)
        {
            try
            {
                return (T)Convert.ChangeType(args[index], typeof(T));
            }
            catch
            {
                Log.Message(LogType.ERROR, "Wrong arguments for the current command!!!");
            }

            return default(T);
        }
    }
}
