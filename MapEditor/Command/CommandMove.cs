using Synapse.Command;
using System;
using UnityEngine;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
          Name = "Move",
          Aliases = new string[] { "Mov", "Mv" },
          Description = "Move the selected Object",
          Permission = "ME.Edit",
          Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
          MainCommandName = "MapEditor",
          Usage = "Move \"to\" or move \"of\" and the amount or new cordonate",
          Arguments = new string[] { "to/of", "Vector" }
          )]
    internal class CommandMove : ISubCommand
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

            if (context.Arguments.Count < 3)
            {
                result.Message = "You need to specifid the cordonate like this : 12 32 45.2";
                result.State = CommandResultState.Error;
                return result;
            }

            var cusor = Plugin.Instance.PlayerSlectedObject[context.Player];

            if (!TryGetCordonate(context.Arguments.Count == 3 ? context.Arguments : context.Arguments.Segment(1), out var vector))
            {
                result.Message = "You need to specifid the cordonate like this : (\"to\" or \"of\") 12 32 45.2";
                result.State = CommandResultState.Error;
            }

            if (context.Arguments.Count == 3 || context.Arguments.FirstElement().ToLower() == "to")
            {
                cusor.Move(vector, false);

                result.Message = "Object moved";
                result.State = CommandResultState.Ok;
            }
            else if (context.Arguments.FirstElement().ToLower() == "of")
            {
                cusor.Move(vector, true);

                result.Message = "Object moved";
                result.State = CommandResultState.Ok;
            }
            else
            {
                result.Message = "Invalide parmater, you need to say if it \"to\" or \"of\", like this : to 12 32 45.2";
                result.State = CommandResultState.Error;
            }
            return result;
        }

        public bool TryGetCordonate(ArraySegment<string> arg, out Vector3 vector)
        {
            if (!float.TryParse(arg.Array[arg.Offset], out var x) || !float.TryParse(arg.Array[arg.Offset + 1], out var y) || !float.TryParse(arg.Array[arg.Offset + 2], out var z))
            {
                vector = Vector3.zero;
                return false;
            }
            else
            {
                vector = new Vector3(x, y, z);
                return true;
            }
        }
    }
}
