using Synapse.Api.Enum;
using VT_Api.Core.Enum;
using VT_Api.Core.Items;

namespace MapEditor.ToolItem
{
    [VtItemInformation(
        BasedItemType = ItemType.GunRevolver,
        ID = (int)ItemID.Mover,
        Name = "Mover"
        )]
    internal class Mover : AbstractWeapon
    {
        public override ushort Ammos => ushort.MaxValue;

        public override AmmoType AmmoType => AmmoType.Ammo44cal;

        public override int DamageAmmont => 0;

        public override bool Realod()
        {
            Item.Durabillity = Ammos;
            return false;
        }
    }
}
