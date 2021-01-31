using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Ores
{
    public class AerialiteOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aerialite Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Ores.AerialiteOre>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 6);
            item.rare = ItemRarityID.Orange;
        }
    }
}
