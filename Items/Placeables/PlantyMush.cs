using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class PlantyMush : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Planty Mush");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<PlantyMush>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 6);
            item.rare = 3;
        }
    }
}
