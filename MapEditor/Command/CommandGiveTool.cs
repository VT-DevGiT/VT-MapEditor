using Synapse;
using Synapse.Command;
using VT_Api.Core.Command;
using VT_Api.Core.Enum;
using VT_Api.Extension;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "GiveTool",
        Aliases = new string[] { "Give" },
        Description = "Give tool for edit",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "specified the name of the Map for unload only this map"
        )]
    internal class CommandGiveTool : ISubCommand
    {
        public int[] ToolsID =
        {
            (int)ItemID.Destroyer,
            (int)ItemID.Mover,
            (int)ItemID.Positioner,
            (int)ItemID.Scaler,
            (int)ItemID.Selector
        };

        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            if (context.Platform == Platform.ServerConsole && context.Arguments.Count == 0)
            {
                result.State = CommandResultState.Error;
                result.Message = "You need to specifid the player";
            }


            string arg = string.Empty;
            if (context.Arguments.Count == 0)
                arg = "ME";
            else for (var i = 0; context.Arguments.Count > i; i++)
            {
                arg += context.Arguments.Array[i];
                if (context.Arguments.Count > i + 1)
                    arg += " ";
            }

            if (Server.Get.TryGetPlayers(arg, out var players, context.Player))
            {
                foreach (var player in players)
                {
                    player.Inventory.Clear();
                    foreach (var id in ToolsID)
                        player.Inventory.AddItem(id);
                    player.GetOrAddComponent<MapEditHandler>().enabled = true;
                }
            }
            else
            {
                result.State = CommandResultState.Error;
                result.Message = "Players not founds !";
            }
            

            return result;
        }
    }
}
