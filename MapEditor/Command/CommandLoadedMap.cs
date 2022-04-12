using Synapse.Command;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "loadedMap",
        Aliases = new string[] { "Loaded" },
        Description = "return the loaded map",
        Permission = "ME.Manage",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor"
        )]
    internal class CommandLoadedMap : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            result.Message = "Loaded map :";

            foreach (var map in Plugin.Instance.LoadedMaps)
                result.Message += $"- {map.Name}";

            result.State = CommandResultState.Ok;

            return result;
        }
    }
}
