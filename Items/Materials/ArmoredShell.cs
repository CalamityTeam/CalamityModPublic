using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ArmoredShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armored Shell");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 7, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
        }
    }
}
