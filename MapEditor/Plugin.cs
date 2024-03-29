﻿using MapGeneration;
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
using System.Xml.Linq;
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
            Version = "v1.1.1"
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

        private Map curentEditedMap = Map.None;
        public Map CurentEditedMap 
        {
            get => curentEditedMap;
            set
            {
                Cursor.ResetID();

                if (curentEditedMap.Name != MapNone)
                    DespawnEditingMap();
             
                if (value != null && value.Name != MapNone)
                    SpawnMap(value, true);
                
                curentEditedMap = value ?? Map.None;
            }
        }
        public List<Map> LoadedMaps { get; } = new List<Map>();

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
        }

        public override void ReloadConfigs()
        {
            base.ReloadConfigs();
            Maps.Clear();
            LoadMapSchematics();
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
                @object?.Destroy();
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
            var newObjects = map.Spawn(editing);
            if (editing) foreach (var newObject in newObjects)
            {
                new Cursor(newObject, map);
                EditingObjects.Add(newObject);
            }
            else
            {
                foreach (var newObject in newObjects)
                    SpawnedObjects.Add(newObject);
                if (!LoadedMaps.Contains(map))
                    LoadedMaps.Add(map);
            }
        }

        public void LoadMapSchematics()
        {
            var alredyRegisterd = string.Empty;
            var idNotRegistred = string.Empty;
            foreach (var file in Directory.GetFiles(MapSchematicDirectory, "*.syml"))
            {
                try
                {
                    var syml = new SYML(file);
                    syml.Load();
                    if (syml.Sections.Count == 0) continue;
                    var section = syml.Sections.First().Value;
                    var map = section.LoadAs<Map>();
                    
                    bool continueOut = false;
                    foreach (var schematic in map.MapSchematics)
                    {
                        if (!SchematicHandler.Get.IsIDRegistered(schematic.ID))
                        {
                            alredyRegisterd += $"\n\tID : {schematic.ID}\n\tFile : {file}";
                            continueOut = true;
                            break;
                        }
                    }
                    if (continueOut) continue;
                    if (Maps.ContainsKey(map.Name.ToLower()))
                    {
                        idNotRegistred += $"\n\tFile : {file}";
                        continue;
                    }

                    Maps.Add(map.Name.ToLower(), map);
                }
                catch (Exception ex)
                {
                    Logger.Get.Error($"Loading Schematic failed - path: {file}\n{ex}");
                }
            }
            if (!string.IsNullOrEmpty(alredyRegisterd))
                Logger.Get.Error($"Schematic ID not regitred, MapSchematic ignored !{alredyRegisterd}");
            if (!string.IsNullOrEmpty(idNotRegistred))
                Logger.Get.Error($"MapSchematic already registered, MapSchematic ignored !{idNotRegistred}");
        }

        public void SaveMapSchematic(Map map, string fileName)
        {
            map.MapSchematics.Clear();
            SerializedMapPoint position;
            Vector3 rotation;

            foreach (var editingObject in EditingObjects)
            {
                var room = editingObject.ObjectData[ObjectKeyRoom];

                switch (true)
                {
                    case true when room is Fixed:
                        {
                            var outside = SynapseController.Server.Map.GetRoom(RoomName.Outside);
                            position = new MapPoint(outside, editingObject.Position);
                            rotation = editingObject.Rotation.eulerAngles;
                        }
                        break;
                    case true when room is string name:
                        {
                            var refRoom = Synapse.Api.Map.Get.Rooms.FirstOrDefault(r => r.RoomName == name);
                            if (refRoom == null)
                            {
                                Logger.Get.Error($"No {name} found !");
                                continue;
                            }
                            name = Regex.Replace(name, @" ?\(.*?\)", string.Empty);
                            name = Regex.Replace(name, " ", string.Empty);
                            name = MapSchematic.Foreach + name;
                            var vector = refRoom.GameObject.transform.InverseTransformPoint(editingObject.Position);
                            position = new SerializedMapPoint(name, vector.x, vector.y, vector.z);
                            rotation = editingObject.Rotation.eulerAngles - refRoom.Rotation.eulerAngles;
                        }
                        break;
                    case true when room is Room synapseRoom:
                        {
                            position = new MapPoint(synapseRoom, editingObject.Position);
                            rotation = editingObject.Rotation.eulerAngles - synapseRoom.Rotation.eulerAngles;
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
