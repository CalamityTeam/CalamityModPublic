﻿using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class SmoothBrimstoneSlag : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAshen.SmoothBrimstoneSlag>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrimstoneSlag>().
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<SmoothBrimstoneSlagWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<AshenPlatform>(2).
                Register();
        }
    }
}
