using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Stardust : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 18;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
            item.rare = 5;
        }
    }
}
