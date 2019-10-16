using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
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
