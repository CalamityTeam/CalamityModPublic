using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class Shimmerspark : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Shimmerspark");
		Tooltip.SetDefault("Fires stars when enemies are near");
	}

    public override void SetDefaults()
    {
    	item.CloneDefaults(ItemID.Chik);
        item.damage = 32;
        item.useTime = 25;
        item.useAnimation = 25;
        item.useStyle = 5;
        item.channel = true;
        item.melee = true;
        item.knockBack = 3.2f;
        item.value = 100000;
        item.rare = 5;
        item.autoReuse = true;
        item.shoot = mod.ProjectileType("ShimmersparkProjectile");
    }
    
    public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "VerstaltiteBar", 6);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}