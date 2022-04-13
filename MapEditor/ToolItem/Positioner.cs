using Synapse.Api;
using Synapse.Api.CustomObjects;
using Synapse.Api.Enum;
using UnityEngine;
using VT_Api.Core.Enum;
using VT_Api.Core.Items;

namespace MapEditor.ToolItem
{
    [VtItemInformation(
        BasedItemType = ItemType.GunRevolver,
        ID = (int)ItemID.Positioner,
        Name = "Positioner"
        )]
    internal class Positioner : AbstractWeapon, ITool
    {
        public SynapseSchematic CurentSchematic { get; set; }
        private int index;
        public int Amount 
        { 
            get => index; 
            set
            {
                if (value < 0)
                    index = SchematicHandler.Get.Schematics.Count;
                else if (SchematicHandler.Get.Schematics.Count <= value + 1)
                    index = 0;
                else
                    index = value;

            }
        }
        public Vector3 Scale { get; set; } = Vector3.one;

        public override ushort Ammos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        string ITool.Info => $"Spawn a Schematic selected (ID : {CurentSchematic.ID}, Name : {CurentSchematic.Name})";

        public override bool Realod()
        {
            Item.Durabillity = Ammos;
            return false;
        }

        public override bool Shoot(Vector3 targetPosition)
        {
            if (Plugin.Instance.CurentEditedMap == null)
            {
                Holder.SendBroadcast(2, "You need to be in Edit Mod", true);
                return false;
            }

            var rotation = new Vector3(Holder.Rotation.x, 0, 0);
            var schematic = SchematicHandler.Get.SpawnSchematic(CurentSchematic, Holder.Position, rotation);
            schematic.Scale = Scale;

            new Cursor(schematic);

            schematic.ObjectData.Add(Plugin.ObjectKeyID, CurentSchematic.ID);
            schematic.ObjectData.Add(Plugin.ObjectKeyMap, Plugin.Instance.CurentEditedMap.Name);
            Plugin.Instance.EditingObjects.Add(schematic);
            return false;
        }

        public override bool Shoot(Vector3 targetPosition, Player target) => false;

    }
}
