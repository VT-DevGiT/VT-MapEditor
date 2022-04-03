using Synapse.Config;
using System.Collections.Generic;

namespace MapEditor
{
    public class PluginConfig : IConfigSection
    {
        public List<string> MapsLoaded = new List<string>(){ "NONE" };

    }
}