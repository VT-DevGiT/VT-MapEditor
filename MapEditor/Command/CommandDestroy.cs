using Synapse.Command;
using System.Linq;
using VT_Api.Core.Command;

namespace MapEditor.Command
{

    [SubCommandInformation(
       Name = "Destroy",
        Aliases = new string[] { "de", "dest", "des" },
        Description = "Enter in the Edit mod",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor"
       )]
    internal class CommandDestroy : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            if (!Plugin.Instance.PlayerSlectedObject.ContainsKey(context.Player) || Plugin.Instance.PlayerSlectedObject[context.Player] == null)
            {
                result.Message = "You need to select an object";
                result.State = CommandResultState.Error;
                return result;
            }

            var cursor = Plugin.Instance.PlayerSlectedObject[context.Player];
            var cursorsToRemove = Plugin.Instance.PlayerSlectedObject.Where(p => p.Value == cursor);
            foreach (var cursorToRemove in cursorsToRemove)
                Plugin.Instance.PlayerSlectedObject[cursorToRemove.Key] = null;

            Plugin.Instance.EditingObjects.Remove(cursor.AttachedObject);
            cursor.AttachedObject.Destroy();

            result.Message = "The object is destroy";
            result.State = CommandResultState.Ok;
            return result;
        }
    }
}
