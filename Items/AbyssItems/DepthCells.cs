using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class DepthCells : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depth Cells");
            Tooltip.SetDefault("The cells of abyssal creatures");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 3;
        }
    }
}
