﻿using Synapse;
using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Plugin;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VT_Api.Core.Plugin;

namespace MapEditor
{
    [PluginInformation(
            Author = "VT",
            Description = "allows you to place schematics on the map and load them each round",
            LoadPriority = 0,
            Name = "MapEditor",
            SynapseMajor = SynapseVersion.Major,
            SynapseMinor = SynapseVersion.Minor,
            SynapsePatch = SynapseVersion.Patch,
            Version = "v1.0.0"
            )]
    public class Plugin : VtAbstractPlugin<Plugin, EventHandlers, Config>
    {
        public const string ObjectKeyMap = "Map";
        public const string ObjectKeyID = "ID";

        public const string MapNone = "NONE";

        public override bool AutoRegister => true;

        private Map curentEditedMap;
        public Map CurentEditedMap 
        {
            get => curentEditedMap;
            set
            {
                if (curentEditedMap != null && curentEditedMap.Name != MapNone)
                    DespawnEditingMap();
                
                SpawnMap(value, true);
                curentEditedMap = value;
            }
        }
        public List<Map> LoadedMaps { get; set; }

        public Dictionary<string, Map> Maps { get; private set; } = new Dictionary<string, Map>();
        public List<SynapseObject> SpawnedObjects { get; } = new List<SynapseObject>();
        public List<SynapseObject> EditingObjects { get; } = new List<SynapseObject>();

        private string _mapSchematicDirectory;
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

        public Map GetMap(string name)
            => Plugin.Instance.Maps.ContainsKey(name) ? Plugin.Instance.Maps[name] : null;
        

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
                    SpawnedObjects[i].Destroy();
                    SpawnedObjects.Remove(SpawnedObjects[i]);
                    i--;
                }
            }
        }

        public void DespawnEditingMap()
        {
            while (EditingObjects.Count != 0)
            {
                SpawnedObjects[0].Destroy();
                SpawnedObjects.Remove(SpawnedObjects[0]);
            }
        }

        public void DespawnMaps()
        {
            foreach (var synapseObject in SpawnedObjects)
                synapseObject.Destroy();
                
            SpawnedObjects.Clear();
            LoadedMaps.Clear();
        }

        public void SpawnMap(Map map, bool editing = false)
        {
            if (!LoadedMaps.Contains(map))
                LoadedMaps.Add(map);
            var newObjects = map.Spawn();
            if (!editing)
                foreach (var newObject in newObjects)
                    SpawnedObjects.Add(newObject);
            else
                foreach (var newObject in newObjects)
                    EditingObjects.Add(newObject);
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
                    
                    foreach (var shematic in map.MapSchematic)
                    {
                        if (!SchematicHandler.Get.IsIDRegistered(shematic.ID))
                        {
                            Logger.Get.Error($"Shematic ID not regitred, MapShematic ignored !\n\tID : {shematic.ID}\n\tFile : {file}");
                            continue;
                        }
                    }
                    if (Maps.ContainsKey(map.Name))
                    {
                        Logger.Get.Error($"MapShematic already registered, MapShematic ignored !\n\tFile : {file}");
                        continue;
                    }

                    Maps.Add(map.Name, map);
                }
                catch (Exception ex)
                {
                    Logger.Get.Error($"Loading Schematic failed - path: {file}\n{ex}");
                }
            }
        }

        public void SaveMapSchematic(Map map, string fileName)
        {
            var syml = new SYML(Path.Combine(Server.Get.Files.SchematicDirectory, fileName + ".syml"));
            var section = new ConfigSection { Section = map.Name };
            section.Import(map);
            syml.Sections.Add(map.Name, section);
            syml.Store();
        }
    }
}
