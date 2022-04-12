using Synapse.Config;
using System.Collections.Generic;

namespace MapEditor
{
    public class Config : IConfigSection
    {
        public List<string> MapsLoaded = new List<string>()
        { 
            Plugin.MapNone
        };


    }
}