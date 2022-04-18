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
        ID = (int)ItemID.Spawner,
        Name = "Spawner"
        )]
    internal class Spawner : AbstractWeapon, ITool
    {
        public Vector3 Scale { get; set; } = Vector3.one;

        public override ushort MaxAmmos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;
        string ITool.Info => $"Spawn a Schematic selected (ID : {CurentSchematic?.ID.ToString() ?? "NONE"}, Name : {CurentSchematic?.Name ?? "NONE"})";

        public SynapseSchematic CurentSchematic { get; set; }
        private int index;
        public int Selected
        {
            get => index;
            set
            {
                if (value + index < 0)
                    index = SchematicHandler.Get.Schematics.Count - 1;
                else if (SchematicHandler.Get.Schematics.Count <= value + 1)
                    index = 0;
                else
                    index = value;
                CurentSchematic = SchematicHandler.Get.Schematics[index];
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
            if (Plugin.Instance.CurentEditedMap == null)
            {
                Holder.SendBroadcast(2, "<color=red>You need to be in Edit Mod</color>", true);
                return false;
            }

            if (CurentSchematic == null)
            {
                Holder.SendBroadcast(4, $"You need to select a schematic\nUse {Plugin.Instance.Config.KeyUp} or {Plugin.Instance.Config.KeyDown} for slected the next or earlier schematic", true);
                return false;
            }

            Holder.GetOrAddComponent<MapEditUI>().UIRuning = true;

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
