using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Ectoblood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ectoblood");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 32;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 16);
            item.rare = 8;
        }
    }
}
