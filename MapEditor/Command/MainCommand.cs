using Synapse.Command;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [CommandInformation(
        Name = "MapEditor",
        Aliases = new string[] { "ME","MED", "MapEdit" },
        Arguments = new string[] { "Load/Unload/Edit/(help)" },
        Description = "Main command to manage custom maps",
        Permission = "ME",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        Usage = "MapEditor help"
        )]
    internal class MainCommand : IMainCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();
            result.State = CommandResultState.Ok;

            return result;
        }
    }
}
