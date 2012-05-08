using Common.Commands;

namespace WorldServer.Game.Commands
{
    public class CommandDefinitions
    {
        public static void LoadCommandDefinitions()
        {
            ConsoleManager.DefineCommand("create", ConsoleCommands.CreateAccount);
        }
    }
}
