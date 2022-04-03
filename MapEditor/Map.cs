using Synapse.Api.CustomObjects;
using Synapse.Config;
using System;
using System.Collections.Generic;

namespace MapEditor
{
    public class Map : IConfigSection
    {
        public string Name { get; set; } = "NONE";
        public List<MapShematic> MapShematics { get; set; } = new List<MapShematic>();

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        internal List<SynapseObject> Spawn()
        {
            var spawnObject = new List<SynapseObject>();
            foreach (var shematic in MapShematics)
                spawnObject.Add(shematic.Spawn());
            return spawnObject;
        }
    }
}
