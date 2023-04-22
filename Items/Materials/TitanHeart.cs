using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class TitanHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            // DisplayName.SetDefault("Titan Heart");
            // Tooltip.SetDefault("Also used at the Astral Beacon");
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
        }    }
}
