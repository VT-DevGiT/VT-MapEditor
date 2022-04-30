using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Enum;
using System.Linq;
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


        public Cursor SlectedObject { get; set; } = null;
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

        public void Select(MapEditUI handler)
        {
            if (!Physics.Raycast(Holder.CameraReference.transform.position, Holder.CameraReference.transform.forward, out RaycastHit hitInfo, 50f))
                handler.Info = "nothing found";
            else
            {
                var compnts = hitInfo.collider.GetComponentsInParent(typeof(Component));

                var primitiveObject = compnts.FirstOrDefault(c => c is SynapseObjectScript) as SynapseObjectScript;

                if (primitiveObject == null || primitiveObject.Object is not DefaultSynapseObject defaultSynapseObject)
                {
                    handler.Info = "You need to interact with a cursor";
                    return;
                }

                if (!Cursor.TryGetCursor(defaultSynapseObject, out var cursor))
                    handler.Info = "You need to interact with a cursor";
                else
                    SlectedObject = cursor;
            }
            return;
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
                    Select(handler);
                    break;

                    //TODO
            }
            return false;
        }
    }
}
