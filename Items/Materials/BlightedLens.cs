using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BlightedLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blighted Lens");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 22;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 56);
            item.rare = 5;
        }
    }
}
