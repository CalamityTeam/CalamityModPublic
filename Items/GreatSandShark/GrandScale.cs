using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class GrandScale : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Scale");
            Tooltip.SetDefault("Large scale of an apex predator");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 4, 50, 0);
            item.rare = 7;
        }
    }
}
