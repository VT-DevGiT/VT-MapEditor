using Synapse.Command;
using VT_Api.Core.Command;

namespace MapEditor.Command
{
    [SubCommandInformation(
        Name = "Info",
        Description = "Send info about who to use the plugin",
        Permission = "ME",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor"
        )]
    internal class CommandInfo : ISubCommand
    {

        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            result.State = CommandResultState.Ok;
            result.Message = $@"Info
Command :
For more info ont the command enter ""MapEditor Help"".
Warning : for som command you need to ave  selected an object ! 

Tool : 
The tools are revolvers, to use them you must fire them.
You must be in edit mode to use themn, use  ""MapEditor Edit"" you must have permission ""ME.Edit"" to enter this command.

To change the Amount or the slected ID of schematic use your key {Plugin.Instance.Config.KeyUp} to Up and key {Plugin.Instance.Config.KeyDown} to Down

the tools can be used without permission!
A lambda player can edit the map !

For mort info contact Warkis#9984 on Discord. 
:)
";
            return result;
        }
    }
}
