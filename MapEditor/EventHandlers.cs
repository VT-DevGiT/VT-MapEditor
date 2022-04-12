using Synapse;
using Synapse.Api;
using System;
using System.Linq;

namespace MapEditor
{
    public class EventHandlers
    {
        public EventHandlers()
        {
            Server.Get.Events.Round.WaitingForPlayersEvent += OnWhaiting;
        }

        private void OnWhaiting()
        {
            if (!Plugin.Instance.Config.MapsLoaded.Any() || 
                (Plugin.Instance.Config.MapsLoaded.Count == 1 && Plugin.Instance.Config.MapsLoaded[1] == Plugin.MapNone))
                return;

            foreach (var mapToLoad in Plugin.Instance.Config.MapsLoaded)
            {
                if (!Plugin.Instance.Maps.TryGetValue(mapToLoad, out var map))
                {
                    Logger.Get.Error($"Map not found ! map name {mapToLoad}");
                }
                Plugin.Instance.SpawnMap(map);
            }
        }
    }
}