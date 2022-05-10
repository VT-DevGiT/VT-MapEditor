using MapGeneration;
using Synapse.Command;
using System;
using System.Linq;
using VT_Api.Core.Command;

namespace MapEditor.Command
{

    [SubCommandInformation(
       Name = "Room",
        Aliases = new string[] { "ro", "rm" },
        Description = "Change the referenced room of the Object",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Arguments = new string[] { "room/none/type", "roomName of Type" }
       )]
    internal class CommandRoom : ISubCommand
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

            if (context.Arguments.Count < 1)
            {
                result.Message = "invalide amount of parm";
                result.State = CommandResultState.Error;
                return result;
            }

            var data = Plugin.Instance.PlayerSlectedObject[context.Player].AttachedObject.ObjectData;

            if (context.Arguments.FirstElement().ToLower() == "none")
            {

                data[Plugin.ObjectKeyRoom] = Plugin.Fixed;
                result.State = CommandResultState.Error;
                result.Message = "position Fixed set";
                return result;
            }

            if (context.Arguments.Count < 2)
            {
                result.Message = "invalide amount of parm";
                result.State = CommandResultState.Error;
                return result;
            }

            var arg = context.Arguments.Segment(1).FirstElement().ToLower();

            if (context.Arguments.FirstElement().ToLower() == "room")
            {
                if (arg == "list")
                {
                    result.Message = "list of room :";
                    result.State = CommandResultState.Ok;
                    foreach (var room in Synapse.Api.Map.Get.Rooms)
                        result.Message += $"\n- {room.RoomName}";
                    return result;
                }

                var roomSelected = Synapse.Api.Map.Get.Rooms.FirstOrDefault(r => r.RoomName.ToLower() == arg);

                if (roomSelected == null)
                {
                    result.Message = "invalide room name, do list for get all type";
                    result.State = CommandResultState.Error;
                    return result;
                }

                data[Plugin.ObjectKeyRoom] = roomSelected;

                result.Message = "Room set";
                result.State = CommandResultState.Ok;
                return result;
            }

            if (context.Arguments.FirstElement().ToLower() == "type")
            {
                if (arg == "list")
                {
                    result.Message = "list of room :";
                    result.State = CommandResultState.Ok;
                    foreach (var room in Synapse.Api.Map.Get.Rooms)
                        result.Message += $"\n- {room.RoomName}";
                    return result;
                }

                if (!Enum.TryParse<RoomName>(arg, out _))
                {
                    result.Message = "invalide room type, do list for get all type";
                    result.State = CommandResultState.Error;
                    return result;
                }

                data[Plugin.ObjectKeyRoom] = MapSchematic.Foreach + context.Arguments.Segment(1);
                result.Message = "Room set";
                result.State = CommandResultState.Ok;
                return result;
            }
            result.State = CommandResultState.Error;
            result.Message = "invalide parm";

            return result;
             
        }
    }
}
