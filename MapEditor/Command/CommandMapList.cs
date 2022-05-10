using Synapse.Command;
using System.Linq;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "MapList",
        Aliases = new string[] { "ListMap", "ml" },
        Description = "List of all Map",
        Permission = "ME.Manage",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor"
        )]
    internal class CommandMapList : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();
            result.State = CommandResultState.Ok;
            result.Message = "Maps :";
            foreach (var map in Plugin.Instance.Maps)
                result.Message += $"\n- {map.Key}";

            return result;
        }
    }
}
