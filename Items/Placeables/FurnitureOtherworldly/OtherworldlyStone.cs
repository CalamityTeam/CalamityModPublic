using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureOtherworldly
{
    [LegacyName("OccultStone")]
    public class OtherworldlyStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Otherworldly Stone");
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureOtherworldly.OtherworldlyStone>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).AddRecipeGroup("AnyStoneBlock", 150).AddIngredient(ModContent.ItemType<DarkPlasma>()).AddIngredient(ModContent.ItemType<ArmoredShell>()).AddIngredient(ModContent.ItemType<TwistingNether>()).AddIngredient(ItemID.Silk, 15).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<OtherworldlyStoneWall>(), 4).AddTile(TileID.WorkBenches).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<OtherworldlyPlatform>(), 2).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
