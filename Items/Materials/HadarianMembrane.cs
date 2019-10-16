using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class HadarianMembrane : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadarian Membrane");
            Tooltip.SetDefault("The membrane of an astral creature's wings");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 50);
            item.rare = 7;
        }
    }
}
