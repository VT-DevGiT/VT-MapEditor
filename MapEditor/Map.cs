using Synapse.Api.CustomObjects;
using Synapse.Config;
using System.Collections.Generic;

namespace MapEditor
{
    public class Map : IConfigSection
    {
        public static Map None = new Map();

        public string Name { get; set; } = Plugin.MapNone;
        public List<MapSchematic> MapSchematic { get; set; } = new List<MapSchematic>();

        public Map() { }

        public Map(string name)
            => Name = name;

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        internal List<SynapseObject> Spawn()
        {
            var spawnObject = new List<SynapseObject>();
            foreach (var schematic in MapSchematic)
            {
                var @object = schematic.Spawn();
                spawnObject.Add(@object);
                @object.ObjectData.Add(Plugin.ObjectKeyMap, Name);
            }
            return spawnObject;
        }
    }
}
