
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Bar");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.AstralBar>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(3).
                AddIngredient<AstralOre>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
