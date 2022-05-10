using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Config;
using System.Collections.Generic;

namespace MapEditor
{
    public class Map : IConfigSection
    {
        public static Map None = new Map();

        public string Name { get; set; } = Plugin.MapNone;
        public List<MapSchematic> MapSchematics { get; set; } = new List<MapSchematic>();

        public Map() { }

        public Map(string name)
            => Name = name;

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        internal List<SynapseObject> Spawn(bool editing = false)
        {
            var spawnObjects = new List<SynapseObject>();
            foreach (var schematic in MapSchematics)
            {
                var @object = schematic.Spawn(editing);
                if (@object == null)
                    continue;
                spawnObjects.AddRange(@object);
            }
            foreach (var spawnObject in spawnObjects)
                spawnObject.ObjectData.Add(Plugin.ObjectKeyMap, Name);
            return spawnObjects;
        }
    }
}
