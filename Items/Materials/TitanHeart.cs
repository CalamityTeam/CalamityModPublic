using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class TitanHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Heart");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.AncientBattleArmorMaterial);
        }
    }
}
