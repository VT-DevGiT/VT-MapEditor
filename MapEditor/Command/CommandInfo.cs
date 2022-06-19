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
Warning : for some commands you need to ave selected an object ! 

Tool : 
The tools are revolvers, to use them you must fire them.
for all tools (except the spawner), you have to pull the cursor.
If you fire the cursor in the center you will get info on the object unless you use the destroyer.
If you fire the colored facets to interact in one direction, example : fire whith the mover on the green face pointing up for up the schematic.

You must be in edit mode to use themn, use  ""MapEditor Edit"" you must have permission ""ME.Edit"" to enter this command.
To change the chosen schematic or increase the displacement Vector use your key {Plugin.Instance.Config.KeyUp} to Up and key {Plugin.Instance.Config.KeyDown} to Down

Note : 
- the tools can be used without permission!
- if you don't understand, don't panic! The plugin is easy to handle. If you really don't understand go see the creator (Warkis#9984).
- if there are spelling mistakes it's normal, I'm a dixlexic, if necessary you can report the mistake to me so that I can correct it.
:)

Credit :
Warkis for the plugin,
AlmightyLks for the UI
";
            return result;
        }
    }
}
