using MapGeneration;
using Synapse;
using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Plugin;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using VT_Api.Core.Enum;
using VT_Api.Core.Plugin;
using Logger = Synapse.Api.Logger;

namespace MapEditor
{
    [PluginInformation(
            Author = "VT",
            Description = "allows you to place schematics on the map and load them each round",
            LoadPriority = 0,
            Name = "VT-MapEditor",
            SynapseMajor = SynapseVersion.Major,
            SynapseMinor = SynapseVersion.Minor,
            SynapsePatch = SynapseVersion.Patch,
            Version = "v1.0.0"
            )]
    public class Plugin : VtAbstractPlugin<Plugin, EventHandlers, Config>
    {

        #region Properties & Variable
        public const string ObjectKeyMap = "Map";
        public const string ObjectKeyRoom = "Room";
        public const string MapNone = "NONE";
        public const string Fixed = "Fix";

        public static int[] ToolsID =
        {
            (int)ItemID.Spawner,
            (int)ItemID.Destroyer,
            (int)ItemID.Mover,
            (int)ItemID.Rotationer,
            (int)ItemID.Scaler,
            (int)ItemID.Selector,
        };

        public override bool AutoRegister => true;

        private Map curentEditedMap;
        public Map CurentEditedMap 
        {
            get => curentEditedMap;
            set
            {
                Cursor.ResetID();

                if (curentEditedMap != null && curentEditedMap.Name != MapNone)
                    DespawnEditingMap();
             
                if (value != null && value.Name != MapNone)
                    SpawnMap(value, true);

                curentEditedMap = value;
            }
        }
        public List<Map> LoadedMaps { get; set; } = new List<Map>();

        public Dictionary<string, Map> Maps { get; } = new Dictionary<string, Map>();
        public List<SynapseObject> SpawnedObjects { get; } = new List<SynapseObject>();
        public List<SynapseObject> EditingObjects { get; } = new List<SynapseObject>();

        private string _mapSchematicDirectory = Path.Combine(Server.Get.Files.SchematicDirectory, "mapSchematics");
        public string MapSchematicDirectory
        {
            get
            {
                if (!Directory.Exists(_mapSchematicDirectory))
                    Directory.CreateDirectory(_mapSchematicDirectory);

                return _mapSchematicDirectory;
            }
            private set
            {
                _mapSchematicDirectory = value;
            }
        }

        public Dictionary<Player, Cursor> PlayerSlectedObject { get; } = new Dictionary<Player, Cursor>();
        #endregion

        #region Plugin
        public override void Load()
        {
            base.Load();
            MapSchematicDirectory = Path.Combine(Server.Get.Files.SchematicDirectory, "mapSchematics");
            LoadMapSchematics();
            foreach (var map in Maps)
            {
                if (Config.MapsLoaded.Contains(map.Value.Name))
                    SpawnMap(map.Value);
            }
        }

        public override void ReloadConfigs()
        {
            base.ReloadConfigs();
            LoadMapSchematics();
            foreach (var map in Maps)
            {
                if (Config.MapsLoaded.Contains(map.Value.Name))
                    SpawnMap(map.Value);
            }
        }
        #endregion

        #region Methods
        public Map GetMap(string name)
            => Maps.ContainsKey(name.ToLower()) ? Maps[name.ToLower()] : null;

        public Map GetOrAddMap(string name)
        {
            name = name.ToLower();
            if (Maps.ContainsKey(name))
                return Maps[name];

            var map = new Map(name);
            Maps.Add(name, map);
            return map;
        }

        public void DespawnMap(Map map)
        {
            if (LoadedMaps.Contains(map))
                LoadedMaps.Remove(map);
            
            for (var i = 0; SpawnedObjects.Count > i; i++)
            {
                if (!SpawnedObjects[i].ObjectData.ContainsKey(ObjectKeyMap))
                    continue;
                if ((SpawnedObjects[i].ObjectData[ObjectKeyMap] as string) == map.Name)
                {
                    var @object = SpawnedObjects[i];
                    SpawnedObjects.Remove(@object);
                    @object.Destroy();
                    i--;
                }
            }
        }

        public void DespawnEditingMap()
        {
            while (EditingObjects.Any())
            {
                var @object = EditingObjects.First();
                EditingObjects.Remove(@object);
                @object.Destroy();
            }
        }

        public void DespawnMaps()
        {
            while (SpawnedObjects.Any())
            {
                var obj = SpawnedObjects[0];
                if (obj?.GameObject != null)
                    obj.Destroy();
                SpawnedObjects.Remove(obj);
            }
            SpawnedObjects.Clear();
            LoadedMaps.Clear();
        }

        public void SpawnMap(Map map, bool editing = false)
        {
            if (!LoadedMaps.Contains(map))
                LoadedMaps.Add(map);
            var newObjects = map.Spawn(editing);
            if (editing) foreach (var newObject in newObjects)
            {
                new Cursor(newObject, map);
                EditingObjects.Add(newObject);
            }
            else foreach (var newObject in newObjects)
                SpawnedObjects.Add(newObject);
        }

        public void LoadMapSchematics()
        {
            foreach (var file in Directory.GetFiles(MapSchematicDirectory, "*.syml"))
            {
                try
                {
                    var syml = new SYML(file);
                    syml.Load();
                    if (syml.Sections.Count == 0) continue;
                    var section = syml.Sections.First().Value;
                    var map = section.LoadAs<Map>();
                    
                    foreach (var schematic in map.MapSchematics)
                    {
                        if (!SchematicHandler.Get.IsIDRegistered(schematic.ID))
                        {
                            Logger.Get.Error($"Schematic ID not regitred, MapSchematic ignored !\n\tID : {schematic.ID}\n\tFile : {file}");
                            continue;
                        }
                    }
                    if (Maps.ContainsKey(map.Name.ToLower()))
                    {
                        Logger.Get.Error($"MapSchematic already registered, MapSchematic ignored !\n\tFile : {file}");
                        continue;
                    }

                    Maps.Add(map.Name.ToLower(), map);
                }
                catch (Exception ex)
                {
                    Logger.Get.Error($"Loading Schematic failed - path: {file}\n{ex}");
                }
            }
        }

        public void SaveMapSchematic(Map map, string fileName)
        {
            map.MapSchematics.Clear();

            foreach (var editingObject in EditingObjects)
            {
                var room = editingObject.ObjectData[ObjectKeyRoom];
                SerializedMapPoint position;
                Vector3 rotation;

                switch (true)
                {
                    case true when room is string name:
                        {
                            name = Regex.Replace(name, @" ?\(.*?\)", string.Empty);
                            name = Regex.Replace(name, " ", string.Empty);
                            name = MapSchematic.Foreach + name;
                            var refRoom = Synapse.Api.Map.Get.Rooms.FirstOrDefault(r => r.RoomName == name);
                            if (refRoom == null)
                            {
                                Logger.Get.Error($"No {name} found !");
                                continue;
                            }
                            var vector = refRoom.GameObject.transform.InverseTransformPoint(editingObject.Position);
                            position = new SerializedMapPoint(name, vector.x, vector.y, vector.z);
                            rotation = editingObject.Rotation.eulerAngles - refRoom.Rotation.eulerAngles;
                        }
                        break;
                    case true when room is Fixed:
                        {
                            var outside = SynapseController.Server.Map.GetRoom(RoomName.Outside);
                            position = new MapPoint(outside, outside.GameObject.transform.InverseTransformPoint(editingObject.Position));
                            rotation = editingObject.Rotation.eulerAngles;
                        }
                        break;
                    case true when room is Room synapseRoom:
                        {
                            position = new MapPoint(synapseRoom, editingObject.Position);
                            rotation = editingObject.Rotation.eulerAngles;
                        }
                        break;
                    default:
                        Logger.Get.Error($"Unknow Room !");
                        continue;
                        
                }

                var schematic = new MapSchematic(editingObject.ID, position, rotation, editingObject.Scale);
                map.MapSchematics.Add(schematic);
            }

            var syml = new SYML(Path.Combine(MapSchematicDirectory, fileName + ".syml"));
            var section = new ConfigSection { Section = map.Name };
            section.Import(map);
            syml.Sections.Add(map.Name, section);
            syml.Store();
        }
        #endregion
    }
}
