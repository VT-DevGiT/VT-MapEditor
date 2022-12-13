using MapEditor.ToolItem;
using MEC;
using Synapse;
using Synapse.Api.Events.SynapseEventArguments;
using System.Linq;
using VT_Api.Core.Enum;
using VT_Api.Core.Events.EventArguments;
using VT_Api.Extension;

namespace MapEditor
{
    public class EventHandlers
    {
        public EventHandlers()
        {
            Server.Get.Events.Round.RoundStartEvent += OnStart;
            Server.Get.Events.Player.PlayerKeyPressEvent += OnKeyPress;
            VtController.Get.Events.Item.CheckLimitItemEvent += OnCheckLimit;
        }

        private void OnCheckLimit(CheckLimitItemEventArgs ev)
        {
            if (Plugin.ToolsID.Contains(ev.ExedentingItem.ID))
                ev.Allow = true;
        }

        private void OnKeyPress(PlayerKeyPressEventArgs ev)
        {
            if (!ev.Player.ItemInHand.IsDefined())
                return;
            
            if (ev.KeyCode == Plugin.Instance.Config.KeyDown)
            {
                if (ev.Player.ItemInHand.TryGetScript(out var script) && script is ITool tool)
                    tool.Selected--;
            }
            else if (ev.KeyCode == Plugin.Instance.Config.KeyUp)
            {
                if (ev.Player.ItemInHand.TryGetScript(out var script) && script is ITool tool)
                    tool.Selected++;
            }
            else if (ev.KeyCode == Plugin.Instance.Config.KeyReset)
            {
                if (ev.Player.ItemInHand.TryGetScript(out var script) && script is ITool tool)
                    tool.Selected = 0;
            }
        }

        private void OnStart()
        {
            Plugin.Instance.CurentEditedMap = null;
            Plugin.Instance.DespawnMaps();

            if (!Plugin.Instance.Config.MapsLoaded.Any() || 
                (Plugin.Instance.Config.MapsLoaded.Count == 1 && Plugin.Instance.Config.MapsLoaded[0] == Plugin.MapNone))
                return;
           
            foreach (var mapName in Plugin.Instance.Config.MapsLoaded)
            {
                var map = Plugin.Instance.GetMap(mapName);
                if (map == null)
                {
                    Synapse.Api.Logger.Get.Error($"Map not found ! map name {mapName}");
                    continue;
                }
                Plugin.Instance.SpawnMap(map);
            }
            
            
        }


    }
}