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
        ID = (int)ItemID.Destroyer,
        Name = "Destroyer"
        )]
    internal class Destroyer : AbstractWeapon, ITool
    {
        public override ushort Ammos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        public int Amount { get; set; } = 0;

        string ITool.Info => "Destroy the Schematic (shoot the cursor)";

        public override bool Realod()
        {   
            Item.Durabillity = Ammos;
            return false;
        }

        public override bool Shoot(Vector3 targetPosition, Player target) => false;

        public override bool Shoot(Vector3 targetPosition)
        {
            MapEditHandler handler;
            if (Plugin.Instance.CurentEditedMap == null || Plugin.Instance.CurentEditedMap.Name == Plugin.MapNone)
            {
                Holder.SendBroadcast(2, "You are not in edit mod", true);
                return false;
            }
            else
            {
                handler = Holder.GetOrAddComponent<MapEditHandler>();
                handler.enabled = true;
            }

            if (!Physics.Raycast(Holder.CameraReference.transform.position, Holder.CameraReference.transform.forward, out RaycastHit hitInfo, 50f))
                handler.Info = "nothing found";

            if (!hitInfo.collider.gameObject.TryGetComponent<SynapseObjectScript>(out var script))
                handler.Info = "nothing found";

            if (script.Object is SynapsePrimitiveObject primitiveObject)
            {
                Cursor.TryInteract(primitiveObject, InteractionType.Destroy, Amount, out var answer, out _);
                handler.Info = answer;
                return false;
            }

            handler.Info = "nothing found";
            return false;
        }

    }
}
