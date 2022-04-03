using Synapse.Api.CustomObjects;
using Synapse.Config;
using System;

namespace MapEditor
{
    [Serializable]
    public class MapShematic
    {
        public int ID { get; set; }
        public SerializedMapPoint MapPoint { get; set; }
        public SerializedVector3 Scale { get; set; }
        public SerializedVector3 Rotation { get; set; }

        public SynapseObject Spawn()
        {
            var schematic = SchematicHandler.Get.SpawnSchematic(ID, MapPoint.Parse().Position, Rotation);
            schematic.Scale = Scale;
            return schematic;
        }
    }
}
