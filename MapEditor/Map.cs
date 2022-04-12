using Synapse.Api.CustomObjects;
using Synapse.Config;
using System;
using System.Collections.Generic;

namespace MapEditor
{
    public class Map : IConfigSection
    {
        public string Name { get; set; } = Plugin.MapNone;
        public List<MapSchematic> MapSchematic { get; set; } = new List<MapSchematic>();

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
