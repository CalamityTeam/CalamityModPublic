using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Ores
{
    public class CharredOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Ores.CharredOre>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 15);
            item.rare = ItemRarityID.LightPurple;
        }
    }
}
