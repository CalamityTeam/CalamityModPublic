using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class VictoryShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victory Shard");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
            item.rare = 1;
        }
    }
}
