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

            if (Plugin.Instance.CurentEditedMap == null || Plugin.Instance.CurentEditedMap.Name == Plugin.MapNone)
            {
                result.Message = "You need to ave a map in Edit mod";
                result.State = CommandResultState.Error;
            }

            Plugin.Instance.SaveMapSchematic(Plugin.Instance.CurentEditedMap, Plugin.Instance.CurentEditedMap.Name);

            return result;
        }
    }
}
