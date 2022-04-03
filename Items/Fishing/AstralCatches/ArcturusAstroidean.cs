using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class ArcturusAstroidean : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arcturus Astroidean");
            Tooltip.SetDefault("Increases fishing power if used in the Astral Infection or Sulphurous Sea");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Orange;
            Item.bait = 40;
        }
    }
}
