using Synapse.Command;
using System.Linq;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "UnloadMap",
        Aliases = new string[] { "Unload" },
        Description = "Load the specified map",
        Permission = "ME.Manage",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "specified the name of the Map for unload only this map",
        Arguments = new string[] { "Map Name" }
        )]
    internal class CommandLoadMap : ISubCommand
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
                if (map != null)
                {
                    result.Message = "Map spawned";
                    result.State = CommandResultState.Ok;
                    PluginClass.Instance.SpawnMap(map);
                }
                else
                {
                    result.Message = "No map whits this name !";
                    result.State = CommandResultState.Error;
                }
            }
            else
            {
                result.Message = "Specifided the name !";
                result.State = CommandResultState.Error;
            }

            return result;
        }
    }
}
