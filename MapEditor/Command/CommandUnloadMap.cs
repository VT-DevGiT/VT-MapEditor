using Synapse.Api;
using Synapse.Command;
using System.Linq;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "UnloadMap",
        Aliases = new string[] { "Unload", "unld" },
        Description = "Unload all maps or the map if specify",
        Permission = "ME.Manage",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "specified the name of the Map for unload only this map",
        Arguments = new string[] { "Map Name" }
        )]
    internal class CommandUnloadMap : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            if (context.Arguments.Any())
            {
                var mapName = string.Empty;
                for (var i = 0; context.Arguments.Count > i; i++)
                {
                    mapName += context.Arguments.Array[i + context.Arguments.Offset];
                    if (context.Arguments.Count > i + 1)
                        mapName += " ";
                }
                var map = Plugin.Instance.GetMap(mapName);
                if (map == null)
                {
                    result.Message = "The Maps do not existe";
                    result.State = CommandResultState.Error;
                    return result;
                }
                Plugin.Instance.DespawnMap(map);
                result.Message = "The Maps was despawned";
                result.State = CommandResultState.Ok;
            }
            else
            {
                Plugin.Instance.DespawnMaps();
                result.Message = "All Maps was despawned";
                result.State = CommandResultState.Ok;
            }

            return result;
        }
    }
}
