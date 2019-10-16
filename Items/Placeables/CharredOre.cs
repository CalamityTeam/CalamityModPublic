using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class CharredOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<CharredOre>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 15);
            item.rare = 6;
        }
    }
}
