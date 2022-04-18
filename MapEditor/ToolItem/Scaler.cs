using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Enum;
using UnityEngine;
using VT_Api.Core.Enum;
using VT_Api.Core.Items;
using VT_Api.Extension;

namespace MapEditor.ToolItem
{
    [VtItemInformation(
       BasedItemType = ItemType.GunRevolver,
       ID = (int)ItemID.Scaler,
       Name = "Scaler"
       )]
    internal class Scaler : AbstractWeapon, ITool
    {
        public override ushort MaxAmmos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        public override bool Shoot(Vector3 targetPosition, Player target) => false;

        public int Selected { get; set; } = 0;

        string ITool.Info => $"Move the Schematic of the Amount ({Selected})";

        public override bool Realod()
        {
            Item.Durabillity = MaxAmmos;
            return false;
        }

        public override void Init()
        {
            Item.Durabillity = MaxAmmos;
        }

        public override bool Shoot(Vector3 targetPosition)
        {
            MapEditUI handler;
            if (Plugin.Instance.CurentEditedMap == null || Plugin.Instance.CurentEditedMap.Name == Plugin.MapNone)
            {
                Holder.SendBroadcast(2, "<color=red>You are not in edit mod</color>", true);
                return false;
            }
            else
            {
                handler = Holder.GetOrAddComponent<MapEditUI>();
                handler.UIRuning = true;
            }

            if (!Physics.Raycast(Holder.CameraReference.transform.position, Holder.CameraReference.transform.forward, out RaycastHit hitInfo, 50f))
            {
                handler.Info = "nothing found";
                return false;
            }
            if (!hitInfo.collider.gameObject.TryGetComponent<SynapseObjectScript>(out var script))
            {
                handler.Info = "nothing found";
                return false;
            }
            if (script.Object is SynapsePrimitiveObject primitiveObject)
            {
                Cursor.TryInteract(primitiveObject, InteractionType.Scale, Selected, out var answer, out _);
                handler.Info = answer;
                return false;
            }

            handler.Info = "nothing found";
            return false;
        }
    }
}
