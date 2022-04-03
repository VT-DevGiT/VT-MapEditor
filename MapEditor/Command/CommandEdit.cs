using Synapse;
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
        Name = "Edit",
        Aliases = new string[] { "EditMap" },
        Description = "Enter in the Edit mod",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "specified the name of the Map for Edit this map or send stop for top the edit mod",
        Arguments = new string[] { "Map Name/NONE" }
        )]
    internal class CommandEdit : ISubCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            

            return result;
        }
    }
}
