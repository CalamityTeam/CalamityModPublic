using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class PerforatorMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perforator Mask");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }

        public override bool DrawHead()
        {
            return false;
        }
    }
}
