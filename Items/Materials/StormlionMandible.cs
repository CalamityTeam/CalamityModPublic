using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class StormlionMandible : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormlion Mandible");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 1, copper: 40);
            item.rare = 1;
        }
    }
}
