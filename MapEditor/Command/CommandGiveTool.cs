using Synapse;
using Synapse.Command;
using VT_Api.Core.Command;
using VT_Api.Core.Enum;
using VT_Api.Extension;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "GiveTools",
        Aliases = new string[] { "GiveTool", "Give", "Tool", "Tools" },
        Description = "Give tool for edit",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "You can specified the players",
        Arguments = new string[]  {"[players]" }
        )]
    internal class CommandGiveTool : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            if (context.Platform == Platform.ServerConsole && context.Arguments.Count == 0)
            {
                result.State = CommandResultState.Error;
                result.Message = "You need to specifid the player";
            }
            
            var arg = string.Empty;
            
            if (context.Arguments.Count == 0)
                arg = "ME";
            else for (var i = 0; context.Arguments.Count > i; i++)
            {
                arg += context.Arguments.Array[i + context.Arguments.Offset];
                if (context.Arguments.Count > i + 1)
                    arg += " ";
            }

            if (Server.Get.TryGetPlayers(arg, out var players, context.Player))
            {
                foreach (var player in players)
                {
                    if (Synapse.Api.Roles.RoleManager.Get.IsIDRegistered(199))
                        player.RoleID = (int)RoleID.Staff;

                    player.Inventory.Clear();
                    foreach (var id in Plugin.ToolsID)
                        player.Inventory.AddItem(id);

                    player.GetOrAddComponent<MapEditUI>().UIRuning = true;
                    player.NoClip = true;
                    player.GodMode = true;
                }
                result.State = CommandResultState.Ok;
                result.Message = "Tool Gived";
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
