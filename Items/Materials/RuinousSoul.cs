using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class RuinousSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruinous Soul");
            Tooltip.SetDefault("A shard of the distant past");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 7, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
        }
    }
}
