using MapGeneration;
using Synapse;
using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapEditor
{
    [Serializable]
    public class MapSchematic
    {
        public const string Foreach = "foreach_";

        public MapSchematic()
        {
            ID = -1;
            MapPoint = new SerializedMapPoint(Plugin.MapNone, 0, 0, 0);
            Scale = Vector3.one;
            Rotation = Vector3.one;
        }

        public MapSchematic(int id, Room room, Vector3 position) : this(id, room, position, Vector3.zero) { }

        public MapSchematic(int id, Room room, Vector3 position, Vector3 roation) : this(id, room, position, roation, Vector3.one) { }

        public MapSchematic(int id, Room room, Vector3 position, Vector3 roation, Vector3 scale) 
        {
            Scale = scale;
            Rotation = roation;
            ID = id;
            MapPoint = new MapPoint(room, position);
        }
        public MapSchematic(int id, SerializedMapPoint mapPoint) : this(id, mapPoint, Vector3.zero) { }

        public MapSchematic(int id, SerializedMapPoint mapPoint, Vector3 roation) : this (id, mapPoint, roation, Vector3.one) { }

        public MapSchematic(int id, SerializedMapPoint mapPoint, Vector3 rotation, Vector3 scale)
        {
            ID = id;
            MapPoint = mapPoint;
            Rotation = rotation;
            Scale = scale;
        }


        public int ID { get; set; }
        public SerializedMapPoint MapPoint { get; set; }
        public SerializedVector3 Scale { get; set; }
        public SerializedVector3 Rotation { get; set; }

        public List<SynapseObject> Spawn(bool editing = false)
        {
            if (editing)
            {
                if (MapPoint.Room.StartsWith(Foreach))
                    return SpawnForeachEdit();
                else return SpawnForRoomEdit();
            }
            else 
            {
                if (MapPoint.Room.StartsWith(Foreach))
                    return SpawnForeach();
                else return SpawnForRoom();
            }
        }

        private List<SynapseObject> SpawnForeach()
        {
            var name = MapPoint.Room.Replace(Foreach, string.Empty);
            var rooms = Synapse.Api.Map.Get.Rooms.Where(r => r.RoomName.Contains(name));
            if (!rooms.Any())
            {
                Synapse.Api.Logger.Get.Error($"Error to spawn a Schematic, this room {name} is not referenced");
                return null;
            }
            var schematics = new List<SynapseObject>();
            foreach (var room in rooms)
            {
                var position = room.GameObject.transform.TransformPoint(new Vector3(MapPoint.X, MapPoint.Y, MapPoint.Z));
                var rotation = Rotation + room.Rotation.eulerAngles;
                var schematic = SchematicHandler.Get.SpawnSchematic(ID, position, rotation);
                schematics.Add(schematic);
            }
            return schematics;
        }
        private List<SynapseObject> SpawnForeachEdit()
        {
            var name = MapPoint.Room.Replace(Foreach, string.Empty);
            var room = Synapse.Api.Map.Get.Rooms.FirstOrDefault(r => r.RoomName.Contains(name));
            if (room == null)
            {
                Synapse.Api.Logger.Get.Error($"Error to spawn a Schematic, this room {name} is not referenced");
                return null;
            }
            var position = room.GameObject.transform.TransformPoint(new Vector3(MapPoint.X, MapPoint.Y, MapPoint.Z));
            var rotation = Rotation + room.Rotation.eulerAngles;
            var schematic = SchematicHandler.Get.SpawnSchematic(ID, position, rotation);
            schematic.ObjectData[Plugin.ObjectKeyRoom] = MapPoint.Room;
            return new List<SynapseObject>() { schematic };
        }

        private List<SynapseObject> SpawnForRoom()
        {
            if (Synapse.Api.Map.Get.Rooms.Find(r => r.RoomName == MapPoint.Room) == null)
                return null;
            var schematic = SchematicHandler.Get.SpawnSchematic(ID, MapPoint.Parse().Position, (Vector3)Rotation);
            schematic.Scale = Scale;
            return new List<SynapseObject>() { schematic };
        }
        private List<SynapseObject> SpawnForRoomEdit()
        {
            var room = Synapse.Api.Map.Get.Rooms.Find(r => r.RoomName == MapPoint.Room);
            if (room == null)
            {
                Synapse.Api.Logger.Get.Warn("Object cant be spawn ! use an other seed and dont save or the object be destroy !");
                Server.Get.Players.ForEach(p => p.SendBroadcast(5, "<color=red>Object cant be spawn ! use an other seed and dont save or the object be destroy !</color>", true));
                return null;
            }
            var schematic = SchematicHandler.Get.SpawnSchematic(ID, MapPoint.Parse().Position, (Vector3)Rotation);
            schematic.Scale = Scale;
            if (room.RoomType == RoomName.Outside)
                schematic.ObjectData[Plugin.ObjectKeyRoom] = Plugin.Fixed;
            else
                schematic.ObjectData[Plugin.ObjectKeyRoom] = room;
            return new List<SynapseObject>() { schematic };
        }
    }
}
