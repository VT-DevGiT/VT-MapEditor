﻿using Synapse.Api;
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
        ID = (int)ItemID.Mover,
        Name = "Mover"
        )]
    internal class Mover : AbstractWeapon, ITool
    {
        public override ushort MaxAmmos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

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
            else
            {
                var compnts = hitInfo.collider.GetComponentsInParent(typeof(Component));

                var primitiveObject = compnts.FirstOrDefault(c => c is SynapseObjectScript) as SynapseObjectScript;

                if (primitiveObject == null || primitiveObject.Object is not DefaultSynapseObject defaultSynapseObject)
                {
                    handler.Info = "You need to interact with a cursor";
                    return false;
                }

                if (!Cursor.CanInteract(defaultSynapseObject, out var answer))
                {
                    handler.Info = answer;
                    return false;
                }

                var cursor = Cursor.GetCursor(defaultSynapseObject);
                var axis = (Axis)defaultSynapseObject.ObjectData[Cursor.KeyCursor];
                cursor.Move(axis, Selected);
                handler.Info  = "Object moved";
                return true;
                
            }
        }

        public override bool Shoot(Vector3 targetPosition, Player target) => false;

    }
}
