using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
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
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 10);
            item.rare = 3;
            item.bait = 40;
        }
    }
}
