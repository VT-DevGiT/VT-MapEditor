using MapEditor.ToolItem;
using Synapse;
using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Events.SynapseEventArguments;
using System.Linq;
using VT_Api.Extension;

namespace MapEditor
{
    public class EventHandlers
    {
        public EventHandlers()
        {
            Server.Get.Events.Round.WaitingForPlayersEvent += OnWhaiting;
            Server.Get.Events.Player.PlayerKeyPressEvent += OnKeyPress;
        }

        private void OnKeyPress(PlayerKeyPressEventArgs ev)
        {
            if (ev.Player.ItemInHand == null)
                return;
            if (ev.KeyCode == Plugin.Instance.Config.KeyDown)
            {
                if (ev.Player.ItemInHand.TryGetScript(out var script) && script is ITool tool)
                    tool.Amount--;
            }
            else if (ev.KeyCode == Plugin.Instance.Config.KeyDown)
            {
                if (ev.Player.ItemInHand.TryGetScript(out var script) && script is ITool tool)
                    tool.Amount++;
            }
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