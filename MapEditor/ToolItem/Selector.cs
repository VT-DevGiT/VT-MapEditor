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
       ID = (int)ItemID.Selector,
       Name = "Selector"
       )]
    public class Selector : AbstractWeapon, ITool
    {
        public override ushort MaxAmmos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        public override bool Shoot(Vector3 targetPosition, Player target) => false;


        public Cursor SlectedObject { get; } = null;
        public int Selected { get; set; } = 0;

        string ITool.Info
        {
            get
            {
                switch (Selected)
                {
                    case 0 when SlectedObject == null:
                        return $"Fire on a cursor for select the Object";
                    case 0 :
                        return $"Selected object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 1 when SlectedObject != null:
                        return $"Move the slected Object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 2 when SlectedObject != null:
                        return $"Copy the seleted Object (ID : {SlectedObject.AttachedObject.ID}, Name :{SlectedObject.AttachedObject.Name})";
                    case 1:
                    case 2:
                    default:
                        return "You need to select an Object !";
                }
            }
        }

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
            switch (Selected)
            {
                case 0:
                    if (!Physics.Raycast(Holder.CameraReference.transform.position, Holder.CameraReference.transform.forward, out RaycastHit hitInfo, 50f))
                        handler.Info = "nothing found";
                    else if (!hitInfo.collider.gameObject.TryGetComponent<SynapseObjectScript>(out var script))
                        handler.Info = "nothing found";
                    else if (script.Object is SynapsePrimitiveObject primitiveObject)
                    {
                        Cursor.TryInteract(primitiveObject, InteractionType.Selector, Selected, out var answer, out _);
                        handler.Info = answer;
                    }
                    else
                        handler.Info = "nothing found";
                    return false;
                    //TODO
            }
            return false;
        }
    }
}
