using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "UnloadMap",
        Aliases = new string[] { "Unload" },
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
                    mapName += context.Arguments.Array[i];
                    if (context.Arguments.Count > i + 1)
                        mapName += " ";
                }

                var map = PluginClass.Instance.GetMap(mapName);
                PluginClass.Instance.DespawnMap(map);
            }
            else
            {
                PluginClass.Instance.DespawnMaps();
                result.Message = "All Maps was despawn";
                result.State = CommandResultState.Ok;
            }

            return result;
        }
    }
}
