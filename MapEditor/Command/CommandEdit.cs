using Synapse.Command;
using System.Linq;
using VT_Api.Core.Command;

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
                mapName += context.Arguments.Array[i];
                if (context.Arguments.Count > i + 1)
                    mapName += " ";
            }

            if (mapName.ToUpper() == Plugin.MapNone)
            {
                Plugin.Instance.CurentEditedMap = null;
                result.Message = "You exit the edit mode";
                result.State = CommandResultState.Ok;
                return result;
            }

            var map = Plugin.Instance.LoadedMaps.FirstOrDefault(m => m.Name == mapName);
            if (map == null)
                Plugin.Instance.LoadedMaps.Add(map = new Map(mapName));

            Plugin.Instance.CurentEditedMap = map;

            return result;
        }
    }
}
