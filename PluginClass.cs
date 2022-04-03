using Synapse;
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
    public class PluginClass : VtAbstractPlugin<PluginClass, EventHandlers, PluginConfig>
    {
        public override bool AutoRegister => true;

        public Map CurentEditedMap { get; set; }
        public List<Map> LoadedMaps { get; set; }

        public HashSet<Map> Maps { get; private set; } = new HashSet<Map>();
        private List<SynapseObject> SpawnedObjects = new List<SynapseObject>();


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
                if (Config.MapsLoaded.Contains(map.Name))
                    SpawnMap(map);
            }
        }

        public override void ReloadConfigs()
        {
            base.ReloadConfigs();
            LoadMapSchematics();
            foreach (var map in Maps)
            {
                if (Config.MapsLoaded.Contains(map.Name))
                    SpawnMap(map);
            }
        }


        public void DespawnMap(Map map)
        {
            if (LoadedMaps.Contains(map))
                LoadedMaps.Remove(map);

            for (var i = 0; SpawnedObjects.Count > i; i++)
            {
                if (!SpawnedObjects[i].ObjectData.ContainsKey("Map"))
                    continue;
                if ((SpawnedObjects[i].ObjectData["Map"] as string) == map.Name)
                {
                    SpawnedObjects[i].Destroy();
                    SpawnedObjects.Remove(SpawnedObjects[i]);
                    i--;
                }
            }
        }

        public void DespawnMaps()
        {
            foreach (var synapseObject in SpawnedObjects)
                synapseObject.Destroy();
                
            SpawnedObjects.Clear();
            LoadedMaps.Clear();
        }

        public void SpawnMap(Map map)
        {
            if (!LoadedMaps.Contains(map))
                LoadedMaps.Add(map);
            var newObjects = map.Spawn();
            foreach (var newObject in newObjects)
            {
                SpawnedObjects.Add(newObject);
                newObject.ObjectData["Map"] = map.Name;
            }
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
                    
                    foreach (var shematic in map.MapShematics)
                    {
                        if (!SchematicHandler.Get.IsIDRegistered(shematic.ID))
                        {
                            Logger.Get.Error($"Shematic ID not regitred, MapShematic ignored !\n\tID : {shematic.ID}\n\tFile : {file}");
                            continue;
                        }
                    }
                    if (Maps.Contains(map))
                    {
                        Logger.Get.Error($"MapShematic already registered, MapShematic ignored !\n\tFile : {file}");
                        continue;
                    }

                    Maps.Add(map);
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
