using Synapse.Command;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "Save",
        Aliases = new string[] { "SaveMap" },
        Description = "Enter in the Edit mod",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "Need to be in the edit mod"
        )]
    internal class CommandSave : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            

            return result;
        }
    }
}
