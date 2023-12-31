﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedCrystal : ModItem, ILocalizedModType
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureProfaned.ProfanedCrystal>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddIngredient(ItemID.Glass, 50).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient<ProfanedCrystalWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
