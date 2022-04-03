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
        Name = "GiveTool",
        Aliases = new string[] { "Give" },
        Description = "Give tool for edit",
        Permission = "ME.Edit",
        Platforms = new Platform[] { Platform.RemoteAdmin, Platform.ServerConsole },
        MainCommandName = "MapEditor",
        Usage = "specified the name of the Map for unload only this map"
        )]
    internal class CommandGiveTool
    {
    }
}
