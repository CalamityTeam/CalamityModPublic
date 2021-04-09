using Terraria;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture.Fountains;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture.Fountains
{
    public class AstralFountainItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Water Fountain");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 42;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.White;
            item.createTile = ModContent.TileType<AstralFountainTile>();
        }
    }
}
