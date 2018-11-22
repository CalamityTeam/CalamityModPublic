using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Cryogen {
public class SnowstormStaff : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Snowstorm Staff");
		Tooltip.SetDefault("Fires a snowflake that follows the mouse cursor");
	}

    public override void SetDefaults()
    {
        item.damage = 53;
        item.magic = true;
        item.channel = true;
        item.mana = 13;
        item.width = 66;
        item.height = 66;
        item.useTime = 22;
        item.scale = 0.8f;
        item.useAnimation = 22;
        item.useStyle = 1;
        item.noMelee = true; //so the item's animation doesn't do damage
        item.knockBack = 5;
        item.value = 450000;
        item.rare = 5;
        item.UseSound = SoundID.Item46;
        item.shoot = mod.ProjectileType("Snowflake");
        item.shootSpeed = 7f;
    }
    
    public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "CryoBar", 6);
        recipe.AddTile(TileID.IceMachine);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}