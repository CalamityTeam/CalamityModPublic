using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("VerstaltiteBar")]
    public class CryonicBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Cryonic Bar");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<CryonicBarTile>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 60);
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicOre>(5).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
