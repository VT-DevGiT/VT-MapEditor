using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.ToolItem
{
    internal interface ITool
    {
        int Amount { get; set; }
        string Info { get; }
    }
}
