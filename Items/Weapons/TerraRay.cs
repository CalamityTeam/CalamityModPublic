using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class TerraRay : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Terra Ray");
		Item.staff[item.type] = true;
	}

    public override void SetDefaults()
    {
        item.damage = 55;
        item.magic = true;
        item.mana = 10;
        item.width = 54;
        item.height = 54;
        item.useTime = 20;
        item.useAnimation = 20;
        item.useStyle = 5;
        item.noMelee = true; //so the item's animation doesn't do damage
        item.knockBack = 5.5f;
        item.value = 1000000;
        item.rare = 8;
        item.UseSound = SoundID.Item60;
        item.autoReuse = true;
        item.shoot = mod.ProjectileType("TerraRay");
        item.shootSpeed = 6f;
    }
    
    public override void AddRecipes()
    {
        ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "LivingShard", 7);
        recipe.AddIngredient(null, "NightsRay");
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
        recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "LivingShard", 7);
        recipe.AddIngredient(null, "CarnageRay");
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
    }
}}