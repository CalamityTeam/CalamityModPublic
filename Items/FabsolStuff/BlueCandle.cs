using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BlueCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Weightless Candle");
            Tooltip.SetDefault("When placed, nearby players gain 15% movement speed, 10% wing time, and 5% acceleration\n" +
                "'The floating flame seems to uplift your very spirit'");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 40;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = Item.buyPrice(0, 50, 0, 0);
            item.rare = 6;
            item.createTile = ModContent.TileType<BlueCandle>();
        }
    }
}
