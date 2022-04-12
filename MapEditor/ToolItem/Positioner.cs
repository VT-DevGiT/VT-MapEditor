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
    internal class Positioner : AbstractWeapon
    {
        public SynapseSchematic CurentSchematic { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;

        public override ushort Ammos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        public override bool Realod()
        {
            Item.Durabillity = Ammos;
            return false;
        }

        public override bool Shoot(Vector3 targetPosition)
        {
            if (Plugin.Instance.CurentEditedMap == null)
            {
                //warn the player
                return false;
            }

            var rotation = new Vector3(Holder.Rotation.x, 0, 0);
            var schematic = SchematicHandler.Get.SpawnSchematic(CurentSchematic, targetPosition, rotation);
            schematic.Scale = Scale;

            schematic.ObjectData.Add(Plugin.ObjectKeyID, CurentSchematic.ID);
            schematic.ObjectData.Add(Plugin.ObjectKeySave, false.ToString());
            schematic.ObjectData.Add(Plugin.ObjectKeyMap, Plugin.Instance.CurentEditedMap.Name);
            return false;
        }

        public override bool Shoot(Vector3 targetPosition, Player target) => false;

    }
}
