using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Config;
using System;
using UnityEngine;

namespace MapEditor
{
    [Serializable]
    public class MapSchematic
    {

        public MapSchematic()
        {
            ID = -1;
            MapPoint = new SerializedMapPoint(Plugin.MapNone, 0, 0, 0);
            Scale = Vector3.one;
            Rotation = Vector3.one;
        }

        public MapSchematic(int id, Room room, Vector3 position)
        {
            ID = ID;
            MapPoint = new MapPoint(room, position);
        }

        public MapSchematic(int id, Room room, Vector3 position, Vector3 roation) : this(id, room, position)
            => Rotation = roation;

        public MapSchematic(int id, Room room, Vector3 position, Vector3 roation, Vector3 scale) : this(id, room, position, roation)
            => Scale = scale;

        public int ID { get; set; }
        public SerializedMapPoint MapPoint { get; set; }
        public SerializedVector3 Scale { get; set; }
        public SerializedVector3 Rotation { get; set; }

        public SynapseObject Spawn()
        {
            var schematic = SchematicHandler.Get.SpawnSchematic(ID, MapPoint.Parse().Position, (Vector3)Rotation);
            schematic.Scale = Scale;
            schematic.ObjectData.Add(Plugin.ObjectKeyID, ID);
            return schematic;
        }
    }
}
