using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class PurifiedGel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purified Gel");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 2, 50, 0);
            item.rare = 4;
        }
    }
}
