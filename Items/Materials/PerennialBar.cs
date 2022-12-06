using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("DraedonBar")]
    public class PerennialBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Perennial Bar");
			ItemID.Sets.SortingPriorityMaterials[Type] = 92; // Shroomite Bar
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.PerennialBar>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Lime;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialOre>(4).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
