using Synapse.Command;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
       Name = "Select",
        Aliases = new string[] { "Scl", "Secl", "sl"  },
        Description = "Enter in the Edit mod",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Arguments = new string[] { "at/of", "Cordonate" },
        Usage = "Select "
       )]
    internal class CommandSelect : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            if (context.Arguments.Count == 0 || context.Arguments.FirstElement().ToLower() == "list")
            {
                result.Message = "List of the boject:";
                foreach (var editingObject in Plugin.Instance.EditingObjects)
                {
                    var id = Cursor.TryGetCursor(editingObject, out var cursor) ? cursor.ID.ToString() : "Unknow";
                    result.State = CommandResultState.Ok;
                    result.Message += @$"
-   ID: {id}
    Schematic Name : {editingObject.Name} 
    Schematic ID : {editingObject.ID}
    Postion : {editingObject.Position}
    Size : {editingObject.Scale}";
                }
            }
            else if (int.TryParse(context.Arguments.FirstElement(), out var id))
            {
                if (!Cursor.TryGetCursor(id, out var cursor))
                {
                    result.Message = "Uncknow ID plis do \"list\"";
                    result.State = CommandResultState.Error;
                }
                else
                {
                    if (!Plugin.Instance.PlayerSlectedObject.ContainsKey(context.Player))
                        Plugin.Instance.PlayerSlectedObject.Add(context.Player, cursor);
                    else
                        Plugin.Instance.PlayerSlectedObject[context.Player] = cursor;
                    result.Message = "Object selected";
                    result.State = CommandResultState.Ok;
                }
            }
            else
            {
                result.Message = "You must specify an id (a number) or you can do \"list\" to get the list of IDs and objects";
                result.State = CommandResultState.Error;
            }
            return result;
        }
    }
}
