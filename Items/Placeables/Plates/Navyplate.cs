using CalamityMod.Items.Placeables.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Plates
{
    public class Navyplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Navyplate");
            Tooltip.SetDefault("It resonates with otherworldly energy.");
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Plates.Navyplate>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 3);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddIngredient(ItemID.Obsidian, 25).
                AddIngredient<SeaPrism>().
                AddTile(TileID.Hellforge).
                Register();
            CreateRecipe().
                AddIngredient<NavyplateWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
