using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class MeldBlob : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meld Blob");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 20);
            item.rare = 9;
        }
    }
}
