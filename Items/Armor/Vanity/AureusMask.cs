using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AureusMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Aureus Mask");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }

        public override bool DrawHead()
        {
            return false;
        }
    }
}
