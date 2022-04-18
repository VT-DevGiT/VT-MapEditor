using Synapse.Api;
using Synapse.Command;
using VT_Api.Core.Command;
using VT_Api.Extension;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "Edit",
        Aliases = new string[] { "EditMap" },
        Description = "Enter in the Edit mod",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "specified the name of the Map for Edit this map or send stop for top the edit mod. SAVE THE MAP BEFORE USING THIS COMMAND, THIS DELETE THE CURENT EDITED MAP !",
        Arguments = new string[] { "Map Name/NONE" }
        )]
    internal class CommandEdit : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            if (context.Arguments.Count == 0)
            {
                result.Message = "You need to specifid the name of the map";
                result.State = CommandResultState.Error;
                return result;
            }

            var mapName = string.Empty;
            for (var i = 0; context.Arguments.Count > i; i++)
            {
                mapName += context.Arguments.Array[i + context.Arguments.Offset];
                if (context.Arguments.Count > i + 1)
                    mapName += " ";
            }

            if (string.IsNullOrWhiteSpace(mapName))
            {
                result.Message = "You need to enter a name (or NONE to exit de mod)";
                result.State = CommandResultState.Ok;
                return result;
            }

            if (mapName.ToUpper() == Plugin.MapNone)
            {
                Plugin.Instance.CurentEditedMap = null;
                result.Message = "You exit the edit mode";
                result.State = CommandResultState.Ok;
                return result;
            }

            var map = Plugin.Instance.GetOrAddMap(mapName);

            Plugin.Instance.CurentEditedMap = map;

            if (context.Platform == Platform.RemoteAdmin)
                context.Player.GetOrAddComponent<MapEditUI>().UIRuning = true;

            result.Message = $"You start to edit {mapName}";
            result.State = CommandResultState.Ok;

            return result;
        }
    }
}
