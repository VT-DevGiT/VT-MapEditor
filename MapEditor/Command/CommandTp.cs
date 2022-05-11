using Synapse.Command;
using UnityEngine;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
       Name = "tp",
       Aliases = new string[] { },
       Description = "Tp to the selected object",
       Permission = "ME.Edit",
       Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
       MainCommandName = "MapEditor"
      )]
    internal class CommandTp : ISubCommand
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

            var postion = Plugin.Instance.PlayerSlectedObject[context.Player].MainObject.Position + new Vector3(0, -5 , 0);
            context.Player.Position = postion;

            result.Message = "You ave been tp";
            result.State = CommandResultState.Ok;
            return result;
        }
    }
}
