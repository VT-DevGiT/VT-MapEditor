using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public override ushort Ammos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        public override bool Shoot(Vector3 targetPosition, Player target) => false;


        public Cursor Slected { get; } = null;
        public int Amount { get; set; } = 0;

        string ITool.Info
        {
            get
            {
                switch (Amount)
                {
                    case 0 when Slected == null:
                        return $"Fire on a cursor for select the Object";
                    case 0 :
                        return $"Selected object (ID : {Slected.AttachedObject.ID}, Name :{Slected.AttachedObject.Name})";
                    case 1 when Slected != null:
                        return $"Move the slected Object (ID : {Slected.AttachedObject.ID}, Name :{Slected.AttachedObject.Name})";
                    case 2 when Slected != null:
                        return $"Copy the seleted Object (ID : {Slected.AttachedObject.ID}, Name :{Slected.AttachedObject.Name})";
                    case 1:
                    case 2:
                    default:
                        return "You need to select an Object !";
                }
            }
        }

        public override bool Realod()
        {
            Item.Durabillity = Ammos;
            return false;
        }

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
                Cursor.TryInteract(primitiveObject, InteractionType.Selector, Amount, out var answer, out _);
                handler.Info = answer;
                return false;
            }

            handler.Info = "nothing found";
            return false;
        }
    }
}
