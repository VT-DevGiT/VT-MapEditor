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
        public override ushort MaxAmmos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        public int Selected { get; set; } = 0;

        string ITool.Info => "Destroy the Schematic (shoot the cursor)";

        public override bool Realod()
        {   
            Item.Durabillity = MaxAmmos;
            return false;
        }

        public override void Init()
        {
            Item.Durabillity = MaxAmmos;
        }

        public override bool Shoot(Vector3 targetPosition, Player target) => false;

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

            var hits = Physics.RaycastAll(Holder.CameraReference.transform.position, 
                                          Holder.CameraReference.transform.forward, 
                                          50f, 
                                          Cursor.layer);

            if (hits.Length == 0)
            {
                Synapse.Api.Logger.Get.Info("Ray cast fail");
                handler.Info = "nothing found";
                return false;
            }

            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit.collider.TryGetComponent<SynapseObjectScript>(out var script) 
                    && script.Object is SynapsePrimitiveObject primitiveObject 
                    && Cursor.IsObjectIsCursor(primitiveObject))
                {
                    Cursor.TryInteract(primitiveObject, InteractionType.Destroy, Selected, out var answer, out _);
                    handler.Info = answer;
                    return false;
                }
            }

            handler.Info = "You need to interact with a cursor";
            return false;
        }

    }
}
